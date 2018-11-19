using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

namespace Assets.Scripts.WorldObjectTypes.Avatar.Accessories;

public class AccessoryLoader
{
	private class Request
	{
		private Action<AvatarAccessory> accessoryCreatedCallback;

		private AccessoryLoaderRequest accessoryLoaderRequest;

		public string SubUrl => accessoryLoaderRequest.SubUrl;

		public Request(Action<AvatarAccessory> accessoryCreatedCallback, AccessoryLoaderRequest accessoryLoaderRequest)
		{
			this.accessoryCreatedCallback = accessoryCreatedCallback;
			this.accessoryLoaderRequest = accessoryLoaderRequest;
		}

		public void Callback(AvatarAccessory avatarAccessory)
		{
			accessoryCreatedCallback(avatarAccessory);
		}

		public void Destroy()
		{
			accessoryLoaderRequest.Remove();
		}
	}

	private class AccessoryLoaderRequest
	{
		private readonly int id;

		private Action<int, AvatarAccessory> accessoryCreatedCallback;

		private readonly string subUrl;

		public string SubUrl => subUrl;

		public AccessoryLoaderRequest(int id, Action<int, AvatarAccessory> accessoryCreatedCallback, string subUrl)
		{
			this.id = id;
			this.accessoryCreatedCallback = accessoryCreatedCallback;
			this.subUrl = subUrl;
		}

		public void Remove()
		{
			accessoryCreatedCallback = null;
			AsyncWWWManager.UnsubscribeWWWRequest(Callback);
		}

		public void LoadAccessory()
		{
			if (Urls.StreamingAssetUrlReady())
			{
				Urls.onStreamingAssetsUrlAvailable = (Urls.OnStreamingAssetsUrlAvailable)Delegate.Remove(Urls.onStreamingAssetsUrlAvailable, new Urls.OnStreamingAssetsUrlAvailable(LoadAccessory));
				string path = StreamingAsset.DBUrlToServerUrl(StreamingAsset.AssetBundleUrl + subUrl);
				GetRequest asyncRequest = new CachedGetRequest(path, Callback, WWWRequestPriority.WaitUntilSyncronizingIsDone);
				AsyncWWWManager.WWWRequest(asyncRequest);
			}
			else
			{
				Urls.onStreamingAssetsUrlAvailable = (Urls.OnStreamingAssetsUrlAvailable)Delegate.Combine(Urls.onStreamingAssetsUrlAvailable, new Urls.OnStreamingAssetsUrlAvailable(LoadAccessory));
			}
		}

		private void Callback(WWW www)
		{
			if (!string.IsNullOrEmpty(www.error))
			{
				return;
			}
			UnityEngine.Object original = StreamingAsset.UnpackBundle<GameObject>(www);
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(original);
			AccessorySettings component = gameObject.GetComponent<AccessorySettings>();
			if (component == null)
			{
				throw new Exception("AvatarAccessory settings not found");
			}
			AvatarAccessory avatarAccessory = null;
			if (component.GetType() == typeof(AccessoryHatSettings))
			{
				avatarAccessory = gameObject.AddComponent<AvatarAccessoryHat>();
			}
			else if (component.GetType() == typeof(AccessoryParticlesSettings))
			{
				avatarAccessory = gameObject.AddComponent<AvatarAccessoryParticles>();
			}
			else
			{
				if (component.GetType() != typeof(AccessoryBackAccessoriesSettings))
				{
					throw new Exception("Unknown settings");
				}
				avatarAccessory = gameObject.AddComponent<AvatarAccessoryBackAccessories>();
			}
			avatarAccessory.InitAccessory(subUrl, subUrl);
			if (accessoryCreatedCallback != null)
			{
				accessoryCreatedCallback(id, avatarAccessory);
				accessoryCreatedCallback = null;
			}
		}
	}

	private int id;

	private Dictionary<int, Request> requests = new Dictionary<int, Request>();

	public void LoadAccessory(string url, Action<AvatarAccessory> accessoryCreatedExternalCallback)
	{
		foreach (KeyValuePair<int, Request> request in requests)
		{
			if (request.Value.SubUrl == url)
			{
				return;
			}
		}
		AddRequest(new AccessoryLoaderRequest(id, AccessoryCreatedInternalCallback, url), accessoryCreatedExternalCallback);
	}

	private void AddRequest(AccessoryLoaderRequest accessoryLoaderRequest, Action<AvatarAccessory> accessoryCreatedExternalCallback)
	{
		requests.Add(id, new Request(accessoryCreatedExternalCallback, accessoryLoaderRequest));
		accessoryLoaderRequest.LoadAccessory();
		id++;
	}

	private void AccessoryCreatedInternalCallback(int doneId, AvatarAccessory avatarAccessory)
	{
		Request request = requests[doneId];
		requests.Remove(doneId);
		request.Callback(avatarAccessory);
	}

	public void Destroy()
	{
		foreach (KeyValuePair<int, Request> request in requests)
		{
			request.Value.Destroy();
		}
		requests.Clear();
	}
}
