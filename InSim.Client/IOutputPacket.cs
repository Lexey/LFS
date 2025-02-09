using InSim.Packets;

namespace InSim.Client
{
    public interface IOutputPacket : IPacket
    {
        /// <summary>Writes the packet data (except a standard header) to the given writer.</summary>
        /// <param name="writer">The binary writer.</param>
        public void WriteData(BinaryWriter writer);
    }
}
