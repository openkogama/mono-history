using System;
using UnityEngine;

public class MVCameraBase : MonoBehaviour
{
	public float cameraRadius = 0.3f;

	protected IgnoreInputTypes ignoreInputTypes;

	public virtual Vector3 FireDirection => transform.forward;

	public virtual Vector3 FireOrigin => transform.position + transform.forward * cameraRadius;

	public virtual CameraType CameraType
	{
		get
		{
			throw new Exception("CameraType not implemented");
		}
	}

	public virtual void HandleInput(MVCameraController camController)
	{
	}

	public void camController_onIgnoreInputTypes(object sender, OnIgnoreInputTypesArgs e)
	{
		ignoreInputTypes = e.inputTypes;
	}

	public virtual void UpdateCamera(MVCameraController camController, Transform targetTransform)
	{
		targetTransform.position = transform.position;
		targetTransform.rotation = transform.rotation;
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
