using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MV.Common;
using UnityEngine;

public class MVBody : MVBlueprintBase, IWorldObjectWithModelingConstraint
{
	private struct AvatatAccessoryPos
	{
		public AvatarAccessorySlot Slot;

		public float Offset;
	}

	private static string prefabPath = "Prefabs/Avatar/skeleton7";

	private Hashtable accessoryData;

	private BoneAnimation animation;

	private AvatarBlobShadowController shadowBlob;

	private AvatarBlinker blinker;

	private BodyData bodyData;

	private Dictionary<int, IModelingConstraint> constraints = new Dictionary<int, IModelingConstraint>();

	private List<Renderer> renderers = new List<Renderer>();

	private List<Collider> colliders = new List<Collider>();

	private List<MVCubeModelInstance> attachedPartModels = new List<MVCubeModelInstance>();

	private bool initialized;

	private MVAvatar attachedAvatar;

	private bool visible = true;

	private bool shadowVisible = true;

	private bool accessoryParticlesVisible = true;

	private bool collidersEnabled = true;

	private Dictionary<int, AvatatAccessoryPos> pendingAccessoryPositions = new Dictionary<int, AvatatAccessoryPos>();

	private Dictionary<int, AvatarAccessory> accessoryMap = new Dictionary<int, AvatarAccessory>();

	private Dictionary<int, AvatatAccessoryPos> accessorySlotMap = new Dictionary<int, AvatatAccessoryPos>();

	private Dictionary<AvatarAccessorySlot, string> slotBoneNameMap = new Dictionary<AvatarAccessorySlot, string>
	{
		{
			AvatarAccessorySlot.Torso,
			"Torso"
		},
		{
			AvatarAccessorySlot.Head,
			"Head"
		},
		{
			AvatarAccessorySlot.LHand,
			"LUpArm"
		},
		{
			AvatarAccessorySlot.RHand,
			"RUpArm"
		},
		{
			AvatarAccessorySlot.LFoot,
			"LLowLeg"
		},
		{
			AvatarAccessorySlot.RFoot,
			"RLowLeg"
		}
	};

	private Vector3 modelScale = Vector3.zero;

	private MVNetworkGame Game => MVGameController.Instance.Game;

	public BoneAnimation Animation => animation;

	public BodyData BodyData => bodyData;

	public MVAvatar AttachedAvatar => attachedAvatar;

	public List<MVCubeModelInstance> AttachedParts => attachedPartModels;

	public AvatarBlobShadowController BlobShadow => shadowBlob;

	public new bool Visible
	{
		get
		{
			return visible;
		}
		set
		{
			if (visible != value)
			{
				visible = value;
				UpdateVisibility();
			}
		}
	}

	public bool ShadowVisible
	{
		get
		{
			return shadowVisible;
		}
		set
		{
			if (shadowVisible != value)
			{
				shadowVisible = value;
				UpdateVisibility();
			}
		}
	}

	public bool AccessoryParticlesVisible
	{
		get
		{
			return accessoryParticlesVisible;
		}
		set
		{
			if (accessoryParticlesVisible != value)
			{
				accessoryParticlesVisible = value;
				UpdateVisibility();
				Game.UpdateWorldObjectDataPartial(Id, "ParticlesVisible", value);
			}
		}
	}

	private bool CollidersEnabled
	{
		get
		{
			return collidersEnabled;
		}
		set
		{
			collidersEnabled = value;
			foreach (Collider collider in colliders)
			{
				collider.enabled = collidersEnabled;
			}
		}
	}

	public bool AccessoriesLoaded => pendingAccessoryPositions.Count == 0;

	public event EventHandler<EventArgs> AccessoriesChanged;

	public MVBody(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, prefabPath, worldObjects)
	{
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		MVWorldObjectClient value = null;
		if (worldObjects.TryGetValue(groupId, out value))
		{
			attachedAvatar = value as MVAvatar;
		}
		GetComponents();
		previewLayerMask |= LayerFlags.Player;
	}

