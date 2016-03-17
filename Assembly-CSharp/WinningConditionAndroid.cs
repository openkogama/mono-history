using UnityEngine;
using UnityEngine.UI;

public class WinningConditionAndroid : MonoBehaviour
{
	[SerializeField]
	private Text limit;

	[SerializeField]
	private Text additionalInfo;

	[SerializeField]
	private GameObject infoBG;

	[SerializeField]
	private Image image;

	public void SetSprite(Sprite sprite)
	{
		image.sprite = sprite;
	}

	public void SetAdditionalInformation(string info)
	{
		if (!(info == string.Empty))
		{
			infoBG.SetActive(value: true);
			additionalInfo.text = info;
		}
	}

	public void SetLimit(int limit)
	{
		this.limit.text = limit.ToString();
	}

	public void HideLimit()
	{
		limit.text = string.Empty;
	}
}
