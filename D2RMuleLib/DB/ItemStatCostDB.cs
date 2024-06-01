using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2RMuleLib
{
    public class ItemStatCostItem
    {
        public string Name;
        public UInt32 ID;
        public UInt32 Encode;
        public UInt32 SaveBits;
        public UInt32 SaveAdd;
        public UInt32 ParamBits;

        public ItemStatCostItem(string name, UInt32 id, UInt32 encode, UInt32 saveBits, UInt32 saveAdd, UInt32 paramBits)
        {
            Name = name;
            ID = id;
            Encode = encode;
            SaveBits = saveBits;
            SaveAdd = saveAdd;
            ParamBits = paramBits;
        }
    }
    public class ItemStatCostDB
    {
        public Dictionary<UInt32, ItemStatCostItem> DB = new Dictionary<UInt32, ItemStatCostItem>();

        public ItemStatCostDB()
        {
            using (StreamReader reader = new StreamReader("excel/itemstatcost.txt"))
            {
                var line = reader.ReadLine();
                if (line == null) return;

                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    if (line == null) return;

                    string[] values = line.Split('\t');

                    string name = values[0];
                    UInt32 id = UInt32.Parse(values[1]);
                    UInt32 encode = 0;
                    UInt32 saveBits = 0;
                    UInt32 saveAdd = 0;
                    UInt32 paramBits = 0;
                    if (values[14] != "") encode = UInt32.Parse(values[14]);
                    if (values[20] != "") saveBits = UInt32.Parse(values[20]);
                    if (values[21] != "" && values[21] != "-1") saveAdd = UInt32.Parse(values[21]);
                    if (values[22] != "") paramBits = UInt32.Parse(values[22]);

                    // Need to check if this ID is already in there.  Looking at you, passive_mastery_replenish_oncrit
                    if (!DB.ContainsKey(id))
                    {
                        DB.Add(id, new ItemStatCostItem(name, id, encode, saveBits, saveAdd, paramBits));
                    }

                }
            }
        }
    }
}
