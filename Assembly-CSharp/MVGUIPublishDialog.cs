using System;
using UnityEngine;

public class MVGUIPublishDialog : UXViewScript
{
	public UXButton publishButton;

	public UXButton cancelButton;

	public static MVGUIPublishDialog New()
	{
		Object val = Object.Instantiate(Resources.Load("Prefabs/GUI/PublishDialog"));
		MVGUIPublishDialog dialog = ((GameObject)((val is GameObject) ? val : null)).GetComponent<MVGUIPublishDialog>();
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
		publishButton.OnClick = PublishButtonOnClick;
		cancelButton.OnClick = CancelButtonOnClick;
	}

	private void PublishButtonOnClick()
	{
		View.Hide();
		MVGameController.Instance.Game.PublishPlanet(new byte[0]);
	}

	private void CancelButtonOnClick()
	{
		View.Hide();
	}
}
