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
			layoutGroupVertical.spacing = layoutGroupVertical.spacing / 1440f * (float)Screen.height;
		}
		if (layoutGroupHorizontal != null)
		{
			layoutGroupHorizontal.spacing = layoutGroupHorizontal.spacing / 1920f * (float)Screen.width;
		}
	}
}
