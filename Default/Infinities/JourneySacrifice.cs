using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using System.Collections.Generic;
using System.ComponentModel;
using SPIC.Default.Displays;
using SpikysLib;

namespace SPIC.Default.Infinities;

public enum JourneyCategory { NotConsumable, Consumable }

public sealed class JourneySacrificeRequirements {
    [DefaultValue(true)] public bool hideWhenResearched = true;
}

public sealed class JourneySacrifice : Infinity<Item>, ITooltipLineDisplay {

    public override Group<Item> Group => Items.Instance;
    public static JourneySacrifice Instance = null!;
    public static JourneySacrificeRequirements Config = null!;


    public override int IconType => ItemID.GoldBunny;
    public override bool Enabled { get; set; } = false;
    public override Color Color { get; set; } = Colors.JourneyMode;

    public override Requirement GetRequirement(Item item, List<object> extras) => new(item.ResearchUnlockCount);

    public (TooltipLine, TooltipLineID?) GetTooltipLine(Item item, int displayed) => (new(Mod, "JourneyResearch", this.GetLocalizedValue("TooltipLine")), TooltipLineID.JourneyResearch);

    public override void ModifyDisplay(Player player, Item item, Item consumable, ref Requirement requirement, ref long count, List<object> extras, ref InfinityVisibility visibility) {
        if(Main.CreativeMenu.GetItemByIndex(0).IsSimilar(item)) visibility = InfinityVisibility.Exclusive;
        else if(Config.hideWhenResearched && Main.LocalPlayerCreativeTracker.ItemSacrifices.GetSacrificeCount(item.type) == item.ResearchUnlockCount) visibility = InfinityVisibility.Hidden;
    }
}