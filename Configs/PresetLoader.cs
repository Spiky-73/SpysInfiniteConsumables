using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SPIC.Configs.Presets;

public static class PresetLoader {
    internal static void Register(Preset preset) => _presets.Add(preset);
    internal static void SetupPresets() {
        foreach (Preset preset in _presets) {
            foreach (IGroup group in InfinityManager.Groups) {
                if (preset.AppliesToGroup(group)) group.Add(preset);
            }
        }
    }
    internal static void Unload() => _presets.Clear();

    public static Preset? GetPreset(string mod, string name) => _presets.Find(p => p.Mod.Name == mod && p.Name == name);

    public static ReadOnlyCollection<Preset> Presets => _presets.AsReadOnly();

    private readonly static List<Preset> _presets = new();
}