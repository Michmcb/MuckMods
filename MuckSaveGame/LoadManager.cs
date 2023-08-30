namespace MuckSaveGame
{
	using HarmonyLib;
	using MuckSaveGame.Dto;
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	[HarmonyPatch]
	public static class LoadManager
	{
		public static string? selectedSavePath = null;
		public static bool serverHasSaveLoaded = false;
		public static bool playersUpdated = false;
		public static bool isEverybodyDead = false;
		public static Dictionary<string, SavedPlayer> Players => SaveSystem.BaseGameManager.Players;

		[HarmonyPatch(typeof(MenuUI), "StartGame")]
		[HarmonyPrefix]
		static void LoadOnStart()
		{
			if (selectedSavePath != null)
			{
				Plugin.Log.LogInfo("This save is loading: " + selectedSavePath);

				try
				{
					SaveSystem.Load(selectedSavePath);
				}
				catch (Exception ex)
				{
					Plugin.Log.LogError("Failed to load: " + ex.ToString());
					return;
				}

				ServerMethods.SendHasSave();
			}
			else
			{
				Plugin.Log.LogInfo("No save has been selected for loading");
			}
		}
		[HarmonyPatch(typeof(GameManager), "SendPlayersIntoGame")]
		[HarmonyPostfix]
		static void AllPlayersReady()
		{
			Plugin.Log.LogInfo("All Players Ready");
			if (LocalClient.serverOwner)
			{
				if (selectedSavePath != null)
				{
					ServerMethods.SendHasSave();
				}
			}
		}
		[HarmonyPatch(typeof(GameLoop), "StartLoop")]
		[HarmonyPostfix]
		static void Postfix()
		{
			Plugin.Log.LogInfo("SPAWN PLAYERS THING");

			if (!playersUpdated)
			{
				if (LocalClient.serverOwner)
				{
					if (selectedSavePath != null)
					{
						SaveSystem.ApplyLoadedData();
					}
					else
					{
						selectedSavePath = SaveSystem.GetPathForSeed(World.worldSeed);
					}
					World.doSave = true;
				}
				playersUpdated = true;
			}
		}
		[HarmonyPatch(typeof(DayCycle), "Awake")]
		[HarmonyPostfix]
		static void DayPatch(DayCycle __instance)
		{
			// This has to be done here unfortunately, because DayCycle does not have a static Instance property.
			if (LocalClient.serverOwner)
			{
				if (selectedSavePath != null && SaveSystem.BaseGameManager.Data != null)
				{
					var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
					DayCycle.time = SaveSystem.BaseGameManager.Data.WorldData.Time;
					typeof(DayCycle).GetProperty("totalTime", flags).SetValue(__instance, SaveSystem.BaseGameManager.Data.WorldData.TotalTime);
				}
			}
		}
		// We don't need to do this here, we can do it in ApplyLoadedData()
		//[HarmonyPatch(typeof(GameLoop), "Awake")]
		//[HarmonyPostfix]
		//static void UpdateMobInfo(GameLoop __instance)
		//{
		//	Plugin.Log.LogInfo("GameLoop is Awake");
		//	if (LocalClient.serverOwner)
		//	{
		//		if (selectedSavePath != null && SaveSystem.BaseGameManager.Data != null)
		//		{
		//			var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
		//			int v = (int) typeof(GameLoop).GetField("activeMobs", flags).GetValue(__instance);// ;, SaveSystem.BaseGameManager.Data.EntityData.Mobs.Count);
		//			Plugin.Log.LogInfo(string.Concat("activeMobs is ", v, " after awakening"));
		//		}
		//	}
		//}
		[HarmonyPatch(typeof(SteamLobby), "FindSeed")]
		[HarmonyPostfix]
		static void GetLoadedSeed(ref int __result)
		{
			if (selectedSavePath != null && SaveSystem.BaseGameManager.Data != null)
			{
				__result = SaveSystem.BaseGameManager.Data.WorldData.Seed;
			}
		}
		[HarmonyPatch(typeof(SpawnChestsInLocations), "SetChests")]
		[HarmonyPrefix]
		static bool DontSpawnRandomChestsIfSaveHasBeenLoaded()
		{
			// The "SetChests" method on the class "SpawnChestsInLocations" basically spawns chests in the world with randomly determined loot
			// If we have loaded a save, then we want to prevent doing that, because we already saved those chests as they are.
			if (LocalClient.serverOwner)
			{
				// If the local client is the server owner, then we only want to spawn chests and their loot if we have no save loaded.
				return selectedSavePath == null;
			}
			else
			{
				// If we're just a client, we only do spawning of the chests if the server has not loaded a save
				return !serverHasSaveLoaded;
			}
		}
	}
}
