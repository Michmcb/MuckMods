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
		if (Plugin.BluePowerups.Length > 0)
		{
			ItemManager.Instance.powerupsBlue = LoadPowerups("Blue", powerupsByName, Plugin.BluePowerups);
		}
		if (Plugin.OrangePowerups.Length > 0)
		{
			ItemManager.Instance.powerupsOrange = LoadPowerups("Orange", powerupsByName, Plugin.OrangePowerups);
		}
	}
	private static Powerup[] LoadPowerups(string type, Dictionary<string, Powerup> powerupsByName, IEnumerable<string> powerupNames)
	{
		List<Powerup> powerups = new();
		foreach (string powerupName in powerupNames)
		{
			if (powerupsByName.TryGetValue(powerupName, out Powerup? powerup))
			{
				powerups.Add(powerup);
			}
			else
			{
				Plugin.Log.LogError("Powerup not found: " + powerupName);
			}
		}
		Plugin.Log.LogInfo(string.Concat("New possible ", type, " powerups are: ", string.Join(", ", powerups.Select(x => x.name))));
		return powerups.ToArray();
	}
}