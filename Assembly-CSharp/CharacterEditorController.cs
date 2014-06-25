using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterEditorController : AEditController
{
	public delegate void OnAvatarBodiesUpdatedDelegate(bool purchased);

	public OnAvatarBodiesUpdatedDelegate OnAvatarBodiesUpdated;

	private AvatarSelectionAnimator animator;

	private List<int> bodiesMarkedForDelete = new List<int>();

	private int bodyindex;

	private MVWorldObjectClientManager WOCM => MVGameController.Instance.WOCM;

	public MVGUIAnimationToggles AnimationToggles { get; private set; }

	public MVGUIAvatarSellcs AvatarSell { get; private set; }

	public MVGUICharacterEditScreenShot CharacterEditScreenshot { get; private set; }

	public MVGUIAvatarShopWindow AvatarShop { get; private set; }

	public MVGUIAvatarSlotButtonView AvatarSlotButtonView { get; private set; }

	public MVGUIAvatarAccessoryButtons AvatarAccessoryButtons { get; private set; }

	public MVGUIAvatarAccessoryShop AvatarAccessoryShop { get; private set; }

	public MVGUIAvatarAccessoryInventory AvatarAccessoryInventory { get; private set; }

	public int BodyCount => animator.Bodies.Count;

	public List<MVBody> Bodies => animator.Bodies;

	public int CurrentBodyIndex => bodyindex;

	public MVBody CurrentBody => animator.Bodies[CurrentBodyIndex];

	public Vector3 CenterPos
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return animator.displayPos;
		}
	}

	public override void Initialize()
	{
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize();
		animator = AvatarSelectionAnimator.Instance;
		CubeTools.paintCubeButton.Toggle();
		HideEditorTools();
		if (editorToggles.LogicRendered)
		{
			editorToggles.ToggleLogicRendering();
		}
		gameInfo.View.Hide();
		IOrderedEnumerable<MVWorldObjectClient> orderedEnumerable = from s in WOCM.GetWorldObjectClientsWhere((MVWorldObjectClient wo) => wo is MVBody mVBody3 && mVBody3.AttachedAvatar == null)
			orderby s.Id
			select s;
		foreach (MVWorldObjectClient item in orderedEnumerable)
		{
			MVBody mVBody = item as MVBody;
			mVBody.ShadowVisible = false;
			animator.Bodies.Add(mVBody);
		}
		MVBody mVBody2 = animator.Bodies.FirstOrDefault();
		animator.bodySpawnPoint = (MVSpawnPointRed)WOCM.GetWorldObjectClientWhere((MVWorldObjectClient wo) => wo is MVSpawnPointRed);
		MVSpawnPointBlue mVSpawnPointBlue = (MVSpawnPointBlue)WOCM.GetWorldObjectClientWhere((MVWorldObjectClient wo) => wo is MVSpawnPointBlue);
		animator.displayPos = animator.bodySpawnPoint.WorldPosition - Vector3.up;
		animator.hidePos = animator.bodySpawnPoint.WorldPosition - 51f * Vector3.up;
		animator.displayRotation = animator.bodySpawnPoint.WorldRotation;
		mVBody2.WorldPosition = animator.displayPos;
		mVBody2.WorldRotation = animator.displayRotation;
		foreach (MVBody item2 in animator.Bodies.Skip(1))
		{
			item2.WorldPosition = animator.hidePos;
			item2.WorldRotation = animator.bodySpawnPoint.WorldRotation;
		}
		MVAvatarLocal avatarLocal = MVGameController.Instance.WOCM.AvatarLocal;
		avatarLocal.WorldPosition = mVSpawnPointBlue.WorldPosition - Vector3.up;
		avatarLocal.WorldRotation = mVSpawnPointBlue.WorldRotation;
		Debug.Log((object)("BodyWOID " + mVBody2.Id));
		UpdateSellButton(mVBody2.Id);
		AvatarSlotButtonView.InitializeAvatarSlotButtonView();
		EditorStateMachine.EnterGroup(mVBody2);
		EditorStateMachine.Event = EditorEvent.CERoam;
	}

	private void UpdateSellButton(int woBodyId)
	{
		if (!MVGameController.Instance.Game.AvatarMetaDataWoMap.TryGetValue(woBodyId, out var avatarMetaData))
		{
			Debug.LogError((object)"Could not find woID");
			return;
		}
		Debug.Log((object)avatarMetaData);
		AvatarSell.UpdateAvatarMetaData(woBodyId, avatarMetaData);
	}

	protected override void ResolveGUIElements()
	{
		base.ResolveGUIElements();
		GameObject gameObject = ((Component)UXUtils.FindGUIObjectOfType<MVGUIAvatarEditor>()).gameObject;
		AnimationToggles = gameObject.GetComponentInChildren<MVGUIAnimationToggles>();
		AvatarSell = gameObject.GetComponentInChildren<MVGUIAvatarSellcs>();
		CharacterEditScreenshot = gameObject.GetComponentInChildren<MVGUICharacterEditScreenShot>();
		AvatarShop = gameObject.GetComponentInChildren<MVGUIAvatarShopWindow>();
		AvatarSlotButtonView = gameObject.GetComponentInChildren<MVGUIAvatarSlotButtonView>();
		AvatarAccessoryButtons = gameObject.GetComponentInChildren<MVGUIAvatarAccessoryButtons>();
		AvatarAccessoryInventory = gameObject.GetComponentInChildren<MVGUIAvatarAccessoryInventory>();
		AvatarAccessoryShop = gameObject.GetComponentInChildren<MVGUIAvatarAccessoryShop>();
	}

	protected override void SetPlayInEditorMode(bool playInEditor)
	{
		base.SetPlayInEditorMode(playInEditor: false);
		MVGameController.Instance.WOCM.AvatarLocal.ShowHealth = false;
	}

	public void ShowAnimationToggles()
	{
		AnimationToggles.View.Show();
	}

	public void HideAnimationToggles()
	{
		AnimationToggles.View.Hide();
	}

	public void ShowAvatarTools()
	{
		AvatarSell.View.Show();
		CharacterEditScreenshot.View.Show();
		AvatarSlotButtonView.View.Show();
		AvatarAccessoryButtons.View.Show();
	}

	public void HideAvatarTools()
	{
		AvatarSell.View.Hide();
		CharacterEditScreenshot.View.Hide();
		AvatarSlotButtonView.View.Hide();
		AvatarAccessoryButtons.View.Hide();
		AvatarShop.View.Hide();
	}

	public void ShowAvatarShopWindow()
	{
		ShowSingleWindow(AvatarShop.View);
	}

	public override void ToggleMenu()
	{
		base.ToggleMenu();
		if (menu.View.isVisible)
		{
			((Component)menu.changeTeamButton).gameObject.active = false;
			((Component)chatWindowToggle.chatIcon).gameObject.active = false;
			((Component)menu.respawnButton.respawn).gameObject.active = false;
			((Component)playButton.playButton).gameObject.active = false;
			((Component)UXUtils.FindGUIObjectOfType<MVGUIPublishButton>().publishButton).gameObject.active = false;
		}
	}

	public override void ShowPlayersWindow(bool show)
	{
		base.ShowPlayersWindow(show);
		if (show)
		{
			((Component)menu.changeTeamButton).gameObject.active = false;
			((Component)chatWindowToggle.chatIcon).gameObject.active = false;
			((Component)menu.respawnButton.respawn).gameObject.active = false;
			((Component)playButton.playButton).gameObject.active = false;
			((Component)UXUtils.FindGUIObjectOfType<MVGUIPublishButton>().publishButton).gameObject.active = false;
		}
	}

	public override void ShowEditorTools(bool cubeEditMode = false)
	{
		if (EditorStateMachine.curEvent != null && EditorStateMachine.CurEvent != EditorEvent.CERoam)
		{
			base.ShowEditorTools(cubeEditMode);
		}
	}

	public override void ToggleDrawPlane()
	{
		if (EditorStateMachine.CurEvent == EditorEvent.CEEditBody)
		{
			base.ToggleDrawPlane();
		}
	}

	public bool IsAvatarShopShown()
	{
		return AvatarShop.View.isVisible;
	}

	public override void ToggleShowUI()
	{
		base.ToggleShowUI();
		if (uiShown)
		{
			if (EditorStateMachine.CurEvent == EditorEvent.CERoam)
			{
				AnimationToggles.View.Show();
				CharacterEditScreenshot.View.Show();
				AvatarSell.View.Show();
				AvatarSlotButtonView.View.Show();
				AvatarAccessoryButtons.View.Show();
			}
			else
			{
				ShowEditorTools(cubeEditMode: true);
				HideAnimationToggles();
				HideAvatarTools();
			}
			gameInfo.View.Hide();
		}
		else
		{
			AvatarSell.View.Hide();
			AnimationToggles.View.Hide();
			CharacterEditScreenshot.View.Hide();
			AvatarShop.View.Hide();
			AvatarSlotButtonView.View.Hide();
			AvatarAccessoryButtons.View.Hide();
			AvatarAccessoryInventory.View.Hide();
			AvatarAccessoryShop.View.Hide();
		}
	}

	public override void RemoveUI()
	{
		base.RemoveUI();
		AvatarSell.View.Hide();
		AnimationToggles.View.Hide();
		CharacterEditScreenshot.View.Hide();
		AvatarSlotButtonView.View.Hide();
		AvatarAccessoryButtons.View.Hide();
		AvatarShop.ResetAvatarShop();
		AvatarShop.View.Hide();
		AvatarAccessoryInventory.View.Hide();
		AvatarAccessoryShop.View.Hide();
	}

	public void OpenAvatarAccessoryInventory()
	{
		OpenAvatarAccessoryView(AvatarAccessoryInventory.View);
	}

	public void OpenAvatarAccessoryShop()
	{
		OpenAvatarAccessoryView(AvatarAccessoryShop.View);
	}

	private void OpenAvatarAccessoryView(UXView view)
	{
		EditorStateMachine.Event = EditorEvent.CEAvatarAccessory;
		ShowSingleWindow(view);
	}

	public void CloseAvatarAccessoryView()
	{
		EditorStateMachine.Event = EditorEvent.CERoam;
		HideCurrentWindow();
	}

	public override void ShowChat(bool fromShortcut)
	{
	}

	public override void ShowNewModelWindow()
	{
	}

	public override void ShowInventory()
	{
	}

	public override void ShowShopInventory()
	{
	}

	public override void ToggleGridSnap()
	{
	}

	public override void ToggleLogicRendering()
	{
	}

	public override void RespawnAvatar()
	{
	}

	public override MVGUIMaterialSelectionWindow ShowMaterialChangeWindow()
	{
		if (EditorStateMachine.CurEvent == EditorEvent.CERoam || EditorStateMachine.CurEvent == EditorEvent.CEAvatarAccessory)
		{
			return null;
		}
		return base.ShowMaterialChangeWindow();
	}

	public void ShiftAvatarIndex(bool forward)
	{
		Debug.Log((object)("Shift avatar " + ((!forward) ? "back" : "forward")));
		int oldIndex = bodyindex;
		if (animator.Bodies.Count >= 2)
		{
			int num = 1;
			if (!forward)
			{
				num = -1;
			}
			bodyindex += num;
			if (bodyindex < 0)
			{
				bodyindex = animator.Bodies.Count - 1;
			}
			if (bodyindex >= animator.Bodies.Count)
			{
				bodyindex = 0;
			}
			SwitchAvatar(oldIndex, bodyindex, Animate: false);
		}
	}

	public void SetActiveAvatar(int woid)
	{
		if (animator.Bodies[bodyindex].Id != woid)
		{
			int oldIndex = bodyindex;
			bodyindex = animator.Bodies.IndexOf(animator.Bodies.Where((MVBody x) => x.Id == woid).FirstOrDefault());
			SwitchAvatar(oldIndex, bodyindex, Animate: false);
			AvatarSlotButtonView.UpdateSelectionCursor();
		}
	}

	public void AddNewAvatar(int WoID)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		MVBody mVBody = WOCM.GetWorldObjectClient(WoID) as MVBody;
		mVBody.ShadowVisible = false;
		animator.Bodies.Add(mVBody);
		animator.Bodies[animator.Bodies.Count - 1].WorldPosition = animator.hidePos;
		animator.Bodies[animator.Bodies.Count - 1].WorldRotation = animator.displayRotation;
		int oldIndex = bodyindex;
		bodyindex = animator.Bodies.Count - 1;
		SwitchAvatar(oldIndex, bodyindex, Animate: false);
		if (OnAvatarBodiesUpdated != null)
		{
			OnAvatarBodiesUpdated(purchased: false);
		}
	}

	public int MarkAndReturnCurrentAvatarID()
	{
		bodiesMarkedForDelete.Add(bodyindex);
		return animator.Bodies[bodyindex].Id;
	}

	public void SubstituteAvatar(int WoID)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		MVBody mVBody = (MVBody)WOCM.GetWorldObjectClient(WoID);
		mVBody.ShadowVisible = false;
		mVBody.WorldPosition = animator.hidePos;
		mVBody.WorldRotation = animator.displayRotation;
		MVGameController.Instance.Game.AvatarMetaDataWoMap.ResetAvatar(animator.Bodies[bodiesMarkedForDelete[0]].Id, WoID);
		animator.Bodies[bodiesMarkedForDelete[0]] = mVBody;
		EditorStateMachine.EnterGroup(mVBody);
		EditorStateMachine.Event = EditorEvent.CERoam;
		SwitchAvatar(bodyindex, bodiesMarkedForDelete[0], Animate: false);
		bodiesMarkedForDelete.RemoveAt(0);
		if (OnAvatarBodiesUpdated != null)
		{
			OnAvatarBodiesUpdated(purchased: false);
		}
	}

	private void SwitchAvatar(int oldIndex, int newIndex, bool Animate)
	{
		if (Animate)
		{
			animator.SetTargetIndex(oldIndex, newIndex);
		}
		else
		{
			animator.SetTargetIndexNoAnim(oldIndex, newIndex);
		}
		animator.Bodies[oldIndex].Visible = false;
		animator.Bodies[newIndex].Visible = true;
		EditorStateMachine.EnterGroup(animator.Bodies[newIndex]);
		EditorStateMachine.Event = EditorEvent.CERoam;
		UpdateSellButton(animator.Bodies[newIndex].Id);
		MVGameController.Instance.Game.SetActiveAvatar(animator.Bodies[newIndex].Id);
	}
}
