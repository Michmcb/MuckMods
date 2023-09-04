namespace MuckFoods;
using UnityEngine;

public sealed class DescrSprite
{
	public DescrSprite(string descr, Sprite sprite)
	{
		Descr = descr;
		Sprite = sprite;
	}
	public string Descr { get; }
	public Sprite Sprite { get; }
}
