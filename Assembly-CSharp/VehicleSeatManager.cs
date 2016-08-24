using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using UnityEngine;

public class VehicleSeatManager : MonoBehaviour
{
	public delegate void OnSeatOccupiedChangeDelegate();

	public OnSeatOccupiedChangeDelegate OnSeatOccupiedChange;

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
					if (vehicleSeatBase != null)
					{
						Debug.LogError("Multiple driver seats");
					}
					vehicleSeatBase = seat;
				}
			}
			if (vehicleSeatBase == null)
			{
				Debug.LogError("No driver seat");
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
		useInteractor = new UseInteractor(woOwner, gameObject, reset: true, triggerBoxEvents.Collider, Use, CheckCanUse);
		triggerBoxEvents.TriggerEnter += useInteractor.triggerBoxEvents_TriggerEnter;
		triggerBoxEvents.TriggerExit += useInteractor.triggerBoxEvents_TriggerExit;
		foreach (MVWorldObjectClient child in wo.Children)
		{
			if (!(child is MVAvatar))
			{
				continue;
			}
			if (!child.RunTimeData.ContainsObscuredKey("seat"))
			{
				Debug.LogError("Did not find seat key");
				continue;
			}
			int num = (ObscuredInt)child.RunTimeData.GetObscuredType("seat");
			if (num == -1)
			{
				Debug.LogError("Found avatar child with seatID -1");
				continue;
			}
			Debug.Log("Found avatar child with seat ID " + num);
			SetToSeatTransform((MVAvatar)child, num);
		}
	}

	private bool CheckCanUse(MVInteractableBase avatarInteractable)
	{
		if (avatarInteractable.HasModifierEffect(AvatarModifierEffect.DisableVehicles))
		{
			return false;
		}
		foreach (VehicleSeatBase seat in seats)
		{
			if (!seat.IsOccupied)
			{
				return true;
			}
		}
		return false;
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
				Debug.Log("Trying to do seat operation " + woOwner);
				if (MVGameControllerBase.Game.PlayerController.AttachWorldObjectToSeat(woOwner.Id, userWoId, seat))
				{
					Debug.Log("Succesfully send AttachWorldObjectToSeat");
					return true;
				}
			}
		}
		return false;
	}

	public void AttachWorldObjectToSeat(int instigatorActorNr, bool instigatorIsLocal, MVAvatar vehicleUser, int vehicleSeatID)
	{
		VehicleSeatBase vehicleSeatBase = seats[vehicleSeatID];
		if (!vehicleUser.RunTimeData.ContainsObscuredKey("seat"))
		{
			Debug.LogError("RunTimeData of wo does not contain seat");
			return;
		}
		bool flag = instigatorActorNr == woOwner.OwnerActorNr;
		bool flag2 = woOwner.OwnerActorNr == MVGameControllerBase.Game.LocalPlayer.ActorNr;
		if (!flag && flag2)
		{
			MVNetworkObject networkObject = MVGameControllerBase.Game.TransformNetworkManager.GetNetworkObject(woOwner.Id);
			if (networkObject == null || !(networkObject is MVNetworkReporter))
			{
				Debug.LogError("Expected reporter when vehicle is local");
				return;
			}
			Debug.Log("Getting rid of reporter as vehicle is stolen by other user");
			MVGameControllerBase.Game.TransformNetworkManager.RemoveNetworkObject(woOwner.Id);
			MVGameControllerBase.Game.RuntimeVariableNetworkManager.RemoveRuntimeDataVariables(woOwner.Id);
		}
		if (vehicleSeatBase.SeatType == SeatType.Driver)
		{
			if (instigatorIsLocal && !flag)
			{
				MVNetworkObject networkObject2 = MVGameControllerBase.Game.TransformNetworkManager.GetNetworkObject(woOwner.Id);
				if (networkObject2 != null && networkObject2 is MVNetworkReporter)
				{
					Debug.LogError("Network reporter already set");
					return;
				}
				if (networkObject2 != null)
				{
					((MVNetworkListener)networkObject2).SetOwnerTransformToMostResentPackage();
					MVGameControllerBase.Game.TransformNetworkManager.RemoveNetworkObject(woOwner.Id);
				}
				MVGameControllerBase.Game.TransformNetworkManager.AddReporter(woOwner.Id, new MVNetworkReporter(woOwner));
				MVGameControllerBase.Game.RuntimeVariableNetworkManager.AddRuntimeDataVariables(woOwner.Id);
			}
			woOwner.OwnerActorNr = instigatorActorNr;
		}
		woOwner.TransferChild(vehicleUser.Id);
		MVGameControllerBase.Game.TransformNetworkManager.RemoveNetworkObject(vehicleUser.Id);
		vehicleUser.RunTimeData.SetObscuredType("seat", (ObscuredInt)vehicleSeatID);
		SetToSeatTransform(vehicleUser, vehicleSeatID);
		if (instigatorIsLocal)
		{
			vehicleSeatBase.SetCamera();
		}
		if (OnSeatOccupiedChange != null)
		{
			OnSeatOccupiedChange();
		}
	}

	private void SetToSeatTransform(MVAvatar vehicleUser, int seatID)
	{
		VehicleSeatBase vehicleSeatBase = seats[seatID];
		vehicleUser.GameObject.transform.parent = vehicleSeatBase.AvatarAttachPoint;
		vehicleUser.Position = -vehicleUser.CharacterControllerCenterOffset;
		vehicleUser.Rotation = Quaternion.identity;
		vehicleSeatBase.Attach(vehicleUser);
		occupiedSeatCount++;
		if (occupiedSeatCount > seats.Count)
		{
			Debug.LogError("occupiedSeatCount more than number of seat " + occupiedSeatCount);
		}
		UpdateTriggerBoxEventsCollider();
	}

	private void UpdateTriggerBoxEventsCollider()
	{
		if (isDead || enterVehicleDisabled)
		{
			triggerBoxEvents.Collider.enabled = false;
		}
		else if (occupiedSeatCount == seats.Count)
		{
			triggerBoxEvents.Collider.enabled = false;
		}
		else
		{
			triggerBoxEvents.Collider.enabled = true;
		}
	}

	public void DetachFromSeat(MVAvatar vehicleUser)
	{
		int index = (ObscuredInt)vehicleUser.RunTimeData.GetObscuredType("seat");
		vehicleUser.GameObject.transform.parent = woOwner.GameObject.transform;
		VehicleSeatBase vehicleSeatBase = seats[index];
		vehicleUser.GameObject.transform.localPosition = vehicleSeatBase.transform.localPosition - vehicleUser.CharacterControllerCenterOffset;
		vehicleUser.GameObject.transform.localRotation = vehicleSeatBase.transform.localRotation;
		MVGameControllerBase.WOCM.RootGroup.TransferChild(vehicleUser.Id);
		vehicleUser.RunTimeData.SetObscuredType("seat", (ObscuredInt)(-1));
		if (vehicleUser.GetType() == typeof(MVAvatarLocal))
		{
			Debug.Log("LOCAL AVATAR");
			vehicleSeatBase.RemoveCamera();
		}
		vehicleSeatBase.Detach(vehicleUser);
		occupiedSeatCount--;
		if (occupiedSeatCount < 0)
		{
			Debug.LogError("occupiedSeatCount less than 0 " + occupiedSeatCount);
		}
		UpdateTriggerBoxEventsCollider();
		if (OnSeatOccupiedChange != null)
		{
			OnSeatOccupiedChange();
		}
	}
}
