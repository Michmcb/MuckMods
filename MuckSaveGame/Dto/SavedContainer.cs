namespace MuckSaveGame.Dto
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Xml.Linq;

	public sealed class SavedContainer : ISaveData
	{
		public SavedContainer(int size, Position position, int rotation, IList<CellIdAmount> inventory)
		{
			Inventory = inventory;
			Size = size;
			Position = position;
			Rotation = rotation;
		}
		public int Size { get; }
		public Position Position { get; }
		public int Rotation { get; }
		public IList<CellIdAmount> Inventory { get; }
		public static SavedContainer Create(Chest chest)
		{
			int size = chest.chestSize;
			Position position = Position.FromVec3(chest.transform.root.position);
			int rotation = (int)chest.transform.rotation.eulerAngles.y;

			List<CellIdAmount> inventory = new();
			for (int i = 0; i < chest.chestSize; i++)
			{
				InventoryItem? item = chest.cells[i];
				if (item is not null)
				{
					inventory.Add(new(i, item.id, item.amount));
				}
			}

			return new(size, position, rotation, inventory);
		}
		public void SaveXml(XElement xml)
		{
			xml.AddElementValue(nameof(Size), Size);
			Position.SaveXml(xml.AddElement(nameof(Position)));
			xml.AddElementValue(nameof(Rotation), Rotation);
			xml.AddElementValues("Inventory", "_", Inventory, (x, v) => v.SaveXml(x));
		}
		public static SavedContainer Load(XElement xml)
		{
			int size = xml.RequiredElement("Size").ValueAsInt();
			int rotation = xml.RequiredElement("Rotation").ValueAsInt();
			Position position = Position.Load(xml.RequiredElement("Position"));
			List<CellIdAmount> inventory = xml.Element("Inventory")?.Elements().Select(CellIdAmount.Load).ToList() ?? new();

			return new SavedContainer(size, position, rotation, inventory);
		}
	}
}
