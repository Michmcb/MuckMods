namespace MuckTimeModifier;

using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

[BepInPlugin("MuckTimeModifier.MichMcb", "Muck Time Modifier", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
	public static ManualLogSource Log = null!;
	public static float DayLengthMultiplier;
	public static float NightLengthMultiplier;
	public static float TimeSpeedDivisor;
	private void Awake()
	{
		// Plugin startup logic
		Log = Logger;
		DayLengthMultiplier = Config.Bind("Main", "DayLengthMultiplier", 1f, "Multiplies the daytime portion of a single day/night cycle by this number. 1 will leave it the same, 2 will double the daytime portion of a day/night cycle, making daytime longer. Difficulty also affects the length of daytime.").Value;
		NightLengthMultiplier = Config.Bind("Main", "NightLengthMultiplier", 1f, "Multiplies the nighttime portion of a single day/night cycle by this number. 1 will leave it the same, 2 will double the nighttime portion of a day/night cycle, making nighttime longer.").Value;
		TimeSpeedDivisor = Config.Bind("Main", "TimeSpeedDivisor", 1.25f, "Divides the timespeed by this number. 1 will leave it the same, 1.5 makes a day/night cycle last 50% longer, 0.5 makes a day/night cycle last half as long. This makes time progress at a faster/slower pace, so it affects the length of both daytime and nighttime.").Value;
		Logger.LogInfo("MuckTimeModifier loaded!");
		Harmony.CreateAndPatchAll(typeof(TimeModifierPatch), null);
	}
}
