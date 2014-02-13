using System;
using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class Water : MonoBehaviour
{
	public enum WaterMode
	{
		Simple,
		Reflective,
		Refractive
	}

	public WaterMode m_WaterMode = WaterMode.Refractive;

	public bool m_DisablePixelLights = true;

	public int m_TextureSize = 256;

	public float m_ClipPlaneOffset = 0.07f;

	public LayerMask m_ReflectLayers = LayerMask.op_Implicit(-1);

	public LayerMask m_RefractLayers = LayerMask.op_Implicit(-1);

	private Hashtable m_ReflectionCameras = new Hashtable();

	private Hashtable m_RefractionCameras = new Hashtable();

	private RenderTexture m_ReflectionTexture;

	private RenderTexture m_RefractionTexture;

	private WaterMode m_HardwareWaterSupport = WaterMode.Refractive;

	private int m_OldReflectionTextureSize;

	private int m_OldRefractionTextureSize;

	private static bool s_InsideWater;

	public Water()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
	}

	public void OnWillRenderObject()
	{
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		if (!((Behaviour)this).enabled || !Object.op_Implicit((Object)(object)((Component)this).renderer) || !Object.op_Implicit((Object)(object)((Component)this).renderer.sharedMaterial) || !((Component)this).renderer.enabled)
		{
			return;
		}
		Camera current = Camera.current;
		if (Object.op_Implicit((Object)(object)current) && !s_InsideWater)
		{
			s_InsideWater = true;
			m_HardwareWaterSupport = FindHardwareWaterSupport();
			WaterMode waterMode = GetWaterMode();
			CreateWaterObjects(current, out var reflectionCamera, out var refractionCamera);
			Vector3 position = ((Component)this).transform.position;
			Vector3 up = ((Component)this).transform.up;
			int pixelLightCount = QualitySettings.pixelLightCount;
			if (m_DisablePixelLights)
			{
				QualitySettings.pixelLightCount = 0;
			}
			UpdateCameraModes(current, reflectionCamera);
			UpdateCameraModes(current, refractionCamera);
			if (waterMode >= WaterMode.Reflective)
			{
				float num = 0f - Vector3.Dot(up, position) - m_ClipPlaneOffset;
				Vector4 plane = new Vector4(up.x, up.y, up.z, num);
				Matrix4x4 reflectionMat = Matrix4x4.zero;
				CalculateReflectionMatrix(ref reflectionMat, plane);
				Vector3 position2 = ((Component)current).transform.position;
				Vector3 position3 = reflectionMat.MultiplyPoint(position2);
				reflectionCamera.worldToCameraMatrix = current.worldToCameraMatrix * reflectionMat;
				Vector4 clipPlane = CameraSpacePlane(reflectionCamera, position, up, 1f);
				Matrix4x4 projection = current.projectionMatrix;
				CalculateObliqueMatrix(ref projection, clipPlane);
				reflectionCamera.projectionMatrix = projection;
				reflectionCamera.cullingMask = -17 & m_ReflectLayers.value;
				reflectionCamera.targetTexture = m_ReflectionTexture;
				GL.SetRevertBackfacing(true);
				((Component)reflectionCamera).transform.position = position3;
				Vector3 eulerAngles = ((Component)current).transform.eulerAngles;
				((Component)reflectionCamera).transform.eulerAngles = new Vector3(0f - eulerAngles.x, eulerAngles.y, eulerAngles.z);
				reflectionCamera.Render();
				((Component)reflectionCamera).transform.position = position2;
				GL.SetRevertBackfacing(false);
				((Component)this).renderer.sharedMaterial.SetTexture("_ReflectionTex", (Texture)(object)m_ReflectionTexture);
			}
			if (waterMode >= WaterMode.Refractive)
			{
				refractionCamera.worldToCameraMatrix = current.worldToCameraMatrix;
				Vector4 clipPlane2 = CameraSpacePlane(refractionCamera, position, up, -1f);
				Matrix4x4 projection2 = current.projectionMatrix;
				CalculateObliqueMatrix(ref projection2, clipPlane2);
				refractionCamera.projectionMatrix = projection2;
				refractionCamera.cullingMask = -17 & m_RefractLayers.value;
				refractionCamera.targetTexture = m_RefractionTexture;
				((Component)refractionCamera).transform.position = ((Component)current).transform.position;
				((Component)refractionCamera).transform.rotation = ((Component)current).transform.rotation;
				refractionCamera.Render();
				((Component)this).renderer.sharedMaterial.SetTexture("_RefractionTex", (Texture)(object)m_RefractionTexture);
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
			case WaterMode.Refractive:
				Shader.DisableKeyword("WATER_SIMPLE");
				Shader.DisableKeyword("WATER_REFLECTIVE");
				Shader.EnableKeyword("WATER_REFRACTIVE");
				break;
			}
			s_InsideWater = false;
		}
	}

	private void OnDisable()
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)m_ReflectionTexture))
		{
			Object.DestroyImmediate((Object)(object)m_ReflectionTexture);
			m_ReflectionTexture = null;
		}
		if (Object.op_Implicit((Object)(object)m_RefractionTexture))
		{
			Object.DestroyImmediate((Object)(object)m_RefractionTexture);
			m_RefractionTexture = null;
		}
		foreach (DictionaryEntry reflectionCamera in m_ReflectionCameras)
		{
			Object.DestroyImmediate((Object)(object)((Component)(Camera)reflectionCamera.Value).gameObject);
		}
		m_ReflectionCameras.Clear();
		foreach (DictionaryEntry refractionCamera in m_RefractionCameras)
		{
			Object.DestroyImmediate((Object)(object)((Component)(Camera)refractionCamera.Value).gameObject);
		}
		m_RefractionCameras.Clear();
	}

	private void Update()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)((Component)this).renderer))
		{
			Material sharedMaterial = ((Component)this).renderer.sharedMaterial;
			if (Object.op_Implicit((Object)(object)sharedMaterial))
			{
				Vector4 vector = sharedMaterial.GetVector("WaveSpeed");
				float num = sharedMaterial.GetFloat("_WaveScale");
				Vector4 val = new Vector4(num, num, num * 0.4f, num * 0.45f);
				double num2 = (double)Time.timeSinceLevelLoad / 20.0;
				Vector4 val2 = new Vector4((float)Math.IEEERemainder((double)(vector.x * val.x) * num2, 1.0), (float)Math.IEEERemainder((double)(vector.y * val.y) * num2, 1.0), (float)Math.IEEERemainder((double)(vector.z * val.z) * num2, 1.0), (float)Math.IEEERemainder((double)(vector.w * val.w) * num2, 1.0));
				sharedMaterial.SetVector("_WaveOffset", val2);
				sharedMaterial.SetVector("_WaveScale4", val);
				Bounds bounds = ((Component)this).renderer.bounds;
				Vector3 size = bounds.size;
				Vector3 val3 = new Vector3(size.x * val.x, size.z * val.y, 1f);
				Matrix4x4 val4 = Matrix4x4.TRS(new Vector3(val2.x, val2.y, 0f), Quaternion.identity, val3);
				sharedMaterial.SetMatrix("_WaveMatrix", val4);
				val3 = new Vector3(size.x * val.z, size.z * val.w, 1f);
				val4 = Matrix4x4.TRS(new Vector3(val2.z, val2.w, 0f), Quaternion.identity, val3);
				sharedMaterial.SetMatrix("_WaveMatrix2", val4);
			}
		}
	}

	private void UpdateCameraModes(Camera src, Camera dest)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Invalid comparison between Unknown and I4
		if ((Object)(object)dest == (Object)null)
		{
			return;
		}
		dest.clearFlags = src.clearFlags;
		dest.backgroundColor = src.backgroundColor;
		if ((int)src.clearFlags == 1)
		{
			Component component = ((Component)src).GetComponent(typeof(Skybox));
			Skybox val = (Skybox)(object)((component is Skybox) ? component : null);
			Component component2 = ((Component)dest).GetComponent(typeof(Skybox));
			Skybox val2 = (Skybox)(object)((component2 is Skybox) ? component2 : null);
			if (!Object.op_Implicit((Object)(object)val) || !Object.op_Implicit((Object)(object)val.material))
			{
				((Behaviour)val2).enabled = false;
			}
			else
			{
				((Behaviour)val2).enabled = true;
				val2.material = val.material;
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
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected Obj, but got Unknown
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Expected Obj, but got Unknown
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Expected Obj, but got Unknown
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Expected Obj, but got Unknown
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		WaterMode waterMode = GetWaterMode();
		reflectionCamera = null;
		refractionCamera = null;
		if (waterMode >= WaterMode.Reflective)
		{
			if (!Object.op_Implicit((Object)(object)m_ReflectionTexture) || m_OldReflectionTextureSize != m_TextureSize)
			{
				if (Object.op_Implicit((Object)(object)m_ReflectionTexture))
				{
					Object.DestroyImmediate((Object)(object)m_ReflectionTexture);
				}
				m_ReflectionTexture = new RenderTexture(m_TextureSize, m_TextureSize, 16);
				((Object)m_ReflectionTexture).name = "__WaterReflection" + ((Object)this).GetInstanceID();
				m_ReflectionTexture.isPowerOfTwo = true;
				((Object)m_ReflectionTexture).hideFlags = (HideFlags)4;
				m_OldReflectionTextureSize = m_TextureSize;
			}
			object? obj = m_ReflectionCameras[currentCamera];
			reflectionCamera = (Camera)((obj is Camera) ? obj : null);
			if (!Object.op_Implicit((Object)(object)reflectionCamera))
			{
				GameObject val = new GameObject("Water Refl Camera id" + ((Object)this).GetInstanceID() + " for " + ((Object)currentCamera).GetInstanceID(), new Type[2]
				{
					typeof(Camera),
					typeof(Skybox)
				});
				reflectionCamera = val.camera;
				((Behaviour)reflectionCamera).enabled = false;
				((Component)reflectionCamera).transform.position = ((Component)this).transform.position;
				((Component)reflectionCamera).transform.rotation = ((Component)this).transform.rotation;
				((Component)reflectionCamera).gameObject.AddComponent("FlareLayer");
				((Object)val).hideFlags = (HideFlags)13;
				m_ReflectionCameras[currentCamera] = reflectionCamera;
			}
		}
		if (waterMode < WaterMode.Refractive)
		{
			return;
		}
		if (!Object.op_Implicit((Object)(object)m_RefractionTexture) || m_OldRefractionTextureSize != m_TextureSize)
		{
			if (Object.op_Implicit((Object)(object)m_RefractionTexture))
			{
				Object.DestroyImmediate((Object)(object)m_RefractionTexture);
			}
			m_RefractionTexture = new RenderTexture(m_TextureSize, m_TextureSize, 16);
			((Object)m_RefractionTexture).name = "__WaterRefraction" + ((Object)this).GetInstanceID();
			m_RefractionTexture.isPowerOfTwo = true;
			((Object)m_RefractionTexture).hideFlags = (HideFlags)4;
			m_OldRefractionTextureSize = m_TextureSize;
		}
		object? obj2 = m_RefractionCameras[currentCamera];
		refractionCamera = (Camera)((obj2 is Camera) ? obj2 : null);
		if (!Object.op_Implicit((Object)(object)refractionCamera))
		{
			GameObject val2 = new GameObject("Water Refr Camera id" + ((Object)this).GetInstanceID() + " for " + ((Object)currentCamera).GetInstanceID(), new Type[2]
			{
				typeof(Camera),
				typeof(Skybox)
			});
			refractionCamera = val2.camera;
			((Behaviour)refractionCamera).enabled = false;
			((Component)refractionCamera).transform.position = ((Component)this).transform.position;
			((Component)refractionCamera).transform.rotation = ((Component)this).transform.rotation;
			((Component)refractionCamera).gameObject.AddComponent("FlareLayer");
			((Object)val2).hideFlags = (HideFlags)13;
			m_RefractionCameras[currentCamera] = refractionCamera;
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
		if (!SystemInfo.supportsRenderTextures || !Object.op_Implicit((Object)(object)((Component)this).renderer))
		{
			return WaterMode.Simple;
		}
		Material sharedMaterial = ((Component)this).renderer.sharedMaterial;
		if (!Object.op_Implicit((Object)(object)sharedMaterial))
		{
			return WaterMode.Simple;
		}
		string tag = sharedMaterial.GetTag("WATERMODE", false);
		if (tag == "Refractive")
		{
			return WaterMode.Refractive;
		}
		if (tag == "Reflective")
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
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = pos + normal * m_ClipPlaneOffset;
		Matrix4x4 worldToCameraMatrix = cam.worldToCameraMatrix;
		Vector3 val2 = worldToCameraMatrix.MultiplyPoint(val);
		Vector3 val3 = worldToCameraMatrix.MultiplyVector(normal);
		Vector3 val4 = val3.normalized * sideSign;
		return new Vector4(val4.x, val4.y, val4.z, 0f - Vector3.Dot(val2, val4));
	}

	private static void CalculateObliqueMatrix(ref Matrix4x4 projection, Vector4 clipPlane)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		Vector4 val = projection.inverse * new Vector4(sgn(clipPlane.x), sgn(clipPlane.y), 1f, 1f);
		Vector4 val2 = clipPlane * (2f / Vector4.Dot(clipPlane, val));
		projection[2] = val2.x - projection[3];
		projection[6] = val2.y - projection[7];
		projection[10] = val2.z - projection[11];
		projection[14] = val2.w - projection[15];
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
