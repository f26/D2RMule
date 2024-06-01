# Development Notes

A collection of notes in no particular order detailing various issues/problems/decisions/etc encountered during development.

## Item Sprites

To export the sprites from the game, I first used CASC viewer to extract game data (google it).

Then I used https://github.com/eezstreet/D2RModding-SpriteEdit.  I grabbed all the .sprite files I was interested in and converted them using this tool.

Note: Had to make minor edits in massExportToolStripMenuItem_Click():
* Added Application.DoEvents() to for loop to prevent error where GUI wasn't updated
* Added exception wrapper to prevent crashes
* Added name of current file to text status label thingy in bottom corner

## Scaling Woes

The way the display works is by drawing sprites on top of backgrounds.  If the resolution (aka DPI) of the PNG files do not match when this occurs, there will be weird scaling that happens.  To ensure this doesn't happen, I nuked all exif data using exiftool (look it up).  I ran the following on all the sprites:

    exiftool -all= *
