namespace MuckConfigurePowerupDrops;

using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

public class ConfigurePowerupsPatch
{
	[HarmonyPatch(typeof(ItemManager), "InitAllPowerups")]
	[HarmonyPostfix]
	private static void RejiggerPowerups()
	{
		Dictionary<string, Powerup> powerupsByName = ItemManager.Instance.allPowerups.Values.ToDictionary(k => k.name, v => v);

		if (Plugin.WhitePowerups.Length > 0)
		{
			ItemManager.Instance.powerupsWhite = LoadPowerups("White", powerupsByName, Plugin.WhitePowerups);
			Plugin.Log.LogInfo("New White powerups pool: " + string.Join(", ", ItemManager.Instance.powerupsWhite.Select(x => x.name)));
		}
		else
		{
			Plugin.Log.LogInfo("No White powerups specified; not modifying any White powerups");
		}
		if (Plugin.BluePowerups.Length > 0)
		{
			ItemManager.Instance.powerupsBlue = LoadPowerups("Blue", powerupsByName, Plugin.BluePowerups);
			Plugin.Log.LogInfo("New Blue powerups pool: " + string.Join(", ", ItemManager.Instance.powerupsBlue.Select(x => x.name)));
		}
		else
		{
			Plugin.Log.LogInfo("No Blue powerups specified; not modifying any Blue powerups");
		}
		if (Plugin.OrangePowerups.Length > 0)
		{
			ItemManager.Instance.powerupsOrange = LoadPowerups("Orange", powerupsByName, Plugin.OrangePowerups);
			Plugin.Log.LogInfo("New Orange powerups pool: " + string.Join(", ", ItemManager.Instance.powerupsOrange.Select(x => x.name)));
		}
		else
		{
			Plugin.Log.LogInfo("No Orange powerups specified; not modifying any Orange powerups");
		}
	}
	[HarmonyPatch(typeof(LootContainerInteract), nameof(LootContainerInteract.ServerExecute))]
	[HarmonyPrefix]
	private static void Stuff(LootContainerInteract __instance, ref int ___basePrice)
	{
		if (Plugin.DropRates.TryGetValue(___basePrice, out ChestDropRates rates))
		{
			__instance.white = rates.White;
			__instance.blue = rates.Blue;
			__instance.gold = rates.Orange;
			Plugin.Log.LogInfo("Replaced chest loot drops");
		}
		else
		{
			Plugin.Log.LogInfo("Not touching chest loot drops");
		}
	}
	private static Powerup[] LoadPowerups(string colour, Dictionary<string, Powerup> powerupsByName, IEnumerable<NameWeight> weightedPowerups)
	{
		List<Powerup> powerups = new();
		foreach (var weightedPowerup in weightedPowerups)
		{
			if (powerupsByName.TryGetValue(weightedPowerup.Name, out Powerup? powerup))
			{
				powerups.AddRange(Enumerable.Repeat(powerup, weightedPowerup.Weight));
				Plugin.Log.LogInfo(string.Concat("Loaded powerup \"", powerup.name, "\" as a ", colour, " powerup, with a weight of ", weightedPowerup.Weight));
			}
			else
			{
				Plugin.Log.LogError("Powerup not found: " + weightedPowerup.Name);
			}
		}
		return powerups.ToArray();
	}
}