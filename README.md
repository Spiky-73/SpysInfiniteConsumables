## Make every consumable item infinite

Collect enough a consumable item to make it infinite, and not consume it!
Depending of the item, this requirement may vary.
There are a wide variety of categorie for all sorts of consumables (consumables, placeables, materials and more) each with their own sub-categories.
You can also use the journey research cost of items as their infinities.
Works with modified max stack and most (if not all) modded items.


## No more item duplication

**WORK IN PROGRESS, BREAKS EASILY**

NPCs, Tiles, walls, etc dropping an item when destroyed, killed, etc won't drop if they were infinite when used.
Include all placeable, critter, and other items.

## Alter the behaviour of the mod

Edit the Infinity config to modify the behaviour of the mod!
Toggle on/off certain categories and features of the mods.
Check and change infinities (use negative values for stacks, i.g.: 5 = 5 items, -5 = 5 stacks).
Add customs for items with incorrect categories (create an issue on the github if needed so in I can fix it).


## Detect the category of most items

If an item's category cannot be determined with the items's stats, it will be found when the item is used!
This should work for almost every summoner, player booster and other items.
Edit the Auto Categories config to modify disable this or clear all the detected categories!


## See what the mod is doing

Edit the Auto Categories config to display various infos used by the mod!
By default, infinte items will be colored and their tooltip will change.
In addition, the category and requirement of items can be showed by enabling toogles.


## Categories

Below you can find all possible the categories.
Note than an item can be in on eon each (i.g. bone: weapon, special ammo, block).

### Consumables
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
 - `Basic`: Arrows, bullets, rockets and darts.
 - `Explosive`: Explodes on impact.
 - `Special`: All other ammunitions.
 - 
### Placeables
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
 - `Crate`: Can be openned.
 - `TreasureBag`: Dropped form a boss in expert mode or above.
 - 
### Materials
Infnity is 1/2 of the requirement.
 - `Basic`: Blocks, common items.
 - `Ore`: Turn into bars.
 - `Furniture`:
 - `Miscellaneous`: Only a material.
 - `NonStackable`: Weapons, armors...
 - 
### Currencies
 - `Coin`: Currencies with more than one items (infnity is 1/10 of the requirement).
 - `SingleCoin`: Currencies with a single item (infnity is 1/5 of the requirement)..


## Configs

### Infinties
This is the main config of the mod (server side)
 - `General`: Controls key aspects of the mods, such as active infinties and special co,features.
 - `Requirement (x5)`: Lists all the modifiable requirement for all categories. Use a negative value for a stack (i.g.: 5 = 5 items, -5 = 5).
 - `Customs`: The list of used modified categories. Overrides the default category.

### Auto categories and Visuals
Controls the displayed item tooltips and the automatic categories (client side).
 - `Infinities Display`: Toggles to displays additions about the items (infinities, categories, requirements)
 - `Automatic categories`: Disable to stop the detection of missing categories. Already detected categories will not be used until re-enabled. Clear the detected categories by seting the counter below to 0.


## Changelog
#### v2.0.0
- rewrote and cleaned the entire codebase.
- new category and infinity system.
- added currencies and grabbags categories.
- merged placeable and wand ammo.
- added informations to the item tooltip.
- added sub categories for most categories.
- reworked infite materials.
- can now detect consumable and explosives.
- Reworked tile duplication to use a chunk system and be induded in .twld file.
- more flexible config.
- added a few new config items.
- removed commands.
- reworked stack detection mecanism to be automatic.
- fixed a ton of bug.
- performance improvement.
- (probably more).
- updated desc and README
- en-US localization
<!-- - added mod icon -->

#### v1.3.1
 - Finaly released on the Mod Browser.
 - `Liquid` buckets can now be infinite.
 - Works with wires and actuators.

#### v1.3
 - Can now works with mods increasing max stacks and added related config items.
 - Furniture will now have the right category.

#### v1.2
 - Prevents item duppliation for infinite consumables.
 - Added new categories.

#### v1.1
 - Custom categories and values introduced.
 - Added `set`, `category` and `values` commands.

#### v1.0
 - Most consumables can now be infinite.