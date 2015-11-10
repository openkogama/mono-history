using System;
using MV.Common;
using UnityEngine;

public class MVGUICrossHairLegacy : IGUICrossHair
{
	private MVGUICrossHair guiCrossHair;

	private Vector3 crossHairPosition = new Vector3(0f, 0f);

	private Vector3 avatarPlanePosition = Vector3.zero;

	private Vector3 direction = Vector3.right;

	public Vector3 Direction
	{
		get
		{
			return direction;
		}
		set
		{
		}
	}

	public Vector3 Origin
	{
		get
		{
			throw new Exception("not implemented");
		}
		set
		{
			throw new Exception("not implemented");
		}
	}

	public Vector3 AvatarPlanePosition => avatarPlanePosition;

	public bool Visible
	{
		get
		{
			return guiCrossHair.group.Visible;
		}
		set
		{
			guiCrossHair.group.SetVisible(value);
		}
	}

	public bool FiredThisFrame { get; private set; }

	public MVGUICrossHairLegacy()
	{
		if (MVGameControllerBase.Game.GameType == MVGameType.Platformer)
		{
			guiCrossHair = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/CrossHairPlatformer")) as GameObject).GetComponent<MVGUICrossHair>();
			UXUtils.AddSubTree(guiCrossHair.transform);
			UXUtils.FindGUIObjectOfType<LockCursorManagerPlatformer>().GUICrossHairLegacy = this;
		}
		else
		{
			guiCrossHair = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/CrossHair")) as GameObject).GetComponent<MVGUICrossHair>();
		}
	}

	public void UpdateCrossHair(int ammo, Color color, float chargeState, bool firedThisFrame)
	{
		FiredThisFrame = firedThisFrame;
		if (guiCrossHair.group.Visible)
		{
			if (ammo == 0 && guiCrossHair.ammoText.Visible)
			{
				guiCrossHair.ammoText.SetVisible(visible: false);
			}
			if (ammo > 0 && !guiCrossHair.ammoText.Visible)
			{
				guiCrossHair.ammoText.SetVisible(visible: true);
			}
			if (guiCrossHair.ammoText.Visible)
			{
				guiCrossHair.ammoText.Text = string.Empty + ammo;
			}
			guiCrossHair.crossHairPlane.SetColor(color, string.Empty);
			guiCrossHair.chargeText.SetVisible(chargeState > 0f);
			if (guiCrossHair.chargeText.Visible)
			{
				guiCrossHair.chargeText.Text = string.Empty + Mathf.Round(chargeState * 100f);
			}
		}
	}

	public void HandleInput()
	{
		crossHairPosition = Input.mousePosition;
		Camera component = UXUtils.UXCamera.GetComponent<Camera>();
		Ray ray = MVGameControllerBase.CameraController.MainCamera.ScreenPointToRay(crossHairPosition);
		if (new Plane(Vector3.back, MVGameControllerBase.WOCM.AvatarLocal.LookAtPos).Raycast(ray, out var enter))
		{
			avatarPlanePosition = ray.GetPoint(enter);
		}
		crossHairPosition = component.ScreenToWorldPoint(crossHairPosition);
		crossHairPosition.z = 0f;
	}

	private Vector3 ConstrainToScreen(Vector3 screenPosPosition)
	{
		if (screenPosPosition.x < 0f)
		{
			screenPosPosition.x = 0f;
		}
		if (screenPosPosition.y < 0f)
		{
			screenPosPosition.y = 0f;
		}
		if (screenPosPosition.x > (float)Screen.width)
		{
			screenPosPosition.x = Screen.width;
		}
		if (screenPosPosition.y > (float)Screen.height)
		{
			screenPosPosition.y = Screen.height;
		}
		return screenPosPosition;
	}
}
