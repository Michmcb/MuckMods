namespace MuckArrows;

using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CreateRecipesPatch
{
	public static InventoryItem[] ArrowRecipes = new InventoryItem[0];
	[HarmonyPatch(typeof(ItemManager), "InitAllItems")]
	[HarmonyPostfix]
	private static void ItemManager_InitAllItems()
	{
		InventoryItem chunkiumBar = ItemManager.Instance.allItems[67];
		InventoryItem obamiumBar = ItemManager.Instance.allItems[71];
		InventoryItem ruby = ItemManager.Instance.allItems[89];
		InventoryItem rock = ItemManager.Instance.allItems[90];
		InventoryItem darkOakWood = ItemManager.Instance.allItems[92];
		InventoryItem wood = ItemManager.Instance.allItems[94];
		InventoryItem oakWood = ItemManager.Instance.allItems[95];
		InventoryItem arrowAdamantite = ItemManager.Instance.allItems[131];
		InventoryItem arrowFlint = ItemManager.Instance.allItems[133];
		InventoryItem arrowMithril = ItemManager.Instance.allItems[135];
		InventoryItem arrowSteel = ItemManager.Instance.allItems[136];

		arrowFlint.attackDamage = Plugin.FlintArrowDamage;
		arrowSteel.attackDamage = Plugin.SteelArrowDamage;
		arrowMithril.attackDamage = Plugin.MithrilArrowDamage;
		arrowAdamantite.attackDamage = Plugin.AdamantiteArrowDamage;

		// All arrows are always added, to avoid any issues with IDs changing if the user were to save, exit, change settings, then re-enter the game
		List<InventoryItem> newArrows = new()
		{
			NewArrow(arrowFlint, "Stone Arrow", "blunt arrows for a long distance slapp", Plugin.StoneArrowDamage, 4, arrowFlint.sprite, arrowFlint.sprite.texture, Plugin.CanCraftStoneArrows,  new() { amount = 2, item = wood }, new() { amount = 1, item = rock }),
			NewArrow(arrowFlint, "Obamium Arrow", "the 44th arrow ever invented", Plugin.ObamiumArrowDamage, 4, Resources.ObamiumArrowSprite, Resources.ObamiumArrowTexture, Plugin.CanCraftObamiumArrows, new() { amount = 2, item = darkOakWood }, new() { amount = 1, item = obamiumBar } ),
			NewArrow(arrowFlint, "Ruby Arrow", "shooting your arrows with style", Plugin.RubyArrowDamage, 4, Resources.RubyArrowSprite, Resources.RubyArrowTexture, Plugin.CanCraftRubyArrows, new() { amount = 2, item = darkOakWood }, new() { amount = 1, item = ruby } ),
			NewArrow(arrowFlint, "Chunkium Arrow", "how do these even fly?", Plugin.ChunkiumArrowDamage, 4, Resources.ChunkiumArrowSprite, Resources.ChunkiumArrowTexture, Plugin.CanCraftChunkiumArrows, new() { amount = 2, item = oakWood }, new() { amount = 1, item = chunkiumBar } ),
		};

		int id = ItemManager.Instance.allScriptableItems.Length;
		foreach (var arrow in newArrows)
		{
			arrow.id = id++;
			ItemManager.Instance.allItems.Add(arrow.id, arrow);
			Plugin.Log.LogItemCreated(arrow);
		}
		ItemManager.Instance.allScriptableItems = ItemManager.Instance.allScriptableItems.Concat(newArrows).ToArray();
		ArrowRecipes = newArrows.ToArray();
	}
	private static InventoryItem NewArrow(InventoryItem baseArrow, string name, string description, int attackDamage, int amount, Sprite sprite, Texture2D texture, bool craftable, params InventoryItem.CraftRequirement[] requirements)
	{
		InventoryItem a = ScriptableObject.CreateInstance<InventoryItem>();
		a.amount = amount;
		a.armor = baseArrow.armor;
		a.armorComponent = baseArrow.armorComponent;
		a.attackDamage = attackDamage;
		a.attackRange = baseArrow.attackRange;
		a.attackSpeed = baseArrow.attackSpeed;
		a.attackTypes = baseArrow.attackTypes;
		a.bowComponent = baseArrow.bowComponent;
		a.buildRotation = baseArrow.buildRotation;
		a.buildable = baseArrow.buildable;
		a.craftAmount = baseArrow.craftAmount;
		a.craftable = baseArrow.craftable;
		a.description = description;
		a.fuel = baseArrow.fuel;
		a.grid = baseArrow.grid;
		a.heal = baseArrow.heal;
		a.hideFlags = baseArrow.hideFlags;
		a.hunger = baseArrow.hunger;
		a.id = baseArrow.id;
		a.important = baseArrow.important;
		a.material = baseArrow.material;
		a.max = baseArrow.max;
		a.mesh = baseArrow.mesh;
		a.name = name;
		a.positionOffset = baseArrow.positionOffset;
		a.prefab = baseArrow.prefab;
		a.processTime = baseArrow.processTime;
		a.processType = baseArrow.processType;
		a.processable = baseArrow.processable;
		a.processedItem = baseArrow.processedItem;
		a.rarity = baseArrow.rarity;
		a.requirements = craftable ? requirements : Array.Empty<InventoryItem.CraftRequirement>();
		a.resourceDamage = baseArrow.resourceDamage;
		a.rotationOffset = baseArrow.rotationOffset;
		a.scale = baseArrow.scale;
		a.sharpness = baseArrow.sharpness;

		a.material = new Material(baseArrow.material)
		{
			mainTexture = texture,
			color = new Color(1f, 1f, 1f, 1f)
		};
		a.sprite = sprite;

		a.stackable = baseArrow.stackable;
		a.stamina = baseArrow.stamina;
		a.stationRequirement = baseArrow.stationRequirement;
		a.swingFx = baseArrow.swingFx;
		a.tag = baseArrow.tag;
		a.tier = baseArrow.tier;
		a.type = baseArrow.type;
		a.unlockWithFirstRequirementOnly = baseArrow.unlockWithFirstRequirementOnly;
		return a;
	}
	[HarmonyPatch(typeof(CraftingUI), "Awake")]
	[HarmonyPostfix]
	private static void CraftingUI_Awake(CraftingUI __instance)
	{
		if (__instance.name == "FletchingNew")
		{
			// Second tab for Arrows
			InventoryItem[] existingRecipes = __instance.tabs[1].items;
			InventoryItem[] recipes = new InventoryItem[existingRecipes.Length + ArrowRecipes.Length];
			existingRecipes.CopyTo(recipes, 0);
			ArrowRecipes.CopyTo(recipes, existingRecipes.Length);
			Array.Sort(recipes, (x, y) => x.attackDamage - y.attackDamage);
			__instance.tabs[1].items = recipes;
		}
	}
}