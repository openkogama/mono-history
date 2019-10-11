using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;

public class RegisteredPromotionController : MonoBehaviour
{
	[SerializeField]
	private RegisteredPromotionPopup registeredPromotionPopupPrefab;

	[SerializeField]
	private float timeBeforeShownPromotion = 180f;

	private float timer;

	private bool isDead;

	private const float showAdDelay = 1.26f;

	private void Update()
	{
		if (MVGameControllerBase.GameMode != MVGameMode.Play)
		{
			return;
		}
		timer += Time.deltaTime;
		if (MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleModeTypeWrapper.IsInMode(SpawnRoleModeType.Dead))
		{
			if (!isDead)
			{
				if (timer >= timeBeforeShownPromotion && Time.time > MVGameControllerBase.LocalPlayer.RespawnTime - (MVGameControllerBase.LocalPlayer.RespawnDuration - 1.26f))
				{
					isDead = true;
					timer = 0f;
				}
				else if (Time.time > MVGameControllerBase.LocalPlayer.RespawnTime - (MVGameControllerBase.LocalPlayer.RespawnDuration - 1.26f))
				{
					isDead = true;
				}
			}
		}
		else
		{
			isDead = false;
		}
	}

	private void ShowRegisteredPromotionPopup()
	{
		RegisteredPromotionPopup registeredPromotion = Object.Instantiate(registeredPromotionPopupPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(registeredPromotion.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.Popup);
		});
	}
}
