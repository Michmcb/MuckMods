namespace MuckSaveGame.Dto
{
	using System.Xml.Linq;

	public sealed class SavedMob
	{
		public SavedMob(int mobType, int bossType, int guardianType, float multiplier, float bossMultiplier, Position position)
		{
			MobType = mobType;
			BossType = bossType;
			GuardianType = guardianType;
			Multiplier = multiplier;
			BossMultiplier = bossMultiplier;
			Position = position;
		}
		public int MobType { get; }
		public int BossType { get; }
		public int GuardianType { get; }
		public float Multiplier { get; }
		public float BossMultiplier { get; }
		public Position Position { get; }
		public void SaveXml(XElement xml)
		{
			xml.AddElementValue(nameof(MobType), MobType);
			xml.AddElementValue(nameof(BossType), BossType);
			xml.AddElementValue(nameof(GuardianType), GuardianType);
			xml.AddElementValue(nameof(Multiplier), Multiplier);
			xml.AddElementValue(nameof(BossMultiplier), BossMultiplier);
			Position.SaveXml(xml.AddElement(nameof(Position)));
		}
		public static SavedMob Load(XElement xml)
		{
			int mobType = xml.RequiredElement("MobType").ValueAsInt();
			int bossType = xml.RequiredElement("BossType").ValueAsInt();
			int guardianType = xml.RequiredElement("GuardianType").ValueAsInt();
			float multiplier = xml.RequiredElement("Multiplier").ValueAsFloat();
			float bossMultiplier = xml.RequiredElement("BossMultiplier").ValueAsFloat();
			Position position = Position.Load(xml.RequiredElement("Position"));
			return new SavedMob(mobType, bossType, guardianType, multiplier, bossMultiplier, position);
		}
	}
}
