using System.Collections;
using UnityEngine;

public class ImpulseRay : MonoBehaviour
{
	public Vector3 target;

	public float radius;

	public Color startColor;

	private MeshRenderer rayRenderer;

	public MeshRenderer RayRenderer
	{
		get
		{
			if (rayRenderer == null)
			{
				rayRenderer = GetComponentInChildren<MeshRenderer>();
			}
			return rayRenderer;
		}
	}

	private void Start()
	{
		StartCoroutine(DoShowRay(target));
	}

	private IEnumerator DoShowRay(Vector3 hit)
	{
		Vector3 up = Vector3.up;
		Color endColor = new Color(0.1f, 0.1f, 0.1f, 0f);
		float time = 0.4f;
		float t = 0f;
		while (t < time)
		{
			Vector3 ray = hit - transform.position;
			Quaternion rayRotation = Quaternion.LookRotation(ray.normalized, up);
			transform.rotation = rayRotation;
			transform.localScale = new Vector3(radius, radius, ray.magnitude * t / time);
			RayRenderer.material.SetColor("_TintColor", Color.Lerp(startColor, endColor, t / time));
			t += Time.deltaTime;
			yield return 0;
		}
		Object.Destroy(gameObject);
	}
}
