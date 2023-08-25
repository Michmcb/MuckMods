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
		}
		else
		{
			Plugin.Log.LogInfo("No White powerups specified; not modifying any White powerups");
		}
		if (Plugin.BluePowerups.Length > 0)
		{
			ItemManager.Instance.powerupsBlue = LoadPowerups("Blue", powerupsByName, Plugin.BluePowerups);
		}
		else
		{
			Plugin.Log.LogInfo("No Blue powerups specified; not modifying any Blue powerups");
		}
		if (Plugin.OrangePowerups.Length > 0)
		{
			ItemManager.Instance.powerupsOrange = LoadPowerups("Orange", powerupsByName, Plugin.OrangePowerups);
		}
		else
		{
			Plugin.Log.LogInfo("No Orange powerups specified; not modifying any Orange powerups");
		}
	}
	private static Powerup[] LoadPowerups(string type, Dictionary<string, Powerup> powerupsByName, IEnumerable<NameWeight> weightedPowerups)
	{
		List<Powerup> powerups = new();
		foreach (var weightedPowerup in weightedPowerups)
		{
			if (powerupsByName.TryGetValue(weightedPowerup.Name, out Powerup? powerup))
			{
				powerups.AddRange(Enumerable.Repeat(powerup, weightedPowerup.Weight));
				Plugin.Log.LogInfo(string.Concat("Loaded powerup \"", powerup.name, "\" as a ", type, " powerup, with a weight of ", weightedPowerup.Weight));
			}
			else
			{
				Plugin.Log.LogError("Powerup not found: " + weightedPowerup.Name);
			}
		}
		return powerups.ToArray();
	}
}