	private void GetComponents()
	{
		if ((Object)(object)animation == (Object)null)
		{
			animation = gameObject.GetComponent<BoneAnimation>();
		}
		if ((Object)(object)blinker == (Object)null)
		{
			blinker = gameObject.GetComponent<AvatarBlinker>();
		}
		if ((Object)(object)shadowBlob == (Object)null)
		{
			shadowBlob = gameObject.GetComponentInChildren<AvatarBlobShadowController>();
		}
		if ((Object)(object)bodyData == (Object)null)
		{
			bodyData = gameObject.GetComponent<BodyData>();
		}
	}

	public override void Initialize()
	{
		if (initialized)
		{
			Debug.LogWarning((object)("Trying to initialize body " + id + " more than once"));
			return;
		}
		base.Initialize();
		InitializeCommon();
		blinker.MeshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
		if (attachedAvatar != null)
		{
			CollidersEnabled = false;
			((Behaviour)shadowBlob).enabled = true;
			attachedPartModels.ForEach((MVCubeModelInstance cmi) =>
			{
				cmi.ReactsToLODChanges = false;
			});
		}
		else
		{
			CollidersEnabled = true;
			((Behaviour)shadowBlob).enabled = false;
		}
		initialized = true;
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		InitializeCommon();
		CollidersEnabled = false;
		((Behaviour)shadowBlob).enabled = false;
		attachedPartModels.ForEach((MVCubeModelInstance cmi) =>
		{
			cmi.ReactsToLODChanges = false;
		});
	}

	private void InitializeCommon()
	{
		AttachCubes();
		GetAccessoriesFromBPData();
	}

	public override void Destroy()
	{
		base.Destroy();
		AvatarAccessory[] array = accessoryMap.Values.ToArray();
		foreach (AvatarAccessory acc in array)
		{
			DestroyAccessory(acc, calledFromDestroy: true);
		}
	}

	private void UpdateVisibility()
	{
		foreach (Renderer renderer in renderers)
		{
			renderer.enabled = visible;
		}
		if (shadowVisible && (Object)(object)shadowBlob != (Object)null)
		{
			((Behaviour)shadowBlob).enabled = visible;
		}
		if ((Object)(object)blinker != (Object)null)
		{
			blinker.Visible = visible;
		}
		foreach (AvatarAccessory value in accessoryMap.Values)
		{
			if (value.Category == AvatarAccessoryCategory.Particles)
			{
				value.Visible = accessoryParticlesVisible && visible;
			}
			else
			{
				value.Visible = visible;
			}
		}
	}

	public void Attach(MVAvatar mvAvatar, bool isLocal)
	{
		attachedAvatar = mvAvatar;
		if (attachedAvatar != null)
		{
			GetComponents();
			if ((Object)(object)animation != (Object)null)
			{
				animation.Attach(attachedAvatar, isLocal);
			}
			if ((Object)(object)blinker != (Object)null)
			{
				blinker.Visible = visible;
				blinker.Attach(mvAvatar);
			}
			CollidersEnabled = false;
			((Behaviour)shadowBlob).enabled = true;
			attachedPartModels.ForEach((MVCubeModelInstance cmi) =>
			{
				cmi.ReactsToLODChanges = false;
			});
		}
	}

	public void Detach()
	{
		Debug.Log((object)("Detaching body " + id));
		animation.Detach();
		if ((Object)(object)blinker != (Object)null)
		{
			blinker.Detach();
		}
		((Behaviour)shadowBlob).enabled = false;
	}

