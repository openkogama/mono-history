using UnityEngine;

public class StreamedTextureToSharedMaterial : StreamingAsset<Texture2D, Texture2D>
{
	[SerializeField]
	[Header("Configuration")]
	[Tooltip("For standard unity shaders \"_MainTex\" is the main textures name.")]
	protected string shaderTextureVariableName = "_MainTex";

	[Header("Dependencies")]
	[SerializeField]
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
