namespace MuckArrows;
using System.IO;
using System.Reflection;
using UnityEngine;

public static class Resources
{
	public static Texture2D ChunkiumArrowTexture = null!;
	public static Sprite ChunkiumArrowSprite = null!;
	public static Texture2D ObamiumArrowTexture = null!;
	public static Sprite ObamiumArrowSprite = null!;
	public static Texture2D RubyArrowTexture = null!;
	public static Sprite RubyArrowSprite = null!;
	public static void Load()
	{
		using Stream resources = Assembly.GetExecutingAssembly().GetManifestResourceStream("MuckArrows.resources");
		AssetBundle assets = AssetBundle.LoadFromStream(resources);
		ChunkiumArrowTexture = assets.LoadAsset<Texture2D>("ChunkiumArrow.png");
		ChunkiumArrowSprite = assets.LoadAsset<Sprite>("ChunkiumArrow.png");
		ObamiumArrowTexture = assets.LoadAsset<Texture2D>("ObamiumArrow.png");
		ObamiumArrowSprite = assets.LoadAsset<Sprite>("ObamiumArrow.png");
		RubyArrowTexture = assets.LoadAsset<Texture2D>("RubyArrow.png");
		RubyArrowSprite = assets.LoadAsset<Sprite>("RubyArrow.png");
	}
}
