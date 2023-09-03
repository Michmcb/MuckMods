namespace MuckSaveGame
{
	using HarmonyLib;
	using MuckSaveGame.Dto;
	using Steamworks;
	using System.Collections.Generic;
	using System.Reflection;
	using UnityEngine;

	[HarmonyPatch]
	internal class ServerMethods
	{
		[HarmonyPatch(typeof(Server), "InitializeServerPackets")]
		[HarmonyPostfix]
		static void InitializeCustomPackets()
		{
			Plugin.Log.LogInfo("SERVER DATA INITIALIZED");

			Server.PacketHandlers.Add(100, new Server.PacketHandler(ReceiveInventory));
			Server.PacketHandlers.Add(101, new Server.PacketHandler(ReceivePowerups));
			Server.PacketHandlers.Add(102, new Server.PacketHandler(ReceivePosition));
			Server.PacketHandlers.Add(103, new Server.PacketHandler(ReceivePlayerStatus));
			Server.PacketHandlers.Add(104, new Server.PacketHandler(ReceiveArmor));
			Server.PacketHandlers.Add(105, new Server.PacketHandler(ReceivePlayerReady));
		}

		//SteamPacketManager
		//LocalClient packethandlers / initializeclientdata()
		//LocalClient HandleData(byte[] data)
		//handledata takes in a packet, and reads received data. num = packet.readint is casted to a serverpacket in serverpackets (enum)
		//LocalClient then passes the key, num, to the packethandlers dictionary, and runs the returned method, passing in the packet

		//initialize packet with specific packethandler method key    => Packet packet = new Packet(47)
		//write data to that packet   => packet.write(data)
		//send the packet    => SendTCPDataToAll(packet));
		private static void SendTCPDataToAll(Packet _packet)
		{
			var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;

			_packet.WriteLength();
			if (NetworkController.Instance.networkType == NetworkController.NetworkType.Classic)
			{
				for (int i = 1; i < Server.MaxPlayers; i++)
				{
					Server.clients[i].tcp.SendData(_packet);
				}
				return;
			}
			foreach (Client client in Server.clients.Values)
			{
				if ((client?.player) != null)
				{
					Plugin.Log.LogInfo("Sending packet to id: " + client.id);

					var tcpVariant = typeof(ServerSend).GetField("TCPvariant", flags);

					SteamPacketManager.SendPacket(client.player.steamId.Value, _packet, (P2PSend)tcpVariant.GetValue("TCPVariant"), SteamPacketManager.NetworkChannel.ToClient);
				}
			}
		}

		private static void SendTCPDataToAll(int exceptClient, Packet _packet)
		{
			var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;

			_packet.WriteLength();
			if (NetworkController.Instance.networkType == NetworkController.NetworkType.Classic)
			{
				for (int i = 1; i < Server.MaxPlayers; i++)
				{
					if (i != exceptClient)
					{
						Server.clients[i].tcp.SendData(_packet);
					}
				}
				return;
			}
			foreach (Client client in Server.clients.Values)
			{
				if ((client?.player) != null && SteamLobby.steamIdToClientId[client.player.steamId.Value] != exceptClient)
				{
					var tcpVariant = typeof(ServerSend).GetField("TCPvariant", flags);

					SteamPacketManager.SendPacket(client.player.steamId.Value, _packet, (P2PSend)tcpVariant.GetValue("TCPVariant"), SteamPacketManager.NetworkChannel.ToClient);
				}
			}
		}

		private static void SendTCPData(int toClient, Packet _packet)
		{
			var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;
			var tcpVariant = typeof(ServerSend).GetField("TCPvariant", flags);

			Packet packet2 = new();
			packet2.SetBytes(_packet.CloneBytes());
			packet2.WriteLength();
			if (NetworkController.Instance.networkType == NetworkController.NetworkType.Classic)
			{
				Server.clients[toClient].tcp.SendData(packet2);
				return;
			}
			SteamPacketManager.SendPacket(Server.clients[toClient].player.steamId.Value, packet2, (P2PSend)tcpVariant.GetValue("TCPVariant"), SteamPacketManager.NetworkChannel.ToClient);
		}

		public static void SendPlayer(int toClient, SavedPlayer player)
		{
			SendPosition(toClient, player.Position);
			GameManager.players[toClient].transform.position = player.Position.ToVec3();

			if (player.Inventory.Count > 0)
			{
				SendInventory(toClient, player.Inventory);
			}

			if (player.Powerups.Count > 0)
			{
				SendPowerups(toClient, player.Powerups);
			}

			SendPlayerStatus(toClient, player.Health, player.MaxHealth, player.Stamina, player.MaxStamina, player.Shield, player.MaxShield, player.Hunger, player.MaxHunger, player.DraculaStacks);

			if (player.Armor.Count > 0)
			{
				SendArmor(toClient, player.Armor, player.Arrows);
			}
		}
		public static void SendHasSave()
		{
			using Packet packet = new(100);
			packet.Write(true);
			SendTCPDataToAll(0, packet);
		}
		public static void SendServerSave()
		{
			using Packet packet = new(101);
			SendTCPDataToAll(0, packet);
		}
		public static void SendInventory(int toClient, List<IdAmount> inventory)
		{
			using Packet packet = new(102);
			foreach (IdAmount cell in inventory)
			{
				packet.Write((short)cell.ItemId);
				packet.Write((short)cell.Amount);
			}

			SendTCPData(toClient, packet);
		}
		public static void SendPowerups(int toClient, List<int> powerups)
		{
			using Packet packet = new(103);
			foreach (int powerup in powerups)
			{
				packet.Write((short)powerup);
			}

			SendTCPData(toClient, packet);
		}
		public static void SendPosition(int toClient, Position position)
		{
			using Packet packet = new(104);
			packet.Write(position.X);
			packet.Write(position.Y);
			packet.Write(position.Z);

			SendTCPData(toClient, packet);
		}
		public static void SendPlayerStatus(int toClient, float hp, int maxhp, float stamina, float maxstamina, float shield, int maxshield, float hunger, float maxhunger, int draculastacks) // add playerstatus
		{
			using Packet packet = new(105);
			packet.Write(hp);
			packet.Write(maxhp);
			packet.Write(stamina);
			packet.Write(maxstamina);
			packet.Write(shield);
			packet.Write(maxshield);
			packet.Write(hunger);
			packet.Write(maxhunger);
			packet.Write(draculastacks);

			SendTCPData(toClient, packet);
		}
		public static void SendArmor(int toClient, List<int> armor, IdAmount arrows)
		{
			using Packet packet = new(106);
			packet.Write((short)armor[0]);
			packet.Write((short)armor[1]);
			packet.Write((short)armor[2]);
			packet.Write((short)armor[3]);

			packet.Write((short)arrows.ItemId);
			packet.Write(arrows.Amount);

			SendTCPData(toClient, packet);
		}
		public static void SendTime(int toClient, float time, float totalTime)
		{
			using Packet packet = new(107);
			packet.Write(time);
			packet.Write(totalTime);

			SendTCPData(toClient, packet);
		}
		private static void ReceiveInventory(int fromClient, Packet _packet)
		{
			Plugin.Log.LogInfo("PACKET RECEIVED");
			Plugin.Log.LogInfo("CLIENT: " + fromClient);

			string steamId = _packet.ReadString(true);

			Plugin.Log.LogInfo("STEAMID: " + steamId);

			List<IdAmount> inventory = new();

			while (_packet.UnreadLength() >= 2)
			{
				inventory.Add(new IdAmount(_packet.ReadShort(true), _packet.ReadShort(true)));
			}

			if (!LoadManager.Players.ContainsKey(steamId))
			{
				LoadManager.Players.Add(steamId, SavedPlayer.CreateEmpty());
				LoadManager.Players[steamId].Inventory = inventory;
			}
			else
			{
				LoadManager.Players[steamId].Inventory = inventory;
			}
		}
		private static void ReceivePowerups(int fromClient, Packet _packet)
		{
			Plugin.Log.LogInfo("PACKET RECEIVED");
			Plugin.Log.LogInfo("CLIENT: " + fromClient);

			string steamId = _packet.ReadString(true);

			Plugin.Log.LogInfo("STEAMID: " + steamId);

			List<int> powerups = new();

			while (_packet.UnreadLength() >= 2)
			{
				powerups.Add(_packet.ReadShort(true));
			}

			if (!LoadManager.Players.ContainsKey(steamId))
			{
				LoadManager.Players.Add(steamId, SavedPlayer.CreateEmpty());
				LoadManager.Players[steamId].Powerups = powerups;
			}
			else
			{
				LoadManager.Players[steamId].Powerups = powerups;
			}
		}
		private static void ReceivePosition(int fromClient, Packet _packet)
		{
			Plugin.Log.LogInfo("PACKET RECEIVED");
			Plugin.Log.LogInfo("CLIENT: " + fromClient);

			string steamId = _packet.ReadString(true);

			Plugin.Log.LogInfo("STEAMID: " + steamId);

			float x = _packet.ReadFloat(true);
			float y = _packet.ReadFloat(true);
			float z = _packet.ReadFloat(true);
			Position position = new(x, y, z);

			if (!LoadManager.Players.ContainsKey(steamId))
			{
				LoadManager.Players.Add(steamId, SavedPlayer.CreateEmpty());
				LoadManager.Players[steamId].Position = position;
			}
			else
			{
				LoadManager.Players[steamId].Position = position;
			}
		}
		private static void ReceivePlayerStatus(int fromClient, Packet _packet)
		{
			Plugin.Log.LogInfo("PACKET RECEIVED");
			Plugin.Log.LogInfo("CLIENT: " + fromClient);

			string steamId = _packet.ReadString(true);

			Plugin.Log.LogInfo("STEAMID: " + steamId);

			float health = _packet.ReadFloat(true);
			int maxHealth = _packet.ReadInt(true);
			float stamina = _packet.ReadFloat(true);
			float maxStamina = _packet.ReadFloat(true);
			float shield = _packet.ReadFloat(true);
			int maxShield = _packet.ReadInt(true);
			float hunger = _packet.ReadFloat(true);
			float maxHunger = _packet.ReadFloat(true);
			int draculaStacks = _packet.ReadInt(true);

			if (!LoadManager.Players.ContainsKey(steamId))
			{
				LoadManager.Players.Add(steamId, SavedPlayer.CreateEmpty());

				LoadManager.Players[steamId].Health = health;
				LoadManager.Players[steamId].MaxHealth = maxHealth;
				LoadManager.Players[steamId].Stamina = stamina;
				LoadManager.Players[steamId].MaxStamina = maxStamina;
				LoadManager.Players[steamId].Shield = shield;
				LoadManager.Players[steamId].MaxShield = maxShield;
				LoadManager.Players[steamId].Hunger = hunger;
				LoadManager.Players[steamId].MaxHunger = maxHunger;
				LoadManager.Players[steamId].DraculaStacks = draculaStacks;
			}
			else
			{
				LoadManager.Players[steamId].Health = health;
				LoadManager.Players[steamId].MaxHealth = maxHealth;
				LoadManager.Players[steamId].Stamina = stamina;
				LoadManager.Players[steamId].MaxStamina = maxStamina;
				LoadManager.Players[steamId].Shield = shield;
				LoadManager.Players[steamId].MaxShield = maxShield;
				LoadManager.Players[steamId].Hunger = hunger;
				LoadManager.Players[steamId].MaxHunger = maxHunger;
				LoadManager.Players[steamId].DraculaStacks = draculaStacks;
			}
		}
		private static void ReceiveArmor(int fromClient, Packet _packet)
		{
			Plugin.Log.LogInfo("PACKET RECEIVED");
			Plugin.Log.LogInfo("CLIENT: " + fromClient);

			string steamId = _packet.ReadString(true);

			Plugin.Log.LogInfo("STEAMID: " + steamId);

			if (!LoadManager.Players.ContainsKey(steamId))
			{
				LoadManager.Players.Add(steamId, SavedPlayer.CreateEmpty());

				List<int> armor = new() { _packet.ReadShort(), _packet.ReadShort(), _packet.ReadShort(), _packet.ReadShort() };

				LoadManager.Players[steamId].Armor = armor;
				LoadManager.Players[steamId].Arrows = new IdAmount(_packet.ReadShort(true), _packet.ReadInt(true));
			}
			else
			{
				List<int> armor = new() { _packet.ReadShort(), _packet.ReadShort(), _packet.ReadShort(), _packet.ReadShort() };

				LoadManager.Players[steamId].Armor = armor;
				LoadManager.Players[steamId].Arrows = new IdAmount(_packet.ReadShort(true), _packet.ReadInt(true));
			}
		}
		private static void ReceivePlayerReady(int fromClient, Packet _packet)
		{
			Plugin.Log.LogInfo("PACKET RECEIVED");
			Plugin.Log.LogInfo("CLIENT: " + fromClient);

			string steamId = _packet.ReadString(true);

			Plugin.Log.LogInfo("STEAMID: " + steamId);

			if (LoadManager.Players.ContainsKey(steamId))
			{
				SendPlayer(fromClient, LoadManager.Players[steamId]);
			}

			if (SaveSystem.BaseGameManager.Data != null)
			{
				SendTime(fromClient, SaveSystem.BaseGameManager.Data.WorldData.Time, SaveSystem.BaseGameManager.Data.WorldData.TotalTime);
			}
		}
	}
}

