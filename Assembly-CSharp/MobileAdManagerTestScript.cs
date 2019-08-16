using System.Collections.Generic;
using Assets.Scripts.AdIntegration;
using Assets.Scripts.AdIntegration.Mobile;
using UnityEngine;

public class MobileAdManagerTestScript : MonoBehaviour
{
	private MobileAdManager mobileAdManager;

	private const int maxOutMessage = 3;

	private Queue<string> outputMessages = new Queue<string>();

	private void Start()
	{
		MobileAdManagerCredentials.Setup(Application.identifier);
		mobileAdManager = new MobileAdManager(testing: true);
		Application.logMessageReceived += ApplicationOnLogMessageReceived;
	}

	private void ApplicationOnLogMessageReceived(string condition, string stacktrace, LogType type)
	{
		outputMessages.Enqueue(condition);
		while (outputMessages.Count > 3)
		{
			outputMessages.Dequeue();
		}
	}

	public void OnGUI()
	{
		GUIStyle gUIStyle = new GUIStyle();
		GUI.skin.button.fontSize = (int)(0.035f * (float)Screen.width);
		float width = 0.35f * (float)Screen.width;
		float height = 0.15f * (float)Screen.height;
		float x = 0.1f * (float)Screen.width;
		float x2 = 0.55f * (float)Screen.width;
		Rect position = new Rect(x, 0.05f * (float)Screen.height, width, height);
		if (GUI.Button(position, "Initialize\nMobileAdManager"))
		{
			mobileAdManager.Initialize();
		}
		Rect position2 = new Rect(x, 0.225f * (float)Screen.height, width, height);
		if (GUI.Button(position2, "Destroy\nMobileAdManager"))
		{
			mobileAdManager.Destroy();
		}
		Rect position3 = new Rect(x2, 0.05f * (float)Screen.height, width, height);
		if (GUI.Button(position3, "Request\nRewardedAd"))
		{
			mobileAdManager.RequestRewardedAd(RewardedAdCallback, string.Empty);
		}
		Rect position4 = new Rect(x2, 0.225f * (float)Screen.height, width, height);
		if (GUI.Button(position4, "Request\nInterstitial"))
		{
			mobileAdManager.RequestInterstitial(InterstitialCallback, string.Empty);
		}
		Rect position5 = new Rect(x2, 0.5f * (float)Screen.height, width, 0.5f * (float)Screen.height);
		GUIStyle gUIStyle2 = new GUIStyle();
		gUIStyle2.wordWrap = true;
		gUIStyle2.fontStyle = FontStyle.Bold;
		gUIStyle2.fontSize = 20;
		string text = string.Empty;
		foreach (string outputMessage in outputMessages)
		{
			text = text + outputMessage + "\n";
		}
		GUI.Label(position5, text, gUIStyle2);
		Rect position6 = new Rect(x, 0.5f * (float)Screen.height, width, 0.5f * (float)Screen.height);
		GUIStyle gUIStyle3 = new GUIStyle();
		gUIStyle3.wordWrap = true;
		gUIStyle3.fontStyle = FontStyle.Bold;
		gUIStyle3.fontSize = 20;
		GUI.Label(position6, mobileAdManager.ToString(), gUIStyle3);
	}

	private void InterstitialCallback(InterstitialAdResult obj)
	{
		Debug.Log("InterstitialAdResult: " + obj);
	}

	private void RewardedAdCallback(RewardedAdResult obj)
	{
		Debug.Log("RewardedAdResult: " + obj);
	}
}
