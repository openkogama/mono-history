using UnityEngine;
using UnityEngine.UI;

public class ChatBubblePositionHandler : MonoBehaviour
{
	[SerializeField]
	private RectTransform textBubbleTransform;

	[SerializeField]
	private Text text;

	private Transform transformToFollow;

	public void Initialize(string text, Transform transformToFollow)
	{
		this.transformToFollow = transformToFollow;
		this.text.text = text;
	}

	private void Update()
	{
		Vector3 position = transformToFollow.position;
		Vector2 vector = Camera.main.WorldToViewportPoint(position);
		textBubbleTransform.anchorMin = vector;
		textBubbleTransform.anchorMax = vector;
	}
}
