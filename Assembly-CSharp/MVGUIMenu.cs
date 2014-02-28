using System;
using UnityEngine;

public class MVGUIMenu : UXViewScript
{
	public MVGUIPlayersWindow playersWindow;

	public MVGUIRespawnButton respawnButton;

	public MVGUIChangeTeamButton changeTeamButton;

	public Transform iconRoot;

	public UXView EditIconRoot;

	public UXView CharacterEditorIconRoot;

	public Material menuBGMaterial;

	public float menuBGWidth = 40f;

	public float menuBGHeight = 5f;

	private bool _shortcut;

	private bool _isInitialized;

	private MVGUIPressM pressMText;

	public override void OnInitialize()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected Obj, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("IconBG");
		val.layer = LayerMask.NameToLayer("UXElement");
		val.transform.parent = iconRoot;
		val.transform.localPosition = new Vector3(0f, 0f, 0.5f);
		val.transform.localScale = Vector3.one;
		val.AddComponent<UXPlane>().SetSize(menuBGWidth, menuBGHeight);
		MeshRenderer val2 = val.AddComponent<MeshRenderer>();
		((Renderer)val2).material = menuBGMaterial;
		pressMText = UXUtils.FindGUIObjectOfType<MVGUIPressM>();
	}

	private void InitializeComponents()
	{
		playersWindow.InitializeListeners();
		respawnButton.Initialize();
		changeTeamButton.Initialize();
		UXIconButton chatIcon = ((Component)this).gameObject.GetComponentInChildren<MVGUIChatWindowToggle>().chatIcon;
		chatIcon.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(chatIcon.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			View.Hide();
		}));
		UXIconButton componentInChildren = ((Component)respawnButton).GetComponentInChildren<UXIconButton>();
		componentInChildren.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(componentInChildren.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			View.Hide();
		}));
		UXIconButton component = ((Component)changeTeamButton).GetComponent<UXIconButton>();
		component.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(component.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			View.Hide();
		}));
		UXIconButton publishButton = ((Component)EditIconRoot).GetComponentInChildren<MVGUIPublishButton>().publishButton;
		publishButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(publishButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			View.Hide();
		}));
		UXToggleIconButton playButton = ((Component)EditIconRoot).GetComponentInChildren<MVGUIPlayButton>().playButton;
		playButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(playButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			View.Hide();
		}));
		_isInitialized = true;
	}

	public override void OnShow()
	{
		base.OnShow();
		if (!_isInitialized)
		{
			InitializeComponents();
		}
		if (MVGameController.Instance.EditController != null)
		{
			EditIconRoot.Show();
		}
		if (MVGameController.Instance.CharacterEditorController != null)
		{
			CharacterEditorIconRoot.Show();
		}
		Screen.lockCursor = false;
		pressMText.ShowOpenText(_shortcut);
		MVGameController.Instance.WOCM.AvatarLocal.ShowHealth = false;
		UXFullscreenColliderBox.Instance.AddBlockingObject(this);
	}

	public override void OnHide()
	{
		base.OnHide();
		if (MVGameController.Instance.EditController != null)
		{
			EditIconRoot.Hide();
		}
		if (MVGameController.Instance.CharacterEditorController != null)
		{
			CharacterEditorIconRoot.Hide();
		}
		if (_isInitialized)
		{
			Screen.lockCursor = ShouldLockCursor();
			pressMText.ShowStandardText();
			if (_isInitialized && MVGameController.Instance.WOCM != null)
			{
				MVGameController.Instance.WOCM.AvatarLocal.ShowHealth = ThirdPersonPlay();
			}
			UXFullscreenColliderBox.Instance.RemoveBlockingObject(this);
		}
	}

	private bool ShouldLockCursor()
	{
		return ThirdPersonPlay() && !UXUtils.FindGUIObjectOfType<UXDialogFactory>().DialogOpen;
	}

	private bool ThirdPersonPlay()
	{
		return (MVGameController.Instance.EditController != null && MVGameController.Instance.EditController.PlayInEditor) || MVGameController.Instance.IngameController is PlayController;
	}

	public void ShowOnShortcut(bool show)
	{
		if (show)
		{
			_shortcut = true;
			if (!View.isVisible)
			{
				View.Show();
			}
		}
		if (!show && _shortcut)
		{
			_shortcut = false;
			View.Hide();
		}
	}
}
