using UnityEngine;

public class TimedObjectDestructor : MonoBehaviour
{
	[SerializeField]
	private float timeOut = 1f;

	[SerializeField]
	private bool detachChildren;

	private void Awake()
	{
		Invoke("DestroyNow", timeOut);
	}

	private void DestroyNow()
	{
		if (detachChildren)
		{
			transform.DetachChildren();
		}
		Object.Destroy(gameObject);
	}
}
