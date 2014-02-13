using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class SimpleVehicleMotorBase : MVRigidBody
{
	protected MVMovableMotorState movableMotorState;

	protected MVInteractableBase interactableLocal;

	protected MvCharacterController controller;

	protected StuckEvaluator stuckEvaluator;

	protected List<MVControllerColliderHit> moveHits = new List<MVControllerColliderHit>();

	public Vector3 DirectInputMoveMap;

	public bool Jump;

	public bool HandleInput;

	public virtual void Init(MvCharacterController characterController, MVInteractableBase interactableLocal)
	{
		characterController.OnControllerColliderHit = (MvCharacterController.OnControllerColliderHitDelegate)Delegate.Combine(characterController.OnControllerColliderHit, new MvCharacterController.OnControllerColliderHitDelegate(OnControllerColliderHit));
		movableMotorState = new MVMovableMotorState();
		stuckEvaluator = new StuckEvaluator(characterController.GetOverlappingObjects);
		this.interactableLocal = interactableLocal;
		controller = characterController;
	}

	public virtual void OnLocalVehicleLeave()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		DirectInputMoveMap = Vector3.zero;
	}

	public bool IsStuck()
	{
		return stuckEvaluator.Update();
	}

	public abstract void VehicleUpdateFunction();

	private void OnControllerColliderHit(MVControllerColliderHit hit)
	{
		moveHits.Add(hit);
	}
}
