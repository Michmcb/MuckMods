namespace MuckTimeModifier;

using HarmonyLib;

public class BossDayPatch
{
	[HarmonyPatch(typeof(GameSettings), "BossDay")]
	[HarmonyPrefix]
	private static bool BossDay(GameSettings __instance, ref int __result)
	{
		switch (__instance.difficulty)
		{
			case GameSettings.Difficulty.Easy: __result = Plugin.EasyBossDay; break;
			case GameSettings.Difficulty.Normal: __result = Plugin.NormalBossDay; break;
			case GameSettings.Difficulty.Gamer: __result = Plugin.GamerBossDay; break;
			default: __result = 5; break;
		}
		return false;
	}
}