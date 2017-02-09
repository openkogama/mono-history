using UnityEngine;

public class ImpulseRay : MonoBehaviour
{
	public float radius;

	public Color startColor;

	private static readonly string tintColor = "_TintColor";

	[SerializeField]
	private MeshRenderer rayRenderer;

	private readonly Color endColor = new Color(0.1f, 0.1f, 0.1f, 0f);

	private readonly float time = 0.4f;

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
		transform.localScale = new Vector3(radius, radius, rayMagnitude * t / time);
		RayRenderer.material.SetColor(tintColor, Color.Lerp(startColor, endColor, t / time));
		t += Time.deltaTime;
		if (t >= time)
		{
			t = 0f;
			PrefabPool.Instance.EnumPoolManager.Return(this, PoolEnums.ImpulseGunRay);
		}
	}
}
