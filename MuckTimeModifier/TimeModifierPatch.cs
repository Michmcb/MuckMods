namespace MuckTimeModifier;

using HarmonyLib;

public class TimeModifierPatch
{
	//public static float ApproxUnitTimeInSeconds = 0f;
	[HarmonyPatch(typeof(DayCycle), "Awake")]
	[HarmonyPostfix]
	private static void SetTimeSpeedAndNightDuration(DayCycle __instance)
	{
		float newNightDuration = Plugin.NightTimeSpeedDivisor;
		float newTimeSpeed = __instance.timeSpeed / Plugin.TimeSpeedDivisor;
		Plugin.Log.LogInfo(string.Concat("Changing Time Speed: ", __instance.timeSpeed, " / ", Plugin.TimeSpeedDivisor, " = ", newTimeSpeed));
		__instance.timeSpeed = newTimeSpeed;
		Plugin.Log.LogInfo(string.Concat("Changing Nighttime Speed divisor: ", __instance.nightDuration, " to ", newNightDuration));
		__instance.nightDuration = newNightDuration;
		//ApproxUnitTimeInSeconds = newTimeSpeed;
	}
	[HarmonyPatch(typeof(GameManager), "Start")]
	[HarmonyPostfix]
	private static void SetDayDuration(GameManager __instance)
	{
		// The dayDuration field on DayCycle is only set to 1f as a default. The actual day duration is changed depending upon the difficulty, and is set by GameManager.
		// This is why we set the dayDuration here.
		float newDayDuration = DayCycle.dayDuration * Plugin.DayLengthMultiplier;
		Plugin.Log.LogInfo(string.Concat("Changing dayDuration: ", DayCycle.dayDuration, " * ", Plugin.DayLengthMultiplier, " = ", newDayDuration));
		DayCycle.dayDuration = newDayDuration;
		float dayPortion = newDayDuration / 2;
		float nightPortion = dayPortion * Plugin.NightTimeSpeedDivisor;
		float totalPortion = dayPortion + nightPortion;

		//Plugin.Log.LogInfo("Total day duration in seconds: " + newDayDuration / ApproxUnitTimeInSeconds);
		Plugin.Log.LogInfo(string.Concat("One day cycle is now ", newDayDuration, " units long. Taking into account that nighttime lasts ", Plugin.NightTimeSpeedDivisor, ", the actual duration is ", totalPortion, " units. Daytime is ", dayPortion, " long, and thus takes up ", dayPortion / totalPortion * 100f, "% of the time, and night is ", nightPortion, " long, and thus takes up ", nightPortion / totalPortion * 100f, "% of the time"));
	}
}