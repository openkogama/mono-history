using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ThemeImageUVScaler : MonoBehaviour
{
	[SerializeField]
	private RawImage image;

	private Rect prevRect;

	private RectTransform RectTransform => (RectTransform)transform;

	protected void Reset()
	{
		image = GetComponent<RawImage>();
	}

	private void Update()
	{
		Rect rect = RectTransform.rect;
		if (image != null && prevRect != rect)
		{
			prevRect = rect;
			rect.x = 0f;
			rect.y = 0f;
			rect.height /= rect.width;
			rect.width = 1f;
			image.uvRect = rect;
		}
	}
}
