using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class EditorWorldObjectCreation
{
	private EditorStateMachine esm;

	public EditorWorldObjectCreation(EditorStateMachine esm)
	{
		this.esm = esm;
		World world = MVGameControllerBase.Game.World;
		world.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Combine(world.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(WOCM_InitializedGameQueryData));
	}

	public void CloneHierarchy(MVWorldObjectClient original, bool cloneToRoot, bool setAsPreviewItem, bool goToInsert = false)
	{
		Debug.Log("CloneHierarchy " + original);
		if (goToInsert)
		{
			esm.Data.Add("goToInsert", null);
		}
		esm.Event = EditorEvent.ESWaitForClone;
		MVWorldObjectClientManager wOCM = MVGameControllerBase.WOCM;
		wOCM.CloneWorldObjectTreeResponse = (EventHandler<CloneWorldObjectTreeResponseEventArgs>)Delegate.Combine(wOCM.CloneWorldObjectTreeResponse, new EventHandler<CloneWorldObjectTreeResponseEventArgs>(CloneWorldObjectTreeResponseHandler));
		MVGameControllerBase.WOCM.CloneWorldObjectTree(original, localOwner: false, setAsPreviewItem, cloneToRoot);
	}

	private void CloneWorldObjectTreeResponseHandler(object sender, CloneWorldObjectTreeResponseEventArgs e)
	{
		if (!e.Success && MVGameControllerBase.GameSessionData.gameMode == MVGameMode.Edit)
		{
			MVGameControllerLegacyUI.EditorController.EditorStateMachine.Event = EditorEvent.ESTerrainEdit;
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
					return false;
				}
			}
		}
		return true;
	}

	public void OnAddItemFromInventory(MVItem item, bool isPreviewItem)
	{
		KoGaMaPackageClient koGaMaPackageFromItem = ARepository.GetKoGaMaPackageFromItem(item);
		if (!ValidateAddItemFromInventory(koGaMaPackageFromItem))
		{
			UXDialogFactory uXDialogFactory = UXUtils.UXDialogFactory;
			uXDialogFactory.CreateDialog(TM._("There is already an object of this type in the game. Only one per game is allowed"), string.Empty).Show();
			koGaMaPackageFromItem.Destroy();
			return;
		}
		if (!MVGameControllerLegacyUI.EditorController.IsLogicRendered())
		{
			MVGameControllerLegacyUI.EditorController.ToggleLogicRendering();
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
			CloneHierarchy(worldObjectClient, cloneToRoot: false, isPreviewItem, goToInsert: true);
		}
		else
		{
			Debug.Log("Creating new wo");
			esm.Event = EditorEvent.ESWaitForSelect;
			Quaternion rotation = Quaternion.identity;
			if (MVGameControllerBase.Game.GameType == MVGameType.Platformer)
			{
				WorldObjectType worldObjectType = koGaMaPackageFromItem.worldObjects[koGaMaPackageFromItem.worldObjectRoot].WorldObjectType;
				rotation = HandlePlatformerRotationSpecialCases(item.itemCategoryID, worldObjectType);
			}
			MVGameControllerBase.Game.AddItemToWorld(item.itemID, esm.ParentGroupID, Vector3.up * 10f, rotation, Vector3.one, localOwner: false, transferOwnershipToServerOnLeave: true, isPreviewItem);
		}
		koGaMaPackageFromItem.Destroy();
	}

	private Quaternion HandlePlatformerRotationSpecialCases(int itemCategory, WorldObjectType worldObjectType)
	{
		switch (worldObjectType)
		{
		case WorldObjectType.GameCoinChest:
			return Quaternion.AngleAxis(-90f, Vector3.up);
		case WorldObjectType.AdvancedGhost:
			return Quaternion.identity;
		default:
			if (itemCategory == 8)
			{
				return Quaternion.AngleAxis(90f, Vector3.up);
			}
			return Quaternion.identity;
		}
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
		MVGameControllerBase.Game.RequestBuiltInItem(BuiltInItem.CubeModel, MVGameControllerBase.WOCM.RootGroup.Id, customData, Vector3.up * 10f, Quaternion.identity, Vector3.one * scale, localOwner: false, transferOwnershipToServerOnLeave: true);
	}

	private void WOCM_InitializedGameQueryData(object sender, InitializedGameQueryDataEventArgs e)
	{
		if (MVGameControllerBase.Game.LocalPlayerActorNumber == e.InstigatorActorNumber && esm.CurEvent == EditorEvent.ESWaitForSelect)
		{
			esm.SelectWO(e.RootWO.Id, addToSelection: false);
		}
	}
}
