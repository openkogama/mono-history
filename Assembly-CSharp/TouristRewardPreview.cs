using UnityEngine;

public class TouristRewardPreview : MonoBehaviour
{
	public void OnPreview()
	{
		NotificationController.PushNotification(TM._("Sign in to claim your prize!"));
	}
}
