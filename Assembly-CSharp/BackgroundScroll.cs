using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class BackgroundScroll : MonoBehaviour
{
	[SerializeField]
	private RawImage image;

	[SerializeField]
	private float Speed = 1f;

	[SerializeField]
	private Vector2 Direction = new Vector2(1f, 0.2f);

	[SerializeField]
	private Vector2 Scale = new Vector2(1f, 0.4f);

	private void Awake()
	{
		Rect uvRect = image.uvRect;
		uvRect.size = Scale;
		image.uvRect = uvRect;
	}

	private void Update()
	{
		Rect uvRect = image.uvRect;
		uvRect.position += Direction * (Speed * Time.deltaTime);
		image.uvRect = uvRect;
	}
}
