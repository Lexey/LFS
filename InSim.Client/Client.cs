using System;
using System.Net.Sockets;
using CodeJam;
using InSim.Packets;

namespace InSim.Client
{
    public class Client : IDisposable
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly CancellationToken cancellationToken_;
        private readonly Parameters parameters_;
        private readonly PacketWriter packetWriter_;
        private readonly Task runTask_;
        private bool disposed_;
        private TcpClient tcpClient_ = new();

        /// <summary>Race start packet received</summary>
        public event Action<RaceStartedPacket>? RaceStartedPacketReceived;
        /// <summary>Split completed packet received</summary>
        public event Action<SplitCompletedPacket>? SplitCompletedPacketReceived;
        /// <summary>Lap completed packet received</summary>
        public event Action<LapCompletedPacket>? LapCompletedPacketReceived;

        private const int PacketSizeOffset = 0;
        private const int PacketTypeOffset = 1;
        private const int RequestIdOffset = 2;
        private const int PacketDataOffset = 3;


        public Client(Parameters parameters, CancellationToken cancellationToken)
        {
            parameters_ = parameters;
            cancellationToken_ = cancellationToken;
            packetWriter_ =
                new PacketWriter(parameters_.ProtocolVersion == ProtocolVersion.V8 ? SizeUnit.Byte : SizeUnit.UInt);
            runTask_ = Run();
        }


        public void Dispose()
        {
            lock (packetWriter_)
            {
                if (disposed_)
                {
                    return;
                }
                disposed_  = true;
                tcpClient_.Dispose();
            }
            // TODO:
            runTask_.Wait(CancellationToken.None);
            runTask_.Dispose();
        }

        private async Task Run()
        {
            for (;;)
            {
                try
                {
                    lock (packetWriter_)
                    {
                        if (disposed_)
                        {
                            return;
                        }

                        tcpClient_?.Dispose();
                        tcpClient_ = new TcpClient();
                    }

                    Logger.Debug("Connecting to InSim at {0}:{1}", parameters_.Host, parameters_.Port);
                    await tcpClient_.ConnectAsync(parameters_.Host, parameters_.Port, cancellationToken_)
                        .ConfigureAwait(false);
                    await SendInitPacket();
                    
                    var buffer = new byte[2048]; // Max packet size = 1024. Should be sufficient to fit at least 2 packets.
                    var bytesInBuffer = 0; // total number of bytes read to the buffer
                    for (;;)
                    {
                        bytesInBuffer = await ReadNextPacketOrMore(buffer, bytesInBuffer);
                        var packetSize = GetPacketByteSize(buffer);
                        Code.AssertState(packetSize <= bytesInBuffer, "Insufficient data has been read");
                        if (IsKeepAlive(buffer))
                        {
                            await SendKeepAlive();
                        }
                        else
                        {
                            ProcessPacket(new ReadOnlySpan<byte>(buffer, 0, packetSize));
                        }
                        bytesInBuffer -= packetSize;
                        if (bytesInBuffer > 0)
                        {
                            Array.Copy(buffer, packetSize, buffer, 0, bytesInBuffer);
                        }
                    }
                }
                catch (OperationCanceledException e)
                {
                    Logger.Debug(e, "Cancelled. Leaving...");
                    return;
                }
                catch (Exception e)
                {
                    lock (packetWriter_)
                    {
                        if (disposed_)
                        {
                            Logger.Debug(e, "Disposed. Leaving...");
                            return;
                        }
                    }

                    Logger.Error(e, "Unexpected exception");
                }
            }
        }

        private async ValueTask SendInitPacket()
        {
            var initPacket = new InitPacket
            {
                ProtocolVersion = parameters_.ProtocolVersion,
                ClientName = parameters_.Name,
                AdminPassword = parameters_.AdminPassword
            };
            var initBytes = packetWriter_.ToBytes(initPacket);
            await tcpClient_.GetStream().WriteAsync(initBytes, cancellationToken_).ConfigureAwait(false);
        }

        private async ValueTask<int> ReadNextPacketOrMore(byte[] buffer, int bytesInBuffer)
        {
            const int minPacketSize = 4;
            bytesInBuffer = await ReadBytesIfNeeded(buffer, bytesInBuffer, minPacketSize);

            var packetSize = GetPacketByteSize(buffer);
            bytesInBuffer = await ReadBytesIfNeeded(buffer, bytesInBuffer, packetSize);
            return bytesInBuffer;
        }

        private async ValueTask<int> ReadBytesIfNeeded(byte[] buffer, int bytesInBuffer, int bytesNeeded)
        {
            while (bytesInBuffer < bytesNeeded)
            {
                var read = await tcpClient_.GetStream().ReadAsync(buffer, bytesInBuffer, bytesNeeded - bytesInBuffer, cancellationToken_).ConfigureAwait(false);
                Code.AssertState(read > 0, "TCP stream has been closed");
                bytesInBuffer += read;
            }
            return bytesInBuffer;
        }

        private int GetPacketByteSize(byte[] buffer)
        {
            var unitSize = buffer[PacketSizeOffset];
            return parameters_.ProtocolVersion == ProtocolVersion.V8 ? unitSize : unitSize * 4;
        }

        private static bool IsKeepAlive(byte[] buffer)
        {
            return buffer[PacketTypeOffset] == (byte)PacketType.Tiny && buffer[PacketDataOffset] == 0;
        }

        private async ValueTask SendKeepAlive()
        {
            var keepAlivePacket = new KeepAlivePacket();
            await tcpClient_.GetStream().WriteAsync(packetWriter_.ToBytes(keepAlivePacket), cancellationToken_)
                .ConfigureAwait(false);
        }

        private void ProcessPacket(ReadOnlySpan<byte> packet)
        {
            var packetType = packet[PacketTypeOffset];
            switch (packetType)
            {
                case (byte)PacketType.Tiny:
                    return; // do not process
                case (byte)PacketType.RaceStarted:
                    ProcessRaceStarted(packet);
                    return;
                case (byte)PacketType.SplitCompleted:
                    ProcessSplitCompleted(packet);
                    return;
                case (byte)PacketType.LapCompleted:
                    ProcessLapCompleted(packet);
                    return;
            }
            Logger.Trace("Received packet of type {0}. Ignored.", packetType);
        }

        private void ProcessRaceStarted(ReadOnlySpan<byte> packet)
        {
            Logger.Debug("Received race started packet");
            RaceStartedPacketReceived?.Invoke(new RaceStartedPacket(packet[RequestIdOffset], packet[PacketDataOffset..]));
        }

        private void ProcessSplitCompleted(ReadOnlySpan<byte> packet)
        {
            Logger.Debug("Received split completed packet");
            SplitCompletedPacketReceived?.Invoke(new SplitCompletedPacket(packet[PacketDataOffset..]));
        }

        private void ProcessLapCompleted(ReadOnlySpan<byte> packet)
        {
            Logger.Debug("Received lap completed packet");
            LapCompletedPacketReceived?.Invoke(new LapCompletedPacket(packet[PacketDataOffset..]));
        }
    }
}
