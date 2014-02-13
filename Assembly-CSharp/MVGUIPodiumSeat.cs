using MV.Common;
using UnityEngine;

public class MVGUIPodiumSeat : MonoBehaviour
{
	public UXScrollingText winnerName;

	public UXText winnerText;

	public void BuildPodiumSeat(string name, int winnerData, MVWinningCondition winningType)
	{
		winnerName.Text = name;
		switch (winningType)
		{
		case MVWinningCondition.MostKills:
			winnerText.Text = "Kills: " + winnerData;
			break;
		case MVWinningCondition.FindAllCollectibles:
			winnerText.Text = "Items: " + winnerData;
			break;
		case MVWinningCondition.HighestAltitude:
		case MVWinningCondition.LowestAltitude:
		{
			Debug.Log((object)("WINNERDATA: " + winnerData));
			float num2 = (float)winnerData / 100f;
			winnerText.Text = string.Format("Altitude: " + num2.ToString("0.000"), new object[0]);
			break;
		}
		default:
		{
			float num = (float)winnerData / 1000f;
			winnerText.Text = $"Time: {num / 60f:00}:{num % 60f:00}.{(float)winnerData % 1000f:000}";
			break;
		}
		}
	}
}
