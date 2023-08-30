namespace MuckSaveGame
{
	using TMPro;
	using UnityEngine;
	using UnityEngine.EventSystems;

	public class SaveButton : MonoBehaviour, IPointerClickHandler
	{
		void Start()
		{
			UIManager.saveButtons.Add(GetComponentInChildren<TextMeshProUGUI>().text, this);
		}
		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				LoadManager.selectedSavePath = SaveSystem.GetPathForFileName(GetComponentInChildren<TextMeshProUGUI>().text);
				UIManager.selectionGUI?.SetActive(false);
			}
			else if (eventData.button == PointerEventData.InputButton.Right)
			{
				if (UIManager.buttonExtraGUI != null)
				{
					UIManager.buttonExtraGUI.transform.position = new Vector3(eventData.position.x + 200, eventData.position.y - 125);
					UIManager.buttonExtraGUI.SetActive(true);
				}

				UIManager.curEditSave = GetComponentInChildren<TextMeshProUGUI>().text;

				if (UIManager.buttonExtraGUI != null)
				{
					UIManager.buttonExtraGUI.GetComponentInChildren<TextMeshProUGUI>().text = UIManager.curEditSave.Length > 12
						? GetComponentInChildren<TextMeshProUGUI>().text.Substring(0, 12) + "..."
						: GetComponentInChildren<TextMeshProUGUI>().text;
				}
			}
		}
		public void UpdateText(string text)
		{
			GetComponentInChildren<TextMeshProUGUI>().text = text;
		}
	}
}
