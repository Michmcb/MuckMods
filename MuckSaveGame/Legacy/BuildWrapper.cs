namespace MuckSaveGame.Legacy
{
	using System;
	using UnityEngine;

	[Serializable]
	public class BuildWrapper
	{
		public int itemId;
		public float[] position;
		public int rotation;

		public BuildWrapper(int _itemId, Vector3 _position, int _rotation)
		{
			itemId = _itemId;

			position = new float[3];

			position[0] = _position.x;
			position[1] = _position.y;
			position[2] = _position.z;

			rotation = _rotation;
		}
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		public BuildWrapper()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		{

		}
	}
}
