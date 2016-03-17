using UnityEngine;

public class PleaseWaitPopup : MonoBehaviour
{
	[SerializeField]
	private RectTransform rectTransform;

	[SerializeField]
	private float spinSpeed = 5f;

	private readonly Vector3 direction = new Vector3(0f, 0f, 1f);

	private void Update()
	{
		rectTransform.Rotate(direction * spinSpeed * Time.deltaTime * 60f);
	}
}
