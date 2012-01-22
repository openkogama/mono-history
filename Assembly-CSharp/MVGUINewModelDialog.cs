using System;
using UnityEngine;

public class MVGUINewModelDialog : UXViewScript
{
	private class Size
	{
		private UXToggle toggle;

		private float scale;

		public UXToggle Toggle => toggle;

		public float Scale => scale;

		public Size(MVGUINewModelDialog dialog, UXToggle toggle, float scale)
		{
			Size size = this;
			this.toggle = toggle;
			this.scale = scale;
			toggle.OnToggle = (UXToggle t) =>
			{
				if (toggle.On)
				{
					Size[] sizes = dialog.sizes;
					foreach (Size size2 in sizes)
					{
						if (size2 != size)
						{
							size2.toggle.On = false;
						}
					}
				}
			};
		}
	}

	public UXTextField nameTextField;

	public UXToggle smallToggle;

	public UXToggle mediumToggle;

	public UXToggle largeToggle;

	public UXButton okButton;

	public UXButton cancelButton;

	private Size[] sizes;

	public static MVGUINewModelDialog New()
	{
		Object val = Object.Instantiate(Resources.Load("Prefabs/GUI/NewModelDialog"));
		MVGUINewModelDialog dialog = ((GameObject)((val is GameObject) ? val : null)).GetComponent<MVGUINewModelDialog>();
		dialog.Initialize();
		UXView uXView = dialog.View;
		uXView.OnHide = (UXView.OnHideDelegate)Delegate.Combine(uXView.OnHide, (UXView.OnHideDelegate)(() =>
		{
			UXFullscreenColliderBox.Instance.RemoveBlockingObject(dialog);
			Object.Destroy((Object)(object)((Component)dialog).gameObject);
		}));
		UXFullscreenColliderBox.Instance.AddBlockingObject(dialog);
		return dialog;
	}

	public void Start()
	{
		sizes = new Size[3]
		{
			new Size(this, smallToggle, 0.25f),
			new Size(this, mediumToggle, 0.5f),
			new Size(this, largeToggle, 1f)
		};
		okButton.OnClick = OKButtonOnClick;
		cancelButton.OnClick = CancelButtonOnClick;
		View.Initialize();
		nameTextField.RequestFocus();
	}

	private void OKButtonOnClick()
	{
		MVGameController.Instance.EditorController.OnAddNewPrototype(nameTextField.Text, GetSelectedScale());
		View.Hide();
	}

	private void CancelButtonOnClick()
	{
		View.Hide();
	}

	private float GetSelectedScale()
	{
		Size[] array = sizes;
		foreach (Size size in array)
		{
			if (size.Toggle.On)
			{
				return size.Scale;
			}
		}
		throw new Exception("No model size selected.");
	}
}
