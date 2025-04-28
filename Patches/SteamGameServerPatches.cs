using HarmonyLib;
using Protector.Services;
using Steamworks;


namespace Protector.Patches;


[HarmonyPatch(typeof(SteamGameServer))]
public class SteamGameServerPatches
{
    [HarmonyPostfix]
    [HarmonyPatch("BeginAuthSession")]
    public static void BeginAuthSession(object[] __args, ref object __result)
    {
        var steamId = (CSteamID) __args[2];

        if (Core.IsUserEnabled(steamId.m_SteamID))
        {
            GateKeeperService.Log.LogInfo($"FOUND IN WHITELISTED:[{steamId.m_SteamID}] was allowed to login.");
        }
        else
        {
            GateKeeperService.Log.LogWarning($"NOT FOUND IN WHITELISTED:[{steamId.m_SteamID}] was prevented to login.");
            __result = EBeginAuthSessionResult.k_EBeginAuthSessionResultInvalidTicket;
        }
		
		
    }
}
