using UnityEngine;

public class UXCamera : MonoBehaviour
{
	private bool fogState;

	public void Awake()
	{
		((Component)this).camera.cullingMask = 1 << LayerMask.NameToLayer("UXElement");
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
