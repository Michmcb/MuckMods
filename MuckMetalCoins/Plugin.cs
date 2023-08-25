namespace MuckMetalCoins;

using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;

[BepInPlugin("MuckMetalCoins.MichMcb", "Muck Metal Coins", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
	public static ManualLogSource Log;
	public static Dictionary<string, Amt> ItemsCraftableIntoCoins = new();
	private void Awake()
	{
		// Plugin startup logic
		Log = Logger;

		Dictionary<string, Amt> defaults = new()
		{
			["Iron bar"] = new(1, 1),
			["Mithril bar"] = new(1, 2),
			["Adamantite bar"] = new(1, 3),
			["Obamium bar"] = new(1, 4),
			["Chunkium bar"] = new(1, 3),
		};
		string itemNames = Config.Bind("Main", "ItemNames", "Iron bar,Mithril bar,Adamantite bar,Obamium bar,Chunkium bar", "The comma-separated names of items that can be crafted into coins at an Anvil. You can specify \"Gold bar\" here to change the number of Coins you get from gold bars, too.").Value;
		foreach (string itemName in itemNames.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries))
		{
			if (!defaults.TryGetValue(itemName, out Amt amt))
			{
				amt = itemName == "Gold bar" ? new(1, 5) : new(1, 1);
			}
			int required = Config.BindMoreThanZero("Amounts", itemName + "_Required", amt.Required, string.Concat("The number of \"", itemName, "\" that are used in the recipe to create Coins."));
			int coins = Config.BindMoreThanZero("Amounts", itemName + "_Coins", amt.Coins, string.Concat("The number of Coins which are created from the number of \"", itemName, "\" used as specified by the setting \"", itemName, "_Required\""));
			ItemsCraftableIntoCoins[itemName] = new(required, coins);
		}

		Logger.LogInfo("MuckMetalCoins loaded!");
		Harmony.CreateAndPatchAll(typeof(CreateRecipesPatch), null);
	}
}
