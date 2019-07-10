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
		drawPlaneControls.gameObject.SetActive(DrawPlane.IsDrawPlaneActive);
	}

	public override void ExecuteToggleState(bool toggleState, UnityAction<bool> toggleCallback)
	{
		DrawPlane.ToggleDrawPlane();
		toggleCallback(DrawPlane.IsDrawPlaneActive);
		drawPlaneControls.gameObject.SetActive(DrawPlane.IsDrawPlaneActive);
	}
}
