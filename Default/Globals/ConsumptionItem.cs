using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SPIC.Default.Infinities;

namespace SPIC.Default.Globals;

public sealed class ConsumptionItem : GlobalItem {

    public override bool ConsumeItem(Item item, Player player) {
        DetectionPlayer detectionPlayer = player.GetModPlayer<DetectionPlayer>();

        // LeftClick
        if (detectionPlayer.InItemCheck) {
            if (item != player.HeldItem) { // Wands
                return !player.HasInfinite(item, 1,
                    () => player.HeldItem.damage != 0 ? InfinityManager.SaveDetectedCategory(item, AmmoCategory.Special, Ammo.Instance): InfinityManager.SaveDetectedCategory(item, PlaceableCategory.Tile, Placeable.Instance),
                    Placeable.Instance, Ammo.Instance
                );
            }
            if(detectionPlayer.UsedCannon) { // Cannons
                return !player.HasInfinite(item, 1,
                    () => {
                        if (!InfinityManager.SaveDetectedCategory(item, AmmoCategory.Special, Ammo.Instance)) return false;
                        UsableCategory usableCategory = InfinityManager.GetCategory(item, Usable.Instance);
                        if (usableCategory == UsableCategory.Tool || usableCategory == UsableCategory.Unknown) InfinityManager.SaveDetectedCategory(item, UsableCategory.None, Usable.Instance);
                        return true;
                    },
                    Ammo.Instance
                );
            }
            if (Configs.InfinitySettings.Instance.DetectMissingCategories) detectionPlayer.TryDetectCategory(true);
            int tileTarget = Main.tile[Player.tileTargetX, Player.tileTargetY].TileType;
            if(tileTarget == TileID.Extractinator || tileTarget == TileID.ChlorophyteExtractinator) return !player.HasInfinite(item, 1, Usable.Instance, GrabBag.Instance);
            return !player.HasInfinite(item, 1, Usable.Instance, Placeable.Instance);

        } else if(detectionPlayer.InRightClick) {
            if (Configs.InfinitySettings.Instance.DetectMissingCategories) detectionPlayer.TryDetectCategory(true);
            return !player.HasInfinite(item, 1,
                () => InfinityManager.SaveDetectedCategory(item, GrabBagCategory.Container, GrabBag.Instance),
                GrabBag.Instance, Usable.Instance
            );
        }else { // Hotkey or special right click action
            if (Configs.InfinitySettings.Instance.DetectMissingCategories) detectionPlayer.TryDetectCategory(true);
            return !player.HasInfinite(item, 1,
                () => InfinityManager.SaveDetectedCategory(item, GrabBagCategory.Container, GrabBag.Instance)
                , Usable.Instance, GrabBag.Instance
            );
        }
    }

    public override bool CanBeConsumedAsAmmo(Item ammo, Item weapon, Player player) => !player.HasInfinite(ammo, 1, Ammo.Instance);

    public override bool? CanConsumeBait(Player player, Item bait) => !player.HasInfinite(bait, 1, Usable.Instance) ? null : false;

    public override bool CanResearch(Item item) {
        s_preResearchSacrifices = Main.LocalPlayerCreativeTracker.ItemSacrifices.GetSacrificeCount(item.type);
        return base.CanResearch(item);
    }

    public override void OnResearched(Item item, bool fullyResearched) {
        int researchCost = item.ResearchUnlockCount;
        int consumed = System.Math.Min(Utils.Clamp(researchCost - s_preResearchSacrifices, 0, researchCost), item.stack);
        if (Main.LocalPlayer.HasInfinite(item, 1, JourneySacrifice.Instance)) item.stack += consumed;
    }
    
    private static int s_preResearchSacrifices;
}
