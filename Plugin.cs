using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using ProjectM.CodeGeneration;
using System.IO;
using VampireCommandFramework;

namespace Protector;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("gg.deca.VampireCommandFramework")]
public class Plugin : BasePlugin
{
    Harmony _harmony;

    public static readonly string RootConfigPath = Path.Combine(Paths.ConfigPath, MyPluginInfo.PLUGIN_GUID);
    internal static Plugin Instance { get; private set; }
    public static ManualLogSource LogInstance => Instance.Log;

    public static ConfigEntry<bool> GateKeeperKickPlayer;

    public static ConfigEntry<bool> GateKeeperFileWatcherEnabled;

    
    public static ConfigEntry<int> GateKeeperUpdateInterval;

    public static ConfigEntry<bool> GateKeeperUpdateEnabled;

    public static ConfigEntry<string> GateKeeperWhitelistFile;


    public override void Load()
    {
        Instance = this;
        // Plugin startup logic
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} version {MyPluginInfo.PLUGIN_VERSION} is loading!");
        InitConfig();
        // Harmony patching
        _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        _harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());

        // Register all commands in the assembly with VCF
        CommandRegistry.RegisterAll();
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} version {MyPluginInfo.PLUGIN_VERSION} is loaded!");
    }

    static void InitConfig()
    {

        CreateDirectories(RootConfigPath);

        GateKeeperFileWatcherEnabled = InitConfigEntry("Config", "GateKeeperFileWatcherEnabled", true, "Enable or disable hot monitor of configuration file.");
        GateKeeperUpdateInterval = InitConfigEntry("Config", "GateKeeperUpdateInterval", 9999, "Delay for background processor in minutes.");
        GateKeeperUpdateEnabled = InitConfigEntry("Config", "GateKeeperUpdateEnabled", false, "Enable background processor.");
        GateKeeperKickPlayer = InitConfigEntry("Config", "GateKeeperKickPlayer", true, "Enable kick for not whitelisted players.");
        GateKeeperWhitelistFile =  InitConfigEntry("Config", "GateKeeperWhitelistFile", Path.Combine(RootConfigPath, "WhiteList.txt"), "Path to the WhiteList file.");

    }

        static ConfigEntry<T> InitConfigEntry<T>(string section, string key, T defaultValue, string description)
        {
            // Bind the configuration entry and get its value
            var entry = Instance.Config.Bind(section, key, defaultValue, description);

            // Check if the key exists in the configuration file and retrieve its current value
            var configFile = Path.Combine(Paths.ConfigPath, MyPluginInfo.PLUGIN_GUID + ".cfg");
            if (File.Exists(configFile))
            {
                var config = new ConfigFile(configFile, true);
                if (config.TryGetEntry(section, key, out ConfigEntry<T> existingEntry))
                {
                    // If the entry exists, update the value to the existing value
                    entry.Value = existingEntry.Value;
                }
            }
            return entry;
        }

    public override bool Unload()
    {
        CommandRegistry.UnregisterAssembly();
        _harmony?.UnpatchSelf();
        return true;
    }

    static void CreateDirectories(string path)
    {
        if (!Directory.Exists(path))
        {
            LogInstance.LogWarning(
                $"No configuration folder found at [{path}], creating it.");
            Directory.CreateDirectory(path);
        }
        else
        {
            LogInstance.LogInfo(
                $"Configuration folder found at [{path}].");

        }
    }

    // // Uncomment for example commmand or delete

    // /// <summary> 
    // /// Example VCF command that demonstrated default values and primitive types
    // /// Visit https://github.com/decaprime/VampireCommandFramework for more info 
    // /// </summary>
    // /// <remarks>
    // /// How you could call this command from chat:
    // ///
    // /// .protector-example "some quoted string" 1 1.5
    // /// .protector-example boop 21232
    // /// .protector-example boop-boop
    // ///</remarks>
    // [Command("protector-example", description: "Example command from protector", adminOnly: true)]
    // public void ExampleCommand(ICommandContext ctx, string someString, int num = 5, float num2 = 1.5f)
    // { 
    //     ctx.Reply($"You passed in {someString} and {num} and {num2}");
    // }
}
