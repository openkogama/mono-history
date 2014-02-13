using System;
using UnityEngine;

[Serializable]
[AddComponentMenu("Image Effects/Color Correction (Curves)")]
[ExecuteInEditMode]
public class ColorCorrectionCurves : PostEffectsBase
{
	public AnimationCurve redChannel;

	public AnimationCurve greenChannel;

	public AnimationCurve blueChannel;

	public bool useDepthCorrection;

	public AnimationCurve zCurve;

	public AnimationCurve depthRedChannel;

	public AnimationCurve depthGreenChannel;

	public AnimationCurve depthBlueChannel;

	private Material _ccMaterial;

	private Material _ccDepthMaterial;

	private Material _selectiveCcMaterial;

	private Texture2D _rgbChannelTex;

	private Texture2D _rgbDepthChannelTex;

	private Texture2D _zCurve;

	public bool selectiveCc;

	public Color selectiveFromColor;

	public Color selectiveToColor;

	public ColorCorrectionMode mode;

	public bool updateTextures;

	public Shader colorCorrectionCurvesShader;

	public Shader simpleColorCorrectionCurvesShader;

	public Shader colorCorrectionSelectiveShader;

	public ColorCorrectionCurves()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		selectiveFromColor = Color.white;
		selectiveToColor = Color.white;
		updateTextures = true;
	}

	public override void Start()
	{
		updateTextures = true;
		CreateMaterials();
		CheckSupport(needDepth: true);
	}

	public override void CreateMaterials()
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected Obj, but got Unknown
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected Obj, but got Unknown
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Expected Obj, but got Unknown
		_ccMaterial = CheckShaderAndCreateMaterial(simpleColorCorrectionCurvesShader, _ccMaterial);
		_ccDepthMaterial = CheckShaderAndCreateMaterial(colorCorrectionCurvesShader, _ccDepthMaterial);
		_selectiveCcMaterial = CheckShaderAndCreateMaterial(colorCorrectionSelectiveShader, _selectiveCcMaterial);
		if (!Object.op_Implicit((Object)(object)_rgbChannelTex))
		{
			_rgbChannelTex = new Texture2D(256, 4, (TextureFormat)5, false);
			((Object)_rgbChannelTex).hideFlags = (HideFlags)13;
		}
		if (!Object.op_Implicit((Object)(object)_rgbDepthChannelTex))
		{
			_rgbDepthChannelTex = new Texture2D(256, 4, (TextureFormat)5, false);
			((Object)_rgbDepthChannelTex).hideFlags = (HideFlags)13;
		}
		if (!Object.op_Implicit((Object)(object)_zCurve))
		{
			_zCurve = new Texture2D(256, 1, (TextureFormat)5, false);
			((Object)_zCurve).hideFlags = (HideFlags)13;
		}
		((Texture)_rgbChannelTex).wrapMode = (TextureWrapMode)1;
		((Texture)_rgbDepthChannelTex).wrapMode = (TextureWrapMode)1;
		((Texture)_zCurve).wrapMode = (TextureWrapMode)1;
	}

	public override void OnEnable()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (useDepthCorrection)
		{
			((Component)this).camera.depthTextureMode = (DepthTextureMode)(((Component)this).camera.depthTextureMode | 1);
		}
	}

	public override void OnDisable()
	{
	}

	public override void UpdateParameters()
	{
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		if (updateTextures && redChannel != null && greenChannel != null && blueChannel != null)
		{
			for (float num = 0f; num <= 1f; num += 1f / 255f)
			{
				float num2 = Mathf.Clamp(redChannel.Evaluate(num), 0f, 1f);
				float num3 = Mathf.Clamp(greenChannel.Evaluate(num), 0f, 1f);
				float num4 = Mathf.Clamp(blueChannel.Evaluate(num), 0f, 1f);
				_rgbChannelTex.SetPixel((int)Mathf.Floor(num * 255f), 0, new Color(num2, num2, num2));
				_rgbChannelTex.SetPixel((int)Mathf.Floor(num * 255f), 1, new Color(num3, num3, num3));
				_rgbChannelTex.SetPixel((int)Mathf.Floor(num * 255f), 2, new Color(num4, num4, num4));
				float num5 = Mathf.Clamp(zCurve.Evaluate(num), 0f, 1f);
				_zCurve.SetPixel((int)Mathf.Floor(num * 255f), 0, new Color(num5, num5, num5));
				num2 = Mathf.Clamp(depthRedChannel.Evaluate(num), 0f, 1f);
				num3 = Mathf.Clamp(depthGreenChannel.Evaluate(num), 0f, 1f);
				num4 = Mathf.Clamp(depthBlueChannel.Evaluate(num), 0f, 1f);
				_rgbDepthChannelTex.SetPixel((int)Mathf.Floor(num * 255f), 0, new Color(num2, num2, num2));
				_rgbDepthChannelTex.SetPixel((int)Mathf.Floor(num * 255f), 1, new Color(num3, num3, num3));
				_rgbDepthChannelTex.SetPixel((int)Mathf.Floor(num * 255f), 2, new Color(num4, num4, num4));
			}
			_rgbChannelTex.Apply();
			_rgbDepthChannelTex.Apply();
			_zCurve.Apply();
			updateTextures = false;
		}
	}

	public override void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		CreateMaterials();
		UpdateParameters();
		if (useDepthCorrection)
		{
			((Component)this).camera.depthTextureMode = (DepthTextureMode)(((Component)this).camera.depthTextureMode | 1);
		}
		RenderTexture val = destination;
		if (selectiveCc)
		{
			val = RenderTexture.GetTemporary(source.width, source.height);
		}
		if (useDepthCorrection)
		{
			_ccDepthMaterial.SetTexture("_RgbTex", (Texture)(object)_rgbChannelTex);
			_ccDepthMaterial.SetTexture("_ZCurve", (Texture)(object)_zCurve);
			_ccDepthMaterial.SetTexture("_RgbDepthTex", (Texture)(object)_rgbDepthChannelTex);
			Graphics.Blit((Texture)(object)source, val, _ccDepthMaterial);
		}
		else
		{
			_ccMaterial.SetTexture("_RgbTex", (Texture)(object)_rgbChannelTex);
			Graphics.Blit((Texture)(object)source, val, _ccMaterial);
		}
		if (selectiveCc)
		{
			_selectiveCcMaterial.SetVector("selColor", new Vector4(selectiveFromColor.r, selectiveFromColor.g, selectiveFromColor.b, selectiveFromColor.a));
			_selectiveCcMaterial.SetVector("targetColor", new Vector4(selectiveToColor.r, selectiveToColor.g, selectiveToColor.b, selectiveToColor.a));
			Graphics.Blit((Texture)(object)val, destination, _selectiveCcMaterial);
			RenderTexture.ReleaseTemporary(val);
		}
	}

	public override void Main()
	{
	}
}
