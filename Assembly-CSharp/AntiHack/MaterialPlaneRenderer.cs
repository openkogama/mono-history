using System.Collections.Generic;
using UnityEngine;

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
	private RenderTexture renderTexture;

	[SerializeField]
	private List<Texture2D> textures = new List<Texture2D>();

	private List<byte> hashes = new List<byte>();

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
		GenerateNewHashes();
	}

	public void OnPostRender_GenerateNewHashes()
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
				hashes.Clear();
				hashes.AddRange(array);
				return false;
			}
		}
		return true;
	}

	public void ClearTextures()
	{
		textures.Clear();
	}

	public void AddTexture(Texture2D texture)
	{
		textures.Add(texture);
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

	private byte CalculateHash(RenderTexture texture)
	{
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = texture;
		Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, mipmap: false);
		texture2D.ReadPixels(new Rect(0f, 0f, renderTexture.width, renderTexture.height), 0, 0);
		texture2D.Apply();
		RenderTexture.active = active;
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
