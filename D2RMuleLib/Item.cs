using D2RMuleLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows.Markup;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace D2RMuleLib
{
    public enum Quality
    {
        Inferior = 1,
        Normal = 2,
        Superior = 3,
        Magic = 4,
        Set = 5,
        Rare = 6,
        Unique = 7,
        Crafted = 8,
        Tempered = 9
    }

    public enum Parent
    {
        Stored = 0,
        Equipped = 1,
        Belt = 2,
        Cursor = 4,
        Item = 6
    }

    public enum Stash
    {
        None = 0,
        Inventory = 1,
        HoradricCube = 4,
        Stash = 5
    }

    public enum Inferior
    {
        Crude = 0,
        Cracked = 1,
        Damaged = 2,
        LowQuality = 3
    }

    public class Item : ICloneable, IComparable<Item>
    {
        public int CompareTo(Item that)
        {
            return this.Type.CompareTo(that.Type);
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
        public bool Modified = false;
        public bool IsIdentified { get; private set; } = false;
        public bool IsSocketed { get; private set; } = false;
        public bool IsEar { get; private set; } = false;
        public bool IsStartingItem { get; private set; } = false;
        public bool IsSimple { get; private set; } = false;
        public bool IsEthereal { get; private set; } = false;
        public bool IsPersonalized { get; private set; } = false;
        public bool IsRuneword { get; private set; } = false;
        public UInt32 NumSocketedItems { get; private set; } = 0;

        public UInt32 Level = 0;
        public Quality Quality { get; private set; } = Quality.Normal;
        public UInt32 Sockets = 0;

        public string DisplayName { get; private set; } = "";

        // Extended item properties
        private Parent _parent = Parent.Stored;
        public Parent Parent
        {
            get
            {
                return this._parent;
            }
            set
            {
                this._parent = value;
                this.Modified = true;
            }
        }

        private Stash _stash = Stash.None;
        public Stash Stash
        {
            get
            {
                return this._stash;
            }
            set
            {
                this._stash = value;
                this.Modified = true;
            }
        }
        public string CharacterName { get; set; } = "";
        public string Location { get; set; } = "Player";
        public string SocketedItems { get; private set; } = "";

        public string TypeCode = "";
        public string Type = "";

        public string Mods { get; private set; } = "";

        public Size Size { get; set; } = new Size(1, 1);

        private Point _position = new Point(0, 0);
        public Point Position
        {
            get
            {
                return this._position;
            }
            set
            {
                this.Modified = true;
                this._position = value;
            }
        }

        public List<Item> socketedItems = new List<Item>();
        public byte[] rawBytes = { };

        public bool HasCustomGraphics { get; private set; } = false;
        public UInt32 CustomGraphicsIndex { get; private set; } = 0;

        public Item() { }
        public Item(BitStreamReader bsr)
        {
            // All items start with two bytes with several bitfields:

            // d2ce::Item::readItem(), line 13088
            bsr.ReadBits(4); // unknown
            this.IsIdentified = bsr.ReadBits(1) == 1;
            bsr.ReadBits(6); // unknown
            this.IsSocketed = bsr.ReadBits(1) == 1;
            bsr.ReadBits(1); // unknown
            bsr.ReadBits(1); // is new
            bsr.ReadBits(2); // unknown
            IsEar = bsr.ReadBits(1) == 1;
            IsStartingItem = bsr.ReadBits(1) == 1;
            bsr.ReadBits(3); // unknown
            this.IsSimple = bsr.ReadBits(1) == 1;
            this.IsEthereal = bsr.ReadBits(1) == 1;
            bsr.ReadBits(1); // unknown
            this.IsPersonalized = bsr.ReadBits(1) == 1;
            bsr.ReadBits(1); // unknown
            this.IsRuneword = bsr.ReadBits(1) == 1;
            bsr.ReadBits(5); // unknown
#if DEBUG

            Console.Write("Properties:");
            if (IsIdentified) Console.Write(" identified");
            if (IsSocketed) Console.Write(" socketed");
            if (IsEar) Console.Write(" ear");
            if (IsStartingItem) Console.Write(" startingItem");
            if (IsSimple) Console.Write(" simple");
            if (IsEthereal) Console.Write(" ethereal");
            if (IsPersonalized) Console.Write(" personalized");
            if (IsRuneword) Console.Write(" runeword");
            Console.WriteLine("");
#endif
            // At this point we've read 4 bytes (32 bits)

            if (IsEar)
                ProcessEarItem(bsr);
            else
                ProcessItem(bsr);
        }

        void ApplyPositionUpdate()
        {
            BitBuffer bb = new BitBuffer(this.rawBytes);
            bb.SetBits(35, 3, (UInt32)this.Parent);
            bb.SetBits(42, 4, (UInt32)this.Position.X);
            bb.SetBits(46, 4, (UInt32)this.Position.Y);
            if (this.Parent == Parent.Stored)
                bb.SetBits(50, 3, (UInt32)this.Stash);
            else
                bb.SetBits(50, 3, (UInt32)Stash.None);
            this.rawBytes = bb.ToBytes();
            return;


            // Write the parent (3 bits at bit offset 35)
            WriteBitfield((UInt32)this.Parent, 3, 35);

            // Set row (bit offset 42)
            WriteBitfield((UInt32)this.Position.X, 4, 42);

            // Set column (bit offset 46)
            WriteBitfield((UInt32)this.Position.Y, 4, 46);

            // Set the stash bit (bit offset 50)
            if (this.Parent == Parent.Stored)
            {
                WriteBitfield((UInt32)this.Stash, 3, 50);
            }
            else
            {
                WriteBitfield((UInt32)Stash.None, 3, 50);
            }
        }

        void WriteBitfield(UInt32 value, UInt32 numBits, int offset)
        {
            // Figure out which byte(s) this bitfield spans
            int startByteIndex = offset / 8;
            int bitsRemainingInFirstByte = 8 - (offset % 8);
            int totalAffectedBytes = 1;
            if (numBits > bitsRemainingInFirstByte)
            {
                totalAffectedBytes += (int)Math.Ceiling((numBits - bitsRemainingInFirstByte) / 8.0);
            }

            // Read affected bytes into a stream of bits
            List<UInt32> bits = new List<UInt32>();
            for (int i = startByteIndex; i < startByteIndex + totalAffectedBytes; i++)
            {
                byte b = this.rawBytes[i];

                // Put the bits in, LSB first
                for (int bitPos = 0; bitPos < 8; bitPos++)
                {
                    byte mask = Convert.ToByte(0x01 << bitPos);
                    byte val = Convert.ToByte((b & mask) >> bitPos);
                    bits.Add(val);
                }
            }

            // Write the updated bitfield
            int startBitInFirstByte = (offset % 8);
            int bitNumber = 0;
            for (int i = startBitInFirstByte; i < startBitInFirstByte + numBits; i++)
            {
                bits[i] = ((value >> bitNumber) & 0x01);
                bitNumber++;
            }

            // Write the stream of bits back out into the appropriate bytes
            for (int i = startByteIndex; i < startByteIndex + totalAffectedBytes; i++)
            {
                // Read the bits out, reading the one at the head each time, remember LSB is first
                byte b = 0;
                for (int shift = 0; shift < 8; shift++)
                {
                    b = (byte)((bits[0] << shift) | b);
                    bits.RemoveAt(0);
                }

                // Write the byte back to the raw byte array
                this.rawBytes[i] = b;
            }

        }

        void ProcessItem(BitStreamReader bsr)
        {
            // At this point we've read 4 bytes (32 bits)

            bsr.ReadBits(3); // unknown
            this._parent = (Parent)bsr.ReadBits(3);   // offset: bit 35
            bsr.ReadBits(4);  // offset: bit 38, "equipped" field, always zero

            // At this point we've read the two common bytes (before calling this function) followed by
            // 3 + 3 + 4 bitfields.  This means we are currently at an offset of 3 bytes 2 bits.  
            // The two 4-bit col/row fields are next.  THeir positions are:
            // Byte 3 bit 2: 4 bit column
            // Byte 3 bit 6-7 and Byte 4 bit 0-1: 4 bit row
            // This info is necessary when saving these fields in case they change

            this._position = new Point((int)bsr.ReadBits(4), this.Position.Y); // offset: bit 42

            // Docs say this is a 3 bit + an unknown but had to bump to 4, since 3 bits max out at row 8 and
            // d2r has 10 rows in the stash

            this._position = new Point(this.Position.X, (int)bsr.ReadBits(4)); // offset: bit 46

            // Only if parent = "stored" does the next 3 bits have meaning
            if (this.Parent == Parent.Stored)
                this._stash = (Stash)bsr.ReadBits(3); // offset: bit 50
            else
                bsr.ReadBits(3); // Throw away, item is not stashed so it doesn't matter

            // NOTE: Item code is four 8bit characters for pre-D2R but huffman encoded for D2R (seriously?!)
            bool done = false;
            while (!done)
            {
                int bitCount = 0;
                while (++bitCount < 10)
                {
                    string bitStr = bsr.PeekBitsStr(bitCount);
                    if (D2DB.Instance().huffmanDecodeMap.ContainsKey(bitStr))
                    {
                        bsr.ReadBits(bitCount); // Actually remove the bits
                        char decodedVal = D2DB.Instance().huffmanDecodeMap[bitStr];
                        bitStr = "";
                        TypeCode += decodedVal;
                        if (TypeCode.Length == 4)
                            done = true;
                        break;
                    }
                }
            }
            TypeCode = TypeCode.Trim();
            ItemInfo theCurrentItem = D2DB.Instance().itemDB[TypeCode];
            this.Type = D2DB.Instance().itemDB[TypeCode].Name;
            this.Size = new Size((int)theCurrentItem.Width, (int)theCurrentItem.Height);

            if (Type.Contains("Rune"))
            {
                DisplayName = Type;
                Type = "Rune";
            }

#if DEBUG

            Console.WriteLine("Name     : " + DisplayName);
            Console.WriteLine("Parent   : " + Parent.ToString());
            Console.WriteLine("Pos      : " + this.Position.X + "," + this.Position.Y);
            Console.WriteLine("Stash    : " + Stash.ToString());
            Console.WriteLine("Typecode : " + TypeCode.ToString() + " (" + Type + ")");
#endif

            if (IsSimple)
            {
                if (theCurrentItem.IsQuest)
                    bsr.ReadBits(2); // quest info
                this.NumSocketedItems = bsr.ReadBits(1);
            }
            else
            {
                this.NumSocketedItems = bsr.ReadBits(3);
                bsr.ReadBits(32); // item ID, unique 32 bit value for each item
                this.Level = bsr.ReadBits(7);
                this.Quality = (Quality)bsr.ReadBits(4);

                this.HasCustomGraphics = bsr.ReadBits(1) == 1;
                if (this.HasCustomGraphics) // Has custom graphics?
                    this.CustomGraphicsIndex = bsr.ReadBits(3);

                if (bsr.ReadBits(1) == 1) // Is class specific?
                    bsr.ReadBits(11); // Class specific data

                if (Quality == Quality.Inferior)
                {
                    this.Type = ((Inferior)bsr.ReadBits(3)).ToString() + " " + this.Type;
                }
                else if (Quality.ToString().Contains("Normal"))
                {
                    // Does this ever happen?  All charms are magic or unique?
                    if (isCharm())
                        bsr.ReadBits(12); // Charm data
                }
                else if (Quality.ToString().Contains("Superior"))
                {
                    bsr.ReadBits(3); // Always zero
                }
                else if (Quality.ToString().Contains("Magic"))
                {
                    UInt32 prefixID = bsr.ReadBits(11);
                    UInt32 suffixID = bsr.ReadBits(11);
                    if (this.DisplayName == "")
                        this.DisplayName = this.Type;
                    if (prefixID > 0)
                    {
                        string prefix = D2DB.Instance().prefixDB[prefixID];
                        this.DisplayName = prefix + " " + this.DisplayName;
                    }
                    if (suffixID > 0)
                    {
                        string suffix = D2DB.Instance().suffixDB[suffixID];
                        this.DisplayName = this.DisplayName + " " + suffix;
                    }
                }
                else if (Quality.ToString().Contains("Set"))
                {
                    UInt32 setBits = bsr.ReadBits(12);
                    this.DisplayName = D2DB.Instance().setDB[setBits];
                    Console.WriteLine("Set data: " + setBits.ToString());
                }
                else if (Quality.ToString().Contains("Rare") || Quality.ToString().Contains("Crafted") || Quality.ToString().Contains("Tempered"))
                {
                    UInt32 firstName = bsr.ReadBits(8);
                    UInt32 secondName = bsr.ReadBits(8);
                    this.DisplayName = D2DB.Instance().rareAffixDB[firstName] + " " + D2DB.Instance().rareAffixDB[secondName];
                    Console.WriteLine("First name: " + firstName.ToString());
                    Console.WriteLine("Second name: " + secondName.ToString());

                    // Up to 6 affixes (3 prefix, 3 suffix, alternating because of course)
                    // NOTE: Prefix/Suffix ID correspond to row numbers from magicprefix.txt and magicsuffix.txt
                    for (int affixCounter = 0; affixCounter < 3; affixCounter++)
                    {
                        bool hasPrefix = bsr.ReadBits(1) == 1;
                        if (hasPrefix)
                        {
                            UInt32 affixBits = bsr.ReadBits(11);
                            Console.WriteLine("Prefix at prefix ID " + affixCounter.ToString() + ": " + affixBits.ToString());
                        }
                        bool hasSuffix = bsr.ReadBits(1) == 1;
                        if (hasSuffix)
                        {
                            UInt32 affixBits = bsr.ReadBits(11);
                            Console.WriteLine("Suffix at prefix ID " + affixCounter.ToString() + ": " + affixBits.ToString());
                        }
                    }
                }
                else if (Quality.ToString().Contains("Unique"))
                {
                    UInt32 uniqueBits = bsr.ReadBits(12);
                    DisplayName = D2DB.Instance().UniqueDB[uniqueBits];
                    Console.WriteLine("Unique: " + DisplayName + " (" + uniqueBits.ToString() + ")");
                }
                else
                {
                    throw new Exception("Unknown quality: " + Quality.ToString());
                }

                if (IsRuneword)
                {
                    UInt32 runewordID = bsr.ReadBits(12);
                    UInt32 runewordOther = bsr.ReadBits(4);
                    this.DisplayName = D2DB.Instance().RunewordDB[runewordID];
                    Console.WriteLine("Runeword: " + DisplayName + " (" + runewordID.ToString() + ")");
                    Console.WriteLine("Runeword other: " + runewordOther.ToString());
                }

                if (IsPersonalized)
                {
                    UInt32 bitsToRead = 8;

                    string personalizedName = "";
                    for (int i = 0; i < 15; i++)
                    {
                        byte b = (byte)bsr.ReadBits((int)bitsToRead);
                        if (b == 0) break;
                        personalizedName += (char)b;
                    }
                    Console.WriteLine("Personalized name: " + personalizedName);
                }

                if (isTome())
                {
                    UInt32 tomeBits = bsr.ReadBits(5);
                    Console.WriteLine("Tome data: " + tomeBits);
                }

                if (isBodyPart())
                {
                    UInt32 monsterID = bsr.ReadBits(10);
                    Console.WriteLine("Body part monster ID: " + monsterID);
                }

                bool realmDataPresent = bsr.ReadBits(1) == 1;
                int noRealmBits = 128;
                if (realmDataPresent)
                {
                    // NOTE: For D2R items, realm data is always 128
                    Console.WriteLine("Realm data is present!");
                    if (!theCurrentItem.IsMisc || isGem() || isRing() || isAmulet() || isCharm() || isRune())
                    {
                        while (noRealmBits > 0)
                        {
                            UInt32 realmBits = bsr.ReadBits(32);
                            noRealmBits -= 32;
                        }
                    }
                    else
                    {
                        UInt32 realmBits = bsr.ReadBits(3);
                    }
                }

                if (theCurrentItem.IsArmor || theCurrentItem.IsWeapon)
                {
                    if (theCurrentItem.IsArmor)
                    {
                        int numBits = 11;
                        UInt32 defenseRating = bsr.ReadBits(numBits);
                        Console.WriteLine("Defense: " + defenseRating.ToString());
                    }

                    UInt32 maxDurability = bsr.ReadBits(8);
                    Console.WriteLine("Max durability: " + maxDurability.ToString());

                    if (maxDurability > 0)
                    {
                        int duraBits = 9;
                        UInt32 durability = bsr.ReadBits(duraBits);
                        Console.WriteLine("Durability: " + durability.ToString());
                    }
                }

                if (theCurrentItem.IsStackable)
                {
                    UInt32 quantity = bsr.ReadBits(9);
                    Console.WriteLine("Quantity: " + quantity.ToString());
                }

                if (IsSocketed)
                {
                    Sockets = bsr.ReadBits(4);
                    Console.WriteLine("Num sockets: " + Sockets.ToString());
                }

                UInt32 setBonusBits = 0;
                if (Quality.ToString().StartsWith("Set"))
                {
                    setBonusBits = bsr.ReadBits(5);
                    Console.WriteLine("Set bonus bits: " + setBonusBits.ToString());
                }

                // Read mod list
                Console.WriteLine("Reading mods:");

                List<MagicalAttribute> attributes = ReadModList(bsr);
                foreach (MagicalAttribute a in attributes)
                {
                    if (a.tooltip == "") continue;
                    if (this.Mods == "") this.Mods = a.tooltip;
                    else this.Mods += ", " + a.tooltip;
                }

                // Set bonus bits is a 5 bit field. Each bit indicates there's a mod list.
                // For each set bit, read the mod list.
                while (setBonusBits > 0)
                {
                    if ((setBonusBits & 0x01) > 0)
                    {
                        Console.WriteLine("Reading set mods:");
                        ReadModList(bsr);
                    }
                    setBonusBits = setBonusBits >> 1;
                }

                if (IsRuneword)
                {
                    Console.WriteLine("Reading runeword mods:");
                    attributes = ReadModList(bsr);
                    foreach (MagicalAttribute a in attributes)
                    {
                        if (a.tooltip == "") continue;
                        if (this.Mods == "") this.Mods = a.tooltip;
                        else this.Mods += ", " + a.tooltip;
                    }
                }


#if DEBUG
                Console.WriteLine("Num socketed items: " + NumSocketedItems.ToString());
                Console.WriteLine("Item level: " + Level.ToString());
                Console.WriteLine("Quality: " + Quality.ToString());
#endif
            } // end extended item bit parsing (aka non simple items)

            // Any remaining bits are padding until the next byte boundary and can be discarded.  If there
            // are socketed items, they will begin at the next byte boundary.
            rawBytes = bsr.rawBytesRead.ToArray();
            bsr.ClearBits();

            // TODO: fix this for socketed items

            this.SocketedItems = "";
            // Read all socketed items
            for (UInt32 s = 0; s < NumSocketedItems; s++)
            {
                // Read socketed items
                Console.WriteLine("\nSocketed item # " + s);
                Item socketedItem = new Item(bsr);
                socketedItems.Add(socketedItem);
                if (this.SocketedItems != "") this.SocketedItems += ", ";
                this.SocketedItems += socketedItem.DisplayName;

                //byte[] newArray = new byte[rawBytes.Length + socketedItem.rawBytes.Length];
                //Array.Copy(rawBytes, newArray, rawBytes.Length);
                //Array.Copy(socketedItem.rawBytes, 0, newArray, rawBytes.Length, socketedItem.rawBytes.Length);
                //rawBytes = newArray;

                // Add socketed item mods to parent item
                if (this.Mods == "") this.Mods = socketedItem.Mods;
                else this.Mods += ", " + socketedItem.Mods;
            }
            this.SocketedItems = this.SocketedItems.Replace(" Rune", "");
        }

        List<MagicalAttribute> ReadModList(BitStreamReader bsr)
        {
            List<MagicalAttribute> list = new List<MagicalAttribute>();
            while (true)
            {
                MagicalAttribute ma = new MagicalAttribute();

                UInt32 modId = bsr.ReadBits(9);
                if (modId == 0x1ff)
                    break;

                // What mod is this?
                ItemStatCostItem thisItem = D2DB.Instance().idb.DB[(uint)modId];

                // Depending on encoding and save bits, read the various values for this mod
                if (thisItem.Encode == 2)
                {
                    ma.Add(bsr.ReadBits(6) - thisItem.SaveAdd);
                    ma.Add(bsr.ReadBits(10) - thisItem.SaveAdd);
                    ma.Add(bsr.ReadBits((int)thisItem.SaveBits) - thisItem.SaveAdd);
                }
                else if (thisItem.Encode == 3)
                {
                    ma.Add(bsr.ReadBits(6) - thisItem.SaveAdd);
                    ma.Add(bsr.ReadBits(10) - thisItem.SaveAdd);
                    ma.Add(bsr.ReadBits(8) - thisItem.SaveAdd);
                    ma.Add(bsr.ReadBits(8) - thisItem.SaveAdd);
                }
                else if (thisItem.Encode == 4)
                    throw new Exception("Time based stat detected.  How?! These were never implemented!");
                else if (thisItem.ParamBits > 0)
                {
                    if (thisItem.Name == "item_addskill_tab")
                    {
                        ma.Add(bsr.ReadBits(3) - thisItem.SaveAdd);
                        ma.Add(bsr.ReadBits(13) - thisItem.SaveAdd);
                    }
                    else
                    {
                        ma.Add(bsr.ReadBits((int)thisItem.ParamBits) - thisItem.SaveAdd);
                    }
                    ma.Add(bsr.ReadBits((int)thisItem.SaveBits) - thisItem.SaveAdd);
                }
                else
                {
                    ma.Add(bsr.ReadBits((int)thisItem.SaveBits) - thisItem.SaveAdd);
                }

                Console.Write("  Mod: " + modId.ToString() + " (" + thisItem.Name + "), values:");
                foreach (UInt32 val in ma.values) Console.Write(" " + val.ToString());

                // Mods with "mindam" have secondary mods attached to them.  Mod 17 (item_maxdamage_percent)
                // also has an extra item attached to it.  No mod ID follows for seconday mods, just the
                // value of the save/param bits for the next row in the sheet.  Additionally, these follow-on
                // mods do not have any param bits, so only the save bits are read in.
                if (!thisItem.Name.ToLower().Contains("mindam") && modId != 17)
                    goto GenerateTooltip;

                // If execution gets here, mod has follow-on mods.
                if (modId == 21) continue; // mindamage is technically a "min" but this one has no "max"
                ItemStatCostItem secondItem = D2DB.Instance().idb.DB[(uint)modId + 1];

                ma.Add(bsr.ReadBits((int)secondItem.SaveBits) - secondItem.SaveAdd);

                // Some mods have a THIRD mod after!
                if (modId != 57 && // poisonmindam, has three mods total: psn min, psn max, psn length
                    modId != 54 && // coldmindam, has three mods total: cold min, cold max, cold length
                    modId != 40)
                    goto GenerateTooltip;

                // If execution gets here, mod has a THIRD follow-on value
                ItemStatCostItem thirdItem = D2DB.Instance().idb.DB[(uint)modId + 2];
                ma.Add(bsr.ReadBits((int)thirdItem.SaveBits) - thirdItem.SaveAdd);

                if (modId != 40) // maxfireresist has maxlight/maxcold/maxpsn res following it (4th mod!)
                    goto GenerateTooltip;

                // If execution gets here, mod has a FOURTH?! follow-on value
                // NOTE TO SELF: I can't find why I added this.  Debugger seems to never hit this code?
                ItemStatCostItem fourthItem = D2DB.Instance().idb.DB[(uint)modId + 6];
                ma.Add(bsr.ReadBits((int)fourthItem.SaveBits) - fourthItem.SaveAdd);

            GenerateTooltip:

                // Ok so this section is a hacky mess.  *MOST* mods have only a single corresponding tooltip
                // text to go with them. These are fairly easy, just use the mod name to look up the tooltip
                // and then replace the "#". Some mods have more than one "#" and some have things like 
                // "[Skill]", which is typically the second argument because it's the "param", which is read
                // in second.  There are some special cases.  Some of the special cases are handled below,
                // some surely aren't, so this section will probably be spaghetti-fied a bit more as more
                // special cases are found.  The goal right now isn't 100% accuracy, it's "best effort".
                // Ultimately, the user would just open the character and look at the item.

                Console.Write("");

                // Skip a few weirdos
                if (thisItem.Name == "secondary_maxdamage") continue;
                if (thisItem.Name == "secondary_mindamage") continue;
                if (thisItem.Name == "mana") continue;
                if (thisItem.Name == "item_throw_maxdamage") continue;
                if (thisItem.Name == "item_extrablood") continue;

                List<string> possibleMods = D2DB.Instance().propertiesDB[thisItem.Name];

                if (thisItem.Name == "item_addskill_tab") // +n skill mods are handled here
                {
                    ma.tooltip = ReplaceFirst(possibleMods[0], "#", ma.values[2].ToString());
                    switch (ma.values[1])
                    {
                        case 0:
                            ma.tooltip += " (Amazon only)";
                            switch (ma.values[0])
                            {
                                case 0: ma.tooltip = ma.tooltip.Replace("[Class Skill Tab]", "Bow and Crossbow"); break;
                                case 1: ma.tooltip = ma.tooltip.Replace("[Class Skill Tab]", "Passive and Magic"); break;
                                case 2: ma.tooltip = ma.tooltip.Replace("[Class Skill Tab]", "Javelin and Spear"); break;
                            }
                            break;
                        case 1:
                            ma.tooltip += " (Sorceress only)";
                            switch (ma.values[0])
                            {
                                case 0: ma.tooltip = ma.tooltip.Replace("[Class Skill Tab]", "Fire"); break;
                                case 1: ma.tooltip = ma.tooltip.Replace("[Class Skill Tab]", "Lightning"); break;
                                case 2: ma.tooltip = ma.tooltip.Replace("[Class Skill Tab]", "Cold"); break;
                            }
                            break;
                        case 2:
                            ma.tooltip += " (Necromancer only)";
                            switch (ma.values[0])
                            {
                                case 0: ma.tooltip = ma.tooltip.Replace("[Class Skill Tab] Skills", "Curses"); break;
                                case 1: ma.tooltip = ma.tooltip.Replace("[Class Skill Tab]", "Posion and Bone"); break;
                                case 2: ma.tooltip = ma.tooltip.Replace("[Class Skill Tab]", "Summoning"); break;
                            }
                            break;
                        case 3:
                            ma.tooltip += " (Paladin only)";
                            switch (ma.values[0])
                            {
                                case 0: ma.tooltip = ma.tooltip.Replace("[Class Skill Tab]", "Combat"); break;
                                case 1: ma.tooltip = ma.tooltip.Replace("[Class Skill Tab] Skills", "Offensive Auras"); break;
                                case 2: ma.tooltip = ma.tooltip.Replace("[Class Skill Tab] Skills", "Defensive Auras"); break;
                            }
                            break;
                        case 4:
                            ma.tooltip += " (Barbarian only)";
                            switch (ma.values[0])
                            {
                                case 0: ma.tooltip = ma.tooltip.Replace("[Class Skill Tab]", "Combat"); break;
                                case 1: ma.tooltip = ma.tooltip.Replace("[Class Skill Tab] Skills", "Masteries"); break;
                                case 2: ma.tooltip = ma.tooltip.Replace("[Class Skill Tab] Skills", "Warcries"); break;
                            }
                            break;
                        case 5:
                            ma.tooltip += " (Druid only)";
                            switch (ma.values[0])
                            {
                                case 0: ma.tooltip = ma.tooltip.Replace("[Class Skill Tab]", "Summoning"); break;
                                case 1: ma.tooltip = ma.tooltip.Replace("[Class Skill Tab]", "Shape Shifting"); break;
                                case 2: ma.tooltip = ma.tooltip.Replace("[Class Skill Tab]", "Elemental"); break;
                            }
                            break;
                        case 6:
                            ma.tooltip += " (Assassin only)";
                            switch (ma.values[0])
                            {
                                case 0: ma.tooltip = ma.tooltip.Replace("[Class Skill Tab] Skills", "Traps"); break;
                                case 1: ma.tooltip = ma.tooltip.Replace("[Class Skill Tab] Skills", "Shadow Disciplines"); break;
                                case 2: ma.tooltip = ma.tooltip.Replace("[Class Skill Tab] Skills", "Martial Arts"); break;
                            }
                            break;
                    }
                }
                else if (thisItem.Name.Contains("item_skillon"))
                {
                    // Mods: item_skillonhit, item_skillongethit, item_skillondeath, item_skillonlevelup
                    // Tooltip: "#% Chance to cast level # [Skill] when xyz"
                    // Values are stored as level, then skill id, then % chance (why?!)
                    ma.tooltip = possibleMods[0];
                    ma.tooltip = ReplaceFirst(ma.tooltip, "#", ma.values[2].ToString());
                    ma.tooltip = ReplaceFirst(ma.tooltip, "#", ma.values[0].ToString());
                    ma.tooltip = ma.tooltip.Replace("[Skill]", D2DB.Instance().skillDB[(uint)ma.values[1]]);
                }
                else if (possibleMods.Count == 1)
                {
                    // Go left to right, filling in each fillable field
                    ma.tooltip = possibleMods[0];
                    int valueIndex = 0;
                    while (true)
                    {
                        string field = DetectFirstReplaceableField(ma.tooltip);
                        if (field == "")
                            break;
                        if (field == "#")
                            ma.tooltip = ReplaceFirst(ma.tooltip, "#", ma.values[valueIndex].ToString());
                        else if (field == "[Skill]")
                            ma.tooltip = ma.tooltip.Replace("[Skill]", D2DB.Instance().skillDB[(uint)ma.values[valueIndex]]);
                        else
                            throw new Exception("ruhroh");
                        valueIndex++;

                        // Some mods (like maxdurability), have two # in them but only one value was
                        // parsed?  Whatever, just replace both # with the same value.
                        if (ma.values.Count == valueIndex)
                            valueIndex--;
                    }
                }
                else
                {
                    // TODO: More than one property matches this mod, need to figure out a way to determine
                    // which one and apply it.
                    ma.tooltip = "";
                }

                // Some cleanup for some mods
                ma.tooltip = ma.tooltip.Replace("--", "-");

                if (ma.tooltip.Contains("+0"))
                    throw new Exception();

                Console.WriteLine(", " + ma.tooltip);
                Console.WriteLine();

                list.Add(ma);
            }

            return list;
        }

        public string HoverText()
        {
            string mods = this.Mods.TrimEnd([',', ' ']);
            string text = "";
            if (this.DisplayName.Length > 0)
                text += this.DisplayName + "\n";
            text += this.Type + "\n" + mods.Replace(",", "\n") + "\n";

            if (this.IsEthereal)
                text += "(Ethereal)" + "\n";

            if (this.IsSocketed)
            {
                text += "Socketed: " + this.Sockets.ToString();
                if (this.socketedItems.Count > 0)
                    text += " (" + this.SocketedItems + ")\n";
                else
                    text += " (empty)\n";
            }

            text.TrimEnd('\n');

            return text;
        }

        public string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        public string DetectFirstReplaceableField(string text)
        {
            int poundPos = text.IndexOf("#");
            int skillPos = text.IndexOf("[Skill]");

            if (poundPos < 0 && skillPos < 0)
                return "";

            if (poundPos < 0 && skillPos >= 0)
                return "[Skill]";
            if (poundPos >= 0 && skillPos < 0)
                return "#";

            if (poundPos < skillPos)
                return "#";
            else
                return "[Skill]";
        }


        void ProcessEarItem(BitStreamReader bsr)
        {
            UInt32 earUnknown1 = bsr.ReadBits(10); // Technically 3 in 1, "parent" + "equipped", they are always zero
            UInt32 earColumn = bsr.ReadBits(4);
            UInt32 earRow = bsr.ReadBits(3);
            UInt32 earUnknown2 = bsr.ReadBits(1);
            UInt32 earStash = bsr.ReadBits(3);
            UInt32 earOpponentClass = bsr.ReadBits(3);
            UInt32 earOpponentLevel = bsr.ReadBits(7);
            List<byte> opponentName = bsr.ReadBitsArray(120);
            UInt32 earUnknown3 = bsr.ReadBits(1);
        }
        public bool isTome()
        {
            return TypeCode == "ibk" || TypeCode == "tbk";
        }

        public bool isBodyPart()
        {
            return D2DB.Instance().itemDB[TypeCode].Type == "body";
        }

        public bool isGem()
        {
            return "gcv gcw gcg gcr gcb skc gcy gfv gfw gfg gfr gfb skf gfy gsv gsw gsg gsr gsb sku gsy gzv glw glg glr glb skl gly gpw gpv gpb gpy gpr skz gpg".Contains(this.TypeCode);
        }

        public bool isSkull()
        {
            return "skc skf sku skl skz".Contains(this.TypeCode);
        }

        public bool isRing()
        {
            return D2DB.Instance().itemDB[TypeCode].Type == "ring";
        }

        public bool isAmulet()
        {
            return D2DB.Instance().itemDB[TypeCode].Type == "amul";
        }

        public bool isHelm()
        {
            return "cap skp hlm fhl ghm crn msk bhm xap xkp xlm xhl xhm xrn xsk xh9 uap ukp ulm uhl uhm urn usk uh9 ci0 ci1 ci2 ci3".Contains(this.TypeCode) || isBarbHelm() || isDruidHelm();
        }
        public bool isBarbHelm()
        {
            return "ba1 ba2 ba3 ba4 ba5 ba6 ba7 ba8 ba9 baa bab bac bad bae baf".Contains(this.TypeCode);
        }
        public bool isDruidHelm()
        {
            return "dr1 dr2 dr3 dr4 dr5 dr6 dr7 dr8 dr9 dra drb drc drd dre drf".Contains(this.TypeCode);
        }

        public bool isLightHelm()
        {
            return "cap xap uap".Contains(this.TypeCode);
        }

        public bool isBelt()
        {
            return "lbl vbl mbl tbl hbl zlb zvb zmb ztb zhb ulc uvc umc utc uhc".Contains(this.TypeCode);
        }

        public bool isArmor()
        {
            return "qui lea hla stu rng scl chn brs spl plt fld gth ful aar ltp xui xea xla xtu xng xcl xhn xrs xpl xlt xld xth xul xar xtp uui uea ula utu ung ucl uhn urs upl ult uld uth uul uar utp".Contains(this.TypeCode);
        }

        public bool isLightArmor()
        {
            return "qui lea hla stu xui xea xla xtu uui uea ula utu".Contains(this.TypeCode);
        }

        public bool isChainArmor()
        {
            return "rng scl chn xng xcl xhn ung ucl uhn".Contains(this.TypeCode);
        }

        public bool isPlateArmor()
        {
            return "brs spl plt fld gth ful aar ltp xrs xpl xlt xld xth xul xar xtp urs upl ult uld uth uul uar utp ".Contains(this.TypeCode);
        }

        public bool isBoots()
        {
            return "lbt vbt mbt tbt hbt xlb xvb xmb xtb xhb ulb uvb umb utb uhb".Contains(this.TypeCode);
        }

        public bool isChainBoots()
        {
            return "mbt xmb umb".Contains(this.TypeCode);
        }

        public bool isMetalBoots()
        {
            return "tbt hbt xtb xhb utb uhb".Contains(this.TypeCode);
        }

        public bool isGloves()
        {
            return "lgl vgl mgl tgl hgl xlg xvg xmg xtg xhg ulg uvg umg utg uhg".Contains(this.TypeCode);
        }

        public bool isChainGloves()
        {
            return "mgl xmg umg".Contains(this.TypeCode);
        }
        public bool isMetalGloves()
        {
            return "tgl hgl xtg xhg utg uhg".Contains(this.TypeCode);
        }
        public bool isShield()
        {
            return "buc sml lrg kit tow gts bsh spk xuc xml xrg xit xow xts xsh xpk uuc uml urg uit uow uts ush upk".Contains(this.TypeCode) || isPaladinShield() || isNecroShield();
        }

        public bool isPaladinShield()
        {
            return "pa1 pa2 pa3 pa4 pa5 pa6 pa7 pa8 pa9 paa pab pac pad pae paf".Contains(this.TypeCode);
        }

        public bool isNecroShield()
        {
            return "ne1 ne2 ne3 ne4 ne5 ne6 ne7 ne8 ne9 nea neb nec ned nee nef".Contains(this.TypeCode);
        }

        public bool isWoodenShield()
        {
            return "tow gts xow xts uow uts pa3 pa4 pa8 pa9 pad pae".Contains(this.TypeCode);
        }

        public bool isWeapon()
        {
            return "hax  axe  2ax  mpi  wax  lax  bax  btx  gax  gix  clb  spc  mac  mst  fla  whm  mau  gma  ssd  scm  sbr  flc  crs  bsd  lsd  wsd  2hs  clm  gis  bsw  flb  gsd  dgr  dir  kri  bld  tkf  tax  bkf  bal  jav  pil  ssp  glv  tsp  spr  tri  brn  spt  pik  bar  vou  scy  pax  hal  wsc  sbw  hbw  lbw  cbw  sbb  lbb  swb  lwb  lxb  mxb  hxb  rxb  sst  lst  gst  bst  wst  wnd  ywn  bwn  gwn  scp  gsc  wsp  ktr  wrb  axf  ces  clw  btl  skr  ob1  ob2  ob3  ob4  ob5  am1  am2  am3  am4  am5  9ha  9ax  92a  9mp  9wa  9la  9ba  9bt  9ga  9gi  9cl  9sp  9ma  9mt  9fl  9wh  9m9  9gm  9ss  9sm  9sb  9fc  9cr  9bs  9ls  9wd  92h  9cm  9gs  9b9  9fb  9gd  9dg  9di  9kr  9bl  9tk  9ta  9bk  9b8  9ja  9pi  9s9  9gl  9ts  9sr  9tr  9br  9st  9p9  9b7  9vo  9s8  9pa  9h9  9wc  8sb  8hb  8lb  8cb  8s8  8l8  8sw  8lw  8lx  8mx  8hx  8rx  8ss  8ls  8cs  8bs  8ws  9wn  9yw  9bw  9gw  9sc  9qs  9ws  9ar  9wb  9xf  9cs  9lw  9hw  9qr  ob6  ob7  ob8  ob9  oba  am6  am7  am8  am9  ama  7ha  7ax  72a  7mp  7wa  7la  7ba  7bt  7ga  7gi  7cl  7sp  7ma  7mf  7fl  7wh  7m7  7gm  7ss  7sm  7sb  7fc  7cr  7bs  7ls  7wd  72h  7cm  7gs  7b7  7fb  7gd  7dg  7di  7kr  7bl  7tk  7ta  7bk  7b8  7ja  7pi  7s7  7gl  7ts  7sr  7tr  7br  7st  7p7  7o7  7vo  7s8  7pa  7h7  7wc  6sb  6hb  6lb  6cb  6s7  6l7  6sw  6lw  6lx  6mx  6hx  6rx  6ss  6ls  6cs  6bs  6ws  7wn  7yw  7bw  7gw  7sc  7qs  7ws  7ar  7wb  7xf  7cs  7lw  7hw  7qr  obb  obc  obd  obe  obf  amb  amc  amd  ame  amf msf hst".Contains(this.TypeCode);
        }

        public bool isCharm()
        {
            return D2DB.Instance().itemDB[TypeCode].Type.EndsWith("cha");
        }

        public bool isRune()
        {
            return D2DB.Instance().itemDB[TypeCode].Type == "rune";
        }

        public bool isQuest()
        {
            return D2DB.Instance().itemDB[TypeCode].Type == "ques";
        }


        public bool isPotion()
        {
            return this.Type.ToLower().Contains("potion");
        }

        public bool isQuiver()
        {
            return this.TypeCode == "aqv" || this.TypeCode == "cqv";
        }

        public bool isScroll()
        {
            return this.TypeCode == "isc" || this.TypeCode == "tsc";
        }

        public bool isKey()
        {
            return this.TypeCode == "key";
        }

        public bool isJewel()
        {
            return this.TypeCode == "jew";
        }

        public bool isAxe()
        {
            return "hax axe 2ax mpi wax lax bax btx gax gix 9ha 9ax 92a 9mp 9wa 9la 9ba 9bt 9ga 9gi 7ha 7ax 72a 7mp 7wa 7la 7ba 7bt 7ga 7gi".Contains(this.TypeCode);
        }

        public bool isMace()
        {
            return "clb spc mac mst fla whm mau gma 9cl 9sp 9ma 9mt 9fl 9wh 9m9 9gm 7cl 7sp 7ma 7mf 7fl 7wh 7m7 7gm".Contains(this.TypeCode);
        }

        public bool isDagger()
        {
            return "dgr dir kri bld 9dg 9di 9kr 9bl 7dg 7di 7kr 7bl".Contains(this.TypeCode);
        }

        public bool isThrowing()
        {
            return "tkf tax bkf bal 9tk 9ta 9bk 9b8 7tk 7ta 7bk 7b8".Contains(this.TypeCode);
        }

        public bool isJavelin()
        {

            return "jav pil ssp glv tsp 9ja 9pi 9s9 9gl 9ts 7ja 7pi 7s7 7gl 7ts am5 ama amf".Contains(this.TypeCode);
        }
        public bool isSpear()
        {
            return "spr tri brn spt pik 9sr 9tr 9br 9st 9p9 7sr 7tr 7br 7st 7p7".Contains(this.TypeCode);
        }

        public bool isPolearm()
        {
            return "bar vou scy pax hal wsc 9b7 9vo 9s8 9pa 9h9 9wc 7o7 7vo 7s8 7pa 7h7 7wc".Contains(this.TypeCode);
        }

        public bool isBow()
        {
            return "sbw hbw lbw cbw sbb lbb swb lwb 8sb 8hb 8lb 8cb 8s8 8l8 8sw 8lw 6sb 6hb 6lb 6cb 6s7 6l7 6sw 6lw am1 am2 am6 am7 amb amc".Contains(this.TypeCode);
        }
        public bool isCrossbow()
        {
            return "lxb mxb hxb rxb 6lx 6mx 6hx 6rx 8lx 8mx 8hx 8rx".Contains(this.TypeCode);
        }

        public bool isStaff()
        {
            return "sst lst gst bst wst 8ss 8ls 8cs 8bs 8ws 6ss 6ls 6cs 6bs 6ws msf hst".Contains(this.TypeCode);
        }
        public bool isWand()
        {
            return "wnd ywn bwn gwn 9wn 9yw 9bw 9gw 7wn 7yw 7bw 7gw".Contains(this.TypeCode);
        }

        public bool isScepter()
        {
            return "scp gsc wsp 9sc 9qs 9ws 7sc 7qs 7ws".Contains(this.TypeCode);
        }

        public bool isKatar()
        {
            return "ktr wrb axf ces clw btl skr 9ar 9wb 9xf 9cs 9lw 9hw 9qr 7ar 7wb 7xf 7cs 7lw 7hw 7qr".Contains(this.TypeCode);
        }

        public bool isSorceressOrb()
        {
            return "ob1 ob2 ob3 ob4 ob5 ob6 ob7 ob8 ob9 oba obb obc obd obe obf".Contains(this.TypeCode);
        }

        public bool isSmallSword()
        {
            return "ssd scm sbr flc crs bsd lsd wsd 9ss 9sm 9sb 9fc 9cr 9bs 9ls 9wd 7ss 7sm 7sb 7fc 7cr 7bs 7ls 7wd".Contains(this.TypeCode);
        }

        public bool isLargeMetalWeapon()
        {
            return isAxe() || isScepter() || (isMace() && !"clb spc 9cl 9sp 7cl 7sp".Contains(this.TypeCode));
        }

        public bool isSword()
        {
            return isKatar() || isSmallSword();
        }

        public bool isWoodWeaponLarge()
        {
            return isPolearm() || isSpear() || "clb spc 9cl 9sp 7cl 7sp am3 am4 am8 am9 amd ame".Contains(this.TypeCode);
        }

        public bool isSmallMetalWeapon()
        {
            return isDagger() || isThrowing();
        }


        public void Save(MemoryStream ms)
        {
            if (this.Modified)
            {
                ApplyPositionUpdate();
                this.Modified = false;
            }

            ms.Write(rawBytes, 0, rawBytes.Length);
            foreach (Item i in socketedItems)
            {
                i.Save(ms);
            }
        }

    }
}
