using UnityEngine;

internal class CollisionListener : MonoBehaviour
{
	private Rigidbody body;

	private bool wasKinematic;

	private bool hadBoxCollider = true;

	private bool hadRigidBody = true;

	private void Awake()
	{
		if ((Object)(object)((Component)this).gameObject.GetComponent<BoxCollider>() == (Object)null)
		{
			((Component)this).gameObject.AddComponent<BoxCollider>();
			hadBoxCollider = false;
		}
		body = ((Component)this).gameObject.GetComponent<Rigidbody>();
		if ((Object)(object)body == (Object)null)
		{
			body = ((Component)this).gameObject.AddComponent<Rigidbody>();
			hadRigidBody = false;
		}
		else
		{
			wasKinematic = body.isKinematic;
		}
		body.isKinematic = true;
	}

	private void OnDrawGizmos()
	{
	}

	public bool SweepTest(Vector3 from, Vector3 to, out RaycastHit hit)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = ((Component)this).transform.position;
		((Component)this).transform.position = from;
		bool result = false;
		Rigidbody val = body;
		Vector3 val2 = to - from;
		Vector3 val3 = to - from;
		if (val.SweepTest(val2, ref hit, val3.magnitude))
		{
			result = true;
		}
		((Component)this).transform.position = position;
		return result;
	}

	private void OnDestroy()
	{
		if (!hadRigidBody)
		{
			Object.Destroy((Object)(object)((Component)this).gameObject.GetComponent<Rigidbody>());
		}
		else
		{
			((Component)this).gameObject.GetComponent<Rigidbody>().isKinematic = wasKinematic;
		}
		if (!hadBoxCollider)
		{
			Object.Destroy((Object)(object)((Component)this).gameObject.GetComponent<BoxCollider>());
		}
	}
}
