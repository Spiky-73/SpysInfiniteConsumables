using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.ModLoader.Config;

namespace SPIC.ConsumableTypes;

public enum JourneySacrificeCategory {
    None = ConsumableType.NoCategory,
    OnlySacrifice,
    Consumable,
}
public class JourneySacrificeRequirements {
    [Label("$Mods.SPIC.Types.Journey.sacrifices")]
    public bool includeNonConsumable;
}

public class JourneySacrifice : ConsumableType<JourneySacrifice> {

    public override Mod Mod => SpysInfiniteConsumables.Instance;
    public override string LocalizedName => Language.GetTextValue("Mods.SPIC.Types.Journey.name");

    public override TooltipLine TooltipLine => TooltipHelper.AddedLine("JourneyResearch", Language.GetTextValue("Mods.SPIC.Types.Journey.lineValue"));

    public override int MaxStack(byte category) => 999;

    public override int Requirement(byte category) => -1;

    public override string LocalizedCategoryName(byte category) => ((JourneySacrificeCategory)category).ToString();
    

    public override JourneySacrificeRequirements CreateRequirements() => new();

    public override Microsoft.Xna.Framework.Color DefaultColor() => Colors.JourneyMode;

    public override byte GetCategory(Item item) {
        if(!CreativeItemSacrificesCatalog.Instance.TryGetSacrificeCountCapToUnlockInfiniteItems(item.type, out int value) || value == 0)
            return (byte)JourneySacrificeCategory.None;


        foreach(int typeID in InfinityManager.EnabledTypes()){
            if(typeID != UID && InfinityManager.GetRequirement(item, typeID) != NoRequirement) return (byte)JourneySacrificeCategory.Consumable;
        }
        return (byte)JourneySacrificeCategory.OnlySacrifice;
    }

    public override int GetRequirement(Item item) {
        JourneySacrificeCategory category = (JourneySacrificeCategory)item.GetCategory(UID);
        if(category == JourneySacrificeCategory.None
                || (category == JourneySacrificeCategory.OnlySacrifice && !((JourneySacrificeRequirements)ConfigRequirements).includeNonConsumable))
            return 0;

        return CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[item.type];
    }
}