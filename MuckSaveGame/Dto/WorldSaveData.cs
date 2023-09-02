namespace MuckSaveGame.Dto
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Xml.Linq;

	public sealed class WorldSaveData
	{
		public WorldSaveData(int currentDay, int seed, float time, float totalTime, float mobUpdateIntervalMin, float mobUpdateIntervalMax, int lastBossNightThatSpawnedBoss, List<int> bossesInRotation)
		{
			CurrentDay = currentDay;
			Seed = seed;
			Time = time;
			TotalTime = totalTime;
			MobUpdateIntervalMin = mobUpdateIntervalMin;
			MobUpdateIntervalMax = mobUpdateIntervalMax;
			LastBossNightThatSpawnedBoss = lastBossNightThatSpawnedBoss;
			BossesInRotation = bossesInRotation;
		}
		public int CurrentDay { get; }
		public int Seed { get; }
		public float Time { get; }
		public float TotalTime { get; }
		public float MobUpdateIntervalMin { get; }
		public float MobUpdateIntervalMax { get; }
		public int LastBossNightThatSpawnedBoss { get; }
		public List<int> BossesInRotation { get; }
		public void SaveXml(XElement xml)
		{
			xml.AddElementValue(nameof(CurrentDay), CurrentDay);
			xml.AddElementValue(nameof(Seed), Seed);
			xml.AddElementValue(nameof(Time), Time);
			xml.AddElementValue(nameof(TotalTime), TotalTime);
			xml.AddElementValue(nameof(MobUpdateIntervalMin), MobUpdateIntervalMin);
			xml.AddElementValue(nameof(MobUpdateIntervalMax), MobUpdateIntervalMax);
			xml.AddElementValue(nameof(LastBossNightThatSpawnedBoss), LastBossNightThatSpawnedBoss);
			xml.AddElementValues("BossesInRotation", "_", BossesInRotation, (x, v) => x.Value = v.ToString());
		}
		public static WorldSaveData Load(XElement xml)
		{
			int currentDay = xml.RequiredElement("CurrentDay").ValueAsInt();
			int seed = xml.RequiredElement("Seed").ValueAsInt();
			float time = xml.RequiredElement("Time").ValueAsFloat();
			float totalTime = xml.RequiredElement("TotalTime").ValueAsFloat();
			float mobUpdateIntervalMin = xml.RequiredElement("MobUpdateIntervalMin").ValueAsFloat();
			float mobUpdateIntervalMax = xml.RequiredElement("MobUpdateIntervalMax").ValueAsFloat();
			int lastBossNightThatSpawnedBoss = xml.RequiredElement("LastBossNightThatSpawnedBoss").ValueAsInt();
			List<int> bossesFoughtInRotation = xml.Element("BossesInRotation")?.Elements().Select(x => x.ValueAsInt()).ToList() ?? new();
			return new WorldSaveData(currentDay, seed, time, totalTime, mobUpdateIntervalMin, mobUpdateIntervalMax, lastBossNightThatSpawnedBoss, bossesFoughtInRotation);
		}
	}
}
