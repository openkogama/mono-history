using UnityEngine;

public class UXCamera : MonoBehaviour
{
	private bool fogState;

	[SerializeField]
	private Camera secondaryUXCamera;

	public Camera SecondaryUXCamera => secondaryUXCamera;

	public void Awake()
	{
		GetComponent<Camera>().cullingMask = 1 << LayerMask.NameToLayer("UXElement");
	}

	public void OnPreRender()
	{
		fogState = RenderSettings.fog;
		RenderSettings.fog = false;
	}

	public void OnPostRender()
	{
		RenderSettings.fog = fogState;
	}
}
