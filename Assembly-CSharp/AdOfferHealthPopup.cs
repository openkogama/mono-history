using UnityEngine;
using UnityEngine.EventSystems;

public class AdOfferHealthPopup : MonoBehaviour
{
	public static bool hasRewarded;

	public void OnClick()
	{
		AdRequestHandler.ShowHealthVideoAd(OnAdShown);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
	}

	private void OnAdShown(bool shouldReward)
	{
		if (shouldReward && !hasRewarded)
		{
			hasRewarded = true;
			MVGameControllerBase.WOCM.AvatarLocal.Avatar.GetComponent<MVInteractableBase>().AddModifier(AvatarModifierPackageType.Shielded);
		}
	}
}
