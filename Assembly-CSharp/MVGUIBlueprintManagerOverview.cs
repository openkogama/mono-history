using System;
using System.Collections.Generic;
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

	private Dictionary<object, object> blueprintData;

	private bool _isInitialized;

	public void SetWoid(int woId)
	{
		this.woId = woId;
		if (woId != -1 && MVGameControllerBase.WOCM.GetWorldObjectClient(woId).Data.ContainsKey(BLUEPRINT_DATA_KEY))
		{
			blueprintWorldObject = MVGameControllerBase.WOCM.GetWorldObjectClient(woId);
			blueprintData = (Dictionary<object, object>)blueprintWorldObject.Data[BLUEPRINT_DATA_KEY];
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
					items = names,
					currentlySelectedIndex = 0
				});
				DialogFactory.CreateDevelopmentDialog("Select Type", "Missing Type", UXDialogType.ComboBox, noButtons: false, stackDialog: true, canClose: false).SetOnResultCallback(OnSelectTypeResponse).SetValues(dictionary)
					.Show();
			}
			if (blueprintData.ContainsKey(CHILDREN_KEY))
			{
				childrenText.Text = string.Empty + ((Dictionary<object, object>)blueprintData[CHILDREN_KEY]).Count;
			}
			else
			{
				blueprintData.Add(CHILDREN_KEY, new Dictionary<object, object>());
				childrenText.Text = string.Empty + 0;
			}
			dataText.Text = string.Empty + (blueprintData.Count - 2);
			PrintData(blueprintData, string.Empty);
		}
		else
		{
			blueprintData = new Dictionary<object, object>();
			blueprintData.Add(CHILDREN_KEY, new Dictionary<object, object>());
			childrenText.Text = string.Empty + 0;
			dataText.Text = string.Empty + 0;
			string[] names2 = Enum.GetNames(typeof(BlueprintType));
			Dictionary<string, DialogData> dictionary2 = new Dictionary<string, DialogData>();
			dictionary2.Add("ComboBox", new TextComboBoxData
			{
				items = names2,
				currentlySelectedIndex = 0
			});
			DialogFactory.CreateDialog(TM._("Select Type"), TM._("Type"), UXDialogType.ComboBox, noButtons: false, stackDialog: true, canClose: false).SetOnResultCallback(OnSelectTypeResponse).SetValues(dictionary2)
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

	private void PrintData(Dictionary<object, object> table, string insert = "")
	{
		foreach (string key in table.Keys)
		{
			Debug.Log($"{insert}{key}: {table[key]}");
			if (table[key] is Dictionary<object, object>)
			{
				PrintData(table[key] as Dictionary<object, object>, insert + "    ");
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
		DialogFactory.CreateCustomDevelopmentDialog("Prefabs/GUI/Dev Tools/BlueprintManager/ManageChildren/BlueprintManagerManageChildren", "Manage Children", noButtons: false, stackDialog: true).AddPositiveButton(TM._("Ok")).AddNegativeButton(TM._("Cancel"))
			.SetOnResultCallback(OnManageChildrenResponse)
			.Show();
		(DialogFactory.CurrentDialogBox as MVGUIBMManageChildren).BuildChildren((Dictionary<object, object>)blueprintData[CHILDREN_KEY]);
	}

	private void OnManageChildrenResponse(UXDialogBox dialogBox)
	{
		if (dialogBox.DialogResult == UXDialogResult.Positive)
		{
			Dictionary<object, object> dictionary = (Dictionary<object, object>)dialogBox.GetResult();
			blueprintData[CHILDREN_KEY] = dictionary;
			childrenText.Text = string.Empty + dictionary.Count;
		}
	}

	private void ManageData()
	{
		DialogFactory.CreateCustomDevelopmentDialog("Prefabs/GUI/Dev Tools/BlueprintManager/ManageData/BlueprintManagerManageData", "Manage Data", noButtons: false, stackDialog: true).AddPositiveButton(TM._("Ok")).AddNegativeButton(TM._("Cancel"))
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
		Dictionary<object, object> dictionary = (Dictionary<object, object>)dialogBox.GetResult();
		byte b = (byte)blueprintData[TYPE_KEY];
		Dictionary<object, object> value = (Dictionary<object, object>)blueprintData[CHILDREN_KEY];
		blueprintData = new Dictionary<object, object>();
		foreach (string key in dictionary.Keys)
		{
			blueprintData.Add(key, dictionary[key]);
		}
		blueprintData.Add(TYPE_KEY, b);
		blueprintData.Add(CHILDREN_KEY, value);
		dataText.Text = string.Empty + dictionary.Count;
	}

	private void CreateBlueprint()
	{
		if (woId != -1)
		{
			Dictionary<object, object> data = blueprintWorldObject.Data;
			if (!data.ContainsKey(BLUEPRINT_DATA_KEY))
			{
				data.Add(BLUEPRINT_DATA_KEY, blueprintData);
			}
			else
			{
				data[BLUEPRINT_DATA_KEY] = blueprintData;
			}
			MVGameControllerBase.Game.UpdateWorldObjectData(blueprintWorldObject.Id, data);
		}
		else
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add(BLUEPRINT_DATA_KEY, blueprintData);
			Dictionary<object, object> value = dictionary;
			MVGameControllerLegacyUI.EditorController.EditorStateMachine.Data.Add("woData", value);
			MVGameControllerLegacyUI.EditorController.EditorStateMachine.PushState(EditorEvent.ESBlueprintCreator);
		}
		OnPositiveClose();
		DialogFactory.CloseDialog();
	}
}
