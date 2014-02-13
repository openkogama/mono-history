using System;
using System.Collections;
using UnityEngine;

public class ScreenShotGenerator : MonoBehaviour
{
	private const int width = 512;

	private const int height = 512;

	public Texture2D genTexture;

	private RenderTexture genRenderTexture;

	private Camera shotCamera;

	private Action<byte[]> generatedScreenShotPNGCallback;

	private Action<Texture2D> generatedScreenShotTexCallback;

	private Vector3 cameraOffset = new Vector3(-1f, 0.5f, 2f);

	private Vector3 lookAtOffset = new Vector3(0f, 0f, 0f);

	private bool clonedObject;

	private GameObject targetObject;

	private Bounds targetBounds;

	private static bool generating;

	public ScreenShotGenerator()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
	}

	private static ScreenShotGenerator CreateInstance()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected Obj, but got Unknown
		GameObject val = new GameObject("ScreenShotGenerator");
		return val.AddComponent<ScreenShotGenerator>();
	}

	public static void Generate(GameObject obj, Vector3 cameraOffset, Vector3 lookAtOffset, Action<byte[]> generatedScreenShotPNGCallback, bool cloneObject = false)
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected Obj, but got Unknown
		ScreenShotGenerator screenShotGenerator = CreateInstance();
		GameObject val = obj;
		if (cloneObject)
		{
			Vector3 val2 = new Vector3(1000f, 1000f, 1000f);
			Quaternion val3 = Quaternion.Euler(0f, 180f, 0f);
			val = (GameObject)Object.Instantiate((Object)(object)obj, val2, val3);
			Behaviour[] componentsInChildren = val.GetComponentsInChildren<Behaviour>();
			Behaviour[] array = componentsInChildren;
			foreach (Behaviour val4 in array)
			{
				val4.enabled = false;
			}
		}
		screenShotGenerator.StartGenerate(val, cameraOffset, lookAtOffset, generatedScreenShotPNGCallback, cloneObject);
	}

	public static void Generate(GameObject obj, Vector3 cameraOffset, Vector3 lookAtOffset, Action<Texture2D> generatedScreenShotTexCallback, bool cloneObject = false)
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected Obj, but got Unknown
		ScreenShotGenerator screenShotGenerator = CreateInstance();
		GameObject val = obj;
		if (cloneObject)
		{
			Vector3 val2 = new Vector3(1000f, 1000f, 1000f);
			Quaternion val3 = Quaternion.Euler(0f, 180f, 0f);
			val = (GameObject)Object.Instantiate((Object)(object)obj, val2, val3);
			Behaviour[] componentsInChildren = val.GetComponentsInChildren<Behaviour>();
			Behaviour[] array = componentsInChildren;
			foreach (Behaviour val4 in array)
			{
				val4.enabled = false;
			}
		}
		screenShotGenerator.StartGenerate(val, cameraOffset, lookAtOffset, generatedScreenShotTexCallback, cloneObject);
	}

	private void StartGenerate(GameObject obj, Vector3 cameraOffset, Vector3 lookAtOffset, Action<byte[]> generatedScreenShotPNGCallback, bool clonedObject)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (generating)
		{
			Debug.LogError((object)("Already generating a screenshot of " + targetObject));
			return;
		}
		Debug.Log((object)("Start generate screenshot of " + obj));
		this.clonedObject = clonedObject;
		targetObject = obj;
		this.cameraOffset = cameraOffset;
		this.lookAtOffset = lookAtOffset;
		this.generatedScreenShotPNGCallback = generatedScreenShotPNGCallback;
		((MonoBehaviour)this).StartCoroutine(GenerateCoroutine(obj));
	}

	private void StartGenerate(GameObject obj, Vector3 cameraOffset, Vector3 lookAtOffset, Action<Texture2D> generatedScreenShotTexCallback, bool clonedObject)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (generating)
		{
			Debug.LogError((object)("Already generating a screenshot of " + targetObject));
			return;
		}
		Debug.Log((object)("Start generate screenshot of " + obj));
		this.clonedObject = clonedObject;
		targetObject = obj;
		this.cameraOffset = cameraOffset;
		this.lookAtOffset = lookAtOffset;
		this.generatedScreenShotTexCallback = generatedScreenShotTexCallback;
		((MonoBehaviour)this).StartCoroutine(GenerateCoroutine(obj));
	}

	private IEnumerator GenerateCoroutine(GameObject obj)
	{
		Bounds? bounds = SharedCubeFunctions.GetAxisAlignedBoundsRecursively(obj.transform);
		if (bounds.HasValue)
		{
			targetBounds = bounds.Value;
		}
		else
		{
			targetBounds = new Bounds(Vector3.zero, Vector3.one);
		}
		InitCamera(512, 512);
		LayerUtil.SetLayerRecursively(targetObject.transform, "Preview");
		yield return (object)new WaitForEndOfFrame();
		GenerateTexture();
		generating = false;
	}

	private void GenerateTexture()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		RenderTexture.active = genRenderTexture;
		genTexture.ReadPixels(new Rect(0f, 0f, 512f, 512f), 0, 0);
		genTexture.Apply();
		if (generatedScreenShotPNGCallback != null)
		{
			generatedScreenShotPNGCallback(genTexture.EncodeToPNG());
		}
		if (generatedScreenShotTexCallback != null)
		{
			generatedScreenShotTexCallback(genTexture);
		}
		shotCamera.targetTexture = null;
		RenderTexture.active = null;
		Object.DestroyImmediate((Object)(object)genRenderTexture);
		if (clonedObject)
		{
			Object.Destroy((Object)(object)targetObject);
		}
		if (generatedScreenShotTexCallback == null)
		{
			Object.Destroy((Object)(object)genTexture);
		}
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}

	private void InitCamera(int width, int height)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Expected Obj, but got Unknown
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected Obj, but got Unknown
		if ((Object)(object)genTexture == (Object)null)
		{
			genTexture = new Texture2D(width, height, (TextureFormat)5, false);
		}
		shotCamera = ((Component)this).gameObject.AddComponent<Camera>();
		shotCamera.clearFlags = (CameraClearFlags)1;
		shotCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
		shotCamera.fieldOfView = 35f;
		shotCamera.depth = -2f;
		shotCamera.aspect = 1f;
		shotCamera.cullingMask = 1 << LayerMask.NameToLayer("Preview");
		genRenderTexture = new RenderTexture(width, height, 24);
		shotCamera.targetTexture = genRenderTexture;
		Vector3 position = Translate(targetBounds.center, targetObject.transform, cameraOffset);
		Vector3 val = Translate(targetBounds.center, targetObject.transform, lookAtOffset);
		((Component)shotCamera).transform.position = position;
		((Component)shotCamera).transform.LookAt(val);
	}

	private Vector3 Translate(Vector3 pos, Transform relativeTo, Vector3 translation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = relativeTo.right * translation.x;
		Vector3 val2 = relativeTo.forward * translation.z;
		Vector3 val3 = relativeTo.up * translation.y;
		pos = pos + val + val2 + val3;
		return pos;
	}

	private void OnPreRender()
	{
	}
}
