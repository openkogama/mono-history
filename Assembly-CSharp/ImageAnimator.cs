using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageAnimator : MonoBehaviour
{
	[SerializeField]
	private Image image;

	[SerializeField]
	private Color startColor;

	[SerializeField]
	private Color endColor;

	[SerializeField]
	private AnimationCurve curve;

	[SerializeField]
	private float speed;

	private float time;

	public void SetColor(Color start, Color end)
	{
		startColor = start;
		endColor = end;
	}

	public float GetCurrentAlpha()
	{
		return curve.Evaluate(time * speed);
	}

	private void Update()
	{
		time += Time.deltaTime;
		image.color = Color.Lerp(startColor, endColor, curve.Evaluate(time * speed));
	}

	private void OnDisable()
	{
		time = 0f;
	}
}
