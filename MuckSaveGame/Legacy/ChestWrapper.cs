namespace MuckSaveGame.Legacy
{
	using System;
	using System.Collections.Generic;

	[Serializable]
	public class ChestWrapper
	{
		//store cellid so that null cells dont need to be saved
		public List<SerializableTuple<int, int, int>> cells = new();
		public int fullCells;
		public int chestSize;
		public float[] position;
		public int rotation;

		public ChestWrapper(Chest originalChest)
		{
			chestSize = originalChest.chestSize;

			for (int i = 0; i < chestSize; i++)
			{
				if (originalChest.cells[i] != null)
				{
					cells.Add(new Tuple<int, int, int>(originalChest.cells[i].id, originalChest.cells[i].amount, i));
					fullCells++;
				}
			}

			position = new float[3];

			position[0] = originalChest.transform.root.position.x;
			position[1] = originalChest.transform.root.position.y;
			position[2] = originalChest.transform.root.position.z;

			rotation = (int)originalChest.transform.rotation.eulerAngles.y;
		}

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		public ChestWrapper()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		{

		}
	}
}
