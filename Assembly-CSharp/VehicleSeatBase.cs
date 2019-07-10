using MV.Common;
using UnityEngine;

public class VehicleSeatBase : MonoBehaviour
{
	private int seatID = -1;

	private MVAvatar owner;

	private MVCameraBase seatCamera;

	[SerializeField]
	private MVCameraBase AndroidCamera;

	[SerializeField]
	private MVCameraBase DesktopCamera;

	public Transform AvatarAttachPoint;

	public SeatType SeatType;

	public bool UnequipVehicleUser;

	[SerializeField]
	private ControlType joystickType;

	public bool IsOccupied { get; set; }

	public MVAvatar Owner => owner;

	public MVCameraBase Camera => seatCamera;

	public int SeatID
	{
		get
		{
			return seatID;
		}
		set
		{
			if (seatID == -1)
			{
				seatID = value;
			}
			else
			{
				Debug.LogError("Trying to re-set seatID");
			}
		}
	}

	private void Awake()
	{
		seatCamera = DesktopCamera;
	}

	public void SetCamera()
	{
		if (seatCamera == null)
		{
			Debug.LogWarning("Camera is null");
			return;
		}
		((IVehicleCamera)seatCamera).Initialize((MVAvatarLocal)Owner);
		((AvatarLocal)Owner.Avatar).CameraController.PushCamera(seatCamera);
	}

	public virtual void RemoveCamera()
	{
		if (seatCamera == null)
		{
			Debug.LogWarning("Camera is null");
		}
		else
		{
			((AvatarLocal)Owner.Avatar).CameraController.RemoveCamera(seatCamera);
		}
	}

	public virtual void Attach(MVAvatar avatar)
	{
		if (UnequipVehicleUser)
		{
			MVEquipable component = avatar.GameObject.GetComponent<MVEquipable>();
			if (component != null)
			{
				component.Holster();
			}
		}
		owner = avatar;
	}

	public virtual void Detach(MVAvatar avatar)
	{
		if (UnequipVehicleUser)
		{
			MVEquipable component = avatar.GameObject.GetComponent<MVEquipable>();
			if (component != null)
			{
				component.Unholster();
			}
		}
	}
}
