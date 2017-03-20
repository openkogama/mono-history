using UnityEngine;

public abstract class MVCameraBase : MonoBehaviour
{
	private CameraImpact cameraImpact;

	protected IgnoreInputTypes ignoreInputTypes;

	public float cameraRadius = 0.3f;

	protected bool InputActive => !MVGameControllerBase.IPlayModeUI.InLobbyState && (ignoreInputTypes & IgnoreInputTypes.MouseMovement) == 0;

	public abstract CameraType CameraType { get; }

	public virtual float FieldOfView => 70f;

	public virtual void Awake()
	{
	}

	public void camController_onIgnoreInputTypes(object sender, OnIgnoreInputTypesArgs e)
	{
		ignoreInputTypes = e.inputTypes;
	}

	public virtual void UpdateCamera(MVCameraController camController, ProtectedTransform targetTransform)
	{
		targetTransform.position = transform.position;
		targetTransform.rotation = transform.rotation;
	}

	protected void UpdateImpactSimulation(ProtectedTransform targetTransform)
	{
		if (cameraImpact == null)
		{
		}
	}

	public void SimulateImpact(Vector3 impactDirection, AnimationCurve impactCurve, float forceMultiplier = 1f, Space impactSpace = Space.World)
	{
		cameraImpact = new CameraImpact(impactDirection, impactCurve, forceMultiplier, impactSpace);
	}

	private void SimulateImpact(Transform targetTransform)
	{
		SimulateImpact(targetTransform, cameraImpact.impactDirection, cameraImpact.impactCurve, cameraImpact.forceMultiplier, cameraImpact.impactSpace);
	}

	private void SimulateImpact(Transform targetTransform, Vector3 impactDirection, AnimationCurve impactCurve, float forceMultiplier, Space impactSpace = Space.World)
	{
		float num = impactCurve.Evaluate(cameraImpact.time) * forceMultiplier;
		Vector3 rhs = ((impactSpace != Space.World) ? transform.up : Vector3.up);
		Vector3 vector = Vector3.Cross(-impactDirection, rhs);
		targetTransform.Translate(impactDirection * num * 4f, impactSpace);
		if (vector != Vector3.zero)
		{
			targetTransform.Rotate(vector, num * 90f, impactSpace);
		}
		cameraImpact.time += Time.deltaTime;
		if (cameraImpact.time > impactCurve.keys[impactCurve.length - 1].time)
		{
			cameraImpact = null;
		}
	}

	public virtual void Enter(MVCameraController camController)
	{
	}

	public virtual void Exit(MVCameraController camController)
	{
	}

	public virtual void Suspend(MVCameraController camController)
	{
	}

	public virtual void Resume(MVCameraController camController)
	{
	}

	public virtual void Reset()
	{
	}

	public virtual void FocusOnObject(MVWorldObjectClient wo)
	{
	}
}
