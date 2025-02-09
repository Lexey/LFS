using System.Runtime.InteropServices;

namespace InSim.Packets
{
    public class SplitCompletedPacket(ReadOnlySpan<byte> data) : IPacket
    {
        public int ByteSize => 16;
        public PacketType Type => PacketType.SplitCompleted;
        public byte RequestId => 0;

        /// <summary>Player Id</summary>
        public byte PlayerId { get; } = data[0]; // PLID

        /// <summary>Split time in milliseconds</summary>
        public uint SplitTimeMs { get; } = MemoryMarshal.Read<uint>(data.Slice(1, 4)); // STime

        /// <summary>Total time in milliseconds</summary>
        public uint TotalTimeMs { get; } = MemoryMarshal.Read<uint>(data.Slice(5, 4)); // ETime

        /// <summary>Split number</summary>
        public byte Split { get; } = data[9]; // Split

        //Packet data layout:
        //byte	PLID;		// player's unique id
        //unsigned	STime;	// split time (ms)
        //unsigned	ETime;	// total time (ms)
        //byte	Split;		// split number 1, 2, 3
        //byte	Penalty;	// current penalty value (see below)
        //byte	NumStops;	// number of pit stops
        //byte	Fuel200;	// /showfuel yes: double fuel percent / no: 255
    }
}
