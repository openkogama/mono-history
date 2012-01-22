using System;
using UnityEngine;

[Serializable]
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Depth of Field")]
public class DepthOfField : PostEffectsBase
{
	public DofQualitySetting quality;

	public float divider;

	public float focalZDistance;

	public float focalStartCurve;

	public float focalEndCurve;

	public float focalZStart;

	public float focalZEnd;

	private float _focalDistance01;

	private float _focalStart01;

	private float _focalEnd01;

	public float focalFalloff;

	public Transform objectFocus;

	public float focalSize;

	public bool enableBokeh;

	public float bokehThreshhold;

	public float bokehFalloff;

	public float noiseAmount;

	public int blurIterations;

	public float blurSpread;

	public int foregroundBlurIterations;

	public float foregroundBlurSpread;

	public float foregroundBlurWeight;

	public Shader weightedBlurShader;

	private Material _weightedBlurMaterial;

	public Shader preDofShader;

	private Material _preDofMaterial;

	public Shader blurShader;

	private Material _blurMaterial;

	public DepthOfField()
	{
		quality = DofQualitySetting.High;
		divider = 2f;
		focalStartCurve = 1.175f;
		focalEndCurve = 1.1f;
		focalZEnd = 10000f;
		_focalDistance01 = 0.1f;
		_focalEnd01 = 1f;
		focalFalloff = 1f;
		focalSize = 0.075f;
		enableBokeh = true;
		bokehThreshhold = 0.2f;
		bokehFalloff = 0.2f;
		noiseAmount = 1.5f;
		blurIterations = 1;
		blurSpread = 1.35f;
		foregroundBlurIterations = 1;
		foregroundBlurSpread = 1f;
		foregroundBlurWeight = 1f;
	}

