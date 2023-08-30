namespace MuckSaveGame.Dto
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Xml.Linq;

	public sealed class BoatSaveData
	{
		public BoatSaveData(bool isBoatMarked, bool isBoatFound, bool isBoatFinished, bool areGemsMarked, List<int> repairedObjectIds)
		{
			IsBoatMarked = isBoatMarked;
			IsBoatFound = isBoatFound;
			IsBoatFinished = isBoatFinished;
			AreGemsMarked = areGemsMarked;
			RepairedObjectIds = repairedObjectIds;
		}
		public bool IsBoatMarked { get; }
		public bool IsBoatFound { get; }
		public bool IsBoatFinished { get; }
		public bool AreGemsMarked { get; }
		public List<int> RepairedObjectIds { get; }
		public void SaveXml(XElement xml)
		{
			xml.AddElementValue(nameof(IsBoatMarked), IsBoatMarked);
			xml.AddElementValue(nameof(IsBoatFound), IsBoatFound);
			xml.AddElementValue(nameof(IsBoatFinished), IsBoatFinished);
			xml.AddElementValue(nameof(AreGemsMarked), AreGemsMarked);
			xml.AddElementValues(nameof(RepairedObjectIds), "_", RepairedObjectIds, (x, v) => x.Value = v.ToString());
		}
		public static BoatSaveData Load(XElement xml)
		{
			// If they're missing, we'll just assume false for these
			bool isBoatMarked = xml.Element("IsBoatMarked")?.ValueAsBool() ?? false;
			bool isBoatFound = xml.Element("IsBoatFound")?.ValueAsBool() ?? false;
			bool isBoatFinished = xml.Element("IsBoatFinished")?.ValueAsBool() ?? false;
			bool areGemsMarked = xml.Element("AreGemsMarked")?.ValueAsBool() ?? false;
			List<int> repairs = xml.Element("RepairedObjectIds")?.Elements().Select(x => x.ValueAsInt()).ToList() ?? new();

			return new BoatSaveData(isBoatMarked, isBoatFound, isBoatFinished, areGemsMarked, repairs);
		}
	}
}
