# D2S Item Format

This documents the d2s item format.  This has been reverse engineered from various online sources.  This is accurate for D2R patch 2.5+ (Sep 2022).  The format of the file can vary greatly for older version IDs.  The way it varies is beyond the scope of this document.  This document focuses only on version ID 99.

My understanding is that D2R will read old character files and upconvert them to the latest format, so if you have an old file be sure to open it in D2R first and it will be saved in the latest format. (This may be innacurate, I did not have very old files to test with, only v96 and higher).

| Offset | Size |  Type  | Description                  |
|:------:|:----:|:------:|:-----------------------------|
| 0      | 4    | UInt32 | Signature, always 0xaa55aa55 |
| 4      | 4    | UInt32 | [Version ID](#version-id)    |
| 8      | 4    | UInt32 | File size in bytes           |
| 12     | 4    | UInt32 | Checksum                     |
| 16     | 4    | UInt32 | Active weapon                |
| 20     | 16   | string | All 0x00                     |
| 36     | 1    | byte   | Character status             |
| 37     | 1    | byte   | Character progression        |
| 38     | 2    | UInt16 | Unknown                      |
| 40     | 1    | byte   | Character class              |
| 41     | 2    | UInt16 | Unknown                      |
| 43     | 1    | byte   | Level                        |
| 44     | 4    | UInt32 | Created time                 |
| 48     | 4    | UInt32 | Last played time             |
| 52     | 4    | UInt16 | Unknown                      |
| 56     | 64   | Array  | Assigned skills              |
| 120    | 4    | UInt32 | Left mouse skill ID          |
| 124    | 4    | UInt32 | Right mouse skill ID         |
| 128    | 4    | UInt32 | Left mouse swap skill ID     |
| 132    | 4    | UInt32 | Right mouse swap skill ID    |
| 136    | 32   | Array  | Character menu appearance    |
| 168    | 3    | UInt24 | Difficulty                   |
| 171    | 4    | UInt32 | Map                          |
| 175    | 2    | UInt16 | Unknown                      |
| 177    | 2    | UInt16 | Merc dead                    |
| 179    | 4    | UInt32 | Merc seed                    |
| 183    | 2    | UInt16 | Merc name ID                 |
| 185    | 2    | UInt16 | Merc type                    |
| 187    | 4    | UInt16 | Merc experience              |
| 191    | 28   | Array  | Unknown                      |
| 219    | 48   | Array  | Character menu appearance    |
| 267    | 16   | Array  | 0x00                         |
| 283    | 48   | Array  | 0x00                         |
| 331    | 1    | byte   | 0x00                         |
| 332    | 3    | UInt24 | 0x00                         |
| 335    | 298  | Array  | Quest info                   |
| 633    | 80   | Array  | Waypoint info                |
| 713    | 52   | Array  | NPC info                     |
| 765    | n    | Array  | Attributes (variable length) |



### Version ID
| Value | Diablo II Version         | Date       |
|:-----:|:-------------------------:|:----------:|
| 71    | D2 1.0 - 1.06             | 2000-06-28 |
| 87    | D2 LoD 1.08               | 2001-06-29 |
| 89    | D2 1.08                   | 2001-06-29 |
| 92    | D2 LoD 1.09               | 2001-08-20 |
| 96    | D2 LoD 1.10 - 1.14        | 2003-10-28 |
| 97    | D2R 1.0 - 1.1             | 2021-08-13 |
| 98    | D2R 1.2 - 1.3 (Patch 2.4) | 2022-04-14 |
| 99    | D2R 1.4 (Patch 2.5)       | 2022-09-22 |

