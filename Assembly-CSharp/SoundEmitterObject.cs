using UnityEngine;

public class SoundEmitterObject : ObjectPrefab
{
	[SerializeField]
	private GameObject visualObject;

	public GameObject VisualObject => visualObject;
}
