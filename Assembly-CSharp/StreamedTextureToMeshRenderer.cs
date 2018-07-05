using UnityEngine;

public class StreamedTextureToMeshRenderer : StreamingAsset<Texture, Texture>
{
	[Header("Configuration")]
	[Tooltip("For standard unity shaders \"_MainTex\" is the main textures name.")]
	[SerializeField]
	protected string shaderTextureVariableName = "_MainTex";

	[SerializeField]
	[Header("Dependencies")]
	private MeshRenderer meshRenderer;

	public bool assetSet;

	public Texture TextureAsset => Asset;

	public void ReDownload()
	{
		Download_Cached(url, onAssetSetAction);
	}

	protected override void OnAssetSet()
	{
		meshRenderer.materials[0].SetTexture(shaderTextureVariableName, Asset);
	}
}
