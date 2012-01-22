using System;
using UnityEngine;

public class MVGUIPurchaseItemBox : UXViewScript
{
	public UXText messageText;

	public UXButton okButton;

	public UXButton cancelButton;

	private int price;

	private int itemID;

	private int prototypeID;

	public static MVGUIPurchaseItemBox New(string message, int price, int itemID, int prototypeID)
	{
		Object val = Object.Instantiate(Resources.Load("Prefabs/GUI/PurchaseItemBox"));
		MVGUIPurchaseItemBox purchaseItemBox = ((GameObject)((val is GameObject) ? val : null)).GetComponent<MVGUIPurchaseItemBox>();
		purchaseItemBox.messageText.Text = message;
		purchaseItemBox.price = price;
		purchaseItemBox.itemID = itemID;
		purchaseItemBox.prototypeID = prototypeID;
		purchaseItemBox.Initialize();
		UXView uXView = purchaseItemBox.View;
		uXView.OnHide = (UXView.OnHideDelegate)Delegate.Combine(uXView.OnHide, (UXView.OnHideDelegate)(() =>
		{
			UXFullscreenColliderBox.Instance.RemoveBlockingObject(purchaseItemBox);
			Object.Destroy((Object)(object)((Component)purchaseItemBox).gameObject);
		}));
		UXButton uXButton = purchaseItemBox.okButton;
		uXButton.OnClick = (UXButton.OnClickDelegate)Delegate.Combine(uXButton.OnClick, (UXButton.OnClickDelegate)(() =>
		{
			MVGameController.Instance.Game.PurchaseItem(itemID, prototypeID);
			purchaseItemBox.View.Hide();
		}));
		UXButton uXButton2 = purchaseItemBox.cancelButton;
		uXButton2.OnClick = (UXButton.OnClickDelegate)Delegate.Combine(uXButton2.OnClick, (UXButton.OnClickDelegate)(() =>
		{
			purchaseItemBox.View.Hide();
		}));
		UXFullscreenColliderBox.Instance.AddBlockingObject(purchaseItemBox);
		return purchaseItemBox;
	}
}
