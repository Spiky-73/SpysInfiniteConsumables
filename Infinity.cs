using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SPIC;

public interface IInfinity : ILocalizedModType, ILoadable {
    IGroup Group { get; }
    bool Enabled { get; }

    Color Color { get; }
    int IconType { get; }
    LocalizedText DisplayName { get; }
}

public abstract class Infinity<TGroup, TConsumable> : ModType, IInfinity where TGroup : Group<TGroup, TConsumable> where TConsumable : notnull {
    protected sealed override void Register() {
        ModTypeLookup<Infinity<TGroup, TConsumable>>.Register(this);
        InfinityManager.Register(this);
    }
    public sealed override void SetupContent() => SetStaticDefaults();

    public override void Unload() {
        DisplayOverrides = null;
        InfinityOverrides = null;
        ExtraDisplays = null;
    }

    public abstract Requirement GetRequirement(TConsumable consumable, List<object> extras);
    
    public TGroup Group { get; internal set; } = null!;
    public virtual bool Enabled { get; set; } = true;

    public abstract int IconType { get; }
    public abstract Color Color { get; set; }
    public string LocalizationCategory => "Infinities";
    public virtual LocalizedText DisplayName => this.GetLocalization("DisplayName", PrettyPrintName);
    
    IGroup IInfinity.Group => Group;

    public event OverrideRequirementFn? RequirementOverrides;
    public void OverrideRequirement(TConsumable consumable, ref Requirement requirement, List<object> extras) => RequirementOverrides?.Invoke(consumable, ref requirement, extras);
    public delegate void OverrideRequirementFn(TConsumable consumable, ref Requirement requirement, List<object> extras);
    
    public event OverrideInfinityFn? InfinityOverrides;
    public void OverrideInfinity(Player player, TConsumable consumable, Requirement requirement, long count, ref long infinity, List<object> extras) => InfinityOverrides?.Invoke(player, consumable, requirement, count, ref infinity, extras);
    public delegate void OverrideInfinityFn(Player player, TConsumable consumable, Requirement requirement, long count, ref long infinity, List<object> extras);
    
    public event OverrideDisplayFn? DisplayOverrides;
    public void OverrideDisplay(Player player, Item item, TConsumable consumable, ref Requirement requirement, ref long count, List<object> extras, ref InfinityVisibility visibility) => DisplayOverrides?.Invoke(player, item, consumable, ref requirement, ref count, extras, ref visibility);
    public delegate void OverrideDisplayFn(Player player, Item item, TConsumable consumable, ref Requirement requirement, ref long count, List<object> extras, ref InfinityVisibility visibility);
    
    public event Func<TConsumable, TConsumable?>? ExtraDisplays;
    public IEnumerable<TConsumable> DisplayedValues(TConsumable consumable) {
        yield return consumable;
        if(ExtraDisplays is not null && Configs.InfinityDisplay.Instance.ShowAlternateDisplays) {
            foreach (Func<TConsumable, TConsumable?> extra in ExtraDisplays.GetInvocationList()) {
                TConsumable? displayed = extra(consumable);
                if (displayed is not null) yield return displayed;
            }
        }
    }
}

public abstract class Infinity<TGroup, TConsumable, TCategory> : Infinity<TGroup, TConsumable> where TGroup : Group<TGroup, TConsumable> where TConsumable : notnull where TCategory : struct, Enum {

    public override void Load() {
        base.Load();
        RequirementOverrides += CustomRequirement;
    }

    public override Requirement GetRequirement(TConsumable consumable, List<object> extras) {
        TCategory category = GetCategory(consumable);
        extras.Add(category);
        return GetRequirement(category);
    }

    private void CustomRequirement(TConsumable consumable, ref Requirement requirement, List<object> extras) {
        if (!Group.Config.HasCustomCategory(consumable, this, out TCategory category)) return;
        extras.Clear();
        extras.Add(category);
        requirement = GetRequirement(category);
    }

    public abstract TCategory GetCategory(TConsumable consumable);
    public abstract Requirement GetRequirement(TCategory category);
}

public abstract class InfinityStatic<TInfinity, TGroup, TConsumable> : Infinity<TGroup, TConsumable> where TInfinity : InfinityStatic<TInfinity, TGroup, TConsumable> where TGroup : Group<TGroup, TConsumable> where TConsumable : notnull {
    public override void SetStaticDefaults() => Instance = (TInfinity)this;
    public override void Unload() {
        Instance = null!;
        base.Unload();
    }

    public static TInfinity Instance = null!;
}
public abstract class InfinityStatic<TInfinity, TGroup, TConsumable, TCategory> : Infinity<TGroup, TConsumable, TCategory> where TInfinity : InfinityStatic<TInfinity, TGroup, TConsumable, TCategory> where TGroup : Group<TGroup, TConsumable> where TConsumable : notnull where TCategory : struct, Enum {
    public override void SetStaticDefaults() => Instance = (TInfinity)this;
    public override void Unload() {
        Instance = null!;
        base.Unload();
    }

    public static TInfinity Instance = null!;
}