using MV.Common;
using UnityEngine;
using UnityEngine.Events;

public class ToggleDrawPlaneHandlerTerrain : ToggleHandler
{
	[SerializeField]
	private ToggleStatHandlerBase toggleStatHandlerBase;

	[SerializeField]
	private GameObject drawPlaneControls;

	private void OnEnable()
	{
		toggleStatHandlerBase.ToggleState = DrawPlane.IsDrawPlaneActive;
		drawPlaneControls.gameObject.SetActive(DrawPlane.IsDrawPlaneActive && MVGameControllerBase.Game.GameType != MVGameType.Platformer);
		if (MVGameControllerBase.Game.GameType == MVGameType.Platformer)
		{
			toggleStatHandlerBase.Toggle();
		}
	}

	public override void ExecuteToggleState(bool toggleState, UnityAction<bool> toggleCallback)
	{
		DrawPlane.ToggleDrawPlane();
		toggleCallback(DrawPlane.IsDrawPlaneActive);
		drawPlaneControls.gameObject.SetActive(DrawPlane.IsDrawPlaneActive && MVGameControllerBase.Game.GameType != MVGameType.Platformer);
		if (MVGameControllerBase.Game.GameType == MVGameType.Platformer)
		{
			DrawPlane.SetToTerrain(DrawPlane.IsDrawPlaneActive);
		}
	}
}
