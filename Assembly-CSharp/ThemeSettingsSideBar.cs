using UnityEngine;
using UnityEngine.UI;

public class ThemeSettingsSideBar : MonoBehaviour
{
	[SerializeField]
	private RectTransform content;

	[SerializeField]
	private Image closeImage;

	[SerializeField]
	private Image backImage;

	public RectTransform Content => content;

	public void InitializeForPreview()
	{
		closeImage.gameObject.SetActive(value: false);
		backImage.gameObject.SetActive(value: true);
	}
}
