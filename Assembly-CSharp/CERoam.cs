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

	private JetPackMode jetPackMode;

	private AccessoryMover accessoryMover;

	private bool didExit;

	private MVWorldObjectClientManager WOCM => MVGameController.Instance.WOCM;

	private AEditController EditController => MVGameController.Instance.EditController;

	private CharacterEditorController CharacterEditorController => MVGameController.Instance.CharacterEditorController;

	public CERoam()
	{
		if (MVGameController.Instance.GameMode == MVGameMode.CharacterEditor)
		{
			accessoryMover = new AccessoryMover();
		}
	}

	public override void Enter(EditorStateMachine esm)
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		didExit = false;
		exitButtonWasPressed = false;
		EditController.HideEditorTools();
		esm.CubeModelingStateMachine.RemoveCursors();
		tintedWo = null;
		SharedCubeFunctions.SetLayerRecursively(esm.ParentGroup.Transform, select: true);
		((Behaviour)((Component)esm.CameraController).GetComponent<GrayscaleEffect>()).enabled = true;
		esm.CameraController.SecondaryCameraActive = true;
		if (esm.ParentGroup is MVBody)
		{
			if ((Object)(object)guiEditModel != (Object)null)
			{
				guiEditModel.View.Hide();
			}
			MVBody mVBody = esm.ParentGroup as MVBody;
			Vector3 centerPos = CharacterEditorController.CenterPos;
			float radius = 10f;
			if (jetPackMode == null)
			{
				jetPackMode = WOCM.AvatarLocal.AvatarModes.JetPackMode;
			}
			jetPackMode.SetMoveConstraint(centerPos, radius);
			jetPackMode.YMovementSpeedScale = 0.25f;
			jetPackMode.XZMovementSpeedScale = 0.8f;
			CharacterEditorController.ShowAnimationToggles();
			CharacterEditorController.ShowAvatarTools();
			CharacterEditorController.AnimationToggles.AttachAnimation(mVBody.Animation);
			CharacterEditorController.AnimationToggles.ToggleAnimation("Idle");
			MVGameController.Instance.Game.CameraController.SetCamera(CameraType.JetPackCamera);
			MVGameController.Instance.Game.CameraController.CurCamera.FocusOnObject(esm.ParentGroup);
		}
		else
		{
			if ((Object)(object)guiEditModel == (Object)null)
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
		jetPackMode.YMovementSpeedScale = 1f;
		jetPackMode.XZMovementSpeedScale = 1f;
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
		if (WOCM.Pick(ref hit, new HashSet<int>(), layerMask) && hit.woId != -1)
		{
			if (MVInputWrapper.GetKeyUp((KeyCode)323) && downWorldObjectID == hit.woId)
			{
				downWorldObjectID = -1;
				return esm.Select(addToSelection: false, layerMask) != null;
			}
			if (MVInputWrapper.GetKeyDown((KeyCode)323))
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
		((Behaviour)((Component)esm.CameraController).GetComponent<GrayscaleEffect>()).enabled = false;
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
				((Behaviour)((Component)esm.CameraController).GetComponent<GrayscaleEffect>()).enabled = true;
				esm.CameraController.SecondaryCameraActive = true;
				esm.Event = EditorEvent.CERoam;
				return true;
			}
		}
		return false;
	}
}
