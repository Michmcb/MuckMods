namespace MuckSaveGame.Dto
{
	using System.Xml.Linq;

	public sealed class SavedDroppedItem
	{
		public SavedDroppedItem(IdAmount item, Position position)
		{
			Item = item;
			Position = position;
		}
		public IdAmount Item { get; }
		public Position Position { get; }
		public void SaveXml(XElement xml)
		{
			Item.SaveXml(xml.AddElement(nameof(Item)));
			Position.SaveXml(xml.AddElement(nameof(Position)));
		}
		public static SavedDroppedItem Load(XElement xml)
		{
			IdAmount item = IdAmount.Load(xml.RequiredElement("Item"));
			Position position = Position.Load(xml.RequiredElement("Position"));
			return new SavedDroppedItem(item, position);
		}
	}
}
