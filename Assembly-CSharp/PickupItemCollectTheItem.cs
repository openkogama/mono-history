using System;
using System.Collections;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.Events;

public class PickupItemCollectTheItem : PickupItem
{
	[SerializeField]
	private Transform cubeModelAttachPoint;

	[SerializeField]
	private float pickupScale = 0.15f;

	private ObjectiveArrow arrow;

	private MVWorldObjectClient woDropOff;

	private int cubeModelKeyId;

	private int spawnerId;

	private int dropOffId;

	private GameObject pickup;

	private bool shouldSpawnInstanceOnUnequip = true;

	public override AvatarItemType Type => AvatarItemType.CollectTheItemCollectable;

	public override bool ActivateGunModeOnEquip => false;

	public override void UpdateWithDirection(Vector3 dir)
	{
	}

	public override void OnLeaveVehicleWithWeapon()
	{
		base.OnLeaveVehicleWithWeapon();
		if (arrow != null)
		{
			arrow.gameObject.SetActive(value: false);
		}
	}

	public override void OnEnterVehicleWithWeapon()
	{
		base.OnEnterVehicleWithWeapon();
		if (arrow != null)
		{
			arrow.gameObject.SetActive(value: true);
		}
	}

	public override void OnUnequip()
	{
		base.OnUnequip();
		if (owner.IsLocal && MVGameControllerBase.WOCM.GetWorldObjectClient(spawnerId) is CollectTheItemCollectable collectTheItemCollectable)
		{
			collectTheItemCollectable.OnCollectTheItemDestroyed = (Action)Delegate.Remove(collectTheItemCollectable.OnCollectTheItemDestroyed, new Action(OnWorldObjectSpawnerDestroyed));
			if (shouldSpawnInstanceOnUnequip)
			{
				collectTheItemCollectable.CreateCollectableInstance(pickup.transform.position, default);
			}
		}
	}

	public void SetShouldSpawnInstanceOnUnequip(bool shouldSpawnInstance)
	{
		shouldSpawnInstanceOnUnequip = shouldSpawnInstance;
	}

	public override void OnEquip()
	{
		base.OnEquip();
	}

	public int GetCubeModelKeyId()
	{
		return cubeModelKeyId;
	}

	private void OnDropoffActivated(bool shouldStayEquipped)
	{
		if (!shouldStayEquipped)
		{
			ParticleSystem particleSystem = UnityEngine.Object.Instantiate(PrefabPool.Instance.CollectTheItemParticles);
			particleSystem.transform.position = pickup.transform.position;
			if (arrow != null)
			{
				arrow.gameObject.SetActive(value: false);
			}
			NotificationController.PushNotification(TM._("A player has delivered the carried item!"), null, 7);
			ForceUnequipPickup();
		}
	}

	public override void OnStateChanged(Dictionary<object, object> newState)
	{
		if (pickup != null)
		{
			return;
		}
		Dictionary<object, object> dictionary = (Dictionary<object, object>)newState["itemData"];
		int id = (int)dictionary["CollectTheItemCollectableId"];
		if (!(MVGameControllerBase.WOCM.GetWorldObjectClient(id) is CollectTheItemCollectable collectTheItemCollectable))
		{
			ForceUnequipPickup();
			return;
		}
		cubeModelKeyId = collectTheItemCollectable.CollectableModelId;
		spawnerId = id;
		dropOffId = collectTheItemCollectable.DropOffId;
		if (cubeModelKeyId == -1 || dropOffId == -1)
		{
			ForceUnequipPickup();
			return;
		}
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(cubeModelKeyId);
		if (worldObjectClient == null)
		{
			return;
		}
		SetupPickupWo(worldObjectClient);
		if (!(owner == null) && owner.IsLocal && MVGameControllerBase.WOCM.GetWorldObjectClient(dropOffId) is CollectTheItemDropOff collectTheItemDropOff)
		{
			collectTheItemDropOff.OnPickupCollected = (Action<bool>)Delegate.Combine(collectTheItemDropOff.OnPickupCollected, new Action<bool>(OnDropoffActivated));
			if (collectTheItemCollectable.HasArrowIndicator)
			{
				SetupDropOffArrow(dropOffId);
			}
		}
	}

	private void SetupDropOffArrow(int dropoffId)
	{
		woDropOff = MVGameControllerBase.WOCM.GetWorldObjectClient(dropoffId);
		arrow = UnityEngine.Object.Instantiate(PrefabPool.Instance.CollectTheItemDropOffArrowPrefab);
		MVWorldObjectClient mVWorldObjectClient = woDropOff;
		mVWorldObjectClient.PositionChanged = (UnityAction<MVWorldObjectClient, PositionChangedEventArgs>)Delegate.Combine(mVWorldObjectClient.PositionChanged, new UnityAction<MVWorldObjectClient, PositionChangedEventArgs>(arrow.OnPositionChanged));
		arrow.Initialize(transform.position + transform.forward * 2f, woDropOff.Transform, transform);
		arrow.gameObject.SetActive(value: true);
	}

