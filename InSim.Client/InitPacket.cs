using System.Text;
using CodeJam;
using InSim.Packets;

namespace InSim.Client
{
    public class InitPacket : IOutputPacket
    {
        private const int FixedStringSize = 16;
        private byte[] clientName_ = [];
        private byte[] adminPassword_ = [];

        public int ByteSize => 44;
        public PacketType Type => PacketType.Init;
        public byte RequestId => 0; // If non-zero, LFS will reply with IS_VER packet.

        /// <summary>
        /// InSim protocol version.
        /// </summary>
        public ProtocolVersion ProtocolVersion { get; set; }

        /// <summary>
        /// Client program name.
        /// </summary>
        public string ClientName
        {
            get => Encoding.ASCII.GetString(clientName_);

            set
            {
                var newValue = Encoding.ASCII.GetBytes(value);
                Code.AssertArgument(newValue.Length <= FixedStringSize, nameof(value), $"Client name length cannot exceed {FixedStringSize} bytes");
                clientName_ = newValue;
            }
        }

        /// <summary>
        /// LFS admin password.
        /// </summary>
        public string AdminPassword
        {
            get => Encoding.ASCII.GetString(adminPassword_);

            set
            {
                var newValue = Encoding.ASCII.GetBytes(value);
                Code.AssertArgument(newValue.Length <= FixedStringSize, nameof(value), $"Admin password length cannot exceed {FixedStringSize} bytes");
                adminPassword_ = newValue;
            }
        }

        /// <summary>Init flags.</summary>
        public InitFlags Flags
        {
            get;
            set;
        }

        /// <summary>
        /// Time interval between car info (NLP or MCI) packets in ms.
        /// </summary>
        public ushort CarInfoIntervalMillis
        {
            get;
            set;
        }

        public void WriteData(BinaryWriter writer)
        {
            writer.Write((byte)0); // Zero byte
            writer.Write((ushort)0); // UDPPort = 0 (not used)
            writer.Write((ushort)Flags);
            writer.Write((byte)ProtocolVersion); // InSimVer
            writer.Write((byte)0); // Prefix = 0 (not used)
            writer.Write(CarInfoIntervalMillis);
            writer.WriteFixedString(adminPassword_, FixedStringSize);
            writer.WriteFixedString(clientName_, FixedStringSize);
        }
    }
}
