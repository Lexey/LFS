using System.Text;

namespace InSim.Packets
{
    public class RaceStartedPacket(byte requestId, ReadOnlySpan<byte> data) : IPacket
    {
        public int ByteSize => 28;
        public PacketType Type => PacketType.RaceStarted;

        public byte RequestId { get; } = requestId;

        /// <summary>Number of race laps. 0 for qualification</summary>
        public byte RaceLaps { get; } = data[1];

        public byte PlayersNumber { get; } = data[3];
        public string TrackName { get; } = Encoding.ASCII.GetString(data.Slice(5, 6));

        // Packet data layout:
        //byte	Zero;
        //byte	RaceLaps;	// 0 if qualifying
        //byte	QualMins;	// 0 if race
        //byte	NumP;		// number of players in race
        //byte	Timing;		// lap timing (see below)
        //char	Track[6];	// short track name
        //byte	Weather;
        //byte	Wind;
        //word	Flags;		// race flags (must pit, can reset, etc - see below)
        //word	NumNodes;	// total number of nodes in the path
        //word	Finish;		// node index - finish line
        //word	Split1;		// node index - split 1
        //word	Split2;		// node index - split 2
        //word	Split3;		// node index - split 3
    }
}
