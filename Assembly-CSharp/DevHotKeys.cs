using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Localize;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class DevHotKeys
{
	private static int testBodyId = 258730;

	private MVWorldObjectClientManager WOCM => MVGameController.Instance.WOCM;

	private MVNetworkGame Game => MVGameController.Instance.Game;

	private EditorStateMachine ESM => MVGameController.Instance.EditController.EditorStateMachine;

	private AEditController EditorController => MVGameController.Instance.EditController;

	public void HandleInput()
	{
		EditorTestKeys();
	}

	private void EditorTestKeys()
	{
		//IL_0f66: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fe7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ff1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ff6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ffb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0619: Unknown result type (might be due to invalid IL or missing references)
		//IL_0620: Expected Obj, but got Unknown
		if (MVInputWrapper.GetKey((KeyCode)48) && MVInputWrapper.GetKeyDown((KeyCode)99))
		{
			MVCameraController cameraController = MVGameController.Instance.Game.CameraController;
			MVGameController.Instance.IngameController.ToggleShowUI();
			if ((Object)(object)cameraController.CurCamera != (Object)(object)cameraController.freeRoamCamera)
			{
				cameraController.SetCamera(CameraType.FreeRoam);
			}
			else
			{
				cameraController.SetCamera(CameraType.ThirdPerson);
			}
		}
		if ((!Application.isEditor && !Debug.isDebugBuild) || !MVInputWrapper.GetKey((KeyCode)48))
		{
			return;
		}
		if (MVInputWrapper.GetKey((KeyCode)57))
		{
			if (MVInputWrapper.GetKeyDown((KeyCode)113))
			{
				MVBody mVBody = WOCM.GetWorldObjectClient(testBodyId) as MVBody;
				Hashtable hashtable = (Hashtable)mVBody.Data["BlueprintData"];
				Debug.LogWarning((object)("Clean " + hashtable.ContainsKey("ChildrenData")));
				hashtable.Remove("ChildrenData");
				Hashtable hashtable2 = null;
				if (hashtable.Contains(BlueprintData.AvatarAccessoryData.ToString("d")))
				{
					hashtable2 = (Hashtable)hashtable[BlueprintData.AvatarAccessoryData.ToString("d")];
					hashtable2.Clear();
					Debug.LogWarning((object)"Clean accessories");
				}
				hashtable.Remove("3");
				Game.UpdateWorldObjectData(mVBody.Id, mVBody.Data);
				Debug.LogWarning((object)("Update woData to " + mVBody.Data.BuildStringRecursive()));
				return;
			}
			if (MVInputWrapper.GetKeyDown((KeyCode)119))
			{
				if (WOCM.GetWorldObjectClient(testBodyId) is MVBody mVBody2)
				{
					AvatarAccessory avatarAccessory = mVBody2.GetAccessories().FirstOrDefault();
					if ((Object)(object)avatarAccessory != (Object)null)
					{
						Debug.Log((object)("Detaching accessory " + avatarAccessory.InventoryID));
						Hashtable hashtable3 = new Hashtable();
						hashtable3.Add(avatarAccessory.InventoryID.ToString(), string.Empty);
						Hashtable value = hashtable3;
						hashtable3 = new Hashtable();
						hashtable3.Add(BlueprintData.AvatarAccessoryData.ToString("d"), value);
						Hashtable value2 = hashtable3;
						hashtable3 = new Hashtable();
						hashtable3.Add("BlueprintData", value2);
						Hashtable collection = hashtable3;
						Game.RemoveWorldObjectDataPartial(mVBody2.Id, "BlueprintData\\3\\276");
						Debug.LogWarning((object)("Remove wo " + mVBody2.Id + " woData \n" + collection.BuildStringRecursive()));
					}
				}
				return;
			}
			if (MVInputWrapper.GetKeyDown((KeyCode)101))
			{
				CharacterEditorController characterEditorController = EditorController as CharacterEditorController;
				if (testBodyId == 258730)
				{
					if (characterEditorController != null)
					{
						testBodyId = characterEditorController.CurrentBody.Id;
					}
					else
					{
						testBodyId = WOCM.AvatarLocal.Body.Id;
					}
				}
				else
				{
					testBodyId = 258730;
				}
				Debug.Log((object)("TestBody set to: " + testBodyId + " playerBody " + WOCM.AvatarLocal.Body.Id));
				return;
			}
			if (MVInputWrapper.GetKeyDown((KeyCode)114))
			{
				ProductInventoryInfo<StreamingAssetInfo> productInventoryInfo = Game.StreamingAssetInventory.Get((ProductInventoryInfo<StreamingAssetInfo> p) => !p.IsRented).FirstOrDefault();
				if (productInventoryInfo != null)
				{
					Game.ExpireAvatarAccessory(productInventoryInfo.InventoryID);
					Debug.Log((object)("Try expire accessory " + productInventoryInfo.InventoryID));
				}
				return;
			}
			if (MVInputWrapper.GetKeyDown((KeyCode)121))
			{
				BrowserComm browserComm = Object.FindObjectOfType(typeof(BrowserComm)) as BrowserComm;
				if ((Object)(object)browserComm != (Object)null)
				{
					Debug.Log((object)"Gen screehsot");
					browserComm.CreatePlanetScreenshot();
				}
				return;
			}
			if (MVInputWrapper.GetKeyDown((KeyCode)97))
			{
				if (WOCM.GetWorldObjectClient(testBodyId) is MVBody mVBody3)
				{
					MVNetworkGame game = Game;
					game.PurchaseProductResponseHandler = (Action<int, Hashtable>)Delegate.Combine(game.PurchaseProductResponseHandler, new Action<int, Hashtable>(SARentProductResponseHandler));
					Game.PurchaseAvatarAccessory(40, mVBody3.Id, AvatarAccessorySlot.WholeBody, 0f);
				}
				return;
			}
			if (MVInputWrapper.GetKeyDown((KeyCode)115))
			{
				if (WOCM.GetWorldObjectClient(testBodyId) is MVBody mVBody4)
				{
					MVNetworkGame game2 = Game;
					game2.PurchaseProductResponseHandler = (Action<int, Hashtable>)Delegate.Combine(game2.PurchaseProductResponseHandler, new Action<int, Hashtable>(SARentProductResponseHandler));
					Game.RentAvatarAccessory(43, mVBody4.Id, AvatarAccessorySlot.WholeBody, 0f);
				}
				return;
			}
			if (MVInputWrapper.GetKeyDown((KeyCode)100))
			{
				if (WOCM.GetWorldObjectClient(testBodyId) is MVBody mVBody5)
				{
					AvatarAccessory avatarAccessory2 = mVBody5.GetAccessories().FirstOrDefault();
					if ((Object)(object)avatarAccessory2 != (Object)null)
					{
						MVNetworkGame game3 = Game;
						game3.PurchaseProductResponseHandler = (Action<int, Hashtable>)Delegate.Combine(game3.PurchaseProductResponseHandler, new Action<int, Hashtable>(SARentProductResponseHandler));
						Game.ExtendRentAvatarAccessory(43, avatarAccessory2.InventoryID, mVBody5.Id, avatarAccessory2.Slot, avatarAccessory2.Offset);
					}
				}
				return;
			}
			if (MVInputWrapper.GetKeyDown((KeyCode)102))
			{
				if (WOCM.GetWorldObjectClient(testBodyId) is MVBody mVBody6)
				{
					AvatarAccessory avatarAccessory3 = mVBody6.GetAccessories().FirstOrDefault();
					if ((Object)(object)avatarAccessory3 != (Object)null)
					{
						Game.ExpireAvatarAccessory(avatarAccessory3.InventoryID, mVBody6.Id);
					}
				}
				return;
			}
			if (MVInputWrapper.GetKeyDown((KeyCode)103))
			{
				if (EditorController is CharacterEditorController characterEditorController2)
				{
					GameObject bodyCloneGO = (GameObject)Object.Instantiate((Object)(object)characterEditorController2.CurrentBody.GameObject);
					AvatarScreenshotGenerator.Generate(bodyCloneGO, null);
				}
				return;
			}
			if (MVInputWrapper.GetKeyDown((KeyCode)104))
			{
				MVBody body = WOCM.AvatarLocal.Body;
				if (body != null)
				{
					ParticleSystemRenderer componentInChildren = body.GameObject.GetComponentInChildren<ParticleSystemRenderer>();
					if ((Object)(object)componentInChildren != (Object)null)
					{
						Debug.LogWarning((object)((Object)((Renderer)componentInChildren).material.shader).name);
					}
				}
				return;
			}
			if (MVInputWrapper.GetKeyDown((KeyCode)122))
			{
				MVBody mVBody7 = WOCM.GetWorldObjectClient(testBodyId) as MVBody;
				Hashtable hashtable4 = (Hashtable)mVBody7.Data["BlueprintData"];
				Hashtable hashtable5 = (Hashtable)hashtable4[BlueprintData.AvatarAccessoryData.ToString("d")];
				foreach (DictionaryEntry item in hashtable5)
				{
					Hashtable hashtable6 = (Hashtable)item.Value;
					int num = (int)hashtable6[AvatarAccessoryData.InventoryID.ToString("d")];
					Debug.Log((object)("Get expInfo " + num));
					InventoryExpirationInfo expirationInfo = Game.StreamingAssetExpirationChecker.GetExpirationInfo(num);
					hashtable6[AvatarAccessoryData.PurchaseTimeTicks.ToString("d")] = expirationInfo.PurchaseTime.Ticks;
					hashtable6[AvatarAccessoryData.RentExpireSeconds.ToString("d")] = TimeSpan.FromSeconds((double)expirationInfo.RentExpireSeconds).Ticks;
				}
				Game.UpdateWorldObjectData(mVBody7.Id, mVBody7.Data);
				return;
			}
			if (MVInputWrapper.GetKeyDown((KeyCode)120))
			{
				Hashtable hashtable3 = new Hashtable();
				hashtable3.Add(AvatarAccessoryData.InventoryID.ToString("d"), 1000);
				hashtable3.Add(AvatarAccessoryData.Slot.ToString("d"), AvatarAccessorySlot.Head);
				hashtable3.Add(AvatarAccessoryData.Offset.ToString("d"), -0.27f);
				hashtable3.Add(AvatarAccessoryData.AssetPath.ToString("d"), "Accessories/Hats/HatCowboy.unity3d");
				hashtable3.Add(AvatarAccessoryData.PurchaseTimeTicks.ToString("d"), Game.DBTime.Ticks);
				hashtable3.Add(AvatarAccessoryData.RentExpireSeconds.ToString("d"), 60);
				Hashtable value3 = hashtable3;
				hashtable3 = new Hashtable();
				hashtable3.Add("1000", value3);
				Hashtable value4 = hashtable3;
				hashtable3 = new Hashtable();
				hashtable3.Add(BlueprintData.AvatarAccessoryData.ToString("d"), value4);
				Hashtable value5 = hashtable3;
				hashtable3 = new Hashtable();
				hashtable3.Add("BlueprintData", value5);
				Hashtable woData = hashtable3;
				if (WOCM.GetWorldObjectClient(testBodyId) is MVBody mVBody8)
				{
					Game.UpdateWorldObjectDataPartial(mVBody8.Id, woData);
				}
				else
				{
					Debug.LogWarning((object)("body " + testBodyId + " not found"));
				}
				return;
			}
			if (MVInputWrapper.GetKeyDown((KeyCode)50) && MVGameController.Instance.EditorController.EditorStateMachine.SingleSelectedWO is MVCubeModelInstance mVCubeModelInstance)
			{
				Hashtable hashtable7 = new Hashtable();
				hashtable7["ClientSideType"] = (byte)11;
				hashtable7[BlueprintData.ChildrenMap] = new Hashtable { { "movable", mVCubeModelInstance.Id } };
				Hashtable hashtable3 = new Hashtable();
				hashtable3.Add("BlueprintData", hashtable7);
				Hashtable value6 = hashtable3;
				MVGameController.Instance.EditorController.EditorStateMachine.Data.Add("woData", value6);
				MVGameController.Instance.EditorController.EditorStateMachine.PushState(EditorEvent.ESBlueprintCreator);
			}
			if (MVInputWrapper.GetKeyDown((KeyCode)51))
			{
				Hashtable hashtable8 = new Hashtable();
				hashtable8["ClientSideType"] = (byte)12;
				Hashtable hashtable9 = new Hashtable();
				MVMovingPlatform mVMovingPlatform = ESM.SelectedWOs.FirstOrDefault((MVWorldObjectClient e) => e is MVMovingPlatform) as MVMovingPlatform;
				List<MVWorldObjectClient> list = ESM.SelectedWOs.Where((MVWorldObjectClient e) => e is MVMovingPlatformNode).ToList();
				int num2 = list.Count();
				if (mVMovingPlatform == null || num2 < 2)
				{
					return;
				}
				Debug.Log((object)"Creating moving platform group");
				hashtable9.Add("Platform", mVMovingPlatform.Id);
				Hashtable hashtable10 = new Hashtable();
				for (int num3 = 0; num3 < list.Count(); num3++)
				{
					hashtable9.Add(num3.ToString(), list[num3].Id);
					if (num3 < list.Count - 1)
					{
						hashtable10.Add(num3.ToString(), num3 + 1);
					}
				}
				hashtable8.Add("StartNode", 0);
				hashtable8.Add("NextNodeMap", hashtable10);
				hashtable8[BlueprintData.ChildrenMap.ToString()] = hashtable9;
				Hashtable hashtable3 = new Hashtable();
				hashtable3.Add("BlueprintData", hashtable8);
				Hashtable value7 = hashtable3;
				EditorController.EditorStateMachine.Data.Add("woData", value7);
				EditorController.EditorStateMachine.PushState(EditorEvent.ESBlueprintCreator);
			}
			if (MVInputWrapper.GetKeyDown((KeyCode)52) && MVGameController.Instance.EditorController.EditorStateMachine.SingleSelectedWO is MVCubeModelInstance mVCubeModelInstance2)
			{
				Hashtable hashtable11 = new Hashtable();
				hashtable11["ClientSideType"] = (byte)13;
				hashtable11[BlueprintData.ChildrenMap.ToString()] = new Hashtable { { "movable", mVCubeModelInstance2.Id } };
				hashtable11["AngularDirection"] = "0 1 0";
				hashtable11["AngularSpeed"] = 1f;
				Hashtable hashtable3 = new Hashtable();
				hashtable3.Add("BlueprintData", hashtable11);
				Hashtable value8 = hashtable3;
				MVGameController.Instance.EditorController.EditorStateMachine.Data.Add("woData", value8);
				MVGameController.Instance.EditorController.EditorStateMachine.PushState(EditorEvent.ESBlueprintCreator);
			}
			if (MVInputWrapper.GetKeyDown((KeyCode)53) && MVGameController.Instance.EditorController.EditorStateMachine.SingleSelectedWO is MVCubeModelInstance mVCubeModelInstance3)
			{
				Hashtable hashtable12 = new Hashtable();
				hashtable12["ClientSideType"] = (byte)13;
				hashtable12[BlueprintData.ChildrenMap.ToString()] = new Hashtable { { "movable", mVCubeModelInstance3.Id } };
				hashtable12["AngularDirection"] = "1 0 0";
				hashtable12["AngularSpeed"] = 1f;
				Hashtable hashtable3 = new Hashtable();
				hashtable3.Add("BlueprintData", hashtable12);
				Hashtable value9 = hashtable3;
				MVGameController.Instance.EditorController.EditorStateMachine.Data.Add("woData", value9);
				MVGameController.Instance.EditorController.EditorStateMachine.PushState(EditorEvent.ESBlueprintCreator);
			}
			if (MVInputWrapper.GetKeyDown((KeyCode)54))
			{
				MVMovingPlatformGroup mVMovingPlatformGroup = MVGameController.Instance.EditorController.EditorStateMachine.SingleSelectedWO as MVMovingPlatformGroup;
				if (mVMovingPlatformGroup != null)
				{
				}
				MVRotator mVRotator = MVGameController.Instance.EditorController.EditorStateMachine.SingleSelectedWO as MVRotator;
				if (mVRotator == null)
				{
				}
			}
			if (MVInputWrapper.GetKeyDown((KeyCode)55))
			{
			}
			if (MVInputWrapper.GetKeyDown((KeyCode)56))
			{
				MVWorldObjectClient singleSelectedWO = MVGameController.Instance.EditorController.EditorStateMachine.SingleSelectedWO;
				if (singleSelectedWO is MVRotator)
				{
					MVGameController.Instance.Game.AddWorldObjectToInventorDev(singleSelectedWO.Id, new byte[10], "Rotator 2", MVGameController.Instance.Game.ItemCategories.NameToID("Blueprint"), overWrite: false);
					Debug.Log((object)"*** ADDING MVRotator TO INVENTORY");
				}
				if (singleSelectedWO is MVMovingPlatformGroup)
				{
					MVGameController.Instance.Game.AddWorldObjectToInventorDev(singleSelectedWO.Id, new byte[10], "Moving platform", MVGameController.Instance.Game.ItemCategories.NameToID("Blueprint"), overWrite: false);
					Debug.Log((object)"*** ADDING MVMovingPlatformGroup TO INVENTORY");
				}
				else if (singleSelectedWO is MVCubeModelInstance)
				{
					MVGameController.Instance.Game.AddWorldObjectToInventory(singleSelectedWO.Id, new byte[10]);
					Debug.Log((object)"*** ADDING CUBE MODEL TO INVENTORY");
				}
			}
		}
		else if (MVInputWrapper.GetKey((KeyCode)56))
		{
			if (MVInputWrapper.GetKeyUp((KeyCode)116))
			{
				InteractionData interactionData = new InteractionData(InteractionPackageType.MutantHit, 10f, Vector3.one);
				Debug.Log((object)interactionData);
				byte[] byteArray = interactionData.ToByteArray();
				InteractionData interactionData2 = new InteractionData(byteArray, withSharedValues: true);
				Debug.Log((object)interactionData2);
			}
		}
		else if (MVInputWrapper.GetKey((KeyCode)55))
		{
			if (MVInputWrapper.GetKeyUp((KeyCode)116))
			{
				Debug.Log((object)"DEV KEY");
				MVGameController.Instance.Game.RegisterWorldObject(WorldObjectType.CollectibleItem, MVGameController.Instance.WOCM.RootGroup.Id, new Hashtable(), Vector3.up * 25f, Quaternion.identity, Vector3.one, localOwner: true, transferOwnershipToServerOnLeave: true);
			}
		}
		else if (MVInputWrapper.GetKey((KeyCode)54))
		{
			if (MVInputWrapper.GetKeyUp((KeyCode)116))
			{
				if (MVGameController.Instance.PlayController.speedometer.View.isVisible)
				{
					MVGameController.Instance.PlayController.speedometer.View.Hide();
				}
				else
				{
					MVGameController.Instance.PlayController.speedometer.View.Show();
				}
			}
			if (MVInputWrapper.GetKeyDown((KeyCode)100))
			{
				UXDialogFactory uXDialogFactory = UXUtils.FindGUIObjectOfType<UXDialogFactory>();
				uXDialogFactory.CreateCustomDevelopmentDialog("Prefabs/GUI/Dev Tools/SelectDevToolDialog", "Dev Tools", noButtons: true, stackDialog: true).Show();
			}
		}
		else
		{
			if (MVInputWrapper.GetKey((KeyCode)53))
			{
				return;
			}
			if (MVInputWrapper.GetKeyDown((KeyCode)100))
			{
				UXDialogFactory uXDialogFactory2 = UXUtils.FindGUIObjectOfType<UXDialogFactory>();
				uXDialogFactory2.CreateCustomDevelopmentDialog("Prefabs/GUI/Dev Tools/SelectDevToolDialog", "Dev Tools", noButtons: true, stackDialog: true).Show();
			}
			if (MVInputWrapper.GetKeyDown((KeyCode)111))
			{
				UXDialogFactory uXDialogFactory3 = UXUtils.FindGUIObjectOfType<UXDialogFactory>();
				if ((Object)(object)uXDialogFactory3.CurrentDialogBox == (Object)null)
				{
					uXDialogFactory3.CreateCustomDevelopmentDialog("Prefabs/GUI/Dev Tools/MovableDebugDialog", "Movable").AddPositiveButton(TextSlotIndex.Apply).AddNegativeButton(TextSlotIndex.Cancel)
						.SetOnResultCallback(UpdateMovable)
						.Show();
				}
			}
			else if (MVInputWrapper.GetKeyDown((KeyCode)117))
			{
				MVGameController.Instance.EditController.EditorStateMachine.PushState(EditorEvent.ESWaitForUngroup);
			}
			else if (MVInputWrapper.GetKeyDown((KeyCode)103))
			{
				MVGameController.Instance.EditController.EditorStateMachine.PushState(EditorEvent.ESWaitForGroup);
			}
			if (MVInputWrapper.GetKeyDown((KeyCode)108) && !MVInputWrapper.GetKey((KeyCode)304))
			{
				MVWorldObjectClient settingsDialogSelectionWO = MVGameController.Instance.EditorController.GetSettingsDialogSelectionWO();
				if (settingsDialogSelectionWO != null && settingsDialogSelectionWO is MVCubeModelBase)
				{
					MVCubeModelBase cm = (MVCubeModelBase)settingsDialogSelectionWO;
					ObjExporterScript.CubeModelToFile(cm);
				}
			}
			else
			{
				if (!MVInputWrapper.GetKeyDown((KeyCode)108) || !MVInputWrapper.GetKey((KeyCode)304))
				{
					return;
				}
				if (MVGameController.Instance.Game.GameMode == MVGameMode.CharacterEditor)
				{
					MVBody currentBody = MVGameController.Instance.CharacterEditorController.CurrentBody;
					{
						foreach (MVWorldObjectClient child in currentBody.Children)
						{
							if (child is MVCubeModelBase)
							{
								MVCubeModelBase cm2 = (MVCubeModelBase)child;
								ObjExporterScript.CubeModelToFile(cm2);
							}
						}
						return;
					}
				}
				ObjExporterScript.CubeModelToFile(MVGameController.Instance.WOCM.GetSingletonWorldObject<MVCubeModelPrototypeTerrain>());
			}
		}
	}

	private void SARentProductResponseHandler(int retCode, Hashtable resData)
	{
		if (retCode == 0)
		{
			MVNetworkGame game = Game;
			game.PurchaseProductResponseHandler = (Action<int, Hashtable>)Delegate.Remove(game.PurchaseProductResponseHandler, new Action<int, Hashtable>(SARentProductResponseHandler));
			int key = (int)resData[(byte)63];
			int num = (int)resData[(byte)74];
			long ticks = (long)resData[(byte)84];
			int num2 = (int)resData[(byte)81];
			DateTime dateTime = new DateTime(ticks);
			Debug.Log((object)("Rented ivnID " + num + " at " + dateTime));
			InventoryExpirationInfo expirationInfo = Game.StreamingAssetExpirationChecker.GetExpirationInfo(num);
			if (expirationInfo != null)
			{
				expirationInfo.Renew(dateTime, num2);
			}
			else if (0 < num2)
			{
				expirationInfo = new InventoryExpirationInfo(MVProductType.StreamingAsset, num, ProductExpirationState.Expiring, dateTime, num2);
				Game.StreamingAssetExpirationChecker.AddExpirationInfo(expirationInfo);
			}
			StreamingAssetInfo value = null;
			Game.StreamingAssetInfoMap.TryGetValue(key, out value);
			if (value != null)
			{
				ProductInventoryInfo<StreamingAssetInfo> productInventoryInfo = Game.StreamingAssetInventory.Get(num);
				if (productInventoryInfo == null)
				{
					productInventoryInfo = new ProductInventoryInfo<StreamingAssetInfo>(num, value, dateTime, isRented: true);
					Game.StreamingAssetInventory.Add(productInventoryInfo);
				}
				if (WOCM.GetWorldObjectClient(testBodyId) is MVBody mVBody && !mVBody.GetAccessoryIDs().Contains(productInventoryInfo.InventoryID))
				{
					AvatarAccessory.Create(productInventoryInfo, AvatarAccessoryCreatedHandler);
				}
			}
		}
		else
		{
			Debug.LogWarning((object)"COULD NOT PURCHASE OR RENT");
		}
	}

	private void AvatarAccessoryCreatedHandler(AvatarAccessory acc)
	{
		MVBody mVBody = WOCM.GetWorldObjectClient(testBodyId) as MVBody;
		Debug.Log((object)("Create accessory " + acc.InventoryID + " body found: " + (mVBody != null)));
		if (mVBody != null && (Object)(object)acc != (Object)null)
		{
			ProductInventoryInfo<StreamingAssetInfo> invInfo = Game.StreamingAssetInventory.Get(acc.InventoryID);
			mVBody.AttachAccessoryPermanent(acc, acc.DefaultSlot, 0f, invInfo);
		}
	}

	private void OnAvatarShopInventoryResultSetResponse(Hashtable outData, int largeQueryId, bool isDone)
	{
		foreach (int key in outData.Keys)
		{
			Hashtable hashtable = (Hashtable)outData[key];
			Debug.Log((object)("data.Length " + ((byte[])hashtable[(byte)93]).Length));
			int num2 = (int)hashtable[(byte)78];
			int num3 = (int)hashtable[(byte)77];
			Debug.Log((object)("priceSilver " + num2));
			Debug.Log((object)("priceGold " + num3));
			Debug.Log((object)("AvatarID " + key));
		}
	}

	private MVBody FindUnattachedBody()
	{
		return (MVBody)MVGameController.Instance.WOCM.GetWorldObjectClientWhere((MVWorldObjectClient wo) => wo is MVBody mVBody && mVBody.AttachedAvatar == null);
	}

	private string GetHashtableString(Hashtable data)
	{
		string text = string.Empty;
		bool flag = true;
		foreach (DictionaryEntry datum in data)
		{
			if (!flag)
			{
				text += ", ";
			}
			text += "{\"";
			text += datum.Key;
			text += "\", ";
			text += datum.Value;
			text += "}";
			flag = false;
		}
		return text;
	}

	private void InitializedAvatarBodyDataHandler(object sender, InitializedGameQueryDataEventArgs e)
	{
		World world = MVGameController.Instance.Game.World;
		world.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Remove(world.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(InitializedAvatarBodyDataHandler));
		Debug.Log((object)"InitializedAvatarBodyDataHandler");
		if (e.RootWO != null)
		{
			int id = e.RootWO.Id;
			MVNetworkGame game = MVGameController.Instance.Game;
			game.LockHierarchy(id, lockHierarchy: true);
			game.TransferOwnership(id, 0, null);
		}
	}

	private void UpdateMovable(UXDialogBox dialogBox)
	{
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		if (dialogBox.DialogResult != UXDialogResult.Positive)
		{
			return;
		}
		Hashtable hashtable = (Hashtable)dialogBox.GetResult();
		string text = (string)hashtable["Rot"];
		string text2 = (string)hashtable["Vel"];
		string text3 = (string)hashtable["AngVel"];
		string text4 = (string)hashtable["Dist"];
		string text5 = (string)hashtable["ParentID"];
		if (MVGameController.Instance.EditorController.EditorStateMachine.SingleSelectedWO == null)
		{
			return;
		}
		Debug.Log((object)MVGameController.Instance.EditorController.EditorStateMachine.SingleSelectedWO);
		if (MVGameController.Instance.EditorController.EditorStateMachine.SingleSelectedWO is MVMovable mVMovable)
		{
			Debug.Log((object)"Setting movable params");
			if (text != null && text.Length != 0)
			{
				Quaternion orgRotation = Quaternion.Euler(StringToVec3(text));
				mVMovable.SetOrgRotation(orgRotation, updateWOData: true);
			}
			if (text2 != null && text2.Length != 0)
			{
				Vector3 velocity = StringToVec3(text2);
				mVMovable.SetVelocity(velocity, updateWOData: true);
			}
			if (text3 == null || text3.Length != 0)
			{
			}
			if (text4 != null && text4.Length != 0)
			{
				text4 = text4.Replace("x", ".");
				float distance = Convert.ToSingle(text4);
				mVMovable.SetDistance(distance, updateWOData: true);
			}
			if (text5 != null && text5.Length != 0)
			{
				text5 = text5.Replace("x", ".");
				int parentMoverID = Convert.ToInt32(text5);
				mVMovable.SetParentMoverID(parentMoverID, updateWOData: true);
			}
			mVMovable.SyncProperties();
		}
	}

	private Vector3 StringToVec3(string vecText)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		string text = vecText.Replace("x", ".");
		string[] array = text.Split(new char[1] { ' ' });
		return new Vector3(Convert.ToSingle(array[0]), Convert.ToSingle(array[1]), Convert.ToSingle(array[2]));
	}

	private void TestHandleRemoveCubes()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		MVSpawnPoint mVSpawnPoint = (MVSpawnPoint)MVGameController.Instance.WOCM.GetWorldObjectsByType(WorldObjectType.SpawnPoint)[0];
		float radius = 4.5f;
		List<CommonOverlapArg> wOIdsWithinRadius = MVGameController.Instance.Game.GetWOIdsWithinRadius(radius, mVSpawnPoint.Position);
		int[] array = new int[wOIdsWithinRadius.Count];
		for (int i = 0; i < wOIdsWithinRadius.Count; i++)
		{
			array[i] = wOIdsWithinRadius[i].wo.Id;
		}
		MVGameController.Instance.Game.RequestRemoveCubesWithinRadius(array, radius, mVSpawnPoint.Position, 200f, DamageFallOffType.Linear);
	}

	private void CommonOverlapTestTest()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		MVSpawnPoint mVSpawnPoint = (MVSpawnPoint)MVGameController.Instance.WOCM.GetWorldObjectsByType(WorldObjectType.SpawnPoint)[0];
		float num = 1.5f;
		Vector3 position = mVSpawnPoint.Position;
		DebugClass.DrawPoint(mVSpawnPoint.Position, 100f);
		Collider[] array = Physics.OverlapSphere(mVSpawnPoint.Position, num);
		List<CommonOverlapResult> list = new List<CommonOverlapResult>();
		List<int> list2 = new List<int>();
		for (int i = 0; i < array.Length; i++)
		{
			MVWorldObjectClient mVObject = MVWorldObjectClientManager.GetMVObject(((Component)array[i]).transform);
			if (mVObject is MVCubeModelBase)
			{
				CommonOverlapArg overlapArg = new CommonOverlapArg(mVObject);
				if (SphereOverlapTest.OverlapWo(overlapArg, num, position, out var overlapResult))
				{
					list.Add(overlapResult);
					Debug.Log((object)overlapResult.cubes.Count);
					list2.Add(overlapResult.woId);
				}
			}
		}
		MVGameController.Instance.Game.RequestRemoveCubesWithinRadius(list2.ToArray(), num, position, 200f, DamageFallOffType.NoFallOff);
	}

	private void FireBazooka()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
		VoxelHit voxelHit = default;
		if (CollisionDetection.MVHit(ray, out voxelHit))
		{
			Missile missile = Missile.CreateMissile();
			((Component)missile).transform.position = MVGameController.Instance.WOCM.AvatarLocal.GameObject.transform.position;
			missile.Fire(voxelHit.point);
		}
	}

	private void MoveGroupToCenterOfMass(EditorStateMachine e)
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		if (e.SingleSelectedWO != null && !(e.SingleSelectedWO is MVGroup))
		{
			Debug.Log((object)"MoveGroupToCenterOfMass: selected object is not a MVGroup");
			return;
		}
		HashSet<int> selectionSet = new HashSet<int>(e.SelectedIDs);
		if (!e.NetworkSelector.RequestOwnership(selectionSet))
		{
			Debug.Log((object)"MoveGroupToCenterOfMass: Could not get ownership");
			return;
		}
		float gridSize = ((!AEditController.IsGridSnap()) ? 0.0625f : 1f);
		MVGroup mVGroup = e.SingleSelectedWO as MVGroup;
		Bounds localBounds = mVGroup.GetLocalBounds(BoundsContext.Default);
		Vector3 closestGridPoint = mVGroup.GetClosestGridPoint(gridSize, localBounds.center);
		mVGroup.Position += closestGridPoint;
		foreach (MVWorldObjectClient child in mVGroup.Children)
		{
			child.Position -= closestGridPoint;
		}
		e.NetworkSelector.RequestReleaseOwnership(selectionSet);
	}
}
