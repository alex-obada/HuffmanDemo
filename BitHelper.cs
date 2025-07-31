using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuffmanDemo
{
    internal static class BitHelper
    {
        public static void WriteBoolList(BinaryWriter writer, List<bool> bits)
        {
            writer.Write(bits.Count);

            int byteCount = (bits.Count + 7) / 8;
            for (int i = 0; i < byteCount; i++)
            {
                byte b = 0;
                for (int bit = 0; bit < 8; bit++)
                {
                    int index = i * 8 + bit;
                    if (index >= bits.Count)
                        break;
                    if (bits[index])
                        b |= (byte)(1 << bit);
                }
                writer.Write(b);
            }
        }

        public static List<bool> ReadBoolList(BinaryReader reader)
        {
            int count = reader.ReadInt32();
            var result = new List<bool>(count);

            int byteCount = (count + 7) / 8;
            for (int i = 0; i < byteCount; i++)
            {
                byte b = reader.ReadByte();
                for (int bit = 0; bit < 8 && result.Count < count; bit++)
                    result.Add((b & (1 << bit)) != 0);
            }

            return result;
        }
    }
}
