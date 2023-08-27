namespace MuckDontDestroyNeighbours;

using BepInEx;
using HarmonyLib;

[BepInPlugin("MuckDontDestroyNeighbours.MichMcb", "Muck Don't Destroy Neighbours", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
	private void Awake()
	{
		// Plugin startup logic
		Logger.LogInfo("MuckDontDestroyNeighbours loaded!");
		Harmony.CreateAndPatchAll(typeof(Patch), null);
	}
}
