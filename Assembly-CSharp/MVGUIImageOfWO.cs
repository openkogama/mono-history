using System;
using System.Collections.Generic;
using MV.Common;

public class MVGUIImageOfWO : UXCustomDialogBox
{
	public UXTextButton pickButton;

	public UXTextButton createImageButton;

	public UXText nameText;

	public UXText woId;

	public UXTextButton useParentButton;

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		UXTextButton uXTextButton = pickButton;
		uXTextButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			PickChild();
		}));
		UXTextButton uXTextButton2 = useParentButton;
		uXTextButton2.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton2.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			GoToParent();
		}));
		UXTextButton uXTextButton3 = createImageButton;
		uXTextButton3.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton3.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			CreateImageOfWO();
		}));
	}

	private void PickChild()
	{
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		dictionary.Add("PickText", new TextData
		{
			text = "Select Child"
		});
		DialogFactory.CreateCustomDevelopmentDialog("Prefabs/GUI/Dev Tools/PickDialog", string.Empty, noButtons: true, stackDialog: true).SetValues(dictionary).SetOnResultCallback(OnPickChildResponse)
			.Show();
	}

	private void CreateImageOfWO()
	{
		int result = 0;
		if (int.TryParse(woId.Text, out result))
		{
			int itemID = MVGameControllerBase.WOCM.GetWorldObjectClient(result).ItemId;
			Action<byte[]> callback = (byte[] imageData) =>
			{
				MVGameControllerBase.Game.UploadScreenshot(imageData, ImageType.Item, itemID);
			};
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(result);
			Coroutines.StartCoroutine(ImageGenerator.CreateTextureFromData(worldObjectClient, callback));
		}
	}

	private void OnPickChildResponse(UXDialogBox dialogBox)
	{
		if (dialogBox.DialogResult == UXDialogResult.Positive)
		{
			int id = (int)dialogBox.GetResult();
			woId.Text = id.ToString();
			nameText.Text = MVGameControllerBase.WOCM.GetWorldObjectClient(id).GameObject.name;
		}
	}

	private void GoToParent()
	{
		int result = 0;
		if (int.TryParse(woId.Text, out result))
		{
			int groupId = MVGameControllerBase.WOCM.GetWorldObjectClient(result).GroupId;
			if (groupId != -1)
			{
				woId.Text = groupId.ToString();
				nameText.Text = MVGameControllerBase.WOCM.GetWorldObjectClient(groupId).GameObject.name;
			}
		}
	}
}
