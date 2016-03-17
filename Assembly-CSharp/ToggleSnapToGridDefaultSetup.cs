using MV.Common;
using UnityEngine;

public class ToggleSnapToGridDefaultSetup : MonoBehaviour
{
	[SerializeField]
	private ToggleStatHandlerBase toggleStatHandlerBase;

	private void Start()
	{
		if (MVGameControllerBase.Game.GameType == MVGameType.Platformer)
		{
			toggleStatHandlerBase.Toggle();
		}
	}
}
