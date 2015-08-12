using MV.Common;
using UnityEngine;

public class VehicleSeatBase : MonoBehaviour
{
	public SeatType SeatType;

	public bool UnequipVehicleUser;

	public MVCameraBase Camera;

	public Transform AvatarAttachPoint;

	private MVAvatar owner;

	private int seatID = -1;

	public bool IsOccupied { get; set; }

	public MVAvatar Owner => owner;

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

	public void SetCamera()
	{
		if (Camera == null)
		{
			Debug.LogWarning("Camera is null");
			return;
		}
		switch (GameDB.GameType)
		{
		case MVGameType.Classic:
			MVGameController.Game.CameraController.PushCamera(Camera);
			break;
		case MVGameType.Platformer:
			break;
		default:
			MVGameController.Game.CameraController.PushCamera(Camera);
			break;
		}
	}

	public virtual void RemoveCamera()
	{
		if (Camera == null)
		{
			Debug.LogWarning("Camera is null");
			return;
		}
		switch (GameDB.GameType)
		{
		case MVGameType.Classic:
			MVGameController.Game.CameraController.RemoveCamera(Camera);
			break;
		case MVGameType.Platformer:
			break;
		default:
			MVGameController.Game.CameraController.RemoveCamera(Camera);
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
