namespace MuckDontDestroyNeighbours;

using HarmonyLib;

public class Patch
{
	[HarmonyPatch(typeof(BuildDestruction), "OnDestroy")]
	[HarmonyPrefix]
	public static bool OnDestroy(BuildDestruction __instance)
	{
		__instance.destroyed = true;
		return false;
	}
}