	private void SetupPickupWo(MVWorldObjectClient wo)
	{
		pickup = CreateMeshClone((MVCubeModelInstance)wo);
		pickup.transform.SetParent(cubeModelAttachPoint);
		MVAvatar mVAvatar = owner.WorldObjectOwner as MVAvatar;
		pickup.transform.localRotation = default;
		if (mVAvatar != null)
		{
			Transform parent = mVAvatar.Body.BodyData.GetPartBone(BodyData.PartIndex.Torso).parent;
			pickup.transform.localRotation = parent.transform.localRotation;
		}
		pickup.transform.localPosition = new Vector3(0f, 0f, 0f);
		pickup.transform.localScale = new Vector3(pickupScale, pickupScale, pickupScale);
		if (!(MVGameControllerBase.WOCM.GetWorldObjectClient(spawnerId) is CollectTheItemCollectable collectTheItemCollectable))
		{
			ForceUnequipPickup();
		}
		else
		{
			collectTheItemCollectable.OnCollectTheItemDestroyed = (Action)Delegate.Combine(collectTheItemCollectable.OnCollectTheItemDestroyed, new Action(OnWorldObjectSpawnerDestroyed));
		}
	}

	private void OnWorldObjectSpawnerDestroyed()
	{
		if (!(arrow == null))
		{
			arrow.gameObject.SetActive(value: false);
			ForceUnequipPickup();
		}
	}

	private void ForceUnequipPickup()
	{
		shouldSpawnInstanceOnUnequip = false;
		MVEquipable component = owner.WorldObjectOwner.GameObject.GetComponent<MVEquipable>();
		if (component != null)
		{
			component.Unequip();
		}
	}

	private void OnDestroy()
	{
		if (!MVGameControllerBase.IsAlive || MVGameControllerBase.Game == null)
		{
			return;
		}
		if (MVGameControllerBase.WOCM.GetWorldObjectClient(spawnerId) is CollectTheItemCollectable collectTheItemCollectable)
		{
			collectTheItemCollectable.OnCollectTheItemDestroyed = (Action)Delegate.Remove(collectTheItemCollectable.OnCollectTheItemDestroyed, new Action(OnWorldObjectSpawnerDestroyed));
		}
		if (!(MVGameControllerBase.WOCM.GetWorldObjectClient(dropOffId) is CollectTheItemDropOff collectTheItemDropOff))
		{
			return;
		}
		collectTheItemDropOff.OnPickupCollected = (Action<bool>)Delegate.Remove(collectTheItemDropOff.OnPickupCollected, new Action<bool>(OnDropoffActivated));
		if (arrow != null)
		{
			if (woDropOff != null)
			{
				MVWorldObjectClient mVWorldObjectClient = woDropOff;
				mVWorldObjectClient.PositionChanged = (UnityAction<MVWorldObjectClient, PositionChangedEventArgs>)Delegate.Remove(mVWorldObjectClient.PositionChanged, new UnityAction<MVWorldObjectClient, PositionChangedEventArgs>(arrow.OnPositionChanged));
			}
			UnityEngine.Object.Destroy(arrow.gameObject);
			arrow = null;
		}
	}

	private static GameObject CreateMeshClone(MVCubeModelInstance cmb)
	{
		GameObject gameObject = new GameObject(cmb.GameObject.name);
		gameObject.transform.localScale = cmb.Transform.localScale;
		foreach (KeyValuePair<IntVector, ChunkInstances.ChunkInstanceVariables> item in (IEnumerable)cmb.ChunkInstances)
		{
			GameObject gameObject2 = item.Value.gameObject;
			GameObject gameObject3 = UnityEngine.Object.Instantiate(gameObject2);
			gameObject3.GetComponent<Renderer>().sharedMaterial = MVGameControllerBase.MaterialLoader.CubeModelMaterial;
			Collider[] componentsInChildren = gameObject3.GetComponentsInChildren<Collider>();
			for (int num = componentsInChildren.Length - 1; num >= 0; num--)
			{
				componentsInChildren[num].enabled = false;
			}
			gameObject3.transform.parent = gameObject.transform;
			gameObject3.transform.localPosition = gameObject2.transform.localPosition;
			gameObject3.transform.localRotation = gameObject2.transform.localRotation;
			gameObject3.transform.localScale = gameObject2.transform.localScale;
		}
		return gameObject;
	}
}
