using System;
using System.Collections.Generic;
using UnityEngine;

public class JetPackVisualization : VehicleVisualizationBase
{
	private Vector3 prevWorldPosition;

	private float smoothMoveSpeed;

	private float smoothPitchFactor;

	private float smoothRollFactor;

	private float pitchMax = 60f;

	private float pitchSpeedTime = 15f;

	private float pitchFactor = 2.5f;

	private float smoothMoveSpeedTime = 10f;

	private Vector3 posDiff = Vector3.zero;

	private float originalMaxEmission;

	private float originalMaxSize;

	public AudioSource moving;

	public VehicleBlinker vehicleBlinker;

	public Transform JetPackRoot;

	public List<ParticleEmitter> thrusters = new List<ParticleEmitter>();

	private MVJetPack.JetModeType mode = MVJetPack.JetModeType.NotSet;

	private bool modeChanged;

	public JetPackVisualization()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
	}

	public void Init(bool isInSpawner, Transform jetPackCubeModel, MVRuntimeDataVariable jetMode)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		Vector3 localPosition = jetPackCubeModel.localPosition;
		Quaternion localRotation = jetPackCubeModel.localRotation;
		jetPackCubeModel.parent = JetPackRoot;
		jetPackCubeModel.localPosition = localPosition;
		jetPackCubeModel.localRotation = localRotation;
		jetMode.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(jetMode.OnChange, (MVRuntimeDataVariable.OnChangeDelegate)((object jetModeVal) =>
		{
			OnJetModeChange((MVJetPack.JetModeType)(byte)jetModeVal);
		}));
		base.isInSpawner = isInSpawner;
		if (isInSpawner)
		{
			((Behaviour)this).enabled = false;
		}
		else
		{
			foreach (ParticleEmitter thruster in thrusters)
			{
				originalMaxSize = thruster.maxSize;
				originalMaxEmission = thruster.maxEmission;
			}
			OnJetModeChange((MVJetPack.JetModeType)(byte)jetMode.Value);
			((Behaviour)moving).enabled = true;
			vehicleBlinker.Init(((Component)JetPackRoot).gameObject.GetComponentsInChildren<MeshFilter>());
			vehicleBlinker.Visible = true;
		}
		prevWorldPosition = ((Component)this).transform.position;
	}

	public void OnJetModeChange(MVJetPack.JetModeType newMode)
	{
		if (mode != newMode)
		{
			if (mode == MVJetPack.JetModeType.Overheating)
			{
				SetMaxSizeForThrusters(originalMaxSize, originalMaxEmission);
			}
			switch (newMode)
			{
			case MVJetPack.JetModeType.On:
				EnableThruster(enable: true);
				break;
			case MVJetPack.JetModeType.Off:
				EnableThruster(enable: false);
				break;
			case MVJetPack.JetModeType.Overheating:
				SetMaxSizeForThrusters(3f, 10f);
				break;
			}
		}
		mode = newMode;
	}

	public void EnableThruster(bool enable)
	{
		foreach (ParticleEmitter thruster in thrusters)
		{
			thruster.emit = enable;
		}
		if (enable)
		{
			moving.volume = 1f;
		}
		else
		{
			moving.volume = 0f;
		}
	}

	public void DoOverheatBlinking()
	{
		vehicleBlinker.StartBlinking(BlinkType.Damage, 0.3f);
	}

	private void Start()
	{
	}

	private void Update()
	{
		HandleJetMode();
	}

	private void HandleJetMode()
	{
		if (modeChanged || mode == MVJetPack.JetModeType.Overheating)
		{
			modeChanged = false;
		}
	}

	private void SetMaxSizeForThrusters(float maxSize, float maxEmission)
	{
		foreach (ParticleEmitter thruster in thrusters)
		{
			thruster.maxSize = maxSize;
			thruster.maxEmission = maxEmission;
		}
	}

	private void FixedUpdate()
	{
		UpdateSpartialValues();
		JetPackPitch();
		JetPackRoll();
	}

	private void UpdateSpartialValues()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = ((Component)this).transform.position;
		posDiff = position - prevWorldPosition;
		prevWorldPosition = position;
		float num = posDiff.magnitude / Time.deltaTime;
		smoothMoveSpeed = Mathf.SmoothStep(smoothMoveSpeed, num, Time.deltaTime * smoothMoveSpeedTime);
	}

	private void JetPackPitch()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((Component)this).transform.rotation * Vector3.forward;
		float num = Vector3.Dot(val.normalized, posDiff.normalized);
		smoothPitchFactor = Mathf.SmoothStep(smoothPitchFactor, smoothMoveSpeed * num * pitchFactor, Time.deltaTime * pitchSpeedTime);
		JetPackRoot.localRotation = Quaternion.AngleAxis(Mathf.Clamp(smoothPitchFactor, 0f - pitchMax, pitchMax), Vector3.right);
	}

	private void JetPackRoll()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((Component)this).transform.rotation * Vector3.right;
		float num = Vector3.Dot(val.normalized, posDiff.normalized);
		smoothRollFactor = Mathf.SmoothStep(smoothRollFactor, smoothMoveSpeed * (0f - num) * pitchFactor, Time.deltaTime * pitchSpeedTime);
		Transform jetPackRoot = JetPackRoot;
		jetPackRoot.localRotation *= Quaternion.AngleAxis(Mathf.Clamp(smoothRollFactor, 0f - pitchMax, pitchMax), Vector3.forward);
	}
}
