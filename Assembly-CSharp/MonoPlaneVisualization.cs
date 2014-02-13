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

	public MonoPlaneVisualization()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
	}

	public void Init(Transform hull, VehicleSeatManager vehicleSeatManager, float maxHealth, MVRuntimeDataVariableClampedFloat health, bool isInSpawner)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		VehicleVisualizationBase.ParentHullTransformToVisualizationRoot(hull, visualizationRoot);
		rollVisualization = new RollVisualization();
		simpleVehicleHealthChangeVisualization.Init(hull, maxHealth, health);
		prevPos = ((Component)this).transform.position;
	}

	private void FixedUpdate()
	{
		JetPackPitch();
		if (rollVisualization != null)
		{
			rollVisualization.AnimateHullInertia(((Component)this).transform, visualizationRoot);
		}
	}

	private void JetPackPitch()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((Component)this).transform.position - prevPos;
		float num = Vector3.Angle(val.normalized, Vector3.up) - 90f;
		smoothPitchFactor = Mathf.SmoothStep(smoothPitchFactor, num * pitchFactor, Time.deltaTime * pitchSpeedTime);
		visualizationRoot.localRotation = Quaternion.AngleAxis(Mathf.Clamp(smoothPitchFactor, 0f - pitchMax, pitchMax), Vector3.right);
		prevPos = ((Component)this).transform.position;
	}
}
