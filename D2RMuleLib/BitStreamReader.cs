using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2RMuleLib
{
    public class BitStreamReader
    {
        public List<byte> rawBytesRead = new List<byte>();
        private Queue<byte> bits = new Queue<byte>();
        private BinaryReader br;
        public UInt32 byteCount = 0;
        public BitStreamReader(BinaryReader binReader)
        {
            br = binReader;
        }

        public void Add(byte value)
        {
            rawBytesRead.Add(value);
        }

        public byte[] GetBytes()
        {
            return rawBytesRead.ToArray();
        }

        public void ClearBits()
        {
            bits.Clear();
            rawBytesRead.Clear();
        }

        public string ReadEarName()
        {
            string name = "";
            while (true)
            {
                char c = (char)ReadBits(7);
                name += c;
                if (c == 0x00)
                    break;
            }

            return name;
        }

        public UInt32 ReadBits(int count)
        {
            if(count == 0) return 0;

            if (count > 32)
                throw new Exception("Too many bits.  BitStreamReader supports a max of 32 bits, you requested " + count.ToString());

            // Read enough bytes so we can read as many bits as requested
            while (bits.Count < count)
                LoadOneByte();

            // Read bits out, LSB first
            UInt32 value = 0;
            for (int bitPos = 0; bitPos < count; bitPos++)
                value = value | (uint)(bits.Dequeue() << bitPos);

            return value;
        }


        public string PeekBitsStr(int count)
        {
            if (count > 32)
                throw new Exception("Too many bits.  BitStreamReader supports a max of 32 bits, you requested " + count.ToString());

            // Read enough bytes so we can read as many bits as requested
            while (bits.Count < count)
                LoadOneByte();

            // Read bits out, LSB first
            Queue<byte> bitsCopy = new Queue<byte>(bits);
            string value = "";
            for (int bitPos = 0; bitPos < count; bitPos++)
            {
                if (bitsCopy.Dequeue() == 1)
                {
                    //value = value + "1";
                    value = "1" + value;
                }
                else
                {
                    //value = value + "0";
                    value = "0" + value;
                }
            }

            return value;
        }

        public List<byte> ReadBitsArray(int count)
        {
            // Read enough bytes so we can read as many bits as requested
            while (bits.Count < count)
                LoadOneByte();

            List<byte> results = new List<byte>();

            for (int i = 0; i < bits.Count; i++)
            {
                results.Add(bits.Dequeue());
            }

            return results;
        }

        public void LoadOneByte()
        {
            byte b = br.ReadByte();
            rawBytesRead.Add(b);

            // Put the bits in, LSB first
            for (int bitPos = 0; bitPos < 8; bitPos++)
            {
                byte mask = Convert.ToByte(0x01 << bitPos);
                byte value = Convert.ToByte((b & mask) >> bitPos);
                bits.Enqueue(value);
            }
        }
    }
}
