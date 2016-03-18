using UnityEngine;

public class ObjectPrefab : MonoBehaviour
{
	[SerializeField]
	protected MeshRenderer[] meshRenderers;

	[SerializeField]
	protected Collider collider;

	public MeshRenderer[] MeshRenderers => meshRenderers;

	public Collider Collider => collider;

	protected virtual void OnValidate()
	{
		meshRenderers = GetComponentsInChildren<MeshRenderer>();
		collider = GetComponent<Collider>();
	}
}
