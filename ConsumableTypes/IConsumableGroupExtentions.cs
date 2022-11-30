using System.Diagnostics.CodeAnalysis;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace SPIC.ConsumableGroup;

public interface ICategory : IConsumableGroup {
    Category GetCategory(object consumable);
    IRequirement Requirement(Category category);
}
public interface ICategory<TConsumable> : IConsumableGroup<TConsumable>, ICategory where TConsumable : notnull {
    Category ICategory.GetCategory(object consumable) => GetCategory((TConsumable)consumable);
    IRequirement ICategory.Requirement(Category category) => Requirement(category);

    new IRequirement Requirement(Category category);
    Category GetCategory(TConsumable consumable);
}
public interface ICategory<TConsumable, TCategory> : ICategory<TConsumable> where TConsumable : notnull where TCategory : System.Enum {
    Category ICategory<TConsumable>.GetCategory(TConsumable consumable) => GetCategory(consumable);
    IRequirement ICategory<TConsumable>.Requirement(Category category) => Requirement((TCategory)category);

    new TCategory GetCategory(TConsumable consumable);
    IRequirement Requirement(TCategory category);
}

public interface IAlternateDisplay {
    bool HasAlternate(Player player, object consumable, [MaybeNullWhen(false)] out object alt);
    TooltipLine AlternateTooltipLine(object consumable, object alternate);

}
public interface IAlternateDisplay<TConsumable> : IAlternateDisplay where TConsumable : notnull{
    bool IAlternateDisplay.HasAlternate(Player player, object consumable, [MaybeNullWhen(false)] out object alt) {
        if(HasAlternate(player, (TConsumable)consumable, out TConsumable? t)){
            alt = t;
            return true;
        }
        alt = null;
        return false;
    }
    TooltipLine IAlternateDisplay.AlternateTooltipLine(object consumable, object alternate) => AlternateTooltipLine((TConsumable)consumable, (TConsumable)alternate);

    TooltipLine AlternateTooltipLine(TConsumable consumable, TConsumable alternate);
    bool HasAlternate(Player player, TConsumable consumable, [MaybeNullWhen(false)] out TConsumable alt);
}

public interface IToggleable : IConsumableGroup {
    bool DefaultsToOn { get; }
    bool Enabled => UID > 0 ? (bool)Config.RequirementSettings.Instance.EnabledGroups[new Config.ConsumableTypeDefinition(UID)]! : Config.RequirementSettings.Instance.EnabledGlobals[new(UID)];
}

public interface IColorable : IConsumableGroup {
    Color DefaultColor { get; }
    Color Color => Config.InfinityDisplay.Instance.Colors[this.ToDefinition()];
}

public interface IConfigurable : IConsumableGroup {
    object Settings { get; internal set; }
    System.Type SettingsType { get; }
}
public interface IConfigurable<TSettings> : IConfigurable where TSettings : notnull, new() {
    System.Type IConfigurable.SettingsType => typeof(TSettings);
    object IConfigurable.Settings { get => Settings; set => Settings = (TSettings)value; }
    new TSettings Settings { get; set; }
}

public interface IDetectable : IConsumableGroup { }