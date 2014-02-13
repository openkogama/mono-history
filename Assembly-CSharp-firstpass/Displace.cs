using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(WaterBase))]
public class Displace : MonoBehaviour
{
	private WaterBase waterBase;

	public void Start()
	{
		if (!Object.op_Implicit((Object)(object)waterBase))
		{
			waterBase = (WaterBase)(object)((Component)this).gameObject.GetComponent(typeof(WaterBase));
		}
	}

	public void OnEnable()
	{
		Shader.EnableKeyword("WATER_VERTEX_DISPLACEMENT_ON");
		Shader.DisableKeyword("WATER_VERTEX_DISPLACEMENT_OFF");
	}

	public void OnDisable()
	{
		Shader.EnableKeyword("WATER_VERTEX_DISPLACEMENT_OFF");
		Shader.DisableKeyword("WATER_VERTEX_DISPLACEMENT_ON");
	}
}
