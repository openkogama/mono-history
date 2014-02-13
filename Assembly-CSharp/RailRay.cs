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
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		rayRenderer = ((Component)this).GetComponent<LineRenderer>();
		particles = ((Component)this).GetComponentInChildren<ParticleSystem>();
		((MonoBehaviour)this).StartCoroutine(DoShowRay(target));
	}

	private IEnumerator DoShowRay(Vector3 hit)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Color endColor = new Color(0.1f, 0.1f, 0.1f, 0f);
		Vector3 origin = ((Component)this).transform.position;
		((Component)this).transform.position = target;
		rayRenderer.SetVertexCount(2);
		rayRenderer.SetPosition(1, origin);
		rayRenderer.SetPosition(0, hit);
		float time = 1.2f;
		float t = 0f;
		while (t < time)
		{
			((Renderer)rayRenderer).material.SetColor("_TintColor", Color.Lerp(startColor, endColor, t / time));
			t += Time.deltaTime;
			yield return 0;
		}
		while (particles.isPlaying)
		{
			yield return 0;
		}
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}
}
