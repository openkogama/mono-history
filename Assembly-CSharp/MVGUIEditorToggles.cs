using System;

public class MVGUIEditorToggles : UXViewScript
{
	public UXToggleIconButton logicRenderingToggle;

	public UXToggleIconButton gridSnapToggle;

	public UXGroup toggleGroup;

	public static bool GridSnap;

	public bool LogicRendered
	{
		get
		{
			return MVGameController.Game.CameraController.IsLogicRendered;
		}
		set
		{
			if (value != MVGameController.Game.CameraController.IsLogicRendered)
			{
				ToggleLogicRendering();
			}
		}
	}

	public void InitializeButtons()
	{
		UXToggleIconButton uXToggleIconButton = logicRenderingToggle;
		uXToggleIconButton.OnToggle = (UXToggleIconButton.OnToggleDelegate)Delegate.Combine(uXToggleIconButton.OnToggle, (UXToggleIconButton.OnToggleDelegate)((bool active) =>
		{
			LogicRendered = active;
		}));
		LogicRendered = true;
		gridSnapToggle.ToggleState = GridSnap;
		UXToggleIconButton uXToggleIconButton2 = gridSnapToggle;
		uXToggleIconButton2.OnToggle = (UXToggleIconButton.OnToggleDelegate)Delegate.Combine(uXToggleIconButton2.OnToggle, (UXToggleIconButton.OnToggleDelegate)((bool active) =>
		{
			GridSnap = active;
		}));
	}

	public void ToggleLogicRendering()
	{
		bool flag = !MVGameController.Game.CameraController.IsLogicRendered;
		MVGameController.Game.CameraController.RenderLogic(flag);
		logicRenderingToggle.SetToggleState(flag);
	}
}
