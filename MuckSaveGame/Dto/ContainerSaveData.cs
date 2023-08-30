namespace MuckSaveGame.Dto
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Xml.Linq;
	using System.Xml.Resolvers;

	public sealed class ContainerSaveData : ISaveData
	{
		public ContainerSaveData(List<SavedContainer> chests, List<SavedContainer> furnaces, List<SavedContainer> cauldrons)
		{
			Chests = chests;
			Furnaces = furnaces;
			Cauldrons = cauldrons;
		}
		public List<SavedContainer> Chests { get; }
		public List<SavedContainer> Furnaces { get; }
		public List<SavedContainer> Cauldrons { get; }
		public void SaveXml(XElement xml)
		{
			xml.AddElementValues("Chests", "_", Chests, (x, v) => v.SaveXml(x));
			xml.AddElementValues("Furnaces", "_", Furnaces, (x, v) => v.SaveXml(x));
			xml.AddElementValues("Cauldrons", "_", Cauldrons, (x, v) => v.SaveXml(x));
		}
		public static ContainerSaveData Load(XElement xml)
		{
			List<SavedContainer> chests = xml.Element("Chests")?.Elements().Select(SavedContainer.Load).ToList() ?? new();
			List<SavedContainer> furnaces = xml.Element("Furnaces")?.Elements().Select(SavedContainer.Load).ToList() ?? new();
			List<SavedContainer> cauldrons = xml.Element("Cauldrons")?.Elements().Select(SavedContainer.Load).ToList() ?? new();

			return new ContainerSaveData(chests, furnaces, cauldrons);
		}
	}
}
