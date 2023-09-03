namespace MuckTimeModifier;

using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

[BepInPlugin("MuckTimeModifier.MichMcb", "Muck Time Modifier", "1.3.0")]
public class Plugin : BaseUnityPlugin
{
	public static ManualLogSource Log = null!;
	public static float DayLengthMultiplier = 1f;
	public static float NightTimeSpeedDivisor = 0.3f;
	public static float TimeSpeedDivisor = 1.25f;
	public static int EasyBossDay = 6;
	public static int NormalBossDay = 4;
	public static int GamerBossDay = 3;
	private void Awake()
	{
		// Plugin startup logic
		Log = Logger;
		Config.SaveOnConfigSet = false;

		// The way time works in Muck is like this:
		// 1 Day is always 1.0f. The time speed is the base amount by which the time of day increments per update, like so:
		// let increment = timeSpeed / dayDuration
		// Nighttime is defined as time being >0.5f. When it's nighttime, the increment is modified by doing this:
		// let increment = increment / nightDuration
		// The effect is basically, a larger nightDuration makes nighttime longer. Duration of 3 means time passes 3 times slower at night.
		DayLengthMultiplier = Config.BindMoreThanZero("Main", "DayLengthMultiplier", DayLengthMultiplier, "Multiplies the base length of a single day/night cycle by this number. May not be 0. For example, 1 will leave it the same, 2 will double the length of a day cycle. Difficulty affects base the length of a day/night cycle.").Value;

		var oldCfg1 = Config.Bind("Main", "NightLengthMultiplier", 0f, "");
		Config.Remove(oldCfg1.Definition);

		NightTimeSpeedDivisor = Config.BindMoreThanZero("Main", "NightTimeSpeedDivisor", NightTimeSpeedDivisor, "During the nighttime, the speed at which time progresses is divided by this number. May not be 0. This has the effect of making time pass faster or slower at night. In vanilla, it is 0.3, meaning nighttime is 30% of the length of daytime (i.e. speed progresses 3.333... times faster)").Value;

		// Config entries only get added to the dictionary once we bind something.
		// // So, we first bind to the old name, then bind to the new one, passing in the old value (or the default value) as the default value for the new one.
		var oldCfg2 = Config.Bind("Main", "DayCycleLengthMultiplier", TimeSpeedDivisor, "");
		Config.Remove(oldCfg2.Definition);

		TimeSpeedDivisor = Config.BindMoreThanZero("Main", "TimeSpeedMultiplier", oldCfg2.Value, "Divides the speed at which time passes by this number. May not be 0. For example, 1 will leave it the same, 1.5 makes a day/night cycle last 50% longer, 0.5 makes a day/night cycle last half as long. It works this way because say, a speed of 1 / 2 = 0.5, which is half speed, thus twice as long. Or 1 / 0.5 = 2; double speed, half as long.").Value;

		int defaultEasy = EasyBossDay;
		int defaultNormal = NormalBossDay;
		int defaultGamer = GamerBossDay;

		EasyBossDay = Config.BindMoreThanZero("Main", "EasyBossDay", EasyBossDay, "On easy difficulty, every time the day number is evenly divisible by this number, then the day is a boss day. This causes a boss to spawn at night. By default it's 6, so this means days 6, 12, 18, etc. boss days.").Value;
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
