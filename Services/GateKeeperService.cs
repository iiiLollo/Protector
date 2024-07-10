﻿using BepInEx.Logging;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using ProjectM;
using ProjectM.Network;
using ProjectM.Physics;
using Protector.Helpers;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Unity.Collections;
using Unity.Entities;
using Unity.Services.Authentication.Internal;
using UnityEngine;

namespace Protector.Services;
internal class GateKeeperService
{


    private List<ulong> OldWhitelisted;
    private List<ulong> Whitelisted;

    private FileSystemWatcher fileWatcher;

    readonly IgnorePhysicsDebugSystem tokenMonoBehaviour;

    

    public static ManualLogSource Log => Plugin.LogInstance;

    public GateKeeperService()
    {

        this.LoadLists();
        tokenMonoBehaviour = (new GameObject("GateKeeperService")).AddComponent<IgnorePhysicsDebugSystem>();

        if (Plugin.GateKeeperUpdateEnabled.Value)
        {
            tokenMonoBehaviour.StartCoroutine(UpdateLoop().WrapToIl2Cpp());
            Log.LogInfo($"GateKeeper background updater started");
        }
        if (Plugin.GateKeeperFileWatcherEnabled.Value)
        {
            initializeFileWatcher();
            tokenMonoBehaviour.StartCoroutine(KickLoop().WrapToIl2Cpp());
            Log.LogInfo($"GateKeeper background filewatcher started");

        }
    }

    // Background process to do some stuff periodically
    static IEnumerator UpdateLoop()
    {
        WaitForSeconds waitForSeconds = new(Plugin.GateKeeperUpdateInterval.Value * 60); // Convert minutes to seconds for update loop

        while (true)
        {
            yield return waitForSeconds;


        }
    }

    // Background process to do some stuff periodically
    private IEnumerator KickLoop()
    {
        while (true)
        {
            if (Whitelist.NeedsReload)
            {
                Whitelist.NeedsReload = false;
                var result = Whitelist.Read();
                Whitelisted = (List<ulong>)result[0];
                OldWhitelisted = (List<ulong>)result[1];
                KickUsers(OldWhitelisted);
            }
            
            yield return 0;
        }
    }

    /// <summary>
    ///     This function watch is KickPlayer is enabled, if so, then we're
    ///     looping through the oldWhitelisted parameter and kick player if necessary.
    /// </summary>
    /// <param name="oldWhitelisted">The list containing all old whitelisted SteamIDs.</param>
    public static void KickUsers(List<ulong> oldWhitelisted)
    {
        
            if (!Plugin.GateKeeperKickPlayer.Value || oldWhitelisted == null || oldWhitelisted.Count == 0)
            {
                return;
            }
            

            Unity.Entities.EntityManager WorldEntityManager = Core.Server.EntityManager;
            var query = WorldEntityManager.CreateEntityQuery(ComponentType.ReadOnly<User>());
            var entities = query.ToEntityArray(Allocator.Temp);

            foreach (var steamId in oldWhitelisted)
                if (!Whitelist.Get().Contains(steamId))
                    foreach (var entity in entities)
                    {
                        var user = WorldEntityManager.GetComponentData<User>(entity);
                        if (user.PlatformId != steamId || !user.IsConnected) continue;
                        KickUser(steamId);
                    }
                
    }

    public void LoadLists()
    {
        var result = Whitelist.Read();
        Whitelisted = (List<ulong>)result[0];
        OldWhitelisted = (List<ulong>)result[1];

    }

    public bool IsUserEnabled(ulong playerId)
    {
        if (Whitelisted.Contains(playerId))
        {
#if DEBUG
            Log.LogInfo($"[{playerId}] is whitelisted.");
#endif
            return true;
        }
        return false;
    }

    private static void KickUser(ulong playerId)
    {
        ServerBootstrapSystem serverBootstrap = Core.Server.GetExistingSystemManaged<ServerBootstrapSystem>();
        serverBootstrap.Kick(playerId, Stunlock.Network.ConnectionStatusChangeReason.BlockedUserOnServer,true);
        Log.LogInfo($"[{playerId}] was kicked because it was removed from the whitelist.");
    }


    private void initializeFileWatcher()
    {

        fileWatcher = new FileSystemWatcher(Path.GetDirectoryName(Plugin.GateKeeperWhitelistFile.Value))
        {
            NotifyFilter = NotifyFilters.LastWrite,
            Filter = Path.GetFileName(Plugin.GateKeeperWhitelistFile.Value)
        };

        fileWatcher.Changed += (_, _) =>
        {
#if DEBUG
            Log.LogInfo($"Whitelist file has changed!");
#endif
            Whitelist.NeedsReload=true;


        };

        fileWatcher.EnableRaisingEvents = true;
    }

}
