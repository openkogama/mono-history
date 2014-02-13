using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class VehicleSeatManager : MonoBehaviour
{
	public List<VehicleSeatBase> seats = new List<VehicleSeatBase>();

	public TriggerBoxEvents triggerBoxEvents;

	private UseInteractor useInteractor;

	private MVVehicleBase woOwner;

	private int occupiedSeatCount;

	private bool isDead;

	private bool enterVehicleDisabled;

	public int OccupiedSeatsCount => occupiedSeatCount;

	public VehicleSeatBase DriverSeat
	{
		get
		{
			VehicleSeatBase vehicleSeatBase = null;
			foreach (VehicleSeatBase seat in seats)
			{
				if (seat.SeatType == SeatType.Driver)
				{
					if ((Object)(object)vehicleSeatBase != (Object)null)
					{
						Debug.LogError((object)"Multiple driver seats");
					}
					vehicleSeatBase = seat;
				}
			}
			if ((Object)(object)vehicleSeatBase == (Object)null)
			{
				Debug.LogError((object)"No driver seat");
			}
			return vehicleSeatBase;
		}
	}

	public bool EnterVehicleDisabled
	{
		get
		{
			return enterVehicleDisabled;
		}
		set
		{
			enterVehicleDisabled = value;
			UpdateTriggerBoxEventsCollider();
		}
	}

	private void OnIsDeadChange(object isDeadRuntime)
	{
		isDead = (bool)isDeadRuntime;
		UpdateTriggerBoxEventsCollider();
	}

	public void Init(MVVehicleBase wo, MVRuntimeDataVariable isDeadRuntimeVariable)
	{
		isDead = (bool)isDeadRuntimeVariable.Value;
		isDeadRuntimeVariable.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(isDeadRuntimeVariable.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(OnIsDeadChange));
		for (int i = 0; i < seats.Count; i++)
		{
			seats[i].SeatID = i;
		}
		woOwner = wo;
		useInteractor = new UseInteractor(woOwner.Id, reset: true, ((Component)triggerBoxEvents).collider, Use);
		triggerBoxEvents.TriggerEnter += useInteractor.triggerBoxEvents_TriggerEnter;
		triggerBoxEvents.TriggerExit += useInteractor.triggerBoxEvents_TriggerExit;
		foreach (MVWorldObjectClient child in wo.Children)
		{
			if (!(child is MVAvatar))
			{
				continue;
			}
			if (!child.RunTimeData.Contains("seat"))
			{
				Debug.LogError((object)"Did not find seat key");
				continue;
			}
			int num = (int)child.RunTimeData["seat"];
			if (num == -1)
			{
				Debug.LogError((object)"Found avatar child with seatID -1");
				continue;
			}
			Debug.Log((object)("Found avatar child with seat ID " + num));
			SetToSeatTransform((MVAvatar)child, num);
		}
	}

	public bool Use(int userWoId)
	{
		if (woOwner.IsDead)
		{
			return false;
		}
		foreach (VehicleSeatBase seat in seats)
		{
			if (!seat.IsOccupied && seat.SeatType == SeatType.Driver)
			{
				Debug.Log((object)("Trying to do seat operation " + woOwner));
				if (MVGameController.Instance.Game.PlayerController.AttachWorldObjectToSeat(woOwner.Id, userWoId, seat))
				{
					Debug.Log((object)"Succesfully send AttachWorldObjectToSeat");
					return true;
				}
			}
		}
		return false;
	}

	public void AttachWorldObjectToSeat(int instigatorActorNr, bool instigatorIsLocal, MVAvatar vehicleUser, int vehicleSeatID)
	{
		VehicleSeatBase vehicleSeatBase = seats[vehicleSeatID];
		if (!vehicleUser.RunTimeData.Contains("seat"))
		{
			Debug.LogError((object)"RunTimeData of wo does not contain seat");
			return;
		}
		bool flag = instigatorActorNr == woOwner.OwnerActorNr;
		bool flag2 = woOwner.OwnerActorNr == MVGameController.Instance.Game.LocalPlayer.ActorNr;
		if (!flag && flag2)
		{
			if (!(woOwner.NetworkObject is MVNetworkReporter))
			{
				Debug.LogError((object)"Expected reporter when vehicle is local");
				return;
			}
			Debug.Log((object)"Getting rid of reporter as vehicle is stolen by other user");
			woOwner.NetworkObject = new MVNetworkListener(woOwner);
		}
		if (vehicleSeatBase.SeatType == SeatType.Driver)
		{
			if (instigatorIsLocal && !flag)
			{
				if (woOwner.NetworkObject is MVNetworkReporter)
				{
					Debug.LogError((object)"Network reporter already set");
					return;
				}
				((MVNetworkListener)woOwner.NetworkObject).SetOwnerTransformToMostResentPackage();
				woOwner.NetworkObject = new MVNetworkReporter(woOwner);
			}
			woOwner.OwnerActorNr = instigatorActorNr;
		}
		woOwner.TransferChild(vehicleUser.Id);
		vehicleUser.ClearTransformQueue();
		vehicleUser.RunTimeData["seat"] = vehicleSeatID;
		SetToSeatTransform(vehicleUser, vehicleSeatID);
		if (instigatorIsLocal)
		{
			vehicleSeatBase.SetCamera();
		}
	}

	private void SetToSeatTransform(MVAvatar vehicleUser, int seatID)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		VehicleSeatBase vehicleSeatBase = seats[seatID];
		vehicleUser.GameObject.transform.parent = vehicleSeatBase.AvatarAttachPoint;
		vehicleUser.Position = -vehicleUser.CharacterControllerCenterOffset;
		vehicleUser.Rotation = Quaternion.identity;
		vehicleSeatBase.Attach(vehicleUser);
		occupiedSeatCount++;
		if (occupiedSeatCount > seats.Count)
		{
			Debug.LogError((object)("occupiedSeatCount more than number of seat " + occupiedSeatCount));
		}
		UpdateTriggerBoxEventsCollider();
	}

	private void UpdateTriggerBoxEventsCollider()
	{
		if (isDead || enterVehicleDisabled)
		{
			((Component)triggerBoxEvents).collider.enabled = false;
		}
		else if (occupiedSeatCount == seats.Count)
		{
			((Component)triggerBoxEvents).collider.enabled = false;
		}
		else
		{
			((Component)triggerBoxEvents).collider.enabled = true;
		}
	}

	public void DetachFromSeat(MVAvatar vehicleUser)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		int index = (int)vehicleUser.RunTimeData["seat"];
		vehicleUser.GameObject.transform.parent = woOwner.GameObject.transform;
		VehicleSeatBase vehicleSeatBase = seats[index];
		vehicleUser.GameObject.transform.localPosition = ((Component)vehicleSeatBase).transform.localPosition - vehicleUser.CharacterControllerCenterOffset;
		vehicleUser.GameObject.transform.localRotation = ((Component)vehicleSeatBase).transform.localRotation;
		MVGameController.Instance.WOCM.RootGroup.TransferChild(vehicleUser.Id);
		vehicleUser.RunTimeData["seat"] = -1;
		vehicleSeatBase.Detach(vehicleUser);
		occupiedSeatCount--;
		if (occupiedSeatCount < 0)
		{
			Debug.LogError((object)("occupiedSeatCount less than 0 " + occupiedSeatCount));
		}
		UpdateTriggerBoxEventsCollider();
	}
}
