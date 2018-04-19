using UnityEngine;
using UnityEngine.UI;

public class DesktopLayoutGroupScaleFixer : MonoBehaviour
{
	private const float screenHeightReference = 1440f;

	private const float screenWidthReference = 1920f;

	[SerializeField]
	private VerticalLayoutGroup layoutGroupVertical;

	[SerializeField]
	private HorizontalLayoutGroup layoutGroupHorizontal;

	private void Start()
	{
		if (layoutGroupVertical != null)
		{
			float num = 1440f * (float)Screen.height;
			RectOffset padding = layoutGroupVertical.padding;
			padding.top = (int)((float)padding.top / 1440f * (float)Screen.height);
			padding.bottom = (int)((float)padding.bottom / 1440f * (float)Screen.height);
			padding.left = (int)((float)padding.left / 1440f * (float)Screen.height);
			padding.right = (int)((float)padding.right / 1440f * (float)Screen.height);
			layoutGroupVertical.padding = padding;
			layoutGroupVertical.spacing = layoutGroupVertical.spacing / 1440f * (float)Screen.height;
		}
		if (layoutGroupHorizontal != null)
		{
			float num2 = 1920f * (float)Screen.width;
			RectOffset padding2 = layoutGroupHorizontal.padding;
			padding2.top = (int)((float)padding2.top / 1920f * (float)Screen.width);
			padding2.bottom = (int)((float)padding2.bottom / 1920f * (float)Screen.width);
			padding2.left = (int)((float)padding2.left / 1920f * (float)Screen.width);
			padding2.right = (int)((float)padding2.right / 1920f * (float)Screen.width);
			layoutGroupHorizontal.padding = padding2;
			layoutGroupHorizontal.spacing = layoutGroupHorizontal.spacing / 1920f * (float)Screen.width;
		}
	}
}
