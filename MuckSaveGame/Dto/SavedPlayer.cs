namespace MuckSaveGame.Dto
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Xml.Linq;
	public sealed class SavedPlayer
	{
		public SavedPlayer(float health, int maxHealth, float stamina, float maxStamina, float shield, int maxShield, float hunger, float maxHunger, int draculaStacks, Position position, List<int> powerups, List<int> armor, List<bool> softUnlocks, List<bool> hardUnlocks, List<bool> stationUnlocks, IdAmount arrows, List<IdAmount> inventory)
		{
			Health = health;
			MaxHealth = maxHealth;
			Stamina = stamina;
			MaxStamina = maxStamina;
			Shield = shield;
			MaxShield = maxShield;
			Hunger = hunger;
			MaxHunger = maxHunger;
			DraculaStacks = draculaStacks;
			Position = position;
			Powerups = powerups;
			Armor = armor;
			SoftUnlocks = softUnlocks;
			HardUnlocks = hardUnlocks;
			StationUnlocks = stationUnlocks;
			Arrows = arrows;
			Inventory = inventory;
		}
		public float Health { get; set; }
		public int MaxHealth { get; set; }
		public float Stamina { get; set; }
		public float MaxStamina { get; set; }
		public float Shield { get; set; }
		public int MaxShield { get; set; }
		public float Hunger { get; set; }
		public float MaxHunger { get; set; }
		public int DraculaStacks { get; set; }
		public Position Position { get; set; }
		public List<int> Powerups { get; set; }
		public List<int> Armor { get; set; }
		public List<bool> SoftUnlocks { get; }
		public List<bool> HardUnlocks { get; }
		public List<bool> StationUnlocks { get; }
		public IdAmount Arrows { get; set; }
		public List<IdAmount> Inventory { get; set; }
		public static SavedPlayer CreateEmpty()
		{
			return new(0f, 0, 0f, 0f, 0f, 0, 0f, 0f, 0, default, new(), new(), new(), new(), new(), default, new());
		}
		public void SaveXml(XElement xml)
		{
			xml.AddElementValue(nameof(Health), Health);
			xml.AddElementValue(nameof(MaxHealth), MaxHealth);
			xml.AddElementValue(nameof(Stamina), Stamina);
			xml.AddElementValue(nameof(MaxStamina), MaxStamina);
			xml.AddElementValue(nameof(Shield), Shield);
			xml.AddElementValue(nameof(MaxShield), MaxShield);
			xml.AddElementValue(nameof(Hunger), Hunger);
			xml.AddElementValue(nameof(MaxHunger), MaxHunger);
			xml.AddElementValue(nameof(DraculaStacks), DraculaStacks);
			Position.SaveXml(xml.AddElement(nameof(Position)));
			xml.AddElementValues(nameof(Powerups), "_", Powerups, (x, v) => x.Value = v.ToString());
			xml.AddElementValues(nameof(Armor), "_", Armor, (x, v) => x.Value = v.ToString());
			xml.AddElementValues(nameof(SoftUnlocks), "_", SoftUnlocks, (x, v) => x.Value = v.ToString());
			xml.AddElementValues(nameof(HardUnlocks), "_", HardUnlocks, (x, v) => x.Value = v.ToString());
			xml.AddElementValues(nameof(StationUnlocks), "_", StationUnlocks, (x, v) => x.Value = v.ToString());
			Arrows.SaveXml(xml.AddElement(nameof(Arrows)));
			xml.AddElementValues(nameof(Inventory), "_", Inventory, (x, v) => v.SaveXml(x));
		}
		public static SavedPlayer Load(XElement xml)
		{
			float health = xml.RequiredElement("Health").ValueAsFloat();
			int maxHealth = xml.RequiredElement("MaxHealth").ValueAsInt();
			float stamina = xml.RequiredElement("Stamina").ValueAsFloat();
			float maxStamina = xml.RequiredElement("MaxStamina").ValueAsFloat();
			float shield = xml.RequiredElement("Shield").ValueAsFloat();
			int maxShield = xml.RequiredElement("MaxShield").ValueAsInt();
			float hunger = xml.RequiredElement("Hunger").ValueAsFloat();
			float maxHunger = xml.RequiredElement("MaxHunger").ValueAsFloat();
			int draculaStacks = xml.RequiredElement("DraculaStacks").ValueAsInt();
			Position position = Position.Load(xml.RequiredElement("Position"));
			IdAmount arrows = IdAmount.Load(xml.RequiredElement("Arrows"));
			List<int> powerups = xml.Element("Powerups")?.Elements().Select(x => x.ValueAsInt()).ToList() ?? new();
			List<int> armor = xml.Element("Armor")?.Elements().Select(x => x.ValueAsInt()).ToList() ?? new();
			List<bool> softUnlocks = xml.Element("SoftUnlocks")?.Elements().Select(x => x.ValueAsBool()).ToList() ?? new();
			List<bool> hardUnlocks = xml.Element("HardUnlocks")?.Elements().Select(x => x.ValueAsBool()).ToList() ?? new();
			List<bool> stationUnlocks = xml.Element("StationUnlocks")?.Elements().Select(x => x.ValueAsBool()).ToList() ?? new();
			List<IdAmount> inventory = xml.Element("Inventory")?.Elements().Select(IdAmount.Load).ToList() ?? new();

			return new SavedPlayer
			(
				health,
				maxHealth,
				stamina,
				maxStamina,
				shield,
				maxShield,
				hunger,
				maxHunger,
				draculaStacks,
				position,
				powerups,
				armor,
				softUnlocks,
				hardUnlocks,
				stationUnlocks,
				arrows,
				inventory
			);
		}
	}
}
