using MV.WorldObject;
using UnityEngine;

public abstract class PlayerListBase : MonoBehaviour
{
	public abstract MVTeam Team { get; }

	public abstract int PlayerCount { get; }

	public abstract int Score { get; }

	public abstract void Initialize(MVTeam team, int score, GameStatCounterType typeToDisplay);

	public abstract void Add(MVPlayer player);
}
