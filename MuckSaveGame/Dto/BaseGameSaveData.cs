namespace MuckSaveGame.Dto
{
	using System.Xml.Linq;
	public sealed class BaseGameSaveData : ISaveData
	{
		public BaseGameSaveData(WorldSaveData worldData, BoatSaveData boatData, PlayerSaveData playerData, ContainerSaveData containerData, EntitySaveData entityData)
		{
			WorldData = worldData;
			BoatData = boatData;
			PlayerData = playerData;
			ContainerData = containerData;
			EntityData = entityData;
		}
		public WorldSaveData WorldData { get; }
		public BoatSaveData BoatData { get; }
		public PlayerSaveData PlayerData { get; }
		public ContainerSaveData ContainerData { get; }
		public EntitySaveData EntityData { get; }
		public void SaveXml(XElement xml)
		{
			WorldData.SaveXml(xml.AddElement("WorldData"));
			BoatData.SaveXml(xml.AddElement("BoatData"));
			PlayerData.SaveXml(xml.AddElement("PlayerData"));
			ContainerData.SaveXml(xml.AddElement("ContainerData"));
			EntityData.SaveXml(xml.AddElement("EntityData"));
		}
		public static BaseGameSaveData Load(XElement xml)
		{
			WorldSaveData worldData = WorldSaveData.Load(xml.RequiredElement("WorldData"));
			BoatSaveData boatData = BoatSaveData.Load(xml.RequiredElement("BoatData"));
			PlayerSaveData playerData = PlayerSaveData.Load(xml.RequiredElement("PlayerData"));
			ContainerSaveData containerData = ContainerSaveData.Load(xml.RequiredElement("ContainerData"));
			EntitySaveData entityData = EntitySaveData.Load(xml.RequiredElement("EntityData"));

			return new BaseGameSaveData(worldData, boatData, playerData, containerData, entityData);
		}
	}
}
