namespace MuckSaveGame
{
	using BepInEx;
	using BepInEx.Logging;
	using HarmonyLib;
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Xml;
	using UnityEngine;
	[BepInPlugin("MuckSaveGame.MichMcb", "MuckSaveGame", "0.6.0")]
	[BepInIncompatibility("flarfo.saveutility")]
	public class Plugin : BaseUnityPlugin
	{
		public static ManualLogSource Log = null!;
		public static XmlWriterSettings XmlWriterSettings = new()
		{
			Encoding = new System.Text.UTF8Encoding(false, false),
			IndentChars = "\t",
			NewLineChars = "\n",
			NewLineHandling = NewLineHandling.None,
			Indent = true,
		};
		public static TimeSpan SaveCooldown;
		public static HashSet<string> SaveMobNames = new();
		public static float MultiplayerSaveDelay = 5f;
		public static float VerticalOffset = 0f;
		public void Awake()
		{
			Log = Logger;

			Config.SaveOnConfigSet = false;
			XmlWriterSettings.Indent = Config.Bind("Main", "Indent", true, "Whether or not to indent the XML save file elements and use newlines. This doesn't affect saving or loading in any way. Indenting the file takes up slightly more space, but makes it easier to read.").Value;
			SaveCooldown = TimeSpan.FromSeconds(Config.Bind("Main", "SaveCooldown", 60, "The number of seconds you must wait until you can save your game again.").Value);
			SaveMobNames = new(Config.Bind("Main", "SaveMobNames", "Big Chunk,Gronk,Guardian,Chief", "The names of the currently spawned mobs that are saved and restored.").Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
			MultiplayerSaveDelay = Config.BindMoreThanZero("Main", "MultiplayerSaveDelay", MultiplayerSaveDelay, "The delay, in seconds, between clicking the Save button and the save actually executing when playing multiplayer. This is so the packets from clients have time to be sent over the network to the game host. There's no delay in singleplayer; saving always happens instantly.").Value;

			VerticalOffset = Config.Bind("Main", "VerticalOffset", VerticalOffset, "Offsets the local player by this value on the Y axis when loading data. This can be useful to prevent you from falling through floors on loading a savegame. Doesn't work in multiplayer yet.").Value;
			Config.Save();

			Directory.CreateDirectory(SaveSystem.GetSavesBasePath());

			SaveSystem.MigrateOldSaves();

			Logger.LogInfo("Loaded MuckSaveGame!");

			AssetBundle assetBundle = GetAssetBundleFromResource("MuckSaveGameAssets");

			UIManager.backgroundImage = assetBundle.LoadAsset<Texture>("Assets/Texture2D/groundCompressed.png");

			Harmony harmony = new("MuckSaveGame");
			harmony.PatchAll();
		}
		public static AssetBundle GetAssetBundleFromResource(string fileName)
		{
			Assembly execAssembly = Assembly.GetExecutingAssembly();

			string resourceName = execAssembly.GetManifestResourceNames().Single(str => str.EndsWith(fileName));

			Log.LogInfo($"Resource Name: {resourceName}");

			using Stream stream = execAssembly.GetManifestResourceStream(resourceName);
			return AssetBundle.LoadFromStream(stream);
		}
	}
}
