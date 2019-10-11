using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.EventSystems;

public class EditorWorldObjectCreation : MonoBehaviour, ICloneHandler, IAddItemFromInventory, ICreateNewPrototype, IEventSystemHandler
{
	[SerializeField]
	private ThemeRepository themeRepository;

	private EditorStateMachine esm;

	public void Initialize(EditorStateMachine esm)
	{
		this.esm = esm;
		World world = MVGameControllerBase.Game.World;
		world.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Combine(world.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(WOCM_InitializedGameQueryData));
	}

	public void Clone(MVWorldObjectClient original, bool cloneToRoot, bool setAsPreviewItem, bool goToInsert = false)
	{
		if (goToInsert)
		{
			esm.Data.Add("goToInsert", null);
		}
		esm.Event = EditorEvent.ESWaitForClone;
		MVWorldObjectClientManager wOCM = MVGameControllerBase.WOCM;
		wOCM.CloneWorldObjectTreeResponse = (EventHandler<CloneWorldObjectTreeResponseEventArgs>)Delegate.Combine(wOCM.CloneWorldObjectTreeResponse, new EventHandler<CloneWorldObjectTreeResponseEventArgs>(CloneWorldObjectTreeResponseHandler));
		MVGameControllerBase.WOCM.CloneWorldObjectTree(original, localOwner: false, setAsPreviewItem, cloneToRoot);
	}

	public void OnAddItemFromInventory(InventoryItem item)
	{
		KoGaMaPackageClient koGaMaPackageFromItem = GetKoGaMaPackageFromItem(item);
		if (!ValidateAddItemFromInventory(koGaMaPackageFromItem))
		{
			koGaMaPackageFromItem.Destroy();
			return;
		}
		while (!esm.ParentGroupIsRoot && esm.ParentGroup.HasInteractionFlag(InteractionFlags.CantAddChildren))
		{
			esm.ExitGroup();
		}
		int worldObjectId = -1;
		if (MVGameControllerBase.WOCM.GetUnmodifiedWorldObject(koGaMaPackageFromItem, ref worldObjectId))
		{
			Debug.Log("Found wo for cloning");
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(worldObjectId);
			Clone(worldObjectClient, cloneToRoot: false, setAsPreviewItem: false, goToInsert: true);
		}
		else
		{
			Debug.Log("Creating new wo");
			esm.Event = EditorEvent.ESWaitForSelect;
			Quaternion identity = Quaternion.identity;
			MVGameControllerBase.OperationRequests.AddItemToWorld(item.itemID, esm.ParentGroupID, Vector3.up * 10f, identity, localOwner: false, transferOwnershipToServerOnLeave: true, isPreviewItem: false);
		}
		koGaMaPackageFromItem.Destroy();
	}

	private static KoGaMaPackageClient GetKoGaMaPackageFromItem(InventoryItem item)
	{
		byte[] data = item.data;
		BytePacker koGaMaData = new BytePacker(data);
		KoGaMaPackageClient koGaMaPackageClient = new KoGaMaPackageClient(koGaMaData, readRuntimeValues: false);
		koGaMaPackageClient.InventoryInitialize();
		return koGaMaPackageClient;
	}

	public void OnAddNewPrototype(string name, float scale)
	{
		while (!esm.ParentGroupIsRoot && esm.ParentGroup.HasInteractionFlag(InteractionFlags.CantAddChildren))
		{
			esm.ExitGroup();
		}
		esm.Event = EditorEvent.ESWaitForSelect;
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add((byte)1, scale);
		dictionary.Add((byte)2, esm.CubeModelingStateMachine.CurrentMaterialId);
		dictionary.Add((byte)3, MVGameControllerBase.Game.LocalPlayer.ProfileID);
		Dictionary<object, object> customData = dictionary;
		esm.Data["IsNewPrototype"] = true;
		MVGameControllerBase.OperationRequests.RequestBuiltInItem(BuiltInItem.CubeModel, MVGameControllerBase.WOCM.RootGroup.Id, customData, Vector3.up * 10f, Quaternion.identity, Vector3.one * scale, localOwner: false, transferOwnershipToServerOnLeave: true);
	}

