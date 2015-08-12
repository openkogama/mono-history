using UnityEngine;

public class MVGUIPodiumSeat : MonoBehaviour
{
	public UXScrollingText winnerName;

	public UXText winnerText;

	public void BuildPodiumSeat(string name, int winnerData, GameStatCounterType statType)
	{
		winnerName.Text = name;
		Debug.Log(statType);
		switch (statType)
		{
		case GameStatCounterType.Kill:
		case GameStatCounterType.OculusKill:
			winnerText.Text = "Kills: " + winnerData;
			break;
		case GameStatCounterType.Collectible:
			winnerText.Text = "Items: " + winnerData;
			break;
		case GameStatCounterType.YUp:
		case GameStatCounterType.YDown:
		{
			Debug.Log("WINNERDATA: " + winnerData);
			float num2 = (float)winnerData / 100f;
			winnerText.Text = string.Format("Altitude: " + num2.ToString("0.000"));
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
