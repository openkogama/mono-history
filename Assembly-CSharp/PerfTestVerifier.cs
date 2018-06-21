using UnityEngine;

public class PerfTestVerifier : MonoBehaviour
{
	[SerializeField]
	private Camera c;

	private static PerfTestVerifier inst;

	protected void Awake()
	{
		inst = this;
	}

	public static void ToggleThemeVisibility()
	{
		if (inst.c.depth != 0f)
		{
			inst.c.depth = 0f;
		}
		else
		{
			inst.c.depth = -2f;
		}
	}
}
