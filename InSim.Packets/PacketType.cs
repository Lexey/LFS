namespace InSim.Packets
{
	/// <summary>
	/// Supported packet types.
	/// </summary>
    public enum PacketType: byte
    {
	    Init = 1, // ISP_ISI, instruction, insim initialise.
	    Tiny = 3, // ISP_TINY, both ways, multi purpose.
	    RaceStarted = 17, // ISP_RST,	info, race start
		LapCompleted = 24, // ISP_LAP, info, lap time
		SplitCompleted = 25, // ISP_SPX, info, split N time
    }
}
