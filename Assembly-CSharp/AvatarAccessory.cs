using System;
using System.Collections.Generic;
using System.Linq;
using MV.Common;
using UnityEngine;

public abstract class AvatarAccessory : MonoBehaviour
{
	private static Dictionary<string, HashSet<AvatarAccessoryParams>> assetPathToParamsMap = new Dictionary<string, HashSet<AvatarAccessoryParams>>();

	private static Dictionary<AvatarAccessoryParams, Action<AvatarAccessory>> paramsToCallbacksMap = new Dictionary<AvatarAccessoryParams, Action<AvatarAccessory>>();

	public AvatarAccessorySlot[] ValidSlots;

	public bool AllignToBody;

	public float DefaultOffset;

	public AvatarAccessorySlot DefaultSlot = AvatarAccessorySlot.Torso;

	public bool ConstantWorldRotation;

	private int _gameObjectID;

	private Transform _transform;

	private Collider[] _colliders;

	private Renderer[] _renderers;

	private bool _visible = true;

	private bool attached;

	private Quaternion worldRotationOnAttach;

	private static MVWorldObjectClientManager WOCM => MVGameController.Instance.WOCM;

	private static MVNetworkGame Game => MVGameController.Instance.Game;

	public AvatarAccessoryCategory Category { get; protected set; }

	public string AssetPath { get; private set; }

	public int InventoryID { get; private set; }

	public InventoryExpirationInfo ExpirationInfo { get; private set; }

	public AvatarAccessorySlot Slot { get; set; }

	public float Offset { get; set; }

	public int GameObjectID => _gameObjectID;

	public Transform Transform => _transform;

	public Collider[] Colliders => _colliders;

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
				Renderer[] renderers = _renderers;
				foreach (Renderer val in renderers)
				{
					val.enabled = value;
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
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
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

	public virtual Vector3 AttachmentPointWorldPos
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return Vector3.zero;
		}
	}

	public virtual bool HasAttachmentPoint => false;

	protected virtual void Awake()
	{
		_gameObjectID = ((Object)((Component)this).gameObject).GetInstanceID();
		_transform = ((Component)this).transform;
		_colliders = ((Component)this).GetComponentsInChildren<Collider>();
		Collider[] colliders = _colliders;
		foreach (Collider val in colliders)
		{
			val.enabled = false;
		}
		_renderers = ((Component)this).GetComponentsInChildren<Renderer>();
		Renderer[] renderers = _renderers;
		foreach (Renderer val2 in renderers)
		{
			val2.enabled = _visible;
		}
	}

	protected virtual void Start()
	{
	}

	protected virtual void Update()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (attached && ConstantWorldRotation)
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
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		Bounds result = default;
		bool flag = true;
		Renderer[] renderers = _renderers;
		foreach (Renderer val in renderers)
		{
			if (flag)
			{
				result = val.bounds;
			}
			else
			{
				result.Encapsulate(val.bounds);
			}
		}
		return result;
	}

	public virtual Bounds GetLocalBounds()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		Bounds worldBounds = GetWorldBounds();
		if ((Object)(object)((Component)this).transform != (Object)null)
		{
			worldBounds.center -= ((Component)this).transform.position;
		}
		return worldBounds;
	}

	protected virtual void InitAccessory(AvatarAccessoryParams p, string bundleName)
	{
		InventoryID = p.InventoryID;
		AssetPath = p.AssetReqPath;
		ExpirationInfo = GetExpirationInfo(p);
		((Object)this).name = "Accessory " + InventoryID + " " + bundleName;
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
			Debug.LogError((object)("StreamingAssetInventory contains expiration info different from the one given in creation params for inventoryID: " + p.InventoryID));
			Debug.LogError((object)string.Concat(new object[4] { "ParExp: ", p.ExpirationInfo, "\n InvExp ", expirationInfo }));
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

	public static void Create(ProductInventoryInfo<StreamingAssetInfo> invInfo, Action<AvatarAccessory> accessoryCreatedCallback)
	{
		if (invInfo.ProductInfo == null)
		{
			Debug.LogError((object)"Can't create from inventory info because ProductInfo is NULL or ProductInfo.ShopInfo is NULL");
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
			Debug.LogError((object)("Cannot create accessory, asset info not fetched. path: " + assetPath));
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
			Game.AssetBundleMgr.RequestAssetBundle(par.AssetReqPath, LoadedAccessoryAsset, autoRetry: true, highPriority: true);
		}
	}

	private static void LoadedAccessoryAsset(AssetBundle bundle, string assetPath)
	{
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Expected Obj, but got Unknown
		HashSet<AvatarAccessoryParams> hashSet = assetPathToParamsMap[assetPath];
		HashSet<AvatarAccessoryParams> hashSet2 = new HashSet<AvatarAccessoryParams>(hashSet);
		HashSet<AvatarAccessoryParams> hashSet3 = new HashSet<AvatarAccessoryParams>();
		int num = assetPath.LastIndexOf('/');
		int num2 = assetPath.LastIndexOf('.');
		string bundleName = assetPath.Substring(num + 1, num2 - num - 1);
		foreach (AvatarAccessoryParams item in hashSet2)
		{
			Action<AvatarAccessory> value = null;
			paramsToCallbacksMap.TryGetValue(item, out value);
			Delegate[] invocationList = value.GetInvocationList();
			Delegate[] array = invocationList;
			for (int i = 0; i < array.Length; i++)
			{
				Action<AvatarAccessory> action = (Action<AvatarAccessory>)array[i];
				if ((Object)(object)bundle == (Object)null || bundle.mainAsset == (Object)null)
				{
					string text = "Failed to create accessory from bundle " + assetPath;
					if ((Object)(object)bundle != (Object)null && bundle.mainAsset == (Object)null)
					{
						text += ". Bundle has no main asset";
					}
					Debug.LogError((object)text);
					action(null);
				}
				else
				{
					GameObject val = (GameObject)Object.Instantiate(bundle.mainAsset);
					AvatarAccessory component = val.GetComponent<AvatarAccessory>();
					component.InitAccessory(item, bundleName);
					action(component);
				}
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
			assetPathToParamsMap.Remove(assetPath);
		}
		else
		{
			LoadedAccessoryAsset(bundle, assetPath);
		}
		Game.AssetBundleMgr.UnsubscribeBundleCallback(assetPath, LoadedAccessoryAsset);
	}
}
