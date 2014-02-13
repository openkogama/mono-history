using UnityEngine;

[ExecuteInEditMode]
public class WaterBase : MonoBehaviour
{
	public Material sharedMaterial;

	public WaterQuality waterQuality = WaterQuality.High;

	public bool edgeBlend = true;

	public void UpdateShader()
	{
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		if (waterQuality > WaterQuality.Medium)
		{
			sharedMaterial.shader.maximumLOD = 501;
		}
		else if (waterQuality > WaterQuality.Low)
		{
			sharedMaterial.shader.maximumLOD = 301;
		}
		else
		{
			sharedMaterial.shader.maximumLOD = 201;
		}
		if (edgeBlend)
		{
			Shader.EnableKeyword("WATER_EDGEBLEND_ON");
			Shader.DisableKeyword("WATER_EDGEBLEND_OFF");
			if (Object.op_Implicit((Object)(object)Camera.main))
			{
				Camera main = Camera.main;
				main.depthTextureMode = (DepthTextureMode)(main.depthTextureMode | 1);
			}
		}
		else
		{
			Shader.EnableKeyword("WATER_EDGEBLEND_OFF");
			Shader.DisableKeyword("WATER_EDGEBLEND_ON");
		}
	}

	public void WaterTileBeingRendered(Transform tr, Camera currentCam)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)currentCam) && edgeBlend)
		{
			currentCam.depthTextureMode = (DepthTextureMode)(currentCam.depthTextureMode | 1);
		}
	}

	public void Update()
	{
		if (Object.op_Implicit((Object)(object)sharedMaterial))
		{
			UpdateShader();
		}
	}
}
