using UnityEngine;

public class FirstTimeActivatableMessageStayForSeconds : FirstTimeActivatableMessage
{
	private float showedTime;

	private bool isShown;

	private bool isDone;

	[SerializeField]
	private string messageText;

	[SerializeField]
	private float stayTimeInSeconds = 2f;

	public override void OnShow()
	{
		base.OnShow();
		firstTimeEventMessage.SetText(TM._(messageText));
		isShown = true;
	}

	private void Update()
	{
		if (!isDone)
		{
			if (showedTime > stayTimeInSeconds)
			{
				FadeOut();
				isDone = true;
			}
			if (isShown && !IsBlocked)
			{
				showedTime += Time.deltaTime;
			}
		}
	}

	protected override void OnDisable()
	{
		if (!isDone && isShown)
		{
			DoDisabled();
		}
	}
}
