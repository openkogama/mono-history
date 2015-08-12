using UnityEngine;

internal class BoxCast : MonoBehaviour
{
	private Rigidbody body;

	private BoxCollider boxCollider;

	private void Awake()
	{
	}

	public void Init(GameObject boxCastObject)
	{
		transform.rotation = boxCastObject.transform.rotation;
		transform.localScale = boxCastObject.transform.localScale;
		boxCollider = gameObject.AddComponent<BoxCollider>();
		Bounds bounds = boxCastObject.GetComponent<MeshFilter>().sharedMesh.bounds;
		boxCollider.size = bounds.size;
		boxCollider.center = bounds.center;
		body = gameObject.AddComponent<Rigidbody>();
		body.isKinematic = true;
	}

	private void OnDrawGizmos()
	{
	}

	public bool SweepTest(out RaycastHit hit)
	{
		Ray ray = Camera.main.ScreenPointToRay(MVInputWrapper.GetPointerPosition());
		transform.position = Camera.main.transform.position;
		return body.SweepTest(ray.direction, out hit);
	}

	public bool SweepTest(out RaycastHit hit, ref Vector3 offset)
	{
		Ray ray = Camera.main.ScreenPointToRay(MVInputWrapper.GetPointerPosition());
		transform.position = Camera.main.transform.position;
		if (body.SweepTest(ray.direction, out hit))
		{
			Ray ray2 = new Ray(hit.point, -ray.direction);
			if (boxCollider.Raycast(ray2, out var hitInfo, (Camera.main.transform.position - hit.point).magnitude * 2f))
			{
				offset = transform.position - hitInfo.point;
				return true;
			}
		}
		return false;
	}

	public bool SweepTest(Vector3 from, Vector3 to, out RaycastHit hit)
	{
		transform.position = from;
		bool result = false;
		if (body.SweepTest(to - from, out hit, (to - from).magnitude))
		{
			result = true;
		}
		return result;
	}
}
