using SPIC.Configs;
using SPIC.Configs.Presets;
using Terraria.ModLoader;


namespace SPIC;

public class SpysInfiniteConsumables : Mod {

    public static SpysInfiniteConsumables Instance { get; private set; } = null!;

    public override void Load() {
        Instance = this;

        InfinityManager.Reset();
        
        VanillaGroups.Placeable.ClearWandAmmos();
        InfinityManager.ClearCache();

        VanillaGroups.Ammo.Register();
        VanillaGroups.Usable.Register();
        VanillaGroups.Placeable.Register();
        VanillaGroups.GrabBag.Register();
        VanillaGroups.Material.Register();
        VanillaGroups.JourneySacrifice.Register();

        VanillaGroups.Currency.RegisterAsGlobal();
        VanillaGroups.Mixed.RegisterAsGlobal();
    }

    public override void PostSetupContent() {
        CurrencyHelper.GetCurrencies();
        CategoryDetection.Instance.LoadConfig();
        InfinityDisplay.Instance.LoadConfig();
    }

    public override void Unload() {
        VanillaGroups.Placeable.ClearWandAmmos();
        CurrencyHelper.ClearCurrencies();
        InfinityManager.Reset();
        PresetLoader.Unload();
        Instance = null!;
    }

    public override object Call(params object[] args) {
        try {
            if (args[0].ToString() == "HasInfinite") {
                int playerID = (int)args[1];
                dynamic consumable = args[2];
                int consumed = (int)args[3];
                string fullName = (string)args[4];

                return InfinityManager.HasInfinite(Terraria.Main.player[playerID], consumable, consumed, InfinityManager.ConsumableGroup(fullName)!);
            }
        }catch(System.InvalidCastException cast){
            Logger.Error("The type of one of the arguments was incorect", cast);
        }catch(System.Exception error){
            Logger.Error("The call failled", error);
        }
        return base.Call();
    }

    public readonly static string[] Versions = new string[] { "2.0.0", "2.1.0", "2.2.0", "2.2.0.1", "2.2.1" };

    private static readonly System.WeakReference<SpysInfiniteConsumables> s_instance = new(null!);
}

