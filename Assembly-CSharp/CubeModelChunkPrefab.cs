using UnityEngine;

public class CubeModelChunkPrefab : MonoBehaviour
{
	[SerializeField]
	private MeshFilter meshFilter;

	[SerializeField]
	private MeshRenderer meshRenderer;

	[SerializeField]
	private BoxCollider boxCollider;

	public MeshFilter MeshFilter => meshFilter;

	public MeshRenderer MeshRenderer => meshRenderer;

	public BoxCollider BoxCollider => boxCollider;
}
