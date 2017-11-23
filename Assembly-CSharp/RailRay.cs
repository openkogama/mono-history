using System.Collections;
using UnityEngine;

public class RailRay : MonoBehaviour
{
	public Vector3 target;

	public float radius;

	public Color startColor;

	public AnimationCurve implosionFade;

	[SerializeField]
	private LineRenderer rayRenderer;

	[SerializeField]
	private ParticleSystem particles;

	[SerializeField]
	private PoolEnums railEnumType;

	private Vector3 origin;

	private Vector3 hit;

	private Color endColor = new Color(0.1f, 0.1f, 0.1f, 0f);

	private float time = 1.2f;

	private float elapsed;

	public LineRenderer RayRenderer => rayRenderer;

	public ParticleSystem Particles
	{
		get
		{
			return particles;
		}
		set
		{
			particles = value;
		}
	}

	private void Awake()
	{
		hit = target;
		elapsed = 0f;
		RayRenderer.SetVertexCount(2);
		RayRenderer.SetPosition(1, transform.localPosition);
		RayRenderer.SetPosition(0, hit);
		RayRenderer.material.SetColor("_TintColor", startColor);
		transform.localPosition = target;
	}

	public void Reset()
	{
		hit = target;
		elapsed = 0f;
		RayRenderer.SetVertexCount(2);
		RayRenderer.SetPosition(1, transform.localPosition);
		RayRenderer.SetPosition(0, hit);
		RayRenderer.material.SetColor("_TintColor", startColor);
		transform.localPosition = target;
	}

	private void Update()
	{
		if (elapsed < time)
		{
			RayRenderer.material.SetColor("_TintColor", Color.Lerp(startColor, endColor, elapsed / time));
			elapsed += Time.deltaTime;
		}
		else if (!Particles.isPlaying)
		{
			PrefabPool.Instance.EnumPoolManager.Return(this, railEnumType);
		}
	}

	private IEnumerator DoShowRay(Vector3 hit)
	{
		Color endColor = new Color(0.1f, 0.1f, 0.1f, 0f);
		Vector3 origin = transform.position;
		transform.position = target;
		RayRenderer.SetVertexCount(2);
		RayRenderer.SetPosition(1, origin);
		RayRenderer.SetPosition(0, hit);
		float time = 1.2f;
		float t = 0f;
		while (t < time)
		{
			RayRenderer.material.SetColor("_TintColor", Color.Lerp(startColor, endColor, t / time));
			t += Time.deltaTime;
			yield return 0;
		}
		while (Particles.isPlaying)
		{
			yield return 0;
		}
		PrefabPool.Instance.EnumPoolManager.Return(this, railEnumType);
	}
}
