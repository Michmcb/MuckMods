namespace MuckSaveGame.Legacy
{
	using System;

	[Serializable]
	public class MobWrapper
	{
		public int mobType;
		public int bossType;
		public int guardianType;

		public float multiplier;
		public float bossMultiplier;

		public float[] position;

		public MobWrapper(Mob originalMob)
		{
			//save guarding color
			guardianType = originalMob.gameObject.GetComponent<Guardian>() ? (int)originalMob.gameObject.GetComponent<Guardian>().type : -1;

			mobType = originalMob.mobType.id;
			bossType = (int)originalMob.bossType;

			multiplier = originalMob.multiplier;
			bossMultiplier = originalMob.bossMultiplier;

			position = new float[3];
			position[0] = originalMob.transform.position.x;
			position[1] = originalMob.transform.position.y;
			position[2] = originalMob.transform.position.z;
		}

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		public MobWrapper()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		{

		}
	}
}
