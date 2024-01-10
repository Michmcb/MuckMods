namespace MuckCharcoal;

using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CreateRecipesPatch
{
	[HarmonyPatch(typeof(ItemManager), "InitAllItems")]
	[HarmonyPostfix]
	private static void AddCoalRecipes()
	{
		// Items are only one-to-one processable.
		var coal = ItemManager.Instance.allItems[2];
		InventoryItem charcoal = ScriptableObject.CreateInstance<InventoryItem>();
		charcoal.important = coal.important;
		charcoal.name = "Charcoal";
		charcoal.description = "crispy wood";
		charcoal.type = coal.type;
		charcoal.tier = coal.tier;
		charcoal.sprite = coal.sprite;
		charcoal.material = coal.material;
		charcoal.mesh = coal.mesh;
		charcoal.rotationOffset = coal.rotationOffset;
		charcoal.positionOffset = coal.positionOffset;
		charcoal.scale = coal.scale;
		charcoal.stackable = coal.stackable;
		charcoal.amount = coal.amount;
		charcoal.max = coal.max;
		charcoal.resourceDamage = coal.resourceDamage;
		charcoal.attackDamage = coal.attackDamage;
		charcoal.attackSpeed = coal.attackSpeed;
		charcoal.attackRange = coal.attackRange;
		charcoal.sharpness = coal.sharpness;
		charcoal.craftable = coal.craftable;
		charcoal.unlockWithFirstRequirementOnly = coal.unlockWithFirstRequirementOnly;
		charcoal.stationRequirement = coal.stationRequirement;
		charcoal.buildable = coal.buildable;
		charcoal.grid = coal.grid;
		charcoal.prefab = coal.prefab;
		charcoal.buildRotation = coal.buildRotation;
		charcoal.processable = coal.processable;
		charcoal.processType = coal.processType;
		charcoal.processedItem = coal.processedItem;
		charcoal.processTime = coal.processTime;
		charcoal.heal = coal.heal;
		charcoal.hunger = coal.hunger;
		charcoal.stamina = coal.stamina;
		charcoal.armor = coal.armor;
		charcoal.swingFx = coal.swingFx;
		charcoal.bowComponent = coal.bowComponent;
		charcoal.armorComponent = coal.armorComponent;
		charcoal.tag = coal.tag;
		charcoal.rarity = coal.rarity;
		charcoal.attackTypes = coal.attackTypes;
		charcoal.hideFlags = coal.hideFlags;
		charcoal.craftAmount = 1;
		charcoal.requirements = coal.requirements;

		// Making it have 3 uses means there's actually a point to making Charcoal (besides just inventory organization)
		// If we say, have 2 stacks of wood and used that as fuel to burn the wood we'd make 1 stack of charcoal
		// And if it had 2 uses, then we'd be right back to where we started!
		// So 3 uses means every time we make a piece of charcoal, we've effectively made 1 extra piece of fuel
		charcoal.fuel = ScriptableObject.CreateInstance<ItemFuel>();
		charcoal.fuel.speedMultiplier = coal.fuel.speedMultiplier;
		charcoal.fuel.hideFlags = coal.fuel.hideFlags;
		charcoal.fuel.maxUses = Plugin.MaxUses;
		charcoal.fuel.currentUses = Plugin.MaxUses;
		charcoal.fuel.name = coal.name;
		
		charcoal.id = ItemManager.Instance.allItems.Keys.DefaultIfEmpty(0).Max() + 1;
		ItemManager.Instance.allItems.Add(charcoal.id, charcoal);
		Plugin.Log.LogItemCreated(charcoal);
		ItemManager.Instance.allScriptableItems = ItemManager.Instance.allScriptableItems.Concat(new InventoryItem[1] { charcoal }).ToArray();

		HashSet<string> processableItemNames = new(Plugin.ProcessableItems);

		foreach (var item in ItemManager.Instance.allItems.Values)
		{
			if (processableItemNames.Contains(item.name))
			{
				Plugin.Log.LogInfo("Making item smeltable into charcoal: " + item.name);
				item.processable = true;
				item.processType = InventoryItem.ProcessType.Smelt;
				item.processTime = Plugin.WoodProcessTime;
				item.processedItem = charcoal;
				processableItemNames.Remove(item.name);
			}
		}
		// Anything leftover is something the config file specified but wasn't found, so report that as an error
		foreach (var notFoundItem in processableItemNames)
		{
			Plugin.Log.LogError("This item was not found: " + notFoundItem);
		}
	}
}