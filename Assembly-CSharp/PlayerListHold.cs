using MV.WorldObject;
using UnityEngine;

public class PlayerListHold : MonoBehaviour
{
	private int score;

	private int playerCount;

	private MVTeam team;

	[SerializeField]
	private RectTransform contentPanel;

	[SerializeField]
	private PlayerElementHold playerElementPrefab;

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
		PlayerElementHold playerElementHold = Object.Instantiate(playerElementPrefab);
		playerElementHold.transform.SetParent(contentPanel, worldPositionStays: false);
		playerElementHold.gameObject.SetActive(value: true);
		playerElementHold.Initialize(player);
		playerCount++;
	}
}
