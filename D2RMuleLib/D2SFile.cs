using System;
using System.Collections;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace D2RMuleLib
{
    public class D2SFile
    {
        // https://github.com/WalterCouto/D2CE/blob/main/d2s_File_Format.md#single-item-layout
        // https://squeek502.github.io/d2itemreader/formats/d2.html

        UInt32 signature;
        D2VersionID versionID;
        UInt32 fileSize;
        UInt32 checkSum;
        UInt32 activeWeapon;
        byte[] oldCharacterName;
        Byte charStatus;
        Byte charProgression;
        UInt16 unknown1;
        Byte charClass;
        public string CharacterClass = "Unknown";
        UInt16 unknown2;
        Byte level;
        UInt32 createdTime;
        UInt32 lastPlayedTime;
        UInt32 unknown3; // All 0xff for v92+
        byte[] assignedSkills; // 64 bytes
        UInt32 leftMouseSkilID;
        UInt32 rightMouseSkillID;
        UInt32 leftSwapMouseSkillID;
        UInt32 rightSwapMouseSkillID;
        byte[] charMenuAppearance; // 32 bytes
        byte[] difficulty; // 3 bytes
        UInt32 map;
        UInt16 unknown4;
        UInt16 mercDead;
        UInt32 mercSeed;
        UInt16 mercNameID;
        UInt16 mercType;
        UInt32 mercExp;

        // 144 bytes, broken down into:
        byte[] unknown5; // 28 bytes
        byte[] d2RCharMenuAppearance; // 48 bytes
        byte[] characterName; // 16 bytes: up to 15 utf-8 characters, null byte padded to 16
        public string CharacterName = "Unknown";
        byte[] unknown6; //48+1+3 = 52 bytes

        byte[] quest; // 298 bytes
        byte[] waypoint; // 80 bytes
        byte[] npc; // 52 bytes

        Attributes attribs;
        public Items playerItems;
        public Items corpseItems;
        public Items mercItems;
        public Items golemItem;

        public bool isHardcore = false;
        bool hasDied = false;
        public bool isExpansion = false;
        bool isLadder = false;

        byte[] headerBytes = { };
        byte[] attributeBytes = { };
        byte[] skillsBytes = { };

        public bool Modified { get; set; } = false;

        const UInt32 HEADER_SIZE = 765;
        const UInt32 FILESIZE_OFFSET = 8;
        const UInt32 CHECKSUM_OFFSET = 12;

        public D2SFile(string filename)
        {

            // Info:
            // https://github.com/WalterCouto/D2CE/blob/main/d2s_File_Format.md
            UInt64 expectedFilesize = GetFileSize(filename);
            UInt32 expectedChecksum = CalculateChecksum(filename);


            using (FileStream fileStream = new System.IO.FileStream(filename, FileMode.Open, FileAccess.Read))
            using (BinaryReader binReader = new System.IO.BinaryReader(fileStream, Encoding.ASCII))
            {
                Console.WriteLine("Opening file: " + filename);
                ReadHeader(binReader, expectedFilesize, expectedChecksum);
                ReadAttributes(binReader);
                ReadSkills(binReader);

                playerItems = new Items(this.CharacterName, binReader);
                corpseItems = new Items(this.CharacterName, binReader, Items.ItemsType.Corpse);

                if (isExpansion) // Merc/golem only present for expansion characters
                {
                    mercItems = new Items(this.CharacterName, binReader, Items.ItemsType.Mercenary);
                    golemItem = new Items(this.CharacterName, binReader, Items.ItemsType.IronGolem);
                }
                else
                {
                    mercItems = new Items(this.CharacterName, Items.ItemsType.Mercenary);
                    golemItem = new Items(this.CharacterName, Items.ItemsType.IronGolem);
                }
            }
        }

        public void Save(string filename)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Write arrays of bytes to the MemoryStream
                memoryStream.Write(headerBytes, 0, headerBytes.Length);
                memoryStream.Write(attributeBytes, 0, attributeBytes.Length);
                memoryStream.Write(skillsBytes, 0, skillsBytes.Length);

                playerItems.Save(memoryStream);
                corpseItems.Save(memoryStream);
                if (isExpansion) // Merc/golem only present for expansion characters
                {
                    mercItems.Save(memoryStream);
                    golemItem.Save(memoryStream);
                }

                // All data has been written to mem stream, regenerate the checksum field
                RegenerateFilesize(memoryStream);
                RegenerateChecksum(memoryStream);

                memoryStream.Seek(0, SeekOrigin.Begin);
                using (FileStream fileStream = File.Create(filename))
                {
                    memoryStream.CopyTo(fileStream);
                }
            }
        }


        public UInt32 CalculateChecksum(string filename)
        {
            // NOTE: Calculation of checksum requires the checksum field to be set to all zero

            byte[] bytes = File.ReadAllBytes(filename);
            bytes[12] = 0;
            bytes[13] = 0;
            bytes[14] = 0;
            bytes[15] = 0;
            Int32 checksum = 0;
            foreach (byte b in bytes)
            {
                if (checksum < 0)
                    checksum = (checksum << 1) + b + 1;
                else
                    checksum = (checksum << 1) + b;
            }

            return (UInt32)checksum;
        }

        public UInt32 CalculateChecksum(MemoryStream ms)
        {
            // NOTE: Calculation of checksum requires the checksum field to be set to all zero

            byte[] bytes = ms.ToArray();
            bytes[12] = 0;
            bytes[13] = 0;
            bytes[14] = 0;
            bytes[15] = 0;
            Int32 checksum = 0;
            foreach (byte b in bytes)
            {
                if (checksum < 0)
                    checksum = (checksum << 1) + b + 1;
                else
                    checksum = (checksum << 1) + b;
            }

            return (UInt32)checksum;
        }

        public void RegenerateChecksum(MemoryStream ms)
        {
            UInt32 checksum = CalculateChecksum(ms);
            ms.Seek(CHECKSUM_OFFSET, SeekOrigin.Begin);
            using (BinaryWriter writer = new BinaryWriter(ms, Encoding.Default, true))
            {
                writer.Write(checksum);
            }
        }

        public void RegenerateFilesize(MemoryStream ms)
        {
            UInt32 filesize = (UInt32)ms.Length;
            ms.Seek(FILESIZE_OFFSET, SeekOrigin.Begin);
            using (BinaryWriter writer = new BinaryWriter(ms, Encoding.Default, true))
            {
                writer.Write(filesize);
            }
        }

        private void ReadHeader(BinaryReader binReader, UInt64 expectedFilesize, UInt32 expectedChecksum)
        {
            // Signature
            signature = binReader.ReadUInt32();
            if (signature != 0xaa55aa55)
                throw new Exception("Unexpected signature in .d2s file.  Expected 0xaa55aa55 but encountered " + signature.ToString("x8"));

            // Version ID.  Only latest version is supported for now.
            versionID = (D2VersionID)binReader.ReadUInt32();
            Console.WriteLine("File version: " + versionID.ToString() + " (" + (uint)versionID + ")");
            if (versionID < D2VersionID._99_D2R14)
                throw new Exception("UNSUPPORTED: Very old .d2s file, seems to be older than 1.14?");

            // File size
            fileSize = binReader.ReadUInt32();
            if (expectedFilesize != fileSize)
            {
                throw new Exception("File is " + expectedFilesize.ToString() + " bytes but size field says it should be " + fileSize.ToString() + " bytes");
            }

            // Checksum
            checkSum = binReader.ReadUInt32();
            if (checkSum != expectedChecksum)
                throw new Exception("Unexpected checksum in .d2s file.  Expected " + expectedChecksum.ToString("x8") + " but encountered " + checkSum.ToString("x8"));

            // Active weapon
            activeWeapon = binReader.ReadUInt32();

            // Name (NOTE: this only contains name for D2R_11 and prev, it is null for latest D2R version)
            oldCharacterName = binReader.ReadBytes(16);

            charStatus = binReader.ReadByte();
            isHardcore = ((charStatus & 0b00000100) > 0);
            hasDied = ((charStatus & 0b00001000) > 0);
            isExpansion = ((charStatus & 0b00100000) > 0);
            isLadder = ((charStatus & 0b01000000) > 0);
            charProgression = binReader.ReadByte();
            unknown1 = binReader.ReadUInt16();
            charClass = binReader.ReadByte();
            switch (charClass)
            {
                case 0: CharacterClass = "Amazon"; break;
                case 1: CharacterClass = "Sorceress"; break;
                case 2: CharacterClass = "Necromancer"; break;
                case 3: CharacterClass = "Paladin"; break;
                case 4: CharacterClass = "Barbarian"; break;
                case 5: CharacterClass = "Druid"; break;
                case 6: CharacterClass = "Assassin"; break;
            }

            unknown2 = binReader.ReadUInt16();
            level = binReader.ReadByte();
            createdTime = binReader.ReadUInt32();
            lastPlayedTime = binReader.ReadUInt32();
            unknown3 = binReader.ReadUInt32();

            // Skills
            assignedSkills = binReader.ReadBytes(64);
            leftMouseSkilID = binReader.ReadUInt32();
            rightMouseSkillID = binReader.ReadUInt32();
            leftSwapMouseSkillID = binReader.ReadUInt32();
            rightSwapMouseSkillID = binReader.ReadUInt32();
            charMenuAppearance = binReader.ReadBytes(32);
            difficulty = binReader.ReadBytes(3);
            map = binReader.ReadUInt32();
            unknown4 = binReader.ReadUInt16();

            // Merc data
            mercDead = binReader.ReadUInt16();
            mercSeed = binReader.ReadUInt32();
            mercNameID = binReader.ReadUInt16();
            mercType = binReader.ReadUInt16();
            mercExp = binReader.ReadUInt32();

            unknown5 = binReader.ReadBytes(28);
            d2RCharMenuAppearance = binReader.ReadBytes(48);
            characterName = binReader.ReadBytes(16);
            CharacterName = Encoding.UTF8.GetString(characterName).TrimEnd('\0');
            unknown6 = binReader.ReadBytes(52);

            // Various arrays
            quest = binReader.ReadBytes(298);
            waypoint = binReader.ReadBytes(80);
            npc = binReader.ReadBytes(52);

            // Header has been read successfully, save its entire contents
            if (binReader.BaseStream.Position != HEADER_SIZE)
            {
                throw new Exception("Unexpected position in stream.  Expected pos " + HEADER_SIZE.ToString() + " after reading the header, but the stream is at position " + binReader.BaseStream.Position.ToString());
            }
            binReader.BaseStream.Position = 0;
            headerBytes = binReader.ReadBytes((int)HEADER_SIZE);
        }

        private void ReadAttributes(BinaryReader binReader)
        {
            attribs = new Attributes(binReader);

            UInt32 attributesSize = (UInt32)(binReader.BaseStream.Position - HEADER_SIZE);
            binReader.BaseStream.Position = HEADER_SIZE;
            attributeBytes = binReader.ReadBytes((int)attributesSize);
        }
        private void ReadSkills(BinaryReader binReader)
        {
            skillsBytes = binReader.ReadBytes(32);
        }

        UInt64 GetFileSize(string filename)
        {
            FileInfo fileInfo = new FileInfo(filename);
            return (UInt64)fileInfo.Length;
        }
    }
}
