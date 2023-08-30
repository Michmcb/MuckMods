namespace MuckSaveGame.Dto
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Xml.Linq;

	public sealed class EntitySaveData
	{
		public EntitySaveData(List<SavedBuild> builds, List<SavedMob> mobs, List<SavedDroppedItem> droppedItems)
		{
			Builds = builds;
			Mobs = mobs;
			DroppedItems = droppedItems;
		}
		public List<SavedBuild> Builds { get; }
		public List<SavedMob> Mobs { get; }
		public List<SavedDroppedItem> DroppedItems { get; }
		public void SaveXml(XElement xml)
		{
			xml.AddElementValues("Builds", "_", Builds, (x, v) => v.SaveXml(x));
			xml.AddElementValues("Mobs", "_", Mobs, (x, v) => v.SaveXml(x));
			xml.AddElementValues("DroppedItems", "_", DroppedItems, (x, v) => v.SaveXml(x));
		}
		public static EntitySaveData Load(XElement xml)
		{
			List<SavedBuild> builds = xml.Element("Builds")?.Elements().Select(SavedBuild.Load).ToList() ?? new();
			List<SavedMob> mobs = xml.Element("Mobs")?.Elements().Select(SavedMob.Load).ToList() ?? new();
			List<SavedDroppedItem> droppedItems = xml.Element("DroppedItems")?.Elements().Select(SavedDroppedItem.Load).ToList() ?? new();

			return new EntitySaveData(builds, mobs, droppedItems);
		}
	}
}
