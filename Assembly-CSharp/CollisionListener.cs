using UnityEngine;

internal class CollisionListener : MonoBehaviour
{
	private Rigidbody body;

	private bool wasKinematic;

	private bool hadBoxCollider = true;

	private bool hadRigidBody = true;

	private void Awake()
	{
		if (gameObject.GetComponent<BoxCollider>() == null)
		{
			gameObject.AddComponent<BoxCollider>();
			hadBoxCollider = false;
		}
		body = gameObject.GetComponent<Rigidbody>();
		if (body == null)
		{
			body = gameObject.AddComponent<Rigidbody>();
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
		Vector3 position = transform.position;
		transform.position = from;
		bool result = false;
		if (body.SweepTest(to - from, out hit, (to - from).magnitude))
		{
			result = true;
		}
		transform.position = position;
		return result;
	}

	private void OnDestroy()
	{
		if (!hadRigidBody)
		{
			Object.Destroy(gameObject.GetComponent<Rigidbody>());
		}
		else
		{
			gameObject.GetComponent<Rigidbody>().isKinematic = wasKinematic;
		}
		if (!hadBoxCollider)
		{
			Object.Destroy(gameObject.GetComponent<BoxCollider>());
		}
	}
}
