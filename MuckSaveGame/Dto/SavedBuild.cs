namespace MuckSaveGame.Dto
{
	using System.Xml.Linq;

	public sealed class SavedBuild
	{
		public SavedBuild(int itemId, Position position, int rotation)
		{
			ItemId = itemId;
			Position = position;
			Rotation = rotation;
		}
		public int ItemId { get; }
		public Position Position { get; }
		public int Rotation { get; }
		public void SaveXml(XElement xml)
		{
			xml.AddElementValue(nameof(ItemId), ItemId);
			Position.SaveXml(xml.AddElement(nameof(Position)));
			xml.AddElementValue(nameof(Rotation), Rotation);
		}
		public static SavedBuild Load(XElement xml)
		{
			int itemId = xml.RequiredElement("ItemId").ValueAsInt();
			Position position = Position.Load(xml.RequiredElement("Position"));
			int rotation = xml.RequiredElement("Rotation").ValueAsInt();
			return new SavedBuild(itemId, position, rotation);
		}
	}
}
