namespace InSim.Client
{
    [Flags]
    public enum InitFlags : ushort
    {
        /// <summary>Guest or single player</summary>
        Local = 4,
        /// <summary>Keep colours in MSO text</summary>
        KeepColors = 8,
        ///// <summary>Receive NLP packets</summary>
        //ReceiveNlp = 16, // Not supported yet
        /// <summary>Receive MultiCarInfo (MCI) packets</summary>
        ReceiveMultiCarInfo = 32,
        ///// <summary>Receive CON packets</summary>
        //ReceiveCon = 64, // Not supported yet
        ///// <summary>Receive OBH packets</summary>
        //ReceiveObh = 128, // Not supported yet
        ///// <summary>Receive HLV packets</summary>
        //ReceiveHlv = 256, // Not supported yet
        ///// <summary>Receive AXM when loading a layout</summary>
        //ReceiveAxmLoad = 512, // Not supported yet
        ///// <summary>Receive AXM when changing objects</summary>
        //ReceiveAxmEdit = 1024, // Not supported yet
        ///// <summary>Process join requests</summary>
        //ProcessJoinRequests = 2048 // Not supported yet
    }
}
