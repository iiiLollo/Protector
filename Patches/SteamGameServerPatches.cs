using HarmonyLib;
using ProjectM.Auth;
using Protector.Services;
using Steamworks;


namespace Protector.Patches;


[HarmonyPatch(typeof(PlatformSystemBase))]
public class SteamGameServerPatches
{
    [HarmonyPostfix]
    [HarmonyPatch("BeginAuthSession")]
    public static void BeginAuthSession(object[] __args, ref object __result)
    {
        
        
       
        var steamId = (System.UInt64) __args[2];

        if (Core.IsUserEnabled(steamId))
        {
            GateKeeperService.Log.LogInfo($"FOUND IN WHITELISTED:[{steamId}] was allowed to login.");
        }
        else
        {
            GateKeeperService.Log.LogWarning($"NOT FOUND IN WHITELISTED:[{steamId}] was prevented to login.");
            __result = EBeginAuthSessionResult.k_EBeginAuthSessionResultInvalidTicket;
        }
		
		
    }
}
