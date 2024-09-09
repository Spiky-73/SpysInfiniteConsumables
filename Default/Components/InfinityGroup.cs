using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using SPIC.Configs;
using SpikysLib.Collections;
using SpikysLib.Configs;
using SpikysLib.Configs.UI;
using SpikysLib.DataStructures;
using Terraria.ModLoader.Config;

namespace SPIC.Default.Components;

public sealed class UsedInfinities : MultiChoice<int> {
    public UsedInfinities() : base() { }
    public UsedInfinities(int value) : base(value) { }

    [Choice] public Text All { get; set; } = new();
    [Choice, Range(1, 9999)] public int Used { get; set; } = 1;

    public override int Value {
        get => Choice == nameof(All) ? 0 : Used;
        set {
            if (value != 0) {
                Choice = nameof(Used);
                Used = value;
            } else Choice = nameof(All);
        }
    }

    public static implicit operator UsedInfinities(int used) => new(used);
}

public sealed class GroupConfig {
    // TODO presets
    public UsedInfinities UsedInfinities { get; set; } = 0;

    [CustomModConfigItem(typeof(DictionaryValuesElement)), KeyValueWrapper(typeof(InfinityConfigsWrapper))] public OrderedDictionary<InfinityDefinition, Toggle<Dictionary<string, object>>> Infinities { get; set; } = [];
}

public sealed class ClientGroupConfig {
    [CustomModConfigItem(typeof(DictionaryValuesElement)), KeyValueWrapper(typeof(InfinityClientConfigsWrapper))] public OrderedDictionary<InfinityDefinition, NestedValue<Color, Dictionary<string, object>>> Infinities { get; set; } = [];
}


public sealed class InfinityGroup<TConsumable> : Component<Infinity<TConsumable>>, IConfigurableComponents<GroupConfig>, IClientConfigurableComponents<ClientGroupConfig> {
    public override void Load() {
        Endpoints.UsedInfinities(this).Register(UsedInfinities);
        Endpoints.GetRequirement(Infinity).Register(GetRequirement);
    }
    
    public void RegisterChild(Group<TConsumable> infinity) {
        _infinities.Add(infinity);
        _orderedInfinities.Add(infinity);
    }

    private Optional<HashSet<Infinity<TConsumable>>> UsedInfinities(TConsumable consumable) {
        HashSet<Infinity<TConsumable>> usedInfinities = [];
        int max = InfinitySettings.Get(this).UsedInfinities;
        foreach (Group<TConsumable> infinity in _orderedInfinities) {
            if (!infinity.Infinity.IsEnabled()) continue;
            var endpoint = Endpoints.GetRequirement(infinity.Infinity);
            bool pastGroup = false;
            Optional<Requirement> v = new();
            foreach (var provider in endpoint.Providers) {
                if (pastGroup && (v = provider(consumable)).HasValue) break;
                if (provider == infinity.GetRequirement) pastGroup = true;
            }
            Requirement value = v.Value;
            foreach (var modifier in endpoint.Modifiers) modifier(consumable, ref value);
            if (value.IsNone) continue;
            usedInfinities.Add(infinity);
            if (usedInfinities.Count == max) break;
        }
        return usedInfinities;
    }

    private Optional<Requirement> GetRequirement(TConsumable consumable) {
        long count = 0;
        float multiplier = float.MaxValue;
        foreach (Infinity<TConsumable> infinity in InfinityManager.UsedInfinities(consumable, this)) {
            Requirement requirement = InfinityManager.GetRequirement(consumable, infinity);
            count = Math.Max(count, requirement.Count);
            multiplier = Math.Min(multiplier, requirement.Multiplier);
        }
        return new Requirement(count, multiplier);
    }

    void IConfigurableComponents<GroupConfig>.OnLoaded(GroupConfig config) {
        _orderedInfinities.Clear();
        List<IInfinity> toRemove = [];
        foreach (Infinity<TConsumable> infinity in _infinities) config.Infinities.GetOrAdd(new(infinity), InfinitySettings.DefaultConfig(infinity));
        foreach ((InfinityDefinition key, Toggle<Dictionary<string, object>> value) in config.Infinities) {
            IInfinity? i = key.Entity;
            if (i is null) continue;
            if (i is not Infinity<TConsumable> infinity || !_infinities.Contains(infinity)) {
                toRemove.Add(i);
                continue;
            }
            InfinitySettings.Instance.LoadInfinityConfig(infinity, value);
            _orderedInfinities.Add(infinity.GetComponent<Group<TConsumable>>());
        }
        foreach (var infinity in toRemove) config.Infinities.Remove(new(infinity));
    }
    void IClientConfigurableComponents<ClientGroupConfig>.OnLoaded(ClientGroupConfig config) {
        List<IInfinity> toRemove = [];
        foreach (Infinity<TConsumable> infinity in _infinities) config.Infinities.GetOrAdd(new(infinity), InfinityDisplays.DefaultConfig(infinity));
        foreach ((InfinityDefinition key, NestedValue<Color, Dictionary<string, object>> value) in config.Infinities) {
            IInfinity? i = key.Entity;
            if (i is null) continue;
            if (i is not Infinity<TConsumable> infinity || !_infinities.Contains(infinity)) {
                toRemove.Add(i);
                continue;
            }
            InfinityDisplays.Instance.LoadInfinityConfig(infinity, value);
        }
        foreach (var infinity in toRemove) config.Infinities.Remove(new(infinity));
    }

    public ReadOnlyCollection<Group<TConsumable>> Infinities => _orderedInfinities.AsReadOnly();
    private readonly List<Group<TConsumable>> _orderedInfinities = [];
    private readonly HashSet<Infinity<TConsumable>> _infinities = [];
}
