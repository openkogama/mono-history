using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WaterBase))]
[ExecuteInEditMode]
public class PlanarReflection : MonoBehaviour
{
	public LayerMask reflectionMask;

	public bool reflectSkybox;

	public Color clearColor = Color.grey;

	public string reflectionSampler = "_ReflectionTex";

	public float clipPlaneOffset = 0.07f;

	private Vector3 oldpos = Vector3.zero;

	private Camera reflectionCamera;

	private Material sharedMaterial;

	private Dictionary<Camera, bool> helperCameras;

	public PlanarReflection()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
	}

	public void Start()
	{
		sharedMaterial = ((WaterBase)(object)((Component)this).gameObject.GetComponent(typeof(WaterBase))).sharedMaterial;
	}

	private Camera CreateReflectionCameraFor(Camera cam)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected Obj, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		string text = ((Object)((Component)this).gameObject).name + "Reflection" + ((Object)cam).name;
		GameObject val = GameObject.Find(text);
		if (!Object.op_Implicit((Object)(object)val))
		{
			val = new GameObject(text, new Type[1] { typeof(Camera) });
		}
		if (!Object.op_Implicit((Object)(object)val.GetComponent(typeof(Camera))))
		{
			val.AddComponent(typeof(Camera));
		}
		Camera camera = val.camera;
		camera.backgroundColor = clearColor;
		camera.clearFlags = (CameraClearFlags)(reflectSkybox ? 1 : 2);
		SetStandardCameraParameter(camera, reflectionMask);
		if (!Object.op_Implicit((Object)(object)camera.targetTexture))
		{
			camera.targetTexture = CreateTextureFor(cam);
		}
		return camera;
	}

	private void SetStandardCameraParameter(Camera cam, LayerMask mask)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		cam.cullingMask = LayerMask.op_Implicit(mask) & ~(1 << LayerMask.NameToLayer("Water"));
		cam.backgroundColor = Color.black;
		((Behaviour)cam).enabled = false;
	}

	private RenderTexture CreateTextureFor(Camera cam)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected Obj, but got Unknown
		RenderTexture val = new RenderTexture(Mathf.FloorToInt(cam.pixelWidth * 0.5f), Mathf.FloorToInt(cam.pixelHeight * 0.5f), 24);
		((Object)val).hideFlags = (HideFlags)4;
		return val;
	}

	public void RenderHelpCameras(Camera currentCam)
	{
		if (helperCameras == null)
		{
			helperCameras = new Dictionary<Camera, bool>();
		}
		if (!helperCameras.ContainsKey(currentCam))
		{
			helperCameras.Add(currentCam, value: false);
		}
		if (!helperCameras[currentCam])
		{
			if (!Object.op_Implicit((Object)(object)reflectionCamera))
			{
				reflectionCamera = CreateReflectionCameraFor(currentCam);
			}
			RenderReflectionFor(currentCam, reflectionCamera);
			helperCameras[currentCam] = true;
		}
	}

	public void LateUpdate()
	{
		if (helperCameras != null)
		{
			helperCameras.Clear();
		}
	}

	public void WaterTileBeingRendered(Transform tr, Camera currentCam)
	{
		RenderHelpCameras(currentCam);
		if (Object.op_Implicit((Object)(object)reflectionCamera) && Object.op_Implicit((Object)(object)sharedMaterial))
		{
			sharedMaterial.SetTexture(reflectionSampler, (Texture)(object)reflectionCamera.targetTexture);
		}
	}

	public void OnEnable()
	{
		Shader.EnableKeyword("WATER_REFLECTIVE");
		Shader.DisableKeyword("WATER_SIMPLE");
	}

	public void OnDisable()
	{
		Shader.EnableKeyword("WATER_SIMPLE");
		Shader.DisableKeyword("WATER_REFLECTIVE");
	}

	private void RenderReflectionFor(Camera cam, Camera reflectCamera)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Expected Obj, but got Unknown
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Expected Obj, but got Unknown
		if (!Object.op_Implicit((Object)(object)reflectCamera) || (Object.op_Implicit((Object)(object)sharedMaterial) && !sharedMaterial.HasProperty(reflectionSampler)))
		{
			return;
		}
		reflectCamera.cullingMask = LayerMask.op_Implicit(reflectionMask) & ~(1 << LayerMask.NameToLayer("Water"));
		SaneCameraSettings(reflectCamera);
		reflectCamera.backgroundColor = clearColor;
		reflectCamera.clearFlags = (CameraClearFlags)(reflectSkybox ? 1 : 2);
		if (reflectSkybox && Object.op_Implicit((Object)(object)((Component)cam).gameObject.GetComponent(typeof(Skybox))))
		{
			Skybox val = (Skybox)((Component)reflectCamera).gameObject.GetComponent(typeof(Skybox));
			if (!Object.op_Implicit((Object)(object)val))
			{
				val = (Skybox)((Component)reflectCamera).gameObject.AddComponent(typeof(Skybox));
			}
			val.material = ((Skybox)((Component)cam).GetComponent(typeof(Skybox))).material;
		}
		GL.SetRevertBackfacing(true);
		Transform transform = ((Component)this).transform;
		Vector3 eulerAngles = ((Component)cam).transform.eulerAngles;
		((Component)reflectCamera).transform.eulerAngles = new Vector3(0f - eulerAngles.x, eulerAngles.y, eulerAngles.z);
		((Component)reflectCamera).transform.position = ((Component)cam).transform.position;
		Vector3 position = ((Component)transform).transform.position;
		position.y = transform.position.y;
		Vector3 up = ((Component)transform).transform.up;
		float num = 0f - Vector3.Dot(up, position) - clipPlaneOffset;
		Vector4 plane = new Vector4(up.x, up.y, up.z, num);
		Matrix4x4 val2 = Matrix4x4.zero;
		val2 = CalculateReflectionMatrix(val2, plane);
		oldpos = ((Component)cam).transform.position;
		Vector3 position2 = val2.MultiplyPoint(oldpos);
		reflectCamera.worldToCameraMatrix = cam.worldToCameraMatrix * val2;
		Vector4 clipPlane = CameraSpacePlane(reflectCamera, position, up, 1f);
		Matrix4x4 projectionMatrix = cam.projectionMatrix;
		projectionMatrix = CalculateObliqueMatrix(projectionMatrix, clipPlane);
		reflectCamera.projectionMatrix = projectionMatrix;
		((Component)reflectCamera).transform.position = position2;
		Vector3 eulerAngles2 = ((Component)cam).transform.eulerAngles;
		((Component)reflectCamera).transform.eulerAngles = new Vector3(0f - eulerAngles2.x, eulerAngles2.y, eulerAngles2.z);
		reflectCamera.Render();
		GL.SetRevertBackfacing(false);
	}

	private void SaneCameraSettings(Camera helperCam)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		helperCam.depthTextureMode = (DepthTextureMode)0;
		helperCam.backgroundColor = Color.black;
		helperCam.clearFlags = (CameraClearFlags)2;
		helperCam.renderingPath = (RenderingPath)1;
	}

	private static Matrix4x4 CalculateObliqueMatrix(Matrix4x4 projection, Vector4 clipPlane)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		Vector4 val = projection.inverse * new Vector4(sgn(clipPlane.x), sgn(clipPlane.y), 1f, 1f);
		Vector4 val2 = clipPlane * (2f / Vector4.Dot(clipPlane, val));
		projection[2] = val2.x - projection[3];
		projection[6] = val2.y - projection[7];
		projection[10] = val2.z - projection[11];
		projection[14] = val2.w - projection[15];
		return projection;
	}

	private static Matrix4x4 CalculateReflectionMatrix(Matrix4x4 reflectionMat, Vector4 plane)
	{
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
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
		return reflectionMat;
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
		Vector3 val = pos + normal * clipPlaneOffset;
		Matrix4x4 worldToCameraMatrix = cam.worldToCameraMatrix;
		Vector3 val2 = worldToCameraMatrix.MultiplyPoint(val);
		Vector3 val3 = worldToCameraMatrix.MultiplyVector(normal);
		Vector3 val4 = val3.normalized * sideSign;
		return new Vector4(val4.x, val4.y, val4.z, 0f - Vector3.Dot(val2, val4));
	}
}
