using UnityEngine;

public class TouristControllerAndroid : MonoBehaviour
{
	public void OnClick()
	{
		MVGameControllerBase.ApplicationQuit(new QuitBrowserRequest(MVGameControllerBase.GameSessionData.signupURL));
	}
}
