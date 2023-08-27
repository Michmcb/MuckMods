namespace MuckRememberLobbySettings;

using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

[BepInPlugin("MuckRememberLobbySettings.MichMcb", "Muck Remember Lobby Settings", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
	public static ManualLogSource Log = null!;
	public static ConfigFile ConfigFile = null!;
	private void Awake()
	{
		Log = Logger;

		Config.SaveOnConfigSet = false;
		ConfigFile = Config;
		Logger.LogInfo("Loaded MuckRememberLobbySettings!");
		Harmony.CreateAndPatchAll(typeof(SettingsPatch), null);
	}
}