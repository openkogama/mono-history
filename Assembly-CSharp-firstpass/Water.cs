using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Water : MonoBehaviour
{
	public enum WaterMode
	{
		Simple,
		Reflective
	}

	public WaterMode m_WaterMode;

	public bool m_DisablePixelLights = true;

	public int m_TextureSize = 256;

	public float m_ClipPlaneOffset = 0.07f;

	public LayerMask m_ReflectLayers = -1;

	private Dictionary<object, object> m_ReflectionCameras = new Dictionary<object, object>();

	private RenderTexture m_ReflectionTexture;

	private WaterMode m_HardwareWaterSupport;

	private int m_OldReflectionTextureSize;

	private static bool s_InsideWater;

	[SerializeField]
	private Renderer meshRenderer;

	public Renderer Renderer => meshRenderer;

	public void OnWillRenderObject()
	{
		if (!enabled || !meshRenderer || !meshRenderer.sharedMaterial || !meshRenderer.enabled)
		{
			return;
		}
		Camera current = Camera.current;
		if ((bool)current && !s_InsideWater)
		{
			s_InsideWater = true;
			m_HardwareWaterSupport = FindHardwareWaterSupport();
			WaterMode waterMode = GetWaterMode();
			CreateWaterObjects(current, out var reflectionCamera, out var refractionCamera);
			Vector3 position = transform.position;
			Vector3 up = transform.up;
			int pixelLightCount = QualitySettings.pixelLightCount;
			if (m_DisablePixelLights)
			{
				QualitySettings.pixelLightCount = 0;
			}
			UpdateCameraModes(current, reflectionCamera);
			UpdateCameraModes(current, refractionCamera);
			if (waterMode >= WaterMode.Reflective)
			{
				float w = 0f - Vector3.Dot(up, position) - m_ClipPlaneOffset;
				Vector4 plane = new Vector4(up.x, up.y, up.z, w);
				Matrix4x4 reflectionMat = Matrix4x4.zero;
				CalculateReflectionMatrix(ref reflectionMat, plane);
				Vector3 position2 = current.transform.position;
				Vector3 position3 = reflectionMat.MultiplyPoint(position2);
				reflectionCamera.worldToCameraMatrix = current.worldToCameraMatrix * reflectionMat;
				Vector4 clipPlane = CameraSpacePlane(reflectionCamera, position, up, 1f);
				Matrix4x4 projectionMatrix = current.CalculateObliqueMatrix(clipPlane);
				reflectionCamera.projectionMatrix = projectionMatrix;
				reflectionCamera.cullingMask = -17 & m_ReflectLayers.value;
				reflectionCamera.targetTexture = m_ReflectionTexture;
				GL.invertCulling = true;
				reflectionCamera.transform.position = position3;
				Vector3 eulerAngles = current.transform.eulerAngles;
				reflectionCamera.transform.eulerAngles = new Vector3(0f - eulerAngles.x, eulerAngles.y, eulerAngles.z);
				reflectionCamera.Render();
				reflectionCamera.transform.position = position2;
				GL.invertCulling = false;
				meshRenderer.sharedMaterial.SetTexture("_ReflectionTex", m_ReflectionTexture);
			}
			if (m_DisablePixelLights)
			{
				QualitySettings.pixelLightCount = pixelLightCount;
			}
			switch (waterMode)
			{
			case WaterMode.Simple:
				Shader.EnableKeyword("WATER_SIMPLE");
				Shader.DisableKeyword("WATER_REFLECTIVE");
				Shader.DisableKeyword("WATER_REFRACTIVE");
				break;
			case WaterMode.Reflective:
				Shader.DisableKeyword("WATER_SIMPLE");
				Shader.EnableKeyword("WATER_REFLECTIVE");
				Shader.DisableKeyword("WATER_REFRACTIVE");
				break;
			}
			s_InsideWater = false;
		}
	}

	private bool IsNanCheck(Vector3 v)
	{
		if (float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z))
		{
			return true;
		}
		return false;
	}

	private void OnDisable()
	{
		if ((bool)m_ReflectionTexture)
		{
			m_ReflectionTexture.Release();
			m_ReflectionTexture = null;
		}
		foreach (KeyValuePair<object, object> reflectionCamera in m_ReflectionCameras)
		{
			UnityEngine.Object.Destroy(((Camera)reflectionCamera.Value).gameObject);
		}
		m_ReflectionCameras.Clear();
	}

	private void Update()
	{
		if ((bool)meshRenderer)
		{
			Material sharedMaterial = meshRenderer.sharedMaterial;
			if ((bool)sharedMaterial)
			{
				Vector4 vector = sharedMaterial.GetVector("WaveSpeed");
				float num = sharedMaterial.GetFloat("_WaveScale");
				Vector4 value = new Vector4(num, num, num * 0.4f, num * 0.45f);
				double num2 = (double)Time.timeSinceLevelLoad / 20.0;
				Vector4 value2 = new Vector4((float)Math.IEEERemainder((double)(vector.x * value.x) * num2, 1.0), (float)Math.IEEERemainder((double)(vector.y * value.y) * num2, 1.0), (float)Math.IEEERemainder((double)(vector.z * value.z) * num2, 1.0), (float)Math.IEEERemainder((double)(vector.w * value.w) * num2, 1.0));
				sharedMaterial.SetVector("_WaveOffset", value2);
				sharedMaterial.SetVector("_WaveScale4", value);
				Vector3 size = meshRenderer.bounds.size;
				Matrix4x4 value3 = Matrix4x4.TRS(s: new Vector3(size.x * value.x, size.z * value.y, 1f), pos: new Vector3(value2.x, value2.y, 0f), q: Quaternion.identity);
				sharedMaterial.SetMatrix("_WaveMatrix", value3);
				value3 = Matrix4x4.TRS(s: new Vector3(size.x * value.z, size.z * value.w, 1f), pos: new Vector3(value2.z, value2.w, 0f), q: Quaternion.identity);
				sharedMaterial.SetMatrix("_WaveMatrix2", value3);
			}
		}
	}

	private void UpdateCameraModes(Camera src, Camera dest)
	{
		if (dest == null)
		{
			return;
		}
		dest.clearFlags = src.clearFlags;
		dest.backgroundColor = src.backgroundColor;
		if (src.clearFlags == CameraClearFlags.Skybox)
		{
			Skybox skybox = src.GetComponent(typeof(Skybox)) as Skybox;
			Skybox skybox2 = dest.GetComponent(typeof(Skybox)) as Skybox;
			if (!skybox || !skybox.material)
			{
				skybox2.enabled = false;
			}
			else
			{
				skybox2.enabled = true;
				skybox2.material = skybox.material;
			}
		}
		dest.farClipPlane = src.farClipPlane;
		dest.nearClipPlane = src.nearClipPlane;
		dest.orthographic = src.orthographic;
		dest.fieldOfView = src.fieldOfView;
		dest.aspect = src.aspect;
		dest.orthographicSize = src.orthographicSize;
	}

	private void CreateWaterObjects(Camera currentCamera, out Camera reflectionCamera, out Camera refractionCamera)
	{
		WaterMode waterMode = GetWaterMode();
		reflectionCamera = null;
		refractionCamera = null;
		if (waterMode < WaterMode.Reflective)
		{
			return;
		}
		if (!m_ReflectionTexture || m_OldReflectionTextureSize != m_TextureSize)
		{
			if ((bool)m_ReflectionTexture)
			{
				UnityEngine.Object.DestroyImmediate(m_ReflectionTexture);
			}
			m_ReflectionTexture = new RenderTexture(m_TextureSize, m_TextureSize, 16);
			m_ReflectionTexture.name = "__WaterReflection" + GetInstanceID();
			m_ReflectionTexture.isPowerOfTwo = true;
			m_ReflectionTexture.hideFlags = HideFlags.DontSave;
			m_OldReflectionTextureSize = m_TextureSize;
		}
		if (m_ReflectionCameras.ContainsKey(currentCamera))
		{
			reflectionCamera = m_ReflectionCameras[currentCamera] as Camera;
		}
		if (!reflectionCamera)
		{
			GameObject gameObject = new GameObject("Water Refl Camera id" + GetInstanceID() + " for " + currentCamera.GetInstanceID(), typeof(Camera), typeof(Skybox));
			reflectionCamera = gameObject.GetComponent<Camera>();
			reflectionCamera.enabled = false;
			reflectionCamera.transform.position = transform.position;
			reflectionCamera.transform.rotation = transform.rotation;
			gameObject.hideFlags = HideFlags.HideAndDontSave;
			m_ReflectionCameras[currentCamera] = reflectionCamera;
		}
	}

	private WaterMode GetWaterMode()
	{
		if (m_HardwareWaterSupport < m_WaterMode)
		{
			return m_HardwareWaterSupport;
		}
		return m_WaterMode;
	}

	private WaterMode FindHardwareWaterSupport()
	{
		if (!meshRenderer)
		{
			return WaterMode.Simple;
		}
		Material sharedMaterial = meshRenderer.sharedMaterial;
		if (!sharedMaterial)
		{
			return WaterMode.Simple;
		}
		string text = sharedMaterial.GetTag("WATERMODE", searchFallbacks: false);
		if (text == "Reflective")
		{
			return WaterMode.Reflective;
		}
		return WaterMode.Simple;
	}

	private static float sgn(float a)
	{
		if (a > 0f)
		{
			return 1f;
		}
		if (a < 0f)
		{
			return -1f;
		}
		return 0f;
	}

	private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
	{
		Vector3 point = pos + normal * m_ClipPlaneOffset;
		Matrix4x4 worldToCameraMatrix = cam.worldToCameraMatrix;
		Vector3 lhs = worldToCameraMatrix.MultiplyPoint(point);
		Vector3 rhs = worldToCameraMatrix.MultiplyVector(normal).normalized * sideSign;
		return new Vector4(rhs.x, rhs.y, rhs.z, 0f - Vector3.Dot(lhs, rhs));
	}

	private static void CalculateObliqueMatrix(ref Matrix4x4 projection, Vector4 clipPlane)
	{
		Vector4 b = projection.inverse * new Vector4(sgn(clipPlane.x), sgn(clipPlane.y), 1f, 1f);
		Vector4 vector = clipPlane * (2f / Vector4.Dot(clipPlane, b));
		projection[2] = vector.x - projection[3];
		projection[6] = vector.y - projection[7];
		projection[10] = vector.z - projection[11];
		projection[14] = vector.w - projection[15];
	}

	private static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
	{
		reflectionMat.m00 = 1f - 2f * plane[0] * plane[0];
		reflectionMat.m01 = -2f * plane[0] * plane[1];
		reflectionMat.m02 = -2f * plane[0] * plane[2];
		reflectionMat.m03 = -2f * plane[3] * plane[0];
		reflectionMat.m10 = -2f * plane[1] * plane[0];
		reflectionMat.m11 = 1f - 2f * plane[1] * plane[1];
		reflectionMat.m12 = -2f * plane[1] * plane[2];
		reflectionMat.m13 = -2f * plane[3] * plane[1];
		reflectionMat.m20 = -2f * plane[2] * plane[0];
		reflectionMat.m21 = -2f * plane[2] * plane[1];
		reflectionMat.m22 = 1f - 2f * plane[2] * plane[2];
		reflectionMat.m23 = -2f * plane[3] * plane[2];
		reflectionMat.m30 = 0f;
		reflectionMat.m31 = 0f;
		reflectionMat.m32 = 0f;
		reflectionMat.m33 = 1f;
	}
}
