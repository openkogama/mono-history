using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionMessage : Notification
{
	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private AnimationCurve fadeCurve;

	[SerializeField]
	private Text text;

	private float currentTime;

	private NotificationLifetime lifeTime;

	protected override NotificationLifetime Lifetime => lifeTime;

	public override void Initialize(Dictionary<object, object> data)
	{
		base.Initialize(data);
		text.text = (string)data[(byte)1];
		currentTime = 0f;
		lifeTime = (NotificationLifetime)(int)data[(byte)2];
		Debug.Log("Initialize");
	}

	private void OnEnable()
	{
		canvasGroup.alpha = fadeCurve.keys[0].value;
	}

	protected override void Update()
	{
		base.Update();
		currentTime += Time.deltaTime;
		canvasGroup.alpha = fadeCurve.Evaluate(currentTime / (float)Lifetime);
	}
}
