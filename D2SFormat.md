# D2S Item Format

This documents the D2S item format.  This has been reverse engineered from various online sources and verified by ingesting dozens of save files both found on the web and created locally by myself.  This information is accurate for D2R patch 2.5+ (Sep 2022) but NOT FOR OLDER FILES.  The format of the file can vary greatly for older version IDs.  The way it varies is beyond the scope of this document.  This document focuses only on version ID 99.  For more info see [Version ID](#version-id) section.


## Overview

 A D2S file is composed of four main sections.  Two sections have a fixed size and two are variable.

| Type     | Size | Section                   |
|:--------:|:----:|:-------------------------:|
| Fixed    | 765  | [Header](#header)         |
| Variable | ~    | [Attributes](#attributes) |
| Fixed    | 32   | [Skills](#skills)         |
| Variable | ~    | [Items](#items)           |

Each section is described in detail in its respective section below.

## Bitfield info

A good bit of the contents of a D2S file are values saved as bitfields.  It is important to understand exactly how these bitfields are laid out.  It's not as simple as reading each bit "left to right" without considering the bit order.  Each byte's bits need to be arranged LSB-first and then each bitfield is read from these bits, also LSB-first.  This is best explained via an example.

### Example:

Take the following hex sequence representing an item: 10 00 a0 00 05 00 f4 30 df 01

This sequence breaks down as follows:

| Byte number         | 1        | 2        | 3        | 4        | 5        | 6        | 7        | 8        | 9        | 10       |
|:-------------------:|:--------:|:--------:|:--------:|:--------:|:--------:|:--------:|:--------:|:--------:|:--------:|:--------:|
| As hex              | 10       | 00       | a0       | 00       | 05       | 00       | f4       | 30       | df       | 01       |
| As binary           | 00010000 | 00000000 | 10100000 | 00000000 | 00000101 | 00000000 | 11110100 | 00110000 | 11011111 | 00000001 |
| As LSB-first binary | 00001000 | 00000000 | 00000101 | 00000000 | 10100000 | 00000000 | 00101111 | 00001100 | 11111011 | 10000000 |


Entire LSB-first 256 bit sequence, color coded to match up with bitfields below:

<span style="color:red">0000</span><span style="color:orange">1</span><span style="color:yellow">000000</span><span style="color:green">0</span><span style="color:blue">0</span><span style="color:purple">0</span><span style="color:red">00</span><span style="color:orange">0</span><span style="color:yellow">0</span><span style="color:green">000</span><span style="color:blue">1</span><span style="color:purple">0</span><span style="color:red">1</span><span style="color:orange">0</span><span style="color:yellow">0</span><span style="color:green">0</span><span style="color:blue">00000</span><span style="color:purple">101</span><span style="color:red">000</span><span style="color:orange">0000</span><span style="color:yellow">0000</span><span style="color:green">0000</span><span style="color:blue">101</span><span style="color:purple">00111</span><span style="color:red">001100</span><span style="color:orange">11011111</span><span style="color:yellow">10</span><span style="color:green">0</span><span style="color:blue">000000</span>     

To parse the item, read item bitfields left to right, bit by bit:

| Bits                                       | Value | Description         |
|:------------------------------------------:|:-----:|:-------------------:|
| <span style="color:red">0000</span>        | 0     | Unknown             |
| <span style="color:orange">1</span>        | 1     | Is identified       |
| <span style="color:yellow">000000</span>   | 0     | Unknown             |
| <span style="color:green">0</span>         | 0     | Is socketed         |
| <span style="color:blue">0</span>          | 0     | Unknown             |
| <span style="color:purple">0</span>        | 0     | Is new              |
| <span style="color:red">00</span>          | 0     | Unknown             |
| <span style="color:orange">0</span>        | 0     | Is ear              |
| <span style="color:yellow">0</span>        | 0     | Is starting item    |
| <span style="color:green">000</span>       | 0     | Unknown             |
| <span style="color:blue">1</span>          | 1     | Is simple           |
| <span style="color:purple">0</span>        | 0     | Is ethereal         |
| <span style="color:red">1</span>           | 1     | Unknown             |
| <span style="color:orange">0</span>        | 0     | Is personalized     |
| <span style="color:yellow">0</span>        | 0     | Unknown             |
| <span style="color:green">0</span>         | 0     | Is runeword         |
| <span style="color:blue">00000</span>      | 0     | Unknown             |
| <span style="color:purple">101</span>      | 5     | Unknown             |
| <span style="color:red">000</span>         | 0     | Parent (0 = Stored) |
| <span style="color:orange">0000</span>     | 0     | Equipped            |
| <span style="color:yellow">0000</span>     | 0     | Column              |
| <span style="color:green">0000</span>      | 0     | Row                 |
| <span style="color:blue">101</span>        | 0     | Stash (5 = Stash)   |
| <span style="color:purple">00111</span>    | r     | Huffman encoded 'r' |
| <span style="color:red">001100</span>      | 2     | Huffman encoded '2' |
| <span style="color:orange">11011111</span> | 0     | Huffman encoded '0' |
| <span style="color:yellow">10</span>       | ' '   | Huffman encoded ' ' |
| <span style="color:green">0</span>         | 0     | Num socketed items  |
| <span style="color:blue">000000</span>     |       | Unused, discarded   |

We can see that the item is "r20" which is a Lem rune.  it is in the stash at position 0,0.

## Header

The header is always 765 bytes and has some housekeeping fields and some general information about the character.  The purpose of some fields are unknown, as no official documentation of the header has ever been provided.  Unknown fields aren't necessarily empty (0x00).  Some are, but others have values in them.  The program does not attempt to modify them regardless: the entire header is read in and even the unknown fields are saved and then written out when it is time to write the file.

All fields in the header are aligned on byte boundaries.  Field sizes vary form 1 to 298 bytes.

| Offset | Size<br>(bytes) |  Data Type | Description                  |
|:------:|:---------------:|:----------:|:-----------------------------|
| 0      | 4               | UInt32     | Signature, always 0xaa55aa55 |
| 4      | 4               | UInt32     | [Version ID](#version-id)    |
| 8      | 4               | UInt32     | File size in bytes           |
| 12     | 4               | UInt32     | Checksum                     |
| 16     | 4               | UInt32     | Active weapon                |
| 20     | 16              | string     | All 0x00                     |
| 36     | 1               | byte       | Character status             |
| 37     | 1               | byte       | Character progression        |
| 38     | 2               | UInt16     | ?                            |
| 40     | 1               | byte       | Character class              |
| 41     | 2               | UInt16     | ?                            |
| 43     | 1               | byte       | Level                        |
| 44     | 4               | UInt32     | Created time                 |
| 48     | 4               | UInt32     | Last played time             |
| 52     | 4               | UInt16     | ?                            |
| 56     | 64              | Array      | Assigned skills              |
| 120    | 4               | UInt32     | Left mouse skill ID          |
| 124    | 4               | UInt32     | Right mouse skill ID         |
| 128    | 4               | UInt32     | Left mouse swap skill ID     |
| 132    | 4               | UInt32     | Right mouse swap skill ID    |
| 136    | 32              | Array      | Character menu appearance    |
| 168    | 3               | UInt24     | Difficulty                   |
| 171    | 4               | UInt32     | Map                          |
| 175    | 2               | UInt16     | ?                            |
| 177    | 2               | UInt16     | Merc dead                    |
| 179    | 4               | UInt32     | Merc seed                    |
| 183    | 2               | UInt16     | Merc name ID                 |
| 185    | 2               | UInt16     | Merc type                    |
| 187    | 4               | UInt16     | Merc experience              |
| 191    | 28              | Array      | ?                            |
| 219    | 48              | Array      | Character menu appearance    |
| 267    | 16              | String     | Character name               |
| 283    | 48              | Array      | ?                            |
| 331    | 1               | byte       | ?                            |
| 332    | 3               | UInt24     | ?                            |
| 335    | 298             | Array      | Quest info                   |
| 633    | 80              | Array      | Waypoint info                |
| 713    | 52              | Array      | NPC info                     |

### Version ID

My understanding is that D2R will read old character files and upconvert them to the latest format, so if you have an old file be sure to open it in D2R first and it will be saved in the latest format. (This may be innacurate, I did not have very old files to test with, only v96 and higher, I can confirm it will work with v96 files).

| Value | Diablo II Version         | Release Date |
|:-----:|:-------------------------:|:------------:|
| 71    | D2 1.0 - 1.06             | 2000-06-28   |
| 87    | D2 LoD 1.08               | 2001-06-29   |
| 89    | D2 1.08                   | 2001-06-29   |
| 92    | D2 LoD 1.09               | 2001-08-20   |
| 96    | D2 LoD 1.10 - 1.14        | 2003-10-28   |
| 97    | D2R 1.0 - 1.1             | 2021-08-13   |
| 98    | D2R 1.2 - 1.3 (Patch 2.4) | 2022-04-14   |
| 99    | D2R 1.4 (Patch 2.5)       | 2022-09-22   |

## Attributes

This section contains multiple attributes.  Each attribute is idenfitied by a 9 bit ID value followed by a 7 to 32 bit number representing its value.  Each attribute is self-explanatory based on its name.

| Attribute ID |  Size<br>(bits) | Description     |
|:------------:|:-----:|:----------------|
| 0            | 10    | Strength        |
| 1            | 10    | Energy          |
| 2            | 10    | Dexterity       |
| 3            | 10    | Vitality        |
| 4            | 10    | Unused Stats    |
| 5            | 8     | Unused Skills   |
| 6            | 21    | Current HP      |
| 7            | 21    | Max HP          |
| 8            | 21    | Current Mana    |
| 9            | 21    | Max Mana        |
| 10           | 21    | Current Stamina |
| 11           | 21    | Max Stamina     |
| 12           | 7     | Level           |
| 13           | 32    | Experience      |
| 14           | 25    | Gold            |
| 15           | 25    | Stashed Gold    |


| Size |  Type  | Description                  |
|:----:|:------:|:-----------------------------|
| 4    | UInt16 | Signature, always 0x6667 ("gf) |
| 4    | UInt32 | [Version ID](#version-id)    |
| 4    | UInt32 | File size in bytes           |


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
                    
## Skills

## Items
