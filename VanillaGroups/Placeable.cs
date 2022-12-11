using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

using SPIC.ConsumableGroup;
using SPIC.Config;
using Terraria.Localization;

namespace SPIC.VanillaGroups;

public enum PlaceableCategory : byte {
    None = CategoryHelper.None,

    Block,
    Wall,
    Wiring,
    Torch,
    Ore,

    LightSource,
    Container,
    Functional,
    CraftingStation,
    Decoration,
    MusicBox,

    Mechanical,
    Liquid,
    Seed,
    Paint
}

public static class PlaceableExtension {
    public static bool IsCommonTile(this PlaceableCategory category) => category != PlaceableCategory.None && category < PlaceableCategory.LightSource;
    public static bool IsFurniture(this PlaceableCategory category) => category != PlaceableCategory.None && !category.IsCommonTile() && category < PlaceableCategory.Mechanical;
    public static bool IsMisc(this PlaceableCategory category) => category != PlaceableCategory.None && !category.IsCommonTile() && !category.IsFurniture();
}

public class PlaceableRequirements {
    [Label("$Mods.SPIC.Groups.Placeable.tiles")]
    public ItemCountWrapper Tiles = new(){Stacks=1};
    [Label("$Mods.SPIC.Groups.Placeable.ores")]
    public ItemCountWrapper Ores = new(){Items=499};
    [Label("$Mods.SPIC.Groups.Placeable.torches")]
    public ItemCountWrapper Torches = new(){Items=99};
    [Label("$Mods.SPIC.Groups.Placeable.furnitures")]
    public ItemCountWrapper Furnitures = new(99){Items=3};
    [Label("$Mods.SPIC.Groups.Placeable.mechanical")]
    public ItemCountWrapper Mechanical = new(){Items=3};
    [Label("$Mods.SPIC.Groups.Placeable.liquids")]
    public ItemCountWrapper Liquids = new(){Items=10};
    [Label("$Mods.SPIC.Groups.Placeable.seeds")]
    public ItemCountWrapper Seeds = new(99){Items=20};
    [Label("$Mods.SPIC.Groups.Placeable.paints")]
    public ItemCountWrapper Paints = new(){Stacks=1};
}

public class Placeable : ItemGroup<Placeable, PlaceableCategory>, IConfigurable<PlaceableRequirements>, IDetectable, IStandardAmmunition<Item> {

    public override Mod Mod => SpysInfiniteConsumables.Instance;
    public override int IconType => ItemID.ArchitectGizmoPack;

    public override bool DefaultsToOn => false;

    public override Requirement<ItemCount> Requirement(PlaceableCategory category) {
        return category switch {
            PlaceableCategory.Block or PlaceableCategory.Wall or PlaceableCategory.Wiring => new CountRequirement<ItemCount>(this.Settings().Tiles),
            PlaceableCategory.Torch => new CountRequirement<ItemCount>(this.Settings().Torches),
            PlaceableCategory.Ore => new CountRequirement<ItemCount>(this.Settings().Ores),

            PlaceableCategory.LightSource
                    or PlaceableCategory.Functional or PlaceableCategory.Decoration
                    or PlaceableCategory.Container or PlaceableCategory.CraftingStation
                => new CountRequirement<ItemCount>(this.Settings().Furnitures),
            PlaceableCategory.MusicBox => new CountRequirement<ItemCount>(new(this.Settings().Furnitures){MaxStack = 1}),
            PlaceableCategory.Liquid => new CountRequirement<ItemCount>(this.Settings().Liquids),
            PlaceableCategory.Mechanical => new CountRequirement<ItemCount>(this.Settings().Mechanical),
            PlaceableCategory.Seed => new CountRequirement<ItemCount>(this.Settings().Seeds),
            PlaceableCategory.Paint => new CountRequirement<ItemCount>(this.Settings().Paints),
            PlaceableCategory.None or _ => new NoRequirement<ItemCount>(),
        };
    }
    
    public override PlaceableCategory GetCategory(Item item) => GetCategory(item, false);
    public static PlaceableCategory GetCategory(Item item, bool wand) {


        switch (item.type) {
        case ItemID.Hellstone or ItemID.DemoniteOre or ItemID.CrimtaneOre: return PlaceableCategory.Ore; // Main.tileSpelunker[tileType] == false
        }

        if (_wandAmmos.TryGetValue(item.type, out int wandType)) return GetCategory(new(wandType), true);
        if (item.paint != 0) return PlaceableCategory.Paint;
        if(item.XMasDeco()) return PlaceableCategory.Decoration;
        if (item.FitsAmmoSlot() && item.mech) return PlaceableCategory.Wiring;

        if (!wand && (!item.consumable || item.useStyle == ItemUseStyleID.None)) return PlaceableCategory.None;

        if (item.createTile != -1) {

            int tileType = item.createTile;
            if (item.accessory) return PlaceableCategory.MusicBox;
            if (TileID.Sets.Platforms[tileType]) return PlaceableCategory.Block;

            if (Main.tileAlch[tileType] || TileID.Sets.CommonSapling[tileType] || TileID.Sets.Grass[tileType] || TileID.Sets.GrassSpecial[tileType]) return PlaceableCategory.Seed;
            if (Main.tileContainer[tileType]) return PlaceableCategory.Container;

            if (item.mech) return PlaceableCategory.Mechanical;

            if (Main.tileFrameImportant[tileType]) {
                bool GoodTile(int t) => t == tileType;

                if (TileID.Sets.Torch[tileType]) return PlaceableCategory.Torch;
                if (System.Array.Exists(TileID.Sets.RoomNeeds.CountsAsTorch, GoodTile)) return PlaceableCategory.LightSource;

                if (System.Array.Exists(TileID.Sets.RoomNeeds.CountsAsChair, GoodTile) || System.Array.Exists(TileID.Sets.RoomNeeds.CountsAsDoor, GoodTile) || System.Array.Exists(TileID.Sets.RoomNeeds.CountsAsTable, GoodTile))
                    return PlaceableCategory.Functional;

                if (Systems.InfiniteRecipe.CraftingStations.Contains(tileType)) return PlaceableCategory.CraftingStation;

                if (TileID.Sets.HasOutlines[tileType]) return PlaceableCategory.Functional;

                return PlaceableCategory.Decoration;
            }

            if (Main.tileSpelunker[tileType]) return PlaceableCategory.Ore;

            return PlaceableCategory.Block;
        }
        if (item.createWall != -1) return PlaceableCategory.Wall;

        return PlaceableCategory.None;
    }

