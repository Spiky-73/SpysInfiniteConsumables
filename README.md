# SpysInfiniteConsumables
**DISCLAIMER: THE MOD WAS NOT TESTED IN MULTIPLAYER!**
A simple Quality of life Mod giving the ability to make almost every consumable and tiles endless when you have enough of them.
Also include a few commands to change the setting in game and other features.

### Categories
Consumables are split in different categories, with each one having a different customizable requirement.
You can correct / change the category of items in the config / via commands (i.g. a boss summer been categorized as `other`).
0. `Blacklist`: Non consumables items
1. `Thrown`: Items of the throwing class - requires 1 stack to be infinite
1. `Recovery`: Healing and mana potions - requires 2 stacks
3. `Buff`: Potions providing a buff (swiftness, archery...) - requires 30 items
4. `Ammo`: Arrows, bullets, rockets and darts - - requires 4 stacks
5. `SpecialAmmo`: All other ammunitions - requires 1 stack
6. `Summoner`: Boss and event summoning items (does not work properly for modded items, you may want to set them up manualy) - requires 3 items
7. `Critter`: Also includes baits - requires 10 items
8. `Block` -  - requires 1 stack
9. `Furniture` - requires 10 items
10. `Wall` - requires 1 stack
11. `Liquid`: Empty or full buckets (items with `bucket` in their name) - requires 10 items
12. `Other`: Consumables that doesn't fit in any other category - requires 1 stack
13. `Custom`: You need to manually set items and requirements for this category in the config or via the commands

### Mod Compatibility
Every modded Items from any mod should work fine with the mod, however their category could be wrong (mostly for boss summoner).
Mods increasing mas stack also work, but they require a few steps to set up the mod correctly:
1. Disable any mod / config item increasing max stack. LEAVE CONTENT MODS ENABLE.
2. Initialize `Generate Stack list` in the config, shouldn't take more than a few seconds. (doesn't do any this if 1 is not done)
3. Revert step 1 (enable mods/config items)

### Commands
They are a few in-game commands to help changing categories:
- `/ID [name|type]`: Returns the `name` and `type` (id) of an item.
- `/SPIC category [name|type]`:  Return the category of an item.
- `/SPIC set <category> [name|type] [req(c=12)]`: Sets the category of an item. (with `req` as the requirement for custom categories).
- `/SPIC values`: return a list off all possible categories index and name

`[name|type]`: the name or type of an item. Type `^` or skip it to use the item you hold
`[req(c=12)]`: the value of the requirement for custom categories.
`<category>`: a consumable category, can be its index or name.

### Config
##### General
Controls key aspects of the mods.
##### Consumables and Tiles
For requirents, use a negative value for (`-X`) for items and a positive one stacks (`-5` => 5 items, `2` => 2 stacks).
##### Custom Categories & values
Add items to the lists to change their category.
The `Custom` category is not accessible from `Custom categories`, as all items in `Custom Requirement` will be in this category.
##### Mod Compatibility
See the `Mod Compatibility` section above to for how to set the list.

# Sidenote
Things I'd like to add to the mod for now:
- [ ] **Multiplayer / NET (if it doesn't work, and it probably won't)**
- [ ] find awork aroung for placable ammo (i.g. coins)
- [ ] redo category detection liquids & wiring
- [ ] redo dupplication prevention for critters & furnitures
- [ ] Make a TCF page

As my first ever mod, I'm quite happy about how it came out :).
 
# Version History
#### v1.3.1
- Finaly released on the Mod Browser
- `Liquid` buckets can now be infinite
- Works with wires and actuators

#### v1.3
- Can now works with mods increasing max stacks and added related config items
- Furniture will now have the right category

#### v1.2
- Prevents item duppliation for infinite consumables
- Added new categories

#### v1.1
- Custom categories and values introduced
- Added `set`, `category` and `values` commands

#### v1.0
- Most consumables can now be infinite