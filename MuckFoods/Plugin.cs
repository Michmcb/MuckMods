namespace MuckFoods;

using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

[BepInPlugin("MuckFoods.MichMcb", "Muck Foods", "1.5.0")]
public class Plugin : BaseUnityPlugin
{
	public static ManualLogSource Log = null!;
	public static bool RebalanceFoods = true;
	public static float CookHealthBonus = 5f;
	public static float CookHungerBonus = 5f;
	public static float CookStaminaBonus = 5f;
	public static float[] HealthBonus = new float[4];
	public static float[] HungerBonus = new float[4];
	public static float[] StaminaBonus = new float[4];
	private void Awake()
	{
		// Plugin startup logic
		Log = Logger;

		Config.SaveOnConfigSet = false;
		RebalanceFoods = Config.Bind("Main", "RebalanceFoods", RebalanceFoods, "If true, then vanilla foods' Health/Hunger/Stamina will be rebalanced. These foods will be affected:\n" +
			"Apple: 5/15/5 -> 4/4/4\n" +
			"Raw Meat: 5/10/0 -> 6/6/3\n" +
			"Gulpon Shroom: 12 Health -> 10 Health\n" +
			"Ligon Shroom: 12 Hunger -> 10 Hunger\n" +
			"Sugon Shroom: 12 Stamina -> 10 Stamina\n" +
			"Slurbon Shroon: 15/15/15 -> 10/10/10\n" +
			"Red Soup: 30/10/10 -> 25/10/10\n" +
			"Yellow Soup: 10/50/10 -> 10/25/10\n" +
			"Purple Soup: 10/10/40 -> 10/10/25\n" +
			"Weird Soup: 40/40/40 -> 30/30/30" + 

			"Cooked Meat: 20/50/5 -> Depends on config\n" +
			"Meat Soup: 30/50/20 -> Depends on config\n" +
			"Apple Pie: 30/60/20 -> Depends on config\n" +
			"Meat Pie: 30/60/20 -> Depends on config").Value;

		CookHealthBonus = Config.Bind("Main", "CookHealthBonus", CookHealthBonus, "The base bonus to health that foods gain when cooked. Additive with bonuses for multiple ingredients").Value;
		CookHungerBonus = Config.Bind("Main", "CookHungerBonus", CookHungerBonus, "The base bonus to hunger that foods gain when cooked. Additive with bonuses for multiple ingredients").Value;
		CookStaminaBonus = Config.Bind("Main", "CookStaminaBonus", CookStaminaBonus, "The base bonus to stamina that foods gain when cooked. Additive with bonuses for multiple ingredients").Value;

		HealthBonus[0] = Config.Bind("Main", "OneFoodHealthBonus", 0f, "The bonus to health that foods with 1 ingredient gain").Value;
		HungerBonus[0] = Config.Bind("Main", "OneFoodHungerBonus", 0f, "The bonus to hunger that foods with 1 ingredient gain").Value;
		StaminaBonus[0] = Config.Bind("Main", "OneFoodStaminaBonus", 0f, "The bonus to stamina that foods with 1 ingredient gain").Value;

		HealthBonus[1] = Config.Bind("Main", "TwoFoodHealthBonus", 3f, "The bonus to health that foods with 2 ingredients gain").Value;
		HungerBonus[1] = Config.Bind("Main", "TwoFoodHungerBonus", 3f, "The bonus to hunger that foods with 2 ingredients gain").Value;
		StaminaBonus[1] = Config.Bind("Main", "TwoFoodStaminaBonus", 3f, "The bonus to stamina that foods with 2 ingredients gain").Value;

		HealthBonus[2] = Config.Bind("Main", "ThreeFoodHealthBonus", 6f, "The bonus to health that foods with 3 ingredients gain").Value;
		HungerBonus[2] = Config.Bind("Main", "ThreeFoodHungerBonus", 6f, "The bonus to hunger that foods with 3 ingredients gain").Value;
		StaminaBonus[2] = Config.Bind("Main", "ThreeFoodStaminaBonus", 6f, "The bonus to stamina that foods with 3 ingredients gain").Value;

		HealthBonus[3] = Config.Bind("Main", "FourFoodHealthBonus", 10f, "The bonus to health that foods with 4 ingredients gain").Value;
		HungerBonus[3] = Config.Bind("Main", "FourFoodHungerBonus", 10f, "The bonus to hunger that foods with 4 ingredients gain").Value;
		StaminaBonus[3] = Config.Bind("Main", "FourFoodStaminaBonus", 10f, "The bonus to stamina that foods with 4 ingredients gain").Value;
		Config.Save();

		Resources.Load();

		Logger.LogInfo("MuckFoods loaded!");
		Harmony.CreateAndPatchAll(typeof(CreateRecipesPatch), null);
	}
}
