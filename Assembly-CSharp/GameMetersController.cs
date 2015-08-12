using System.Collections.Generic;

public class GameMetersController : UXViewScript
{
	private Dictionary<GameMeterType, GameMeterBase> gameMeters = new Dictionary<GameMeterType, GameMeterBase>();

	public override void OnInitialize()
	{
		base.OnInitialize();
		ResolveGUIElements();
	}

	private void ResolveGUIElements()
	{
		GameMeterBase[] componentsInChildren = GetComponentsInChildren<GameMeterBase>(includeInactive: true);
		GameMeterBase[] array = componentsInChildren;
		foreach (GameMeterBase gameMeterBase in array)
		{
			gameMeters.Add(gameMeterBase.GameMeterType, gameMeterBase);
		}
	}
}
