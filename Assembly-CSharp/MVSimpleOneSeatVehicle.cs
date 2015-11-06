using System;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public abstract class MVSimpleOneSeatVehicle : MVVehicleBase
{
	protected class LocalObjectsSimpleVehicle : LocalObjectsBase
	{
		protected VehiclePickupOwner pickupOwner;

		protected PickupGUI pickupGUI;

		protected MVTriggerHandler triggerHandler;

		protected SimpleVehicleMotorBase vehicleMotor;

		private MVSimpleOneSeatVehicle owner;

		public override int Id => owner.Id;

		protected override MVVehicleBase Owner => owner;

		public LocalObjectsSimpleVehicle(MVSimpleOneSeatVehicle vehicleBase, SmoothCharacterController smoothController, SimpleVehicleMotorBase hoverCraftMotor)
		{
			MVRuntimeDataVariableClampedFloat health = vehicleBase.Health;
			health.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(health.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(OnHealthChange));
			GameObject gameObject = vehicleBase.GameObject;
			VehicleInteractable vehicleInteractable = gameObject.AddComponent<VehicleInteractable>();
			vehicleInteractable.Init(vehicleBase.Modifiers, vehicleBase.Health);
			hoverCraftMotor.Init(smoothController, vehicleInteractable);
			onLeave = (Action)Delegate.Combine(onLeave, new Action(hoverCraftMotor.OnLocalVehicleLeave));
			VehicleEquipable vehicleEquipable = gameObject.AddComponent<VehicleEquipable>();
			vehicleEquipable.Init(vehicleInteractable, vehicleBase.CurrentItem);
			pickupOwner = vehicleBase.GameObject.GetComponent<VehiclePickupOwner>();
			pickupOwner.IsLocal = true;
			onDestroy = (Action)Delegate.Combine(onDestroy, new Action(pickupOwner.OnLocalObjectsDestroyed));
			pickupGUI = new PickupGUI(pickupOwner);
			onDestroy = (Action)Delegate.Combine(onDestroy, new Action(pickupGUI.Destroy));
			onEnter = (Action)Delegate.Combine(onEnter, new Action(pickupGUI.Enter));
			onLeave = (Action)Delegate.Combine(onLeave, new Action(pickupGUI.Leave));
			triggerHandler = gameObject.AddComponent<MVTriggerHandler>();
			localComponents.Add(vehicleInteractable);
			localComponents.Add(smoothController);
			localComponents.Add(hoverCraftMotor);
			localComponents.Add(vehicleEquipable);
			localComponents.Add(triggerHandler);
			vehicleMotor = hoverCraftMotor;
			owner = vehicleBase;
		}

		public override void Destroy()
		{
			base.Destroy();
			MVRuntimeDataVariableClampedFloat health = owner.Health;
			health.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Remove(health.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(OnHealthChange));
		}

		public override void Leave()
		{
			base.Leave();
			owner.IsFiring.Value = false;
			triggerHandler.Reset();
		}

		public override void Enter()
		{
			base.Enter();
			triggerHandler.enabled = true;
			triggerHandler.Reset();
		}

		public override IInputToPlayerMovement FixedUpdate(IInputToPlayerMovement movementMap)
		{
			if (movementMap != null)
			{
				vehicleMotor.DirectInputMoveMap = movementMap.Direction;
				vehicleMotor.Jump = movementMap.Jump;
				vehicleMotor.HandleInput = true;
			}
			else
			{
				vehicleMotor.HandleInput = false;
			}
			vehicleMotor.VehicleUpdateFunction();
			return movementMap;
		}

		public override InputToInGameAction Update(InputToInGameAction interactionInput)
		{
			vehicleMotor.UpdateFunction();
			if (interactionInput == null)
			{
				return null;
			}
			pickupOwner.HandleFire(interactionInput.Fire, owner.IsFiring);
			pickupGUI.Update();
			interactionInput.Fire = false;
			if (interactionInput.Drop)
			{
				owner.GameObject.GetComponent<MVEquipable>().Unequip();
			}
			if (vehicleMotor.IsStuck())
			{
				Debug.Log("Vehicle is stuck");
			}
			interactionInput.IgnorePickupOwner = true;
			return interactionInput;
		}
	}

	public MVRuntimeDataVariableClampedFloat Health;

	public MVRuntimeDataVariable Modifiers;

	public MVRuntimeDataVariable CurrentItem;

	public MVRuntimeDataVariable IsFiring;

	protected EditableCubeModelWrapper editableCubeModelWrapper;

	protected MVSimpleOneSeatVehicle(Dictionary<object, object> data, string _vehiclePrefab, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, _vehiclePrefab, worldObjects)
	{
	}

	public override void Initialize()
	{
		base.Initialize();
		float maxValue = (float)RuntimeVariablesRepository.GetRuntimeVariables(WorldObjectType)["health"];
		Health = RuntimeDataVariables.NewClampedFloat("health", 0.2f, writeThrough: false, 0f, maxValue);
		CurrentItem = RuntimeDataVariables.New("currentItem", 0f, writeThrough: true);
		IsFiring = RuntimeDataVariables.New("isFiring", 0f, writeThrough: false);
		Modifiers = RuntimeDataVariables.New("modifiers", 1f, writeThrough: false);
		VehiclePickupOwner vehiclePickupOwner = gameObject.AddComponent<VehiclePickupOwner>();
		MVPickupMountPoint componentInChildren = gameObject.GetComponentInChildren<MVPickupMountPoint>();
		vehiclePickupOwner.Init(CurrentItem, IsFiring, componentInChildren.transform);
	}

	public override void Select(Color color)
	{
		AddSelectionBox();
	}

	public override void DeSelect()
	{
		RemoveSelectionBox();
	}

	public override bool OnEnterObject(EditorStateMachine e)
	{
		return editableCubeModelWrapper.OnEnterObject(e);
	}

	public override bool OnExitObject(EditorStateMachine e)
	{
		return editableCubeModelWrapper.OnExitObject(e);
	}
}
