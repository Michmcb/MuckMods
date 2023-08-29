namespace MuckTimeModifier;

using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

[BepInPlugin("MuckTimeModifier.MichMcb", "Muck Time Modifier", "1.2.0")]
public class Plugin : BaseUnityPlugin
{
	public static ManualLogSource Log = null!;
	public static float DayLengthMultiplier = 1f;
	public static float NightLengthMultiplier = 1f;
	public static float TimeSpeedDivisor = 1.25f;
	public static int EasyBossDay = 6;
	public static int NormalBossDay = 4;
	public static int GamerBossDay = 3;
	private void Awake()
	{
		// Plugin startup logic
		Log = Logger;
		Config.SaveOnConfigSet = false;
		DayLengthMultiplier = Config.BindMoreThanZero("Main", "DayLengthMultiplier", DayLengthMultiplier, "Multiplies the daytime portion of a single day/night cycle by this number. May not be 0. For example, 1 will leave it the same, 2 will double the daytime portion of a day/night cycle, making daytime longer. Difficulty also affects the length of daytime.").Value;
		NightLengthMultiplier = Config.BindMoreThanZero("Main", "NightLengthMultiplier", NightLengthMultiplier, "Multiplies the nighttime portion of a single day/night cycle by this number. May not be 0. For example, 1 will leave it the same, 2 will double the nighttime portion of a day/night cycle, making nighttime longer.").Value;

		// Config entries only get added to the dictionary once we bind something.
		// // So, we first bind to the old name, then bind to the new one, passing in the old value (or the default value) as the default value for the new one.
		var oldCfg = Config.Bind("Main", "TimeSpeedDivisor", TimeSpeedDivisor, "");
		Config.Remove(oldCfg.Definition);

		TimeSpeedDivisor = Config.BindMoreThanZero("Main", "DayCycleLengthMultiplier", oldCfg.Value, "Divides the timespeed by this number. May not be 0. For example, 1 will leave it the same, 1.5 makes a day/night cycle last 50% longer, 0.5 makes a day/night cycle last half as long. This makes time progress at a faster/slower pace, so it affects the length of both daytime and nighttime.").Value;

		int defaultEasy = EasyBossDay;
		int defaultNormal = NormalBossDay;
		int defaultGamer = GamerBossDay;

		EasyBossDay = Config.BindMoreThanZero("Main", "EasyBossDay", EasyBossDay, "On easy difficulty, every time the day number is evenly divisible by this number, then the day is a boss day. This causes a boss to spawn at night. By default it's.Value 6, so this means days 6, 12, 18, etc. boss days.").Value;
		NormalBossDay = Config.BindMoreThanZero("Main", "NormalBossDay", NormalBossDay, "On normal difficulty, every time the day number is evenly divisible by this number, then the day is a boss day.").Value;
		GamerBossDay = Config.BindMoreThanZero("Main", "GamerBossDay", GamerBossDay, "On gamer difficulty, every time the day number is evenly divisible by this number, then the day is a boss day.").Value;

		Config.Save();

		Logger.LogInfo("MuckTimeModifier loaded!");
		Harmony.CreateAndPatchAll(typeof(TimeModifierPatch), null);
		// We only patch the method for a boss day if it's different to the defaults
		if (defaultEasy != EasyBossDay || defaultNormal != NormalBossDay || defaultGamer != GamerBossDay)
		{
			Logger.LogInfo("Patching BossDay");
			Harmony.CreateAndPatchAll(typeof(BossDayPatch), null);
		}
	}
}
