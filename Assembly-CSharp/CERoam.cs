using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class CERoam : ESStateBase
{
	private MVGUIEditModel guiEditModel;

	private bool exitButtonWasPressed;

	private int downWorldObjectID = -1;

	private bool enterEditNextFrame;

	private AccessoryMover accessoryMover;

	private bool didExit;

	private MVWorldObjectClientManager WOCM => MVGameControllerBase.WOCM;

	private CharacterEditorController CharacterEditorController => MVGameControllerLegacyUI.CharacterEditorController;

	public CERoam()
	{
		if (MVGameControllerBase.GameMode == MVGameMode.CharacterEditor)
		{
			accessoryMover = new AccessoryMover();
		}
	}

	public override void Enter(EditorStateMachine esm)
	{
		didExit = false;
		exitButtonWasPressed = false;
		CharacterEditorController.HideEditorTools();
		esm.CubeModelingStateMachine.RemoveCursors();
		tintedWo = null;
		SharedCubeFunctions.SetLayerRecursively(esm.ParentGroup.Transform, select: true);
		esm.CameraController.BlueModeEnabled = true;
		if (esm.ParentGroup is MVBody)
		{
			if (guiEditModel != null)
			{
				guiEditModel.View.Hide();
			}
			MVBody mVBody = esm.ParentGroup as MVBody;
			MVAvatarLocal.JetPackMode jetPackMode = (MVAvatarLocal.JetPackMode)MVGameControllerBase.WOCM.AvatarLocal.CurrentMode;
			jetPackMode.SetMoveConstraint(CharacterEditorController.CenterPos, 10f);
			jetPackMode.ModifySpeed(0.8f, 0.25f);
			CharacterEditorController.ShowAnimationToggles();
			CharacterEditorController.ShowAvatarTools();
			CharacterEditorController.AnimationToggles.AttachAnimation(mVBody.Animation);
			CharacterEditorController.AnimationToggles.ToggleAnimation("Idle");
			MVGameControllerBase.CameraController.SetCamera(CameraType.EditorCamera);
			MVGameControllerBase.CameraController.CurCamera.FocusOnObject(esm.ParentGroup);
		}
		else
		{
			if (guiEditModel == null)
			{
				guiEditModel = UXUtils.FindGUIObjectOfType<MVGUIEditModel>();
			}
			guiEditModel.View.Show();
			guiEditModel.exitButton.OnClick = () =>
			{
				exitButtonWasPressed = true;
			};
			guiEditModel.exitText.OnClick = (UXMouseClickObject clickObject, Vector3 mousePositionWorld) =>
			{
				exitButtonWasPressed = true;
			};
			UXMouseClickObject exitText = guiEditModel.exitText;
			exitText.OnMouseDown = (UXMouseClickObject.OnMouseDownDelegate)Delegate.Combine(exitText.OnMouseDown, (UXMouseClickObject.OnMouseDownDelegate)((UXMouseClickObject clickObject, Vector3 mousePositionWorld) => true));
		}
		WOCM.AvatarLocal.LaserPointer.ChangeState(LaserPointerState.Idle);
	}

	public override void Execute(EditorStateMachine esm)
	{
		if (enterEditNextFrame)
		{
			enterEditNextFrame = false;
			esm.Event = EditorEvent.CEEditBody;
		}
		TintObjectsOnMouseOver(esm);
		if (exitButtonWasPressed)
		{
			HandleEscape(esm);
			exitButtonWasPressed = false;
		}
		else if (HandleSelect(esm))
		{
			EnterObject(esm);
		}
	}

	public override void Exit(EditorStateMachine esm)
	{
		DeTintCurrent();
		CharacterEditorController.AnimationToggles.DetachAnimation();
		((MVAvatarLocal.JetPackMode)MVGameControllerBase.WOCM.AvatarLocal.CurrentMode).ModifySpeed(1f, 1f);
		didExit = true;
		accessoryMover.MVGUIAvatarAccessoryMoveIcon.SetVisible(visible: false);
	}

	private bool HandleSelect(EditorStateMachine esm)
	{
		if (didExit)
		{
			return false;
		}
		if (accessoryMover.MoveAccessory())
		{
			return false;
		}
		VoxelHit hit = default;
		int layerMask = -5 & ~(1 << LayerMask.NameToLayer("Hidden"));
		if (MVGameControllerLegacyUI.Pick(ref hit, new HashSet<int>(), layerMask) && hit.woId != -1)
		{
			if (MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelect) && downWorldObjectID == hit.woId)
			{
				downWorldObjectID = -1;
				return esm.Select(addToSelection: false, layerMask) != null;
			}
			if (MVInputWrapper.GetBooleanControlDown(KogamaControls.PointerSelect))
			{
				downWorldObjectID = hit.woId;
			}
		}
		return false;
	}

	private void HandleEscape(EditorStateMachine esm)
	{
		esm.DeSelectAll();
		esm.ExitGroup();
		esm.CameraController.GetComponent<GrayscaleEffect>().enabled = false;
		SharedCubeFunctions.SetLayerRecursively(esm.ParentGroup.Transform, select: false);
		esm.Event = EditorEvent.CERoam;
	}

	private bool EnterObject(EditorStateMachine esm)
	{
		if (esm.SingleSelectedWO != null)
		{
			if (esm.SingleSelectedWO is MVCubeModelInstance && esm.SingleSelectedWO.GroupId == esm.ParentGroupID)
			{
				CharacterEditorController.AnimationToggles.ToggleAnimation("TPose");
				enterEditNextFrame = true;
				return true;
			}
			if (esm.SingleSelectedWO is MVGroup)
			{
				MVGroup mVGroup = (MVGroup)esm.SingleSelectedWO;
				esm.EnterGroup(mVGroup);
				SharedCubeFunctions.SetLayerRecursively(mVGroup.Transform, select: true);
				esm.CameraController.BlueModeEnabled = true;
				esm.Event = EditorEvent.CERoam;
				return true;
			}
		}
		return false;
	}
}
