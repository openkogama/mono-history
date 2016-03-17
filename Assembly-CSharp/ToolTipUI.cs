using UnityEngine;
using UnityEngine.UI;

public class ToolTipUI : MonoBehaviour
{
	[SerializeField]
	private RectTransform rectTransform;

	[SerializeField]
	private Text toolTipText;

	public void Set(Vector2 position, string tooltip)
	{
		toolTipText.text = tooltip;
		rectTransform.pivot = GetPivot(position);
		rectTransform.position = position;
		rectTransform.SetAsLastSibling();
	}

	private Vector2 GetPivot(Vector2 position)
	{
		Vector2 result = new Vector2(0f, 0f);
		if (position.x > (float)Screen.width / 2f)
		{
			result.x = 1f;
		}
		if (position.y > (float)Screen.height / 2f)
		{
			result.y = 1f;
		}
		return result;
	}
}
