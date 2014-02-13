using UnityEngine;

public class DummyWorldObjectClient
{
	public GameObject GameObject;

	public int Id;

	public DummyWorldObjectClient(GameObject gameObject)
	{
		GameObject = gameObject;
	}

	public Vector3 GetTargetPosition()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return GameObject.transform.position;
	}
}
