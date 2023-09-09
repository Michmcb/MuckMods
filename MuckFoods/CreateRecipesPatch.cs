namespace MuckFoods;

using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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

		// Just make sure inedible items don't add any stats to foods
		SetStats(bowl, 0f, 0f, 0f);
		SetStats(flax, 0f, 0f, 0f);

		// We adjust vanilla foods so things don't get too insane
		// Meat soup and cooked meat are super easy to get, so they are nerfed
		if (Plugin.RebalanceFoods)
		{
			SetStats(apple, 4f, 4f, 4f);
			SetStats(rawMeat, 6f, 6f, 3f);
			gulponShroom.heal = 10f;
			ligonShroom.hunger = 10f;
			sugonShroom.stamina = 10f;
			SetStats(slurbonShroom, 10f, 10f, 10f);

			SetStats(redSoup, 25f, 10f, 10f);
			SetStats(yellowSoup, 10f, 25f, 10f);
			SetStats(purpleSoup, 10f, 10f, 25f);
			SetStats(weirdSoup, 30f, 30f, 30f);

			Restoration.FromIngredients(rawMeat).Apply(cookedMeat);
			Restoration.FromIngredients(cookedMeat, bowl).Apply(meatSoup);
			Restoration.FromIngredients(apple, bread, bowl).Apply(applePie);
			Restoration.FromIngredients(cookedMeat, bowl, bread).Apply(meatPie);
		}

		var bakedApple = CreateFood(apple, Resources.AppleBakedSprite, "Baked Apple", "soft and sweet, but looks familiar somehow...", CraftRequirements(apple), apple);
		var toastyGulponShroom = CreateFood(gulponShroom, Resources.ShroomRedToastySprite, "Toasted Gulpon Shroom", "nice and hot!", CraftRequirements(gulponShroom), gulponShroom);
		var toastyLigonShroom = CreateFood(ligonShroom, Resources.ShroomYellowToastySprite, "Toasted Ligon Shroom", "tasty and filling", CraftRequirements(ligonShroom), ligonShroom);
		var toastySugonShroom = CreateFood(sugonShroom, Resources.ShroomPurpleToastySprite, "Toasted Sugon Shroom", "keeps you jumping at your best", CraftRequirements(sugonShroom), sugonShroom);
		var toastySlurbonShroom = CreateFood(slurbonShroom, Resources.ShroomSusToastySprite, "Toasted Slurbon Shroom", "still kinda sus...", CraftRequirements(slurbonShroom), slurbonShroom);
		bakedApple.processTime = cookedMeat.processTime;
		toastyGulponShroom.processTime = cookedMeat.processTime;
		toastyLigonShroom.processTime = cookedMeat.processTime;
		toastySugonShroom.processTime = cookedMeat.processTime;
		toastySlurbonShroom.processTime = cookedMeat.processTime;

		List<InventoryItem> newItems = new()
		{
			bakedApple,
			toastyGulponShroom,
			toastyLigonShroom,
			toastySugonShroom,
			toastySlurbonShroom,
			CreateFood(applePie, applePie.sprite, "Pie Pie", "it's pie flavoured", CraftRequirements(dough, bowl), bread, bowl),
			CreateFood(bread, Resources.FlaxseedBreadSprite, "Flaxseed Bread", "what else can you do with flax?", CraftRequirements(dough, flax), bread, flax),
			CreateFood(meatSoup, Resources.AppleMeatStewSprite, "Apple Meat Stew", "yummy", CraftRequirements(apple, rawMeat, bowl), apple, cookedMeat, bowl),
			CreateFood(applePie, Resources.PieAppleMeatSprite, "Apple Meat Pie", "dessert and dinner in one", CraftRequirements(apple, rawMeat, dough, bowl), apple, cookedMeat, bread, bowl),
			CreateFood(weirdSoup, Resources.LessWeirdSoupSprite, "Less Weird Soup", "food for a fun guy", CraftRequirements(gulponShroom, ligonShroom, sugonShroom, bowl), gulponShroom, ligonShroom, sugonShroom, bowl)
		};

		var shrooms = new (InventoryItem room, string name, string colour, InventoryItem soup)[]
		{
			(gulponShroom, "Gulpon", "Red", redSoup),
			(ligonShroom, "Ligon", "Yellow", yellowSoup),
			(sugonShroom, "Sugon", "Purple", purpleSoup),
			(slurbonShroom, "Slurbon", "Weird", weirdSoup),
		};
		var descrShroomPie = new DescrSprite[]
		{
			new("spicy pie", Resources.PieRedSprite),
			new("pie with shroom", Resources.PieYellowSprite),
			new("pie in the sky", Resources.PiePurpleSprite),
			new("mystery pie", Resources.PieSusSprite),
		};
		var descrShroomApplePie = new DescrSprite[]
		{
			new("pretty healthy", Resources.PieRedAppleSprite),
			new("pie with apple and shroom", Resources.PieYellowAppleSprite),
			new("pretty fast pie", Resources.PiePurpleAppleSprite),
			new("who needs meat anyway?", Resources.PieSusAppleSprite),
		};
		var descrShroomMeatPie = new DescrSprite[]
		{
			new("don't die pie", Resources.PieRedMeatSprite),
			new("could feed an army (or just you)", Resources.PieYellowMeatSprite),
			new("fills you with energy", Resources.PiePurpleMeatSprite),
			new("sus, savoury, scrumptious", Resources.PieSusMeatSprite),
		};
		var descrShroomAppleStew = new DescrSprite[]
		{
			new("red and vegan friendly", Resources.SoupRedAppleSprite),
			new("it's sweet and savoury", Resources.SoupYellowAppleSprite),
			new("better than nothing ig", Resources.SoupPurpleAppleSprite),
			new("lv 5 vegans only", Resources.SoupSusAppleSprite),
		};
		var descrShroomMeatStew = new DescrSprite[]
		{
			new("savoury stew", Resources.SoupRedMeatSprite),
			new("tender meat and shroom", Resources.SoupYellowMeatSprite),
			new("tasty stew for when you're tired", Resources.SoupPurpleMeatSprite),
			new("i think the meat became rainbows", Resources.SoupSusMeatSprite),
		};
		var descrShroomAppleMeatStew = new DescrSprite[]
		{
			new("reddest food ever made", Resources.SoupRedAppleMeatSprite),
			new("a real hunter-gatherer's meal", Resources.SoupYellowAppleMeatSprite),
			new("keeps you going", Resources.SoupPurpleAppleMeatSprite),
			new("the heartiest stew in the land", Resources.SoupSusAppleMeatSprite),
		};
		int i = 0;
		foreach (var mush in shrooms)
		{
			newItems.Add(CreateFood(applePie, descrShroomPie[i].Sprite, mush.name + " Shroom Pie", descrShroomPie[i].Descr, CraftRequirements(dough, mush.room, bowl), bread, mush.soup, bowl));
			newItems.Add(CreateFood(applePie, descrShroomApplePie[i].Sprite, string.Concat("Apple and ", mush.name, " Shroom Pie"), descrShroomApplePie[i].Descr, CraftRequirements(dough, apple, mush.room, bowl), bread, apple, mush.soup, bowl));
			newItems.Add(CreateFood(applePie, descrShroomMeatPie[i].Sprite, string.Concat("Meat and ", mush.name, " Shroom Pie"), descrShroomMeatPie[i].Descr, CraftRequirements(dough, rawMeat, mush.room, bowl), bread, cookedMeat, mush.soup, bowl));
			newItems.Add(CreateFood(mush.soup, descrShroomAppleStew[i].Sprite, string.Concat("Apple and ", mush.name, " Shroom Stew"), descrShroomAppleStew[i].Descr, CraftRequirements(apple, mush.room, bowl), apple, mush.soup, bowl));
			newItems.Add(CreateFood(mush.soup, descrShroomMeatStew[i].Sprite, string.Concat("Meat and ", mush.name, " Shroom Stew"), descrShroomMeatStew[i].Descr, CraftRequirements(rawMeat, mush.room, bowl), cookedMeat, mush.soup, bowl));
			newItems.Add(CreateFood(mush.soup, descrShroomAppleMeatStew[i].Sprite, mush.colour + " Gourmet Stew", descrShroomAppleMeatStew[i].Descr, CraftRequirements(rawMeat, apple, mush.room, bowl), cookedMeat, apple, mush.soup, bowl));
			++i;
		}
		applePie.sprite = Resources.PieAppleSprite;
		meatPie.sprite = Resources.PieMeatSprite;

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
	private static InventoryItem CreateFood(InventoryItem baseItem, Sprite sprite, string name, string descr, InventoryItem.CraftRequirement[] requirements, params InventoryItem[] restoresSumOf)
	{
		Plugin.Log.LogInfo("Making: " + name);
		Restoration resto = Restoration.FromIngredients(restoresSumOf);
		InventoryItem food = Clone(baseItem, name, descr, sprite, requirements, resto);
		Plugin.Log.LogInfo(string.Concat("Added food \"", name, "\" which restores ", resto.Heal, " Health, ", resto.Hunger, " Hunger, ", resto.Stamina, " Stamina. Made from: ", string.Join(",", food.requirements.Select(x => x.item.name))));
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
	private static InventoryItem Clone(InventoryItem item, string name, string description, Sprite sprite, InventoryItem.CraftRequirement[] requirements, Restoration resto)
	{
		InventoryItem i = ScriptableObject.CreateInstance<InventoryItem>();
		i.material = item.material;
		i.sprite = sprite;
		i.name = name;
		i.description = description;
		i.requirements = requirements;

		resto.Apply(i);

		// Not necessary
		//i.id = item.id;
		//i.heal = item.heal;
		//i.hunger = item.hunger;
		//i.stamina = item.stamina;

		i.important = false;
		i.type = InventoryItem.ItemType.Food;
		i.tier = item.tier;
		i.mesh = item.mesh;
		i.rotationOffset = item.rotationOffset;
		i.positionOffset = item.positionOffset;
		i.scale = item.scale;
		i.stackable = true;
		i.amount = 1;
		i.max = 69;
		i.resourceDamage = 1;
		i.attackDamage = 1;
		i.attackSpeed = 1;
		i.attackRange = 0;
		i.sharpness = 0;
		i.craftable = false;
		i.unlockWithFirstRequirementOnly = item.unlockWithFirstRequirementOnly;
		i.stationRequirement = item.stationRequirement;
		i.buildable = false;
		i.grid = false;
		i.prefab = item.prefab;
		i.buildRotation = item.buildRotation;
		i.processable = false;
		i.processType = item.processType;
		i.processedItem = item.processedItem;
		i.processTime = 5;
		i.armor = 0;
		i.swingFx = item.swingFx;
		i.bowComponent = item.bowComponent;
		i.armorComponent = item.armorComponent;
		i.fuel = item.fuel;
		i.tag = InventoryItem.ItemTag.Food;
		i.rarity = item.rarity;
		i.attackTypes = item.attackTypes;
		i.hideFlags = item.hideFlags;
		i.craftAmount = 1;
		return i;
	}
}