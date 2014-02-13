using System;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Contrast Enhance (Unsharp Mask)")]
[ExecuteInEditMode]
public class ContrastEnhance : PostEffectsBase
{
	public float intensity;

	public float threshhold;

	private Material _separableBlurMaterial;

	private Material _contrastCompositeMaterial;

	public float blurSpread;

	public Shader separableBlurShader;

	public Shader contrastCompositeShader;

	public ContrastEnhance()
	{
		intensity = 0.5f;
		blurSpread = 1f;
	}

	public override void CreateMaterials()
	{
		_contrastCompositeMaterial = CheckShaderAndCreateMaterial(contrastCompositeShader, _contrastCompositeMaterial);
		_separableBlurMaterial = CheckShaderAndCreateMaterial(separableBlurShader, _separableBlurMaterial);
	}

	public override void Start()
	{
		CreateMaterials();
		CheckSupport(needDepth: false);
	}

	public override void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		CreateMaterials();
		RenderTexture temporary = RenderTexture.GetTemporary((int)((float)source.width / 2f), (int)((float)source.height / 2f), 0);
		RenderTexture temporary2 = RenderTexture.GetTemporary((int)((float)source.width / 4f), (int)((float)source.height / 4f), 0);
		RenderTexture temporary3 = RenderTexture.GetTemporary((int)((float)source.width / 4f), (int)((float)source.height / 4f), 0);
		Graphics.Blit((Texture)(object)source, temporary);
		Graphics.Blit((Texture)(object)temporary, temporary2);
		_separableBlurMaterial.SetVector("offsets", new Vector4(0f, blurSpread * 1f / (float)temporary2.height, 0f, 0f));
		Graphics.Blit((Texture)(object)temporary2, temporary3, _separableBlurMaterial);
		_separableBlurMaterial.SetVector("offsets", new Vector4(blurSpread * 1f / (float)temporary2.width, 0f, 0f, 0f));
		Graphics.Blit((Texture)(object)temporary3, temporary2, _separableBlurMaterial);
		_contrastCompositeMaterial.SetTexture("_MainTexBlurred", (Texture)(object)temporary2);
		_contrastCompositeMaterial.SetFloat("intensity", intensity);
		_contrastCompositeMaterial.SetFloat("threshhold", threshhold);
		Graphics.Blit((Texture)(object)source, destination, _contrastCompositeMaterial);
		RenderTexture.ReleaseTemporary(temporary);
		RenderTexture.ReleaseTemporary(temporary2);
		RenderTexture.ReleaseTemporary(temporary3);
	}

	public override void Main()
	{
	}
}
