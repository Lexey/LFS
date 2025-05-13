using System.Runtime.InteropServices;

namespace InSim.Packets
{
    public class MultiCarInfoPacket : IPacket
    {
        [Flags]
        public enum CarInfoFlags : byte
        {
            Blue = 1, // This car is in the way of a driver who is a lap ahead.
            Yellow = 2, // This car is slow or stopped and in a dangerous place.
            Lag = 32, // This car is lagging (missing or delayed position packets).
            FirstCarInfo = 64, // This is the first car info in the set of MCI packets.
            LastCarInfo = 128 // This is the last car info in the set of MCI packets.
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CarInfo // Car info in 28 bytes - there is an array of these in the MCI (below)
        {
            /// <summary>Current path node</summary>
            ushort PathNodeIndex;
            /// <summary>Current lap</summary>
            ushort Lap;
            /// <summary>Player's unique id</summary>
            byte PlayerId;  
            /// <summary>Current race position: 0 = unknown, 1 = leader, etc</summary>
            byte Position;
            /// <summary>Info flags</summary>
            CarInfoFlags Flags;
            byte Spare;
            /// <summary>X coordinate (65536 = 1 metre)</summary>
            int X;
            /// <summary> Y coordinate (65536 = 1 metre)</summary>
            int	Y;
            /// <summary> Z coordinate (65536 = 1 metre)</summary>
            int Z;
            /// <summary>Speed (32768 = 100 m/s)</summary>
            ushort Speed;
            /// <summary>Car's motion direction if Speed > 0: 0 = world y direction, 32768 = 180 deg anticlockwise</summary>
            ushort Direction;
            /// <summary>Direction of forward axis: 0 = world y direction, 32768 = 180 deg anticlockwise</summary>
            ushort	Heading;
            /// <summary>Rate of change of heading: (16384 = 360 deg/s)</summary>
            short AngularVelocity;
        };

        public int ByteSize { get; private set; } 
        public PacketType Type => PacketType.MultiCarInfo;
        public byte RequestId => 0;

        /// <summary>Car infos</summary>
        public IReadOnlyList<CarInfo> Infos { get; private set; }

        public MultiCarInfoPacket(ReadOnlySpan<byte> data)
        {
            var carsCount = data[0];
            var carInfoSize = Marshal.SizeOf<CarInfo>();
            ByteSize = 4 + carsCount * carInfoSize;
            var infos = new List<CarInfo>();
            for (var i = 0; i < carsCount; ++i)
            {
                var carInfo = MemoryMarshal.Read<CarInfo>(data.Slice(1 + i * carInfoSize, carInfoSize));
                infos.Add(carInfo);
            }
            Infos = infos;
        }

        //Packet data layout:
        // byte	NumC;		// number of valid CompCar structs in this packet
        // CompCar	Info[MCI_MAX_CARS]; // car info for each player, 1 to MCI_MAX_CARS (NumC)
    }
}