	private void CloneWorldObjectTreeResponseHandler(object sender, CloneWorldObjectTreeResponseEventArgs e)
	{
		if (!e.Success && MVGameControllerBase.GameSessionData.gameMode == MVGameMode.Edit)
		{
			esm.Event = EditorEvent.ESTerrainEdit;
		}
		MVWorldObjectClientManager wOCM = MVGameControllerBase.WOCM;
		wOCM.CloneWorldObjectTreeResponse = (EventHandler<CloneWorldObjectTreeResponseEventArgs>)Delegate.Remove(wOCM.CloneWorldObjectTreeResponse, new EventHandler<CloneWorldObjectTreeResponseEventArgs>(CloneWorldObjectTreeResponseHandler));
		esm.SelectWO(e.RootId, addToSelection: false);
	}

	private bool ValidateAddItemFromInventory(KoGaMaPackageClient package)
	{
		foreach (MVWorldObjectClient value in package.worldObjects.Values)
		{
			if (value.IsSingletonObject())
			{
				List<MVWorldObjectClient> worldObjectsByType = MVGameControllerBase.WOCM.GetWorldObjectsByType(value.WorldObjectType);
				if (worldObjectsByType.Count > 0)
				{
					MVGameControllerBase.MainCameraManager.CurrentCamera.FocusOnObject(worldObjectsByType[0]);
					NotificationController.PushNotification(TM._("There can be only one of this object."));
					return false;
				}
			}
			if (!IsItemAnAllowedWinningCondition(value))
			{
				NotificationController.PushNotification(TM._("Only one winning condition will be actively displayed per game."));
			}
		}
		return true;
	}

	private bool IsItemAnAllowedWinningCondition(MVWorldObjectClient worldObject)
	{
		switch (worldObject.WorldObjectType)
		{
		case WorldObjectType.Flag:
			if (IsWinningConditionPlaceable<FlagReachedClient>())
			{
				return true;
			}
			break;
		case WorldObjectType.CollectibleItem:
			if (IsWinningConditionPlaceable<AllCollectiblesCollectedClient>())
			{
				return true;
			}
			break;
		case WorldObjectType.KillLimit:
			if (IsWinningConditionPlaceable<KillLimitClient>())
			{
				return true;
			}
			break;
		case WorldObjectType.OculusKillLimit:
			if (IsWinningConditionPlaceable<OculusKillLimitClient>())
			{
				return true;
			}
			break;
		default:
			return true;
		}
		return false;
	}

	private bool IsWinningConditionPlaceable<T>() where T : WinningCondition
	{
		if (MVGameControllerBase.Game.WinningConditionManager.CanPlaceWinningCondition<T>())
		{
			return true;
		}
		return false;
	}

	private List<MVWorldObjectClient> GetPlacedWinningConditions()
	{
		List<MVWorldObjectClient> worldObjectsByType = MVGameControllerBase.WOCM.GetWorldObjectsByType(WorldObjectType.OculusKillLimit);
		if (worldObjectsByType.Count > 0)
		{
			return worldObjectsByType;
		}
		List<MVWorldObjectClient> worldObjectsByType2 = MVGameControllerBase.WOCM.GetWorldObjectsByType(WorldObjectType.KillLimit);
		if (worldObjectsByType2.Count > 0)
		{
			return worldObjectsByType2;
		}
		List<MVWorldObjectClient> worldObjectsByType3 = MVGameControllerBase.WOCM.GetWorldObjectsByType(WorldObjectType.CollectibleItem);
		if (worldObjectsByType3.Count > 0)
		{
			return worldObjectsByType3;
		}
		List<MVWorldObjectClient> worldObjectsByType4 = MVGameControllerBase.WOCM.GetWorldObjectsByType(WorldObjectType.Flag);
		if (worldObjectsByType4.Count > 0)
		{
			return worldObjectsByType4;
		}
		return null;
	}

	private void WOCM_InitializedGameQueryData(object sender, InitializedGameQueryDataEventArgs e)
	{
		if (MVGameControllerBase.Game.LocalPlayer.ActorNr == e.InstigatorActorNumber && esm.CurEvent == EditorEvent.ESWaitForSelect)
		{
			esm.SelectWO(e.RootWO.Id, addToSelection: false);
		}
	}
}
