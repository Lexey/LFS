using CodeJam;

namespace InSim.Client
{
    public static class Utils
    {
        public static void WriteFixedString(this BinaryWriter writer, byte[] value, int fixedSize)
        {
            Code.AssertArgument(value.Length <= fixedSize, nameof(value), "Fixed sting length exceeds {0} bytes", fixedSize);
            writer.Write(value);
            for (var i = value.Length; i < fixedSize; ++i)
            {
                writer.Write((byte)0);
            }
        }
    }
}
