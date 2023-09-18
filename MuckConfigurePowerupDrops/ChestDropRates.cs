namespace MuckConfigurePowerupDrops;

public readonly struct ChestDropRates
{
	public ChestDropRates(float white, float blue, float orange)
	{
		White = white;
		Blue = blue;
		Orange = orange;
	}
	public float White { get; }
	public float Blue { get; }
	public float Orange { get; }
}
