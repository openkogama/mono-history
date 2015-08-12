using Newtonsoft.Json;
using UnityEngine;

public class TestWWWMgr : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		if (!Input.GetKeyUp(KeyCode.T))
		{
		}
	}

	private void Callback(WWW www)
	{
		Debug.Log("Result " + www.error);
		Debug.Log("Returned " + www.assetBundle);
		Debug.Log(www.assetBundle.mainAsset);
		Object.Instantiate(www.assetBundle.mainAsset);
	}

	private void Callback(WWW result, string path)
	{
		Debug.Log(result.bytes.Length);
		Debug.Log(result.text);
		InitialLevelData message = JsonConvert.DeserializeObject<InitialLevelData>(result.text);
		Debug.Log(message);
	}
}
