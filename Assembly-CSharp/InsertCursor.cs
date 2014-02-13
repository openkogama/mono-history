using UnityEngine;

public class InsertCursor : MonoBehaviour
{
	private MeshRenderer[] renderers = new MeshRenderer[0];

	private void Start()
	{
		renderers = ((Component)this).GetComponentsInChildren<MeshRenderer>();
		((Behaviour)this).enabled = false;
	}

	private void OnDisable()
	{
		MeshRenderer[] array = renderers;
		foreach (MeshRenderer val in array)
		{
			((Renderer)val).enabled = false;
		}
	}

	private void OnEnable()
	{
		MeshRenderer[] array = renderers;
		foreach (MeshRenderer val in array)
		{
			((Renderer)val).enabled = true;
		}
	}
}
