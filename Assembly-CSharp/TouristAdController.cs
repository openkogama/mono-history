using UnityEngine;

public class TouristAdController : MonoBehaviour
{
	[SerializeField]
	private float timeBeforeAdShown = 180f;

	private float timer;

	private bool hasBeenKilled;

	private TouristModeController promotionSliderCreator;

	public void Initialize(TouristModeController promotionSliderController)
	{
		promotionSliderCreator = promotionSliderController;
		if (!MVGameControllerBase.IsTouristSession)
		{
			promotionSliderCreator.SetActive(active: true);
			Object.Destroy(gameObject);
		}
		else
		{
			gameObject.SetActive(value: true);
		}
	}

	private void Update()
	{
		if (MVGameControllerBase.JoinState != MVJoinState.Playing)
		{
			return;
		}
		timer += Time.deltaTime;
		if (MVGameControllerBase.WOCM.AvatarLocal.IsDead)
		{
			hasBeenKilled = true;
		}
		else if (hasBeenKilled)
		{
			if (timer >= timeBeforeAdShown)
			{
				promotionSliderCreator.ShowAnyPromotionSlide();
				timer = 0f;
			}
			else
			{
				promotionSliderCreator.ShowAnyPromotionSlide();
			}
			hasBeenKilled = false;
		}
	}
}