	private void GetAccessoriesFromBPData()
	{
		accessoryData = (Hashtable)blueprintData[BlueprintData.AvatarAccessoryData.ToString("d")];
		if (accessoryData == null)
		{
			return;
		}
		HashSet<int> hashSet = new HashSet<int>();
		foreach (DictionaryEntry accessoryDatum in accessoryData)
		{
			Hashtable hashtable = (Hashtable)accessoryDatum.Value;
			int num = (int)hashtable[AvatarAccessoryData.InventoryID.ToString("d")];
			string assetPath = (string)hashtable[AvatarAccessoryData.AssetPath.ToString("d")];
			DateTime purchaseTime = new DateTime((long)hashtable[AvatarAccessoryData.PurchaseTimeTicks.ToString("d")]);
			int num2 = (int)hashtable[AvatarAccessoryData.RentExpireSeconds.ToString("d")];
			hashSet.Add(num);
			AvatarAccessorySlot slot = (AvatarAccessorySlot)(int)hashtable[AvatarAccessoryData.Slot.ToString("d")];
			float offset = (float)hashtable[AvatarAccessoryData.Offset.ToString("d")];
			AvatatAccessoryPos value = new AvatatAccessoryPos
			{
				Slot = slot,
				Offset = offset
			};
			if (!accessoryMap.ContainsKey(num))
			{
				if (!pendingAccessoryPositions.ContainsKey(num))
				{
					pendingAccessoryPositions[num] = value;
					if (0 < num2)
					{
						if (!Game.StreamingAssetExpirationChecker.Contains(num))
						{
							InventoryExpirationInfo expInfo = new InventoryExpirationInfo(MVProductType.StreamingAsset, num, ProductExpirationState.Expiring, purchaseTime, num2);
							Game.StreamingAssetExpirationChecker.AddExpirationInfo(expInfo);
						}
						else
						{
							Debug.LogWarning((object)("Body " + id + " not adding stored expiration data to checker"));
						}
					}
					AvatarAccessory.Create(num, assetPath, purchaseTime, num2, LoadedAccessoryCallback);
				}
				else
				{
					Debug.Log((object)("Body " + id + " data contains multiple accessories with ID " + num));
					Debug.LogError((object)"Body data contains multiple accessories with the same inventoryID. This is caused by user hiding and show accessories");
				}
			}
			else
			{
				Debug.LogWarning((object)("Body " + id + " already has attached accessory " + num));
			}
		}
		List<AvatarAccessory> list = new List<AvatarAccessory>();
		foreach (KeyValuePair<int, AvatarAccessory> item in accessoryMap)
		{
			if (!hashSet.Contains(item.Key))
			{
				list.Add(item.Value);
			}
		}
		if (0 < list.Count)
		{
			Debug.Log((object)("Body " + id + " removing accessories missing in bpData: " + list.BuildString(eachEntryNewLine: false)));
			list.ForEach((AvatarAccessory a) =>
			{
				DestroyAccessory(a);
			});
		}
		foreach (KeyValuePair<int, AvatarAccessory> item2 in accessoryMap)
		{
			if (AccessoryShouldBeSelecable(item2.Value))
			{
				MakeAccessorySelectable(item2.Value, item2.Value.Slot);
			}
		}
	}

	private void LoadedAccessoryCallback(AvatarAccessory accessory)
	{
		if (!((Object)(object)accessory == (Object)null))
		{
			if (accessory.ExpirationInfo != null)
			{
				Debug.Log((object)("Body " + id + " loaded acccessory: " + accessory.InventoryID + " expiring at " + accessory.ExpirationInfo.RentExpireTime));
			}
			else
			{
				Debug.Log((object)("Body " + id + " loaded acccessory: " + accessory.InventoryID + " NOT expiring"));
			}
			AvatatAccessoryPos avatatAccessoryPos = pendingAccessoryPositions[accessory.InventoryID];
			pendingAccessoryPositions.Remove(accessory.InventoryID);
			if (AttachAccessory(accessory, avatatAccessoryPos.Slot, avatatAccessoryPos.Offset) && AccessoryShouldBeSelecable(accessory))
			{
				MakeAccessorySelectable(accessory, avatatAccessoryPos.Slot);
			}
		}
	}

	private bool AccessoryShouldBeSelecable(AvatarAccessory accessory)
	{
		if (MVGameController.Instance.GameMode != MVGameMode.CharacterEditor)
		{
			return false;
		}
		if (accessory.Category != AvatarAccessoryCategory.Hat)
		{
			return false;
		}
		if ((object)Group.GetType() == typeof(MVAvatarLocal))
		{
			return false;
		}
		return true;
	}

