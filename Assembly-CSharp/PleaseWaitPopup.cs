using UnityEngine;
using UnityEngine.UI;

public class PleaseWaitPopup : MonoBehaviour
{
	[SerializeField]
	private RectTransform rectTransform;

	[SerializeField]
	private float spinSpeed = 5f;

	[SerializeField]
	private CanvasGroup popUpcanvasGroup;

	[SerializeField]
	private AnimationCurve alphaCurveOverTime;

	[SerializeField]
	private float fullyRevealedTime;

	[SerializeField]
	private Image fullScreenBackGround;

	[SerializeField]
	private float fullScreenBackGroundMaxAlpha;

	private readonly Vector3 direction = new Vector3(0f, 0f, 1f);

	private float timeSinceStart;

	private void Update()
	{
		timeSinceStart += Time.deltaTime;
		if (timeSinceStart > fullyRevealedTime)
		{
			timeSinceStart = fullyRevealedTime;
		}
		float num = alphaCurveOverTime.Evaluate(timeSinceStart);
		if (num > fullScreenBackGroundMaxAlpha)
		{
			num = fullScreenBackGroundMaxAlpha;
		}
		fullScreenBackGround.color = new Color(fullScreenBackGround.color.r, fullScreenBackGround.color.g, fullScreenBackGround.color.b, num);
		popUpcanvasGroup.alpha = alphaCurveOverTime.Evaluate(timeSinceStart);
		rectTransform.Rotate(direction * spinSpeed * Time.deltaTime * 60f);
	}
}
