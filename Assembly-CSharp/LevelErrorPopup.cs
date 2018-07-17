using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelErrorPopup : MonoBehaviour
{
	[SerializeField]
	private RawImage requiredLevelImage;

	[SerializeField]
	private RawImage playerLevelImage;

	private UnityAction resultCallback;

	public void Initialize(UnityAction resultCallback, int requiredLevel)
	{
		BadgeManager.GetBadgeTexture(requiredLevel, OnLevelRequirementLoaded);
		BadgeManager.GetBadgeTexture(MVGameControllerBase.Game.LocalPlayer.Level, OnPlayerLevelLoaded);
		this.resultCallback = resultCallback;
	}

	private void OnLevelRequirementLoaded(WWW www)
	{
		if (www == null || www.texture == null)
		{
			Debug.LogWarning("Badge not loaded for accessory level requirement");
		}
		else
		{
			requiredLevelImage.texture = www.texture;
		}
	}

	private void OnPlayerLevelLoaded(WWW www)
	{
		if (www == null || www.texture == null)
		{
			Debug.LogWarning("Badge not loaded for accessory level requirement");
		}
		else
		{
			playerLevelImage.texture = www.texture;
		}
	}

	public void OnButtonPressed()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		resultCallback();
	}
}
