using System;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Vignette")]
[ExecuteInEditMode]
public class Vignetting : PostEffectsBase
{
	public float vignetteIntensity;

	public float chromaticAberrationIntensity;

	public float blurVignette;

	public Shader vignetteShader;

	private Material _vignetteMaterial;

	public Shader separableBlurShader;

	private Material _separableBlurMaterial;

	public Shader chromAberrationShader;

	private Material _chromAberrationMaterial;

	public Vignetting()
	{
		vignetteIntensity = 0.375f;
	}

	public override void Start()
	{
		CreateMaterials();
		CheckSupport(needDepth: false);
	}

	public override void CreateMaterials()
	{
		_vignetteMaterial = CheckShaderAndCreateMaterial(vignetteShader, _vignetteMaterial);
		_separableBlurMaterial = CheckShaderAndCreateMaterial(separableBlurShader, _separableBlurMaterial);
		_chromAberrationMaterial = CheckShaderAndCreateMaterial(chromAberrationShader, _chromAberrationMaterial);
	}

	public override void OnEnable()
	{
	}

	public override void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		CreateMaterials();
		RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height, 0);
		RenderTexture temporary2 = RenderTexture.GetTemporary((int)((float)source.width / 2f), (int)((float)source.height / 2f), 0);
		RenderTexture temporary3 = RenderTexture.GetTemporary((int)((float)source.width / 4f), (int)((float)source.height / 4f), 0);
		RenderTexture temporary4 = RenderTexture.GetTemporary((int)((float)source.width / 4f), (int)((float)source.height / 4f), 0);
		Graphics.Blit((Texture)(object)source, temporary2, _chromAberrationMaterial, 0);
		Graphics.Blit((Texture)(object)temporary2, temporary3);
		for (int i = 0; i < 2; i++)
		{
			_separableBlurMaterial.SetVector("offsets", new Vector4(0f, 1.5f / (float)temporary3.height, 0f, 0f));
			Graphics.Blit((Texture)(object)temporary3, temporary4, _separableBlurMaterial);
			_separableBlurMaterial.SetVector("offsets", new Vector4(1.5f / (float)temporary3.width, 0f, 0f, 0f));
			Graphics.Blit((Texture)(object)temporary4, temporary3, _separableBlurMaterial);
		}
		_vignetteMaterial.SetFloat("vignetteIntensity", vignetteIntensity);
		_vignetteMaterial.SetFloat("blurVignette", blurVignette);
		_vignetteMaterial.SetTexture("_VignetteTex", (Texture)(object)temporary3);
		Graphics.Blit((Texture)(object)source, temporary, _vignetteMaterial);
		_chromAberrationMaterial.SetFloat("chromaticAberrationIntensity", chromaticAberrationIntensity);
		Graphics.Blit((Texture)(object)temporary, destination, _chromAberrationMaterial, 1);
		RenderTexture.ReleaseTemporary(temporary);
		RenderTexture.ReleaseTemporary(temporary2);
		RenderTexture.ReleaseTemporary(temporary3);
		RenderTexture.ReleaseTemporary(temporary4);
	}

	public override void Main()
	{
	}
}
