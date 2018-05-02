using UnityEngine;

public class StreamedTextureToMaterial : StreamingAsset<Texture, Texture>
{
	[SerializeField]
	[Tooltip("For standard unity shaders \"_MainTex\" is the main textures name.")]
	[Header("Configuration")]
	protected string shaderTextureVariableName = "_MainTex";

	[SerializeField]
	[Header("Dependencies")]
	private MeshRenderer meshRenderer;

	public bool assetSet;

	public Texture TextureAsset => Asset;

	public void ReDownload()
	{
		Download(url, onAssetSetAction);
	}

	protected override void OnAssetSet()
	{
		meshRenderer.materials[0].SetTexture(shaderTextureVariableName, Asset);
	}
}
