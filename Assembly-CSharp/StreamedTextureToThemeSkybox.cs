using UnityEngine;

public class StreamedTextureToThemeSkybox : StreamingAsset<Texture, Texture>
{
	[SerializeField]
	private string shaderPropertyName = "_MainTex";

	[SerializeField]
	private ThemeSkybox skybox;

	protected override void OnAssetSet()
	{
		skybox.Material.SetTexture(shaderPropertyName, Asset);
	}

	protected void Reset()
	{
		if (skybox == null)
		{
			skybox = GetComponent<ThemeSkybox>();
		}
		if (skybox == null)
		{
			skybox = GetComponentInParent<ThemeSkybox>();
		}
	}
}
