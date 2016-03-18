using System;
using System.Collections.Generic;
using System.Linq;
using MV.Common;
using UnityEngine;

public abstract class AvatarAccessory : MonoBehaviour
{
	private static Dictionary<string, HashSet<AvatarAccessoryParams>> assetPathToParamsMap = new Dictionary<string, HashSet<AvatarAccessoryParams>>();

	private static Dictionary<AvatarAccessoryParams, Action<AvatarAccessory>> paramsToCallbacksMap = new Dictionary<AvatarAccessoryParams, Action<AvatarAccessory>>();

	private int _gameObjectID;

	private Transform _transform;

	private Collider[] _colliders;

	private Renderer[] _renderers;

	private bool _visible = true;

	private bool attached;

	private Quaternion worldRotationOnAttach;

	private static MVNetworkGame Game => MVGameControllerBase.Game;

	public abstract AccessorySettings AccessorySettings { get; }

	public AvatarAccessoryCategory Category { get; protected set; }

	public string AssetPath { get; private set; }

	public int InventoryID { get; private set; }

	public InventoryExpirationInfo ExpirationInfo { get; private set; }

	public AvatarAccessorySlot Slot { get; set; }

	public float Offset { get; set; }

	public int GameObjectID => _gameObjectID;

	public Transform Transform
	{
		get
		{
			if (_transform == null)
			{
				_transform = transform;
			}
			return _transform;
		}
	}

	public Collider[] Colliders
	{
		get
		{
			if (_colliders == null)
			{
				_colliders = GetComponentsInChildren<Collider>();
			}
			return _colliders;
		}
	}

	public Renderer[] Renderers
	{
		get
		{
			if (_renderers == null)
			{
				_renderers = GetComponentsInChildren<Renderer>();
			}
			return _renderers;
		}
	}

	public bool Visible
	{
		get
		{
			return _visible;
		}
		set
		{
			if (_visible != value)
			{
				Renderer[] renderers = Renderers;
				foreach (Renderer renderer in renderers)
				{
					renderer.enabled = value;
				}
				_visible = value;
			}
		}
	}

	public bool Attached
	{
		get
		{
			return attached;
		}
		set
		{
			if (attached != value)
			{
				attached = value;
				if (attached)
				{
					worldRotationOnAttach = Transform.rotation;
				}
			}
		}
	}

	public virtual Vector3 AttachmentPointWorldPos => Vector3.zero;

	public virtual bool HasAttachmentPoint => false;

	protected virtual void Awake()
	{
		_gameObjectID = gameObject.GetInstanceID();
		Collider[] colliders = Colliders;
		foreach (Collider collider in colliders)
		{
			collider.enabled = false;
		}
		Renderer[] renderers = Renderers;
		foreach (Renderer renderer in renderers)
		{
			renderer.enabled = _visible;
		}
	}

	protected virtual void Start()
	{
	}

	protected virtual void Update()
	{
		if (attached && AccessorySettings.ConstantWorldRotation)
		{
			Transform.rotation = worldRotationOnAttach;
		}
	}

	public override int GetHashCode()
	{
		return 17 * InventoryID;
	}

	public override bool Equals(object o)
	{
		if (o == null)
		{
			return false;
		}
		if (o == this)
		{
			return true;
		}
		return false;
	}

	public virtual Bounds GetWorldBounds()
	{
		Bounds result = default;
		bool flag = true;
		Renderer[] renderers = Renderers;
		foreach (Renderer renderer in renderers)
		{
			if (flag)
			{
				result = renderer.bounds;
			}
			else
			{
				result.Encapsulate(renderer.bounds);
			}
		}
		return result;
	}

	public virtual Bounds GetLocalBounds()
	{
		Bounds worldBounds = GetWorldBounds();
		if (transform != null)
		{
			worldBounds.center -= transform.position;
		}
		return worldBounds;
	}

	protected virtual void InitAccessory(AvatarAccessoryParams p, string bundleName)
	{
		InventoryID = p.InventoryID;
		AssetPath = p.AssetReqPath;
		ExpirationInfo = GetExpirationInfo(p);
		name = "Accessory " + InventoryID + " " + bundleName;
	}

	private InventoryExpirationInfo GetExpirationInfo(AvatarAccessoryParams p)
	{
		if (p.InventoryID == 0)
		{
			return null;
		}
		InventoryExpirationInfo expirationInfo = Game.StreamingAssetExpirationChecker.GetExpirationInfo(p.InventoryID);
		if (expirationInfo == null && p.ExpirationInfo != null)
		{
			Game.StreamingAssetExpirationChecker.AddExpirationInfo(p.ExpirationInfo);
			return p.ExpirationInfo;
		}
		if (p.ExpirationInfo == null)
		{
			return expirationInfo;
		}
		if (!expirationInfo.Equals(p.ExpirationInfo))
		{
			Debug.LogError("StreamingAssetInventory contains expiration info different from the one given in creation params for inventoryID: " + p.InventoryID);
			Debug.LogError(string.Concat("ParExp: ", p.ExpirationInfo, "\n InvExp ", expirationInfo));
		}
		return expirationInfo;
	}

