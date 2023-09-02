namespace MuckConfigurePowerupDrops;
using HarmonyLib;

public class GetChestPriceMultiplierPatch
{
	[HarmonyPatch(typeof(GameSettings), "GetChestPriceMultiplier")]
	[HarmonyPrefix]
	private static bool ChestPriceMultiplier(GameSettings __instance, ref float __result)
	{
		switch (__instance.difficulty)
		{
			case GameSettings.Difficulty.Easy: __result = Plugin.EasyChestPriceDivisor; break;
			case GameSettings.Difficulty.Normal: __result = Plugin.NormalChestPriceDivisor; break;
			case GameSettings.Difficulty.Gamer: __result = Plugin.GamerChestPriceDivisor; break;
			default: __result = 5f; break;
		}
		return false;
	}
}
