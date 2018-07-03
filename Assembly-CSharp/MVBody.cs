using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class MVBody : MVBlueprintBase, IWorldObjectWithModelingConstraint
{
	private BodyAccessoriesController bodyAccessoriesController;

	private BodyAccessoriesController previewBodyAccessoriesController;

	private bool collidersEnabled = true;

	private bool shadowVisible = true;

	private bool visible = true;

	private MVBodyObject bodyObject;

	private Dictionary<int, IModelingConstraint> constraints = new Dictionary<int, IModelingConstraint>();

	private List<Renderer> renderers = new List<Renderer>();

	private List<Collider> colliders = new List<Collider>();

	private List<MVCubeModelInstance> attachedPartModels = new List<MVCubeModelInstance>();

	private bool initialized;

	private MVAvatar attachedAvatar;

	private Vector3 modelScale = Vector3.zero;

	private BodyClone bodyClone;

	public BoneAnimation Animation => bodyObject.BoneAnimation;

	public MVAvatar AttachedAvatar => attachedAvatar;

	public BodyData BodyData => bodyObject.BodyData;

	public List<MVCubeModelInstance> AttachedParts => attachedPartModels;

	public AvatarBlobShadowController BlobShadow => bodyObject.AvatarBlobShadowController;

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
			return bodyAccessoriesController.AccessoryMoveOverride;
		}
		set
		{
			bodyAccessoriesController.AccessoryMoveOverride = value;
		}
	}

	public MVBody(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVBodyPrefab, worldObjects)
	{
		bodyObject = (MVBodyObject)component;
		MVWorldObjectClient value = null;
		if (worldObjects.TryGetValue(groupId, out value))
		{
			attachedAvatar = value as MVAvatar;
		}
		previewLayerMask |= LayerFlags.Player;
		gameObject.layer = LayerMask.NameToLayer("Player");
		if (!blueprintData.ContainsKey(BlueprintData.AvatarAccessoryData2.ToString("d")))
		{
			Debug.LogWarning("Accessory data not found. This should only happen in the avatar shop. This is due to the fact that the server sends the body data to the clients w/o deserializing.");
			blueprintData[BlueprintData.AvatarAccessoryData2.ToString("d")] = new Dictionary<object, object>();
		}
	}

	public bool IsAccessorySlotOccupied(AccessorySlotType accessorySlotType)
	{
		return bodyAccessoriesController.IsAccessorySlotOccupied(accessorySlotType);
	}

	public float GetAccessoryOffset(AccessorySlotType slot)
	{
		if (previewBodyAccessoriesController != null)
		{
			return previewBodyAccessoriesController.GetOffset(slot);
		}
		return bodyAccessoriesController.GetOffset(slot);
	}

	public void ApplyAccessoryOffset(float yOffset, AccessorySlotType slot)
	{
		Dictionary<object, object> accessoryData;
		if (previewBodyAccessoriesController != null)
		{
			previewBodyAccessoriesController.ApplyAccessoryOffset(yOffset, slot);
			accessoryData = previewBodyAccessoriesController.AccessoryData;
		}
		else
		{
			bodyAccessoriesController.ApplyAccessoryOffset(yOffset, slot);
			accessoryData = bodyAccessoriesController.AccessoryData;
		}
		UpdateBodyClone(accessoryData);
	}

	public float GetAccessoryScale(AccessorySlotType slot)
	{
		if (previewBodyAccessoriesController != null)
		{
			return previewBodyAccessoriesController.GetScale(slot);
		}
		return bodyAccessoriesController.GetScale(slot);
	}

	public void ApplyAccessorySize(float size, AccessorySlotType slot)
	{
		Dictionary<object, object> accessoryData;
		if (previewBodyAccessoriesController != null)
		{
			previewBodyAccessoriesController.ApplySizeChange(size, slot);
			accessoryData = previewBodyAccessoriesController.AccessoryData;
		}
		else
		{
			bodyAccessoriesController.ApplySizeChange(size, slot);
			accessoryData = bodyAccessoriesController.AccessoryData;
		}
		UpdateBodyClone(accessoryData);
	}

	public bool IsAccessoryEquipped(int streamingAssetId)
	{
		return bodyAccessoriesController.IsAccessoryEquipped(streamingAssetId);
	}

	public GameObject CreateClone()
	{
		Debug.LogWarning("I think it is preferable that the clone is solely owned by the body. ");
		GameObject gameObject = Object.Instantiate(GameObject);
		gameObject.transform.parent = null;
		Debug.LogWarning("All colliders must be removed");
		gameObject.transform.position = gameObject.transform.position + Vector3.up * 10f;
		bodyClone = gameObject.AddComponent<BodyClone>();
		AvatarAccessory[] componentsInChildren = bodyClone.GetComponentsInChildren<AvatarAccessory>();
		for (int num = componentsInChildren.Length - 1; num >= 0; num--)
		{
			Object.Destroy(componentsInChildren[num].gameObject);
		}
		bodyClone.Initialize(Id, GetAccessoryData());
		return gameObject;
	}

	public void DestroyClone()
	{
		if (!(bodyClone == null))
		{
			bodyClone.Destroy();
			Object.Destroy(bodyClone.gameObject);
		}
	}

	public void PreviewAccessory(AccessoryDataClient viewItem)
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary = HashtableFunctions.DeepCopyHashTable(GetAccessoryData());
		Dictionary<object, object> dictionary2 = new Dictionary<object, object>();
		dictionary2[AvatarAccessoryData.Slot.ToString("d")] = (int)viewItem.accessorySlotType;
		dictionary2[AvatarAccessoryData.InventoryID.ToString("d")] = viewItem.streamingAssetID;
		dictionary2[AvatarAccessoryData.Offset.ToString("d")] = 0f;
		dictionary2[AvatarAccessoryData.Scale.ToString("d")] = 1f;
		dictionary2[AvatarAccessoryData.AssetPath.ToString("d")] = viewItem.url;
		Dictionary<object, object> dictionary3 = dictionary;
		int accessorySlotType = (int)viewItem.accessorySlotType;
		dictionary3[accessorySlotType.ToString()] = dictionary2;
		bodyAccessoriesController.UpdateAccessoryVisibility(visible: false);
		bodyAccessoriesController.AccessoryMoveOverride = false;
		previewBodyAccessoriesController = new BodyAccessoriesController(Id, BodyData, dictionary, isVisible: true);
		previewBodyAccessoriesController.RefreshAccessories(dictionary);
		previewBodyAccessoriesController.AccessoryMoveOverride = true;
		UpdateBodyClone(previewBodyAccessoriesController.AccessoryData);
	}

	public void EndPreviewAccessory()
	{
		previewBodyAccessoriesController.Destroy();
		previewBodyAccessoriesController = null;
		bodyAccessoriesController.UpdateAccessoryVisibility(visible: true);
		bodyAccessoriesController.AccessoryMoveOverride = true;
		UpdateBodyClone(bodyAccessoriesController.AccessoryData);
	}

	private void UpdateBodyClone(Dictionary<object, object> accessoryData)
	{
		if (bodyClone != null)
		{
			bodyClone.RefreshAccessories(accessoryData);
		}
	}

	public void SyncOffset(AccessorySlotType slot, float offset)
	{
		if (previewBodyAccessoriesController == null)
		{
			MVGameControllerBase.OperationRequests.UpdateAvatarAccessoryOffset(id, slot, offset);
		}
	}

	public void SyncScale(AccessorySlotType slot, float scale)
	{
		if (previewBodyAccessoriesController == null)
		{
			MVGameControllerBase.OperationRequests.UpdateAvatarAccessoryScale(id, slot, scale);
		}
	}

	public override void Initialize()
	{
		if (initialized)
		{
			Debug.LogWarning("Trying to initialize body " + id + " more than once");
			return;
		}
		base.Initialize();
		MeshFilter[] componentsInChildren = gameObject.GetComponentsInChildren<MeshFilter>();
		bodyObject.AvatarBlinker.MeshFilters = componentsInChildren;
		InitializeCommon();
		if (attachedAvatar != null)
		{
			CollidersEnabled = false;
			BlobShadow.enabled = true;
		}
		else
		{
			CollidersEnabled = true;
			BlobShadow.enabled = false;
		}
		initialized = true;
		UpdateVisibility();
	}

	public void InitializeHealth(float currentHealthAmount)
	{
		bodyObject.AvatarBlinker.SetPreviousHealth(currentHealthAmount);
	}

	public void InitializeShield(float currentShieldAmount)
	{
		bodyObject.AvatarBlinker.SetPreviousShield(currentShieldAmount);
	}

	public void UpdateBlinking()
	{
		bodyObject.AvatarBlinker.UpdateBlinking();
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		InitializeCommon();
		CollidersEnabled = false;
		BlobShadow.enabled = false;
	}

	public override void Destroy()
	{
		base.Destroy();
		bodyAccessoriesController.Destroy();
		DestroyClone();
	}

	public void Attach(MVAvatar mvAvatar, bool isLocal)
	{
		attachedAvatar = mvAvatar;
		if (attachedAvatar != null)
		{
			if (bodyObject.BoneAnimation != null)
			{
				bodyObject.BoneAnimation.Attach(attachedAvatar, isLocal);
			}
			if (bodyObject.AvatarBlinker != null)
			{
				bodyObject.AvatarBlinker.Visible = visible;
				bodyObject.AvatarBlinker.Attach(mvAvatar);
			}
			CollidersEnabled = false;
			BlobShadow.enabled = true;
		}
	}

	public void Detach()
	{
		bodyObject.BoneAnimation.Detach();
		if (bodyObject.AvatarBlinker != null)
		{
			bodyObject.AvatarBlinker.Detach();
		}
		BlobShadow.enabled = false;
	}

	public void StartBlinking(BlinkType type, float duration)
	{
		bodyObject.AvatarBlinker.StartBlinking(type, duration);
	}

	public void StopBlinking(BlinkType type)
	{
		bodyObject.AvatarBlinker.StopBlinking(type);
	}

	public void ToggleBlinking(bool shouldShowBlinking)
	{
		bodyObject.AvatarBlinker.Visible = shouldShowBlinking;
	}

	public GameObject CopyByValue()
	{
		bool flag = Visible;
		if (!Visible)
		{
			Visible = true;
		}
		GameObject gameObject = Object.Instantiate(GameObject);
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
				if (!(material == null))
				{
					Material item = new Material(material);
					list.Add(item);
				}
			}
			meshRenderer.materials = list.ToArray();
		}
	}

	private void InitializeCommon()
	{
		AttachCubes();
		bodyAccessoriesController = new BodyAccessoriesController(Id, BodyData, new Dictionary<object, object>(), visible);
		RefreshAccessories();
		Animation.enabled = true;
	}

	private void RefreshAccessories()
	{
		bodyAccessoriesController.RefreshAccessories(GetAccessoryData());
		UpdateBodyClone(bodyAccessoriesController.AccessoryData);
	}

	private Dictionary<object, object> GetAccessoryData()
	{
		return (Dictionary<object, object>)blueprintData[BlueprintData.AvatarAccessoryData2.ToString("d")];
	}

	private void UpdateVisibility()
	{
		foreach (Renderer renderer in renderers)
		{
			renderer.enabled = visible;
		}
		if (shadowVisible && BlobShadow != null)
		{
			BlobShadow.enabled = visible;
		}
		if (bodyObject.AvatarBlinker != null)
		{
			bodyObject.AvatarBlinker.Visible = visible;
		}
		bodyAccessoriesController.UpdateAccessoryVisibility(visible);
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
				Vector3 partConstraintMin = BodyData.GetPartConstraintMin(part);
				Vector3 partConstraintMax = BodyData.GetPartConstraintMax(part);
				int partConstraintMinCount = BodyData.GetPartConstraintMinCount(part);
				value = new ModelingBoxCountConstraint(cubeModel, partConstraintMin.ToIntVector(), partConstraintMax.ToIntVector(), partConstraintMinCount);
				constraints.Add(cubeModel.Id, value);
			}
		}
		return value;
	}

	public override void OnDataUpdate()
	{
		base.OnDataUpdate();
		Debug.LogWarning("TODO In case of partial update this should be done only if accessory info changed");
		RefreshAccessories();
		Debug.Log("OnDataUpdate");
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
		Transform partBone = BodyData.GetPartBone(boneName);
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
	}

	private void AlignModel(string boneName, Transform bone, GameObject model)
	{
		Vector3 partBoneSpacePosition = BodyData.GetPartBoneSpacePosition(boneName);
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