	private static bool MapAssetPathToParams(AvatarAccessoryParams param)
	{
		bool flag = false;
		HashSet<AvatarAccessoryParams> value = null;
		assetPathToParamsMap.TryGetValue(param.AssetReqPath, out value);
		if (value != null)
		{
			value.Add(param);
			return false;
		}
		value = new HashSet<AvatarAccessoryParams>();
		value.Add(param);
		assetPathToParamsMap[param.AssetReqPath] = value;
		return true;
	}

	private static void MapParamsToCallback(AvatarAccessoryParams param, Action<AvatarAccessory> accessoryCreatedCallback)
	{
		Action<AvatarAccessory> value = null;
		paramsToCallbacksMap.TryGetValue(param, out value);
		if (value != null)
		{
			value = (Action<AvatarAccessory>)Delegate.Combine(value, accessoryCreatedCallback);
			paramsToCallbacksMap[param] = value;
		}
		else
		{
			value = accessoryCreatedCallback;
			paramsToCallbacksMap[param] = value;
		}
	}

	public static void Create(StreamingAssetInfo assetInfo, Action<AvatarAccessory> accessoryCreatedCallback)
	{
		Create(assetInfo.RequestPath, accessoryCreatedCallback);
	}

	private static void Create(string assetPath, Action<AvatarAccessory> accessoryCreatedCallback)
	{
		AvatarAccessoryParams par = new AvatarAccessoryParams(0, assetPath);
		Create(par, accessoryCreatedCallback);
	}

	public static void Create(ProductInventoryInfo invInfo, Action<AvatarAccessory> accessoryCreatedCallback)
	{
		if (invInfo.ProductInfo == null)
		{
			Debug.LogError("Can't create from inventory info because ProductInfo is NULL or ProductInfo.ShopInfo is NULL");
			return;
		}
		AvatarAccessoryParams par = new AvatarAccessoryParams(invInfo.InventoryID, invInfo.ProductInfo.RequestPath);
		Create(par, accessoryCreatedCallback);
	}

	public static void Create(int inventoryID, string assetPath, DateTime purchaseTime, int rentExpireSeconds, Action<AvatarAccessory> accessoryCreatedCallback)
	{
		StreamingAssetInfo streamingAssetInfo = Game.StreamingAssetInfoMap.Values.FirstOrDefault((StreamingAssetInfo sai) => sai.AssetPath == assetPath);
		if (streamingAssetInfo == null)
		{
			Debug.LogError("Cannot create accessory, asset info not fetched. path: " + assetPath);
			return;
		}
		AvatarAccessoryParams par = new AvatarAccessoryParams(inventoryID, streamingAssetInfo.RequestPath);
		Create(par, accessoryCreatedCallback);
	}

	private static void Create(AvatarAccessoryParams par, Action<AvatarAccessory> accessoryCreatedCallback)
	{
		MapParamsToCallback(par, accessoryCreatedCallback);
		if (MapAssetPathToParams(par))
		{
			AsyncWWWManager.WWWRequest(new StreamingAssetRequestTempHack(Urls.StreamingAssets + par.AssetReqPath, LoadedAccessoryAsset));
		}
	}

	private static void LoadedAccessoryAsset(WWW www, UnityEngine.Object mainAsset)
	{
		string text = www.url.TrimStart(Urls.StreamingAssets.ToCharArray());
		text = text.Split('?')[0];
		HashSet<AvatarAccessoryParams> hashSet = assetPathToParamsMap[text];
		HashSet<AvatarAccessoryParams> hashSet2 = new HashSet<AvatarAccessoryParams>(hashSet);
		HashSet<AvatarAccessoryParams> hashSet3 = new HashSet<AvatarAccessoryParams>();
		int num = text.LastIndexOf('/');
		int num2 = text.LastIndexOf('.');
		string bundleName = text.Substring(num + 1, num2 - num - 1);
		foreach (AvatarAccessoryParams item in hashSet2)
		{
			Action<AvatarAccessory> value = null;
			paramsToCallbacksMap.TryGetValue(item, out value);
			Delegate[] invocationList = value.GetInvocationList();
			Delegate[] array = invocationList;
			for (int i = 0; i < array.Length; i++)
			{
				Action<AvatarAccessory> action = (Action<AvatarAccessory>)array[i];
				if (www == null || www.assetBundle == null)
				{
					Debug.Log("Got to here ");
					string text2 = "Failed to create accessory from bundle " + www.url;
					if (www != null && www.assetBundle == null)
					{
						text2 += ". Bundle has no main asset";
					}
					Debug.LogError(text2);
					action(null);
					continue;
				}
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
				avatarAccessory.InitAccessory(item, bundleName);
				action(avatarAccessory);
			}
			paramsToCallbacksMap.TryGetValue(item, out value);
			Delegate[] invocationList2 = value.GetInvocationList();
			if (invocationList.Length == invocationList2.Length)
			{
				paramsToCallbacksMap.Remove(item);
				continue;
			}
			Delegate[] array2 = invocationList;
			foreach (Delegate obj in array2)
			{
				value = (Action<AvatarAccessory>)Delegate.Remove(value, (Action<AvatarAccessory>)obj);
			}
			hashSet3.Add(item);
		}
		hashSet2.ExceptWith(hashSet3);
		hashSet.ExceptWith(hashSet2);
		if (hashSet.Count == 0)
		{
			assetPathToParamsMap.Remove(text);
		}
		else
		{
			LoadedAccessoryAsset(www, mainAsset);
		}
	}
}
