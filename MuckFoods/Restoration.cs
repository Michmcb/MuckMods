namespace MuckFoods;

using System;

public readonly struct Restoration
{
	public Restoration(float heal, float hunger, float stamina)
	{
		Heal = heal;
		Hunger = hunger;
		Stamina = stamina;
	}
	public float Heal { get; }
	public float Hunger { get; }
	public float Stamina { get; }
	public void Apply(InventoryItem item)
	{
		item.heal = Math.Max(0f, Heal);
		item.hunger = Math.Max(0f, Hunger);
		item.stamina = Math.Max(0f, Stamina);
	}
	public static Restoration FromItem(InventoryItem item)
	{
		return new Restoration(item.heal, item.hunger, item.stamina);
	}
	public static Restoration FromFloats(float[] values)
	{
		float heal = values.Length > 0 ? values[0] : 0f;
		float hunger = values.Length > 1 ? values[1] : 0f;
		float stamina = values.Length > 2 ? values[2] : 0f;
		return new Restoration(heal, hunger, stamina);
	}
	public static Restoration FromIngredients(params InventoryItem[] items)
	{
		float heal = Plugin.CookHealthBonus + Plugin.HealthBonus[items.Length - 1];
		float hunger = Plugin.CookHungerBonus + Plugin.HungerBonus[items.Length - 1];
		float stamina = Plugin.CookStaminaBonus + Plugin.StaminaBonus[items.Length - 1];
		foreach (var item in items)
		{
			Plugin.Log.LogInfo(string.Concat("Adding ", item.name, " ingredient value: ", item.heal, "/", item.hunger, "/", item.stamina));
			heal += item.heal;
			hunger += item.hunger;
			stamina += item.stamina;
		}
		return new Restoration(heal, hunger, stamina);
	}
}
