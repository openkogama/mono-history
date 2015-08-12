using System.Collections.Generic;
using UnityEngine;

public class MVGUIBMManageChildren : UXCustomDialogBox
{
	public MVGUIBMChildLine childLinePrefab;

	public UXScrollableBox childBox;

	public UXText childText;

	public UXTextButton addChildButton;

	private UXLine deleteLine;

	private bool _isInitialized;

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		if (!_isInitialized)
		{
			addChildButton.OnClick = () =>
			{
				AddChild();
			};
			_isInitialized = true;
		}
	}

	public override void OnCloseDialog()
	{
		base.OnCloseDialog();
		(UXUtils.FindComponentInParents(typeof(UXView), transform.parent) as UXView).ReleaseFocus();
	}

	public void BuildChildren(Dictionary<object, object> childMap)
	{
		foreach (string key in childMap.Keys)
		{
			BuildChildLine(key, (int)childMap[key]);
		}
		childText.Text = $"Children: {childBox.GetLines().Count}";
	}

	private void BuildChildLine(string name, int woid)
	{
		MVGUIBMChildLine mVGUIBMChildLine = Object.Instantiate(childLinePrefab);
		mVGUIBMChildLine.BuildLine(name, woid);
		mVGUIBMChildLine.OnRemoveChildLine = OnDeleteChildLine;
		childBox.AddLine(mVGUIBMChildLine);
	}

	private void OnDeleteChildLine(UXLine line)
	{
		deleteLine = line;
		DialogFactory.CreateDialog("Delete Child ?", "Delete", UXDialogType.Simple, noButtons: false, stackDialog: true).SetOnResultCallback(OnDeleteChildResponse).AddPositiveButton("Yes")
			.AddNegativeButton("No")
			.Show();
	}

	private void OnDeleteChildResponse(UXDialogBox dialog)
	{
		if (dialog.DialogResult == UXDialogResult.Positive)
		{
			childBox.RemoveLine(deleteLine);
		}
		deleteLine = null;
		childText.Text = $"Children: {childBox.GetLines().Count}";
	}

	private void AddChild()
	{
		DialogFactory.CreateCustomDevelopmentDialog("Prefabs/GUI/Dev Tools/BlueprintManager/ManageChildren/BlueprintManagerAddChild", "Add Child", noButtons: true, stackDialog: true).SetOnResultCallback(OnAddChildResponse).Show();
	}

	private void OnAddChildResponse(UXDialogBox dialogBox)
	{
		if (dialogBox.DialogResult == UXDialogResult.Positive)
		{
			Dictionary<object, object> dictionary = (Dictionary<object, object>)dialogBox.GetResult();
			string text = (string)dictionary["name"];
			int woid = (int)dictionary["woId"];
			BuildChildLine(text, woid);
			childText.Text = $"Children: {childBox.GetLines().Count}";
		}
	}

	public override object GetResult()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		foreach (UXLine line in childBox.GetLines())
		{
			MVGUIBMChildLine mVGUIBMChildLine = (MVGUIBMChildLine)line;
			if (mVGUIBMChildLine.GetName() != string.Empty && mVGUIBMChildLine.GetWoId() > 0)
			{
				dictionary.Add(mVGUIBMChildLine.GetName(), mVGUIBMChildLine.GetWoId());
			}
			else
			{
				Debug.Log($"Ignoring child with name '{mVGUIBMChildLine.GetName()}' and woid '{mVGUIBMChildLine.GetWoId()}'");
			}
		}
		return dictionary;
	}
}
