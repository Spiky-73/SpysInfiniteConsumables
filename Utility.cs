using Terraria;
using Terraria.ModLoader;

namespace SPIC {

    public struct NPCStats {
        public int boss, total;
    }

    public static class Utility {

        public static int NameToType(string name, bool noCaps = true) {
            string fullName = name.Replace("_", " ");
            if (noCaps) fullName = fullName.ToLower();

            for (int k = 0; k < ItemLoader.ItemCount; k++) {
                string testedName = noCaps ? Lang.GetItemNameValue(k).ToLower() : Lang.GetItemNameValue(k);
                if (fullName == testedName) return k;
            }

            throw new UsageException("Invalid Name" + name);
        }

        public static bool InChest(this Player player, out Item[] chest) => (chest = player.Chest()) is not null;
        public static Item[] Chest(this Player player) => player.chest switch {
            > -1 => Main.chest[player.chest].item,
            -2 => player.bank.item,
            -3 => player.bank2.item,
            -4 => player.bank3.item,
            -6 => player.bank4.item,
            _ => null
        };
        
        public static int CountItems(this Item[] container, int type, params int[] ignoreSots) {
            int total = 0;
            for (int i = 0; i < container.Length; i++) {
                if(System.Array.IndexOf(ignoreSots, i) == -1 && container[i].type == type)
                    total += container[i].stack;
            }
            return total;
        }
        
        public static int CountItems(this Player player, int type, bool includeChest = false) {
            int total = player.inventory.CountItems(type, 58);
            total += new Item[] { Main.mouseItem }.CountItems(type);
            if (!includeChest) return total;
            if(player.InChest(out Item[] chest)) total += chest.CountItems(type);
            if(SpysInfiniteConsumables.MagicStorageLoaded && InMagicStorage(out var heart)) total += heart.CountItems(type);
            return total;
        }
        [JITWhenModsEnabled("MagicStorage")]
        public static bool InMagicStorage(out MagicStorage.Components.TEStorageHeart heart) => (heart = MagicStorage.StoragePlayer.LocalPlayer.GetStorageHeart()) is not null;
        
        [JITWhenModsEnabled("MagicStorage")]
        public static int CountItems(this MagicStorage.Components.TEStorageHeart heart, int type){
            int count = 0;
            var storedItems = heart.GetStoredItems();
            foreach(Item i in storedItems)if(i.type == type) count += i.stack;
            return count;
        }

        public static void RemoveFromInventory(this Player player, int type, int count = 1) {
            foreach (Item i in player.inventory) {
                if(i.type != type) continue;
                if (i.stack < count) {
                    count -= i.stack;
                    i.TurnToAir();
                }
                else {
                    i.stack -= count;
                    return;
                }
            }
        }

        public static long CountCoins(this Item[] container, params int[] ignoreSlots) {
            long count = 0L;
            for (int i = 0; i < container.Length; i++) {
                if(System.Array.IndexOf(ignoreSlots,i) == -1 && container[i].IsACoin)
                    count += container[i].value / 5 * container[i].stack;
            }
            return count;
        }

        public static bool Placeable(this Item item) => item.createTile != -1 || item.createWall != -1;

        public static int WorldDifficulty() => Main.masterMode ? 2 : Main.expertMode ? 1 : 0;

        public static NPCStats GetNPCStats() {
            NPCStats stats = new();
            foreach (NPC npc in Main.npc) {
                if (!npc.active) continue;
                stats.total++;
                if (npc.boss) stats.boss++;
            }
            return stats;
        }

        public static int RequirementToItems(int infinity, int maxStack) => infinity switch {
            < 0 => -infinity * maxStack,
            _ => infinity
        };

        public static int CountItemsInWorld(){
            int i = 0;
            foreach(Item item in Main.item) if(!item.IsAir) i++;
            return i;
        }
    }
}