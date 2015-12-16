using System;
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

	private Dictionary<object, object> accessoryData;

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

	private bool accessoryMoveOverride;

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

	private MVNetworkGame Game => MVGameControllerBase.Game;

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

	public bool AccessoryMoveOverride
	{
		get
		{
			return accessoryMoveOverride;
		}
		set
		{
			accessoryMoveOverride = value;
			if (accessoryMoveOverride)
			{
				MakeAccessoriesSelectable();
				return;
			}
			SelectionHelperAvatarAccessory[] componentsInChildren = gameObject.GetComponentsInChildren<SelectionHelperAvatarAccessory>(includeInactive: true);
			SelectionHelperAvatarAccessory[] array = componentsInChildren;
			foreach (SelectionHelperAvatarAccessory selectionHelperAvatarAccessory in array)
			{
				UnityEngine.Object.Destroy(selectionHelperAvatarAccessory.gameObject);
			}
		}
	}

	public bool AccessoriesLoaded => pendingAccessoryPositions.Count == 0;

	public event EventHandler<EventArgs> AccessoriesChanged;

	public MVBody(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVBodyPrefab, worldObjects)
	{
		MVWorldObjectClient value = null;
		if (worldObjects.TryGetValue(groupId, out value))
		{
			attachedAvatar = value as MVAvatar;
		}
		GetComponents();
		previewLayerMask |= LayerFlags.Player;
		gameObject.layer = LayerMask.NameToLayer("Player");
	}

	public override void Initialize()
	{
		if (initialized)
		{
			Debug.LogWarning("Trying to initialize body " + id + " more than once");
			return;
		}
		base.Initialize();
		blinker.MeshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
		InitializeCommon();
		if (attachedAvatar != null)
		{
			CollidersEnabled = false;
			shadowBlob.enabled = true;
			attachedPartModels.ForEach((MVCubeModelInstance cmi) =>
			{
				cmi.ReactsToLODChanges = false;
			});
		}
		else
		{
			CollidersEnabled = true;
			shadowBlob.enabled = false;
		}
		initialized = true;
	}

	public void EditorSwapAccessoryAssetPath(int invID, string assetPath)
	{
		if (Application.isEditor)
		{
			if (!accessoryMap.ContainsKey(invID))
			{
				Debug.LogError(string.Format("Tried to swap asset path for of missing accessory, ID " + invID));
				return;
			}
			string keyPath = "BlueprintData\\" + BlueprintData.AvatarAccessoryData.ToString("d") + "\\" + invID + "\\" + AvatarAccessoryData.AssetPath.ToString("d");
			Game.UpdateWorldObjectDataPartial(Id, keyPath, assetPath);
		}
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		InitializeCommon();
		CollidersEnabled = false;
		shadowBlob.enabled = false;
		attachedPartModels.ForEach((MVCubeModelInstance cmi) =>
		{
			cmi.ReactsToLODChanges = false;
		});
	}

	public override void Destroy()
	{
		base.Destroy();
		AvatarAccessory[] array = accessoryMap.Values.ToArray();
		foreach (AvatarAccessory acc in array)
		{
			try
			{
				DestroyAccessory(acc, calledFromDestroy: true);
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
		}
	}

	public Vector3 GetSlotPosition(AvatarAccessorySlot slot, Vector3 offset)
	{
		if (slot == AvatarAccessorySlot.WholeBody)
		{
			return gameObject.transform.position + GetLocalBounds(BoundsContext.Preview).center + offset;
		}
		string text = slotBoneNameMap[slot];
		Transform partBone = bodyData.GetPartBone(text);
		if (partBone == null)
		{
			Debug.LogError($"Accessory: Failed to get bone {text} for slot {slot}");
		}
		Vector3 vector = partBone.position;
		vector += partBone.right * offset.x;
		vector += partBone.up * offset.y;
		return vector + partBone.forward * offset.z;
	}

	public bool AttachAccessory(AvatarAccessory acc, AvatarAccessorySlot slot, float offset)
	{
		if (slot == AvatarAccessorySlot.Undefined || !Enum.IsDefined(typeof(AvatarAccessorySlot), slot))
		{
			Debug.LogError("Trying to attach accessory to an invalid slot " + (int)slot);
			return false;
		}
		if (!acc.AccessorySettings.ValidSlots.Contains(slot))
		{
			Debug.LogError(string.Concat("Trying to attach accessory in slot: ", slot, ", allowed: ", acc.AccessorySettings.ValidSlots.BuildString()));
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
			foreach (Collider collider in array)
			{
				collider.enabled = false;
			}
			Debug.Log("Attached accessory");
			acc.Transform.SetLayerRecursively(gameObject.layer);
			acc.Visible = Visible;
			bool flag = MVGameControllerBase.GameMode != MVGameMode.CharacterEditor || attachedAvatar == null;
			ProductInventoryInfo productInventoryInfo = Game.StreamingAssetInventory.Get(acc.InventoryID);
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
		Debug.LogError("Trying to add accessory " + acc.InventoryID + " second time!");
		return false;
	}

	public void ApplyAccessoryOffset(AvatarAccessory acc, AvatarAccessorySlot slot)
	{
		acc.Transform.localPosition = Vector3.zero;
		acc.Transform.localRotation = Quaternion.identity;
		Vector3 zero = Vector3.zero;
		if (!acc.HasAttachmentPoint)
		{
			zero.y += acc.Offset;
		}
		if (acc is AvatarAccessoryHat && acc.AccessorySettings.AllignToBody)
		{
			MVCubeModelInstance bodyPart = GetBodyPart(slotBoneNameMap[slot]);
			Bounds localBounds = bodyPart.GetLocalBounds(BoundsContext.Default);
			Vector3 vector = localBounds.center + new Vector3(0f, localBounds.extents.y, 0f);
			Vector3 vector2 = bodyPart.Transform.TransformPoint(vector);
			zero.y += acc.Transform.parent.InverseTransformPoint(vector2).y;
		}
		acc.Transform.position = GetSlotPosition(slot, zero);
		if (acc.HasAttachmentPoint)
		{
			Vector3 translation = acc.Transform.position - acc.AttachmentPointWorldPos;
			acc.Transform.Translate(translation, Space.World);
		}
	}

	public void AttachAccessoryPermanent(AvatarAccessory acc, AvatarAccessorySlot slot, float offset, ProductInventoryInfo invInfo)
	{
		if (AttachAccessory(acc, slot, offset))
		{
			MarkAccessoryPermanent(acc, invInfo);
		}
	}

	public void MarkAccessoryPermanent(ProductInventoryInfo invInfo)
	{
		AvatarAccessory value = null;
		accessoryMap.TryGetValue(invInfo.InventoryID, out value);
		if (value == null)
		{
			Debug.LogError("Cant set accessory " + invInfo.InventoryID + " as permanet because it hasn't been added to the body " + id);
		}
		else
		{
			MarkAccessoryPermanent(value, invInfo);
		}
	}

	public void Attach(MVAvatar mvAvatar, bool isLocal)
	{
		attachedAvatar = mvAvatar;
		if (attachedAvatar != null)
		{
			GetComponents();
			if (animation != null)
			{
				animation.Attach(attachedAvatar, isLocal);
			}
			if (blinker != null)
			{
				blinker.Visible = visible;
				blinker.Attach(mvAvatar);
			}
			CollidersEnabled = false;
			shadowBlob.enabled = true;
			attachedPartModels.ForEach((MVCubeModelInstance cmi) =>
			{
				cmi.ReactsToLODChanges = false;
			});
		}
	}

	public void Detach()
	{
		Debug.Log("Detaching body " + id);
		animation.Detach();
		if (blinker != null)
		{
			blinker.Detach();
		}
		shadowBlob.enabled = false;
	}

	public IEnumerable<AvatarAccessory> GetAccessories(AvatarAccessorySlot slot)
	{
		Debug.Log("IEnumerable<AvatarAccessory> GetAccessories(AvatarAccessorySlot slot)");
		return accessoryMap.Values.Where((AvatarAccessory a) => a.Slot == slot);
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
		Debug.Log("int GetAccessoryID(AvatarAccessorySlot slot)");
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
		Debug.LogError("Accessory with inventory ID " + inventoryID + " is not attached");
		return AvatarAccessorySlot.Undefined;
	}

	public void DetachAccessory(AvatarAccessory acc, bool calledFromDestroy = false)
	{
		ProductInventoryInfo productInventoryInfo = Game.StreamingAssetInventory.Get(acc.InventoryID);
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
		Debug.Log("Destroying accessories");
		DetachAccessory(acc, calledFromDestroy);
		UnityEngine.Object.Destroy(acc.gameObject);
	}

	public void DestroyAccessory(int inventoryID)
	{
		if (!accessoryMap.ContainsKey(inventoryID))
		{
			Debug.LogWarning("Trying to detach non-attached avatar accessory with inventoryid: " + inventoryID);
		}
		else
		{
			DestroyAccessory(accessoryMap[inventoryID]);
		}
	}

	public void StartBlinking(BlinkType type, float duration)
	{
		blinker.StartBlinking(type, duration);
	}

	public void StopBlinking(BlinkType type)
	{
		blinker.StopBlinking(type);
	}

	public GameObject CopyByValue()
	{
		bool flag = Visible;
		if (!Visible)
		{
			Visible = true;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(GameObject);
		CopyMaterialsByValue(gameObject);
		Visible = flag;
		return gameObject;
	}

	private void CopyMaterialsByValue(GameObject bodyCloneGO)
	{
		MeshRenderer[] componentsInChildren = bodyCloneGO.GetComponentsInChildren<MeshRenderer>();
		MeshRenderer[] array = componentsInChildren;
		foreach (MeshRenderer meshRenderer in array)
		{
			List<Material> list = new List<Material>();
			Material[] sharedMaterials = meshRenderer.sharedMaterials;
			foreach (Material material in sharedMaterials)
			{
				if (material == null)
				{
					Debug.Log("MeshRendererer.GameObjectName " + meshRenderer.gameObject.name);
					continue;
				}
				Material item = new Material(material);
				list.Add(item);
			}
			meshRenderer.materials = list.ToArray();
		}
	}

	private void InitializeCommon()
	{
		AttachCubes();
		GetAccessoriesFromBPData();
	}

	private void GetComponents()
	{
		if (animation == null)
		{
			animation = gameObject.GetComponent<BoneAnimation>();
		}
		if (blinker == null)
		{
			blinker = gameObject.GetComponent<AvatarBlinker>();
		}
		if (shadowBlob == null)
		{
			shadowBlob = gameObject.GetComponentInChildren<AvatarBlobShadowController>();
		}
		if (bodyData == null)
		{
			bodyData = gameObject.GetComponent<BodyData>();
		}
	}

	private void UpdateVisibility()
	{
		foreach (Renderer renderer in renderers)
		{
			renderer.enabled = visible;
		}
		if (shadowVisible && shadowBlob != null)
		{
			shadowBlob.enabled = visible;
		}
		if (blinker != null)
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

	private void GetAccessoriesFromBPData()
	{
		if (!blueprintData.ContainsKey(BlueprintData.AvatarAccessoryData.ToString("d")))
		{
			return;
		}
		accessoryData = (Dictionary<object, object>)blueprintData[BlueprintData.AvatarAccessoryData.ToString("d")];
		if (accessoryData == null)
		{
			return;
		}
		HashSet<int> hashSet = new HashSet<int>();
		foreach (KeyValuePair<object, object> accessoryDatum in accessoryData)
		{
			Dictionary<object, object> dictionary = (Dictionary<object, object>)accessoryDatum.Value;
			int num = (int)dictionary[AvatarAccessoryData.InventoryID.ToString("d")];
			string assetPath = (string)dictionary[AvatarAccessoryData.AssetPath.ToString("d")];
			DateTime purchaseTime = new DateTime((long)dictionary[AvatarAccessoryData.PurchaseTimeTicks.ToString("d")]);
			int num2 = (int)dictionary[AvatarAccessoryData.RentExpireSeconds.ToString("d")];
			hashSet.Add(num);
			AvatarAccessorySlot slot = (AvatarAccessorySlot)(int)dictionary[AvatarAccessoryData.Slot.ToString("d")];
			float offset = (float)dictionary[AvatarAccessoryData.Offset.ToString("d")];
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
							Debug.LogWarning("Body " + id + " not adding stored expiration data to checker");
						}
					}
					AvatarAccessory.Create(num, assetPath, purchaseTime, num2, LoadedAccessoryCallback);
				}
				else
				{
					Debug.Log("Body " + id + " data contains multiple accessories with ID " + num);
					Debug.LogError("Body data contains multiple accessories with the same inventoryID. This is caused by user hiding and show accessories");
				}
			}
			else
			{
				Debug.LogWarning("Body " + id + " already has attached accessory " + num);
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
			Debug.Log("Body " + id + " removing accessories missing in bpData: " + list.BuildString(eachEntryNewLine: false));
			list.ForEach((AvatarAccessory a) =>
			{
				DestroyAccessory(a);
			});
		}
		MakeAccessoriesSelectable();
	}

	private void MakeAccessoriesSelectable()
	{
		foreach (KeyValuePair<int, AvatarAccessory> item in accessoryMap)
		{
			if (AccessoryShouldBeSelecable(item.Value))
			{
				Debug.Log("Trying to make accessory selectable");
				MakeAccessorySelectable(item.Value, item.Value.Slot);
			}
		}
	}

	private void LoadedAccessoryCallback(AvatarAccessory accessory)
	{
		if (!(accessory == null))
		{
			if (accessory.ExpirationInfo != null)
			{
				Debug.Log("Body " + id + " loaded acccessory: " + accessory.InventoryID + " expiring at " + accessory.ExpirationInfo.RentExpireTime);
			}
			else
			{
				Debug.Log("Body " + id + " loaded acccessory: " + accessory.InventoryID + " NOT expiring");
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
		if (accessoryMoveOverride && accessory.Category == AvatarAccessoryCategory.Hat)
		{
			return true;
		}
		if (MVGameControllerBase.GameMode != MVGameMode.CharacterEditor)
		{
			return false;
		}
		if (accessory.Category != AvatarAccessoryCategory.Hat)
		{
			return false;
		}
		if (Group.GetType() == typeof(MVAvatarLocal))
		{
			return false;
		}
		return true;
	}

	private void MakeAccessorySelectable(AvatarAccessory accessory, AvatarAccessorySlot accessorySlot)
	{
		MeshFilter[] componentsInChildren = accessory.Transform.gameObject.GetComponentsInChildren<MeshFilter>();
		MeshFilter[] array = componentsInChildren;
		foreach (MeshFilter meshFilter in array)
		{
			GameObject gameObject = new GameObject("SelectionHelper");
			gameObject.transform.parent = meshFilter.gameObject.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
			meshCollider.sharedMesh = meshFilter.sharedMesh;
			gameObject.layer = LayerMask.NameToLayer("Hidden");
			SelectionHelperAvatarAccessory selectionHelperAvatarAccessory = gameObject.AddComponent<SelectionHelperAvatarAccessory>();
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

	private void MarkAccessoryPermanent(AvatarAccessory acc, ProductInventoryInfo invInfo)
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary[AvatarAccessoryData.InventoryID.ToString("d")] = acc.InventoryID;
		dictionary[AvatarAccessoryData.Slot.ToString("d")] = acc.Slot;
		dictionary[AvatarAccessoryData.Offset.ToString("d")] = acc.Offset;
		dictionary[AvatarAccessoryData.AssetPath.ToString("d")] = invInfo.ProductInfo.AssetPath;
		dictionary[AvatarAccessoryData.PurchaseTimeTicks.ToString("d")] = invInfo.PurchaseTime.Ticks;
		dictionary[AvatarAccessoryData.RentExpireSeconds.ToString("d")] = (invInfo.IsRented ? invInfo.ProductInfo.ShopInfo.RentExpireSeconds : 0);
		Game.UpdateWorldObjectDataPartial(Id, "BlueprintData\\" + BlueprintData.AvatarAccessoryData.ToString("d") + "\\" + acc.InventoryID, dictionary);
	}

	private void RemoveFromBPData(AvatarAccessory acc)
	{
		if (!blueprintData.ContainsKey(BlueprintData.AvatarAccessoryData.ToString("d")))
		{
			Debug.LogWarning("No avatar accessoryData");
			return;
		}
		Dictionary<object, object> dictionary = (Dictionary<object, object>)blueprintData[BlueprintData.AvatarAccessoryData.ToString("d")];
		if (dictionary != null && dictionary.ContainsKey(acc.InventoryID.ToString()))
		{
			dictionary.Remove(acc.InventoryID.ToString());
			Game.RemoveWorldObjectDataPartial(Id, "BlueprintData\\" + BlueprintData.AvatarAccessoryData.ToString("d") + "\\" + acc.InventoryID);
		}
	}

	public override Bounds GetLocalBounds(BoundsContext boundsContext)
	{
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
		foreach (KeyValuePair<object, object> item in childIdMap)
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
		Transform partBone = bodyData.GetPartBone(boneName);
		MVCubeModelInstance bodyPart = GetBodyPart(boneName);
		attachedPartModels.Add(bodyPart);
		GameObject gameObject = bodyPart.GameObject;
		gameObject.name = boneName + " model " + bodyPart.Id;
		gameObject.SetLayerRecursively(LayerMask.NameToLayer("Player"));
		modelScale = gameObject.transform.localScale;
		gameObject.transform.parent = partBone;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		colliders.Add(gameObject.GetComponentInChildren<Collider>());
		renderers.Add(gameObject.GetComponentInChildren<Renderer>());
		AlignModel(boneName, partBone, gameObject);
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
			if (value.AccessorySettings.AllignToBody)
			{
				value.Attached = false;
				ApplyAccessoryOffset(value, value.Slot);
				value.Attached = true;
			}
		}
	}

	private void AlignModel(string boneName, Transform bone, GameObject model)
	{
		Vector3 partBoneSpacePosition = bodyData.GetPartBoneSpacePosition(boneName);
		Quaternion quaternion = Quaternion.identity;
		switch (boneName)
		{
		case "Head":
			quaternion = Quaternion.LookRotation(bone.right, bone.up);
			break;
		case "Torso":
			quaternion = Quaternion.LookRotation(-bone.right, bone.up);
			break;
		case "RArm":
			quaternion = Quaternion.LookRotation(-bone.up, -bone.right);
			break;
		case "LArm":
			quaternion = Quaternion.LookRotation(-bone.up, bone.right);
			break;
		case "RUpLeg":
			quaternion = Quaternion.LookRotation(bone.forward, bone.up);
			break;
		case "RLowLeg":
			quaternion = Quaternion.LookRotation(bone.right, bone.up);
			break;
		case "LUpLeg":
			quaternion = Quaternion.LookRotation(bone.forward, bone.up);
			break;
		case "LLowLeg":
			quaternion = Quaternion.LookRotation(bone.right, bone.up);
			break;
		}
		partBoneSpacePosition.Scale(modelScale);
		model.transform.rotation = quaternion;
		model.transform.Translate(partBoneSpacePosition);
	}
}
