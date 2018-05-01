using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebriefingWinnerGUI : MonoBehaviour
{
	[Serializable]
	private struct WinningConditionData
	{
		public WinningConditionType ScoreType;

		public Image ScoreImage;

		public GameObject WinningConditionImage;

		public Text AdditionalInfo;

		public GameObject InfoBG;
	}

	[SerializeField]
	private Text winnerName;

	[SerializeField]
	private Text timer;

	[SerializeField]
	private Text winValue;

	[SerializeField]
	private RawImage winnerImage;

	[SerializeField]
	private ImageAnimator backgroundImage;

	[SerializeField]
	private List<WinningConditionData> winConImages;

	public void SetWinnerImage(Color startColor, RenderTexture image)
	{
		Color end = startColor * 1.25f;
		end.a = 1f;
		backgroundImage.SetColor(startColor, end);
		winnerImage.texture = image;
	}

	public void SetAdditionalInformation(string text, WinningConditionType winConType)
	{
		if (text == string.Empty)
		{
			return;
		}
		for (int i = 0; i < winConImages.Count; i++)
		{
			if (winConImages[i].ScoreType == winConType)
			{
				winConImages[i].InfoBG.SetActive(value: true);
				winConImages[i].AdditionalInfo.text = text;
			}
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

	public void ActivateScoreImage(WinningConditionType statType)
	{
		for (int i = 0; i < winConImages.Count; i++)
		{
			if (statType == winConImages[i].ScoreType)
			{
				winConImages[i].ScoreImage.gameObject.SetActive(value: true);
				winConImages[i].WinningConditionImage.SetActive(value: true);
			}
			else
			{
				winConImages[i].ScoreImage.gameObject.SetActive(value: false);
				winConImages[i].WinningConditionImage.SetActive(value: false);
			}
		}
	}
}
