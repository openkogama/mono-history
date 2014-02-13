using System;

public class AvatarAccessoryShopViewItem : AvatarAccessoryBasicViewItem
{
	public delegate void OnPreviewAvatarAccessoryDelegate(AvatarAccessoryShopViewItem viewItem);

	public OnPreviewAvatarAccessoryDelegate OnPreviewAvatarAccessory;

	public UXToggleIconButton previewButton;

	public override void Initialize()
	{
		if (!_isBuilding)
		{
			base.Initialize();
			UXToggleIconButton uXToggleIconButton = previewButton;
			uXToggleIconButton.OnToggle = (UXToggleIconButton.OnToggleDelegate)Delegate.Combine(uXToggleIconButton.OnToggle, new UXToggleIconButton.OnToggleDelegate(OnPreviewButtonToggle));
		}
	}

	private void OnPreviewButtonToggle(bool toggle)
	{
		if (toggle)
		{
			if (OnPreviewAvatarAccessory != null)
			{
				OnPreviewAvatarAccessory(this);
			}
		}
		else if (OnPreviewAvatarAccessory != null)
		{
			OnPreviewAvatarAccessory(null);
		}
	}

	public override void SetVisible(bool visible)
	{
		base.SetVisible(visible);
		previewButton.SetVisible(visible);
	}
}
