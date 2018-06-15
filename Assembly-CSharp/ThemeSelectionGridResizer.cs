using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ThemeSelectionGridResizer : MonoBehaviour
{
	[SerializeField]
	private GridLayoutGroup grid;

	private float prevSize;

	private RectTransform RectTransform => (RectTransform)transform;

	protected void Update()
	{
		float width = RectTransform.rect.width;
		if (width != prevSize)
		{
			float num = (width - grid.spacing.x - (float)grid.padding.left - (float)grid.padding.right) / 2f;
			grid.cellSize = new Vector2(num, num);
			prevSize = width;
		}
	}
}
