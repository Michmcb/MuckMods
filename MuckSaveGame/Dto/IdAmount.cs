namespace MuckSaveGame.Dto
{
	using System.Xml.Linq;

	public readonly struct IdAmount
	{
		public IdAmount(int itemId, int amount)
		{
			ItemId = itemId;
			Amount = amount;
		}
		public int ItemId { get; }
		public int Amount { get; }
		public static IdAmount FromItem(InventoryItem item)
		{
			return new(item.id, item.amount);
		}
		public void SaveXml(XElement xml)
		{
			xml.AddElementValue("ItemId", ItemId);
			xml.AddElementValue("Amount", Amount);
		}
		public static IdAmount Load(XElement xml)
		{
			int itemId = xml.RequiredElement("ItemId").ValueAsInt();
			int amount = xml.RequiredElement("Amount").ValueAsInt();
			return new IdAmount(itemId, amount);
		}
	}
}
