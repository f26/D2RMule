using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2RMuleLib
{
    internal class Attributes
    {
        private BitStreamReader bsr;
        public UInt32 Strength = 0;
        public UInt32 Energy = 0;
        public UInt32 Dexterity = 0;
        public UInt32 Vitality = 0;
        public UInt32 UnusedStats = 0;
        public UInt32 UnusedSkills = 0;
        public UInt32 CurrentHP = 0;
        public UInt32 MaxHP = 0;
        public UInt32 CurrentMana = 0;
        public UInt32 MaxMana = 0;
        public UInt32 CurrentStamina = 0;
        public UInt32 MaxStamina = 0;
        public UInt32 Level = 0;
        public UInt32 Experience = 0;
        public UInt32 Gold = 0;
        public UInt32 StashedGold = 0;

        public Attributes(BinaryReader binReader)
        {
            bsr = new BitStreamReader(binReader);

            // Verify header
            UInt16 magic = binReader.ReadUInt16();
            if (magic != 0x6667)
            {
                throw new Exception("Unexpected attribute magic: " + magic.ToString("x4"));
            }
            bsr.Add(0x67);
            bsr.Add(0x66);

            // Read each field
            bool done = false;
            while (!done)
            {
                UInt32 id = bsr.ReadBits(9);
                switch (id)
                {
                    case 0: this.Strength = bsr.ReadBits(10); break;
                    case 1: this.Energy = bsr.ReadBits(10); break;
                    case 2: this.Dexterity = bsr.ReadBits(10); break;
                    case 3: this.Vitality = bsr.ReadBits(10); break;
                    case 4: this.UnusedStats = bsr.ReadBits(10); break;
                    case 5: this.UnusedSkills = bsr.ReadBits(8); break;
                    case 6: this.CurrentHP = bsr.ReadBits(21); break;
                    case 7: this.MaxHP = bsr.ReadBits(21); break;
                    case 8: this.CurrentMana = bsr.ReadBits(21); break;
                    case 9: this.MaxMana = bsr.ReadBits(21); break;
                    case 10: this.CurrentStamina = bsr.ReadBits(21); break;
                    case 11: this.MaxStamina = bsr.ReadBits(21); break;
                    case 12: this.Level = bsr.ReadBits(7); break;
                    case 13: this.Experience = bsr.ReadBits(32); break;
                    case 14: this.Gold = bsr.ReadBits(25); break;
                    case 15: this.StashedGold = bsr.ReadBits(25); break;
                    case 0x1ff: done = true; break;
                    default:
                        throw new Exception("Unknown attribute ID encountered: " + id.ToString("x2"));
                }
            }

            // DEBUG
            //Console.WriteLine("Strength: " + this.Strength);
            //Console.WriteLine("Energy: " + this.Energy);
            //Console.WriteLine("Dexterity: " + this.Dexterity);
            //Console.WriteLine("Vitality: " + this.Vitality);
            //Console.WriteLine("UnusedStats: " + this.UnusedStats);
            //Console.WriteLine("UnusedSkills: " + this.UnusedSkills);
            //Console.WriteLine("CurrentHP: " + this.CurrentHP);
            //Console.WriteLine("MaxHP: " + this.MaxHP);
            //Console.WriteLine("CurrentMana: " + this.CurrentMana);
            //Console.WriteLine("MaxMana: " + this.MaxMana);
            //Console.WriteLine("CurrentStamina: " + this.CurrentStamina);
            //Console.WriteLine("MaxStamina: " + this.MaxStamina);
            //Console.WriteLine("Level: " + this.Level);
            //Console.WriteLine("Experience: " + this.Experience);
            //Console.WriteLine("Gold: " + this.Gold);
            //Console.WriteLine("StashedGold: " + this.StashedGold);
        }

    }
}
