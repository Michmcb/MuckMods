namespace MuckConfigurePowerupDrops;

using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;

[BepInPlugin("MuckConfigurePowerupDrops.MichMcb", "Muck Configure Powerup Drops", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
	public static ManualLogSource Log = null!;
	public static string[] WhitePowerups = new string[0];
	public static string[] BluePowerups = new string[0];
	public static string[] OrangePowerups = new string[0];
	private void Awake()
	{
		Log = Logger;

		string whitePowerups = Config.Bind("Main", "WhitePowerups", "Broccoli,Dumbbell,Jetpack,Orange Juice,Peanut Butter,Blue Pill,Red Pill,Sneaker,Robin Hood Hat,Spooo Bean", "The powerups that may drop from Common sources (such as bosses or Black/Brown chests). You may specify a powerup multiple times; this increases the likelihood that that powerup will drop compared to other.").Value;
		string bluePowerups = Config.Bind("Main", "BluePowerups", "Bulldozer,Horseshoe,Danis Milk,Piggybank,Crimson Dagger,Dracula,Janniks Frog,Juice", "The powerups that may drop from Rare sources (such as bosses or Blue chests). You may specify a powerup multiple times; this increases the likelihood that that powerup will drop compared to other.").Value;
		string orangePowerups = Config.Bind("Main", "OrangePowerups", "Adrenaline,Berserk,Checkered Shirt,Sniper Scope,Knuts Hammer,Wings of Glory,Enforcer", "The powerups that may drop from Legendary sources (such as bosses or Gold chests). You may specify a powerup multiple times; this increases the likelihood that that powerup will drop compared to other.").Value;

		char[] comma = new char[] { ',' };
		WhitePowerups = whitePowerups.Split(comma, StringSplitOptions.RemoveEmptyEntries);
		BluePowerups = bluePowerups.Split(comma, StringSplitOptions.RemoveEmptyEntries);
		OrangePowerups = orangePowerups.Split(comma, StringSplitOptions.RemoveEmptyEntries);

		Logger.LogInfo("MuckConfigurePowerupDrops loaded!");
		Harmony.CreateAndPatchAll(typeof(ConfigurePowerupsPatch), null);
	}
}
