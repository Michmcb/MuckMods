namespace MuckMetalCoins;

using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class CreateRecipesPatch
{
	public static InventoryItem[] CoinRecipes = new InventoryItem[0];
	[HarmonyPatch(typeof(ItemManager), "InitAllItems")]
	[HarmonyPostfix]
	private static void ItemManager_InitAllItems()
	{
		// If they specified the item used to make Coins in vanilla (i.e. Gold bar) then we modify the Coins vanilla recipe
		Dictionary<string, Amt> dict = new(Plugin.ItemsCraftableIntoCoins);
		InventoryItem coin = ItemManager.Instance.allItems[3];
		int vanillaCoinIngredientId = -1;
		InventoryItem.CraftRequirement vanillaCoinIngredient = coin.requirements.FirstOrDefault();
		if (vanillaCoinIngredient != null)
		{
			vanillaCoinIngredientId = vanillaCoinIngredient.item.id;
			if (dict.TryGetValue(vanillaCoinIngredient.item.name, out Amt amt))
			{
				coin.craftAmount = amt.Coins;
				vanillaCoinIngredient.amount = amt.Required;
				Plugin.Log.LogInfo(string.Concat("Modified recipe to make ", amt.Coins, " Coins from ", amt.Required, " ", vanillaCoinIngredient.item.name));
				dict.Remove(vanillaCoinIngredient.item.name);
			}
		}
		List<InventoryItem> newCoins = new();
		foreach (var item in ItemManager.Instance.allItems.Values)
		{
			if (dict.TryGetValue(item.name, out Amt amt))
			{
				if (item.id != vanillaCoinIngredientId)
				{
					// This is a pretty hacky way to add the new coins, as technically we're adding a different item which has the exact same details as the original
					// Because the ID is the same, Muck allows the items to stack together
					// Plus the inventory check for money specifically looks for items with the name "Coin"
					// So, since our new coin is indistinguishable from the vanilla coins, it works fine
					InventoryItem newCoin = ScriptableObject.CreateInstance<InventoryItem>();
					newCoin.important = coin.important;
					newCoin.id = coin.id;
					newCoin.name = coin.name;
					newCoin.description = coin.description;
					newCoin.type = coin.type;
					newCoin.tier = coin.tier;
					newCoin.sprite = coin.sprite;
					newCoin.material = coin.material;
					newCoin.mesh = coin.mesh;
					newCoin.rotationOffset = coin.rotationOffset;
					newCoin.positionOffset = coin.positionOffset;
					newCoin.scale = coin.scale;
					newCoin.stackable = coin.stackable;
					newCoin.amount = coin.amount;
					newCoin.max = coin.max;
					newCoin.resourceDamage = coin.resourceDamage;
					newCoin.attackDamage = coin.attackDamage;
					newCoin.attackSpeed = coin.attackSpeed;
					newCoin.attackRange = coin.attackRange;
					newCoin.sharpness = coin.sharpness;
					newCoin.craftable = coin.craftable;
					newCoin.unlockWithFirstRequirementOnly = coin.unlockWithFirstRequirementOnly;
					newCoin.stationRequirement = coin.stationRequirement;
					newCoin.buildable = coin.buildable;
					newCoin.grid = coin.grid;
					newCoin.prefab = coin.prefab;
					newCoin.buildRotation = coin.buildRotation;
					newCoin.processable = coin.processable;
					newCoin.processType = coin.processType;
					newCoin.processedItem = coin.processedItem;
					newCoin.processTime = coin.processTime;
					newCoin.heal = coin.heal;
					newCoin.hunger = coin.hunger;
					newCoin.stamina = coin.stamina;
					newCoin.armor = coin.armor;
					newCoin.swingFx = coin.swingFx;
					newCoin.bowComponent = coin.bowComponent;
					newCoin.armorComponent = coin.armorComponent;
					newCoin.fuel = coin.fuel;
					newCoin.tag = coin.tag;
					newCoin.rarity = coin.rarity;
					newCoin.attackTypes = coin.attackTypes;
					newCoin.hideFlags = coin.hideFlags;

					newCoin.craftAmount = amt.Coins;
					newCoin.requirements = new InventoryItem.CraftRequirement[] { new() { amount = amt.Required, item = item } };
					newCoins.Add(newCoin);
					Plugin.Log.LogInfo(string.Concat("Added recipe to make ", amt.Coins, " Coins from ", amt.Required, " ", item.name));
				}
				dict.Remove(item.name);
			}
		}
		foreach (string itemName in dict.Keys)
		{
			Plugin.Log.LogError("This item was not found: " + itemName);
		}
		CoinRecipes = newCoins.ToArray();
	}
	[HarmonyPatch(typeof(CraftingUI), "Awake")]
	[HarmonyPostfix]
	private static void CraftingUI_Awake(CraftingUI __instance)
	{
		if (__instance.name == "AnvilNew")
		{
			// First tab for coins
			__instance.tabs[0].items = __instance.tabs[0].items.Concat(CoinRecipes).ToArray();
		}
	}
}