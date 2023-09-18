namespace MuckConfigurePowerupDrops;

using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
[BepInPlugin("MuckConfigurePowerupDrops.MichMcb", "Muck Configure Powerup Drops", "1.3.0")]
public class Plugin : BaseUnityPlugin
{
	public static ManualLogSource Log = null!;
	public static float EasyChestPriceDivisor = 8f;
	public static float NormalChestPriceDivisor = 6f;
	public static float GamerChestPriceDivisor = 5f;
	public static Dictionary<int, ChestDropRates> DropRates = new();
	public static NameWeight[] WhitePowerups = new NameWeight[0];
	public static NameWeight[] BluePowerups = new NameWeight[0];
	public static NameWeight[] OrangePowerups = new NameWeight[0];
	private void Awake()
	{
		Log = Logger;

		Config.SaveOnConfigSet = false;
		char[] comma = new char[] { ',' };
		string[] whitePowerups = Config.Bind("Main", "WhitePowerups", "Broccoli,Dumbbell,Jetpack,Orange Juice,Peanut Butter,Blue Pill,Red Pill,Sneaker,Robin Hood Hat,Spooo Bean", "The powerups that may drop from Common sources, such as bosses or Black/Brown chests. If this setting is empty, White powerups will not be modified. Specifying the same powerup multiple times increases its weight within this pool.").Value.Split(comma, StringSplitOptions.RemoveEmptyEntries);
		string[] bluePowerups = Config.Bind("Main", "BluePowerups", "Bulldozer,Horseshoe,Danis Milk,Piggybank,Crimson Dagger,Dracula,Janniks Frog,Juice", "The powerups that may drop from Rare sources, such as bosses or Blue chests. If this setting is empty, Blue powerups will not be modified. Specifying the same powerup multiple times increases its weight within this pool.").Value.Split(comma, StringSplitOptions.RemoveEmptyEntries);
		string[] orangePowerups = Config.Bind("Main", "OrangePowerups", "Adrenaline,Berserk,Checkered Shirt,Sniper Scope,Knuts Hammer,Wings of Glory,Enforcer", "The powerups that may drop from Legendary sources, such as bosses or Gold chests. If this setting is empty, Gold powerups will not be modified. Specifying the same powerup multiple times increases its weight within this pool.").Value.Split(comma, StringSplitOptions.RemoveEmptyEntries);

		float blackChestWhiteDrop = Config.BindNonNegative("Probability", "BlackChestWhiteDrop", 0.9f, "The probability that a Black chest will drop a White powerup").Value;
		float blackChestBlueDrop = Config.BindNonNegative("Probability", "BlackChestBlueDrop", 0.09f, "The probability that a Black chest will drop a Blue powerup").Value;
		float blackChestOrangeDrop = Config.BindNonNegative("Probability", "BlackChestOrangeDrop", 0.01f, "The probability that a Black chest will drop an Orange powerup").Value;

		float brownChestWhiteDrop = Config.BindNonNegative("Probability", "BrownChestWhiteDrop", 0.803f, "The probability that a Brown chest will drop a White powerup").Value;
		float brownChestBlueDrop = Config.BindNonNegative("Probability", "BrownChestBlueDrop", 0.185f, "The probability that a Brown chest will drop a Blue powerup").Value;
		float brownChestOrangeDrop = Config.BindNonNegative("Probability", "BrownChestOrangeDrop", 0.012f, "The probability that a Brown chest will drop an Orange powerup").Value;

		float blueChestWhiteDrop = Config.BindNonNegative("Probability", "BlueChestWhiteDrop", 0f, "The probability that a Blue chest will drop a White powerup").Value;
		float blueChestBlueDrop = Config.BindNonNegative("Probability", "BlueChestBlueDrop", 0.86f, "The probability that a Blue chest will drop a Blue powerup").Value;
		float blueChestOrangeDrop = Config.BindNonNegative("Probability", "BlueChestOrangeDrop", 0.15f, "The probability that a Blue chest will drop an Orange powerup").Value;

		float goldChestWhiteDrop = Config.BindNonNegative("Probability", "GoldChestWhiteDrop", 0f, "The probability that a Gold chest will drop a White powerup").Value;
		float goldChestBlueDrop = Config.BindNonNegative("Probability", "GoldChestBlueDrop", 0f, "The probability that a Gold chest will drop a Blue powerup").Value;
		float goldChestOrangeDrop = Config.BindNonNegative("Probability", "GoldChestOrangeDrop", 1f, "The probability that a Gold chest will drop an Orange powerup").Value;

		int blackChestBasePrice = Config.Bind("Internal", "AssumeChestWithThisBasePriceIsABlackChest", 0, "DO NOT EDIT UNLESS YOU HAVE A MOD THAT CHANGES BASE PRICES OF CHESTS! If a chest has this base price, then it's considered to be a black chest and will have its drop rates adjusted accordingly.").Value;
		int brownChestBasePrice = Config.Bind("Internal", "AssumeChestWithThisBasePriceIsABrownChest", 25, "DO NOT EDIT UNLESS YOU HAVE A MOD THAT CHANGES BASE PRICES OF CHESTS! If a chest has this base price, then it's considered to be a brown chest and will have its drop rates adjusted accordingly.").Value;
		int blueChestBasePrice = Config.Bind("Internal", "AssumeChestWithThisBasePriceIsABlueChest", 80, "DO NOT EDIT UNLESS YOU HAVE A MOD THAT CHANGES BASE PRICES OF CHESTS! If a chest has this base price, then it's considered to be a blue chest and will have its drop rates adjusted accordingly.").Value;
		int goldChestBasePrice = Config.Bind("Internal", "AssumeChestWithThisBasePriceIsAGoldChest", 200, "DO NOT EDIT UNLESS YOU HAVE A MOD THAT CHANGES BASE PRICES OF CHESTS! If a chest has this base price, then it's considered to be a gold chest and will have its drop rates adjusted accordingly.").Value;

		DropRates[blackChestBasePrice] = new ChestDropRates(blackChestWhiteDrop, blackChestBlueDrop, blackChestOrangeDrop);
		DropRates[brownChestBasePrice] = new ChestDropRates(brownChestWhiteDrop, brownChestBlueDrop, brownChestOrangeDrop);
		DropRates[blueChestBasePrice] = new ChestDropRates(blueChestWhiteDrop, blueChestBlueDrop, blueChestOrangeDrop);
		DropRates[goldChestBasePrice] = new ChestDropRates(goldChestWhiteDrop, goldChestBlueDrop, goldChestOrangeDrop);

		float defaultEasy = EasyChestPriceDivisor;
		float defaultNormal = NormalChestPriceDivisor;
		float defaultGamer = GamerChestPriceDivisor;

		EasyChestPriceDivisor = Config.BindMoreThanZero("Cost", "EasyChestPriceDivisor", EasyChestPriceDivisor, "The divisor which decreases how quickly the price of opening a chest will increase as days progress. Every day beyond Day 3, chests increase in price every day by their base price divided by this number. So for example, with a setting of 5, a 25-gold chest price goes up by 25/5 = 5 gold per day.").Value;
		NormalChestPriceDivisor = Config.BindMoreThanZero("Cost", "NormalChestPriceDivisor", NormalChestPriceDivisor, "The divisor which decreases how quickly the price of opening a chest will increase as days progress.").Value;
		GamerChestPriceDivisor = Config.BindMoreThanZero("Cost", "GamerChestPriceDivisor", GamerChestPriceDivisor, "The divisor which decreases how quickly the price of opening a chest will increase as days progress.").Value;

		WhitePowerups = LoadPowerups(whitePowerups);
		BluePowerups = LoadPowerups(bluePowerups);
		OrangePowerups = LoadPowerups(orangePowerups);
		Config.Save();

		Logger.LogInfo("MuckConfigurePowerupDrops loaded!");
		Harmony.CreateAndPatchAll(typeof(ConfigurePowerupsPatch), null);
		// We only patch this stuff if it's different to default
		if (defaultEasy != EasyChestPriceDivisor || defaultNormal != NormalChestPriceDivisor || defaultGamer != GamerChestPriceDivisor)
		{
			Harmony.CreateAndPatchAll(typeof(GetChestPriceMultiplierPatch), null);
		}
	}
	private NameWeight[] LoadPowerups(string[] names)
	{
		NameWeight[] ncs = new NameWeight[names.Length];
		for (int i = 0; i < names.Length; i++)
		{
			string name = names[i];
			int count = Config.BindNonNegative("Weight", name + "_Weight", 1, string.Concat("The weight of the \"", name, "\" powerup across all pools, cumulative with being specified multiple times.")).Value;
			ncs[i] = new NameWeight(name, count);
		}
		return ncs;
	}
}
