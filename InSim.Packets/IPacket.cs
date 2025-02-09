namespace InSim.Packets
{
    public interface IPacket
    {
        /// <summary>
        /// Packet size in bytes.
        /// </summary>
        public int ByteSize { get; }
        /// <summary>
        /// Packet type.
        /// </summary>
        public PacketType Type { get; }

        /// <summary>
        /// Request id for request packets or replies.
        /// </summary>
        public byte RequestId { get; }
    }
}
