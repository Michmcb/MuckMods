namespace MuckFoods;

using System.IO;
using System.Reflection;
using UnityEngine;

public static class Resources
{
	public static Sprite AppleBakedSprite = null!;
	public static Sprite AppleMeatStewSprite = null!;
	public static Sprite FlaxseedBreadSprite = null!;
	public static Sprite LessWeirdSoupSprite = null!;
	public static Sprite PieAppleMeatSprite = null!;
	public static Sprite PieAppleSprite = null!;
	public static Sprite PieMeatSprite = null!;
	public static Sprite PiePurpleAppleSprite = null!;
	public static Sprite PiePurpleMeatSprite = null!;
	public static Sprite PiePurpleSprite = null!;
	public static Sprite PieRedAppleSprite = null!;
	public static Sprite PieRedMeatSprite = null!;
	public static Sprite PieRedSprite = null!;
	public static Sprite PieSusAppleSprite = null!;
	public static Sprite PieSusMeatSprite = null!;
	public static Sprite PieSusSprite = null!;
	public static Sprite PieYellowAppleSprite = null!;
	public static Sprite PieYellowMeatSprite = null!;
	public static Sprite PieYellowSprite = null!;
	public static Sprite SoupPurpleAppleMeatSprite = null!;
	public static Sprite SoupPurpleAppleSprite = null!;
	public static Sprite SoupPurpleMeatSprite = null!;
	public static Sprite SoupRedAppleMeatSprite = null!;
	public static Sprite SoupRedAppleSprite = null!;
	public static Sprite SoupRedMeatSprite = null!;
	public static Sprite SoupSusAppleMeatSprite = null!;
	public static Sprite SoupSusAppleSprite = null!;
	public static Sprite SoupSusMeatSprite = null!;
	public static Sprite SoupYellowAppleMeatSprite = null!;
	public static Sprite SoupYellowAppleSprite = null!;
	public static Sprite SoupYellowMeatSprite = null!;
	public static Sprite ShroomRedToastySprite = null!;
	public static Sprite ShroomYellowToastySprite = null!;
	public static Sprite ShroomPurpleToastySprite = null!;
	public static Sprite ShroomSusToastySprite = null!;
	public static void Load()
	{
		using Stream resources = Assembly.GetExecutingAssembly().GetManifestResourceStream("MuckFoods.resources");
		AssetBundle assets = AssetBundle.LoadFromStream(resources);
		AppleBakedSprite = assets.LoadAsset<Sprite>("AppleBaked.png");
		ShroomRedToastySprite = assets.LoadAsset<Sprite>("ShroomRedToasty.png");
		ShroomYellowToastySprite = assets.LoadAsset<Sprite>("ShroomYellowToasty.png");
		ShroomPurpleToastySprite = assets.LoadAsset<Sprite>("ShroomPurpleToasty.png");
		ShroomSusToastySprite = assets.LoadAsset<Sprite>("ShroomSusToasty.png");

		AppleMeatStewSprite = assets.LoadAsset<Sprite>("AppleMeatStew.png");
		FlaxseedBreadSprite = assets.LoadAsset<Sprite>("FlaxseedBread.png");
		LessWeirdSoupSprite = assets.LoadAsset<Sprite>("LessWeirdSoup.png");

		PieAppleMeatSprite = assets.LoadAsset<Sprite>("PieAppleMeat.png");
		PieAppleSprite = assets.LoadAsset<Sprite>("PieApple.png");
		PieMeatSprite = assets.LoadAsset<Sprite>("PieMeat.png");

		PiePurpleAppleSprite = assets.LoadAsset<Sprite>("PiePurpleApple.png");
		PiePurpleMeatSprite = assets.LoadAsset<Sprite>("PiePurpleMeat.png");
		PiePurpleSprite = assets.LoadAsset<Sprite>("PiePurple.png");

		PieRedAppleSprite = assets.LoadAsset<Sprite>("PieRedApple.png");
		PieRedMeatSprite = assets.LoadAsset<Sprite>("PieRedMeat.png");
		PieRedSprite = assets.LoadAsset<Sprite>("PieRed.png");

		PieSusAppleSprite = assets.LoadAsset<Sprite>("PieSusApple.png");
		PieSusMeatSprite = assets.LoadAsset<Sprite>("PieSusMeat.png");
		PieSusSprite = assets.LoadAsset<Sprite>("PieSus.png");

		PieYellowAppleSprite = assets.LoadAsset<Sprite>("PieYellowApple.png");
		PieYellowMeatSprite = assets.LoadAsset<Sprite>("PieYellowMeat.png");
		PieYellowSprite = assets.LoadAsset<Sprite>("PieYellow.png");

		SoupPurpleAppleMeatSprite = assets.LoadAsset<Sprite>("SoupPurpleAppleMeat.png");
		SoupPurpleAppleSprite = assets.LoadAsset<Sprite>("SoupPurpleApple.png");
		SoupPurpleMeatSprite = assets.LoadAsset<Sprite>("SoupPurpleMeat.png");

		SoupRedAppleMeatSprite = assets.LoadAsset<Sprite>("SoupRedAppleMeat.png");
		SoupRedAppleSprite = assets.LoadAsset<Sprite>("SoupRedApple.png");
		SoupRedMeatSprite = assets.LoadAsset<Sprite>("SoupRedMeat.png");

		SoupSusAppleMeatSprite = assets.LoadAsset<Sprite>("SoupSusAppleMeat.png");
		SoupSusAppleSprite = assets.LoadAsset<Sprite>("SoupSusApple.png");
		SoupSusMeatSprite = assets.LoadAsset<Sprite>("SoupSusMeat.png");

		SoupYellowAppleMeatSprite = assets.LoadAsset<Sprite>("SoupYellowAppleMeat.png");
		SoupYellowAppleSprite = assets.LoadAsset<Sprite>("SoupYellowApple.png");
		SoupYellowMeatSprite = assets.LoadAsset<Sprite>("SoupYellowMeat.png");

		Plugin.Log.LogInfo("Assets loaded");
	}
}
