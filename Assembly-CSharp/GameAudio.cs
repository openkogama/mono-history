using UnityEngine;

public class GameAudio : MonoBehaviour
{
	public void BeforeRound()
	{
		Debug.Log("Game Audio: BeforeRound");
	}

	public void StartRound()
	{
		Debug.Log("Game Audio: StartRound");
	}

	public void RoundEndsNoWinner()
	{
		Debug.Log("Game Audio: RoundEndsNoWinnder");
	}

	public void YouWonTheGame()
	{
		Debug.Log("Game Audio: YouWonTheGame");
	}

	public void GameWonBySomeone()
	{
		Debug.Log("Game Audio: GameWonBySomeone");
	}
}
