namespace MuckTimeModifier;

using HarmonyLib;

public class TimeModifierPatch
{
	[HarmonyPatch(typeof(DayCycle), "Awake")]
	[HarmonyPostfix]
	private static void SetTimeSpeedAndNightDuration(DayCycle __instance)
	{
		float newNightDuration = __instance.nightDuration * Plugin.NightLengthMultiplier;
		float newTimeSpeed = __instance.timeSpeed / Plugin.TimeSpeedDivisor;
		Plugin.Log.LogInfo(string.Concat("Changing timeSpeed: ", __instance.timeSpeed, " / ", Plugin.TimeSpeedDivisor, " = ", newTimeSpeed));
		__instance.timeSpeed = newTimeSpeed;
		Plugin.Log.LogInfo(string.Concat("Changing nightDuration: ", __instance.nightDuration, " * ", Plugin.NightLengthMultiplier, " = ", newNightDuration));
		__instance.nightDuration = newNightDuration;
	}
	[HarmonyPatch(typeof(GameManager), "Start")]
	[HarmonyPostfix]
	private static void SetDayDuration(GameManager __instance)
	{
		// The day duration is only set to 1f as a default. The actual day duration is changed depending upon the difficulty, and is set by GameManager.
		// This is why we set the dayDuration here.
		float newDayDuration = DayCycle.dayDuration * Plugin.DayLengthMultiplier;
		Plugin.Log.LogInfo(string.Concat("Changing dayDuration: ", DayCycle.dayDuration, " * ", Plugin.DayLengthMultiplier, " = ", newDayDuration));
		DayCycle.dayDuration = newDayDuration;
	}
}