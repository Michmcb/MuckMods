namespace MuckSaveGame.Dto
{
	using System.Xml.Linq;
	using UnityEngine;

	public readonly struct Position
	{
		public Position(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}
		public float X { get; }
		public float Y { get; }
		public float Z { get; }
		public static Position FromVec3(Vector3 vec)
		{
			return new(vec.x, vec.y, vec.z);
		}
		public static Position FromFloats(float[] p)
		{
			return new Position(p[0], p[1], p[2]);
		}
		public Vector3 ToVec3()
		{
			return new Vector3(X, Y, Z);
		}
		public override string ToString()
		{
			return string.Concat(X, ",", Y, ",", Z);
		}
		public void SaveXml(XElement xml)
		{
			xml.AddElementValue("X", X);
			xml.AddElementValue("Y", Y);
			xml.AddElementValue("Z", Z);
		}
		public static Position Load(XElement xml)
		{
			float x = xml.RequiredElement("X").ValueAsFloat();
			float y = xml.RequiredElement("Y").ValueAsFloat();
			float z = xml.RequiredElement("Z").ValueAsFloat();
			return new Position(x, y, z);
		}
	}
}
