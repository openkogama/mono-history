using UnityEngine;

public class StreamedSharedMaterialHandler : MonoBehaviour
{
	[SerializeField]
	private StreamedTextureToSharedMaterial streamedTextureToSharedMaterialPrefab;

	private static bool streamComponentSet;

	public static void Reset()
	{
		streamComponentSet = false;
	}

	public void StartTextureStreaming()
	{
		if (!streamComponentSet)
		{
			MVGameControllerBase.StreamingAssetManager.Instantiate(streamedTextureToSharedMaterialPrefab);
			streamComponentSet = true;
		}
	}
}
