namespace MuckSaveGame
{
	using HarmonyLib;
	using MuckSaveGame.Dto;
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	[HarmonyPatch]
	public static class World
	{
		public static bool doSave = false;
		public static int worldSeed;
		internal static Dictionary<int, SavedBuild> builds = new();
		public static bool isBoatMarked = false;
		public static bool isBoatFound = false;
		public static bool isBoatFinished = false;
		public static bool areGemsMarked = false;
		public static bool isLeavingIsland = false;
		public static int chest;
		public static int furnace;
		public static int cauldron;
		[HarmonyPatch(typeof(ItemManager), "InitAllItems")]
		[HarmonyPostfix]
		static void GetBuildIds()
		{
			chest = ItemManager.Instance.GetItemByName("Chest").id;
			furnace = ItemManager.Instance.GetItemByName("Furnace").id;
			cauldron = ItemManager.Instance.GetItemByName("Cauldron").id;
		}
		[HarmonyPatch(typeof(SteamLobby), "FindSeed")]
		static void Postfix(ref int __result)
		{
			worldSeed = __result;
		}
		[HarmonyPatch(typeof(GameLoop), "NewDay")]
		static void Postfix(int day)
		{
			if (UIManager.useAutoSave)
			{
				Plugin.Log.LogInfo("Day " + day);
				if (day == 0 || !doSave)
				{
					Plugin.Log.LogInfo("DO NOT SAVE");
					return;
				}

				Save();
			}
		}
		public static void Save()
		{
			if (LocalClient.serverOwner)
			{
				if (LoadManager.selectedSavePath != "")
				{
					ClientSend.SendChatMessage("<color=#ADD8E6>Saving...");
					ChatBox.Instance.AppendMessage(-1, "<color=#ADD8E6>Saving...", "");

					Plugin.Log.LogInfo("SAVING");

					//send packets to receive player data
					ServerMethods.SendServerSave();

					WorldTimer timer = new GameObject("World Timer", new[] { typeof(WorldTimer) }).GetComponent<WorldTimer>();
					timer.StartSave(Plugin.MultiplayerSaveDelay);
				}
			}
		}
		[HarmonyPatch(typeof(GameManager), "LeaveGame")]
		[HarmonyPostfix]
		static void LeaveGameReset()
		{
			Plugin.Log.LogInfo("RESETTING");

			builds.Clear();
			isBoatFinished = false;
			isBoatFound = false;
			isBoatMarked = false;
			areGemsMarked = false;

			doSave = false;

			LoadManager.Players.Clear();
			LoadManager.selectedSavePath = null;
			LoadManager.serverHasSaveLoaded = false;
			LoadManager.playersUpdated = false;
			UIManager.canSaveAfter = DateTime.MinValue;
			SaveSystem.Unload();
		}
		[HarmonyPatch(typeof(BuildManager), "BuildItem")]
		static void Postfix(int buildOwner, int itemID, int objectId, Vector3 position, int yRotation)
		{
			if (LocalClient.serverOwner)
			{
				if (itemID == chest || itemID == furnace || itemID == cauldron || builds.ContainsKey(objectId))
				{
					return;
				}

				builds.Add(objectId, new SavedBuild(itemID, Position.FromVec3(position), yRotation));
			}
		}
		[HarmonyPatch(typeof(ResourceManager), "RemoveItem")]
		[HarmonyPostfix]
		static void RemoveBuild(int id)
		{
			if (LocalClient.serverOwner)
			{
				builds.Remove(id);
			}
		}
		[HarmonyPatch(typeof(Boat), "FindShip")]
		[HarmonyPostfix]
		private static void Boat_FindShip()
		{
			if (LocalClient.serverOwner)
			{
				isBoatFound = true;
			}
		}
		[HarmonyPatch(typeof(Boat), "MarkShip")]
		[HarmonyPostfix]
		private static void Boat_MarkShip()
		{
			if (LocalClient.serverOwner)
			{
				isBoatMarked = true;
			}
		}
		[HarmonyPatch(typeof(Boat), "MarkGems")]
		[HarmonyPostfix]
		private static void Boat_MarkGems()
		{
			if (LocalClient.serverOwner)
			{
				areGemsMarked = true;
			}
		}
		[HarmonyPatch(typeof(Boat), "BoatFinished")]
		[HarmonyPostfix]
		private static void Boat_BoatFinished()
		{
			if (LocalClient.serverOwner)
			{
				isBoatFinished = true;
			}
		}
		[HarmonyPatch(typeof(Boat), "LeaveIsland")]
		[HarmonyPostfix]
		private static void Boat_LeaveIsland()
		{
			if (LocalClient.serverOwner)
			{
				isLeavingIsland = true;
			}
		}
	}
}
