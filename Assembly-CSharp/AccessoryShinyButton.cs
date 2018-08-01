using UnityEngine;
using UnityEngine.UI;

public class AccessoryShinyButton : MonoBehaviour
{
	[SerializeField]
	private RectTransform buttonRect;

	[SerializeField]
	private Transform buttonShine;

	[SerializeField]
	private AnimationCurve buttonShinePositionCurve;

	[SerializeField]
	private Image topFlare;

	[SerializeField]
	private AnimationCurve topFlarePositionCurve;

	[SerializeField]
	private Image bottomFlare;

	[SerializeField]
	private AnimationCurve bottomFlarePositionCurve;

	[SerializeField]
	private AnimationCurve flareAlphaCurve;

	[SerializeField]
	private float animationDuration;

	private float currentProgress;

	private RectTransform shine;

	private void Start()
	{
		shine = buttonShine.GetComponent<RectTransform>();
		currentProgress = 0f;
	}

	private void Update()
	{
		currentProgress += Time.deltaTime;
		if (currentProgress > animationDuration)
		{
			currentProgress = 0f;
		}
		Vector3 localPosition = shine.localPosition;
		localPosition.x = buttonRect.rect.width * buttonShinePositionCurve.Evaluate(currentProgress) - shine.rect.width;
		shine.localPosition = localPosition;
		localPosition = topFlare.transform.localPosition;
		localPosition.x = buttonRect.rect.width * topFlarePositionCurve.Evaluate(currentProgress);
		topFlare.transform.localPosition = localPosition;
		Color color = topFlare.color;
		color.a = flareAlphaCurve.Evaluate(currentProgress);
		topFlare.color = color;
		localPosition = bottomFlare.transform.localPosition;
		localPosition.x = buttonRect.rect.width * bottomFlarePositionCurve.Evaluate(currentProgress);
		bottomFlare.transform.localPosition = localPosition;
		color = bottomFlare.color;
		color.a = flareAlphaCurve.Evaluate(currentProgress);
		bottomFlare.color = color;
	}
}
