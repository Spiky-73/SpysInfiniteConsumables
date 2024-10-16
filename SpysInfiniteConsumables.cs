using System.IO;
using System.Text.RegularExpressions;
using SPIC.Configs;
using SpikysLib;
using SpikysLib.Configs;
using SpikysLib.Localization;
using Terraria.ModLoader;

namespace SPIC;

public sealed class SpysInfiniteConsumables : Mod, IPreLoadMod {

    public static SpysInfiniteConsumables Instance { get; private set; } = null!;

    public void PreLoadMod() {
        Instance = this;
        LanguageHelper.ModifyKey += s => InfinityRoutingRegex.Replace(s, "Mods.SPIC.Infinities.$1$3");
        LanguageHelper.ModifyKey += s => DisplayRoutingRegex.Replace(s, "Mods.SPIC.Displays.$1$2");
    }
    public override void PostSetupContent() {
        InfinitySettings.Instance.Load();
        Configs.InfinityDisplay.Instance.Load();
    }

    public override void HandlePacket(BinaryReader reader, int whoAmI) => PacketHandlerLoader.Handle(this, reader, whoAmI);

    public override void Unload() {        
        InfinityLoader.Unload();
        Instance = null!;
    }

    public override object Call(params object[] args) {
        try {
            if (args[0].ToString() == "HasInfinite") {
                int playerID = (int)args[1];
                dynamic consumable = args[2];
                int consumed = (int)args[3];
                string[] parts = ((string)args[4]).Split('/', 2);
                string mod = parts[0];
                string name = parts[0];

                return InfinityManager.HasInfinite(Terraria.Main.player[playerID], consumable, consumed, (dynamic)InfinityLoader.GetInfinity(mod, name)!);
            }
        }catch(System.InvalidCastException cast){
            Logger.Error("The type of one of the arguments was incorrect", cast);
        }catch(System.Exception error){
            Logger.Error("The call failed", error);
        }
        return base.Call();
    }

    private static readonly Regex CamelCaseRegex = new("([A-Z])");
    private static readonly Regex InfinityRoutingRegex = new("""^Mods\.SPIC\.Configs\.(\w+)(?<!Infinity)(Category|Requirements|Display)(.*)$""");
    private static readonly Regex DisplayRoutingRegex = new("""^Mods\.SPIC\.Configs\.(\w+)Config(.*)$""");
}
