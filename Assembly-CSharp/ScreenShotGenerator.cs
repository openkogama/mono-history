using System;
using System.Collections;
using UnityEngine;

public class ScreenShotGenerator : MonoBehaviour
{
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

	private const int width = 512;

	private const int height = 512;

	private static bool generating = false;

	private static readonly int renderLayers = (1 << LayerMask.NameToLayer("CamRotateTarget")) | (1 << LayerMask.NameToLayer("PlayerSelected"));

	private static ScreenShotGenerator CreateInstance()
	{
		GameObject gameObject = new GameObject("ScreenShotGenerator");
		return gameObject.AddComponent<ScreenShotGenerator>();
	}

	public static void Generate(GameObject obj, Vector3 cameraOffset, Vector3 lookAtOffset, Action<byte[]> generatedScreenShotPNGCallback, bool cloneObject = false)
	{
		ScreenShotGenerator screenShotGenerator = CreateInstance();
		GameObject gameObject = obj;
		if (cloneObject)
		{
			Vector3 position = new Vector3(1000f, 1000f, 1000f);
			Quaternion rotation = Quaternion.Euler(0f, 180f, 0f);
			gameObject = UnityEngine.Object.Instantiate(obj, position, rotation);
			Behaviour[] componentsInChildren = gameObject.GetComponentsInChildren<Behaviour>();
			Behaviour[] array = componentsInChildren;
			foreach (Behaviour behaviour in array)
			{
				behaviour.enabled = false;
			}
		}
		screenShotGenerator.StartGenerate(gameObject, cameraOffset, lookAtOffset, generatedScreenShotPNGCallback, cloneObject);
	}

	public static void Generate(GameObject obj, Vector3 cameraOffset, Vector3 lookAtOffset, Action<Texture2D> generatedScreenShotTexCallback, bool cloneObject = false)
	{
		ScreenShotGenerator screenShotGenerator = CreateInstance();
		GameObject gameObject = obj;
		if (cloneObject)
		{
			Vector3 position = new Vector3(1000f, 1000f, 1000f);
			Quaternion rotation = Quaternion.Euler(0f, 180f, 0f);
			gameObject = UnityEngine.Object.Instantiate(obj, position, rotation);
			Behaviour[] componentsInChildren = gameObject.GetComponentsInChildren<Behaviour>();
			Behaviour[] array = componentsInChildren;
			foreach (Behaviour behaviour in array)
			{
				behaviour.enabled = false;
			}
		}
		screenShotGenerator.StartGenerate(gameObject, cameraOffset, lookAtOffset, generatedScreenShotTexCallback, cloneObject);
	}

	private void StartGenerate(GameObject obj, Vector3 cameraOffset, Vector3 lookAtOffset, Action<byte[]> generatedScreenShotPNGCallback, bool clonedObject)
	{
		if (generating)
		{
			Debug.LogError("Already generating a screenshot of " + targetObject);
			return;
		}
		Debug.Log("Start generate screenshot of " + obj);
		this.clonedObject = clonedObject;
		targetObject = obj;
		this.cameraOffset = cameraOffset;
		this.lookAtOffset = lookAtOffset;
		this.generatedScreenShotPNGCallback = generatedScreenShotPNGCallback;
		StartCoroutine(GenerateCoroutine(obj));
	}

	private void StartGenerate(GameObject obj, Vector3 cameraOffset, Vector3 lookAtOffset, Action<Texture2D> generatedScreenShotTexCallback, bool clonedObject)
	{
		if (generating)
		{
			Debug.LogError("Already generating a screenshot of " + targetObject);
			return;
		}
		Debug.Log("Start generate screenshot of " + obj);
		this.clonedObject = clonedObject;
		targetObject = obj;
		this.cameraOffset = cameraOffset;
		this.lookAtOffset = lookAtOffset;
		this.generatedScreenShotTexCallback = generatedScreenShotTexCallback;
		StartCoroutine(GenerateCoroutine(obj));
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
		LayerUtil.SetLayerRecursively(targetObject.transform, renderLayers, LayerMask.NameToLayer("Preview"));
		yield return new WaitForEndOfFrame();
		GenerateTexture();
		generating = false;
	}

	private void GenerateTexture()
	{
		Debug.Log("Generate screen shot");
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
		if (clonedObject)
		{
			UnityEngine.Object.Destroy(targetObject);
		}
		if (generatedScreenShotTexCallback == null)
		{
			UnityEngine.Object.Destroy(genTexture);
		}
		UnityEngine.Object.Destroy(gameObject);
		UnityEngine.Object.Destroy(genRenderTexture);
	}

	private void InitCamera(int width, int height)
	{
		if (genTexture == null)
		{
			genTexture = new Texture2D(width, height, TextureFormat.ARGB32, mipChain: false);
		}
		shotCamera = gameObject.AddComponent<Camera>();
		shotCamera.clearFlags = CameraClearFlags.Skybox;
		shotCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
		shotCamera.fieldOfView = 35f;
		shotCamera.depth = -2f;
		shotCamera.aspect = 1f;
		shotCamera.cullingMask = 1 << LayerMask.NameToLayer("Preview");
		genRenderTexture = new RenderTexture(width, height, 24);
		shotCamera.targetTexture = genRenderTexture;
		Vector3 position = Translate(targetBounds.center, targetObject.transform, cameraOffset);
		Vector3 worldPosition = Translate(targetBounds.center, targetObject.transform, lookAtOffset);
		shotCamera.transform.position = position;
		shotCamera.transform.LookAt(worldPosition);
	}

	private Vector3 Translate(Vector3 pos, Transform relativeTo, Vector3 translation)
	{
		Vector3 vector = relativeTo.right * translation.x;
		Vector3 vector2 = relativeTo.forward * translation.z;
		Vector3 vector3 = relativeTo.up * translation.y;
		pos = pos + vector + vector2 + vector3;
		return pos;
	}

	private void OnPreRender()
	{
	}
}
