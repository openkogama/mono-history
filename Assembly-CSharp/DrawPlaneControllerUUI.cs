using MV.WorldObject;
using UnityEngine;

public class DrawPlaneControllerUUI : MonoBehaviour
{
	private bool inputEnabled = true;

	[SerializeField]
	protected WorldEditorDrawPlane worldEditorDrawPlane;

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
		get
		{
			return worldEditorDrawPlane.Orientation;
		}
		set
		{
			worldEditorDrawPlane.Orientation = value;
		}
	}

	public Vector3 Pos => worldEditorDrawPlane.Pos;

	public void Initialize()
	{
		worldEditorDrawPlane = Object.Instantiate(worldEditorDrawPlane);
		worldEditorDrawPlane.TargetGameObject = MVGameControllerBase.WOCM.GetSingletonWorldObject<MVCubeModelPrototypeTerrain>().GameObject;
		worldEditorDrawPlane.Active = false;
	}

	public bool Pick(ref Vector3 hit)
	{
		return worldEditorDrawPlane.Pick(ref hit);
	}

	public void DrawPlaneToModel(GameObject gameObject)
	{
		Debug.Log(gameObject.name);
		Debug.Log(worldEditorDrawPlane.name);
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
