namespace MuckSaveGame
{
	using System;
	using System.Collections;
	using System.Reflection;
	using UnityEngine;

	public class WorldTimer : MonoBehaviour
	{
		private IEnumerator? coroutine;
		public void StartSave(float _time)
		{
			coroutine = SaveCoroutine(_time);
			StartCoroutine(coroutine);
		}
		public void StartPlayerDeath(float _time)
		{
			Plugin.Log.LogInfo("Starting Player Death");
			coroutine = PlayerDeathCoroutine(_time);
			Plugin.Log.LogInfo("Player Death Started");
			StartCoroutine(coroutine);
		}
		private IEnumerator PlayerDeathCoroutine(float _time)
		{
			Plugin.Log.LogInfo("Player Death Started Coroutine");
			yield return new WaitForSecondsRealtime(_time);

			var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

			Plugin.Log.LogInfo("PLAYER DYING");

			typeof(PlayerStatus).GetMethod("PlayerDied", flags).Invoke(PlayerStatus.Instance, new object[] { 0, -1 });
			Plugin.Log.LogInfo("Player Died");
			UnityEngine.Object.Destroy(this);
		}
		private IEnumerator SaveCoroutine(float _time)
		{
			// I'm not quite sure why we are waiting like 5 seconds before saving, maybe it's to do with players in multiplayer?
			if (World.isLeavingIsland)
			{
				ClientSend.SendChatMessage("<color=#FF0000>Can't save, leaving island!");
				ChatBox.Instance.AppendMessage(-1, "<color=#FF0000>Can't save, leaving island!", "");
				UnityEngine.Object.Destroy(this);
				yield break;
			}
			if (GameManager.players.Count > 1)
			{
				yield return new WaitForSecondsRealtime(_time);
			}

			if (LoadManager.selectedSavePath != null)
			{
				try
				{
					SaveSystem.Save(LoadManager.selectedSavePath);
					ClientSend.SendChatMessage($"<color=#ADD8E6>Save Completed! Seed: {World.worldSeed}");
					ChatBox.Instance.AppendMessage(-1, $"<color=#ADD8E6>Save Completed! Seed: {World.worldSeed}", "");
				}
				catch (Exception ex)
				{
					Plugin.Log.LogError(ex.ToString());
					ClientSend.SendChatMessage("<color=#FF0000>Save Failed!");
					ChatBox.Instance.AppendMessage(-1, "<color=#FF0000>Save Failed", "");
				}
			}

			UnityEngine.Object.Destroy(this);
		}
	}
}
