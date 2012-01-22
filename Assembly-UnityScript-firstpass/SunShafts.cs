using System;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Sun Shafts")]
[ExecuteInEditMode]
public class SunShafts : PostEffectsBase
{
	public SunShaftsResolution resolution;

	public Transform sunTransform;

	public int radialBlurIterations;

	public Color sunColor;

	public float sunShaftBlurRadius;

	public float sunShaftIntensity;

	public float useSkyBoxAlpha;

	public float maxRadius;

	public bool useDepthTexture;

	public Shader clearShader;

	private Material _clearMaterial;

	public Shader depthDecodeShader;

	private Material _encodeDepthRGBA8Material;

	public Shader depthBlurShader;

	private Material _radialDepthBlurMaterial;

	public Shader sunShaftsShader;

	private Material _sunShaftsMaterial;

	public Shader simpleClearShader;

	private Material _simpleClearMaterial;

	public Shader compShader;

	private Material _compMaterial;

	public SunShafts()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		radialBlurIterations = 2;
		sunColor = Color.white;
		sunShaftBlurRadius = 0.0164f;
		sunShaftIntensity = 1.25f;
		useSkyBoxAlpha = 0.75f;
		maxRadius = 1.25f;
		useDepthTexture = true;
	}

	public override void CreateMaterials()
	{
		_clearMaterial = CheckShaderAndCreateMaterial(clearShader, _clearMaterial);
		_sunShaftsMaterial = CheckShaderAndCreateMaterial(sunShaftsShader, _sunShaftsMaterial);
		_encodeDepthRGBA8Material = CheckShaderAndCreateMaterial(depthDecodeShader, _encodeDepthRGBA8Material);
		_radialDepthBlurMaterial = CheckShaderAndCreateMaterial(depthBlurShader, _radialDepthBlurMaterial);
		_simpleClearMaterial = CheckShaderAndCreateMaterial(simpleClearShader, _simpleClearMaterial);
		_compMaterial = CheckShaderAndCreateMaterial(compShader, _compMaterial);
	}

	public override void Start()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		CreateMaterials();
		CheckSupport(useDepthTexture);
		if (useDepthTexture)
		{
			((Component)this).camera.depthTextureMode = (DepthTextureMode)(((Component)this).camera.depthTextureMode | 1);
		}
	}

	public override void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		CreateMaterials();
		float num = 4f;
		if (resolution == SunShaftsResolution.Normal)
		{
			num = 2f;
		}
		if (resolution == SunShaftsResolution.High)
		{
			num = 1f;
		}
		checked
		{
			RenderTexture temporary = RenderTexture.GetTemporary((int)((float)source.width / num), (int)((float)source.height / num), 0);
			RenderTexture temporary2 = RenderTexture.GetTemporary((int)((float)source.width / num), (int)((float)source.height / num), 0);
			Graphics.Blit((Texture)(object)source, destination);
			if (!useDepthTexture)
			{
				RenderTexture val = (RenderTexture.active = RenderTexture.GetTemporary(source.width, source.height, 0));
				GL.ClearWithSkybox(false, ((Component)this).camera);
				_compMaterial.SetTexture("_Skybox", (Texture)(object)val);
				Graphics.Blit((Texture)(object)source, source, _compMaterial);
				RenderTexture.ReleaseTemporary(val);
			}
			else
			{
				Graphics.Blit((Texture)(object)source, source, _clearMaterial);
			}
			_encodeDepthRGBA8Material.SetFloat("noSkyBoxMask", 1f - useSkyBoxAlpha);
			_encodeDepthRGBA8Material.SetFloat("dontUseSkyboxBrightness", 0f);
			Graphics.Blit((Texture)(object)source, temporary2, _encodeDepthRGBA8Material);
			DrawBorder(temporary2, _simpleClearMaterial);
			Vector3 val2 = Vector3.one * 0.5f;
			val2 = ((!Object.op_Implicit((Object)(object)sunTransform)) ? new Vector3(0.5f, 0.5f, 0f) : ((Component)this).camera.WorldToViewportPoint(sunTransform.position));
			_radialDepthBlurMaterial.SetVector("blurRadius4", new Vector4(1f, 1f, 0f, 0f) * sunShaftBlurRadius);
			_radialDepthBlurMaterial.SetVector("sunPosition", new Vector4(val2.x, val2.y, val2.z, maxRadius));
			if (radialBlurIterations < 1)
			{
				radialBlurIterations = 1;
			}
			for (int i = 0; i < radialBlurIterations; i++)
			{
				Graphics.Blit((Texture)(object)temporary2, temporary, _radialDepthBlurMaterial);
				Graphics.Blit((Texture)(object)temporary, temporary2, _radialDepthBlurMaterial);
			}
			_sunShaftsMaterial.SetFloat("sunShaftIntensity", sunShaftIntensity);
			if (!(val2.z < 0f))
			{
				_sunShaftsMaterial.SetVector("sunColor", new Vector4(sunColor.r, sunColor.g, sunColor.b, sunColor.a));
			}
			else
			{
				_sunShaftsMaterial.SetVector("sunColor", new Vector4(0f, 0f, 0f, 0f));
			}
			_sunShaftsMaterial.SetTexture("_ColorBuffer", (Texture)(object)source);
			Graphics.Blit((Texture)(object)temporary2, destination, _sunShaftsMaterial);
			RenderTexture.ReleaseTemporary(temporary2);
			RenderTexture.ReleaseTemporary(temporary);
		}
	}

	public override void Main()
	{
	}
}
