namespace MuckSaveGame
{
	using System.Xml.Linq;

	/// <summary>
	/// All of your savegame data, ready to be written to XML.
	/// </summary>
	public interface ISaveData
	{
		/// <summary>
		/// Called when your data should be written to XML.
		/// Try not to muck with any parents of <paramref name="xml"/>, as you may cause errors in the save file.
		/// </summary>
		/// <param name="xml">The root <see cref="XElement"/> to which your data should be written.</param>
		void SaveXml(XElement xml);
	}
}
