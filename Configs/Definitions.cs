using SPIC.Configs.Presets;
using System.Linq;
using Newtonsoft.Json;
using Terraria.ModLoader;

namespace SPIC.Configs;

public sealed class PresetDefinition : Definition<PresetDefinition> {
    public PresetDefinition() : base() { }
    public PresetDefinition(string key) : base(key) { }
    public PresetDefinition(string mod, string name) : base(mod, name) { }

    public override int Type => PresetLoader.GetPreset(Mod, Name) is null ? -1 : 1;
    public override bool AllowNull => true;

    public override string DisplayName => PresetLoader.GetPreset(Mod, Name)?.DisplayName.Value ?? base.DisplayName;
    public override string? Tooltip => PresetLoader.GetPreset(Mod, Name)?.GetLocalization("Tooltip").Value;

    [JsonIgnore] public IGroup? Filter { get; set; }
    
    public override PresetDefinition[] GetValues() => (Filter?.Presets ?? PresetLoader.Presets).Select(preset => new PresetDefinition(preset.Mod.Name, preset.Name)).ToArray();
}

public sealed class GroupDefinition : Definition<GroupDefinition> {
    public GroupDefinition() : base(){}
    public GroupDefinition(string fullName) : base(fullName) {}
    public GroupDefinition(IGroup group) : base(group.Mod.Name, group.Name) {}

    public override int Type => InfinityManager.GetGroup(Mod, Name) is null ? -1 : 1;

    public override string DisplayName => InfinityManager.GetGroup(Mod, Name)?.DisplayName.Value ?? base.DisplayName;

    public override GroupDefinition[] GetValues() => InfinityManager.Groups.Select(consumable => new GroupDefinition(consumable)).ToArray();
}

public sealed class InfinityDefinition : Definition<InfinityDefinition> {
    public InfinityDefinition() : base() { }
    public InfinityDefinition(string fullName) : base(fullName) { }
    public InfinityDefinition(string mod, string name) : base(mod, name) { }
    public InfinityDefinition(IInfinity infinity) : this(infinity.Mod.Name, infinity.Name) { }

    public override int Type => InfinityManager.GetInfinity(Mod, Name) is null ? -1 : 1;

    [JsonIgnore] public IGroup? Filter { get; set; }

    public override string DisplayName { get {
        IInfinity? infinity = InfinityManager.GetInfinity(Mod, Name);
        return infinity is not null ? $"[i:{infinity.IconType}] {infinity.DisplayName}" : base.DisplayName;
    } }

    public override InfinityDefinition[] GetValues() => (Filter?.Infinities ?? InfinityManager.Infinities).Select(intinity => new InfinityDefinition(intinity)).ToArray();
}

public sealed class DisplayDefinition : Definition<DisplayDefinition> {
    public DisplayDefinition() : base() { }
    public DisplayDefinition(string fullName) : base(fullName) { }
    public DisplayDefinition(string mod, string name) : base(mod, name) { }

    public override int Type => DisplayLoader.GetDisplay(Mod, Name) is null ? -1 : 1;

    [JsonIgnore] public IGroup? Filter { get; set; }

    public override string DisplayName { get {
        Display? display = DisplayLoader.GetDisplay(Mod, Name);
        return display is not null ? $"[i:{display.IconType}] {display.DisplayName}" : base.DisplayName;
    } }

    public override DisplayDefinition[] GetValues() => DisplayLoader.Displays.Select(display => new DisplayDefinition(display.Mod.Name, display.Name)).ToArray();
}