	private void MakeAccessorySelectable(AvatarAccessory accessory, AvatarAccessorySlot accessorySlot)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected Obj, but got Unknown
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		MeshFilter[] componentsInChildren = ((Component)accessory.Transform).gameObject.GetComponentsInChildren<MeshFilter>();
		MeshFilter[] array = componentsInChildren;
		foreach (MeshFilter val in array)
		{
			GameObject val2 = new GameObject("SelectionHelper");
			val2.transform.parent = ((Component)val).gameObject.transform;
			val2.transform.localPosition = Vector3.zero;
			val2.transform.localRotation = Quaternion.identity;
			val2.transform.localScale = Vector3.one;
			MeshCollider val3 = val2.AddComponent<MeshCollider>();
			val3.sharedMesh = val.sharedMesh;
			val2.layer = LayerMask.NameToLayer("Hidden");
			SelectionHelperAvatarAccessory selectionHelperAvatarAccessory = val2.AddComponent<SelectionHelperAvatarAccessory>();
			selectionHelperAvatarAccessory.Init(accessory, accessorySlot, Id);
		}
	}

	private Transform GetSlotTransform(AvatarAccessorySlot slot)
	{
		if (slot == AvatarAccessorySlot.WholeBody)
		{
			return gameObject.transform;
		}
		return bodyData.GetPartBone(slotBoneNameMap[slot]);
	}

	public Vector3 GetSlotPosition(AvatarAccessorySlot slot, Vector3 offset)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		if (slot == AvatarAccessorySlot.WholeBody)
		{
			Vector3 val = gameObject.transform.position;
			Bounds localBounds = GetLocalBounds(BoundsContext.Preview);
			return val + localBounds.center + offset;
		}
		string text = slotBoneNameMap[slot];
		Transform partBone = bodyData.GetPartBone(text);
		if ((Object)(object)partBone == (Object)null)
		{
			Debug.LogError((object)$"Accessory: Failed to get bone {text} for slot {slot}");
		}
		Vector3 val2 = partBone.position;
		val2 += partBone.right * offset.x;
		val2 += partBone.up * offset.y;
		return val2 + partBone.forward * offset.z;
	}

	public bool AttachAccessory(AvatarAccessory acc, AvatarAccessorySlot slot, float offset)
	{
		if (slot == AvatarAccessorySlot.Undefined || !Enum.IsDefined(typeof(AvatarAccessorySlot), slot))
		{
			Debug.LogError((object)("Trying to attach accessory to an invalid slot " + (int)slot));
			return false;
		}
		if (!Enumerable.Contains(acc.ValidSlots, slot))
		{
			Debug.LogError((object)string.Concat(new object[4]
			{
				"Trying to attach accessory in slot: ",
				slot,
				", allowed: ",
				acc.ValidSlots.BuildString()
			}));
			return false;
		}
		if (!accessoryMap.ContainsKey(acc.InventoryID))
		{
			accessoryMap.Add(acc.InventoryID, acc);
			accessorySlotMap.Add(acc.InventoryID, new AvatatAccessoryPos
			{
				Slot = slot,
				Offset = offset
			});
			acc.Transform.parent = GetSlotTransform(slot);
			acc.Slot = slot;
			acc.Offset = offset;
			ApplyAccessoryOffset(acc, slot);
			acc.Attached = true;
			Collider[] array = acc.Colliders;
			foreach (Collider val in array)
			{
				val.enabled = false;
			}
			acc.Transform.SetLayerRecursively(gameObject.layer);
			acc.Visible = Visible;
			bool flag = Game.GameMode != MVGameMode.CharacterEditor || attachedAvatar == null;
			ProductInventoryInfo<StreamingAssetInfo> productInventoryInfo = Game.StreamingAssetInventory.Get(acc.InventoryID);
			if (flag && productInventoryInfo != null)
			{
				productInventoryInfo.EquippedOn = this;
				Game.StreamingAssetInventory.NotifyProductInventoryChange();
			}
			if (AccessoriesChanged != null)
			{
				AccessoriesChanged(this, EventArgs.Empty);
			}
			return true;
		}
		Debug.LogError((object)("Trying to add accessory " + acc.InventoryID + " second time!"));
		return false;
	}

	public void ApplyAccessoryOffset(AvatarAccessory acc, AvatarAccessorySlot slot)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		acc.Transform.localPosition = Vector3.zero;
		acc.Transform.localRotation = Quaternion.identity;
		Vector3 zero = Vector3.zero;
		if (!acc.HasAttachmentPoint)
		{
			zero.y += acc.Offset;
		}
		if (acc is AvatarAccessoryHat && acc.AllignToBody)
		{
			MVCubeModelInstance bodyPart = GetBodyPart(slotBoneNameMap[slot]);
			Bounds localBounds = bodyPart.GetLocalBounds(BoundsContext.Default);
			Vector3 val = localBounds.center + new Vector3(0f, localBounds.extents.y, 0f);
			Vector3 val2 = bodyPart.Transform.TransformPoint(val);
			Vector3 val3 = acc.Transform.parent.InverseTransformPoint(val2);
			zero.y += val3.y;
		}
		acc.Transform.position = GetSlotPosition(slot, zero);
		if (acc.HasAttachmentPoint)
		{
			Vector3 val4 = acc.Transform.position - acc.AttachmentPointWorldPos;
			acc.Transform.Translate(val4, (Space)0);
		}
	}

	public void AttachAccessoryPermanent(AvatarAccessory acc, AvatarAccessorySlot slot, float offset, ProductInventoryInfo<StreamingAssetInfo> invInfo)
	{
		if (AttachAccessory(acc, slot, offset))
		{
			MarkAccessoryPermanent(acc, invInfo);
		}
	}

	public void MarkAccessoryPermanent(ProductInventoryInfo<StreamingAssetInfo> invInfo)
	{
		AvatarAccessory value = null;
		accessoryMap.TryGetValue(invInfo.InventoryID, out value);
		if ((Object)(object)value == (Object)null)
		{
			Debug.LogError((object)("Cant set accessory " + invInfo.InventoryID + " as permanet because it hasn't been added to the body " + id));
		}
		else
		{
			MarkAccessoryPermanent(value, invInfo);
		}
	}

	private void MarkAccessoryPermanent(AvatarAccessory acc, ProductInventoryInfo<StreamingAssetInfo> invInfo)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[AvatarAccessoryData.InventoryID.ToString("d")] = acc.InventoryID;
		hashtable[AvatarAccessoryData.Slot.ToString("d")] = acc.Slot;
		hashtable[AvatarAccessoryData.Offset.ToString("d")] = acc.Offset;
		hashtable[AvatarAccessoryData.AssetPath.ToString("d")] = invInfo.ProductInfo.AssetPath;
		hashtable[AvatarAccessoryData.PurchaseTimeTicks.ToString("d")] = invInfo.PurchaseTime.Ticks;
		hashtable[AvatarAccessoryData.RentExpireSeconds.ToString("d")] = (invInfo.IsRented ? invInfo.ProductInfo.ShopInfo.RentExpireSeconds : 0);
		Game.UpdateWorldObjectDataPartial(Id, "BlueprintData\\" + BlueprintData.AvatarAccessoryData.ToString("d") + "\\" + acc.InventoryID, hashtable);
	}

	public IEnumerable<AvatarAccessory> GetAccessories(AvatarAccessorySlot slot, AvatarAccessoryCategory category)
	{
		return accessoryMap.Values.Where((AvatarAccessory a) => a.Slot == slot && a.Category == category);
	}

	public IEnumerable<AvatarAccessory> GetAccessories(AvatarAccessorySlot slot)
	{
		return accessoryMap.Values.Where((AvatarAccessory a) => a.Slot == slot);
	}

	public IEnumerable<AvatarAccessory> GetAccessories(AvatarAccessoryCategory category)
	{
		return accessoryMap.Values.Where((AvatarAccessory a) => a.Category == category);
	}

	public AvatarAccessory GetAccessory(int inventoryID)
	{
		AvatarAccessory value = null;
		accessoryMap.TryGetValue(inventoryID, out value);
		return value;
	}

	public HashSet<AvatarAccessory> GetAccessories()
	{
		return new HashSet<AvatarAccessory>(accessoryMap.Values);
	}

	public HashSet<int> GetAccessoryIDs()
	{
		return new HashSet<int>(accessoryMap.Keys);
	}

	public int GetAccessoryID(AvatarAccessorySlot slot)
	{
		if (!AnyAccessoryInSlot(slot))
		{
			return -1;
		}
		if (accessoryMap.Values.Any((AvatarAccessory a) => a.Slot == slot))
		{
			return accessoryMap.First((KeyValuePair<int, AvatarAccessory> pair) => pair.Value.Slot == slot).Key;
		}
		if (pendingAccessoryPositions.Values.Any((AvatatAccessoryPos a) => a.Slot == slot))
		{
			return pendingAccessoryPositions.First((KeyValuePair<int, AvatatAccessoryPos> pair) => pair.Value.Slot == slot).Key;
		}
		return -1;
	}

	public bool HasAccessoryWithID(int inventoryID)
	{
		return accessoryMap.ContainsKey(inventoryID);
	}

	public bool AnyAccessoryInSlot(AvatarAccessorySlot slot)
	{
		return pendingAccessoryPositions.Values.Any((AvatatAccessoryPos a) => a.Slot == slot) || accessoryMap.Values.Any((AvatarAccessory a) => a.Slot == slot);
	}

	public AvatarAccessorySlot GetAvatarAccessorySlot(int inventoryID)
	{
		if (accessoryMap.ContainsKey(inventoryID))
		{
			return accessoryMap[inventoryID].Slot;
		}
		Debug.LogError((object)("Accessory with inventory ID " + inventoryID + " is not attached"));
		return AvatarAccessorySlot.Undefined;
	}

	public void DetachAccessory(AvatarAccessory acc, bool calledFromDestroy = false)
	{
		ProductInventoryInfo<StreamingAssetInfo> productInventoryInfo = Game.StreamingAssetInventory.Get(acc.InventoryID);
		if (productInventoryInfo != null)
		{
			productInventoryInfo.EquippedOn = null;
			Game.StreamingAssetInventory.NotifyProductInventoryChange();
		}
		acc.Attached = false;
		acc.Transform.parent = null;
		pendingAccessoryPositions.Remove(acc.InventoryID);
		accessoryMap.Remove(acc.InventoryID);
		accessorySlotMap.Remove(acc.InventoryID);
		if (!calledFromDestroy)
		{
			RemoveFromBPData(acc);
		}
		if (AccessoriesChanged != null)
		{
			AccessoriesChanged(this, EventArgs.Empty);
		}
	}

	public void DestroyAccessory(AvatarAccessory acc, bool calledFromDestroy = false)
	{
		Debug.Log((object)"Destroying accessories");
		DetachAccessory(acc, calledFromDestroy);
		Object.Destroy((Object)(object)((Component)acc).gameObject);
	}

	public void DestroyAccessory(int inventoryID)
	{
		if (!accessoryMap.ContainsKey(inventoryID))
		{
			Debug.LogWarning((object)("Trying to detach non-attached avatar accessory with inventoryid: " + inventoryID));
		}
		else
		{
			DestroyAccessory(accessoryMap[inventoryID]);
		}
	}

	private void RemoveFromBPData(AvatarAccessory acc)
	{
		Hashtable hashtable = (Hashtable)blueprintData[BlueprintData.AvatarAccessoryData.ToString("d")];
		if (hashtable != null && hashtable.Contains(acc.InventoryID.ToString()))
		{
			hashtable.Remove(acc.InventoryID.ToString());
			Game.RemoveWorldObjectDataPartial(Id, "BlueprintData\\" + BlueprintData.AvatarAccessoryData.ToString("d") + "\\" + acc.InventoryID);
		}
	}

	public override Bounds GetLocalBounds(BoundsContext boundsContext)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		return new Bounds(new Vector3(0f, 1f, 0f), new Vector3(0.8f, 2.2f, 1f));
	}

	public override MVWorldObjectClient Clone(int ownerActorNumber, int cloneGroupId, CloneBookkeeping cloneBookkeeping, Dictionary<int, MVWorldObjectClient> worldObjects, Dictionary<int, RuntimePrototypeCubeModel> prototypes)
	{
		MVBody mVBody = (MVBody)base.Clone(ownerActorNumber, cloneGroupId, cloneBookkeeping, worldObjects, prototypes);
		mVBody.attachedAvatar = null;
		return mVBody;
	}

	public IModelingConstraint GetModelConstaint(MVCubeModelInstance cubeModel)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		IModelingConstraint value = null;
		if (idChildMap.ContainsKey(cubeModel.Id))
		{
			constraints.TryGetValue(cubeModel.Id, out value);
			if (value == null)
			{
				string part = (string)idChildMap[cubeModel.Id];
				Vector3 partConstraintMin = bodyData.GetPartConstraintMin(part);
				Vector3 partConstraintMax = bodyData.GetPartConstraintMax(part);
				int partConstraintMinCount = bodyData.GetPartConstraintMinCount(part);
				value = new ModelingBoxCountConstraint(cubeModel, partConstraintMin.ToIntVector(), partConstraintMax.ToIntVector(), partConstraintMinCount);
				constraints.Add(cubeModel.Id, value);
			}
		}
		return value;
	}

	public override void OnDataUpdate()
	{
		base.OnDataUpdate();
		GetAccessoriesFromBPData();
		if (Data.ContainsKey("ParticlesVisible"))
		{
			accessoryParticlesVisible = (bool)Data["ParticlesVisible"];
			UpdateVisibility();
		}
	}

	public MVCubeModelInstance GetBodyPart(string part)
	{
		return GetChild(part) as MVCubeModelInstance;
	}

	private void AttachCubes()
	{
		foreach (DictionaryEntry item in childIdMap)
		{
			string boneName = (string)item.Key;
			AttachCube(boneName);
		}
		foreach (Renderer renderer in renderers)
		{
			renderer.enabled = visible;
		}
	}

	private void AttachCube(string boneName)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		Transform partBone = bodyData.GetPartBone(boneName);
		MVCubeModelInstance bodyPart = GetBodyPart(boneName);
		attachedPartModels.Add(bodyPart);
		GameObject val = bodyPart.GameObject;
		((Object)val).name = boneName + " model " + bodyPart.Id;
		val.SetLayerRecursively(LayerMask.NameToLayer("Player"));
		modelScale = val.transform.localScale;
		val.transform.parent = partBone;
		val.transform.localPosition = Vector3.zero;
		val.transform.localRotation = Quaternion.identity;
		colliders.Add(val.GetComponentInChildren<Collider>());
		renderers.Add(val.GetComponentInChildren<Renderer>());
		AlignModel(boneName, partBone, val);
		bodyPart.BeingEditedChanged += CubeModelBase_BeingEditedChanged;
	}

	private void CubeModelBase_BeingEditedChanged(object sender, EditStateEventArgs e)
	{
		if (e.BeingEdited)
		{
			return;
		}
		foreach (AvatarAccessory value in accessoryMap.Values)
		{
			if (value.AllignToBody)
			{
				value.Attached = false;
				ApplyAccessoryOffset(value, value.Slot);
				value.Attached = true;
			}
		}
	}

	private void AlignModel(string boneName, Transform bone, GameObject model)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		Vector3 partBoneSpacePosition = bodyData.GetPartBoneSpacePosition(boneName);
		Quaternion val = Quaternion.identity;
		switch (boneName)
		{
		case "Head":
			val = Quaternion.LookRotation(bone.right, bone.up);
			break;
		case "Torso":
			val = Quaternion.LookRotation(-bone.right, bone.up);
			break;
		case "RArm":
			val = Quaternion.LookRotation(-bone.up, -bone.right);
			break;
		case "LArm":
			val = Quaternion.LookRotation(-bone.up, bone.right);
			break;
		case "RUpLeg":
			val = Quaternion.LookRotation(bone.forward, bone.up);
			break;
		case "RLowLeg":
			val = Quaternion.LookRotation(bone.right, bone.up);
			break;
		case "LUpLeg":
			val = Quaternion.LookRotation(bone.forward, bone.up);
			break;
		case "LLowLeg":
			val = Quaternion.LookRotation(bone.right, bone.up);
			break;
		}
		partBoneSpacePosition.Scale(modelScale);
		model.transform.rotation = val;
		model.transform.Translate(partBoneSpacePosition);
	}

	public void StartBlinking(BlinkType type, float duration)
	{
		blinker.StartBlinking(type, duration);
	}

	public void StopBlinking(BlinkType type)
	{
		blinker.StopBlinking(type);
	}
}
