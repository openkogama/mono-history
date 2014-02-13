using System;
using System.Collections.Generic;
using UnityEngine;

internal class AssetBundleCache
{
	internal class AssetBundleCacheNode
	{
		internal AssetBundle assetBundle;

		internal AssetBundleCacheState state;

		public BundleDownloadedCallback OnComplete;
	}

	internal enum AssetBundleCacheState
	{
		Unknown,
		Pending,
		Complete,
		FailedPermantly
	}

	private Dictionary<string, AssetBundleCacheNode> nodes = new Dictionary<string, AssetBundleCacheNode>();

	internal AssetBundle GetBundle(string bundlePath)
	{
		AssetBundleCacheNode value = null;
		nodes.TryGetValue(bundlePath, out value);
		if (value != null && value.state == AssetBundleCacheState.Complete)
		{
			return value.assetBundle;
		}
		return null;
	}

	internal void AddRequest(string bundlePath)
	{
		AssetBundleCacheNode assetBundleCacheNode = new AssetBundleCacheNode();
		assetBundleCacheNode.state = AssetBundleCacheState.Pending;
		nodes.Add(bundlePath, assetBundleCacheNode);
	}

	internal void SubscribeToNode(string bundlePath, BundleDownloadedCallback bundleReceiveCallback)
	{
		if (!nodes.ContainsKey(bundlePath))
		{
			AddRequest(bundlePath);
		}
		if (bundleReceiveCallback != null)
		{
			if (nodes[bundlePath].state == AssetBundleCacheState.Complete || nodes[bundlePath].state == AssetBundleCacheState.FailedPermantly)
			{
				bundleReceiveCallback(nodes[bundlePath].assetBundle, bundlePath);
			}
			else if (nodes[bundlePath].state == AssetBundleCacheState.Pending)
			{
				AssetBundleCacheNode assetBundleCacheNode = nodes[bundlePath];
				assetBundleCacheNode.OnComplete = (BundleDownloadedCallback)Delegate.Combine(assetBundleCacheNode.OnComplete, bundleReceiveCallback);
			}
			else
			{
				Debug.LogError((object)"Attempt to subscribe to node that has unknown state!");
			}
		}
	}

	internal void UnsubscribeFromNode(string bundlePath, BundleDownloadedCallback bundleReceiveCallback)
	{
		AssetBundleCacheNode value = null;
		nodes.TryGetValue(bundlePath, out value);
		if (value != null)
		{
			AssetBundleCacheNode assetBundleCacheNode = value;
			assetBundleCacheNode.OnComplete = (BundleDownloadedCallback)Delegate.Remove(assetBundleCacheNode.OnComplete, bundleReceiveCallback);
		}
	}

	internal void UpdateCache(string bundlePath, AssetBundle assetBundle)
	{
		if (nodes.ContainsKey(bundlePath))
		{
			nodes[bundlePath].state = ((!((Object)(object)assetBundle == (Object)null)) ? AssetBundleCacheState.Complete : AssetBundleCacheState.FailedPermantly);
			nodes[bundlePath].assetBundle = assetBundle;
			if (nodes[bundlePath].OnComplete != null)
			{
				nodes[bundlePath].OnComplete(assetBundle, bundlePath);
			}
		}
		else
		{
			Debug.LogError((object)"AssetBundleCache received callback, but no node registered!");
		}
	}

	internal bool IsBundleRequested(string bundlePath)
	{
		if (nodes.ContainsKey(bundlePath))
		{
			return true;
		}
		return false;
	}
}
