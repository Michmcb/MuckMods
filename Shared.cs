namespace BepInEx
{
	using BepInEx.Configuration;

	public static class ConfigExtensions
	{
		public static int BindMoreThanZero(this ConfigFile config, string section, string key, int defaultValue, string description)
		{
			ConfigEntry<int> cfg = config.Bind(section, key, defaultValue, description);
			if (cfg.Value <= 0)
			{
				cfg.Value = defaultValue;
			}
			return cfg.Value;
		}
		public static float BindMoreThanZero(this ConfigFile config, string section, string key, float defaultValue, string description)
		{
			ConfigEntry<float> cfg = config.Bind(section, key, defaultValue, description);
			if (cfg.Value <= 0f)
			{
				cfg.Value = defaultValue;
			}
			return cfg.Value;
		}
	}
}