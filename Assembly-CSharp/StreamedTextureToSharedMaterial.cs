using UnityEngine;

public class StreamedTextureToSharedMaterial : StreamingAsset<Texture2D, Texture2D>
{
	[Tooltip("For standard unity shaders \"_MainTex\" is the main textures name.")]
	[SerializeField]
	[Header("Configuration")]
	protected string shaderTextureVariableName = "_MainTex";

	[SerializeField]
	[Header("Dependencies")]
	protected Material material;

	public void Reset()
	{
		if (material == null && GetComponent<Renderer>() != null)
		{
			material = GetComponent<Renderer>().sharedMaterial;
		}
	}

	protected override void OnAssetSet()
	{
		material.SetTexture(shaderTextureVariableName, Asset);
	}
}
