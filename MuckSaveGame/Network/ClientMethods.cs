namespace MuckSaveGame
{
	using BepInEx;
	using HarmonyLib;
	using System.Collections.Generic;
	using System.Reflection;
	using UnityEngine;
	using InventoryItem = global::InventoryItem;

	[HarmonyPatch]
	internal class ClientMethods
	{
		[HarmonyPatch(typeof(LocalClient), "InitializeClientData")]
		static void Postfix()
		{
			Plugin.Log.LogInfo("CLIENT DATA INITIALIZED");

			LocalClient.packetHandlers.Add(100, new LocalClient.PacketHandler(ReceiveServerHasSave));
			LocalClient.packetHandlers.Add(101, new LocalClient.PacketHandler(HandleSave));
			LocalClient.packetHandlers.Add(102, new LocalClient.PacketHandler(ReceiveInventory));
			LocalClient.packetHandlers.Add(103, new LocalClient.PacketHandler(ReceivePowerups));
			LocalClient.packetHandlers.Add(104, new LocalClient.PacketHandler(ReceivePosition));
			LocalClient.packetHandlers.Add(105, new LocalClient.PacketHandler(ReceivePlayerStatus));
			LocalClient.packetHandlers.Add(106, new LocalClient.PacketHandler(ReceiveArmor));
			LocalClient.packetHandlers.Add(107, new LocalClient.PacketHandler(ReceiveTime));
		}
		public static void SendInventory()
		{
			Plugin.Log.LogInfo("SENDING INVENTORY");

			using Packet packet = new(100);
			packet.Write(SteamManager.Instance.PlayerSteamIdString);

			foreach (InventoryCell cell in InventoryUI.Instance.cells)
			{
				if (cell.currentItem)
				{
					int cellAmount = cell.currentItem.amount;
					if (cellAmount > 0)
					{
						packet.Write((short)cell.currentItem.id);
						packet.Write((short)cellAmount);
					}
				}
				else
				{
					packet.Write((short)-1);
					packet.Write((short)0);
				}
			}

			Net.ClientSendTCPData(packet);
		}
		public static void SendPowerups()
		{
			Plugin.Log.LogInfo("SENDING POWERUPS");

			var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;

			int[] powerups = (int[])typeof(PowerupInventory).GetField("powerups", flags).GetValue(PowerupInventory.Instance);

			using Packet packet = new(101);
			packet.Write(SteamManager.Instance.PlayerSteamIdString);

			foreach (int powerup in powerups)
			{
				packet.Write((short)powerup);
			}

			Net.ClientSendTCPData(packet);
		}
		public static void SendPosition()
		{
			Plugin.Log.LogInfo("SENDING POSITION");

			float x = PlayerMovement.Instance.transform.position.x;
			float y = PlayerMovement.Instance.transform.position.y;
			float z = PlayerMovement.Instance.transform.position.z;

			using Packet packet = new(102);
			packet.Write(SteamManager.Instance.PlayerSteamIdString);

			packet.Write(x);
			packet.Write(y);
			packet.Write(z);

			Net.ClientSendTCPData(packet);
		}
		public static void SendPlayerStatus()
		{
			Plugin.Log.LogInfo("SENDING PLAYERSTATUS");

			using Packet packet = new(103);
			packet.Write(SteamManager.Instance.PlayerSteamIdString);

			packet.Write(PlayerStatus.Instance.hp);
			packet.Write(PlayerStatus.Instance.maxHp);
			packet.Write(PlayerStatus.Instance.stamina);
			packet.Write(PlayerStatus.Instance.maxStamina);
			packet.Write(PlayerStatus.Instance.shield);
			packet.Write(PlayerStatus.Instance.maxShield);
			packet.Write(PlayerStatus.Instance.hunger);
			packet.Write(PlayerStatus.Instance.maxHunger);
			packet.Write(PlayerStatus.Instance.draculaStacks);

			Net.ClientSendTCPData(packet);
		}
		public static void SendArmor()
		{
			Plugin.Log.LogInfo("SENDING ARMOR");

			using Packet packet = new(104);
			packet.Write(SteamManager.Instance.PlayerSteamIdString);

			for (int i = 0; i < 4; i++)
			{
				if (InventoryUI.Instance.armorCells[i].currentItem)
				{
					packet.Write((short)InventoryUI.Instance.armorCells[i].currentItem.id);
				}
				else
				{
					packet.Write((short)-1);
				}

			}

			if (InventoryUI.Instance.arrows.currentItem)
			{
				packet.Write((short)InventoryUI.Instance.arrows.currentItem.id);
				packet.Write(InventoryUI.Instance.arrows.currentItem.amount);
			}
			else
			{
				packet.Write((short)-1);
				packet.Write(-1);
			}

			Net.ClientSendTCPData(packet);
		}
		public static void SendPlayerReady()
		{
			using Packet packet = new(105);
			packet.Write(SteamManager.Instance.PlayerSteamIdString);

			Net.ClientSendTCPData(packet);
		}
		public static void ReceiveServerHasSave(Packet _packet)
		{
			Plugin.Log.LogInfo("Server Has Save Received!");
			LoadManager.serverHasSaveLoaded = _packet.ReadBool(true);
		}
		public static void HandleSave(Packet _packet)
		{
			Plugin.Log.LogInfo("Send Save Received!");

			SendInventory();
			SendPowerups();
			SendPosition();
			SendPlayerStatus();
			SendArmor();
		}
		private static void ReceiveInventory(Packet _packet)
		{
			Plugin.Log.LogInfo("Inventory Received!");

			InventoryItem[] allScriptableItems = ItemManager.Instance.allScriptableItems;

			int count = 0;

			while (_packet.UnreadLength() >= 2)
			{
				int id = _packet.ReadShort(true);
				int amount = _packet.ReadShort(true);

				if (id != -1)
				{
					InventoryUI.Instance.cells[count].ForceAddItem(allScriptableItems[id], amount);
				}

				count++;
			}

			InventoryUI.Instance.UpdateAllCells();
			Hotbar.Instance.UpdateHotbar();
		}
		private static void ReceivePowerups(Packet _packet)
		{
			Plugin.Log.LogInfo("Powerups Received!");

			List<int> powerups = new();

			while (_packet.UnreadLength() >= 2)
			{
				int amount = _packet.ReadShort(true);

				powerups.Add(amount);
			}

			var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

			typeof(PowerupInventory).GetField("powerups", flags).SetValue(PowerupInventory.Instance, powerups.ToArray());

			for (int i = 0; i < powerups.Count; i++)
			{
				for (int j = 0; j < powerups[i]; j++)
				{
					PowerupUI.Instance.AddPowerup(i);
				}
			}
		}
		private static void ReceivePosition(Packet _packet)
		{
			Plugin.Log.LogInfo("Position Received!");

			Vector3 receivedPosition = new();
			receivedPosition.x = _packet.ReadFloat(true);
			receivedPosition.y = _packet.ReadFloat(true);
			receivedPosition.z = _packet.ReadFloat(true);

			PlayerMovement.Instance.transform.position = receivedPosition;
		}
		private static void ReceivePlayerStatus(Packet _packet)
		{
			Plugin.Log.LogInfo("Player Status Received!");

			PlayerStatus.Instance.hp = _packet.ReadFloat(true);
			PlayerStatus.Instance.maxHp = _packet.ReadInt(true);
			PlayerStatus.Instance.stamina = _packet.ReadFloat(true);
			PlayerStatus.Instance.maxStamina = _packet.ReadFloat(true);
			PlayerStatus.Instance.shield = _packet.ReadFloat(true);
			PlayerStatus.Instance.maxShield = _packet.ReadInt(true);
			PlayerStatus.Instance.hunger = _packet.ReadFloat(true);
			PlayerStatus.Instance.maxHunger = _packet.ReadFloat(true);

			PlayerStatus.Instance.draculaStacks = _packet.ReadInt(true);

			PlayerStatus.Instance.UpdateStats();

			if (PlayerStatus.Instance.hp <= 0)
			{
				try
				{
					ClientSend.PlayerHitObject(1, 1, 1, new Vector3(), 1);
					WorldTimer timer = new GameObject("World Timer", new[] { typeof(WorldTimer) }).GetComponent<WorldTimer>();

					timer.StartPlayerDeath(0.3f);
				}
				catch
				{
					var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

					typeof(PlayerStatus).GetMethod("PlayerDied", flags).Invoke(PlayerStatus.Instance, new object[] { 0, -1 });
				}
			}
		}
		private static void ReceiveArmor(Packet _packet)
		{
			Plugin.Log.LogInfo("Armor Received!");

			InventoryItem[] allScriptableItems = ItemManager.Instance.allScriptableItems;

			OtherInput.Instance.ToggleInventory(OtherInput.CraftingState.Inventory);
			for (int i = 0; i < 4; i++)
			{
				short id = _packet.ReadShort(true);

				if (id != -1)
				{
					InventoryUI.Instance.AddArmor(allScriptableItems[id]);
					PlayerStatus.Instance.UpdateArmor(i, id);
				}
			}
			OtherInput.Instance.ToggleInventory(OtherInput.CraftingState.Inventory);

			short arrowId = _packet.ReadShort(true);
			int arrowAmount = _packet.ReadInt(true);

			if (arrowId != -1)
			{
				InventoryUI.Instance.arrows.ForceAddItem(allScriptableItems[arrowId], arrowAmount);
			}
		}
		private static void ReceiveTime(Packet _packet)
		{
			Plugin.Log.LogInfo("Time Receieved!");

			var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

			DayCycle.time = _packet.ReadFloat(true);
			typeof(DayCycle).GetProperty("totalTime", flags).SetValue(null, _packet.ReadFloat(true));
		}
		[HarmonyPatch(typeof(GameManager), "SpawnPlayer")]
		[HarmonyPostfix]
		static void PlayerSpawned(int id, string username, Color color, Vector3 position, float orientationY)
		{
			if (!LocalClient.serverOwner)
			{
				Plugin.Log.LogInfo("Not Owner!");
				if (LoadManager.serverHasSaveLoaded)
				{
					Plugin.Log.LogInfo("Has Save!");
					if (id == LocalClient.instance.myId)
					{
						Plugin.Log.LogInfo("Sending Player Ready!");

						SendPlayerReady();
					}
				}
			}
		}
	}
}
