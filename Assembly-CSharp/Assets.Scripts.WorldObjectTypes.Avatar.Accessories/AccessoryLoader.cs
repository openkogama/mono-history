using System;
using System.Collections.Generic;
using System.Linq;
using MV.Common;
using UnityEngine;

namespace Assets.Scripts.WorldObjectTypes.Avatar.Accessories;

public class AccessoryLoader
{
	private class Request
	{
		private Action<AvatarAccessory> accessoryCreatedCallback;

		private AccessoryLoaderRequest accessoryLoaderRequest;

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

		private readonly AvatarAccessoryParams parameters;

		private AccessoryLoaderRequest(int id, Action<int, AvatarAccessory> accessoryCreatedCallback, AvatarAccessoryParams parameters)
		{
			this.id = id;
			this.accessoryCreatedCallback = accessoryCreatedCallback;
			this.parameters = parameters;
		}

		public AccessoryLoaderRequest(int id, StreamingAssetInfo assetInfo, Action<int, AvatarAccessory> accessoryCreatedCallback)
			: this(id, accessoryCreatedCallback, new AvatarAccessoryParams(0, assetInfo.RequestPath))
		{
		}

		public AccessoryLoaderRequest(int id, ProductInventoryInfo invInfo, Action<int, AvatarAccessory> accessoryCreatedCallback)
			: this(id, accessoryCreatedCallback, new AvatarAccessoryParams(invInfo.InventoryID, invInfo.ProductInfo.RequestPath))
		{
		}

		public AccessoryLoaderRequest(int id, int inventoryID, string assetPath, DateTime purchaseTime, Action<int, AvatarAccessory> accessoryCreatedCallback)
			: this(id, accessoryCreatedCallback, new AvatarAccessoryParams(inventoryID, MVGameControllerBase.Game.StreamingAssetInfoMap.Values.FirstOrDefault((StreamingAssetInfo sai) => sai.AssetPath == assetPath).RequestPath))
		{
		}

		public void Remove()
		{
			AsyncWWWManager.UnsubscribeWWWRequest(Callback);
		}

		public void LoadAccessory()
		{
			string streamingAssets = Urls.StreamingAssets;
			AvatarAccessoryParams avatarAccessoryParams = parameters;
			AsyncWWWManager.WWWRequest(new StreamingAssetRequestTempHack(streamingAssets + avatarAccessoryParams.AssetReqPath, Callback, WWWRequestPriority.WaitUntilSyncronizingIsDone));
		}

		private void Callback(WWW www, UnityEngine.Object mainAsset)
		{
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(mainAsset);
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
			else
			{
				if (component.GetType() != typeof(AccessoryParticlesSettings))
				{
					throw new Exception("Unknown settings");
				}
				avatarAccessory = gameObject.AddComponent<AvatarAccessoryParticles>();
			}
			AvatarAccessory avatarAccessory2 = avatarAccessory;
			AvatarAccessoryParams p = parameters;
			AvatarAccessoryParams avatarAccessoryParams = parameters;
			avatarAccessory2.InitAccessory(p, avatarAccessoryParams.AssetReqPath);
			accessoryCreatedCallback(id, avatarAccessory);
			accessoryCreatedCallback = null;
		}
	}

	private int id;

	private Dictionary<int, Request> requests = new Dictionary<int, Request>();

	public void LoadAccessory(StreamingAssetInfo assetInfo, Action<AvatarAccessory> accessoryCreatedCallback)
	{
		AddRequest(new AccessoryLoaderRequest(id, assetInfo, AccessoryCreatedCallback), accessoryCreatedCallback);
	}

	public void LoadAccessory(ProductInventoryInfo invInfo, Action<AvatarAccessory> accessoryCreatedCallback)
	{
		AddRequest(new AccessoryLoaderRequest(id, invInfo, AccessoryCreatedCallback), accessoryCreatedCallback);
	}

	public void LoadAccessory(int inventoryID, string assetPath, DateTime purchaseTime, Action<AvatarAccessory> accessoryCreatedCallback)
	{
		AddRequest(new AccessoryLoaderRequest(id, inventoryID, assetPath, purchaseTime, AccessoryCreatedCallback), accessoryCreatedCallback);
	}

	private void AddRequest(AccessoryLoaderRequest accessoryLoaderRequest, Action<AvatarAccessory> accessoryCreatedCallback)
	{
		requests.Add(id, new Request(accessoryCreatedCallback, accessoryLoaderRequest));
		accessoryLoaderRequest.LoadAccessory();
		id++;
	}

	private void AccessoryCreatedCallback(int doneId, AvatarAccessory avatarAccessory)
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
