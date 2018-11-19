using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace AntiHack;

public class MaterialPlaneRenderer : MonoBehaviour
{
	[SerializeField]
	private Camera cam;

	[SerializeField]
	private Mesh plane;

	[SerializeField]
	private Material material;

	[SerializeField]
	private List<Texture2D> textures = new List<Texture2D>();

	private List<byte> hashes = new List<byte>(1);

	private RenderTextureDescriptor renderTextureDesc = new RenderTextureDescriptor
	{
		dimension = TextureDimension.Tex2D,
		width = 16,
		height = 16,
		msaaSamples = 1,
		bindMS = false,
		colorFormat = RenderTextureFormat.Default,
		depthBufferBits = 0,
		sRGB = false,
		useMipMap = false,
		autoGenerateMips = false,
		volumeDepth = 1
	};

	private RenderTexture renderTexture;

	private static bool errorReportSent;

	protected void OnValidate()
	{
		if (cam == null)
		{
			cam = GetComponent<Camera>();
		}
		cam.enabled = false;
		transform.position = Vector3.zero;
		transform.rotation = Quaternion.Euler(90f, 0f, 0f);
	}

	protected void OnPostRender()
	{
		OnPostRender_GenerateNewHashes();
	}

	public void Initialize()
	{
		renderTexture = new RenderTexture(renderTextureDesc);
		renderTexture.wrapMode = TextureWrapMode.Clamp;
		renderTexture.filterMode = FilterMode.Point;
		renderTexture.anisoLevel = 1;
		GenerateNewHashes();
	}

	public bool VerifyTextureIntegrity()
	{
		byte[] array = hashes.ToArray();
		GenerateNewHashes();
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != hashes[i] && !errorReportSent)
			{
				errorReportSent = true;
				StatHatWrapper.Count("TextureIntegrityBreached", 1);
				Debug.Log("Texture integrity breached.\n textures[" + i + "], \"" + textures[i].name + "\" has been changed.");
				CheatHandling.TextureHackDetected();
				return false;
			}
		}
		return true;
	}

	private void GenerateNewHashes()
	{
		hashes.Clear();
		for (int i = 0; i < textures.Count; i++)
		{
			Texture2D mainTexture = textures[i];
			material.mainTexture = mainTexture;
			cam.Render();
		}
	}

	private void OnPostRender_GenerateNewHashes()
	{
		if (material.SetPass(0))
		{
			Graphics.DrawMeshNow(plane, Vector3.zero, Quaternion.identity);
			hashes.Add(CalculateHash(renderTexture));
		}
		else
		{
			Debug.LogError("MaterialPlaneRenderer failed to set shader pass.");
		}
	}

	private byte CalculateHash(RenderTexture renderTexture)
	{
		Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, mipChain: false);
		texture2D.ReadPixels(new Rect(0f, 0f, renderTexture.width, renderTexture.height), 0, 0);
		texture2D.Apply();
		return CalculateHash(texture2D);
	}

	private byte CalculateHash(Texture2D texture)
	{
		byte b = 0;
		Color32[] pixels = texture.GetPixels32();
		for (int i = 0; i < pixels.Length; i++)
		{
			Color32 color = pixels[i];
			b += color.r;
			b += color.g;
			b += color.b;
		}
		return b;
	}
}
