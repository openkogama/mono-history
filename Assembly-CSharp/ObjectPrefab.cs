using UnityEngine;

public class ObjectPrefab : MonoBehaviour
{
	[SerializeField]
	protected Renderer[] meshRenderers;

	[SerializeField]
	protected Collider mainCollider;

	public Renderer[] MeshRenderers => meshRenderers;

	public Collider Collider => mainCollider;

	protected virtual void OnValidate()
	{
		meshRenderers = GetComponentsInChildren<MeshRenderer>();
		mainCollider = GetComponent<Collider>();
	}
}
