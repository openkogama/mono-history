using System;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class SphereVolumeIndicator : MonoBehaviour
{
	public float radius = 5f;

	public float arcDistance = 0.1f;

	private ParticleSystem.Particle[] particles;

	[SerializeField]
	private ParticleSystem pSystem;

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
		pSystem.loop = false;
		pSystem.playOnAwake = false;
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
		int num = (int)(2f * radius * (float)Math.PI / arcDistance);
		particles = new ParticleSystem.Particle[num * 3];
		float num2 = (float)Math.PI * 2f / (float)num;
		for (int i = 0; i < 3 * num; i++)
		{
			particles[i].lifetime = UnityEngine.Random.Range(1f, 2f);
			particles[i].velocity = Vector3.zero;
			particles[i].startLifetime = 2f;
			particles[i].startSize = 0.25f;
			particles[i].rotation = 0f;
			particles[i].startColor = Color.white;
			particles[i].angularVelocity = 0f;
			particles[i].randomSeed = 0u;
		}
		for (int j = 0; j < num; j++)
		{
			float f = (float)j * num2;
			float num3 = Mathf.Sin(f) * radius;
			float num4 = Mathf.Cos(f) * radius;
			particles[j].position = new Vector3(num4, num3, 0f);
			particles[num + j].position = new Vector3(num4, 0f, num3);
			particles[2 * num + j].position = new Vector3(0f, num4, num3);
		}
		pSystem.SetParticles(particles, particles.Length);
	}
}
