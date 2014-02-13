using UnityEngine;

public class MVCameraBase : MonoBehaviour
{
	public float cameraRadius = 0.3f;

	protected IgnoreInputTypes ignoreInputTypes;

	public virtual Vector3 FireDirection
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return ((Component)this).transform.forward;
		}
	}

	public virtual Vector3 FireOrigin
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			return ((Component)this).transform.position + ((Component)this).transform.forward * cameraRadius;
		}
	}

	public virtual void HandleInput(MVCameraController camController)
	{
	}

	private void camController_onIgnoreInputTypes(object sender, OnIgnoreInputTypesArgs e)
	{
		ignoreInputTypes = e.inputTypes;
	}

	public virtual void UpdateCamera(MVCameraController camController, Transform targetTransform)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		targetTransform.position = ((Component)this).transform.position;
		targetTransform.rotation = ((Component)this).transform.rotation;
	}

	public virtual void Init(MVCameraController camController)
	{
		camController.onIgnoreInputTypes += camController_onIgnoreInputTypes;
	}

	public virtual void Enter(MVCameraController camController)
	{
	}

	public virtual void Exit(MVCameraController camController)
	{
	}

	public virtual void Respawn()
	{
	}

	public virtual void FocusOnObject(MVWorldObjectClient wo)
	{
	}
}
