using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2RMuleLib
{
    public class BitBuffer
    {
        List<int> bits = new List<int>();

        public BitBuffer() { }
        public BitBuffer(byte[] rawBytes)
        {
            foreach (byte b in rawBytes)
            {
                Add(b);
            }
        }

        public void Add(byte b)
        {
            // Add all the bits, LSB first
            for (int i = 0; i < 8; i++)
                this.bits.Add((b >> i) & 0x1);
        }

        public void SetBits(int offset, int length, UInt64 value)
        {
            // Starting at the provided offset, set the appropriate amount of bits
            int bitCounter = 0;
            for (int i = offset; i < offset + length; i++)
            {
                int bit = (int)(value >> bitCounter) & 0x1;
                bits[i] = bit;
                bitCounter++;
            }
        }

        public byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();

            // Make a copy of the bits because we don't want to modify them
            List<int> bitsCopy = new List<int>(this.bits);

            // Convert each 8 bits back into a single byte
            int thisByte = 0;
            while (bitsCopy.Count > 0)
            {
                // Remember bits were placed in LSB first, so read them out that way
                for (int i = 0; i < 8; i++)
                {
                    thisByte = thisByte | (bitsCopy[0] << i);
                    bitsCopy.RemoveAt(0);
                    if (i == 7)
                    {
                        bytes.Add((byte)thisByte);
                        thisByte = 0;
                    }
                }
            }

            return bytes.ToArray();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (byte b in this.ToBytes())
            {
                sb.Append(b.ToString("x2"));
                sb.Append(": ");
                sb.AppendLine(b.ToString("b8"));
            }

            return sb.ToString();
        }
    }
}
