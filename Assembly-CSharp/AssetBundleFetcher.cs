using System.Collections;
using UnityEngine;

public class AssetBundleFetcher : MonoBehaviour
{
	public string bundleUrl;

	public void DoFetch(string rootUrl, string bundlePath, BundleDownloadedCallback systemCallback)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected Obj, but got Unknown
		bundleUrl = bundlePath;
		WWW www = new WWW(rootUrl + bundlePath);
		((MonoBehaviour)this).StartCoroutine(FetchCoroutine(bundlePath, www, systemCallback));
	}

	private IEnumerator FetchCoroutine(string bundlePath, WWW www, BundleDownloadedCallback systemCallback)
	{
		yield return www;
		systemCallback(www.assetBundle, bundlePath);
	}
}
