namespace MuckSaveGame
{
	using System.Xml.Linq;

	/// <summary>
	/// Manages custom savegame data.
	/// </summary>
	public interface ISaveDataManager
	{
		/// <summary>
		/// A unique name. This should probably be your plugin name.
		/// </summary>
		string Name { get; }
		/// <summary>
		/// Called when the user has clicked save. Collect and prepare all your savegame data here,
		/// and return the object which contains the data as an <see cref="ISaveData"/>.
		/// After this method is called, <see cref="ISaveData.SaveXml(System.Xml.XmlWriter)"/> will be called.
		/// </summary>
		/// <returns></returns>
		ISaveData GetSaveData();
		/// <summary>
		/// Called when loading savegame data. The parameter <paramref name="xml"/> is the root element of your data.
		/// DO NOT yet apply this data to the game! That work must be done in <see cref="ApplyLoadedData"/>.
		/// </summary>
		/// <param name="xml">The element from which you can load data.</param>
		void LoadXml(XElement xml);
		/// <summary>
		/// Called when your loaded data must be applied to the game.
		/// </summary>
		void ApplyLoadedData();
		/// <summary>
		/// Called when the user has left the game. Put any cleanup in here.
		/// </summary>
		void Unload();
	}
}
