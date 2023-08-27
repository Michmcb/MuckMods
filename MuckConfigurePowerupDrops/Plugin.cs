namespace MuckConfigurePowerupDrops;

using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;

[BepInPlugin("MuckConfigurePowerupDrops.MichMcb", "Muck Configure Powerup Drops", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
	public static ManualLogSource Log = null!;
	public static NameWeight[] WhitePowerups = new NameWeight[0];
	public static NameWeight[] BluePowerups = new NameWeight[0];
	public static NameWeight[] OrangePowerups = new NameWeight[0];
	private void Awake()
	{
		Log = Logger;

		Config.SaveOnConfigSet = false;
		char[] comma = new char[] { ',' };
		string[] whitePowerups = Config.Bind("Main", "WhitePowerups", "Broccoli,Dumbbell,Jetpack,Orange Juice,Peanut Butter,Blue Pill,Red Pill,Sneaker,Robin Hood Hat,Spooo Bean", "The powerups that may drop from Common sources, such as bosses or Black/Brown chests. If this setting is empty, White powerups will not be modified. Specifying the same powerup multiple times increases its weight within this pool.").Value.Split(comma, StringSplitOptions.RemoveEmptyEntries);
		string[] bluePowerups = Config.Bind("Main", "BluePowerups", "Bulldozer,Horseshoe,Danis Milk,Piggybank,Crimson Dagger,Dracula,Janniks Frog,Juice", "The powerups that may drop from Rare sources, such as bosses or Blue chests. If this setting is empty, Blue powerups will not be modified. Specifying the same powerup multiple times increases its weight within this pool.").Value.Split(comma, StringSplitOptions.RemoveEmptyEntries);
		string[] orangePowerups = Config.Bind("Main", "OrangePowerups", "Adrenaline,Berserk,Checkered Shirt,Sniper Scope,Knuts Hammer,Wings of Glory,Enforcer", "The powerups that may drop from Legendary sources, such as bosses or Gold chests. If this setting is empty, Gold powerups will not be modified. Specifying the same powerup multiple times increases its weight within this pool.").Value.Split(comma, StringSplitOptions.RemoveEmptyEntries);

		WhitePowerups = LoadPowerups(whitePowerups);
		BluePowerups = LoadPowerups(bluePowerups);
		OrangePowerups = LoadPowerups(orangePowerups);
		Config.Save();

		Logger.LogInfo("MuckConfigurePowerupDrops loaded!");
		Harmony.CreateAndPatchAll(typeof(ConfigurePowerupsPatch), null);
	}
	private NameWeight[] LoadPowerups(string[] names)
	{
		NameWeight[] ncs = new NameWeight[names.Length];
		for (int i = 0; i < names.Length; i++)
		{
			string name = names[i];
			int count = Config.BindMoreThanZero("Weight", name + "_Weight", 1, string.Concat("The weight of the \"", name, "\" powerup across all pools, cumulative with being specified multiple times."));
			ncs[i] = new NameWeight(name, count);
		}
		return ncs;
	}
}
