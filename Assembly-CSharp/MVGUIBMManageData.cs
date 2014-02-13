using System;
using System.Collections;
using System.Collections.Generic;
using Localize;
using UnityEngine;

public class MVGUIBMManageData : UXCustomDialogBox
{
	private const string CHILDREN_KEY = "ChildrenMap";

	private const string TYPE_KEY = "ClientSideType";

	public MVGUIBMIntDataLine intLinePrefab;

	public MVGUIBMFloatDataLine floatLinePrefab;

	public MVGUIBMStringDataLine stringLinePrefab;

	public UXScrollableBox dataBox;

	public UXTextButton addDataButton;

	public UXText unknownDataText;

	private UXLine deleteLine;

	private int _unknownData;

	private bool _isInitialized;

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		if (!_isInitialized)
		{
			addDataButton.OnClick = () =>
			{
				AddData();
			};
			_isInitialized = true;
		}
	}

	public void BuildData(Hashtable data)
	{
		_unknownData = 0;
		foreach (string key in data.Keys)
		{
			if (!(key == "ChildrenMap") && !(key == "ClientSideType"))
			{
				BuildDataLine(key, data[key]);
			}
		}
		unknownDataText.Text = $"Data with other type: {_unknownData}";
	}

	private void BuildDataLine(string name, object data)
	{
		MVGUIBMDataLine mVGUIBMDataLine = null;
		if (data is int)
		{
			mVGUIBMDataLine = (MVGUIBMIntDataLine)(object)Object.Instantiate((Object)(object)intLinePrefab);
		}
		else if (data is float)
		{
			mVGUIBMDataLine = (MVGUIBMFloatDataLine)(object)Object.Instantiate((Object)(object)floatLinePrefab);
		}
		else if (data is string)
		{
			mVGUIBMDataLine = (MVGUIBMStringDataLine)(object)Object.Instantiate((Object)(object)stringLinePrefab);
		}
		if ((Object)(object)mVGUIBMDataLine != (Object)null)
		{
			mVGUIBMDataLine.BuildLine(name, data);
			mVGUIBMDataLine.OnRemoveDataLine = OnDeleteDataLine;
			dataBox.AddLine(mVGUIBMDataLine);
		}
		else
		{
			_unknownData++;
		}
	}

	private void OnDeleteDataLine(UXLine line)
	{
		deleteLine = line;
		DialogFactory.CreateDialog(TextSlotIndex.DeleteConfirm, TextSlotIndex.DeleteHeadline, UXDialogType.Simple, noButtons: false, stackDialog: true).SetOnResultCallback(OnDeleteDataResponse).AddPositiveButton(TextSlotIndex.Confirm)
			.AddNegativeButton(TextSlotIndex.Reject)
			.Show();
	}

	private void OnDeleteDataResponse(UXDialogBox dialog)
	{
		if (dialog.DialogResult == UXDialogResult.Positive)
		{
			dataBox.RemoveLine(deleteLine);
		}
		deleteLine = null;
	}

	private void AddData()
	{
		string[] names = Enum.GetNames(typeof(BlueprintManagerDataType));
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		dictionary.Add("ComboBox", new TextComboBoxData
		{
			items = names
		});
		DialogFactory.CreateCustomDevelopmentDialog("Prefabs/GUI/Dev Tools/BlueprintManager/ManageData/BlueprintManagerAddData", "Add Data", noButtons: true, stackDialog: true).SetValues(dictionary).SetOnResultCallback(OnAddDataResponse)
			.Show();
	}

	private void OnAddDataResponse(UXDialogBox dialogBox)
	{
		if (dialogBox.DialogResult == UXDialogResult.Positive)
		{
			Hashtable hashtable = (Hashtable)dialogBox.GetResult();
			string name = (string)hashtable["name"];
			BlueprintManagerDataType type = (BlueprintManagerDataType)(int)hashtable["type"];
			BuildDataLine(name, GetDefaultDataFromDataType(type));
		}
	}

	private object GetDefaultDataFromDataType(BlueprintManagerDataType type)
	{
		return type switch
		{
			BlueprintManagerDataType.Int => (object)0, 
			BlueprintManagerDataType.Float => 0f, 
			BlueprintManagerDataType.String => string.Empty, 
			_ => null, 
		};
	}

	public override object GetResult()
	{
		Hashtable hashtable = new Hashtable();
		foreach (UXLine line in dataBox.GetLines())
		{
			MVGUIBMDataLine mVGUIBMDataLine = (MVGUIBMDataLine)line;
			if (mVGUIBMDataLine.GetName() != string.Empty && mVGUIBMDataLine.GetValue() != null)
			{
				hashtable.Add(mVGUIBMDataLine.GetName(), mVGUIBMDataLine.GetValue());
			}
			else
			{
				Debug.Log((object)$"Ignoring data with name '{mVGUIBMDataLine.GetName()}' and value '{mVGUIBMDataLine.GetValue()}'");
			}
		}
		return hashtable;
	}
}
