namespace MuckRememberLobbySettings;

using BepInEx.Configuration;
using HarmonyLib;
using System.Reflection;
using TMPro;

public class SettingsPatch
{
	public static bool DoUpdates = false;
	public static UiSettings? Difficulty = null;
	public static UiSettings? FriendlyFire = null;
	public static UiSettings? Gamemode = null;
	public static TMP_InputField? Seed = null;
	public static MethodInfo UiSettings_UpdateSetting = typeof(UiSettings).GetMethod("UpdateSetting", BindingFlags.NonPublic | BindingFlags.Instance);
	public static ConfigEntry<int> BindSetting(UiSettings s)
	{
		ConfigEntry<int> cfg = Plugin.ConfigFile.Bind("Main", s.name, s.setting, "The last selected setting for " + s.name);
		if (cfg.Value <= 0)
		{
			cfg.Value = s.setting;
		}
		return cfg;
	}
	public static void UpdateSetting(UiSettings s, int value)
	{
		UiSettings_UpdateSetting.Invoke(s, new object[] { value });
	}
	[HarmonyPatch(typeof(LobbySettings), "Start")]
	[HarmonyPostfix]
	private static void LobbySettingsStart(LobbySettings __instance)
	{
		DoUpdates = true;
		Seed = __instance.seed;
		Difficulty = __instance.difficultySetting;
		FriendlyFire = __instance.friendlyFireSetting;
		Gamemode = __instance.gamemodeSetting;
		if (Difficulty != null)
		{
			int settingValue = BindSetting(Difficulty).Value;
			UpdateSetting(Difficulty, settingValue);
		}
		if (FriendlyFire != null)
		{
			int settingValue = BindSetting(FriendlyFire).Value;
			UpdateSetting(FriendlyFire, settingValue);
		}
		if (Gamemode != null)
		{
			int settingValue = BindSetting(Gamemode).Value;
			UpdateSetting(Gamemode, settingValue);
		}
		if (Seed is not null)
		{
			string settingValue = Plugin.ConfigFile.Bind("Main", "seed", Seed.text, "The last seed entered").Value;
			Seed.text = settingValue;
		}
		Plugin.ConfigFile.Save();
	}
	[HarmonyPatch(typeof(MenuUI), "StartGame")]
	[HarmonyPostfix]
	private static void StartGame()
	{
		DoUpdates = false;
	}
	[HarmonyPatch(typeof(MenuUI), "LeaveGame")]
	[HarmonyPostfix]
	private static void LeaveGame()
	{
		DoUpdates = true;
	}
	[HarmonyPatch(typeof(UiSettings), "UpdateSetting")]
	[HarmonyPostfix]
	private static void UpdateUiSetting()
	{
		if (DoUpdates)
		{
			if (Difficulty != null) BindSetting(Difficulty).Value = Difficulty.setting;
			if (FriendlyFire != null) BindSetting(FriendlyFire).Value = FriendlyFire.setting;
			if (Gamemode != null) BindSetting(Gamemode).Value = Gamemode.setting;
			if (Seed is not null) Plugin.ConfigFile.Bind("Main", "seed", Seed.text, "The last selected setting for seed").Value = Seed.text;
			Plugin.ConfigFile.Save();
		}
	}
}