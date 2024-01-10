namespace MuckArrows;

using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

[BepInPlugin("MuckArrows.MichMcb", "Muck Arrows", "1.2.0")]
public class Plugin : BaseUnityPlugin
{
	public static ManualLogSource Log = null!;
	public static bool CanCraftStoneArrows = true;
	public static bool CanCraftObamiumArrows = true;
	public static bool CanCraftRubyArrows = true;
	public static bool CanCraftChunkiumArrows = true;
	public static int FlintArrowDamage = 20;
	public static int SteelArrowDamage = 30;
	public static int MithrilArrowDamage = 40;
	public static int AdamantiteArrowDamage = 50;
	public static int FireArrowDamage = 100;
	public static int LightningArrowDamage = 100;
	public static int WaterArrowDamage = 100;
	public static int StoneArrowDamage = 5;
	public static int ChunkiumArrowDamage = 55;
	public static int ObamiumArrowDamage = 60;
	public static int RubyArrowDamage = 75;

	private void Awake()
	{
		// Plugin startup logic
		Log = Logger;

		Config.SaveOnConfigSet = false;
		CanCraftStoneArrows = Config.Bind("Main", nameof(CanCraftStoneArrows), CanCraftStoneArrows, "If true, allows crafting (practically useless) stone arrows. Stone arrows can still exist in the game even if this setting is false, so toggling it will not cause any issues with saved games.").Value;
		CanCraftObamiumArrows = Config.Bind("Main", nameof(CanCraftObamiumArrows), CanCraftStoneArrows, "If true, allows crafting Obamium Arrows. As above, they always exist.").Value;
		CanCraftRubyArrows = Config.Bind("Main", nameof(CanCraftRubyArrows), CanCraftStoneArrows, "If true, allows crafting Ruby Arrows. As above, they always exist.").Value;
		CanCraftChunkiumArrows = Config.Bind("Main", nameof(CanCraftChunkiumArrows), CanCraftStoneArrows, "If true, allows crafting Chunkium Arrows. As above, they always exist.").Value;

		FlintArrowDamage = Config.BindMoreThanZero("Damage", nameof(FlintArrowDamage), FlintArrowDamage, "The damage that Flint arrows deal. Vanilla: 20").Value;
		SteelArrowDamage = Config.BindMoreThanZero("Damage", nameof(SteelArrowDamage), SteelArrowDamage, "The damage that Steel arrows deal. Vanilla: 30").Value;
		MithrilArrowDamage = Config.BindMoreThanZero("Damage", nameof(MithrilArrowDamage), MithrilArrowDamage, "The damage that Mithril arrows deal. Vanilla: 40").Value;
		AdamantiteArrowDamage = Config.BindMoreThanZero("Damage", nameof(AdamantiteArrowDamage), AdamantiteArrowDamage, "The damage that Adamantite arrows deal. Vanilla: 50").Value;
		FireArrowDamage = Config.BindMoreThanZero("Damage", nameof(FireArrowDamage), FireArrowDamage, "The damage that Fire arrows deal. Vanilla: 100").Value;
		LightningArrowDamage = Config.BindMoreThanZero("Damage", nameof(LightningArrowDamage), LightningArrowDamage, "The damage that Lightning arrows deal. Vanilla: 100").Value;
		WaterArrowDamage = Config.BindMoreThanZero("Damage", nameof(WaterArrowDamage), WaterArrowDamage, "The damage that Water arrows deal. Vanilla: 100").Value;

		StoneArrowDamage = Config.BindMoreThanZero("Damage", nameof(StoneArrowDamage), StoneArrowDamage, "The damage that Stone arrows deal. Vanilla: 5").Value;
		ChunkiumArrowDamage = Config.BindMoreThanZero("Damage", nameof(ChunkiumArrowDamage), ChunkiumArrowDamage, "The damage that Chunkium arrows deal. Vanilla: 55").Value;
		ObamiumArrowDamage = Config.BindMoreThanZero("Damage", nameof(ObamiumArrowDamage), ObamiumArrowDamage, "The damage that Obamium arrows deal. Vanilla: 60").Value;
		RubyArrowDamage = Config.BindMoreThanZero("Damage", nameof(RubyArrowDamage), RubyArrowDamage, "The damage that Ruby arrows deal. Vanilla: 75").Value;

		Config.Save();

		Resources.Load();

		Logger.LogInfo("MuckArrows loaded!");
		Harmony.CreateAndPatchAll(typeof(CreateRecipesPatch), null);
	}
}
