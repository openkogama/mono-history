using System;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Edge Blur (Luminance)")]
[ExecuteInEditMode]
public class LuminanceEdgeBlur : PostEffectsBase
{
	public Shader luminance2Normals;

	private Material _luminance2NormalsBasedBlur;

	public bool showGeneratedNormals;

	public float offsetScale;

	public float blurRadius;

	public LuminanceEdgeBlur()
	{
		offsetScale = 0.1f;
		blurRadius = 18f;
	}

	public override void CreateMaterials()
	{
		_luminance2NormalsBasedBlur = CheckShaderAndCreateMaterial(luminance2Normals, _luminance2NormalsBasedBlur);
	}

	public override void Start()
	{
		CreateMaterials();
		CheckSupport(needDepth: false);
	}

	public override void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		CreateMaterials();
		_luminance2NormalsBasedBlur.SetFloat("_OffsetScale", offsetScale);
		_luminance2NormalsBasedBlur.SetFloat("_BlurRadius", blurRadius);
		if (showGeneratedNormals)
		{
			Shader.EnableKeyword("SHOW_DEBUG_ON");
			Shader.DisableKeyword("SHOW_DEBUG_OFF");
		}
		else
		{
			Shader.DisableKeyword("SHOW_DEBUG_ON");
			Shader.EnableKeyword("SHOW_DEBUG_OFF");
		}
		Graphics.Blit((Texture)(object)source, destination, _luminance2NormalsBasedBlur);
	}

	public override void Main()
	{
	}
}
