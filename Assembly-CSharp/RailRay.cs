using System.Collections;
using UnityEngine;

public class RailRay : MonoBehaviour
{
	public Vector3 target;

	public float radius;

	public Color startColor;

	public AnimationCurve implosionFade;

	private LineRenderer rayRenderer;

	private ParticleSystem particles;

	public LineRenderer RayRenderer
	{
		get
		{
			if (rayRenderer == null)
			{
				rayRenderer = GetComponent<LineRenderer>();
			}
			return rayRenderer;
		}
	}

	public ParticleSystem Particles
	{
		get
		{
			if (particles == null)
			{
				particles = GetComponentInChildren<ParticleSystem>();
			}
			return particles;
		}
	}

	private void Start()
	{
		StartCoroutine(DoShowRay(target));
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
		Object.Destroy(gameObject);
	}
}
