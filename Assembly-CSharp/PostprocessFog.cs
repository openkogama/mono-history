using System;
using UnityEngine;

[AddComponentMenu("Image Effects/Skybox-based Fog")]
[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class PostprocessFog : MonoBehaviour
{
	public float startDistance = 200f;

	public Cubemap skyboxCubemap;

	public Shader fogShader;

	private Material fogMaterial;

	private Camera transparentCam;

	private void Start()
	{
		fogMaterial = CreateMaterial(fogShader, fogMaterial, checkShaderSupport: true);
		CheckSupport(needDepth: true);
		GameObject gameObject = new GameObject("Transparent Camera");
		gameObject.transform.parent = transform;
		transparentCam = gameObject.AddComponent<Camera>();
		transparentCam.CopyFrom(GetComponent<Camera>());
		transparentCam.depth = 10f;
		transparentCam.depthTextureMode = DepthTextureMode.None;
		transparentCam.clearFlags = CameraClearFlags.Nothing;
		transparentCam.cullingMask = 1 << LayerMask.NameToLayer("Logic");
		GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("Logic"));
	}

	private void OnEnable()
	{
		RenderSettings.fog = false;
		GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
	}

	private void OnDisable()
	{
		RenderSettings.fog = true;
		RenderSettings.fogColor = GetComponent<Camera>().backgroundColor;
		RenderSettings.fogStartDistance = startDistance;
		RenderSettings.fogEndDistance = GetComponent<Camera>().farClipPlane;
		GetComponent<Camera>().depthTextureMode = DepthTextureMode.None;
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		fogMaterial = CreateMaterial(fogShader, fogMaterial, checkShaderSupport: false);
		float nearClipPlane = GetComponent<Camera>().nearClipPlane;
		float farClipPlane = GetComponent<Camera>().farClipPlane;
		Matrix4x4 identity = Matrix4x4.identity;
		float num = GetComponent<Camera>().fieldOfView * 0.5f;
		Vector3 vector = GetComponent<Camera>().transform.right * nearClipPlane * Mathf.Tan(num * ((float)Math.PI / 180f)) * GetComponent<Camera>().aspect;
		Vector3 vector2 = GetComponent<Camera>().transform.up * nearClipPlane * Mathf.Tan(num * ((float)Math.PI / 180f));
		Vector3 vector3 = GetComponent<Camera>().transform.forward * nearClipPlane - vector + vector2;
		float num2 = vector3.magnitude * farClipPlane / nearClipPlane;
		vector3.Normalize();
		vector3 *= num2;
		Vector3 vector4 = GetComponent<Camera>().transform.forward * nearClipPlane + vector + vector2;
		vector4.Normalize();
		vector4 *= num2;
		Vector3 vector5 = GetComponent<Camera>().transform.forward * nearClipPlane + vector - vector2;
		vector5.Normalize();
		vector5 *= num2;
		Vector3 vector6 = GetComponent<Camera>().transform.forward * nearClipPlane - vector - vector2;
		vector6.Normalize();
		vector6 *= num2;
		identity.SetRow(0, vector3);
		identity.SetRow(1, vector4);
		identity.SetRow(2, vector5);
		identity.SetRow(3, vector6);
		float f = 0.05f;
		float value = (0f - Mathf.Log(f)) / (farClipPlane + (0f - startDistance));
		fogMaterial.SetMatrix("_FrustumCornersWS", identity);
		fogMaterial.SetFloat("_StartDistance", 0f - startDistance);
		fogMaterial.SetFloat("_Density", value);
		fogMaterial.SetTexture("_SkyMap", skyboxCubemap);
		CustomGraphicsBlit(source, destination, fogMaterial);
	}

	private void NotSupported()
	{
		Debug.LogError("The image effect " + ToString() + "is not supported on this platform!");
		enabled = false;
	}

	private bool CheckSupport(bool needDepth)
	{
		if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures)
		{
			NotSupported();
			return false;
		}
		if (needDepth && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
		{
			NotSupported();
			return false;
		}
		return true;
	}

	private Material CreateMaterial(Shader s, Material m2Create, bool checkShaderSupport)
	{
		if ((bool)m2Create && m2Create.shader == s)
		{
			return m2Create;
		}
		if (!s)
		{
			Debug.Log("Missing shader in " + ToString());
			enabled = false;
			return null;
		}
		if (!s.isSupported)
		{
			if (checkShaderSupport)
			{
				Debug.LogError("The shader " + s.ToString() + " on effect " + ToString() + " is not supported on this platform!");
				NotSupported();
			}
			return null;
		}
		m2Create = new Material(s);
		m2Create.hideFlags = HideFlags.DontSave;
		if ((bool)m2Create)
		{
			return m2Create;
		}
		return null;
	}

	private static void CustomGraphicsBlit(RenderTexture source, RenderTexture dest, Material fxMaterial)
	{
		RenderTexture.active = dest;
		fxMaterial.SetTexture("_MainTex", source);
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
