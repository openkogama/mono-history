using System;
using UnityEngine;

public class MVGUIEditorToggles : UXViewScript
{
	public UXToggleIconButton drawplaneToggle;

	public UXToggleIconButton logicRenderingToggle;

	public UXToggleIconButton gridSnapToggle;

	public UXGroup toggleGroup;

	public static bool GridSnap;

	private bool logicRendered;

	public bool LogicRendered
	{
		get
		{
			return logicRendered;
		}
		set
		{
			if (value != logicRendered)
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
		logicRendered = !logicRendered;
		Camera camera = ((Component)MVGameController.Instance.Game.CameraController).camera;
		if ((Object)(object)camera != (Object)null)
		{
			if (logicRendered)
			{
				camera.cullingMask |= 1 << (LayerMask.NameToLayer("Logic") & 0x1F);
			}
			else
			{
				camera.cullingMask &= ~(1 << (LayerMask.NameToLayer("Logic") & 0x1F));
			}
		}
		logicRenderingToggle.SetToggleState(logicRendered);
	}
}
