using System.Collections.Generic;
using Assets.Scripts.WorldObjectTypes.Avatar.Accessories;
using MV.Common;
using UnityEngine;

public class BodyAccessoriesController
{
	private static Dictionary<AccessorySlotType, string> slotBoneNameMap = new Dictionary<AccessorySlotType, string>
	{
		{
			AccessorySlotType.Torso,
			"Torso"
		},
		{
			AccessorySlotType.Head,
			"Head"
		},
		{
			AccessorySlotType.Back,
			"Torso"
		}
	};

	private static Dictionary<AccessorySlotType, Vector3> slotBoneOffset = new Dictionary<AccessorySlotType, Vector3>
	{
		{
			AccessorySlotType.Head,
			new Vector3(0f, 0.6f, 0.07f)
		},
		{
			AccessorySlotType.Torso,
			new Vector3(0f, 0f, 0f)
		},
		{
			AccessorySlotType.Back,
			new Vector3(0f, 0.4342f, -0.36f)
		}
	};

	private bool accessoryMoveOverride;

	private Dictionary<AccessorySlotType, AvatarAccessory> accessoryMap = new Dictionary<AccessorySlotType, AvatarAccessory>();

	private AccessoryLoader accessoryLoader = new AccessoryLoader();

	private Dictionary<object, object> accessoryData;

	private BodyData bodyData;

	private int bodyWoId;

	private bool accessoriesVisible;

