using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace D2RMuleLib
{
    public class ItemInfo
    {
        public string Name;
        public string Code;
        public string Type;
        public UInt32 Width;
        public UInt32 Height;
        public bool IsArmor = false;
        public bool IsWeapon = false;
        public bool IsMisc = false;
        public bool IsStackable = false;
        public bool IsQuest = false;
        public string AlernateGfx = "";

        public ItemInfo(string name, string type, string code, UInt32 width, UInt32 height)
        {
            this.Name = name;
            this.Type = type;
            this.Code = code;
            this.Width = width;
            this.Height = height;
        }
    }
    public class D2DB
    {
        public Dictionary<string, ItemInfo> itemDB = new Dictionary<string, ItemInfo>();
        public Dictionary<UInt32, string> RunewordDB = new Dictionary<UInt32, string>();
        public Dictionary<UInt32, string> UniqueDB = new Dictionary<UInt32, string>();
        public ItemStatCostDB idb = new ItemStatCostDB();
        public Dictionary<string, char> huffmanDecodeMap = new Dictionary<string, char>();
        public Dictionary<UInt32, string> prefixDB = new Dictionary<uint, string>();
        public Dictionary<UInt32, string> suffixDB = new Dictionary<uint, string>();
        public Dictionary<UInt32, string> setDB = new Dictionary<uint, string>();
        public Dictionary<UInt32, string> rareAffixDB = new Dictionary<uint, string>();
        public Dictionary<string, List<string>> propertiesDB = new Dictionary<string, List<string>>();
        public Dictionary<UInt32, string> skillDB = new Dictionary<uint, string>();

        private static D2DB instance;

        public static D2DB Instance()
        {
            if (instance == null)
            {
                instance = new D2DB();
            }
            return instance;
        }

        private D2DB()
        {
            InitializeHuffmanDecodeMap();
            using (StreamReader reader = new StreamReader("excel/misc.txt"))
            {
                reader.ReadLine(); // Throw away header
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line == null) return;
                    string[] values = line.Split('\t');
                    if (values[0] == "Expansion") continue; // Expansion demarcator, ignore
                    ItemInfo i = new ItemInfo(values[0], values[31], values[14], Convert.ToUInt32(values[18]), Convert.ToUInt32(values[19]));
                    i.IsMisc = true;
                    if (values[42] != null && values[42] == "1") i.IsStackable = true;
                    if (values[46] != "") i.IsQuest = true;
                    itemDB.Add(values[14], i);
                }
            }
            using (StreamReader reader = new StreamReader("excel/armor.txt"))
            {
                reader.ReadLine(); // Throw away header
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line == null) return;
                    string[] values = line.Split('\t');
                    if (values[0] == "Expansion") continue; // Expansion demarcator, ignore
                    ItemInfo i = new ItemInfo(values[0], values[51], values[18], Convert.ToUInt32(values[27]), Convert.ToUInt32(values[28]));
                    i.IsArmor = true;
                    i.AlernateGfx = values[22];
                    itemDB.Add(values[18], i);
                }
            }
            using (StreamReader reader = new StreamReader("excel/weapons.txt"))
            {
                reader.ReadLine(); // Throw away header
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line == null) return;
                    string[] values = line.Split('\t');
                    if (values[0] == "Expansion") continue; // Expansion demarcator, ignore
                    ItemInfo i = new ItemInfo(values[0], values[1], values[3], Convert.ToUInt32(values[44]), Convert.ToUInt32(values[45]));
                    i.IsWeapon = true;
                    if (values[46] != null && values[46] == "1") i.IsStackable = true;
                    if (values[68] != "") i.IsQuest = true;
                    i.AlernateGfx = values[4];
                    itemDB.Add(values[3], i);
                }
            }
            using (StreamReader reader = new StreamReader("excel/runes.txt"))
            {
                // The names of runewords can be found in runes.txt.  each name is associated
                // with a runeword ID (the first column, format is "Runeword#").  For some reason,
                // the ID does not match up with what's in the file.  The offset to go from what's
                // in runes.txt to what will be found in the item itself is as follows:
                // Up to Runeword75, runeword ID from item will be +26 higher than from runes.txt
                // From Runeword81, runeword ID from item will be +25 higher than from runes.txt.
                // Why? Who knows! This info derived experimentally using D2CE.
                reader.ReadLine(); // Throw away header
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line == null) return;
                    string[] values = line.Split('\t');
                    if (values[2] != "1") continue; // "complete" column must be 1 for it to be a valid runeword
                    UInt32 id = Convert.ToUInt32(values[0].Replace("Runeword", ""));
                    if (id <= 75) id += 26;
                    else id += 25;
                    RunewordDB.Add(id, values[1]);
                }
            }
            using (StreamReader reader = new StreamReader("excel/uniqueitems.txt"))
            {
                reader.ReadLine(); // Throw away header
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line == null) return;
                    string[] values = line.Split('\t');
                    if (values[1] == "") continue; // Some rows are headers and should be skipped if empty
                    UInt32 id = Convert.ToUInt32(values[1]);
                    UniqueDB.Add(id, values[0]);
                }
            }

            using (StreamReader reader = new StreamReader("excel/magicprefix.txt"))
            {
                reader.ReadLine(); // Throw away header
                UInt32 index = 1;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line == null) return;
                    string[] values = line.Split('\t');
                    if (values[0] == "Expansion") continue; // Expansion demarcator, ignore
                    string affix = values[0];
                    prefixDB.Add(index, affix);
                    index++;
                }
            }
            using (StreamReader reader = new StreamReader("excel/magicsuffix.txt"))
            {
                reader.ReadLine(); // Throw away header
                UInt32 index = 1;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line == null) return;
                    string[] values = line.Split('\t');
                    if (values[0] == "Expansion") continue; // Expansion demarcator, ignore
                    string affix = values[0];
                    suffixDB.Add(index, affix);
                    index++;
                }
            }
            using (StreamReader reader = new StreamReader("excel/setitems.txt"))
            {
                reader.ReadLine(); // Throw away header
                UInt32 index = 1;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line == null) return;
                    string[] values = line.Split('\t');
                    if (values[0] == "Expansion") continue; // Expansion demarcator, ignore
                    string affix = values[0];
                    setDB.Add(index, affix);
                    index++;
                }
            }

            // Rare/crafted affixes are funny:  read in each file, ignoring header.  First read suffix, then
            // prefix.  Add each affix to a list, incrementing its ID (starting with 1).  Final IDs will
            // match the rare/crafted affix bits.  Strange.
            using (StreamReader reader = new StreamReader("excel/raresuffix.txt"))
            {
                reader.ReadLine(); // Throw away header
                UInt32 index = 1;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line == null) return;
                    rareAffixDB.Add((uint)(rareAffixDB.Count) + 1, line.Split('\t')[0]);
                    index++;
                }
            }
            using (StreamReader reader = new StreamReader("excel/rareprefix.txt"))
            {
                reader.ReadLine(); // Throw away header
                UInt32 index = 1;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line == null) return;
                    rareAffixDB.Add((uint)(rareAffixDB.Count) + 1, line.Split('\t')[0]);
                    index++;
                }
            }

            using (StreamReader reader = new StreamReader("excel/properties.txt"))
            {
                reader.ReadLine(); // Throw away header
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line == null) return;
                    string[] values = line.Split('\t');
                    string code = values[0];
                    string name = values[3];
                    string tooltip = values[30];

                    if (name == "") continue;
                    if (tooltip == "") continue;

                    if (propertiesDB.ContainsKey(name))
                    {
                        if (!propertiesDB[name].Contains(tooltip))
                            propertiesDB[name].Add(tooltip);
                    }
                    else
                    {
                        propertiesDB[name] = new List<string> { tooltip };
                    }
                }

                // Some of these don't have a name in properties.txt, its blank?!  

                propertiesDB["item_maxdamage_percent"] = new List<string> { "+#% Enhanced Damage" };
                propertiesDB["mindamage"] = new List<string> { "+# to Minimum Damage" };
                propertiesDB["maxdamage"] = new List<string> { "+# to Maximum Damage" };
                propertiesDB["item_indesctructible"] = new List<string> { "Indestructible" };


                propertiesDB["item_pierce_fire"] = new List<string> { "-#% to Enemy Fire Resistance" };
                propertiesDB["item_pierce_ltng"] = new List<string> { "-#% to Enemy Lightning Resistance" };
                propertiesDB["item_pierce_cold"] = new List<string> { "-#% to Enemy Cold Resistance" };
                propertiesDB["item_pierce_pois"] = new List<string> { "-#% to Enemy Poison Resistance" };

            }
            using (StreamReader reader = new StreamReader("excel/skills.txt"))
            {
                reader.ReadLine(); // Throw away header
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line == null) return;
                    string[] values = line.Split('\t');
                    UInt32 skillID = Convert.ToUInt32(values[1]);
                    string skillTxt = values[0];
                    if (skillTxt == "DiabWall") skillTxt = "Firestorm";
                    skillDB.Add(skillID, skillTxt);
                }
            }

            // TODO: Add more txt files here
        }

        private void InitializeHuffmanDecodeMap()
        {
            // Huffman encoding map for decoding the huffman-encoded item type code
            huffmanDecodeMap.Add("111101000", '\0');
            huffmanDecodeMap.Add("01", ' ');
            huffmanDecodeMap.Add("11011111", '0');
            huffmanDecodeMap.Add("0011111", '1');
            huffmanDecodeMap.Add("001100", '2');
            huffmanDecodeMap.Add("1011011", '3');
            huffmanDecodeMap.Add("01011111", '4');
            huffmanDecodeMap.Add("01101000", '5');
            huffmanDecodeMap.Add("1111011", '6');
            huffmanDecodeMap.Add("11110", '7');
            huffmanDecodeMap.Add("001000", '8');
            huffmanDecodeMap.Add("01110", '9');
            huffmanDecodeMap.Add("01111", 'a');
            huffmanDecodeMap.Add("1010", 'b');
            huffmanDecodeMap.Add("00010", 'c');
            huffmanDecodeMap.Add("100011", 'd');
            huffmanDecodeMap.Add("000011", 'e');
            huffmanDecodeMap.Add("110010", 'f');
            huffmanDecodeMap.Add("01011", 'g');
            huffmanDecodeMap.Add("11000", 'h');
            huffmanDecodeMap.Add("0111111", 'i');
            huffmanDecodeMap.Add("011101000", 'j');
            huffmanDecodeMap.Add("010010", 'k');
            huffmanDecodeMap.Add("10111", 'l');
            huffmanDecodeMap.Add("10110", 'm');
            huffmanDecodeMap.Add("101100", 'n');
            huffmanDecodeMap.Add("1111111", 'o');
            huffmanDecodeMap.Add("10011", 'p');
            huffmanDecodeMap.Add("10011011", 'q');
            huffmanDecodeMap.Add("00111", 'r');
            huffmanDecodeMap.Add("0100", 's');
            huffmanDecodeMap.Add("00110", 't');
            huffmanDecodeMap.Add("10000", 'u');
            huffmanDecodeMap.Add("0111011", 'v');
            huffmanDecodeMap.Add("00000", 'w');
            huffmanDecodeMap.Add("11100", 'x');
            huffmanDecodeMap.Add("0101000", 'y');
            huffmanDecodeMap.Add("00011011", 'z');
        }
    }
}
