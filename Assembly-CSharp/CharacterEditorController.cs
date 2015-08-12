using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterEditorController : AEditController
{
	public delegate void OnAvatarBodiesUpdatedDelegate(bool purchased);

	private MVGUIResetAvatar resetAvatarButton;

	private MVGUIFullscreenToggle fullscreenToggle;

	private MVGUIMuteToggle muteToggle;

	private AvatarSelectionAnimator animator;

	private List<int> bodiesMarkedForDelete = new List<int>();

	private int bodyindex;

	private AvatarAccessoryController avatarAccessoryController;

	public OnAvatarBodiesUpdatedDelegate OnAvatarBodiesUpdated;

	private MVWorldObjectClientManager WOCM => MVGameController.WOCM;

	public MVGUIAnimationToggles AnimationToggles { get; private set; }

	public MVGUIAvatarSellcs AvatarSell { get; private set; }

	public MVGUICharacterEditScreenShot CharacterEditScreenshot { get; private set; }

	public MVGUIAvatarShopWindow AvatarShop { get; private set; }

	public MVGUIAvatarSlotButtonView AvatarSlotButtonView { get; private set; }

	public int BodyCount => animator.Bodies.Count;

	public List<MVBody> Bodies => animator.Bodies;

	public int CurrentBodyIndex => bodyindex;

	public MVBody CurrentBody
	{
		get
		{
			if (animator == null)
			{
				return null;
			}
			return animator.Bodies[CurrentBodyIndex];
		}
	}

	public Vector3 CenterPos => animator.displayPos;

	public override void Initialize()
	{
		base.Initialize();
		animator = AvatarSelectionAnimator.Instance;
		MVGameController.Game.CameraController.RenderLogic(renderLogic: false);
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
		MVAvatarLocal avatarLocal = MVGameController.WOCM.AvatarLocal;
		avatarLocal.WorldPosition = mVSpawnPointBlue.WorldPosition - Vector3.up;
		avatarLocal.WorldRotation = mVSpawnPointBlue.WorldRotation;
		Debug.Log("BodyWOID " + mVBody2.Id);
		UpdateSellButton(mVBody2.Id);
		MVGameController.WOCM.AvatarLocal.AvatarMode = MVGameController.WOCM.AvatarLocal.AvatarModes.JetPackMode;
		AvatarSlotButtonView.InitializeAvatarSlotButtonView();
		EditorStateMachine.EnterGroup(mVBody2);
		EditorStateMachine.Event = EditorEvent.CERoam;
	}

	public override void Update()
	{
		base.Update();
		cubeModelingController.Update();
	}

	public override void HandleInput()
	{
		base.HandleInput();
		cubeModelingController.HandleInput();
	}

	private void UpdateSellButton(int woBodyId)
	{
		if (!MVGameController.Game.AvatarMetaDataWoMap.TryGetValue(woBodyId, out var avatarMetaData))
		{
			Debug.LogError("Could not find woID");
			return;
		}
		Debug.Log(avatarMetaData);
		AvatarSell.UpdateAvatarMetaData(woBodyId, avatarMetaData);
	}

	protected override void ResolveGUIElements()
	{
		base.ResolveGUIElements();
		MVGUIAvatarEditor mVGUIAvatarEditor = UXUtils.FindGUIObjectOfType<MVGUIAvatarEditor>();
		GameObject gameObject = mVGUIAvatarEditor.gameObject;
		cubeModelingController = new CubeModelingController(this, EditorStateMachine.CubeModelingStateMachine);
		cubeModelingController.ResolveCubeTools(gameObject);
		cubeModelingController.CubeTools.paintCubeButton.Toggle();
		cubeModelingController.HideEditorTools();
		cubeModelingController.HideCurrentSelectedMaterial();
		AnimationToggles = AIngameController.FindGUIObjectOfType<MVGUIAnimationToggles>(gameObject);
		AvatarSell = AIngameController.FindGUIObjectOfType<MVGUIAvatarSellcs>(gameObject);
		CharacterEditScreenshot = AIngameController.FindGUIObjectOfType<MVGUICharacterEditScreenShot>(gameObject);
		AvatarShop = AIngameController.FindGUIObjectOfType<MVGUIAvatarShopWindow>(gameObject);
		AvatarSlotButtonView = AIngameController.FindGUIObjectOfType<MVGUIAvatarSlotButtonView>(gameObject);
		avatarAccessoryController = AIngameController.FindGUIObjectOfType<AvatarAccessoryController>(gameObject);
		fullscreenToggle = AIngameController.FindGUIObjectOfType<MVGUIFullscreenToggle>(gameObject);
		muteToggle = AIngameController.FindGUIObjectOfType<MVGUIMuteToggle>(gameObject);
		resetAvatarButton = AIngameController.FindGUIObjectOfType<MVGUIResetAvatar>(gameObject);
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
		resetAvatarButton.View.Show();
		CharacterEditScreenshot.View.Show();
		AvatarSlotButtonView.View.Show();
		avatarAccessoryController.AvatarAccessoryButtons.View.Show();
		fullscreenToggle.View.Show();
		muteToggle.View.Show();
	}

	public void HideAvatarTools()
	{
		AvatarSell.View.Hide();
		CharacterEditScreenshot.View.Hide();
		AvatarSlotButtonView.View.Hide();
		avatarAccessoryController.AvatarAccessoryButtons.View.Hide();
		resetAvatarButton.View.Hide();
		AvatarShop.View.Hide();
		fullscreenToggle.View.Hide();
		muteToggle.View.Hide();
	}

	public void ShowAvatarShopWindow()
	{
		ShowSingleWindow(AvatarShop.View);
	}

	public void ShowEditorTools()
	{
		cubeModelingController.ShowEditorTools();
		cubeModelingController.ShowCurrentSelectedMaterial();
	}

	public void HideEditorTools()
	{
		cubeModelingController.HideEditorTools();
		cubeModelingController.HideCurrentSelectedMaterial();
	}

	public void ToggleDrawPlane()
	{
		if (EditorStateMachine.CurEvent == EditorEvent.CEEditBody)
		{
			cubeModelingController.ToggleDrawPlane();
		}
	}

	public bool IsAvatarShopShown()
	{
		return AvatarShop.View.isVisible;
	}

	public MVGUIMaterialSelectionWindow ShowMaterialChangeWindow()
	{
		if (EditorStateMachine.CurEvent == EditorEvent.CERoam || EditorStateMachine.CurEvent == EditorEvent.CEAvatarAccessory)
		{
			return null;
		}
		return cubeModelingController.ShowMaterialChangeWindow();
	}

	public void ShiftAvatarIndex(bool forward)
	{
		Debug.Log("Shift avatar " + ((!forward) ? "back" : "forward"));
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
		MVBody mVBody = (MVBody)WOCM.GetWorldObjectClient(WoID);
		mVBody.ShadowVisible = false;
		mVBody.WorldPosition = animator.hidePos;
		mVBody.WorldRotation = animator.displayRotation;
		MVGameController.Game.AvatarMetaDataWoMap.ResetAvatar(animator.Bodies[bodiesMarkedForDelete[0]].Id, WoID);
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
		MVGameController.Game.SetActiveAvatar(animator.Bodies[newIndex].Id);
	}
}
