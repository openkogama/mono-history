using System;
using System.Collections.Generic;
using MV.Common;
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

	private float originalEmissionRate;

	private float originalParticleSize;

	public AudioSource moving;

	public VehicleBlinker vehicleBlinker;

	public Transform JetPackRoot;

	private float lastOverHeatNotificationTime;

	public List<ParticleSystem> thrusters = new List<ParticleSystem>();

	private MVJetPack.JetModeType mode = MVJetPack.JetModeType.NotSet;

	private bool modeChanged;

	public void Init(bool isInSpawner, Transform jetPackCubeModel, MVRuntimeDataVariable jetMode)
	{
		Vector3 localPosition = jetPackCubeModel.localPosition;
		Quaternion localRotation = jetPackCubeModel.localRotation;
		jetPackCubeModel.parent = JetPackRoot;
		jetPackCubeModel.localPosition = localPosition;
		jetPackCubeModel.localRotation = localRotation;
		jetMode.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(jetMode.OnChange, (MVRuntimeDataVariable.OnChangeDelegate)((object jetModeVal) =>
		{
			OnJetModeChange((MVJetPack.JetModeType)jetModeVal);
		}));
		base.isInSpawner = isInSpawner;
		if (isInSpawner)
		{
			enabled = false;
		}
		else
		{
			foreach (ParticleSystem thruster in thrusters)
			{
				originalParticleSize = thruster.main.startSizeMultiplier;
				originalEmissionRate = thruster.emission.rateOverTimeMultiplier;
			}
			OnJetModeChange((MVJetPack.JetModeType)jetMode.Value);
			moving.enabled = true;
			vehicleBlinker.Init(JetPackRoot.gameObject.GetComponentsInChildren<MeshFilter>());
			vehicleBlinker.Visible = true;
			enabled = true;
		}
		prevWorldPosition = transform.position;
	}

	public void OnJetModeChange(MVJetPack.JetModeType newMode)
	{
		if (mode != newMode)
		{
			if (mode == MVJetPack.JetModeType.Overheating)
			{
				SetMaxSizeForThrusters(originalParticleSize, originalEmissionRate);
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
		foreach (ParticleSystem thruster in thrusters)
		{
			if (enable)
			{
				thruster.Play();
			}
			else
			{
				thruster.Stop();
			}
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

	public void ShowOverHeatWarning()
	{
		if (Time.time - lastOverHeatNotificationTime > 2f)
		{
			lastOverHeatNotificationTime = Time.time;
			NotificationController.PushNotification(NotificationType.JetPackOverheating, NotificationsManager.eNotificationPanel.primary);
		}
	}

	private void Update()
	{
		UpdateSpartialValues();
		JetPackPitch();
		JetPackRoll();
		HandleJetMode();
	}

	private void HandleJetMode()
	{
		if (modeChanged || mode == MVJetPack.JetModeType.Overheating)
		{
			modeChanged = false;
		}
	}

	private void SetMaxSizeForThrusters(float particleSize, float emissionRate)
	{
		foreach (ParticleSystem thruster in thrusters)
		{
			ParticleSystem.MainModule main = thruster.main;
			main.startSizeMultiplier = particleSize;
			ParticleSystem.EmissionModule emission = thruster.emission;
			emission.rateOverTimeMultiplier = emissionRate;
		}
	}

	private void UpdateSpartialValues()
	{
		Vector3 position = transform.position;
		posDiff = position - prevWorldPosition;
		prevWorldPosition = position;
		float to = posDiff.magnitude / Time.deltaTime;
		smoothMoveSpeed = Mathf.SmoothStep(smoothMoveSpeed, to, Time.deltaTime * smoothMoveSpeedTime);
	}

	private void JetPackPitch()
	{
		float num = Vector3.Dot((transform.rotation * Vector3.forward).normalized, posDiff.normalized);
		smoothPitchFactor = Mathf.SmoothStep(smoothPitchFactor, smoothMoveSpeed * num * pitchFactor, Time.deltaTime * pitchSpeedTime);
		JetPackRoot.localRotation = Quaternion.AngleAxis(Mathf.Clamp(smoothPitchFactor, 0f - pitchMax, pitchMax), Vector3.right);
	}

	private void JetPackRoll()
	{
		float num = Vector3.Dot((transform.rotation * Vector3.right).normalized, posDiff.normalized);
		smoothRollFactor = Mathf.SmoothStep(smoothRollFactor, smoothMoveSpeed * (0f - num) * pitchFactor, Time.deltaTime * pitchSpeedTime);
		JetPackRoot.localRotation *= Quaternion.AngleAxis(Mathf.Clamp(smoothRollFactor, 0f - pitchMax, pitchMax), Vector3.forward);
	}
}
