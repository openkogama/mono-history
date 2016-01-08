using System;
using MV.WorldObject;
using UnityEngine;

public class DrawPlaneController
{
	public delegate void OnWorkPlaneAltitudeChangedDelegate(int altitude);

	private bool inputEnabled = true;

	public OnWorkPlaneAltitudeChangedDelegate OnDrawplaneAltitudeChanged;

	protected WorldEditorDrawPlane worldEditorDrawPlane;

	protected MVGUIDrawplaneControls workplaneArrows;

	public bool IsDrawPlaneActive => worldEditorDrawPlane.Active;

	public bool InputEnabled
	{
		get
		{
			return inputEnabled;
		}
		set
		{
			inputEnabled = value;
		}
	}

	public int Altitude => worldEditorDrawPlane.Altitude;

	public DrawPlaneAxis Orientation
	{
		set
		{
			worldEditorDrawPlane.Orientation = value;
		}
	}

	public Vector3 Pos => worldEditorDrawPlane.Pos;

	public void ResolveCubeTools(GameObject gui)
	{
		CreateDrawPlane();
		workplaneArrows = AIngameController.FindGUIObjectOfType<MVGUIDrawplaneControls>(gui);
	}

	public void CreateDrawPlane()
	{
		if (!(this.worldEditorDrawPlane != null))
		{
			this.worldEditorDrawPlane = UnityEngine.Object.Instantiate(PrefabPool.Instance.DrawPlaneObject).GetComponent<WorldEditorDrawPlane>();
			this.worldEditorDrawPlane.TargetGameObject = MVGameControllerBase.WOCM.GetSingletonWorldObject<MVCubeModelPrototypeTerrain>().GameObject;
			this.worldEditorDrawPlane.Active = false;
			WorldEditorDrawPlane worldEditorDrawPlane = this.worldEditorDrawPlane;
			worldEditorDrawPlane.OnAltitudeChanged = (WorldEditorDrawPlane.AltitudeChangedDelegate)Delegate.Combine(worldEditorDrawPlane.OnAltitudeChanged, new WorldEditorDrawPlane.AltitudeChangedDelegate(NotifyAltitudeUpdate));
		}
	}

	public bool Pick(ref Vector3 hit)
	{
		return worldEditorDrawPlane.Pick(ref hit);
	}

	private void NotifyAltitudeUpdate(int altitude)
	{
		if (OnDrawplaneAltitudeChanged != null)
		{
			OnDrawplaneAltitudeChanged(altitude);
		}
	}

	public void DrawPlaneToModel(GameObject gameObject)
	{
		worldEditorDrawPlane.CachePos();
		worldEditorDrawPlane.TargetGameObject = gameObject;
		worldEditorDrawPlane.SetToTargetGameObjectZero();
	}

	public void ToggleDrawPlane()
	{
		if (!(worldEditorDrawPlane == null))
		{
			if (worldEditorDrawPlane.IsOnLandscape)
			{
				worldEditorDrawPlane.SetToCameraPos();
			}
			else
			{
				worldEditorDrawPlane.SetToTargetGameObjectZero();
			}
			worldEditorDrawPlane.Active = !worldEditorDrawPlane.Active;
			workplaneArrows.View.SetVisible(worldEditorDrawPlane.Active);
		}
	}

	public void CheckInput()
	{
		if (MVInputWrapper.GetBooleanControl(KogamaControls.MoveDrawPlaneDown))
		{
			MoveDrawPlane(-1);
		}
		if (MVInputWrapper.GetBooleanControl(KogamaControls.MoveDrawPlaneUp))
		{
			MoveDrawPlane(1);
		}
	}

	public void Update()
	{
		if (InputEnabled)
		{
			CheckInput();
		}
		if (worldEditorDrawPlane != null && worldEditorDrawPlane.Active)
		{
			worldEditorDrawPlane.UpdateDrawPlane();
		}
	}

	public void HideDrawPlane()
	{
		Debug.Log("Hide drawplane");
		if (worldEditorDrawPlane.Active)
		{
			ToggleDrawPlane();
		}
	}

	public void ReturnDrawPlaneToLandscape()
	{
		Debug.Log("Return drawplane");
		worldEditorDrawPlane.ReturnDrawPlaneToLandscape();
	}

	public bool GetCubePosOnDrawplane(GameObject gameObject, out IntVector intVectorHitPosition)
	{
		return worldEditorDrawPlane.GetCubePosOnDrawplane(gameObject, out intVectorHitPosition);
	}

	public void MoveDrawPlane(int dir)
	{
		worldEditorDrawPlane.MoveDrawPlane(dir);
	}
}
