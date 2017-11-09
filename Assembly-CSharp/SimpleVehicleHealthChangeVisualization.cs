using System;
using UnityEngine;

public class SimpleVehicleHealthChangeVisualization : MonoBehaviour
{
	private float prevHealth;

	private float maxHealth;

	public ParticleSystem fire;

	public VehicleBlinker vehicleBlinker;

	public ParticleEmitter ellipsoidParticleEmitter;

	public float damageParticleFactor = 10f;

	public void Init(Transform hull, float maxHealth, MVRuntimeDataVariableClampedFloat health)
	{
		health.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(health.OnChange, (MVRuntimeDataVariable.OnChangeDelegate)((object healthVal) =>
		{
			OnHealthChange((float)healthVal);
		}));
		prevHealth = health.Value;
		this.maxHealth = maxHealth;
		vehicleBlinker.Init(hull.gameObject.GetComponentsInChildren<MeshFilter>());
		vehicleBlinker.Visible = true;
	}

	private void OnHealthChange(float newHealth)
	{
		if (newHealth < maxHealth && !ellipsoidParticleEmitter.emit)
		{
			ellipsoidParticleEmitter.emit = true;
			ParticleSystem.EmissionModule emission = fire.emission;
			emission.enabled = true;
		}
		if (newHealth == maxHealth && ellipsoidParticleEmitter.emit)
		{
			ellipsoidParticleEmitter.emit = false;
			ParticleSystem.EmissionModule emission2 = fire.emission;
			emission2.enabled = false;
			return;
		}
		if (prevHealth > newHealth)
		{
			vehicleBlinker.StartBlinking(BlinkType.Damage, 0.3f);
		}
		float num = (1f - newHealth / maxHealth) * damageParticleFactor;
		ellipsoidParticleEmitter.minSize = num;
		ellipsoidParticleEmitter.maxSize = num;
		fire.startSize = num * 0.3f;
		prevHealth = newHealth;
	}
}
