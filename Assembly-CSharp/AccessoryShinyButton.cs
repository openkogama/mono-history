using UnityEngine;
using UnityEngine.UI;

public class AccessoryShinyButton : MonoBehaviour
{
	[SerializeField]
	private RawImage image;

	[SerializeField]
	private AnimationCurve scrollEffect;

	[SerializeField]
	private Vector2 direction;

	private void Update()
	{
		Rect uvRect = image.uvRect;
		Color color = image.color;
		color.a = 0.8f - scrollEffect.Evaluate(Time.time);
		image.color = color;
		uvRect.position += direction * (scrollEffect.Evaluate(Time.time) * Time.deltaTime);
		image.uvRect = uvRect;
	}
}
