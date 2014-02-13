using UnityEngine;

[RequireComponent(typeof(WaterBase))]
[ExecuteInEditMode]
public class SpecularLighting : MonoBehaviour
{
	public Transform specularLight;

	private WaterBase waterBase;

	public void Start()
	{
		waterBase = (WaterBase)(object)((Component)this).gameObject.GetComponent(typeof(WaterBase));
	}

	public void Update()
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)waterBase))
		{
			waterBase = (WaterBase)(object)((Component)this).gameObject.GetComponent(typeof(WaterBase));
		}
		if (Object.op_Implicit((Object)(object)specularLight) && Object.op_Implicit((Object)(object)waterBase.sharedMaterial))
		{
			waterBase.sharedMaterial.SetVector("_WorldLightDir", Vector4.op_Implicit(((Component)specularLight).transform.forward));
		}
	}
}
