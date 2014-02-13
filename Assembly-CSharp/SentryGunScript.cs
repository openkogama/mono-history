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
			smokeEffect.particleEmitter.emit = value;
		}
	}

	public void Initialize()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		color = ((Renderer)sentryRenderer).material.GetColor("_Color");
		((Component)glowPlane).renderer.material.SetColor("_TintColor", color);
		sentryMesh = ((Component)sentryRenderer).GetComponent<MeshFilter>().mesh;
	}

	public void Explode()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		Object.Instantiate((Object)(object)explosionEffectPrefab, ((Component)this).transform.position, ((Component)this).transform.rotation);
	}

	public void BlinkDamage()
	{
		damageBlinkTimeoutTime = Time.time + 0.5f;
	}

	public void SetHealth(float value)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		Vector3 localScale = healthPivot.localScale;
		localScale.x = value / 300f;
		healthPivot.localScale = localScale;
	}

	public void SetGlowFactor(float glow)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		((Renderer)sentryRenderer).material.SetFloat("_GlowFactor", glow);
		color.a = glow;
		((Component)glowPlane).renderer.material.SetColor("_TintColor", color);
	}

	public void UpdateAnimation()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		((Renderer)sentryRenderer).material.SetTextureOffset("_MainTex", new Vector2(Mathf.Repeat(Time.time, 1f), 0f));
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
			((Renderer)sentryRenderer).sharedMaterial = materialFireBeam;
			break;
		case SentryGunBeamType.IceBeam:
			((Renderer)sentryRenderer).sharedMaterial = materialIceBeam;
			break;
		}
	}

	private void SetSound(SentryGunBeamType beamType)
	{
		switch (beamType)
		{
		case SentryGunBeamType.FireBeam:
			((Component)this).gameObject.audio.clip = audioClipFireBeam;
			break;
		case SentryGunBeamType.IceBeam:
			((Component)this).gameObject.audio.clip = audioClipIceBeam;
			break;
		}
	}

	private void LateUpdate()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		((Component)glowPlane).transform.up = -((Component)Camera.main).transform.forward;
		if (Time.time < damageBlinkTimeoutTime && Mathf.Repeat(Time.time * 4f, 1f) < 0.5f)
		{
			Graphics.DrawMesh(sentryMesh, ((Component)sentryRenderer).transform.localToWorldMatrix, blinkDamageMaterial, LayerMask.NameToLayer("Default"));
		}
	}
}
