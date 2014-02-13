using System;
using System.Collections;
using System.Collections.Generic;
using Localize;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class EditorWorldObjectCreation
{
	private EditorStateMachine esm;

	public EditorWorldObjectCreation(EditorStateMachine esm)
	{
		this.esm = esm;
		World world = MVGameController.Instance.Game.World;
		world.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Combine(world.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(WOCM_InitializedGameQueryData));
	}

	public void CloneHierarchy(MVWorldObjectClient original, bool cloneToRoot, bool setAsPreviewItem, bool goToInsert = false)
	{
		Debug.Log((object)("CloneHierarchy " + original));
		if (goToInsert)
		{
			esm.Data.Add("goToInsert", null);
		}
		esm.Event = EditorEvent.ESWaitForClone;
		MVWorldObjectClientManager wOCM = MVGameController.Instance.WOCM;
		wOCM.CloneWorldObjectTreeResponse = (EventHandler<CloneWorldObjectTreeResponseEventArgs>)Delegate.Combine(wOCM.CloneWorldObjectTreeResponse, new EventHandler<CloneWorldObjectTreeResponseEventArgs>(CloneWorldObjectTreeResponseHandler));
		MVGameController.Instance.WOCM.CloneWorldObjectTree(original, localOwner: false, setAsPreviewItem, cloneToRoot);
	}

	private void CloneWorldObjectTreeResponseHandler(object sender, CloneWorldObjectTreeResponseEventArgs e)
	{
		MVWorldObjectClientManager wOCM = MVGameController.Instance.WOCM;
		wOCM.CloneWorldObjectTreeResponse = (EventHandler<CloneWorldObjectTreeResponseEventArgs>)Delegate.Remove(wOCM.CloneWorldObjectTreeResponse, new EventHandler<CloneWorldObjectTreeResponseEventArgs>(CloneWorldObjectTreeResponseHandler));
		esm.SelectWO(e.RootId, addToSelection: false);
	}

	private bool ValidateAddItemFromInventory(KoGaMaPackageClient package)
	{
		foreach (MVWorldObjectClient value in package.worldObjects.Values)
		{
			if (value.IsSingletonObject())
			{
				List<MVWorldObjectClient> worldObjectsByType = MVGameController.Instance.WOCM.GetWorldObjectsByType(value.WorldObjectType);
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
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		KoGaMaPackageClient koGaMaPackageFromItem = ARepository.GetKoGaMaPackageFromItem(item);
		if (!ValidateAddItemFromInventory(koGaMaPackageFromItem))
		{
			UXDialogFactory uXDialogFactory = UXUtils.FindGUIObjectOfType<UXDialogFactory>();
			uXDialogFactory.CreateDialog(TextSlotIndex.ErrorSingletonWOExists).Show();
			koGaMaPackageFromItem.Destroy();
			return;
		}
		if (!MVGameController.Instance.EditController.IsLogicRendered())
		{
			MVGameController.Instance.EditController.ToggleLogicRendering();
		}
		while (!esm.ParentGroupIsRoot && esm.ParentGroup.HasInteractionFlag(InteractionFlags.CantAddChildren))
		{
			esm.ExitGroup();
		}
		int worldObjectId = -1;
		if (MVGameController.Instance.WOCM.GetUnmodifiedWorldObject(koGaMaPackageFromItem, ref worldObjectId))
		{
			Debug.Log((object)"Found wo for cloning");
			MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(worldObjectId);
			CloneHierarchy(worldObjectClient, cloneToRoot: false, isPreviewItem, goToInsert: true);
		}
		else
		{
			Debug.Log((object)"Creating new wo");
			esm.Event = EditorEvent.ESWaitForSelect;
			MVGameController.Instance.Game.AddItemToWorld(item.itemID, esm.ParentGroupID, Vector3.up * 10f, Quaternion.identity, Vector3.one, localOwner: false, transferOwnershipToServerOnLeave: true, isPreviewItem);
		}
		koGaMaPackageFromItem.Destroy();
	}

	public void OnAddNewPrototype(string name, float scale)
	{
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		while (!esm.ParentGroupIsRoot && esm.ParentGroup.HasInteractionFlag(InteractionFlags.CantAddChildren))
		{
			esm.ExitGroup();
		}
		esm.Event = EditorEvent.ESWaitForSelect;
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)1, scale);
		hashtable.Add((byte)2, esm.CubeModelingStateMachine.CurrentMaterialId);
		hashtable.Add((byte)3, MVGameController.Instance.Game.LocalPlayer.ProfileID);
		Hashtable customData = hashtable;
		esm.Data["IsNewPrototype"] = true;
		MVGameController.Instance.Game.RequestBuiltInItem(BuiltInItem.CubeModel, MVGameController.Instance.WOCM.RootGroup.Id, customData, Vector3.up * 10f, Quaternion.identity, Vector3.one * scale, localOwner: false, transferOwnershipToServerOnLeave: true);
	}

	private void WOCM_InitializedGameQueryData(object sender, InitializedGameQueryDataEventArgs e)
	{
		if (MVGameController.Instance.Game.LocalPlayerActorNumber == e.InstigatorActorNumber && esm.CurEvent == EditorEvent.ESWaitForSelect)
		{
			esm.SelectWO(e.RootWO.Id, addToSelection: false);
		}
	}
}
