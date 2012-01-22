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
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.rotation = boxCastObject.transform.rotation;
		((Component)this).transform.localScale = boxCastObject.transform.localScale;
		boxCollider = ((Component)this).gameObject.AddComponent<BoxCollider>();
		Bounds bounds = boxCastObject.GetComponent<MeshFilter>().sharedMesh.bounds;
		boxCollider.size = bounds.size;
		boxCollider.center = bounds.center;
		body = ((Component)this).gameObject.AddComponent<Rigidbody>();
		body.isKinematic = true;
	}

	private void OnDrawGizmos()
	{
	}

	public bool SweepTest(out RaycastHit hit)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		Ray val = Camera.main.ScreenPointToRay(Input.mousePosition);
		((Component)this).transform.position = ((Component)Camera.main).transform.position;
		return body.SweepTest(val.direction, ref hit);
	}

	public bool SweepTest(out RaycastHit hit, ref Vector3 offset)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		Ray val = Camera.main.ScreenPointToRay(Input.mousePosition);
		((Component)this).transform.position = ((Component)Camera.main).transform.position;
		if (body.SweepTest(val.direction, ref hit))
		{
			Ray val2 = new Ray(hit.point, -val.direction);
			BoxCollider val3 = boxCollider;
			Vector3 val4 = ((Component)Camera.main).transform.position - hit.point;
			RaycastHit val5 = default;
			if (((Collider)val3).Raycast(val2, ref val5, val4.magnitude * 2f))
			{
				offset = ((Component)this).transform.position - val5.point;
				return true;
			}
		}
		return false;
	}

	public bool SweepTest(Vector3 from, Vector3 to, out RaycastHit hit)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.position = from;
		bool result = false;
		Rigidbody val = body;
		Vector3 val2 = to - from;
		Vector3 val3 = to - from;
		if (val.SweepTest(val2, ref hit, val3.magnitude))
		{
			result = true;
		}
		return result;
	}
}
