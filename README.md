<!-- TODO UPDATE -->

## Make every consumable item infinite

Collect enough a consumable item to make it infinite, and not consume it!
Depending of the item, this requirement may vary.
There are a wide variety of categorie for all sorts of consumables (usables, placeables, materials and more) each with their own sub-categories.
You can also use the journey research cost of items as their infinities.
Works with modified max stack and most (if not all) modded items.


## No more item duplication

**WORK IN PROGRESS, BREAKS EASILY**

NPCs, Tiles, walls, etc dropping an item when destroyed, killed, etc won't drop if they were infinite when used.
Include all placeable, critter, and other items.

## Alter the behaviour of the mod

Edit the `Infinity Requirements` config to modify the behaviour of the mod!
Toggle on/off certain categories and features of the mods.
Check and change infinities (use negative values for stacks, e.g.: 5 <=> 5 items, -5 <=> 5 stacks).
Add customs for items with incorrect categories (create an issue on the github if needed so in I can fix it).


## Detect the category of most items

If an item's category cannot be determined with the items's stats, it will be found when the item is used!
This should work for almost every summoner, player booster and more.
Edit the `Auto Categories` config to modify disable this or clear all the detected categories!


## Easely see the infinities of an item

The infinities of every item can be displayed in various ways: 
 - In its tooltip via colored lines. This can also be used to display the categories and requirements of item.
 - On the item sprite via glow. Not that it can sometimes be hard to see.
 - In the item slot via colored dots.

Edit the `Infinity Display` config to change which one is active and display aditional informations or change the color of the infinities.


## Categories

Below you can find all vanilla infinities and their respective categories.
Note than an item can be in on one each (e.g. bone: weapon, special ammo, block).

### Usables
Items consumed uppon use.
 - `Weapon`: Deals damage.
 - `Recovery`: Heals life or mana.
 - `Buff`: Provides a buff (swiftness, archery...).
 - `Player booster`: Give a permanent boost to the player (live, mana...).
 - `World booster`: Permanently modifies the world.
 - `Summoner`: Summons a boss or an event.
 - `Critter`: Summong a critter or is a bait.
 - `Explosive`: Destroys tiles or creates liquid.
 - `Tool`: Has a miscellaneous use (move the player, light an area...).
 - `Unknown`: Is one of the above but not yep determined

### Ammunitions
Items used as ammunition and consumed by their weapon.
 - `Basic`: Arrows, bullets, rockets and darts.
 - `Explosive`: Explodes on impact.
 - `Special`: All other ammunitions.
 - 
### Placeables
Items placing a tile.
 - `Block`: Places a block.
 - `Wall`: Places a wall.
 - `Wiring`: Wires.
 - `Torch`: Places a torch.
 - `Ore`: Raw minerals.
 - `Gem`: Not used for now.
 - `LightSource`: Emits light.
 - `Container`: Can store items.
 - `Functional`: Can be interacted with.
 - `CraftingStation`: Use to craf items.
 - `Decoration`: A basic furniture.
 - `MusicBox`: Plays music.
 - `Mechanical`: Can be triggered with a wire.
 - `Liquid`: Places or remove liquid.
 - `Seed`: Grows into a plant.
 - `Paint`: Color walls and tiles.
 - 
### Grab bags
Items giving various items when openned.
 - `Crate`: Can be openned.
 - `TreasureBag`: Dropped form a boss in expert mode or above.
 - 
### Materials
Items used for crafting.
Infinity is 1/2 of the requirement.
 - `Basic`: Blocks, common items.
 - `Ore`: Turn into bars.
 - `Furniture`:
 - `Miscellaneous`: Only a material.
 - `NonStackable`: Weapons, armors...
 - 
### Currencies
Items used to buy from NPCs.
 - `Coin`: Currencies with more than one items (infinity is 1/10 of the requirement).
 - `SingleCoin`: Currencies with a single item (infinity is 1/5 of the requirement)..


## Configs

### Requirements
This is the main config of the mod (server side)
 - `General`: Controls key aspects of the mods, such as active infinties and special co,features.
 - `Requirement (x5)`: Lists all the modifiable requirement for all categories. Use a negative value for a stack (e.g.: 5 = 5 items, -5 = 5 stacks).
 - `Customs`: The list of used modified categories. Overrides the default category.

### Category detection
Controls automatic category detection (client side).
 - `Detected Categories`: All the detected categories. Reset the config or clear individual items to remove them.

### Infinity Display
Various ways to display the infinities of items.
 - `Item Tooltip` In the item tooltip
 - `Item glow` In the colore of the item
 - `Colored dots` With colored dots around the item. Start end end are the positions of the 1st and last dot able to be displayed. In they are more infinities than dots, the will cycle every few seconds.


## Changelog

#### v2.2
 - Reworked and abstracted infinity API to be easier to use.
 - Remanes and typo fixes.
 - Disabled tile dupplication as it is full of bugs.
 - Renamed 'Consumables' to 'Usables' for clarity.

#### v2.1
 - Ported to next tml stable.
 - Magic storage integration.
 - Infinities are now displayed in item sprite, glow and tooltip.
 - Added Infinity display config.
 - Imporved the way infinites are stored.
 - Improved locatization.
 - Fixed colored tooltip not been correctly displayed .
 - Fixed mouseItem not been counted or been counted twice.
 - Fixed grabbags and openned by leftclick not been detected.
 - Fixed potions used by right clik not been detected.
 - Fixed detected categories been applied one tick
 - Fixed a bug in stack detection causing the mod to crash on load.
 - Fixed other bugs.

#### v2.0
 - rewrote and cleaned the entire codebase.
 - new category and infinity system.
 - added currencies and grabbags categories.
 - merged placeable and wand ammo.
 - added informations to the item tooltip.
 - added sub categories for most categories.
 - reworked infite materials.
 - can now detect usable and explosive items.
 - Reworked tile duplication to use a chunk system and be induded in .twld file.
 - more flexible configs.
 - added many new config items.
 - removed commands.
 - reworked stack detection mecanism to be automatic.
 - performance improvement.
 - updated desc and README
 - en-US localization
 - added mod icon
 - fixed a ton of bug.

#### v1.3.1
 - Finaly released on the Mod Browser.
 - `Liquid` buckets can now be infinite.
 - Works with wires and actuators.

#### v1.3
 - Can now works with mods increasing max stacks and added related config items.
 - Furniture will now have the right category.

#### v1.2
 - Prevents item duppliation for infinite usables.
 - Added new categories.

#### v1.1
 - Custom categories and values introduced.
 - Added `set`, `category` and `values` commands.

#### v1.0
 - Consumables can now be infinite.