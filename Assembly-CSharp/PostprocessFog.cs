using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Skybox-based Fog")]
public class PostprocessFog : MonoBehaviour
{
	public float startDistance = 200f;

	public Cubemap skyboxCubemap;

	public Shader fogShader;

	private Material fogMaterial;

	private Camera transparentCam;

	private void Start()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected Obj, but got Unknown
		fogMaterial = CreateMaterial(fogShader, fogMaterial, checkShaderSupport: true);
		CheckSupport(needDepth: true);
		GameObject val = new GameObject("Transparent Camera");
		val.transform.parent = ((Component)this).transform;
		transparentCam = val.AddComponent<Camera>();
		transparentCam.CopyFrom(((Component)this).camera);
		transparentCam.depth = 10f;
		transparentCam.depthTextureMode = (DepthTextureMode)0;
		transparentCam.clearFlags = (CameraClearFlags)4;
		transparentCam.cullingMask = 1 << LayerMask.NameToLayer("Logic");
		Camera camera = ((Component)this).camera;
		camera.cullingMask &= ~(1 << (LayerMask.NameToLayer("Logic") & 0x1F));
	}

	private void OnEnable()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		RenderSettings.fog = false;
		Camera camera = ((Component)this).camera;
		camera.depthTextureMode = (DepthTextureMode)(camera.depthTextureMode | 1);
	}

	private void OnDisable()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		RenderSettings.fog = true;
		RenderSettings.fogColor = ((Component)this).camera.backgroundColor;
		RenderSettings.fogStartDistance = startDistance;
		RenderSettings.fogEndDistance = ((Component)this).camera.farClipPlane;
		((Component)this).camera.depthTextureMode = (DepthTextureMode)0;
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		fogMaterial = CreateMaterial(fogShader, fogMaterial, checkShaderSupport: false);
		float nearClipPlane = ((Component)this).camera.nearClipPlane;
		float farClipPlane = ((Component)this).camera.farClipPlane;
		Matrix4x4 identity = Matrix4x4.identity;
		float num = ((Component)this).camera.fieldOfView * 0.5f;
		Vector3 val = ((Component)((Component)this).camera).transform.right * nearClipPlane * Mathf.Tan(num * ((float)Math.PI / 180f)) * ((Component)this).camera.aspect;
		Vector3 val2 = ((Component)((Component)this).camera).transform.up * nearClipPlane * Mathf.Tan(num * ((float)Math.PI / 180f));
		Vector3 val3 = ((Component)((Component)this).camera).transform.forward * nearClipPlane - val + val2;
		float num2 = val3.magnitude * farClipPlane / nearClipPlane;
		val3.Normalize();
		val3 *= num2;
		Vector3 val4 = ((Component)((Component)this).camera).transform.forward * nearClipPlane + val + val2;
		val4.Normalize();
		val4 *= num2;
		Vector3 val5 = ((Component)((Component)this).camera).transform.forward * nearClipPlane + val - val2;
		val5.Normalize();
		val5 *= num2;
		Vector3 val6 = ((Component)((Component)this).camera).transform.forward * nearClipPlane - val - val2;
		val6.Normalize();
		val6 *= num2;
		identity.SetRow(0, Vector4.op_Implicit(val3));
		identity.SetRow(1, Vector4.op_Implicit(val4));
		identity.SetRow(2, Vector4.op_Implicit(val5));
		identity.SetRow(3, Vector4.op_Implicit(val6));
		float num3 = 0.05f;
		float num4 = (0f - Mathf.Log(num3)) / (farClipPlane + (0f - startDistance));
		fogMaterial.SetMatrix("_FrustumCornersWS", identity);
		fogMaterial.SetFloat("_StartDistance", 0f - startDistance);
		fogMaterial.SetFloat("_Density", num4);
		fogMaterial.SetTexture("_SkyMap", (Texture)(object)skyboxCubemap);
		CustomGraphicsBlit(source, destination, fogMaterial);
	}

	private void NotSupported()
	{
		Debug.LogError((object)("The image effect " + ((Object)this).ToString() + "is not supported on this platform!"));
		((Behaviour)this).enabled = false;
	}

	private bool CheckSupport(bool needDepth)
	{
		if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures)
		{
			NotSupported();
			return false;
		}
		if (needDepth && !SystemInfo.SupportsRenderTextureFormat((RenderTextureFormat)1))
		{
			NotSupported();
			return false;
		}
		return true;
	}

	private Material CreateMaterial(Shader s, Material m2Create, bool checkShaderSupport)
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Expected Obj, but got Unknown
		if (Object.op_Implicit((Object)(object)m2Create) && (Object)(object)m2Create.shader == (Object)(object)s)
		{
			return m2Create;
		}
		if (!Object.op_Implicit((Object)(object)s))
		{
			Debug.Log((object)("Missing shader in " + ((Object)this).ToString()));
			((Behaviour)this).enabled = false;
			return null;
		}
		if (!s.isSupported)
		{
			if (checkShaderSupport)
			{
				Debug.LogError((object)("The shader " + ((Object)s).ToString() + " on effect " + ((Object)this).ToString() + " is not supported on this platform!"));
				NotSupported();
			}
			return null;
		}
		m2Create = new Material(s);
		((Object)m2Create).hideFlags = (HideFlags)4;
		if (Object.op_Implicit((Object)(object)m2Create))
		{
			return m2Create;
		}
		return null;
	}

	private static void CustomGraphicsBlit(RenderTexture source, RenderTexture dest, Material fxMaterial)
	{
		RenderTexture.active = dest;
		fxMaterial.SetTexture("_MainTex", (Texture)(object)source);
		GL.PushMatrix();
		GL.LoadOrtho();
		fxMaterial.SetPass(0);
		GL.Begin(7);
		GL.MultiTexCoord2(0, 0f, 0f);
		GL.Vertex3(0f, 0f, 3f);
		GL.MultiTexCoord2(0, 1f, 0f);
		GL.Vertex3(1f, 0f, 2f);
		GL.MultiTexCoord2(0, 1f, 1f);
		GL.Vertex3(1f, 1f, 1f);
		GL.MultiTexCoord2(0, 0f, 1f);
		GL.Vertex3(0f, 1f, 0f);
		GL.End();
		GL.PopMatrix();
	}
}
