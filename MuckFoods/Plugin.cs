namespace MuckFoods;

using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

[BepInPlugin("MuckFoods.MichMcb", "Muck Foods", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
	public static ManualLogSource Log;
	private void Awake()
	{
		// Plugin startup logic
		Log = Logger;
		Logger.LogInfo("MuckFoods loaded!");
		Harmony.CreateAndPatchAll(typeof(CreateRecipesPatch), null);
	}
}
