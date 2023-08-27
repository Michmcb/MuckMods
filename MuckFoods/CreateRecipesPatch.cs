namespace MuckFoods;

using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BepInEx;

public class CreateRecipesPatch
{
	public static InventoryItem[] NewFoods = new InventoryItem[0];
	[HarmonyPatch(typeof(ItemManager), "InitAllItems")]
	[HarmonyPostfix]
	private static void InitializeFoods()
	{
		var apple = ItemManager.Instance.allItems[45];
		var bowl = ItemManager.Instance.allItems[46];
		var dough = ItemManager.Instance.allItems[47];
		var flax = ItemManager.Instance.allItems[49];
		var rawMeat = ItemManager.Instance.allItems[50];
		var gulponShroom = ItemManager.Instance.allItems[51];
		var ligonShroom = ItemManager.Instance.allItems[52];
		var slurbonShroom = ItemManager.Instance.allItems[53];
		var sugonShroom = ItemManager.Instance.allItems[54];

		var bread = ItemManager.Instance.allItems[56];
		var cookedMeat = ItemManager.Instance.allItems[57];
		var applePie = ItemManager.Instance.allItems[58];
		var meatPie = ItemManager.Instance.allItems[59];

		var meatSoup = ItemManager.Instance.allItems[60];
		var purpleSoup = ItemManager.Instance.allItems[61];
		var redSoup = ItemManager.Instance.allItems[62];
		var weirdSoup = ItemManager.Instance.allItems[63];
		var yellowSoup = ItemManager.Instance.allItems[64];

		// Adjust vanilla foods so things don't get too insane
		// Meat soup and cooked meat are super easy to get, so they are nerfed
		if (Plugin.RebalanceFoods)
		{
			apple.hunger = 5f;
			rawMeat.hunger = 5f;
			gulponShroom.heal = 10f;
			ligonShroom.hunger = 10f;
			sugonShroom.stamina = 10f;
			SetStats(slurbonShroom, 10f, 10f, 10f);
			SetStats(cookedMeat, 15f, 15f, 5f);
			SetStats(meatSoup, 20f, 20f, 10f);
			SetStats(applePie, 35f, 35f, 35f);
			SetStats(meatPie, 55f, 55f, 40f);
			SetStats(purpleSoup, 10f, 10f, 30f);
			SetStats(redSoup, 30f, 10f, 10f);
			SetStats(weirdSoup, 30f, 30f, 30f);
			SetStats(yellowSoup, 10f, 30f, 10f);
		}

		var shrooms = new (InventoryItem room, string name, string colour, InventoryItem soup)[]
		{
			(gulponShroom, "Gulpon", "Red", redSoup),
			(ligonShroom, "Ligon", "Yellow", yellowSoup),
			(sugonShroom, "Sugon", "Purple", purpleSoup),
			(slurbonShroom, "Slurbon", "Weird", weirdSoup),
		};

		// The way we have made things work is basically every food restores the same as if you cooked everything and ate it, plus 5 to everything.
		// For meat, we consider meat soup, instead of cooked meat.
		List<InventoryItem> newItems = new()
		{
			CreateFood(meatSoup, "Applesauce", "perfect for weak teeth", CraftRequirements(apple, bowl), apple),
			CreateFood(applePie, "Pie Pie", "it's pie flavoured", CraftRequirements(dough, bowl), bread),
			CreateFood(bread, "Flaxseed Bread", "what else can one do with flax?", CraftRequirements(dough, flax), bread),
			CreateFood(meatSoup, "Apple Meat Stew", "yummy", CraftRequirements(apple, rawMeat, bowl), apple, meatSoup),
			CreateFood(applePie, "Apple Meat Pie", "dessert and dinner in one", CraftRequirements(apple, rawMeat, dough, bowl), apple, rawMeat, bread),
			CreateFood(weirdSoup, "Less Weird Soup", "food for a fun guy", CraftRequirements(gulponShroom, ligonShroom, sugonShroom, bowl), redSoup, yellowSoup, purpleSoup),
		};

		int i = 0;
		var descrShroomPie = new string[] { "spicy pie", "pie with shroom", "pie in the sky", "mystery pie" };
		var descrShroomMeatPie = new string[] { "don't die pie", "could feed an army (or just you)", "fills you with energy", "sus, savoury, scrumptious" };
		var descrShroomApplePie = new string[] { "pretty healthy", "pie with apple and shroom", "pretty fast pie", "who needs meat anyway?" };
		var descrShroomAppleStew = new string[] { "red and vegan friendly", "it's sweet and savoury", "better than nothing ig", "lv 5 vegans only" };
		var descrShroomMeatStew = new string[] { "savoury stew", "tender meat and shroom", "tasty stew for when you're tired", "i think the meat became rainbows" };
		var descrShroomAppleMeatStew = new string[] { "reddest food ever made", "a real hunter-gatherer's meal", "keeps you going", "the heartiest stew in the land" };
		foreach (var mush in shrooms)
		{
			newItems.Add(CreateFood(applePie, mush.name + " Shroom Pie", descrShroomPie[i], CraftRequirements(dough, mush.room, bowl), bread, mush.soup));
			newItems.Add(CreateFood(applePie, string.Concat("Apple and ", mush.name, " Shroom Pie"), descrShroomMeatPie[i], CraftRequirements(dough, apple, mush.room, bowl), bread, apple, mush.soup));
			newItems.Add(CreateFood(applePie, string.Concat("Meat and ", mush.name, " Shroom Pie"), descrShroomApplePie[i], CraftRequirements(dough, rawMeat, mush.room, bowl), bread, meatSoup, mush.soup));
			newItems.Add(CreateFood(mush.soup, string.Concat("Apple and ", mush.name, " Shroom Stew"), descrShroomAppleStew[i], CraftRequirements(apple, mush.room, bowl), apple, mush.soup));
			newItems.Add(CreateFood(mush.soup, string.Concat("Meat and ", mush.name, " Shroom Stew"), descrShroomMeatStew[i], CraftRequirements(rawMeat, mush.room, bowl), meatSoup, mush.soup));
			newItems.Add(CreateFood(mush.soup, mush.colour + " Gourmet Stew", descrShroomAppleMeatStew[i], CraftRequirements(rawMeat, apple, mush.room, bowl), meatSoup, apple, mush.soup));
			++i;
		}

		// Now we assign IDs to all the newly created items
		// The way IDs work is basically Muck has the array "allScriptableItems", and IDs are assigned based on index.
		// So we loop over our new items in order and assign them IDs, add them to the dictionary, then concat it into the array
		int id = ItemManager.Instance.allScriptableItems.Length;
		foreach (var item in newItems)
		{
			item.id = id++;
			ItemManager.Instance.allItems.Add(item.id, item);
			Plugin.Log.LogItemCreated(item);
		}
		ItemManager.Instance.allScriptableItems = ItemManager.Instance.allScriptableItems.Concat(newItems).ToArray();
		NewFoods = newItems.ToArray();
	}
	[HarmonyPatch(typeof(CauldronUI), "Awake")]
	[HarmonyPostfix]
	private static void AddFoodsToCauldron()
	{
		CauldronUI.Instance.processableFood = CauldronUI.Instance.processableFood.Concat(NewFoods).ToArray();
	}
	private static InventoryItem CreateFood(InventoryItem baseItem, string name, string descr, InventoryItem.CraftRequirement[] requirements, params InventoryItem[] restoresTheSumOfThisStuff)
	{
		float heal = Plugin.HealthBonus[restoresTheSumOfThisStuff.Length - 1];
		float hunger = Plugin.HungerBonus[restoresTheSumOfThisStuff.Length - 1];
		float stamina = Plugin.StaminaBonus[restoresTheSumOfThisStuff.Length - 1];
		foreach (var s in restoresTheSumOfThisStuff)
		{
			heal += s.heal;
			hunger += s.hunger;
			stamina += s.stamina;
		}
		InventoryItem food = Clone(baseItem);
		food.name = name;
		food.description = descr;
		food.requirements = requirements;
		SetStats(food, heal, hunger, stamina);
		Plugin.Log.LogInfo(string.Concat("Added food \"", name, "\" which restores ", heal, " Health, ", hunger, " Hunger, ", stamina, " Stamina. Made from: ", string.Join(",", food.requirements.Select(x => x.item.name))));
		return food;
	}
	private static InventoryItem.CraftRequirement[] CraftRequirements(params InventoryItem[] items)
	{
		InventoryItem.CraftRequirement[] r = new InventoryItem.CraftRequirement[items.Length];
		for (int i = 0; i < items.Length; i++)
		{
			r[i] = new() { item = items[i], amount = 1 };
		}
		return r;
	}
	private static void SetStats(InventoryItem food, float heal, float hunger, float stamina)
	{
		food.heal = heal;
		food.hunger = hunger;
		food.stamina = stamina;
	}
	private static InventoryItem Clone(InventoryItem item)
	{
		InventoryItem i = ScriptableObject.CreateInstance<InventoryItem>();
		i.important = item.important;
		i.id = item.id;
		i.name = item.name;
		i.description = item.description;
		i.type = item.type;
		i.tier = item.tier;
		i.sprite = item.sprite;
		i.material = item.material;
		i.mesh = item.mesh;
		i.rotationOffset = item.rotationOffset;
		i.positionOffset = item.positionOffset;
		i.scale = item.scale;
		i.stackable = item.stackable;
		i.amount = item.amount;
		i.max = item.max;
		i.resourceDamage = item.resourceDamage;
		i.attackDamage = item.attackDamage;
		i.attackSpeed = item.attackSpeed;
		i.attackRange = item.attackRange;
		i.sharpness = item.sharpness;
		i.craftable = item.craftable;
		i.unlockWithFirstRequirementOnly = item.unlockWithFirstRequirementOnly;
		i.stationRequirement = item.stationRequirement;
		i.buildable = item.buildable;
		i.grid = item.grid;
		i.prefab = item.prefab;
		i.buildRotation = item.buildRotation;
		i.processable = item.processable;
		i.processType = item.processType;
		i.processedItem = item.processedItem;
		i.processTime = item.processTime;
		i.heal = item.heal;
		i.hunger = item.hunger;
		i.stamina = item.stamina;
		i.armor = item.armor;
		i.swingFx = item.swingFx;
		i.bowComponent = item.bowComponent;
		i.armorComponent = item.armorComponent;
		i.fuel = item.fuel;
		i.tag = item.tag;
		i.rarity = item.rarity;
		i.attackTypes = item.attackTypes;
		i.hideFlags = item.hideFlags;
		i.craftAmount = item.amount;
		i.requirements = item.requirements;
		return i;
	}
}