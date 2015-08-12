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

	private void Start()
	{
		rayRenderer = GetComponent<LineRenderer>();
		particles = GetComponentInChildren<ParticleSystem>();
		StartCoroutine(DoShowRay(target));
	}

	private IEnumerator DoShowRay(Vector3 hit)
	{
		Color endColor = new Color(0.1f, 0.1f, 0.1f, 0f);
		Vector3 origin = transform.position;
		transform.position = target;
		rayRenderer.SetVertexCount(2);
		rayRenderer.SetPosition(1, origin);
		rayRenderer.SetPosition(0, hit);
		float time = 1.2f;
		float t = 0f;
		while (t < time)
		{
			rayRenderer.material.SetColor("_TintColor", Color.Lerp(startColor, endColor, t / time));
			t += Time.deltaTime;
			yield return 0;
		}
		while (particles.isPlaying)
		{
			yield return 0;
		}
		Object.Destroy(gameObject);
	}
}
