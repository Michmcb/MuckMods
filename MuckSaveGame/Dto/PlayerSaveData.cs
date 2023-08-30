namespace MuckSaveGame.Dto
{
	using System.Collections.Generic;
	using System.Xml.Linq;

	public sealed class PlayerSaveData
	{
		public PlayerSaveData(SavedPlayer localPlayer, Dictionary<string, SavedPlayer> clientPlayers)
		{
			LocalPlayer = localPlayer;
			ClientPlayers = clientPlayers;
		}
		public SavedPlayer LocalPlayer { get; }
		public Dictionary<string, SavedPlayer> ClientPlayers { get; }
		public void SaveXml(XElement xml)
		{
			LocalPlayer.SaveXml(xml.AddElement("Player"));
			foreach (KeyValuePair<string, SavedPlayer> kvp in ClientPlayers)
			{
				XElement p = xml.AddElement("Player");
				p.SetAttributeValue("steamId", kvp.Key);
				kvp.Value.SaveXml(p);
			}
		}
		public static PlayerSaveData Load(XElement xml)
		{
			SavedPlayer? localPlayer = null;
			Dictionary<string, SavedPlayer> clientPlayers = new();
			foreach (XElement x in xml.Elements())
			{
				string? steamId = x.Attribute("steamId")?.Value;
				SavedPlayer player = SavedPlayer.Load(x);
				if (steamId != null)
				{
					clientPlayers[steamId] = player;
				}
				else
				{
					localPlayer = player;
				}
			}
			return new PlayerSaveData(localPlayer ?? SavedPlayer.CreateEmpty(), clientPlayers);
		}
	}
}
