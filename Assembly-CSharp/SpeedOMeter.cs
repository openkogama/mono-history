using UnityEngine;
using UnityEngine.UI;

public class SpeedOMeter : MonoBehaviour
{
	public float fadeTime = 2f;

	[SerializeField]
	private CanvasGroup speedGroup;

	[SerializeField]
	private Text speedText;

	private float curSpeed;

	private float currFade;

	private bool prevFading = true;

	private bool fading = true;

	private void Awake()
	{
		speedGroup.alpha = 0f;
	}

	private void Update()
	{
		if (MVGameControllerBase.WOCM == null || MVGameControllerBase.Game.PlayerController.CurrentWorldObject == null)
		{
			return;
		}
		MVRigidBody component = MVGameControllerBase.Game.PlayerController.CurrentWorldObject.GameObject.GetComponent<MVRigidBody>();
		if (component == null)
		{
			speedGroup.alpha = 0f;
			return;
		}
		float b = component.Velocity.magnitude * 3.6f;
		curSpeed = Mathf.Lerp(curSpeed, b, Time.deltaTime);
		if (curSpeed > 100f)
		{
			fading = false;
			if (prevFading != fading)
			{
				currFade = 0f;
				prevFading = fading;
			}
			Show();
		}
		else
		{
			fading = true;
			if (prevFading != fading)
			{
				currFade = 0f;
				prevFading = fading;
			}
			Fade();
		}
		speedText.text = (int)curSpeed + " km/h";
	}

	private void Fade()
	{
		if (!(speedGroup.alpha <= 0f))
		{
			currFade += Time.deltaTime;
			if (currFade <= fadeTime)
			{
				speedGroup.alpha = Mathf.Lerp(1f, 0f, currFade);
			}
		}
	}

	private void Show()
	{
		if (!(speedGroup.alpha >= 1f))
		{
			currFade += Time.deltaTime;
			if (currFade <= fadeTime)
			{
				speedGroup.alpha = Mathf.Lerp(0f, 1f, currFade);
			}
		}
	}
}
