using System;
using System.Collections;
using System.Collections.Generic;
using Localize;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class MVGUIBlueprintManagerOverview : UXCustomDialogBox
{
	private readonly string BLUEPRINT_DATA_KEY = "BlueprintData";

	private readonly string CHILDREN_KEY = BlueprintData.ChildrenMap.ToString();

	private readonly string TYPE_KEY = BlueprintData.ClientSideType.ToString();

	public UXText typeText;

	public UXText childrenText;

	public UXTextButton manageChildren;

	public UXText dataText;

	public UXTextButton manageData;

	public UXTextButton okButton;

	private int woId;

	private MVWorldObjectClient blueprintWorldObject;

	private Hashtable blueprintData;

	private bool _isInitialized;

	public void SetWoid(int woId)
	{
		this.woId = woId;
		if (woId != -1 && MVGameController.Instance.WOCM.GetWorldObjectClient(woId).Data.ContainsKey(BLUEPRINT_DATA_KEY))
		{
			blueprintWorldObject = MVGameController.Instance.WOCM.GetWorldObjectClient(woId);
			blueprintData = (Hashtable)blueprintWorldObject.Data[BLUEPRINT_DATA_KEY];
			if (blueprintData.ContainsKey(TYPE_KEY))
			{
				typeText.Text = string.Empty + (BlueprintType)(byte)blueprintData[TYPE_KEY];
			}
			else
			{
				string[] names = Enum.GetNames(typeof(BlueprintType));
				Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
				dictionary.Add("ComboBox", new TextComboBoxData
				{
					items = names
				});
				DialogFactory.CreateDevelopmentDialog("Select Type", "Missing Type", UXDialogType.ComboBox, noButtons: false, stackDialog: true, canClose: false).SetOnResultCallback(OnSelectTypeResponse).SetValues(dictionary)
					.Show();
			}
			if (blueprintData.ContainsKey(CHILDREN_KEY))
			{
				childrenText.Text = string.Empty + ((Hashtable)blueprintData[CHILDREN_KEY]).Count;
			}
			else
			{
				blueprintData.Add(CHILDREN_KEY, new Hashtable());
				childrenText.Text = string.Empty + 0;
			}
			dataText.Text = string.Empty + (blueprintData.Count - 2);
			PrintData(blueprintData, string.Empty);
		}
		else
		{
			blueprintData = new Hashtable();
			blueprintData.Add(CHILDREN_KEY, new Hashtable());
			childrenText.Text = string.Empty + 0;
			dataText.Text = string.Empty + 0;
			string[] names2 = Enum.GetNames(typeof(BlueprintType));
			Dictionary<string, DialogData> dictionary2 = new Dictionary<string, DialogData>();
			dictionary2.Add("ComboBox", new TextComboBoxData
			{
				items = names2
			});
			DialogFactory.CreateDialog(TextSlotIndex.SelectType, TextSlotIndex.SelectTypeHeadline, UXDialogType.ComboBox, noButtons: false, stackDialog: true, canClose: false).SetOnResultCallback(OnSelectTypeResponse).SetValues(dictionary2)
				.Show();
		}
	}

	private void OnSelectTypeResponse(UXDialogBox dialogBox)
	{
		if (dialogBox.DialogResult == UXDialogResult.Positive)
		{
			string text = (string)dialogBox.GetResult();
			if (text != string.Empty)
			{
				BlueprintType blueprintType = (BlueprintType)(byte)Enum.Parse(typeof(BlueprintType), text);
				blueprintData.Add(TYPE_KEY, (byte)blueprintType);
				typeText.Text = text;
			}
		}
	}

	private void PrintData(Hashtable table, string insert = "")
	{
		foreach (string key in table.Keys)
		{
			Debug.Log((object)$"{insert}{key}: {table[key]}");
			if (table[key] is Hashtable)
			{
				PrintData(table[key] as Hashtable, insert + "    ");
			}
		}
	}

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		if (!_isInitialized)
		{
			UXTextButton uXTextButton = manageChildren;
			uXTextButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
			{
				ManageChildren();
			}));
			UXTextButton uXTextButton2 = manageData;
			uXTextButton2.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton2.OnClick, (UXBaseButton.OnClickDelegate)(() =>
			{
				ManageData();
			}));
			UXTextButton uXTextButton3 = okButton;
			uXTextButton3.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton3.OnClick, (UXBaseButton.OnClickDelegate)(() =>
			{
				CreateBlueprint();
			}));
			_isInitialized = true;
		}
	}

	private void ManageChildren()
	{
		DialogFactory.CreateCustomDevelopmentDialog("Prefabs/GUI/Dev Tools/BlueprintManager/ManageChildren/BlueprintManagerManageChildren", "Manage Children", noButtons: false, stackDialog: true).AddPositiveButton(TextSlotIndex.Ok).AddNegativeButton(TextSlotIndex.Cancel)
			.SetOnResultCallback(OnManageChildrenResponse)
			.Show();
		(DialogFactory.CurrentDialogBox as MVGUIBMManageChildren).BuildChildren((Hashtable)blueprintData[CHILDREN_KEY]);
	}

	private void OnManageChildrenResponse(UXDialogBox dialogBox)
	{
		if (dialogBox.DialogResult == UXDialogResult.Positive)
		{
			Hashtable hashtable = (Hashtable)dialogBox.GetResult();
			blueprintData[CHILDREN_KEY] = hashtable;
			childrenText.Text = string.Empty + hashtable.Count;
		}
	}

	private void ManageData()
	{
		DialogFactory.CreateCustomDevelopmentDialog("Prefabs/GUI/Dev Tools/BlueprintManager/ManageData/BlueprintManagerManageData", "Manage Data", noButtons: false, stackDialog: true).AddPositiveButton(TextSlotIndex.Ok).AddNegativeButton(TextSlotIndex.Cancel)
			.SetOnResultCallback(OnManageDataResponse)
			.Show();
		(DialogFactory.CurrentDialogBox as MVGUIBMManageData).BuildData(blueprintData);
	}

	private void OnManageDataResponse(UXDialogBox dialogBox)
	{
		if (dialogBox.DialogResult != UXDialogResult.Positive)
		{
			return;
		}
		Hashtable hashtable = (Hashtable)dialogBox.GetResult();
		byte b = (byte)blueprintData[TYPE_KEY];
		Hashtable value = (Hashtable)blueprintData[CHILDREN_KEY];
		blueprintData = new Hashtable();
		foreach (string key in hashtable.Keys)
		{
			blueprintData.Add(key, hashtable[key]);
		}
		blueprintData.Add(TYPE_KEY, b);
		blueprintData.Add(CHILDREN_KEY, value);
		dataText.Text = string.Empty + hashtable.Count;
	}

	private void CreateBlueprint()
	{
		if (woId != -1)
		{
			Hashtable data = blueprintWorldObject.Data;
			if (!data.ContainsKey(BLUEPRINT_DATA_KEY))
			{
				data.Add(BLUEPRINT_DATA_KEY, blueprintData);
			}
			else
			{
				data[BLUEPRINT_DATA_KEY] = blueprintData;
			}
			MVGameController.Instance.Game.UpdateWorldObjectData(blueprintWorldObject.Id, data);
		}
		else
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add(BLUEPRINT_DATA_KEY, blueprintData);
			Hashtable value = hashtable;
			MVGameController.Instance.EditController.EditorStateMachine.Data.Add("woData", value);
			MVGameController.Instance.EditController.EditorStateMachine.PushState(EditorEvent.ESBlueprintCreator);
		}
		OnPositiveClose();
		DialogFactory.CloseDialog();
	}
}
