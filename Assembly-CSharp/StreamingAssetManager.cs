using UnityEngine;

public class StreamingAssetManager : MonoBehaviour
{
	public void Instantiate(StreamingAsset streamingAssetPrefab)
	{
		StreamingAsset streamingAsset = Object.Instantiate(streamingAssetPrefab);
		streamingAsset.transform.SetParent(MVGameControllerBase.StreamingAssetManager.transform, worldPositionStays: false);
	}
}
