# D2RMule

A muling program for Diablo 2 Resurrected.  Works only for offline characters (duh).

Written in C# .NET 8 with Visual Studio 2022.

**DISCLAIMER** : While I have tried to ensure this program works properly (I use it myself), I make no claims regarding the safety of this program with regards to not corrupting/losing your precious character files and items.  Always make regular backups.

## Features

* Unlimited vault space (ok technically 4 billion items is the max)
* Easily transfer items between characters/vault in a natural way
* Separate vaults for all combinations of classic/expandion and normal/hardcore characters.
* Vault filtering to easily find items
* High quality D2R graphics (extracted directly from game files)
* Sounds! (also extracted from game files)
* Item information displayed in a tool tip on hover
* Item list utility uses [ObjectListView](https://github.com/f26/ObjectListView) to display all items

## Controls
* Left click an item = pick it up
* Left click an empty spot = drop item on cursor
* Right click an item = send to vault
* Right click an item in the vault = send to player stash

## Limitations

A list of known limitations, issues, shortcomings, etc.

* NOT AN ITEM EDITOR.  WILL NOT EDIT ITEMS.
* Only compatible with latest D2R character files
* Item information displayed on hover is not a 100% exact match to what you see in the game due to a problem I suffer from called "meh, good enough"
* While there are checks to ensure specific items cannot be equipped on specific slots, the checks are lazy.  Ex: you can equip two 1 handed weapons or two shields or weapons/armor you don't have the str/dex/lvl req for.  No idea what will happen when you load this character in game, feel free to test it out.
* Does not display shared stash items
* Does not display belt items
* Does not display corpse items
* Does not display the item an iron golem is made from (if present)

## Other

For dev notes, see [DevNotes.md](./DevNotes.md)