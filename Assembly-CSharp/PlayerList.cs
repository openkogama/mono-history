using MV.WorldObject;
using UnityEngine;

public class PlayerList : MonoBehaviour
{
	private int score;

	private int playerCount;

	private MVTeam team;

	[SerializeField]
	private RectTransform contentPanel;

	[SerializeField]
	private PlayerElement playerElementPrefab;

	[SerializeField]
	private TeamTab teamTab;

	public MVTeam Team => team;

	public int PlayerCount => playerCount;

	public int Score => score;

	public void Initialize(MVTeam team, int score)
	{
		this.score = score;
		this.team = team;
		teamTab.Initialize(team);
	}

	public void Add(MVPlayer player)
	{
		PlayerElement playerElement = Object.Instantiate(playerElementPrefab);
		playerElement.transform.SetParent(contentPanel, worldPositionStays: false);
		playerElement.gameObject.SetActive(value: true);
		playerElement.Initialize(player);
		playerCount++;
	}
}
