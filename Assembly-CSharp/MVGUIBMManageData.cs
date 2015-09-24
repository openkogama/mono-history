using System;
using System.Collections.Generic;
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

	public void BuildData(Dictionary<object, object> data)
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
			mVGUIBMDataLine = UnityEngine.Object.Instantiate(intLinePrefab);
		}
		else if (data is float)
		{
			mVGUIBMDataLine = UnityEngine.Object.Instantiate(floatLinePrefab);
		}
		else if (data is string)
		{
			mVGUIBMDataLine = UnityEngine.Object.Instantiate(stringLinePrefab);
		}
		if (mVGUIBMDataLine != null)
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
		DialogFactory.CreateDialog(TM._("Are you sure\nwant to delete?"), TM._("Delete"), UXDialogType.Simple, noButtons: false, stackDialog: true).SetOnResultCallback(OnDeleteDataResponse).AddPositiveButton(TM._("Yes"))
			.AddNegativeButton(TM._("No"))
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
			items = names,
			currentlySelectedIndex = 0
		});
		DialogFactory.CreateCustomDevelopmentDialog("Prefabs/GUI/Dev Tools/BlueprintManager/ManageData/BlueprintManagerAddData", "Add Data", noButtons: true, stackDialog: true).SetValues(dictionary).SetOnResultCallback(OnAddDataResponse)
			.Show();
	}

	private void OnAddDataResponse(UXDialogBox dialogBox)
	{
		if (dialogBox.DialogResult == UXDialogResult.Positive)
		{
			Dictionary<object, object> dictionary = (Dictionary<object, object>)dialogBox.GetResult();
			string text = (string)dictionary["name"];
			BlueprintManagerDataType type = (BlueprintManagerDataType)(int)dictionary["type"];
			BuildDataLine(text, GetDefaultDataFromDataType(type));
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
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		foreach (UXLine line in dataBox.GetLines())
		{
			MVGUIBMDataLine mVGUIBMDataLine = (MVGUIBMDataLine)line;
			if (mVGUIBMDataLine.GetName() != string.Empty && mVGUIBMDataLine.GetValue() != null)
			{
				dictionary.Add(mVGUIBMDataLine.GetName(), mVGUIBMDataLine.GetValue());
			}
			else
			{
				Debug.Log($"Ignoring data with name '{mVGUIBMDataLine.GetName()}' and value '{mVGUIBMDataLine.GetValue()}'");
			}
		}
		return dictionary;
	}
}
