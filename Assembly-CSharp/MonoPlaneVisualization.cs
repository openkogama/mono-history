using UnityEngine;

public class MonoPlaneVisualization : VehicleVisualizationBase
{
	private RollVisualization rollVisualization;

	public Transform visualizationRoot;

	private float pitchMax = 87f;

	private float pitchSpeedTime = 10f;

	private float pitchFactor = 2.5f;

	private float smoothPitchFactor;

	public SimpleVehicleHealthChangeVisualization simpleVehicleHealthChangeVisualization;

	private Vector3 prevPos = Vector3.zero;

	public void Init(Transform hull, VehicleSeatManager vehicleSeatManager, float maxHealth, MVRuntimeDataVariableClampedFloat health, bool isInSpawner)
	{
		VehicleVisualizationBase.ParentHullTransformToVisualizationRoot(hull, visualizationRoot);
		rollVisualization = new RollVisualization();
		simpleVehicleHealthChangeVisualization.Init(hull, maxHealth, health);
		prevPos = transform.position;
	}

	private void FixedUpdate()
	{
		JetPackPitch();
		if (rollVisualization != null)
		{
			rollVisualization.AnimateHullInertia(transform, visualizationRoot);
		}
	}

	private void JetPackPitch()
	{
		float num = Vector3.Angle((transform.position - prevPos).normalized, Vector3.up) - 90f;
		smoothPitchFactor = Mathf.SmoothStep(smoothPitchFactor, num * pitchFactor, Time.deltaTime * pitchSpeedTime);
		visualizationRoot.localRotation = Quaternion.AngleAxis(Mathf.Clamp(smoothPitchFactor, 0f - pitchMax, pitchMax), Vector3.right);
		prevPos = transform.position;
	}
}
