using MV.WorldObject;
using UnityEngine;
using UnityEngine.EventSystems;

public static class DrawPlane
{
	private static DrawPlaneControllerUUI drawPlaneController;

	public static bool IsDrawPlaneActive => drawPlaneController.IsDrawPlaneActive;

	public static int Altitude => drawPlaneController.Altitude;

	public static DrawPlaneAxis Orientation
	{
		get
		{
			return drawPlaneController.Orientation;
		}
		set
		{
			drawPlaneController.Orientation = value;
		}
	}

	public static Vector3 Pos => drawPlaneController.Pos;

	public static bool InputEnabled
	{
		get
		{
			return drawPlaneController.InputEnabled;
		}
		set
		{
			drawPlaneController.InputEnabled = value;
		}
	}

	public static void Initialize(DrawPlaneControllerUUI drawPlaneController)
	{
		DrawPlane.drawPlaneController = drawPlaneController;
	}

	public static void Reset()
	{
		drawPlaneController = null;
	}

	public static bool Pick(ref Vector3 hit)
	{
		if (EventSystem.current.IsPointerOverGameObject())
		{
			return false;
		}
		return drawPlaneController.Pick(ref hit);
	}

	public static void DrawPlaneToModel(GameObject gameObject)
	{
		drawPlaneController.DrawPlaneToModel(gameObject);
	}

	public static void ToggleDrawPlane()
	{
		drawPlaneController.ToggleDrawPlane();
	}

	public static void HideDrawPlane()
	{
		drawPlaneController.HideDrawPlane();
	}

	public static void ReturnDrawPlaneToLandscape()
	{
		drawPlaneController.ReturnDrawPlaneToLandscape();
	}

	public static bool GetCubePosOnDrawplane(GameObject gameObject, out IntVector intVectorHitPosition)
	{
		return drawPlaneController.GetCubePosOnDrawplane(gameObject, out intVectorHitPosition);
	}

	public static void MoveDrawPlane(int dir)
	{
		drawPlaneController.MoveDrawPlane(dir);
	}

	public static void SetToTerrain(bool active)
	{
		((DrawPlaneController2DUUI)drawPlaneController).SetToTerrain(active);
	}
}
