using Terraria.Localization;
using Terraria.ModLoader;

namespace SPIC.Configs.Presets;

public abstract class Preset : ModType, ILocalizedModType {

    public string LocalizationCategory => "Configs.Presets";
    public virtual LocalizedText DisplayName => this.GetLocalization("DisplayName", new System.Func<string>(PrettyPrintName));

    public sealed override void SetupContent() => SetStaticDefaults();

    protected sealed override void Register() {
        ModTypeLookup<Preset>.Register(this);
        PresetLoader.Register(this);
    }

    public abstract int CriteriasCount { get; }

    public abstract bool MeetsCriterias(GroupConfig config);
    public abstract void ApplyCriterias(GroupConfig config);
    public virtual bool AppliesToGroup(IGroup group) => true;
}