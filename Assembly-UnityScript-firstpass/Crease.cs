using System;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Crease")]
[ExecuteInEditMode]
public class Crease : PostEffectsBase
{
	public float intensity;

	public int softness;

	public float spread;

	public Shader blurShader;

	private Material _blurMaterial;

	public Shader depthFetchShader;

	private Material _depthFetchMaterial;

	public Shader creaseApplyShader;

	private Material _creaseApplyMaterial;

	public Crease()
	{
		intensity = 0.5f;
		softness = 1;
		spread = 1f;
	}

	public override void CreateMaterials()
	{
		_blurMaterial = CheckShaderAndCreateMaterial(blurShader, _blurMaterial);
		_depthFetchMaterial = CheckShaderAndCreateMaterial(depthFetchShader, _depthFetchMaterial);
		_creaseApplyMaterial = CheckShaderAndCreateMaterial(creaseApplyShader, _creaseApplyMaterial);
	}

	public override void Start()
	{
		CreateMaterials();
		CheckSupport(needDepth: true);
	}

	public override void OnEnable()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).camera.depthTextureMode = (DepthTextureMode)(((Component)this).camera.depthTextureMode | 1);
	}

	public override void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		CreateMaterials();
		RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height, 0);
		RenderTexture temporary2 = RenderTexture.GetTemporary(source.width / 2, source.height / 2, 0);
		RenderTexture temporary3 = RenderTexture.GetTemporary(source.width / 2, source.height / 2, 0);
		Graphics.Blit((Texture)(object)source, temporary, _depthFetchMaterial);
		Graphics.Blit((Texture)(object)temporary, temporary2);
		for (int i = 0; i < softness; i = checked(i + 1))
		{
			_blurMaterial.SetVector("offsets", new Vector4(0f, spread / (float)temporary2.height, 0f, 0f));
			Graphics.Blit((Texture)(object)temporary2, temporary3, _blurMaterial);
			_blurMaterial.SetVector("offsets", new Vector4(spread / (float)temporary2.width, 0f, 0f, 0f));
			Graphics.Blit((Texture)(object)temporary3, temporary2, _blurMaterial);
		}
		_creaseApplyMaterial.SetTexture("_HrDepthTex", (Texture)(object)temporary);
		_creaseApplyMaterial.SetTexture("_LrDepthTex", (Texture)(object)temporary2);
		_creaseApplyMaterial.SetFloat("intensity", intensity);
		Graphics.Blit((Texture)(object)source, destination, _creaseApplyMaterial);
		RenderTexture.ReleaseTemporary(temporary);
		RenderTexture.ReleaseTemporary(temporary2);
		RenderTexture.ReleaseTemporary(temporary3);
	}

	public override void Main()
	{
	}
}
