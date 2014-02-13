using System.Collections;
using Localize;
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
		(UXUtils.FindComponentInParents(typeof(UXView), ((Component)this).transform.parent) as UXView).ReleaseFocus();
	}

	public void BuildChildren(Hashtable childMap)
	{
		foreach (string key in childMap.Keys)
		{
			BuildChildLine(key, (int)childMap[key]);
		}
		childText.Text = $"Children: {childBox.GetLines().Count}";
	}

	private void BuildChildLine(string name, int woid)
	{
		MVGUIBMChildLine mVGUIBMChildLine = Object.Instantiate((Object)(object)childLinePrefab) as MVGUIBMChildLine;
		mVGUIBMChildLine.BuildLine(name, woid);
		mVGUIBMChildLine.OnRemoveChildLine = OnDeleteChildLine;
		childBox.AddLine(mVGUIBMChildLine);
	}

	private void OnDeleteChildLine(UXLine line)
	{
		deleteLine = line;
		DialogFactory.CreateDialog(TextSlotIndex.DeleteChildConfirm, TextSlotIndex.DeleteHeadline, UXDialogType.Simple, noButtons: false, stackDialog: true).SetOnResultCallback(OnDeleteChildResponse).AddPositiveButton(TextSlotIndex.Confirm)
			.AddNegativeButton(TextSlotIndex.Reject)
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
			Hashtable hashtable = (Hashtable)dialogBox.GetResult();
			string name = (string)hashtable["name"];
			int woid = (int)hashtable["woId"];
			BuildChildLine(name, woid);
			childText.Text = $"Children: {childBox.GetLines().Count}";
		}
	}

	public override object GetResult()
	{
		Hashtable hashtable = new Hashtable();
		foreach (UXLine line in childBox.GetLines())
		{
			MVGUIBMChildLine mVGUIBMChildLine = (MVGUIBMChildLine)line;
			if (mVGUIBMChildLine.GetName() != string.Empty && mVGUIBMChildLine.GetWoId() > 0)
			{
				hashtable.Add(mVGUIBMChildLine.GetName(), mVGUIBMChildLine.GetWoId());
			}
			else
			{
				Debug.Log((object)$"Ignoring child with name '{mVGUIBMChildLine.GetName()}' and woid '{mVGUIBMChildLine.GetWoId()}'");
			}
		}
		return hashtable;
	}
}
