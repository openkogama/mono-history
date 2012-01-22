using System;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Edge Detection (Geometry)")]
[ExecuteInEditMode]
public class EdgeDetectEffectNormals : PostEffectsBase
{
	public EdgeDetectMode mode;

	public float sensitivityDepth;

	public float sensitivityNormals;

	public float edgesOnly;

	public Color edgesOnlyBgColor;

	public Shader edgeDetectShader;

	private Material _edgeDetectMaterial;

	public EdgeDetectEffectNormals()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		mode = EdgeDetectMode.Thin;
		sensitivityDepth = 1f;
		sensitivityNormals = 1f;
		edgesOnlyBgColor = Color.white;
	}

	public override void CreateMaterials()
	{
		_edgeDetectMaterial = CheckShaderAndCreateMaterial(edgeDetectShader, _edgeDetectMaterial);
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
		((Component)this).camera.depthTextureMode = (DepthTextureMode)(((Component)this).camera.depthTextureMode | 2);
	}

	public override void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		CreateMaterials();
		Vector2 val = new Vector2
		{
			x = sensitivityDepth,
			y = sensitivityNormals
		};
		((Texture)source).filterMode = (FilterMode)0;
		_edgeDetectMaterial.SetVector("sensitivity", new Vector4(val.x, val.y, 1f, val.y));
		_edgeDetectMaterial.SetFloat("_BgFade", edgesOnly);
		Vector4 val2 = Color.op_Implicit(edgesOnlyBgColor);
		_edgeDetectMaterial.SetVector("_BgColor", val2);
		if (mode == EdgeDetectMode.Thin)
		{
			Graphics.Blit((Texture)(object)source, destination, _edgeDetectMaterial, 0);
		}
		else
		{
			Graphics.Blit((Texture)(object)source, destination, _edgeDetectMaterial, 1);
		}
	}

	public override void Main()
	{
	}
}
