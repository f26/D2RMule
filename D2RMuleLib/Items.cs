using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace D2RMuleLib
{
    public class Items
    {
        private BitStreamReader bsr;

        public List<Item> items = new List<Item>();
        public UInt16 magic = 0;
        public string ErrorString = "";

        ItemsType itemsType = ItemsType.Player;

        public enum ItemsType
        {
            Player,
            Corpse,
            Mercenary,
            IronGolem
        }

        public Items(string characterName, ItemsType type = ItemsType.Player)
        {
            itemsType = type;
        }
        public Items(string characterName, BinaryReader binReader, ItemsType type = ItemsType.Player, bool asVault = false)
        {
            itemsType = type;
            Console.WriteLine("Reading " + type.ToString() + " items");
            UInt32 itemCount = 0;

            if (type == ItemsType.Player)
            {
                // d2ce::Items::readItems(), line 21356
                magic = binReader.ReadUInt16();
                if (magic != 0x4d4a)
                {
                    throw new Exception("Unexpected player item magic: " + magic.ToString("x4"));
                }
                if (asVault)
                    itemCount = binReader.ReadUInt32();
                else
                    itemCount = binReader.ReadUInt16();
            }
            else if (type == ItemsType.Corpse)
            {
                // D2CE::Item.cpp::21361
                magic = binReader.ReadUInt16();
                if (magic != 0x4d4a)
                {
                    throw new Exception("Unexpected corpse magic: " + magic.ToString("x4"));
                }

                UInt16 isDead = binReader.ReadUInt16();
                if (isDead == 0)
                {
                    Console.WriteLine("Player is not dead, has no corpse");
                }
                else
                {
                    UInt32 unknown = binReader.ReadUInt32();
                    UInt32 xLoc = binReader.ReadUInt32();
                    UInt32 yLoc = binReader.ReadUInt32();
                    Console.WriteLine("Player is dead, has a corpse at " + xLoc.ToString() + "," + yLoc.ToString());
                }
            }
            else if (type == ItemsType.Mercenary)
            {
                //  d2ce::Items::readItems(), line 21366
                magic = binReader.ReadUInt16(); // jf
                if (magic != 0x666a)
                {
                    throw new Exception("Unexpected merc magic: " + magic.ToString("x4"));
                }

                // Merc info is weird because it always has a merc magic value but then there are
                // only values after it if a merc has been hired
                byte firstMercMagicChar = (byte)binReader.PeekChar();

                if (firstMercMagicChar != 0x4a)
                {
                    // There must not be a merc hired
                    Console.WriteLine("Player does not have a mercenary");
                }
                else
                {
                    // Player must have a mercenary, there should be a magic number and # of items
                    UInt16 magic = binReader.ReadUInt16();
                    if (magic != 0x4d4a)
                    {
                        throw new Exception("Unexpected merc item magic: " + magic.ToString("x4"));
                    }
                    itemCount = binReader.ReadUInt16();
                }
            }
            else if (type == ItemsType.IronGolem)
            {
                // d2ce::Items::readMercItems(), line 21159
                magic = binReader.ReadUInt16();
                if (magic != 0x666b)
                {
                    throw new Exception("Unexpected golem magic: " + magic.ToString("x4"));
                }

                byte hasGolem = binReader.ReadByte();
                if (hasGolem == 0)
                {
                    Console.WriteLine("Player does not have a golem");
                }
                else
                {
                    Console.WriteLine("Player has an iron golem.");
                    itemCount = 1; // only one item on the iron golem
                }
            }

            // Read items

            if (itemCount == 0) return;
            Console.WriteLine("No. of items: " + itemCount.ToString());

            bsr = new BitStreamReader(binReader);
            for (int itemCounter = 0; itemCounter < itemCount; itemCounter++)
            {
                Console.WriteLine("\nItem #: " + itemCounter);
                Item it = new Item(bsr);
                it.CharacterName = characterName;
                it.Location = type.ToString();

                // Set location accordingly
                if (it.Location == "Player")
                {
                    if (it.Parent == Parent.Stored)
                        it.Location = it.Stash.ToString();
                    else
                        it.Location = it.Parent.ToString();
                }
                items.Add(it);
            }
        }

        public void Save(MemoryStream ms, bool asVault = false)
        {
            // Depending on what kind of item list this is, slightly different intro bytes
            using (BinaryWriter writer = new BinaryWriter(ms, Encoding.Default, true))
            {
                // All item lists start with the magic value
                writer.Write(magic);

                switch (itemsType)
                {
                    // Player/corpse include the number of items next
                    case ItemsType.Player:
                    case ItemsType.Corpse:
                        if (asVault) writer.Write((UInt32)items.Count);
                        else writer.Write((UInt16)items.Count);

                        break;

                    // Mercenary item list never includes an item count, the item list just immediately
                    // starts after the mercenary header, but ONLY if a mercenary has been hired and has
                    // items.  The item list starts with a traditional player/corpse magic value and item
                    // count.  If there are no items, nothing follows the merc magic value.
                    case ItemsType.Mercenary:

                        writer.Write((UInt16)0x4d4a);
                        writer.Write((UInt16)items.Count);

                        break;

                    // Golem item list has a single byte to indicate whether or not there is a golem.  It
                    // is followed by a single item if it's set to 1.
                    case ItemsType.IronGolem:
                        if (items.Count > 0)
                            writer.Write((byte)1);
                        else
                            writer.Write((byte)0);
                        break;
                }
            }

            // Now save each item
            foreach (Item i in items)
            {
                i.Save(ms);
            }
        }

        public void Sort()
        {
            items.Sort();
        }
    }
}

