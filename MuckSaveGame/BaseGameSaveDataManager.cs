namespace MuckSaveGame
{
	using MuckSaveGame.Dto;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Xml.Linq;
	using UnityEngine;

	public sealed class BaseGameSaveDataManager : ISaveDataManager
	{
		public BaseGameSaveData? Data { get; set; }
		public string Name => "main";
		public int LastBossNightThatSpawnedBoss { get; set; }
		public Dictionary<string, SavedPlayer> Players { get; private set; } = new();
		public ISaveData GetSaveData()
		{
			// TODO If we want to make objects such as trees go away, for example, we could MAYBE keep track of destroyed object IDs. This is of course, assuming that their ID is going to be consistent, which it may or may not be. We'd have to test that.
			BindingFlags bind = BindingFlags.NonPublic | BindingFlags.Instance;
			var gameLoopType = typeof(GameLoop);
			var vec = (Vector2)gameLoopType.GetField("checkMobUpdateInterval", bind).GetValue(GameLoop.Instance);
			WorldSaveData worldSaveData = new
			(
				currentDay: GameLoop.Instance.currentDay,
				seed: World.worldSeed,
				time: DayCycle.time,
				totalTime: DayCycle.totalTime,
				mobUpdateIntervalMin: vec.x,
				mobUpdateIntervalMax: vec.y,
				lastBossNightThatSpawnedBoss: LastBossNightThatSpawnedBoss,
				((List<MobType>)gameLoopType.GetField("bossRotation", bind).GetValue(GameLoop.Instance)).Select(x => x.id).ToList()
			);

			PlayerStatus p = PlayerStatus.Instance;
			List<int> powerups = new((int[])typeof(PowerupInventory).GetField("powerups", bind).GetValue(PowerupInventory.Instance));

			var armorCells = InventoryUI.Instance.armorCells;
			List<int> armor = new(armorCells.Length);
			for (int i = 0; i < armorCells.Length; i++)
			{
				armor.Add(armorCells[i].currentItem
					? armorCells[i].currentItem.id
					: -1);
			}

			var uiEventsType = typeof(UiEvents);
			bool[] unlockedSoft = (bool[])uiEventsType.GetField("unlockedSoft", bind).GetValue(UiEvents.Instance);
			bool[] unlockedHard = (bool[])uiEventsType.GetField("unlockedHard", bind).GetValue(UiEvents.Instance);
			bool[] stationsUnlocked = (bool[])uiEventsType.GetField("stationsUnlocked", bind).GetValue(UiEvents.Instance);

			IdAmount arrows = InventoryUI.Instance.arrows.currentItem
				? IdAmount.FromItem(InventoryUI.Instance.arrows.currentItem)
				: default;

			List<IdAmount> inventory = new(InventoryUI.Instance.cells.Count);
			foreach (var cell in InventoryUI.Instance.cells)
			{
				if (cell.currentItem)
				{
					inventory.Add(IdAmount.FromItem(cell.currentItem));
				}
				else
				{
					inventory.Add(default);
				}
			}

			SavedPlayer localPlayer = new
			(
				health: p.hp,
				maxHealth: p.maxHp,
				stamina: p.stamina,
				maxStamina: p.maxStamina,
				shield: p.shield,
				maxShield: p.maxShield,
				hunger: p.hunger,
				maxHunger: p.maxHunger,
				draculaStacks: p.draculaStacks,
				position: Position.FromVec3(PlayerMovement.Instance.transform.position),
				powerups: powerups,
				armor: armor,
				softUnlocks: new(unlockedSoft),
				hardUnlocks: new(unlockedHard),
				stationUnlocks: new(stationsUnlocked),
				arrows: arrows,
				inventory: inventory
			);

			PlayerSaveData playerSaveData = new(localPlayer, LoadManager.Players);

			List<SavedContainer> chests = new();
			List<SavedContainer> furnaces = new();
			List<SavedContainer> cauldrons = new();

			foreach (Chest chest in ChestManager.Instance.chests.Values)
			{
				SavedContainer container = SavedContainer.Create(chest);
				switch (chest.chestSize)
				{
					case 3:
						furnaces.Add(container);
						break;
					case 6:
						cauldrons.Add(container);
						break;
					case 21:
						chests.Add(container);
						break;
				}
			}

			ContainerSaveData containerSaveData = new(chests, furnaces, cauldrons);

			List<SavedBuild> builds = World.builds.Values.ToList();

			Component[] boatComponents = (Component[])typeof(Boat).GetField("repairs", bind).GetValue(Boat.Instance);
			List<int> boatRepairs = new(boatComponents.Length);
			foreach (RepairInteract? repairInteract in boatComponents.Cast<RepairInteract>())
			{
				// RepairInteract acts a bit strangely; the object isn't actually null, but the equality operator returns true if compared to null sometimes. Adding these causes (harmless) NullReferenceExceptions later
				// RepairInteract can also be cast to a boolean. By casting it to a boolean, it tells us "Has it not been interacted with yet?"

				// However we can't actually exclude them if they compare equal to null. If we do that, then 
				if (!repairInteract) // repairInteract != null && 
				{
					boatRepairs.Add(repairInteract.GetId());
				}
			}
			BoatSaveData boatSaveData = new(World.isBoatMarked, World.isBoatFound, World.isBoatFinished, World.areGemsMarked, boatRepairs);

			List<SavedMob> mobs = new();
			foreach (Mob mob in MobManager.Instance.mobs.Values)
			{
				// 9 is Big Chunk, 10 is Gronk, 14 is Guardian, 16 is Chief
				//if (mob.mobType.id == 9 || mob.mobType.id == 10 || mob.mobType.id == 14 || mob.mobType.id == 16)
				if (Plugin.SaveMobNames.Contains(mob.mobType.name))
				{
					// guardianType is the colour of the Guardian
					Plugin.Log.LogInfo("Saving mob: " + mob.mobType.name);
					Guardian g = mob.gameObject.GetComponent<Guardian>();
					int guardianType = g ? (int)g.type : -1;
					mobs.Add(new SavedMob(mob.mobType.id, (int)mob.bossType, guardianType, mob.multiplier, mob.bossMultiplier, Position.FromVec3(mob.transform.position)));
				}
				else
				{
					Plugin.Log.LogInfo("Mob will not be saved: " + mob.mobType.name);
				}
			}

			List<SavedDroppedItem> droppedItems = new();
			foreach (GameObject gameObject in ItemManager.Instance.list.Values)
			{
				Item i = gameObject.GetComponent<Item>();
				if (i)
				{
					droppedItems.Add(new SavedDroppedItem(new IdAmount(i.item.id, i.item.amount), Position.FromVec3(gameObject.transform.position)));
				}
			}

			EntitySaveData entities = new(builds, mobs, droppedItems);

			return new BaseGameSaveData(worldSaveData, boatSaveData, playerSaveData, containerSaveData, entities);
		}
		public void ApplyLoadedData()
		{
			if (Data == null) { return; }
			WorldSaveData w = Data.WorldData;
			GameManager.instance.currentDay = w.CurrentDay;
			GameManager.instance.UpdateDay(w.CurrentDay);
			ServerSend.NewDay(w.CurrentDay);
			LastBossNightThatSpawnedBoss = w.LastBossNightThatSpawnedBoss;

			LoadChestWrappers(Data.ContainerData.Chests, World.chest);
			LoadChestWrappers(Data.ContainerData.Furnaces, World.furnace);
			LoadChestWrappers(Data.ContainerData.Cauldrons, World.cauldron);

			LoadLocalPlayer(Data.PlayerData.LocalPlayer);
			LoadMobs(Data.EntityData.Mobs);
			LoadItems(Data.EntityData.DroppedItems);
			LoadBuilds(Data.EntityData.Builds);
			LoadBoat(Data.BoatData);

			Plugin.Log.LogInfo("Applying loaded data");
			// We don't use Add(key, value) here because the aallMobs array seems to have multiple instances of the same mob for whatever reason
			Dictionary<int, MobType> mobsById = new();
			foreach (MobType mob in MobSpawner.Instance.allMobs)
			{
				mobsById[mob.id] = mob;
			}
			// Now set the information on the GameLoop so mobs have the correct strength and spawn in the correct number
			GameLoop.Instance.currentDay = w.CurrentDay;
			var gameLoopType = typeof(GameLoop);
			BindingFlags bind = BindingFlags.Instance | BindingFlags.NonPublic;
			gameLoopType.GetField("activeMobs", bind).SetValue(GameLoop.Instance, Data.EntityData.Mobs.Count);
			float totalWeight = (float)gameLoopType.GetMethod("CalculateSpawnWeights", bind).Invoke(GameLoop.Instance, new object[] { w.CurrentDay });
			gameLoopType.GetField("totalWeight", bind).SetValue(GameLoop.Instance, totalWeight);
			gameLoopType.GetMethod("FindMobCap", bind).Invoke(GameLoop.Instance, null);

			if (w.MobUpdateIntervalMin > 0 && w.MobUpdateIntervalMax > 0)
			{
				float num = 1f - PowerupInventory.CumulativeDistribution(w.CurrentDay, 0.05f, 0.5f);
				Vector2 checkMobUpdateInterval = new Vector2(w.MobUpdateIntervalMin, w.MobUpdateIntervalMax) * num;
				gameLoopType.GetField("checkMobUpdateInterval", bind).SetValue(GameLoop.Instance, checkMobUpdateInterval);
			}

			// Add bosses to the rotation if we need to
			// But if it's empty then don't touch it, because Muck's code gets a boss from the list BEFORE it checks to see if it's empty
			// This should only be empty when a save was migrated, or if it was fiddled with (which is fine, if a user wants to edit their save, hey, more power to them!)
			if (w.BossesInRotation.Count > 0)
			{
				var bossRotationField = gameLoopType.GetField("bossRotation", bind);
				List<MobType> bossRotation = (List<MobType>)bossRotationField.GetValue(GameLoop.Instance);
				bossRotation.Clear();

				foreach (int id in w.BossesInRotation)
				{
					if (mobsById.TryGetValue(id, out MobType? mobType))
					{
						bossRotation.Add(mobType);
					}
				}
			}
			Players = Data.PlayerData.ClientPlayers;
		}
		public void LoadXml(XElement xml)
		{
			Data = BaseGameSaveData.Load(xml);
		}
		private void LoadChestWrappers(IEnumerable<SavedContainer> containers, int buildId)
		{
			foreach (SavedContainer container in containers)
			{
				int nextId = ChestManager.Instance.GetNextId();

				BuildManager.Instance.BuildItem(0, buildId, nextId, container.Position.ToVec3(), container.Rotation);
				ServerSend.SendBuild(0, buildId, nextId, container.Position.ToVec3(), container.Rotation);

				Chest curChest = ChestManager.Instance.chests[nextId];
				curChest.cells = new InventoryItem[container.Size];

				int i = 0;
				foreach (CellIdAmount inv in container.Inventory)
				{
					if (ItemManager.Instance.allItems.TryGetValue(inv.ItemId, out InventoryItem? item))
					{
						InventoryItem inventoryItem = ScriptableObject.CreateInstance<InventoryItem>();
						inventoryItem.Copy(item, inv.Amount);
						if (inv.Cell < curChest.cells.Length)
						{
							curChest.cells[inv.Cell] = inventoryItem;
							ServerSend.UpdateChest(0, nextId, inv.Cell, inv.ItemId, inv.Amount);
						}
						else
						{
							Plugin.Log.LogError(string.Concat("Inventory at index ", i, " specified cell ", inv.Cell, " but the inventory only has ", curChest.cells.Length, " cells; index is out of range. Skipping"));
						}
					}
					else
					{
						Plugin.Log.LogError(string.Concat("Could not find item with ID ", inv.ItemId, ". Skipping"));
					}
					i++;
				}
				if (buildId == World.furnace)
				{
					// TODO we want to make Furnaces automatically start up when they are loaded - not sure if this works in multiplayer?
					curChest.UpdateCraftables();
				}
			}
		}
		private void LoadItems(IList<SavedDroppedItem> droppedItems)
		{
			foreach (SavedDroppedItem droppedItem in droppedItems)
			{
				int id = droppedItem.Item.ItemId;
				if (ItemManager.Instance.allItems.ContainsKey(id))
				{
					int amount = droppedItem.Item.Amount;
					int nextId = ItemManager.Instance.GetNextId();
					Vector3 pos = droppedItem.Position.ToVec3();

					ItemManager.Instance.DropItemAtPosition(id, amount, pos, nextId);
					ServerSend.DropItemAtPosition(id, amount, nextId, pos);
				}
				else
				{
					Plugin.Log.LogError(string.Concat("Error loading dropped item: Could not find item with ID ", id, ". Skipping"));
				}
			}
		}
		private void LoadBuilds(IList<SavedBuild> builds)
		{
			foreach (SavedBuild build in builds)
			{
				if (ItemManager.Instance.allItems.ContainsKey(build.ItemId))
				{
					int nextId = BuildManager.Instance.GetNextBuildId();
					BuildManager.Instance.BuildItem(0, build.ItemId, nextId, build.Position.ToVec3(), build.Rotation);
					ServerSend.SendBuild(0, build.ItemId, nextId, build.Position.ToVec3(), build.Rotation);
				}
				else
				{
					Plugin.Log.LogError(string.Concat("Error loading build: Could not find item with ID ", build.ItemId, ". Skipping"));
				}
			}
		}
		private void LoadMobs(IList<SavedMob> mobs)
		{
			Plugin.Log.LogInfo("Spawning Mobs!");

			var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
			int mobCount = 0;

			foreach (SavedMob mob in mobs)
			{
				if (mob.MobType < MobSpawner.Instance.allMobs.Length)
				{
					int nextId = MobManager.Instance.GetNextId();

					MobSpawner.Instance.ServerSpawnNewMob(nextId, mob.MobType, mob.Position.ToVec3(), mob.Multiplier, mob.BossMultiplier, (Mob.BossType)mob.BossType, mob.GuardianType);
					mobCount++;
				}
				else
				{
					Plugin.Log.LogError(string.Concat("Error loading mob: Mob type is ", mob.MobType, " but the maximum valid mob type is ", MobSpawner.Instance.allMobs.Length, "; index is out of range. Skipping"));
				}
			}

			typeof(MobManager).GetField("mobId", flags).SetValue(MobManager.Instance, mobCount);
		}
		private void LoadLocalPlayer(SavedPlayer playerData)
		{
			InventoryItem[] allScriptableItems = ItemManager.Instance.allScriptableItems;

			var bind = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

			var uiEventsType = typeof(UiEvents);
			bool[] unlockedSoft = (bool[])uiEventsType.GetField("unlockedSoft", bind).GetValue(UiEvents.Instance);
			bool[] unlockedHard = (bool[])uiEventsType.GetField("unlockedHard", bind).GetValue(UiEvents.Instance);
			bool[] stationsUnlocked = (bool[])uiEventsType.GetField("stationsUnlocked", bind).GetValue(UiEvents.Instance);

			for (int i = 0; i < playerData.SoftUnlocks.Count && i < unlockedSoft.Length; i++)
			{
				unlockedSoft[i] = playerData.SoftUnlocks[i];
			}
			for (int i = 0; i < playerData.HardUnlocks.Count && i < unlockedHard.Length; i++)
			{
				unlockedHard[i] = playerData.HardUnlocks[i];
			}
			for (int i = 0; i < playerData.StationUnlocks.Count && i < stationsUnlocked.Length; i++)
			{
				stationsUnlocked[i] = playerData.StationUnlocks[i];
			}

			//var unlockItemSoft = typeof(UiEvents).GetMethod("UnlockItemSoft", flags);
			//for (int i = 0; i < playerData.SoftUnlocks.Count; i++)
			//{
			//	if (playerData.SoftUnlocks[i])
			//	{
			//		unlockItemSoft.Invoke(UiEvents.Instance, new object[] { i });
			//	}
			//}

			typeof(PowerupInventory).GetField("powerups", bind).SetValue(PowerupInventory.Instance, playerData.Powerups.ToArray());

			// UpdateStats() Calculates the max HP and the max shields, so we don't actually have to set those, but we do anyways
			// However, if player HP is 0, that means the user opened the save menu and hit save after they died but before the Defeat window came up.
			// To kill them properly, the player must have more than 0 HP, so we set their HP to their max HP and later on deal damage.
			bool playerShouldBeDead = false;
			if (playerData.Health <= 0f)
			{
				playerShouldBeDead = true;
				PlayerStatus.Instance.hp = playerData.MaxHealth;
			}
			else
			{
				PlayerStatus.Instance.hp = playerData.Health;
			}
			PlayerStatus.Instance.maxHp = playerData.MaxHealth;
			PlayerStatus.Instance.stamina = playerData.Stamina;
			PlayerStatus.Instance.maxStamina = playerData.MaxStamina;
			PlayerStatus.Instance.shield = playerData.Shield;
			PlayerStatus.Instance.maxShield = playerData.MaxShield;
			PlayerStatus.Instance.hunger = playerData.Hunger;
			PlayerStatus.Instance.maxHunger = playerData.MaxHunger;

			PlayerStatus.Instance.draculaStacks = playerData.DraculaStacks;

			Plugin.Log.LogInfo("Player Position: " + playerData.Position.ToString());
			PlayerMovement.Instance.transform.position = playerData.Position.ToVec3();

			for (int powerupId = 0; powerupId < playerData.Powerups.Count; powerupId++)
			{
				for (int powerupCount = 0; powerupCount < playerData.Powerups[powerupId]; powerupCount++)
				{
					if (ItemManager.Instance.allPowerups.ContainsKey(powerupId))
					{
						PowerupUI.Instance.AddPowerup(powerupId);
					}
					else
					{
						Plugin.Log.LogError(string.Concat("Error loading Player powerups: Powerup ID is ", powerupId, " which was not found. Skipping"));
					}
				}
			}

			OtherInput.Instance.ToggleInventory(OtherInput.CraftingState.Inventory);
			for (int i = 0; i < 4; i++)
			{
				var itemId = playerData.Armor[i];
				if (itemId != -1)
				{
					if (itemId < allScriptableItems.Length)
					{
						InventoryUI.Instance.AddArmor(allScriptableItems[itemId]);
						PlayerStatus.Instance.UpdateArmor(i, itemId);
					}
					else
					{
						Plugin.Log.LogError(string.Concat("Error loading Player armor: Item ID is ", itemId, " but the maximum valid Item ID is ", allScriptableItems.Length, ". Skipping"));
					}
				}
			}
			OtherInput.Instance.ToggleInventory(OtherInput.CraftingState.Inventory);

			for (int i = 0; i < InventoryUI.Instance.cells.Count; i++)
			{
				var invItem = playerData.Inventory[i];
				if (invItem.Amount != 0)
				{
					if (invItem.ItemId < allScriptableItems.Length)
					{
						InventoryUI.Instance.cells[i].ForceAddItem(allScriptableItems[invItem.ItemId], invItem.Amount);
					}
					else
					{
						Plugin.Log.LogError(string.Concat("Error loading Player inventory: Item ID is ", invItem.ItemId, " but the maximum valid Item ID is ", allScriptableItems.Length, ". Skipping"));
					}
				}
			}

			if (playerData.Arrows.Amount != 0)
			{
				if (playerData.Arrows.ItemId < allScriptableItems.Length)
				{
					InventoryUI.Instance.arrows.ForceAddItem(allScriptableItems[playerData.Arrows.ItemId], playerData.Arrows.Amount);
				}
				else
				{
					Plugin.Log.LogError(string.Concat("Error loading arrows: Item ID is ", playerData.Arrows.ItemId, " but the maximum valid Item ID is ", allScriptableItems.Length, ". Skipping"));
				}
			}

			typeof(UiEvents).GetMethod("Unlock", bind).Invoke(UiEvents.Instance, null);
			InventoryUI.Instance.UpdateAllCells();
			Hotbar.Instance.UpdateHotbar();
			PlayerStatus.Instance.UpdateStats();

			if (LocalClient.serverOwner && GameManager.players.Count == 1 && playerShouldBeDead)
			{
				PlayerStatus.Instance.DealDamage(playerData.MaxHealth + playerData.MaxShield, ignoreProtection: true);
			}
		}
		private void LoadClients()
		{
			List<Player> clientPlayers = new();

			Client[] clients = Server.clients.Values.ToArray();

			for (int i = 0; i < Server.clients.Values.Count; i++)
			{
				if (clients[i].player != null)
				{
					Plugin.Log.LogInfo(i);
					clientPlayers.Add(clients[i].player);
				}
			}

			for (int i = 0; i < clientPlayers.Count; i++)
			{
				if (Players.ContainsKey(clientPlayers[i].steamId.ToString()))
				{
					if (i != 0)
					{
						int curId = clients[i].id;

						SavedPlayer curPlayer = Players[clientPlayers[i].steamId.ToString()];

						Plugin.Log.LogInfo("Sending Inventory To: " + i);
						Plugin.Log.LogInfo("Steam Id: " + clientPlayers[i].steamId.ToString());

						ServerMethods.SendPlayer(curId, curPlayer);
					}
				}
			}
		}
		private void LoadBoat(BoatSaveData boatData)
		{
			BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

			foreach (int id in boatData.RepairedObjectIds)
			{
				Plugin.Log.LogInfo("Interacting with ID " + id);
				//ResourceManager.Instance.list[id]
				ClientSend.Interact(id);
			}

			if (boatData.IsBoatMarked)
			{
				Boat.Instance.MarkShip();
			}
			if (boatData.IsBoatFound)
			{
				Boat.Instance.FindShip();
			}
			if (boatData.AreGemsMarked)
			{
				typeof(Boat).GetMethod("MarkGems", flags).Invoke(Boat.Instance, null);
			}
			if (boatData.IsBoatFinished)
			{
				Boat.Instance.BoatFinished(ResourceManager.Instance.GetNextId());
			}
		}
		public void Unload()
		{
			Data = null;
			Players.Clear();
		}
	}
}
