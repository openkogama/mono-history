using System;
using UnityEngine;

public class MVGUIInventoryButton : UXViewScript
{
	public GameObject vaultPrefab;

	private MVGUIInventory vault;

	public UXButton button;

	private AudioBankSound openSound;

	private AudioBankSound closeSound;

	public override void OnInitialize()
	{
		openSound = GUIAudioBank.Instance.GetSound("inventory_open");
		closeSound = GUIAudioBank.Instance.GetSound("inventory_close");
		UXButton uXButton = button;
		uXButton.OnClick = (UXButton.OnClickDelegate)Delegate.Combine(uXButton.OnClick, new UXButton.OnClickDelegate(HandleOnClick));
	}

	public void HandleOnClick()
	{
		if ((Object)(object)vault == (Object)null)
		{
			Object val = Object.Instantiate(Resources.Load("Prefabs/GUI/Inventory"));
			GameObject val2 = (GameObject)(object)((val is GameObject) ? val : null);
			vault = val2.GetComponent<MVGUIInventory>();
			vault.Initialize();
			vault.View.Initialize();
			UXView uXView = vault.View;
			uXView.OnHide = (UXView.OnHideDelegate)Delegate.Combine(uXView.OnHide, (UXView.OnHideDelegate)(() =>
			{
				UXFullscreenColliderBox.Instance.RemoveBlockingObject(vault);
				UXFullscreenColliderBox.Instance.OnClick = null;
			}));
			UXView uXView2 = vault.View;
			uXView2.OnShow = (UXView.OnShowDelegate)Delegate.Combine(uXView2.OnShow, (UXView.OnShowDelegate)(() =>
			{
				UXFullscreenColliderBox.Instance.AddBlockingObject(vault);
				UXFullscreenColliderBox.Instance.OnClick = () =>
				{
					vault.View.Hide();
				};
			}));
		}
		if (vault.View.isVisible)
		{
			closeSound.Play();
			vault.View.Hide();
			Object.Destroy((Object)(object)((Component)vault).gameObject);
		}
		else
		{
			openSound.Play();
			vault.View.Show();
		}
	}
}
