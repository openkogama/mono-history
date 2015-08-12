using System;
using UnityEngine;

public abstract class SimpleVehicleMotorBase : MVRigidBody
{
	protected MVMovableMotorState movableMotorState;

	protected MVInteractableBase interactableLocal;

	protected SmoothCharacterController smoothController;

	protected StuckEvaluator stuckEvaluator;

	public Vector3 DirectInputMoveMap;

	public bool Jump;

	public bool HandleInput;

	protected MvCharacterController Controller => smoothController.Controller;

	public virtual void Init(SmoothCharacterController smoothController, VehicleInteractable interactableLocal)
	{
		Init();
		this.smoothController = smoothController;
		MvCharacterController controller = Controller;
		controller.OnControllerColliderHit = (Action<MVControllerColliderHit>)Delegate.Combine(controller.OnControllerColliderHit, new Action<MVControllerColliderHit>(interactableLocal.HandleMoveHit));
		movableMotorState = new MVMovableMotorState();
		stuckEvaluator = new StuckEvaluator(Controller.GetOverlappingObjects);
		this.interactableLocal = interactableLocal;
	}

	public virtual void OnLocalVehicleLeave()
	{
		DirectInputMoveMap = Vector3.zero;
	}

	public bool IsStuck()
	{
		return stuckEvaluator.Update();
	}

	public abstract void VehicleUpdateFunction();

	public override void Reset()
	{
		base.Reset();
		smoothController.Reset();
	}

	public void UpdateFunction()
	{
		smoothController.SmoothMove();
	}
}