    public override Microsoft.Xna.Framework.Color DefaultColor => Colors.RarityAmber;
    public override TooltipLine TooltipLine => TooltipHelper.AddedLine("Placeable", Lang.tip[33].Value);

    public enum WandType {
        None,
        Tile,
        Wire,
        PaintBrush,
        PaintRoller
    }

    public TooltipLine WeaponLine(Item weapon, Item ammo) => GetWandType(weapon) switch {
        WandType.Tile => TooltipHelper.AddedLine($"WandConsumes", Language.GetTextValue("Mods.SPIC.ItemTooltip.weaponAmmo", ammo.Name)),
        WandType.Wire => TooltipHelper.AddedLine($"Tooltip0", Language.GetTextValue("Mods.SPIC.ItemTooltip.weaponAmmo", ammo.Name)),
        WandType.None or WandType.PaintBrush or WandType.PaintRoller or _=> TooltipHelper.AddedLine($"PaintConsumes", Language.GetTextValue("Mods.SPIC.ItemTooltip.weaponAmmo", ammo.Name))
    };

    public static WandType GetWandType(Item item) => item switch { { tileWand: not -1 } => WandType.Tile,
        { type: ItemID.Wrench or ItemID.BlueWrench or ItemID.GreenWrench or ItemID.YellowWrench or ItemID.MulticolorWrench or ItemID.WireKite } => WandType.Wire,
        { type: ItemID.Paintbrush or ItemID.SpectrePaintbrush } => WandType.PaintBrush,
        { type: ItemID.PaintRoller or ItemID.SpectrePaintRoller } => WandType.PaintRoller,
        _ => WandType.None
    };

    public bool HasAmmo(Player player, Item wand, [MaybeNullWhen(false)] out Item tile){
        tile = GetWandType(wand) switch {
            WandType.Tile => System.Array.Find(player.inventory, item => item.type == wand.tileWand),
            WandType.Wire => System.Array.Find(player.inventory, item => item.type == ItemID.Wire),
            WandType.PaintBrush or WandType.PaintRoller => player.PickPaint(),
            WandType.None or _ => null
        };
        return tile is not null;
    }

    private static readonly Dictionary<int, int> _wandAmmos = new(); // ammoType, wandType
    internal static void ClearWandAmmos() => _wandAmmos.Clear();
    public static void RegisterWand(Item wand) {
        if(Instance.GetCategory(new(wand.tileWand)) == PlaceableCategory.None) _wandAmmos.TryAdd(wand.tileWand, wand.type);
    }


    // public static bool CanNoDuplicationWork(Item item = null) => Main.netMode == NetmodeID.SinglePlayer && (item == null || !AlwaysDrop(item));

    // //- TODO Update as tml updates
    // // Wires and actuators
    // // Wall XxX
    // // 2x5, 3x5, 3x6
    // // Sunflower, Gnome
    // // Chest
    // // drop in 2x1 bug : num instead of num3
    // public static bool AlwaysDrop(Item item) {
    //     if (item.type == ItemID.Wire || item.type == ItemID.Actuator) return true;
    //     if ((item.createTile < TileID.Dirt && item.createWall != WallID.None) || item.createTile == TileID.TallGateClosed) return false;
    //     if (item.createTile == TileID.GardenGnome || item.createTile == TileID.Sunflower || TileID.Sets.BasicChest[item.createTile]) return true;

    //     TileObjectData data = TileObjectData.GetTileData(item.createTile, item.placeStyle);

    //     // No data or 1x1 moditem
    //     if (data == null || (item.ModItem != null && data.Width > 1 && data.Height > 1)) return false;
    //     if ((data.Width == 2 && data.Height == 1) || (data.Width == 2 && data.Height == 5) || (data.Width == 3 && data.Height == 4) || (data.Width == 3 && data.Height == 5) || (data.Width == 3 && data.Height == 6)) return true;

    //     return data.AnchorWall || (TileID.Sets.HasOutlines[item.createTile] && System.Array.Exists(TileID.Sets.RoomNeeds.CountsAsDoor, t => t == item.createTile));
    // }
}