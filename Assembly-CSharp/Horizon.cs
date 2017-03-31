using UnityEngine;

public class Horizon : MonoBehaviour
{
	[SerializeField]
	private MeshRenderer meshRenderer;

	private Transform parent;

	private Vector3 offset;

	protected void Awake()
	{
		offset = transform.localPosition;
		parent = transform.parent;
		transform.SetParent(null, worldPositionStays: false);
	}

	protected void Update()
	{
		transform.localPosition = parent.position + offset;
	}
}
