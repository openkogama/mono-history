using System;
using System.Collections.Generic;
using UnityEngine;

public class MVGUIAvatarSlotButtonView : UXViewScript
{
	public int VisibleButtons;

	public float SlotButtonSpacing = 5f;

	public Transform ButtonOffset;

	private Transform SlotButtonRoot;

	public UXIconButton addAvatarButton;

	public UXIconButton moveSlotsRightButton;

	public UXIconButton moveSlotsLeftButton;

	public UXPlane selectionCursor;

	private Queue<MVGUIAvatarSlotButton> _takePictureQueue = new Queue<MVGUIAvatarSlotButton>();

	private CharacterEditorController CEController => MVGameControllerLegacyUI.CharacterEditorController;

	private List<MVBody> AvatarBodies => AvatarSelectionAnimator.Instance.Bodies;

	public void InitializeAvatarSlotButtonView()
	{
		SlotButtonRoot = new GameObject("SlotButtonRoot").transform;
		SlotButtonRoot.parent = ButtonOffset;
		SlotButtonRoot.localPosition = Vector3.zero;
		SlotButtonRoot.localScale = Vector3.one;
		SlotButtonRoot.localRotation = Quaternion.identity;
		selectionCursor.SetVisible(visible: true);
		CharacterEditorController characterEditorController = MVGameControllerLegacyUI.CharacterEditorController;
		characterEditorController.OnAvatarBodiesUpdated = (CharacterEditorController.OnAvatarBodiesUpdatedDelegate)Delegate.Combine(characterEditorController.OnAvatarBodiesUpdated, new CharacterEditorController.OnAvatarBodiesUpdatedDelegate(UpdateAvatarSlotButtons));
		UpdateAvatarSlotButtons();
		UXIconButton uXIconButton = addAvatarButton;
		uXIconButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXIconButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			CEController.ShowAvatarShopWindow();
		}));
		UXIconButton uXIconButton2 = moveSlotsLeftButton;
		uXIconButton2.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXIconButton2.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			ScrollAvatarSlots(1);
		}));
		UXIconButton uXIconButton3 = moveSlotsRightButton;
		uXIconButton3.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXIconButton3.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			ScrollAvatarSlots(-1);
		}));
		ScrollAvatarSlots(0);
	}

	public void UpdateAvatarSlotButtons(bool purchased = false)
	{
		int num = AvatarBodies.Count - SlotButtonRoot.childCount;
		for (int i = 0; i < num; i++)
		{
			AddAvatarSlotButton(SlotButtonRoot.childCount, purchased);
		}
		_takePictureQueue.Clear();
		MVGUIAvatarSlotButton[] componentsInChildren = SlotButtonRoot.GetComponentsInChildren<MVGUIAvatarSlotButton>(includeInactive: true);
		foreach (MVGUIAvatarSlotButton item in componentsInChildren)
		{
			_takePictureQueue.Enqueue(item);
		}
		if (_takePictureQueue.Count > 0)
		{
			TakeNextPicture();
		}
		UpdateAfterScroll();
	}

	private void TakeNextPicture()
	{
		if (_takePictureQueue.Count > 0)
		{
			MVGUIAvatarSlotButton mVGUIAvatarSlotButton = _takePictureQueue.Dequeue();
			mVGUIAvatarSlotButton.OnFinishedUpdating = TakeNextPicture;
			mVGUIAvatarSlotButton.UpdateAvatarPicture();
			UpdateSlotVisibility();
		}
	}

	private void UpdateSlotVisibility()
	{
		foreach (Transform item in SlotButtonRoot)
		{
			MVGUIAvatarSlotButton component = item.gameObject.GetComponent<MVGUIAvatarSlotButton>();
			float num = component.transform.localPosition.x + SlotButtonRoot.transform.localPosition.x;
			component.SetVisible(num >= 0f && num <= (float)VisibleButtons * SlotButtonSpacing);
		}
	}

	public void AddAvatarSlotButton(int index, bool purchased)
	{
		MVGUIAvatarSlotButton component = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/AvatarSlotButton")) as GameObject).GetComponent<MVGUIAvatarSlotButton>();
		component.transform.parent = SlotButtonRoot;
		component.transform.localPosition = GetNextAvatarSlotButtonPos(index);
		component.transform.localScale = Vector3.one;
		component.transform.localRotation = Quaternion.identity;
		component.BuildAvatarSlotButton(index, purchased);
		component.OnAvatarSlotPick = (MVGUIAvatarSlotButton.OnAvatarSlotPickDelegate)Delegate.Combine(component.OnAvatarSlotPick, new MVGUIAvatarSlotButton.OnAvatarSlotPickDelegate(OnAvatarSlotPick));
	}

	public void FocusOnSlotIndex(int index)
	{
		int num = 0;
		if (index > VisibleButtons - 1)
		{
			num = index - VisibleButtons / 2;
			while (num + VisibleButtons > AvatarBodies.Count)
			{
				num--;
			}
		}
		ScrollAvatarSlotsTo(num);
	}

	private void ScrollAvatarSlotsTo(int index)
	{
		SlotButtonRoot.localPosition = Vector3.left * SlotButtonSpacing * index;
		UpdateAfterScroll();
	}

	private void ScrollAvatarSlots(int direction)
	{
		SlotButtonRoot.localPosition += Vector3.right * SlotButtonSpacing * direction;
		UpdateAfterScroll();
	}

	private void UpdateAfterScroll()
	{
		UpdateSlotVisibility();
		UpdateSelectionCursor();
		moveSlotsLeftButton.SetVisible(SlotButtonRoot.localPosition.x < 0f);
		int num = (int)(-1f * SlotButtonRoot.localPosition.x / SlotButtonSpacing);
		moveSlotsRightButton.SetVisible(num + VisibleButtons < SlotButtonRoot.childCount - 1);
	}

	public void UpdateSelectionCursor()
	{
		Vector3 nextAvatarSlotButtonPos = GetNextAvatarSlotButtonPos(CEController.CurrentBodyIndex);
		float num = nextAvatarSlotButtonPos.x + SlotButtonRoot.transform.localPosition.x;
		selectionCursor.SetVisible(num >= 0f && num <= (float)VisibleButtons * SlotButtonSpacing);
		selectionCursor.transform.localPosition = nextAvatarSlotButtonPos + SlotButtonRoot.transform.localPosition;
	}

	private Vector3 GetNextAvatarSlotButtonPos(int index)
	{
		return Vector3.right * SlotButtonSpacing * index;
	}

	private void OnAvatarSlotPick(int index)
	{
		CEController.SetActiveAvatar(AvatarBodies[index].Id);
		UpdateSelectionCursor();
	}
}
