using System;
using UnityEngine;
using UnityEngine.UI;

public class ShowingAdsPopup : MonoBehaviour
{
	[SerializeField]
	private Button button;

	private float startTime;

	private float timeoutTime;

	private Action skipAction;

	public void Initialize(float timeoutTime, Action OnSkipPressed)
	{
		skipAction = OnSkipPressed;
		startTime = Time.time;
		this.timeoutTime = timeoutTime;
	}

	private void Update()
	{
		if (Time.time - startTime >= timeoutTime)
		{
			button.gameObject.SetActive(value: true);
			enabled = false;
		}
	}

	public void OnSkip()
	{
		if (skipAction != null)
		{
			skipAction();
		}
	}
}
