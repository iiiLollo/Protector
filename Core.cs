using BepInEx.Logging;
using ProjectM;
using ProjectM.Scripting;
using Protector.Helpers;
using Protector.Services;
using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine.InputSystem.Utilities;

namespace Protector;

internal static class Core
{
    public static World Server { get; } = GetWorld("Server") ?? throw new System.Exception("There is no Server world (yet)...");
    public static EntityManager EntityManager { get; } = Server.EntityManager;
    public static ServerScriptMapper ServerScriptMapper { get; internal set; }
    public static ServerGameManager ServerGameManager => ServerScriptMapper.GetServerGameManager();
    public static PrefabCollectionSystem PrefabCollectionSystem { get; internal set; }
    public static ManualLogSource Log => Plugin.LogInstance;

    public static GateKeeperService gKs;


    static bool hasInitialized;
    public static void Initialize()
    {
        if (hasInitialized) return;

        Whitelist.Config(Plugin.GateKeeperWhitelistFile.Value);

        ServerScriptMapper = Server.GetExistingSystemManaged<ServerScriptMapper>();
        PrefabCollectionSystem = Server.GetExistingSystemManaged<PrefabCollectionSystem>();

        // Initialize utility services
        Log.LogInfo($"{MyPluginInfo.PLUGIN_NAME}[{MyPluginInfo.PLUGIN_VERSION}] core initialized!");

        gKs = new GateKeeperService();

        hasInitialized = true;
    }
    static World GetWorld(string name)
    {
        foreach (var world in World.s_AllWorlds)
        {
            if (world.Name == name)
            {
                return world;
            }
        }
        return null;
    }

    internal static bool IsUserEnabled(ulong m_SteamID)
    {
#if DEBUG
        Log.LogInfo($"New user logging in SteamID:{m_SteamID}");
#endif
        return gKs.IsUserEnabled(m_SteamID);


    }
}