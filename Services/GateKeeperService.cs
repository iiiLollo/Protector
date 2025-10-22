using BepInEx.Logging;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using Il2CppInterop.Runtime;
using ProjectM.Network;
using ProjectM.Physics;
using Protector.Helpers;
using Stunlock.Network;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;


namespace Protector.Services;
internal class GateKeeperService
{


    private List<ulong> OldWhitelisted;
    private List<ulong> Whitelisted { get; set; }

    private FileSystemWatcher fileWatcher;

    readonly IgnorePhysicsDebugSystem tokenMonoBehaviour;

    public const int MAX_REPLY_LENGTH = 509;


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
            Log.LogInfo($"GateKeeper background filewatcher started");
        }

        tokenMonoBehaviour.StartCoroutine(KickLoop().WrapToIl2Cpp());
        Log.LogInfo($"GateKeeper background Whitelist monitor started");



    }



    // Background process to do some stuff periodically
    private IEnumerator UpdateLoop()
    {
        WaitForSeconds waitForSeconds = new(Plugin.GateKeeperUpdateInterval.Value * 60); // Convert minutes to seconds for update loop

        while (true)
        {
            yield return waitForSeconds;
#if DEBUG
            Log.LogInfo($"Triggered time based reload.");
#endif
            this.MarkForReload();


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
                    KickUser(user);
                }

    }

    public void LoadLists()
    {
        var result = Whitelist.Read();
        Whitelisted = (List<ulong>)result[0];
        OldWhitelisted = (List<ulong>)result[1];

    }

    public bool IsUserEnabled(System.UInt64 playerId)
    {
        if (Whitelisted.Contains(playerId))
        {
#if DEBUG
            Log.LogInfo($"[{playerId}] is whitelisted.");
#endif
            return true;
        }
        else
        {
#if DEBUG
            Log.LogInfo($"[{playerId}] is not whitelisted.");
#endif

        }
        return false;
    }


    public static void KickUser(User user)
    {
        EntityManager entityManager = Core.EntityManager;
        Entity entity = entityManager.CreateEntity(new ComponentType[3]
        {
      ComponentType.ReadOnly<NetworkEventType>(),
      ComponentType.ReadOnly<SendEventToUser>(),
      ComponentType.ReadOnly<KickEvent>()
        });
        StreamingServices.Write<KickEvent>(entity, new KickEvent()
        {
            PlatformId = user.PlatformId
        });
        StreamingServices.Write<SendEventToUser>(entity, new SendEventToUser()
        {
            UserIndex = user.Index
        });
        StreamingServices.Write<NetworkEventType>(entity, new NetworkEventType()
        {
            EventId = NetworkEvents.EventId_KickEvent,
            IsAdminEvent = false,
            IsDebugEvent = false
        });
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
            this.MarkForReload();


        };

        fileWatcher.EnableRaisingEvents = true;
    }

    public List<ulong> getWhitelisted()
    {
        return new List<ulong>(this.Whitelisted);
    }

    public void MarkForReload()
    {
        Whitelist.NeedsReload = true;
    }

}