	public override void CreateMaterials()
	{
		_weightedBlurMaterial = CheckShaderAndCreateMaterial(weightedBlurShader, _weightedBlurMaterial);
		_blurMaterial = CheckShaderAndCreateMaterial(blurShader, _blurMaterial);
		_preDofMaterial = CheckShaderAndCreateMaterial(preDofShader, _preDofMaterial);
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
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0603: Unknown result type (might be due to invalid IL or missing references)
		//IL_0641: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0813: Unknown result type (might be due to invalid IL or missing references)
		//IL_0695: Unknown result type (might be due to invalid IL or missing references)
		//IL_06dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_072d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0774: Unknown result type (might be due to invalid IL or missing references)
		CreateMaterials();
		((Texture)source).filterMode = (FilterMode)1;
		if (Object.op_Implicit((Object)(object)objectFocus))
		{
			Vector3 val = ((Component)this).camera.WorldToViewportPoint(objectFocus.position);
			val.z /= ((Component)this).camera.farClipPlane;
			_focalDistance01 = val.z;
		}
		else
		{
			_focalDistance01 = ((Component)this).camera.WorldToViewportPoint(focalZDistance * ((Component)((Component)this).camera).transform.forward + ((Component)((Component)this).camera).transform.position).z / ((Component)this).camera.farClipPlane;
		}
		if (!(focalZEnd <= ((Component)this).camera.farClipPlane))
		{
			focalZEnd = ((Component)this).camera.farClipPlane;
		}
		_focalStart01 = ((Component)this).camera.WorldToViewportPoint(focalZStart * ((Component)((Component)this).camera).transform.forward + ((Component)((Component)this).camera).transform.position).z / ((Component)this).camera.farClipPlane;
		_focalEnd01 = ((Component)this).camera.WorldToViewportPoint(focalZEnd * ((Component)((Component)this).camera).transform.forward + ((Component)((Component)this).camera).transform.position).z / ((Component)this).camera.farClipPlane;
		if (!(_focalDistance01 >= _focalStart01))
		{
			_focalDistance01 = _focalStart01 + float.Epsilon;
		}
		if (!(_focalEnd01 >= _focalStart01))
		{
			_focalEnd01 = _focalStart01 + float.Epsilon;
		}
		_preDofMaterial.SetFloat("focalDistance01", _focalDistance01);
		_preDofMaterial.SetFloat("focalFalloff", focalFalloff);
		_preDofMaterial.SetFloat("focalStart01", _focalStart01);
		_preDofMaterial.SetFloat("focalEnd01", _focalEnd01);
		_preDofMaterial.SetFloat("focalSize", focalSize * 0.5f);
		_preDofMaterial.SetFloat("_ForegroundBlurWeight", foregroundBlurWeight);
		_preDofMaterial.SetVector("_CurveParams", new Vector4(focalStartCurve, focalEndCurve, 0f, 0f));
		float num = (0f - bokehFalloff) / (1f * (float)foregroundBlurIterations);
		_preDofMaterial.SetVector("_BokehThreshhold", new Vector4(bokehThreshhold, 1f / (1f - bokehThreshhold) * (1f - num), num, noiseAmount));
		_preDofMaterial.SetVector("_InvRenderTargetSize", new Vector4(1f / (1f * (float)source.width), 1f / (1f * (float)source.height), 0f, 0f));
		RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height, 0);
		checked
		{
			RenderTexture temporary2 = RenderTexture.GetTemporary((int)((float)source.width / divider), (int)((float)source.height / divider), 0);
			RenderTexture temporary3 = RenderTexture.GetTemporary((int)((float)source.width / divider), (int)((float)source.height / divider), 0);
			RenderTexture temporary4 = RenderTexture.GetTemporary((int)((float)source.width / divider), (int)((float)source.height / divider), 0);
			RenderTexture temporary5 = RenderTexture.GetTemporary((int)((float)source.width / divider), (int)((float)source.height / divider), 0);
			((Texture)temporary).filterMode = (FilterMode)1;
			((Texture)temporary3).filterMode = (FilterMode)1;
			((Texture)temporary4).filterMode = (FilterMode)1;
			((Texture)temporary5).filterMode = (FilterMode)1;
			((Texture)temporary2).filterMode = (FilterMode)1;
			if (quality >= DofQualitySetting.High)
			{
				Graphics.Blit((Texture)(object)source, temporary, _preDofMaterial, 11);
				Graphics.Blit((Texture)(object)temporary, temporary3, _preDofMaterial, 12);
				if (foregroundBlurIterations < 1)
				{
					foregroundBlurIterations = 1;
				}
				int num2 = ((!enableBokeh) ? 6 : 9);
				for (int i = 0; i < foregroundBlurIterations; i++)
				{
					_preDofMaterial.SetVector("_Vh", new Vector4(foregroundBlurSpread, 0f, 0f, 0f));
					Graphics.Blit((Texture)(object)temporary3, temporary5, _preDofMaterial, num2);
					_preDofMaterial.SetVector("_Vh", new Vector4(0f, foregroundBlurSpread, 0f, 0f));
					Graphics.Blit((Texture)(object)temporary5, temporary3, _preDofMaterial, num2);
					if (enableBokeh)
					{
						_preDofMaterial.SetVector("_Vh", new Vector4(foregroundBlurSpread, 0f - foregroundBlurSpread, 0f, 0f));
						Graphics.Blit((Texture)(object)temporary3, temporary5, _preDofMaterial, num2);
						_preDofMaterial.SetVector("_Vh", new Vector4(0f - foregroundBlurSpread, 0f - foregroundBlurSpread, 0f, 0f));
						Graphics.Blit((Texture)(object)temporary5, temporary3, _preDofMaterial, num2);
					}
				}
				Graphics.Blit((Texture)(object)source, source, _preDofMaterial, 4);
				Graphics.Blit((Texture)(object)source, temporary2, _preDofMaterial, 12);
			}
			else
			{
				Graphics.Blit((Texture)(object)source, source, _preDofMaterial, 3);
				Graphics.Blit((Texture)(object)source, temporary2, _preDofMaterial, 12);
			}
			if (blurIterations < 1)
			{
				blurIterations = 1;
			}
			float num3 = (0f - bokehFalloff) / (1f * (float)blurIterations);
			_weightedBlurMaterial.SetVector("_Threshhold", new Vector4(bokehThreshhold, 1f / (1f - bokehThreshhold) * (1f - num3), num3, noiseAmount));
			if (quality >= DofQualitySetting.Medium)
			{
				_weightedBlurMaterial.SetVector("offsets", new Vector4(0f, blurSpread * 1.5f / (float)source.height, 0f, 0f));
				Graphics.Blit((Texture)(object)temporary2, temporary5, _weightedBlurMaterial, 1);
				_weightedBlurMaterial.SetVector("offsets", new Vector4(blurSpread * 1.5f / (float)source.width, 0f, 0f, 0f));
				Graphics.Blit((Texture)(object)temporary5, temporary2, _weightedBlurMaterial, 1);
				int num4 = ((!enableBokeh) ? 1 : 0);
				for (int j = 0; j < blurIterations; j++)
				{
					_weightedBlurMaterial.SetVector("offsets", new Vector4(0f, blurSpread / (float)source.height, 0f, 0f));
					Graphics.Blit((Texture)(object)((j != 0) ? temporary4 : temporary2), temporary5, _weightedBlurMaterial, num4);
					_weightedBlurMaterial.SetVector("offsets", new Vector4(blurSpread / (float)source.width, 0f, 0f, 0f));
					Graphics.Blit((Texture)(object)temporary5, temporary4, _weightedBlurMaterial, num4);
					if (enableBokeh)
					{
						_weightedBlurMaterial.SetVector("offsets", new Vector4(blurSpread / (float)source.width, blurSpread / (float)source.height, 0f, 0f));
						Graphics.Blit((Texture)(object)temporary4, temporary5, _weightedBlurMaterial, num4);
						_weightedBlurMaterial.SetVector("offsets", new Vector4(blurSpread / (float)source.width, (0f - blurSpread) / (float)source.height, 0f, 0f));
						Graphics.Blit((Texture)(object)temporary5, temporary4, _weightedBlurMaterial, num4);
					}
				}
			}
			else
			{
				for (int j = 0; j < blurIterations; j++)
				{
					_blurMaterial.SetVector("offsets", new Vector4(0f, blurSpread / (float)source.height, 0f, 0f));
					Graphics.Blit((Texture)(object)((j != 0) ? temporary4 : temporary2), temporary5, _blurMaterial);
					_blurMaterial.SetVector("offsets", new Vector4(blurSpread / (float)source.width, 0f, 0f, 0f));
					Graphics.Blit((Texture)(object)temporary5, temporary4, _blurMaterial);
				}
			}
			bool flag = _focalDistance01 > 0f;
			if (flag)
			{
				flag = focalStartCurve > 0f;
			}
			bool flag2 = flag;
			_preDofMaterial.SetTexture("_FgLowRez", (Texture)(object)temporary3);
			_preDofMaterial.SetTexture("_BgLowRez", (Texture)(object)temporary4);
			_preDofMaterial.SetTexture("_BgUnblurredTex", (Texture)(object)temporary2);
			_weightedBlurMaterial.SetTexture("_TapLow", (Texture)(object)temporary4);
			_weightedBlurMaterial.SetTexture("_TapMedium", (Texture)(object)temporary2);
			Graphics.Blit((Texture)(object)temporary4, temporary4, _weightedBlurMaterial, 3);
			if (quality > DofQualitySetting.Medium)
			{
				Graphics.Blit((Texture)(object)source, (!flag2) ? destination : temporary, _preDofMaterial, 0);
			}
			else if (quality == DofQualitySetting.Medium)
			{
				Graphics.Blit((Texture)(object)source, destination, _preDofMaterial, 2);
			}
			else if (quality == DofQualitySetting.Low)
			{
				Graphics.Blit((Texture)(object)source, destination, _preDofMaterial, 1);
			}
			if (quality > DofQualitySetting.Medium && flag2)
			{
				Graphics.Blit((Texture)(object)temporary, temporary2, _preDofMaterial, 12);
				Graphics.Blit((Texture)(object)temporary, destination, _preDofMaterial, 10);
			}
			RenderTexture.ReleaseTemporary(temporary);
			RenderTexture.ReleaseTemporary(temporary3);
			RenderTexture.ReleaseTemporary(temporary4);
			RenderTexture.ReleaseTemporary(temporary5);
			RenderTexture.ReleaseTemporary(temporary2);
		}
	}

	public override void Main()
	{
	}
}
