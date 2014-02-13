using System.Collections;
using UnityEngine;

public class ImpulseRay : MonoBehaviour
{
	public Vector3 target;

	public float radius;

	public Color startColor;

	private MeshRenderer rayRenderer;

	private void Start()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		rayRenderer = ((Component)this).GetComponentInChildren<MeshRenderer>();
		((MonoBehaviour)this).StartCoroutine(DoShowRay(target));
	}

	private IEnumerator DoShowRay(Vector3 hit)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Vector3 up = Vector3.up;
		Color endColor = new Color(0.1f, 0.1f, 0.1f, 0f);
		float time = 0.4f;
		float t = 0f;
		while (t < time)
		{
			Vector3 ray = hit - ((Component)this).transform.position;
			Quaternion rayRotation = Quaternion.LookRotation(ray.normalized, up);
			((Component)this).transform.rotation = rayRotation;
			((Component)this).transform.localScale = new Vector3(radius, radius, ray.magnitude * t / time);
			((Renderer)rayRenderer).material.SetColor("_TintColor", Color.Lerp(startColor, endColor, t / time));
			t += Time.deltaTime;
			yield return 0;
		}
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}
}
