namespace MuckCharcoal;

using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;

[BepInPlugin("MuckCharcoal.MichMcb", "Muck Charcoal", "1.3.0")]
public class Plugin : BaseUnityPlugin
{
	public static ManualLogSource Log = null!;
	public static int MaxUses = 3;
	public static int WoodProcessTime = 5;
	public static string ProcessableItemNames = "Wood,Birch Wood,Fir Wood,Oak Wood,Dark Oak Wood";
	public static string[] ProcessableItems = new string[0];
	private void Awake()
	{
		Log = Logger;

		Config.SaveOnConfigSet = false;
		MaxUses = Config.BindMoreThanZero("Main", "MaxUses", MaxUses, "The number of items that one piece of charcoal can smelt or cook").Value;
		WoodProcessTime = Config.BindMoreThanZero("Main", "WoodProcessTime", WoodProcessTime, "The time, in seconds, it takes to burn one piece of wood and turn it into charcoal").Value;
		ProcessableItemNames = Config.Bind("Main", "ProcessableItemNames", ProcessableItemNames, "The comma-separated item names which can be processed in a furnace and turned into charcoal").Value;
		ProcessableItems = ProcessableItemNames.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
		Logger.LogInfo("MuckCharcoal loaded!");
		Config.Save();

		Harmony.CreateAndPatchAll(typeof(CreateRecipesPatch), null);
	}
}