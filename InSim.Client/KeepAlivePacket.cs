using InSim.Packets;

namespace InSim.Client
{
    public class KeepAlivePacket : IOutputPacket
    {
        public int ByteSize => 4;
        public PacketType Type => PacketType.Tiny;
        public byte RequestId => 0;

        public void WriteData(BinaryWriter writer)
        {
            writer.Write((byte)0); // TINY_NONE subtype.
        }
    }
}
