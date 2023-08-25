﻿namespace MuckFoods;

using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

[BepInPlugin("MuckFoods.MichMcb", "Muck Foods", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
	public static ManualLogSource Log;
	public static bool RebalanceFoods;
	public static float[] HealthBonus = new float[3];
	public static float[] HungerBonus = new float[3];
	public static float[] StaminaBonus = new float[3];
	private void Awake()
	{
		// Plugin startup logic
		Log = Logger;

		RebalanceFoods = Config.Bind("Main", "RebalanceFoods", true, "If true, then vanilla foods' Health/Hunger/Stamina will be rebalanced as follows:\n" +
			"Apple: 5/15/5 -> 5/5/5\n" +
			"Raw Meat: 5/10/0 -> 5/5/0\n" +
			"Gulpon Shroom: 12 Health -> 10 Health\n" +
			"Ligon Shroom: 12 Hunger -> 10 Hunger\n" +
			"Sugon Shroom: 12 Stamina -> 10 Stamina\n" +
			"Slurbon Shroon: 15/15/15 -> 10/10/10\n" +
			"Bread: Unchanged (25/25/25)\n" +
			"Cooked Meat: 20/50/5 -> 15/15/5\n" +
			"Meat Soup: 30/50/20 -> 20/20/10\n" +
			"Apple Pie: 30/60/20 -> 35/35/35\n" +
			"Red Soup: Unchanged (30/10/10)\n" +
			"Yellow Soup: 10/50/10 -> 10/30/10\n" +
			"Purple Soup: 10/10/40 -> 10/10/30\n" +
			"Weird Soup: 40/40/40 -> 30/30/30").Value;

		HealthBonus[0] = Config.Bind("Main", "OneFoodHealthBonus", 2.5f, "The bonus to health that foods with 1 edible ingredient gain").Value;
		HungerBonus[0] = Config.Bind("Main", "OneFoodHungerBonus", 2.5f, "The bonus to hunger that foods with 1 edible ingredient gain").Value;
		StaminaBonus[0] = Config.Bind("Main", "OneFoodStaminaBonus", 2.5f, "The bonus to stamina that foods with 1 edible ingredient gain").Value;

		HealthBonus[1] = Config.Bind("Main", "TwoFoodHealthBonus", 5f, "The bonus to health that foods with 2 edible ingredient gain").Value;
		HungerBonus[1] = Config.Bind("Main", "TwoFoodHungerBonus", 5f, "The bonus to hunger that foods with 2 edible ingredient gain").Value;
		StaminaBonus[2] = Config.Bind("Main", "TwoFoodStaminaBonus", 5f, "The bonus to stamina that foods with 2 edible ingredient gain").Value;

		HealthBonus[2] = Config.Bind("Main", "ThreeFoodHealthBonus", 7.5f, "The bonus to health that foods with 3 edible ingredient gain").Value;
		HungerBonus[2] = Config.Bind("Main", "ThreeFoodHungerBonus", 7.5f, "The bonus to hunger that foods with 3 edible ingredient gain").Value;
		StaminaBonus[2] = Config.Bind("Main", "ThreeFoodStaminaBonus", 7.5f, "The bonus to stamina that foods with 3 edible ingredient gain").Value;


		Logger.LogInfo("MuckFoods loaded!");
		Harmony.CreateAndPatchAll(typeof(CreateRecipesPatch), null);
	}
}