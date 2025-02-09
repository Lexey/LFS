using System.Text;
using InSim.Packets;

namespace InSim.Client
{
    /// <summary>Converts packet to bytes.</summary>
    public class PacketWriter(SizeUnit sizeUnit)
    {
        public byte[] ToBytes(IOutputPacket packet)
        {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms, Encoding.ASCII);
            writer.Write(GetPacketSize(packet));
            writer.Write((byte)packet.Type);
            writer.Write(packet.RequestId);
            packet.WriteData(writer);
            return ms.ToArray();
        }

        private byte GetPacketSize(IOutputPacket packet)
        {
            return (byte)(sizeUnit == SizeUnit.Byte ? packet.ByteSize : packet.ByteSize / 4);
        }
    }
}
