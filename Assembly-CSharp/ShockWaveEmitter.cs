using UnityEngine;

public class ShockWaveEmitter : MonoBehaviour
{
	[Tooltip("Impulse by range from emission.")]
	[SerializeField]
	private AnimationCurve strengthByDistance;

	[SerializeField]
	[Tooltip("Rotation and translation over time [0,1]")]
	private AnimationCurve cameraKnockbackCurve;

	private float MaxImpulse => strengthByDistance.keys[0].value;

	public void Trigger()
	{
		TriggerFrom(transform.position);
	}

	public void TriggerFrom(Vector3 point, float sizeMultiplier = 1f)
	{
		foreach (int localControlledWorldObject in MVGameControllerBase.Game.PlayerController.LocalControlledWorldObjects)
		{
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(localControlledWorldObject);
			MVRigidBody component = worldObjectClient.GameObject.GetComponent<MVRigidBody>();
			if (!(component != null))
			{
				continue;
			}
			Vector3 vector = (worldObjectClient.Transform.position - point) / sizeMultiplier;
			float num = strengthByDistance.Evaluate(vector.magnitude) * sizeMultiplier;
			if (num > 0.05f)
			{
				vector.y = Mathf.Sqrt(vector.x * vector.x + vector.z * vector.z);
				vector.y /= 2f;
				component.AddImpulse(vector.normalized * num);
				if (worldObjectClient.Id == MVGameControllerBase.WOCM.AvatarLocal.Id)
				{
					MVGameControllerBase.CameraController.CurCamera.SimulateImpact(vector.normalized, cameraKnockbackCurve, num / MaxImpulse);
				}
			}
		}
	}
}
