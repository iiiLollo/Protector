using HarmonyLib;
using ProjectM;

namespace Protector.Patches;

[HarmonyPatch(typeof(SpawnTeamSystem_OnPersistenceLoad), nameof(SpawnTeamSystem_OnPersistenceLoad.OnUpdate))]
public static class InitializationPatch
{
	[HarmonyPostfix]
	public static void OnUpdatePostfix()
	{
		Core.Initialize();
	}
}
