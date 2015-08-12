using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MV.Common;
using UnityEngine;

public static class ValdisHotKeys
{
	private static int testBodyId = 258730;

	private static MVWorldObjectClientManager WOCM => MVGameController.WOCM;

	private static MVNetworkGame Game => MVGameController.Game;

	private static EditorStateMachine ESM => MVGameController.EditController.EditorStateMachine;

	private static AEditController EditorController => MVGameController.EditController;

	public static void Handle()
	{
		if (MVInputWrapper.DebugGetKeyDown(KeyCode.Q))
		{
			MVBody mVBody = WOCM.GetWorldObjectClient(testBodyId) as MVBody;
			Hashtable hashtable = (Hashtable)mVBody.Data["BlueprintData"];
			Debug.LogWarning("Clean " + hashtable.ContainsKey("ChildrenData"));
			hashtable.Remove("ChildrenData");
			Hashtable hashtable2 = null;
			if (hashtable.Contains(BlueprintData.AvatarAccessoryData.ToString("d")))
			{
				hashtable2 = (Hashtable)hashtable[BlueprintData.AvatarAccessoryData.ToString("d")];
				hashtable2.Clear();
				Debug.LogWarning("Clean accessories");
			}
			hashtable.Remove("3");
			Game.UpdateWorldObjectData(mVBody.Id, mVBody.Data);
			Debug.LogWarning("Update woData to " + mVBody.Data.BuildStringRecursive());
			return;
		}
		if (MVInputWrapper.DebugGetKeyDown(KeyCode.W))
		{
			if (WOCM.GetWorldObjectClient(testBodyId) is MVBody mVBody2)
			{
				AvatarAccessory avatarAccessory = mVBody2.GetAccessories().FirstOrDefault();
				if (avatarAccessory != null)
				{
					Debug.Log("Detaching accessory " + avatarAccessory.InventoryID);
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
					Debug.LogWarning("Remove wo " + mVBody2.Id + " woData \n" + collection.BuildStringRecursive());
				}
			}
			return;
		}
		if (MVInputWrapper.DebugGetKeyDown(KeyCode.E))
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
			Debug.Log("TestBody set to: " + testBodyId + " playerBody " + WOCM.AvatarLocal.Body.Id);
			return;
		}
		if (MVInputWrapper.DebugGetKeyDown(KeyCode.R))
		{
			ProductInventoryInfo productInventoryInfo = Game.StreamingAssetInventory.Get((ProductInventoryInfo p) => !p.IsRented).FirstOrDefault();
			if (productInventoryInfo != null)
			{
				Game.ExpireAvatarAccessory(productInventoryInfo.InventoryID);
				Debug.Log("Try expire accessory " + productInventoryInfo.InventoryID);
			}
			return;
		}
		if (MVInputWrapper.DebugGetKeyDown(KeyCode.Y))
		{
			BrowserComm browserComm = UnityEngine.Object.FindObjectOfType(typeof(BrowserComm)) as BrowserComm;
			if (browserComm != null)
			{
				Debug.Log("Gen screeshot");
				browserComm.CreatePlanetScreenshot();
			}
			return;
		}
		if (MVInputWrapper.DebugGetKeyDown(KeyCode.A))
		{
			if (WOCM.GetWorldObjectClient(testBodyId) is MVBody mVBody3)
			{
				MVNetworkGame game = Game;
				game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Combine(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(SARentProductResponseHandler));
				Game.PurchaseAvatarAccessory(40, mVBody3.Id, AvatarAccessorySlot.WholeBody, 0f);
			}
			return;
		}
		if (MVInputWrapper.DebugGetKeyDown(KeyCode.S))
		{
			if (WOCM.GetWorldObjectClient(testBodyId) is MVBody mVBody4)
			{
				MVNetworkGame game2 = Game;
				game2.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Combine(game2.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(SARentProductResponseHandler));
				Game.RentAvatarAccessory(43, mVBody4.Id, AvatarAccessorySlot.WholeBody, 0f);
			}
			return;
		}
		if (MVInputWrapper.DebugGetKeyDown(KeyCode.D))
		{
			if (WOCM.GetWorldObjectClient(testBodyId) is MVBody mVBody5)
			{
				AvatarAccessory avatarAccessory2 = mVBody5.GetAccessories().FirstOrDefault();
				if (avatarAccessory2 != null)
				{
					MVNetworkGame game3 = Game;
					game3.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Combine(game3.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(SARentProductResponseHandler));
					Game.ExtendRentAvatarAccessory(43, avatarAccessory2.InventoryID, mVBody5.Id, avatarAccessory2.Slot, avatarAccessory2.Offset);
				}
			}
			return;
		}
		if (MVInputWrapper.DebugGetKeyDown(KeyCode.F))
		{
			if (WOCM.GetWorldObjectClient(testBodyId) is MVBody mVBody6)
			{
				AvatarAccessory avatarAccessory3 = mVBody6.GetAccessories().FirstOrDefault();
				if (avatarAccessory3 != null)
				{
					Game.ExpireAvatarAccessory(avatarAccessory3.InventoryID, mVBody6.Id);
				}
			}
			return;
		}
		if (MVInputWrapper.DebugGetKeyDown(KeyCode.G))
		{
			if (EditorController is CharacterEditorController characterEditorController2)
			{
				GameObject bodyCloneGO = UnityEngine.Object.Instantiate(characterEditorController2.CurrentBody.GameObject);
				AvatarScreenshotGenerator.Generate(bodyCloneGO, null);
			}
			return;
		}
		if (MVInputWrapper.DebugGetKeyDown(KeyCode.H))
		{
			MVBody body = WOCM.AvatarLocal.Body;
			if (body != null)
			{
				ParticleSystemRenderer componentInChildren = body.GameObject.GetComponentInChildren<ParticleSystemRenderer>();
				if (componentInChildren != null)
				{
					Debug.LogWarning(componentInChildren.material.shader.name);
				}
			}
			return;
		}
		if (MVInputWrapper.DebugGetKeyDown(KeyCode.Z))
		{
			MVBody mVBody7 = WOCM.GetWorldObjectClient(testBodyId) as MVBody;
			Hashtable hashtable4 = (Hashtable)mVBody7.Data["BlueprintData"];
			Hashtable hashtable5 = (Hashtable)hashtable4[BlueprintData.AvatarAccessoryData.ToString("d")];
			foreach (DictionaryEntry item in hashtable5)
			{
				Hashtable hashtable6 = (Hashtable)item.Value;
				int num = (int)hashtable6[AvatarAccessoryData.InventoryID.ToString("d")];
				Debug.Log("Get expInfo " + num);
				InventoryExpirationInfo expirationInfo = Game.StreamingAssetExpirationChecker.GetExpirationInfo(num);
				hashtable6[AvatarAccessoryData.PurchaseTimeTicks.ToString("d")] = expirationInfo.PurchaseTime.Ticks;
				hashtable6[AvatarAccessoryData.RentExpireSeconds.ToString("d")] = TimeSpan.FromSeconds(expirationInfo.RentExpireSeconds).Ticks;
			}
			Game.UpdateWorldObjectData(mVBody7.Id, mVBody7.Data);
			return;
		}
		if (MVInputWrapper.DebugGetKeyDown(KeyCode.X))
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
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add(BlueprintData.AvatarAccessoryData.ToString("d"), value4);
			Dictionary<object, object> value5 = dictionary;
			dictionary = new Dictionary<object, object>();
			dictionary.Add("BlueprintData", value5);
			Dictionary<object, object> woData = dictionary;
			if (WOCM.GetWorldObjectClient(testBodyId) is MVBody mVBody8)
			{
				Game.UpdateWorldObjectDataPartial(mVBody8.Id, woData);
			}
			else
			{
				Debug.LogWarning("body " + testBodyId + " not found");
			}
			return;
		}
		if (MVInputWrapper.DebugGetKeyDown(KeyCode.Alpha2) && MVGameController.EditorController.EditorStateMachine.SingleSelectedWO is MVCubeModelInstance mVCubeModelInstance)
		{
			Hashtable hashtable7 = new Hashtable();
			hashtable7["ClientSideType"] = (byte)11;
			hashtable7[BlueprintData.ChildrenMap] = new Hashtable { { "movable", mVCubeModelInstance.Id } };
			Hashtable hashtable3 = new Hashtable();
			hashtable3.Add("BlueprintData", hashtable7);
			Hashtable value6 = hashtable3;
			MVGameController.EditorController.EditorStateMachine.Data.Add("woData", value6);
			MVGameController.EditorController.EditorStateMachine.PushState(EditorEvent.ESBlueprintCreator);
		}
		if (MVInputWrapper.DebugGetKeyDown(KeyCode.Alpha3))
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
			Debug.Log("Creating moving platform group");
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
		if (MVInputWrapper.DebugGetKeyDown(KeyCode.Alpha4) && MVGameController.EditorController.EditorStateMachine.SingleSelectedWO is MVCubeModelInstance mVCubeModelInstance2)
		{
			Hashtable hashtable11 = new Hashtable();
			hashtable11["ClientSideType"] = (byte)13;
			hashtable11[BlueprintData.ChildrenMap.ToString()] = new Hashtable { { "movable", mVCubeModelInstance2.Id } };
			hashtable11["AngularDirection"] = "0 1 0";
			hashtable11["AngularSpeed"] = 1f;
			Hashtable hashtable3 = new Hashtable();
			hashtable3.Add("BlueprintData", hashtable11);
			Hashtable value8 = hashtable3;
			MVGameController.EditorController.EditorStateMachine.Data.Add("woData", value8);
			MVGameController.EditorController.EditorStateMachine.PushState(EditorEvent.ESBlueprintCreator);
		}
		if (MVInputWrapper.DebugGetKeyDown(KeyCode.Alpha5) && MVGameController.EditorController.EditorStateMachine.SingleSelectedWO is MVCubeModelInstance mVCubeModelInstance3)
		{
			Hashtable hashtable12 = new Hashtable();
			hashtable12["ClientSideType"] = (byte)13;
			hashtable12[BlueprintData.ChildrenMap.ToString()] = new Hashtable { { "movable", mVCubeModelInstance3.Id } };
			hashtable12["AngularDirection"] = "1 0 0";
			hashtable12["AngularSpeed"] = 1f;
			Hashtable hashtable3 = new Hashtable();
			hashtable3.Add("BlueprintData", hashtable12);
			Hashtable value9 = hashtable3;
			MVGameController.EditorController.EditorStateMachine.Data.Add("woData", value9);
			MVGameController.EditorController.EditorStateMachine.PushState(EditorEvent.ESBlueprintCreator);
		}
		if (MVInputWrapper.DebugGetKeyDown(KeyCode.Alpha6))
		{
			MVMovingPlatformGroup mVMovingPlatformGroup = MVGameController.EditorController.EditorStateMachine.SingleSelectedWO as MVMovingPlatformGroup;
			if (mVMovingPlatformGroup != null)
			{
			}
			MVRotator mVRotator = MVGameController.EditorController.EditorStateMachine.SingleSelectedWO as MVRotator;
			if (mVRotator == null)
			{
			}
		}
		if (MVInputWrapper.DebugGetKeyDown(KeyCode.Alpha7))
		{
		}
		if (MVInputWrapper.DebugGetKeyDown(KeyCode.Alpha8))
		{
			MVWorldObjectClient singleSelectedWO = MVGameController.EditorController.EditorStateMachine.SingleSelectedWO;
			if (singleSelectedWO is MVRotator)
			{
				MVGameController.Game.AddWorldObjectToInventorDev(singleSelectedWO.Id, new byte[10], "Rotator 2", MVGameController.Game.ItemCategories.NameToID("Blueprint"), overWrite: false);
				Debug.Log("*** ADDING MVRotator TO INVENTORY");
			}
			if (singleSelectedWO is MVMovingPlatformGroup)
			{
				MVGameController.Game.AddWorldObjectToInventorDev(singleSelectedWO.Id, new byte[10], "Moving platform", MVGameController.Game.ItemCategories.NameToID("Blueprint"), overWrite: false);
				Debug.Log("*** ADDING MVMovingPlatformGroup TO INVENTORY");
			}
			else if (singleSelectedWO is MVCubeModelInstance)
			{
				MVGameController.Game.AddWorldObjectToInventory(singleSelectedWO.Id, new byte[10]);
				Debug.Log("*** ADDING CUBE MODEL TO INVENTORY");
			}
		}
	}

	private static void SARentProductResponseHandler(int retCode, Dictionary<object, object> resData)
	{
		if (retCode == 0)
		{
			MVNetworkGame game = Game;
			game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Remove(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(SARentProductResponseHandler));
			int key = (int)resData[(byte)62];
			int num = (int)resData[(byte)73];
			long ticks = (long)resData[(byte)83];
			int num2 = (int)resData[(byte)80];
			DateTime dateTime = new DateTime(ticks);
			Debug.Log("Rented ivnID " + num + " at " + dateTime);
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
				ProductInventoryInfo productInventoryInfo = Game.StreamingAssetInventory.Get(num);
				if (productInventoryInfo == null)
				{
					productInventoryInfo = new ProductInventoryInfo(num, value, dateTime, isRented: true);
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
			Debug.LogWarning("COULD NOT PURCHASE OR RENT");
		}
	}

	private static void AvatarAccessoryCreatedHandler(AvatarAccessory acc)
	{
		MVBody mVBody = WOCM.GetWorldObjectClient(testBodyId) as MVBody;
		Debug.Log("Create accessory " + acc.InventoryID + " body found: " + (mVBody != null));
		if (mVBody != null && acc != null)
		{
			ProductInventoryInfo invInfo = Game.StreamingAssetInventory.Get(acc.InventoryID);
			mVBody.AttachAccessoryPermanent(acc, acc.DefaultSlot, 0f, invInfo);
		}
	}

	private static void OnAvatarShopInventoryResultSetResponse(Hashtable outData, int largeQueryId, bool isDone)
	{
		foreach (int key in outData.Keys)
		{
			Hashtable hashtable = (Hashtable)outData[key];
			Debug.Log("data.Length " + ((byte[])hashtable[(byte)92]).Length);
			int num2 = (int)hashtable[(byte)77];
			int num3 = (int)hashtable[(byte)76];
			Debug.Log("priceSilver " + num2);
			Debug.Log("priceGold " + num3);
			Debug.Log("AvatarID " + key);
		}
	}
}
