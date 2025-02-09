using System.Runtime.InteropServices;

namespace InSim.Packets
{
    public class LapCompletedPacket(ReadOnlySpan<byte> data) : IPacket
    {
        public int ByteSize => 20;
        public PacketType Type => PacketType.LapCompleted;
        public byte RequestId => 0;

        /// <summary>Player Id</summary>
        public byte PlayerId { get; } = data[0]; // PLID

        /// <summary>Lap time in milliseconds</summary>
        public uint LapTimeMs { get; } = MemoryMarshal.Read<uint>(data.Slice(1, 4)); // LTime

        /// <summary>Total time in milliseconds</summary>
        public uint TotalTimeMs { get; } = MemoryMarshal.Read<uint>(data.Slice(5, 4)); // ETime

        /// <summary>Number of laps done</summary>
        public ushort LapsCompleted { get; } = MemoryMarshal.Read<ushort>(data.Slice(9, 2)); // LapsDone;

        //Packet data layout:
        //byte	PLID;		// player's unique id
        //unsigned	LTime;	// lap time (ms)
        //unsigned	ETime;	// total time (ms)
        //word	LapsDone;	// laps completed
        //word	Flags;		// player flags
        //byte	Sp0;
        //byte	Penalty;	// current penalty value (see below)
        //byte	NumStops;	// number of pit stops
        //byte	Fuel200;	// /showfuel yes: double fuel percent / no: 255
    }
}
