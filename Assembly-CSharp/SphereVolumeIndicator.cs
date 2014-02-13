using System;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class SphereVolumeIndicator : MonoBehaviour
{
	public float radius = 5f;

	public float arcDistance = 0.1f;

	private Particle[] particles;

	public float Radius
	{
		get
		{
			return radius;
		}
		set
		{
			radius = value;
			SetupParticles(radius, arcDistance);
		}
	}

	private void Awake()
	{
		((Component)this).particleSystem.loop = false;
		((Component)this).particleSystem.playOnAwake = false;
	}

	private void Start()
	{
		if (particles == null)
		{
			SetupParticles(radius, arcDistance);
		}
	}

	private void SetupParticles(float radius, float arcDistance)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		int num = (int)(2f * radius * (float)Math.PI / arcDistance);
		particles = new Particle[num * 3];
		float num2 = (float)Math.PI * 2f / (float)num;
		for (int i = 0; i < 3 * num; i++)
		{
			particles[i].lifetime = Random.Range(1f, 2f);
			particles[i].velocity = Vector3.zero;
			particles[i].startLifetime = 2f;
			particles[i].size = 0.25f;
			particles[i].rotation = 0f;
			particles[i].color = Color32.op_Implicit(Color.white);
			particles[i].angularVelocity = 0f;
			particles[i].randomValue = 0f;
		}
		for (int j = 0; j < num; j++)
		{
			float num3 = (float)j * num2;
			float num4 = Mathf.Sin(num3) * radius;
			float num5 = Mathf.Cos(num3) * radius;
			particles[j].position = new Vector3(num5, num4, 0f);
			particles[num + j].position = new Vector3(num5, 0f, num4);
			particles[2 * num + j].position = new Vector3(0f, num5, num4);
		}
		((Component)this).particleSystem.SetParticles(particles, particles.Length);
	}
}
