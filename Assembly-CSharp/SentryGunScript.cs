using UnityEngine;

public class SentryGunScript : MonoBehaviour
{
	public MeshRenderer sentryRenderer;

	public SphereVolumeIndicator rangeVisualization;

	public Transform glowPlane;

	public Material materialFireBeam;

	public Material materialIceBeam;

	public AudioClip audioClipFireBeam;

	public AudioClip audioClipIceBeam;

	public Material blinkDamageMaterial;

	public GameObject explosionEffectPrefab;

	public GameObject smokeEffect;

	private Color color;

	private Mesh sentryMesh;

	private float damageBlinkTimeoutTime;

	public Transform healthPivot;

	public bool SmokeEnabled
	{
		set
		{
			smokeEffect.GetComponent<ParticleEmitter>().emit = value;
		}
	}

	public void Initialize()
	{
		color = sentryRenderer.material.GetColor("_Color");
		glowPlane.GetComponent<Renderer>().material.SetColor("_TintColor", color);
		sentryMesh = sentryRenderer.GetComponent<MeshFilter>().mesh;
	}

	public void Explode()
	{
		Object.Instantiate(explosionEffectPrefab, transform.position, transform.rotation);
	}

	public void BlinkDamage()
	{
		damageBlinkTimeoutTime = Time.time + 0.5f;
	}

	public void SetHealth(float value)
	{
		Vector3 localScale = healthPivot.localScale;
		localScale.x = value / 300f;
		healthPivot.localScale = localScale;
	}

	public void SetGlowFactor(float glow)
	{
		sentryRenderer.material.SetFloat("_GlowFactor", glow);
		color.a = glow;
		glowPlane.GetComponent<Renderer>().material.SetColor("_TintColor", color);
	}

	public void UpdateAnimation()
	{
		sentryRenderer.material.SetTextureOffset("_MainTex", new Vector2(Mathf.Repeat(Time.time, 1f), 0f));
	}

	public void SetLaserRange(float range)
	{
		rangeVisualization.Radius = range;
	}

	public void SetSentryGunBeamType(SentryGunBeamType beamType)
	{
		SetActiveMaterial(beamType);
		SetSound(beamType);
	}

	private void SetActiveMaterial(SentryGunBeamType beamType)
	{
		switch (beamType)
		{
		case SentryGunBeamType.FireBeam:
			sentryRenderer.sharedMaterial = materialFireBeam;
			break;
		case SentryGunBeamType.IceBeam:
			sentryRenderer.sharedMaterial = materialIceBeam;
			break;
		}
	}

	private void SetSound(SentryGunBeamType beamType)
	{
		switch (beamType)
		{
		case SentryGunBeamType.FireBeam:
			gameObject.GetComponent<AudioSource>().clip = audioClipFireBeam;
			break;
		case SentryGunBeamType.IceBeam:
			gameObject.GetComponent<AudioSource>().clip = audioClipIceBeam;
			break;
		}
	}

	private void LateUpdate()
	{
		glowPlane.transform.up = -Camera.main.transform.forward;
		if (Time.time < damageBlinkTimeoutTime && Mathf.Repeat(Time.time * 4f, 1f) < 0.5f)
		{
			Graphics.DrawMesh(sentryMesh, sentryRenderer.transform.localToWorldMatrix, blinkDamageMaterial, LayerMask.NameToLayer("Default"));
		}
	}
}
