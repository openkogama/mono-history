using UnityEngine;

public class ImpulseRay : MonoBehaviour
{
	[SerializeField]
	private MeshRenderer rayRenderer;

	public float radius;

	public Color startColor;

	private const string tintColor = "_TintColor";

	private const float time = 0.4f;

	private readonly Color endColor = new Color(0.1f, 0.1f, 0.1f, 0f);

	private float t;

	private float rayMagnitude;

	public MeshRenderer RayRenderer => rayRenderer;

	public void Initialize(Vector3 target)
	{
		Vector3 vector = target - transform.position;
		Quaternion rotation = Quaternion.LookRotation(vector.normalized, Vector3.up);
		transform.rotation = rotation;
		rayMagnitude = vector.magnitude;
	}

	private void Update()
	{
		transform.localScale = new Vector3(radius, radius, rayMagnitude * t / 0.4f);
		RayRenderer.material.SetColor("_TintColor", Color.Lerp(startColor, endColor, t / 0.4f));
		t += Time.deltaTime;
		if (t >= 0.4f)
		{
			t = 0f;
			PrefabPool.Instance.EnumPoolManager.Return(this, PoolEnums.ImpulseGunRay);
		}
	}
}
