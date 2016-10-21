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
		switch (MVGameControllerBase.Game.GameType)
		{
		case MVGameType.Classic:
			MVGameControllerBase.CameraController.PushCamera(seatCamera);
			break;
		case MVGameType.Platformer:
			break;
		default:
			MVGameControllerBase.CameraController.PushCamera(seatCamera);
			break;
		}
	}

	public virtual void RemoveCamera()
	{
		if (seatCamera == null)
		{
			Debug.LogWarning("Camera is null");
			return;
		}
		switch (MVGameControllerBase.Game.GameType)
		{
		case MVGameType.Classic:
			MVGameControllerBase.CameraController.RemoveCamera(seatCamera);
			break;
		case MVGameType.Platformer:
			break;
		default:
			MVGameControllerBase.CameraController.RemoveCamera(seatCamera);
			break;
		}
	}

	public virtual void Attach(MVAvatar avatar)
	{
		if (UnequipVehicleUser)
		{
			MVEquipable component = avatar.GameObject.GetComponent<MVEquipable>();
			if (component != null)
			{
				component.Unequip();
			}
		}
		owner = avatar;
	}

	public virtual void Detach(MVAvatar avatar)
	{
	}
}
