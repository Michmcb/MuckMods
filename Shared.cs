namespace BepInEx
{
	using BepInEx.Configuration;
	using BepInEx.Logging;
	using UnityEngine;

	public static class Extensions
	{
		public static void LogItemCreated(this ManualLogSource log, InventoryItem item)
		{
			log.LogInfo(string.Concat("Item \"",item.name,"\" created, ID ", item.id));
		}
		public static ConfigEntry<int> BindMoreThanZero(this ConfigFile config, string section, string key, int defaultValue, string description)
		{
			ConfigEntry<int> cfg = config.Bind(section, key, defaultValue, description);
			if (cfg.Value <= 0)
			{
				cfg.Value = defaultValue;
			}
			return cfg;
		}
		public static ConfigEntry<float> BindMoreThanZero(this ConfigFile config, string section, string key, float defaultValue, string description)
		{
			ConfigEntry<float> cfg = config.Bind(section, key, defaultValue, description);
			if (cfg.Value <= 0f)
			{
				cfg.Value = defaultValue;
			}
			return cfg;
		}
		public static InventoryItem Clone(this InventoryItem i)
		{
			InventoryItem i2 = ScriptableObject.CreateInstance<InventoryItem>();
			i2.amount = i.amount;
			i2.armor = i.armor;
			i2.armorComponent = i.armorComponent;
			i2.attackDamage = i.attackDamage;
			i2.attackRange = i.attackRange;
			i2.attackSpeed = i.attackSpeed;
			i2.attackTypes = i.attackTypes;
			i2.bowComponent = i.bowComponent;
			i2.buildRotation = i.buildRotation;
			i2.buildable = i.buildable;
			i2.craftAmount = i.craftAmount;
			i2.craftable = i.craftable;
			i2.description = i.description;
			i2.fuel = i.fuel;
			i2.grid = i.grid;
			i2.heal = i.heal;
			i2.hideFlags = i.hideFlags;
			i2.hunger = i.hunger;
			i2.id = i.id;
			i2.important = i.important;
			i2.material = i.material;
			i2.max = i.max;
			i2.mesh = i.mesh;
			i2.name = i.name;
			i2.positionOffset = i.positionOffset;
			i2.prefab = i.prefab;
			i2.processTime = i.processTime;
			i2.processType = i.processType;
			i2.processable = i.processable;
			i2.processedItem = i.processedItem;
			i2.rarity = i.rarity;
			i2.requirements = i.requirements;
			i2.resourceDamage = i.resourceDamage;
			i2.rotationOffset = i.rotationOffset;
			i2.scale = i.scale;
			i2.sharpness = i.sharpness;
			i2.sprite = i.sprite;
			i2.stackable = i.stackable;
			i2.stamina = i.stamina;
			i2.stationRequirement = i.stationRequirement;
			i2.swingFx = i.swingFx;
			i2.tag = i.tag;
			i2.tier = i.tier;
			i2.type = i.type;
			i2.unlockWithFirstRequirementOnly = i.unlockWithFirstRequirementOnly;
			return i2;
		}
	}
}