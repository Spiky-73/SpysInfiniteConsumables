using System.Collections.Generic;
using SPIC.Configs;
using Terraria;

namespace SPIC.Infinities;

public class Currencies : Group<Currencies, int> {
    public override long CountConsumables(Player player, int consumable) => player.CountCurrency(consumable, true, true);

    public override string CountToString(int consumable, long count, InfinityDisplay.CountStyle style, bool rawValue = false) {
        if(rawValue && GetCategory(consumable, Shop.Instance) == ShopCategory.SingleCoin) return count.ToString();
        switch (style) {
        case InfinityDisplay.CountStyle.Sprite:
            List<KeyValuePair<int, long>> items = CurrencyHelper.CurrencyCountToItems(consumable, count);
            List<string> parts = new();
            foreach ((int t, long c) in items) parts.Add($"{c}[i:{t}]");
            return string.Join(" ", parts);
        case InfinityDisplay.CountStyle.Name or _:
        default:
            return CurrencyHelper.PriceText(consumable, count);
        }
    }

    public override int ToConsumable(Item item) => item.CurrencyType();
    public override Item ToItem(int consumable) => new(CurrencyHelper.LowestValueType(consumable));
    public override int GetType(int consumable) => consumable;

    public override int FromType(int type) => type;
}