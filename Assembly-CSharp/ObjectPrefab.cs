using UnityEngine;

public class ObjectPrefab : MonoBehaviour
{
	[SerializeField]
	protected MeshRenderer[] meshRenderers;

	[SerializeField]
	protected Collider mainCollider;

	public MeshRenderer[] MeshRenderers => meshRenderers;

	public Collider Collider => mainCollider;

	protected virtual void OnValidate()
	{
		meshRenderers = GetComponentsInChildren<MeshRenderer>();
		mainCollider = GetComponent<Collider>();
	}
}
