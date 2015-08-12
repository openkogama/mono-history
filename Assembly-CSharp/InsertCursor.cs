using UnityEngine;

public class InsertCursor : MonoBehaviour
{
	private MeshRenderer[] renderers = new MeshRenderer[0];

	private void Start()
	{
		renderers = GetComponentsInChildren<MeshRenderer>();
		enabled = false;
	}

	private void OnDisable()
	{
		MeshRenderer[] array = renderers;
		foreach (MeshRenderer meshRenderer in array)
		{
			meshRenderer.enabled = false;
		}
	}

	private void OnEnable()
	{
		MeshRenderer[] array = renderers;
		foreach (MeshRenderer meshRenderer in array)
		{
			meshRenderer.enabled = true;
		}
	}
}
