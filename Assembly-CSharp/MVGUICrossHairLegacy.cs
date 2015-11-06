using UnityEngine;

public class MVGUICrossHairLegacy : IGUICrossHair
{
	private Aiming2DCameraDesktop aiming2DCamera = new Aiming2DCameraDesktop();

	private MVGUICrossHair guiCrossHair;

	private Vector3 origin;

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
			return origin;
		}
		set
		{
			origin = value;
		}
	}

	public bool Visible
	{
		get
		{
			if (!guiCrossHair.group)
			{
				return false;
			}
			return guiCrossHair.group.Visible;
		}
		set
		{
			if ((bool)guiCrossHair.group)
			{
				guiCrossHair.group.SetVisible(value);
			}
		}
	}

	public bool FiredThisFrame { get; private set; }

	public Vector3 ScreenPos
	{
		get
		{
			Camera component = UXUtils.UXCamera.GetComponent<Camera>();
			return component.WorldToScreenPoint(guiCrossHair.transform.position);
		}
	}

	public MVGUICrossHairLegacy()
	{
		guiCrossHair = (Object.Instantiate(Resources.Load("Prefabs/GUI/CrossHair")) as GameObject).GetComponent<MVGUICrossHair>();
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

	public void UpdateCrosshairPosition()
	{
		float scale = MVGameControllerBase.CameraController.GetCamera<PlatformerCamera>().Scale;
		aiming2DCamera.Scale = scale;
		aiming2DCamera.UpdateFireDirection(origin);
		Vector3 vector = AimPositionOnCameraPlane(origin, aiming2DCamera.Direction);
		direction = CrossHairDirectionOnAvatarPlane(vector);
		SetPosition(vector);
	}

	private Vector3 CrossHairDirectionOnAvatarPlane(Vector3 crossHairOnCameraPlane)
	{
		Plane plane = new Plane(Vector3.back, origin);
		Ray ray = new Ray(MVGameControllerBase.CameraController.MainCamera.transform.position, crossHairOnCameraPlane - MVGameControllerBase.CameraController.MainCamera.transform.position);
		if (plane.Raycast(ray, out var enter))
		{
			return ray.GetPoint(enter) - origin;
		}
		Debug.LogWarning("This happens when going from hidden lobby state to playing");
		return Vector3.zero;
	}

	private Vector3 AimPositionOnCameraPlane(Vector3 origin, Vector3 inputDirection)
	{
		Vector3 vector = MVGameControllerBase.CameraController.MainCamera.transform.rotation * inputDirection;
		return vector + origin;
	}

	private void SetPosition(Vector3 position)
	{
		Vector3 position2 = MVGameControllerBase.CameraController.MainCamera.WorldToScreenPoint(position);
		Camera component = UXUtils.UXCamera.GetComponent<Camera>();
		guiCrossHair.transform.position = component.ScreenToWorldPoint(position2);
	}
}
