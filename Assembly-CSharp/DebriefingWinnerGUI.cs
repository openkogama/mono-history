using UnityEngine;
using UnityEngine.UI;

public class DebriefingWinnerGUI : MonoBehaviour
{
	[SerializeField]
	private Text winnerName;

	[SerializeField]
	private WinningConditionAndroid winningConditionPrefab;

	[SerializeField]
	private Text timer;

	[SerializeField]
	private Text winValue;

	[SerializeField]
	private Text additionalInfo;

	[SerializeField]
	private GameObject infoBG;

	[SerializeField]
	private RawImage winnerImage;

	[SerializeField]
	private ImageAnimator backgroundImage;

	public void SetWinnerImage(Color startColor, RenderTexture image)
	{
		Color end = startColor * 1.25f;
		end.a = 1f;
		backgroundImage.SetColor(startColor, end);
		winnerImage.texture = image;
	}

	public void SetWinningConditionSprite(Sprite sprite)
	{
		winningConditionPrefab.SetSprite(sprite);
	}

	public void SetAdditionalInformation(string text)
	{
		if (!(text == string.Empty))
		{
			infoBG.SetActive(value: true);
			additionalInfo.text = text;
		}
	}

	public void SetWinValue(string winVal)
	{
		winValue.text = winVal;
	}

	public void SetTimerText(string time)
	{
		timer.text = time;
	}

	public void SetWinnerText(string winner)
	{
		winnerName.text = winner;
	}
}
