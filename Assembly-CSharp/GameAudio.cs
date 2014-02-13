using UnityEngine;

public class GameAudio : MonoBehaviour
{
	public void BeforeRound()
	{
		Debug.Log((object)"Game Audio: BeforeRound");
	}

	public void StartRound()
	{
		Debug.Log((object)"Game Audio: StartRound");
	}

	public void RoundEndsNoWinner()
	{
		Debug.Log((object)"Game Audio: RoundEndsNoWinnder");
	}

	public void YouWonTheGame()
	{
		Debug.Log((object)"Game Audio: YouWonTheGame");
	}

	public void GameWonBySomeone()
	{
		Debug.Log((object)"Game Audio: GameWonBySomeone");
	}
}
