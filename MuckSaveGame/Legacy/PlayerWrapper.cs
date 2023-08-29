namespace MuckSaveGame.Legacy
{
	using System;
	using System.Collections.Generic;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	[Serializable]
	public class PlayerWrapper
	{
		public float health;
		public int maxHealth;
		public float stamina;
		public float maxStamina;
		public float shield;
		public int maxShield;
		public float hunger;
		public float maxHunger;

		public int draculaHpIncrease;

		public List<int> powerups;
		public int[] armor;

		public float[] position;

		public List<SerializableTuple<int, int>> inventory = new();
		public SerializableTuple<int, int> arrows;
	}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
