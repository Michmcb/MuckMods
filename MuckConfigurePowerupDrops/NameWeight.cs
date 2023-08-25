namespace MuckConfigurePowerupDrops;

public readonly struct NameWeight
{
	public NameWeight(string name, int weight)
	{
		Name = name;
		Weight = weight;
	}
	public string Name { get; }
	public int Weight { get; }
}
