using MV.Common;
using UnityEngine;

public class SentryGunScript : MonoBehaviour
{
	public MeshRenderer sentryRenderer;

	public Transform glowPlane;

	public Material materialFireBeam;

	public Material materialIceBeam;

	public AudioClip audioClipFireBeam;

	public AudioClip audioClipIceBeam;

	public Material blinkDamageMaterial;

	public GameObject smokeEffect;

	public Transform healthPivot;

	[SerializeField]
	private Mesh sentryMesh;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private ParticleEmitter smokeEffectEmitter;

	[SerializeField]
	private Renderer glowPlaneRenderer;

	private Color color;

	private SphereVolumeIndicator rangeVisualization;

	private float damageBlinkTimeoutTime;

	public bool SmokeEnabled
	{
		set
		{
			smokeEffectEmitter.emit = value;
		}
	}

	protected void Awake()
	{
		rangeVisualization = Object.Instantiate(PrefabPool.Instance.RangeVisualizationObject);
		rangeVisualization.transform.parent = transform;
		rangeVisualization.transform.localPosition = Vector3.zero;
	}

	public void Explode()
	{
		Object.Instantiate(PrefabPool.Instance.ParticleExplosion, transform.position, transform.rotation);
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
		glowPlaneRenderer.material.SetColor("_TintColor", color);
	}

	public void UpdateAnimation()
	{
		sentryRenderer.material.SetTextureOffset("_MainTex", new Vector2(Mathf.Repeat(Time.time, 1f), 0f));
	}

	public void SetLaserRange(float range)
	{
		if (MVGameControllerBase.GameMode == MVGameMode.Edit)
		{
			rangeVisualization.SetRadius(range);
		}
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
		color = sentryRenderer.sharedMaterial.GetColor("_Color");
		glowPlaneRenderer.material.SetColor("_TintColor", color);
	}

	private void SetSound(SentryGunBeamType beamType)
	{
		switch (beamType)
		{
		case SentryGunBeamType.FireBeam:
			audioSource.clip = audioClipFireBeam;
			break;
		case SentryGunBeamType.IceBeam:
			audioSource.clip = audioClipIceBeam;
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
