using UnityEngine;
using UnityEngine.UI;

public class FirstTimePlayModeWelcome : MonoBehaviour
{
	[SerializeField]
	private Text welcomeText;

	private void Start()
	{
		string text = TM._("Welcome:");
		text = text + " " + MVGameControllerBase.Game.LocalPlayer.Username;
		welcomeText.text = text;
	}
}
