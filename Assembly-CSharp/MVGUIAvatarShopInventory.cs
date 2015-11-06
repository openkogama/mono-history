using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class MVGUIAvatarShopInventory : MVGUIInventoryGroup
{
	private MVItem _purchaseItem;

	protected override void InitializeCollectionView()
	{
		repositoryCollection = new ShopRepositoryCollection(MVGameControllerBase.Game.AvatarShopRepository, new int[0]);
		collectionView.InstansiateViewItem = InstansiateViewItem;
		UXCollectionView uXCollectionView = collectionView;
		uXCollectionView.OnItemSelection = (UXCollectionView.OnBasicItemEventDelegate)Delegate.Combine(uXCollectionView.OnItemSelection, new UXCollectionView.OnBasicItemEventDelegate(OnItemSelection));
		collectionView.Collection = repositoryCollection;
		collectionView.Initialize();
		collectionView.SetVisible(gameObject.GetComponent<UXViewScript>().View.isVisible);
	}

	public override void InitializeAfterReset()
	{
		CreatePreviewItemRoot();
		repositoryCollection = new ShopRepositoryCollection(MVGameControllerBase.Game.AvatarShopRepository, new int[0]);
		collectionView.Collection = repositoryCollection;
	}

	private void OnItemSelection(IUXCollectionItem item)
	{
		ShowAvatarPurchaseDialog(item);
	}

	private void ShowAvatarPurchaseDialog(IUXCollectionItem collectionItem)
	{
		_purchaseItem = (MVItem)collectionItem.Object;
		UXDialogFactory uXDialogFactory = UXUtils.UXDialogFactory;
		uXDialogFactory.CreateCustomDialog("Prefabs/GUI/Dialogs/BrightProductShopDialog", string.Empty, noButtons: true).SetOnResultCallback(OnPurchaseDialogResult).SetValues(BuildDialogData(_purchaseItem, collectionItem.Index))
			.Show();
		MVGUIProductShopDialog mVGUIProductShopDialog = (MVGUIProductShopDialog)uXDialogFactory.CurrentDialogBox;
		mVGUIProductShopDialog.SetPrice(_purchaseItem.priceGold, _purchaseItem.priceSilver);
		mVGUIProductShopDialog.OnTryPurchaseProduct = () =>
		{
			MVNetworkGame game = MVGameControllerBase.Game;
			game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Combine(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(OnProductPurchaseAvatarResponse));
			World world = MVGameControllerBase.Game.World;
			world.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Combine(world.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(InitializedPurchasedAvatar));
			MVGameControllerBase.Game.PurchaseAvatar(_purchaseItem.itemID);
		};
	}

	private void InitializedPurchasedAvatar(object sender, InitializedGameQueryDataEventArgs e)
	{
		World world = MVGameControllerBase.Game.World;
		world.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Remove(world.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(InitializedPurchasedAvatar));
		if (e.RootWO != null)
		{
			Debug.Log("Purchased avatar has been added to world. WorldObjectId is: " + e.RootWO);
			MVGameControllerLegacyUI.CharacterEditorController.AddNewAvatar(e.RootWO.Id);
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(e.RootWO.Id);
			GameObject bodyCloneGO = UnityEngine.Object.Instantiate(worldObjectClient.GameObject);
			Action<Texture2D> screenShotDataTexHandler = (Texture2D pngData) =>
			{
				MVGameControllerBase.Game.UploadScreenshot(pngData.EncodeToPNG(), ImageType.Avatar, MVGameControllerBase.Game.LocalPlayer.ProfileID);
			};
			AvatarScreenshotGenerator.Generate(bodyCloneGO, screenShotDataTexHandler);
		}
	}

	private void OnProductPurchaseAvatarResponse(int returnCode, Dictionary<object, object> purchaseResponseData)
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Remove(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(OnProductPurchaseAvatarResponse));
		Debug.Log("Avatar purchase response: " + (MVPurchaseReturnCode)returnCode);
		if (returnCode != 0)
		{
			World world = MVGameControllerBase.Game.World;
			world.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Remove(world.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(InitializedPurchasedAvatar));
		}
	}

	private Dictionary<string, DialogData> BuildDialogData(MVItem item, int slotIndex)
	{
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		dictionary.Add("ProductName", new TextData
		{
			text = "New Avatar"
		});
		AvatarViewItem avatarViewItem = (AvatarViewItem)collectionView.GetCollectionViewItemFromIndex(slotIndex);
		MVGUIAvatarShopPreview component = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/ShopPreview/AvatarShopPreview")) as GameObject).GetComponent<MVGUIAvatarShopPreview>();
		component.SetAvatarViewItem(avatarViewItem);
		dictionary.Add("ProductPreview", new ProductPreviewData
		{
			productPreview = component.gameObject
		});
		return dictionary;
	}

	private void OnPurchaseDialogResult(UXDialogBox dialogBox)
	{
		if (dialogBox.DialogResult == UXDialogResult.Positive)
		{
			MVGameControllerLegacyUI.CharacterEditorController.AvatarShop.View.Hide();
		}
		_purchaseItem = null;
	}
}