	public Dictionary<object, object> AccessoryData => accessoryData;

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
				foreach (KeyValuePair<AccessorySlotType, AvatarAccessory> item in accessoryMap)
				{
					MakeAccessorySelectable(item.Value, item.Key);
				}
				return;
			}
			SelectionHelperAvatarAccessory[] componentsInChildren = bodyData.gameObject.GetComponentsInChildren<SelectionHelperAvatarAccessory>(includeInactive: true);
			SelectionHelperAvatarAccessory[] array = componentsInChildren;
			foreach (SelectionHelperAvatarAccessory selectionHelperAvatarAccessory in array)
			{
				Object.Destroy(selectionHelperAvatarAccessory.gameObject);
			}
		}
	}

	public BodyAccessoriesController(int bodyWoId, BodyData bodyData, Dictionary<object, object> accessoryData, bool isVisible)
	{
		this.bodyWoId = bodyWoId;
		this.bodyData = bodyData;
		this.accessoryData = accessoryData;
		accessoriesVisible = isVisible;
	}

	public float GetOffset(AccessorySlotType accessorySlot)
	{
		Dictionary<object, object> dictionary = accessoryData;
		int num = (int)accessorySlot;
		Dictionary<object, object> dictionary2 = (Dictionary<object, object>)dictionary[num.ToString()];
		return (float)dictionary2[AvatarAccessoryData.Offset.ToString("d")];
	}

	public float GetScale(AccessorySlotType accessorySlot)
	{
		Dictionary<object, object> dictionary = accessoryData;
		int num = (int)accessorySlot;
		Dictionary<object, object> dictionary2 = (Dictionary<object, object>)dictionary[num.ToString()];
		return (float)dictionary2[AvatarAccessoryData.Scale.ToString("d")];
	}

	public bool IsAccessoryEquipped(int streamingAssetId)
	{
		if (AccessoryData == null)
		{
			return false;
		}
		foreach (KeyValuePair<object, object> accessoryDatum in AccessoryData)
		{
			Dictionary<object, object> dictionary = (Dictionary<object, object>)accessoryDatum.Value;
			int num = (int)dictionary[AvatarAccessoryData.InventoryID.ToString("d")];
			if (num == streamingAssetId)
			{
				return true;
			}
		}
		return false;
	}

	public void Destroy()
	{
		if (accessoryLoader != null)
		{
			accessoryLoader.Destroy();
		}
		accessoryLoader = null;
		foreach (KeyValuePair<AccessorySlotType, AvatarAccessory> item in accessoryMap)
		{
			Object.Destroy(item.Value.gameObject);
		}
		accessoryMap = null;
		accessoryData = null;
	}

	public Vector3 GetSlotPosition(AccessorySlotType slot, Vector3 offset)
	{
		string text = slotBoneNameMap[slot];
		Transform partBone = bodyData.GetPartBone(text);
		if (partBone == null)
		{
			Debug.LogError($"Accessory: Failed to get bone {text} for slot {slot}");
		}
		Vector3 position = partBone.position;
		position += partBone.right * offset.x;
		position += partBone.up * offset.y;
		return position + partBone.forward * offset.z;
	}

	private bool AttachAccessory(AvatarAccessory acc, AccessorySlotType slot, float offset, float scale)
	{
		if (!accessoryMap.ContainsKey(slot))
		{
			accessoryMap.Add(slot, acc);
		}
		else
		{
			Debug.LogError("Trying to add accessory a second time! This should be fixed in refresh.");
			Debug.LogWarning("Out commented return false");
		}
		acc.Transform.parent = GetSlotTransform(slot);
		UpdateOffset(slot, offset);
		UpdateScale(slot, scale);
		Collider[] colliders = acc.Colliders;
		foreach (Collider collider in colliders)
		{
			collider.enabled = false;
		}
		acc.Transform.SetLayerRecursively(bodyData.gameObject.layer);
		acc.Visible = true;
		acc.gameObject.AddComponent<FadeableObject>();
		return true;
	}

	private Transform GetSlotTransform(AccessorySlotType slot)
	{
		return bodyData.GetPartBone(slotBoneNameMap[slot]);
	}

	public void UpdateAccessoryVisibility(bool visible)
	{
		foreach (AvatarAccessory value in accessoryMap.Values)
		{
			value.Visible = visible;
		}
		accessoriesVisible = visible;
	}

	public void ApplySizeChange(float size, AccessorySlotType slot)
	{
		accessoryMap[slot].Scale = size;
		accessoryMap[slot].transform.localScale = Vector3.one * size;
		Dictionary<object, object> dictionary = accessoryData;
		int num = (int)slot;
		Dictionary<object, object> dictionary2 = (Dictionary<object, object>)dictionary[num.ToString()];
		dictionary2[AvatarAccessoryData.Scale.ToString("d")] = size;
	}

	public void ApplyAccessoryOffset(float yOffset, AccessorySlotType slot)
	{
		accessoryMap[slot].Transform.localPosition = Vector3.zero + slotBoneOffset[slot] + Vector3.up * yOffset;
		accessoryMap[slot].Transform.localRotation = Quaternion.identity;
		Dictionary<object, object> dictionary = accessoryData;
		int num = (int)slot;
		Dictionary<object, object> dictionary2 = (Dictionary<object, object>)dictionary[num.ToString()];
		dictionary2[AvatarAccessoryData.Offset.ToString("d")] = yOffset;
	}

	public void RefreshAccessories(Dictionary<object, object> accessoryData)
	{
		this.accessoryData = accessoryData;
		List<AccessorySlotType> list = new List<AccessorySlotType>();
		foreach (KeyValuePair<AccessorySlotType, AvatarAccessory> item in accessoryMap)
		{
			if (!IsAccessoryInWoData(item.Value))
			{
				list.Add(item.Key);
			}
		}
		foreach (AccessorySlotType item2 in list)
		{
			DestroyAccessory(item2);
		}
		foreach (KeyValuePair<object, object> accessoryDatum in AccessoryData)
		{
			string s = (string)accessoryDatum.Key;
			int result = -1;
			if (int.TryParse(s, out result))
			{
				AccessorySlotType slot = (AccessorySlotType)result;
				Dictionary<object, object> dictionary = (Dictionary<object, object>)accessoryDatum.Value;
				string text = (string)dictionary[AvatarAccessoryData.AssetPath.ToString("d")];
				if (!accessoryMap.ContainsKey(slot) || accessoryMap[slot].AssetPath != text)
				{
					accessoryLoader.LoadAccessory(text, (AvatarAccessory accessory) =>
					{
						LoadedAccessoryCallback(accessory, slot);
					});
				}
				else if (accessoryMap.ContainsKey(slot) && accessoryMap[slot].AssetPath == text)
				{
					float offset = (float)dictionary[AvatarAccessoryData.Offset.ToString("d")];
					UpdateOffset(slot, offset);
					float scale = (float)dictionary[AvatarAccessoryData.Scale.ToString("d")];
					UpdateScale(slot, scale);
				}
			}
			else
			{
				Debug.LogError("Failed to parse slot");
			}
		}
	}

	private void UpdateScale(AccessorySlotType slot, float scale)
	{
		ApplySizeChange(scale, slot);
	}

	private void UpdateOffset(AccessorySlotType slot, float offset)
	{
		ApplyAccessoryOffset(offset, slot);
	}

	private void DestroyAccessory(AccessorySlotType slot)
	{
		AvatarAccessory avatarAccessory = accessoryMap[slot];
		avatarAccessory.Transform.parent = null;
		accessoryMap.Remove(slot);
		Object.Destroy(avatarAccessory.gameObject);
	}

	private bool IsAccessoryInWoData(AvatarAccessory avatarAccessory)
	{
		foreach (Dictionary<object, object> value in AccessoryData.Values)
		{
			string text = (string)value[AvatarAccessoryData.AssetPath.ToString("d")];
			if (text == avatarAccessory.AssetPath)
			{
				return true;
			}
		}
		return false;
	}

	private void LoadedAccessoryCallback(AvatarAccessory accessory, AccessorySlotType slot)
	{
		if (accessory == null)
		{
			Debug.LogError("Failed to load accessory!");
			return;
		}
		Dictionary<object, object> dictionary = AccessoryData;
		int num = (int)slot;
		Dictionary<object, object> dictionary2 = (Dictionary<object, object>)dictionary[num.ToString()];
		string value = (string)dictionary2[AvatarAccessoryData.AssetPath.ToString("d")];
		if (accessory.AssetPath.Contains(value))
		{
			float offset = (float)dictionary2[AvatarAccessoryData.Offset.ToString("d")];
			float scale = (float)dictionary2[AvatarAccessoryData.Scale.ToString("d")];
			if (AttachAccessory(accessory, slot, offset, scale) && AccessoryShouldBeSelecable(accessory) && accessoryMoveOverride)
			{
				MakeAccessorySelectable(accessory, slot);
			}
		}
		UpdateAccessoryVisibility(accessoriesVisible);
	}

	public bool IsAccessorySlotOccupied(AccessorySlotType accessorySlotType)
	{
		Dictionary<object, object> dictionary = AccessoryData;
		int num = (int)accessorySlotType;
		return dictionary.ContainsKey(num.ToString());
	}

	private bool AccessoryShouldBeSelecable(AvatarAccessory accessory)
	{
		if (accessoryMoveOverride && accessory.GetType() == typeof(AvatarAccessoryHat))
		{
			return true;
		}
		if (accessoryMoveOverride && accessory.GetType() == typeof(AvatarAccessoryBackAccessories))
		{
			return true;
		}
		if (MVGameControllerBase.GameMode != MVGameMode.CharacterEditor)
		{
			return false;
		}
		if (accessory.GetType() == typeof(AvatarAccessoryHat))
		{
			return false;
		}
		return true;
	}

	private void MakeAccessorySelectable(AvatarAccessory accessory, AccessorySlotType accessorySlot)
	{
		int accessoryStreamingAssetsId = -1;
		if (AccessoryData != null)
		{
			foreach (KeyValuePair<object, object> accessoryDatum in AccessoryData)
			{
				Dictionary<object, object> dictionary = (Dictionary<object, object>)accessoryDatum.Value;
				if (accessorySlot == (AccessorySlotType)(int)dictionary[AvatarAccessoryData.Slot.ToString("d")])
				{
					accessoryStreamingAssetsId = (int)dictionary[AvatarAccessoryData.InventoryID.ToString("d")];
				}
			}
		}
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
			selectionHelperAvatarAccessory.Init(accessory, accessorySlot, bodyWoId, accessoryStreamingAssetsId);
		}
	}
}
