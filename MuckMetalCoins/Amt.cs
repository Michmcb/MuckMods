namespace MuckMetalCoins;

public readonly struct Amt
{
	public Amt(int required, int coins)
	{
		Required = required;
		Coins = coins;
	}
	public int Required { get; }
	public int Coins { get; }
}
