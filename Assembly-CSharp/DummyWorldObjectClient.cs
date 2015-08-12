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
		return GameObject.transform.position;
	}
}
