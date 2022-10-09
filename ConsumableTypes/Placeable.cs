using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace SPIC.ConsumableTypes;

public enum PlaceableCategory : byte {
    None = Category.None,

    Block,
    Wall,
    Wiring,
    Torch,
    Ore,
    Gem,

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
    [Label("$Mods.SPIC.Types.Placeable.tiles")]
    public Configs.Requirement Tiles = -1;
    [Label("$Mods.SPIC.Types.Placeable.ores")]
    public Configs.Requirement Ores = 499;
    [Label("$Mods.SPIC.Types.Placeable.torches")]
    public Configs.Requirement Torches = 99;
    [Label("$Mods.SPIC.Types.Placeable.furnitures")]
    public Configs.Requirement Furnitures = 3;
    [Label("$Mods.SPIC.Types.Placeable.mechanical")]
    public Configs.Requirement Mechanical = 3;
    [Label("$Mods.SPIC.Types.Placeable.liquids")]
    public Configs.Requirement Liquids = 10;
    [Label("$Mods.SPIC.Types.Placeable.seeds")]
    public Configs.Requirement Seeds = 20;
    [Label("$Mods.SPIC.Types.Placeable.paints")]
    public Configs.Requirement Paints = -1;
}

public class Placeable : ConsumableType<Placeable>, IStandardConsumableType<PlaceableCategory, PlaceableRequirements>, IDefaultAmmunition, ICustomizable, IDetectable {

    public override Mod Mod => SpysInfiniteConsumables.Instance;
    public override int IconType => ItemID.ArchitectGizmoPack;

    public bool DefaultsToOn => false;
    public PlaceableRequirements Settings { get; set; }

    public int MaxStack(PlaceableCategory category) => category switch {
        PlaceableCategory.Block => 999,
        PlaceableCategory.Wall => 999,
        PlaceableCategory.Torch => 999,
        PlaceableCategory.Ore => 999,
        PlaceableCategory.Gem => 999,
        PlaceableCategory.Wiring => 999,

        PlaceableCategory.LightSource => 99,
        PlaceableCategory.Container => 99,
        PlaceableCategory.CraftingStation => 99,
        PlaceableCategory.Functional => 99,
        PlaceableCategory.Decoration => 99,
        PlaceableCategory.MusicBox => 1,

        PlaceableCategory.Mechanical => 999,
        PlaceableCategory.Liquid => 99,
        PlaceableCategory.Seed => 99,

        PlaceableCategory.Paint => 999,

        PlaceableCategory.None or _ => 999,
    };
    public int Requirement(PlaceableCategory category) {
        return category switch {
            PlaceableCategory.Block or PlaceableCategory.Wall or PlaceableCategory.Wiring => Settings.Tiles,
            PlaceableCategory.Torch => Settings.Torches,
            PlaceableCategory.Ore => Settings.Ores,

            PlaceableCategory.LightSource or PlaceableCategory.MusicBox
                    or PlaceableCategory.Functional or PlaceableCategory.Decoration
                    or PlaceableCategory.Container or PlaceableCategory.CraftingStation
                => Settings.Furnitures,

            PlaceableCategory.Liquid => Settings.Liquids,
            PlaceableCategory.Mechanical => Settings.Mechanical,
            PlaceableCategory.Seed => Settings.Seeds,
            PlaceableCategory.Paint => Settings.Paints,
            PlaceableCategory.None or _ => IConsumableType.NoRequirement,
        };
    }

    public PlaceableCategory GetCategory(Item item) {
        if (!(item.consumable && item.useStyle != ItemUseStyleID.None) && item.paint == 0 && !s_ammos.ContainsKey(item.type) && !(item.FitsAmmoSlot() && item.mech))
            return PlaceableCategory.None;
        return GetCategory_NoCheck(item);
    }

    private static PlaceableCategory GetCategory_NoCheck(Item item) {

        if (item.paint != 0) return PlaceableCategory.Paint;

        switch (item.type) {
        case ItemID.Hellstone: return PlaceableCategory.Ore;
        }

        if (item.createTile != -1) {

            int tileType = item.createTile;
            if (item.accessory) return PlaceableCategory.MusicBox;
            if (TileID.Sets.Platforms[tileType]) return PlaceableCategory.Block;

            if (Main.tileAlch[tileType] || TileID.Sets.TreeSapling[tileType] || TileID.Sets.Grass[tileType]) return PlaceableCategory.Seed;
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

        if(item.FitsAmmoSlot() && item.mech) return PlaceableCategory.Wiring;
        if (s_ammos.TryGetValue(item.type, out PlaceableCategory category)) return category;

        return PlaceableCategory.None;
    }

    public Microsoft.Xna.Framework.Color DefaultColor => Colors.RarityAmber;
    public TooltipLine TooltipLine => TooltipHelper.AddedLine("Placeable", Lang.tip[33].Value);

    public enum WandType {
        None,
        Tile,
        Wire,
        PaintBrush,
        PaintRoller
    }

    public static WandType GetWandType(Item item) => item switch { { tileWand: not -1 } => WandType.Tile,
        { type: ItemID.Wrench or ItemID.BlueWrench or ItemID.GreenWrench or ItemID.YellowWrench or ItemID.MulticolorWrench or ItemID.WireKite } => WandType.Wire,
        { type: ItemID.Paintbrush or ItemID.SpectrePaintbrush } => WandType.PaintBrush,
        { type: ItemID.PaintRoller or ItemID.SpectrePaintRoller } => WandType.PaintRoller,
        _ => WandType.None
    };

    public bool ConsumesAmmo(Item item) => GetWandType(item) != WandType.None;
    public Item GetAmmo(Player player, Item wand) => GetWandType(wand) switch {
        WandType.Tile => System.Array.Find(player.inventory, item => item.type == wand.tileWand),
        WandType.Wire => System.Array.Find(player.inventory, item => item.type == ItemID.Wire),
        WandType.PaintBrush or WandType.PaintRoller => player.PickPaint(),
        WandType.None or _ => null
    };

    private static readonly Dictionary<int, PlaceableCategory> s_ammos = new(); // type, category (ammo)
    internal static void ClearWandAmmos() => s_ammos.Clear();
    public static void RegisterWand(Item wand) => s_ammos.TryAdd(wand.tileWand, GetCategory_NoCheck(wand));

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
