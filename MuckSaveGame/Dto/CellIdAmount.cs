namespace MuckSaveGame.Dto
{
	using System.Xml.Linq;

	public readonly struct CellIdAmount
	{
		public CellIdAmount(int cell, int itemId, int amount)
		{
			Cell = cell;
			ItemId = itemId;
			Amount = amount;
		}
		public int Cell { get; }
		public int ItemId { get; }
		public int Amount { get; }
		public void SaveXml(XElement xml)
		{
			xml.AddElementValue("ItemId", ItemId);
			xml.AddElementValue("Amount", Amount);
			xml.AddElementValue("Cell", Cell);
		}
		public static CellIdAmount Load(XElement xml)
		{
			int cell = xml.RequiredElement("Cell").ValueAsInt();
			int itemId = xml.RequiredElement("ItemId").ValueAsInt();
			int amount = xml.RequiredElement("Amount").ValueAsInt();

			return new CellIdAmount(cell, itemId, amount);
		}
	}
}
