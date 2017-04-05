using UnityEngine;

public class ImageScaleAnimator : MonoBehaviour
{
	[SerializeField]
	private RectTransform scaleTarget;

	[SerializeField]
	[Tooltip("Scale negative for downscaling")]
	private AnimationCurve scaleCurve;

	[SerializeField]
	private float scaleSpeed;

	private float time;

	private Vector2 startSize;

	private void Start()
	{
		float width = scaleTarget.rect.width;
		float height = scaleTarget.rect.height;
		startSize = new Vector2(width, height);
	}

	private void Update()
	{
		time += Time.deltaTime;
		float num = scaleCurve.Evaluate(time * scaleSpeed);
		scaleTarget.sizeDelta = startSize + new Vector2(startSize.x * num, startSize.y * num);
	}
}
