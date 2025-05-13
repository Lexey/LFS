namespace InSim.Client
{
    public class Parameters(string host, ushort port,  ProtocolVersion protocolVersion)
    {
        /// <summary>Host to connect to</summary>
        public string Host { get; } = host;
        /// <summary>Port to connect to</summary>
        public ushort Port { get; } = port;
        /// <summary>Protocol version</summary>
        public ProtocolVersion ProtocolVersion { get; } = protocolVersion;

        /// <summary>
        /// Client name.
        /// Should be an ASCII string up to 16 characters.
        /// </summary>
        public string Name { get; set; } = "InSim.Lite";

        /// <summary>
        /// Admin password.
        /// Should be an ASCII string up to 16 characters. Default is empty string.
        /// </summary>
        public string AdminPassword { get; set; } = string.Empty;

        /// <summary>Init flags</summary>
        public InitFlags InitFlags { get; set; }

        /// <summary>
        /// Time interval between car info (NLP or MCI) packets in ms.
        /// </summary>
        public ushort CarInfoIntervalMillis { get; set; }
    }
}
