namespace MuckSaveGame
{
	using MuckSaveGame.Dto;
	using MuckSaveGame.Legacy;
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Xml;
	using System.Xml.Linq;
	using System.Xml.Serialization;
	public static class SaveSystem
	{
		public static readonly BaseGameSaveDataManager BaseGameManager = new();
		private static readonly Dictionary<string, ISaveDataManager> Registrations = new()
		{
			[BaseGameManager.Name] = BaseGameManager,
		};
		private static readonly List<ISaveDataManager> OrderedRegistrations = new() { BaseGameManager };
		public static void Register(ISaveDataManager manager)
		{
			Registrations.Add(manager.Name, manager);
			OrderedRegistrations.Add(manager);
		}
		public static string GetSavesBasePath()
		{
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Saves");
		}
		public static string GetPathForSeed(int seed)
		{
			return GetPathForFileName(seed + ".mucksave");
		}
		public static string GetPathForFileName(string fileName)
		{
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Saves", fileName);
		}
		public static void Save(string path)
		{
			List<(ISaveData data, string name)> saveData = new(OrderedRegistrations.Count);
			foreach (ISaveDataManager reg in OrderedRegistrations)
			{
				saveData.Add((reg.GetSaveData(), reg.Name));
			}

			XDocument xdoc = new();
			XElement root = new("MuckSaveGame");
			xdoc.Add(root);
			foreach ((ISaveData data, string name) in saveData)
			{
				XElement x = root.AddElement("Data");
				x.SetAttributeValue("type", name);
				data.SaveXml(x);
			}
			using FileStream fout = new(path, FileMode.Create, FileAccess.Write);
			using XmlWriter xout = XmlWriter.Create(fout, Plugin.XmlWriterSettings);
			xdoc.Save(xout);
		}
		public static void ApplyLoadedData()
		{
			foreach (ISaveDataManager reg in OrderedRegistrations)
			{
				reg.ApplyLoadedData();
			}
		}
		public static BaseGameSaveData? Load(string path)
		{
			Plugin.Log.LogInfo("Loading");
			if (File.Exists(path))
			{
				XDocument xdoc;
				using (FileStream stream = new(path, FileMode.Open, FileAccess.Read))
				{
					xdoc = XDocument.Load(stream);
				}
				if (xdoc != null)
				{
					foreach (XElement xe in xdoc.Root.Elements())
					{
						if (Registrations.TryGetValue(xe.Attribute("type").Value, out ISaveDataManager? registration))
						{
							registration.LoadXml(xe);
						}
					}
					return BaseGameManager.Data;
				}
				else
				{
					Plugin.Log.LogError($"Could not load file at {path}");
					return null;
				}
			}
			else
			{
				Plugin.Log.LogError($"Save File does not Exist at {path}");
				return null;
			}
		}
		public static void Unload()
		{
			for (int i = OrderedRegistrations.Count - 1; i >= 0; i--)
			{
				OrderedRegistrations[i].Unload();
			}
		}
		public static List<string> GetAllSaves()
		{
			List<string> savePaths = new();
			foreach (var path in Directory.EnumerateFiles(GetSavesBasePath()))
			{
				if (".mucksave".Equals(Path.GetExtension(path), StringComparison.OrdinalIgnoreCase))
				{
					savePaths.Add(path);
				}
			}
			return savePaths;
		}
		public static void MigrateOldSaves()
		{
			foreach (string path in Directory.EnumerateFiles(GetSavesBasePath()))
			{
				if (!".muck".Equals(Path.GetExtension(path), StringComparison.OrdinalIgnoreCase)) continue;
				Plugin.Log.LogInfo("Migrating legacy save: " + path);
				bool delete = false;
				try
				{
					WorldSave w;
					using (FileStream fin = new(path, FileMode.Open, FileAccess.Read))
					{
						XmlSerializer serializer = new(typeof(WorldSave));
						w = (WorldSave)serializer.Deserialize(fin);
					}

					WorldSaveData worldData = new(w.currentDay, w.worldSeed, w.time, w.totalTime, 0f, 0f, 0, new());
					bool isBoatMarked = false;
					bool isBoatFound = false;
					bool isBoatFinished = false;
					bool areGemsMarked = false;
					switch (w.boatStatus)
					{
						case 1:
							isBoatFound = true;
							break;
						case 2:
							isBoatFound = true;
							areGemsMarked = true;
							break;
						case 3:
							isBoatFound = true;
							areGemsMarked = true;
							isBoatFinished = true;
							break;
					}
					BoatSaveData boatData = new(isBoatMarked, isBoatFound, isBoatFinished, areGemsMarked, w.repairs);
					Dictionary<string, SavedPlayer> clientPlayers = new();
					PlayerSaveData playerData = new(new(w.health, w.maxHealth, w.stamina, w.maxStamina, w.shield, w.maxShield, w.hunger, w.maxHunger, w.draculaHpIncrease, Position.FromFloats(w.position), w.powerups.ToList(), w.armor.ToList(), w.softUnlocks.ToList(), new(), new(), new IdAmount(w.arrows.Item1, w.arrows.Item2), w.inventory.Select(x => new IdAmount(x.Item1, x.Item2)).ToList()), clientPlayers);
					ContainerSaveData containerData = new(ConvertContainers(w.chests), ConvertContainers(w.furnaces), ConvertContainers(w.cauldrons));
					EntitySaveData entityData = new
					(
						builds: w.builds.Select(x => new SavedBuild(x.itemId, Position.FromFloats(x.position), x.rotation)).ToList(),
						mobs: w.mobs.Select(x => new SavedMob(x.mobType, x.bossType, x.guardianType, x.multiplier, x.bossMultiplier, Position.FromFloats(x.position))).ToList(),
						droppedItems: w.droppedItems.Select(x => new SavedDroppedItem(new IdAmount(x.itemId, x.amount), Position.FromFloats(x.position))).ToList()
					);
					BaseGameSaveData newSaveData = new(worldData, boatData, playerData, containerData, entityData);

					XDocument xdoc = new();
					XElement root = new("MuckSaveGame");
					xdoc.Add(root);
					XElement x = root.AddElement("Data");
					x.SetAttributeValue("type", BaseGameManager.Name);
					newSaveData.SaveXml(x);
					using (FileStream fout = new(path + "save", FileMode.Create, FileAccess.Write))
					{
						using XmlWriter xout = XmlWriter.Create(fout, Plugin.XmlWriterSettings);
						xdoc.Save(xout);
					}
					Plugin.Log.LogInfo("Legacy save migrated successfully!");
					delete = true;
				}
				catch (Exception ex)
				{
					Plugin.Log.LogError("Failed to migrate legacy save: " + ex.ToString());
				}
				if (delete)
				{
					try
					{
						File.Delete(path);
					}
					catch (Exception ex)
					{
						Plugin.Log.LogError("Failed to delete legacy save: " + ex.ToString());
					}
				}
			}
		}
		private static List<SavedContainer> ConvertContainers(List<ChestWrapper> chests)
		{
			List<SavedContainer> containers = new(chests.Count);
			foreach (ChestWrapper c in chests)
			{
				containers.Add(new(c.chestSize, Position.FromFloats(c.position), c.rotation, c.cells.Select(x => new CellIdAmount(x.Item3, x.Item1, x.Item2)).ToList()));
			}
			return containers;
		}
	}
}
