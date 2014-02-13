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

	private CharacterEditorController CEController => MVGameController.Instance.CharacterEditorController;

	private List<MVBody> AvatarBodies => AvatarSelectionAnimator.Instance.Bodies;

	public void InitializeAvatarSlotButtonView()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		SlotButtonRoot = new GameObject("SlotButtonRoot").transform;
		SlotButtonRoot.parent = ButtonOffset;
		SlotButtonRoot.localPosition = Vector3.zero;
		SlotButtonRoot.localScale = Vector3.one;
		SlotButtonRoot.localRotation = Quaternion.identity;
		selectionCursor.SetVisible(visible: true);
		CharacterEditorController characterEditorController = MVGameController.Instance.CharacterEditorController;
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
		int num = AvatarBodies.Count - SlotButtonRoot.GetChildCount();
		for (int i = 0; i < num; i++)
		{
			AddAvatarSlotButton(SlotButtonRoot.GetChildCount(), purchased);
		}
		_takePictureQueue.Clear();
		MVGUIAvatarSlotButton[] componentsInChildren = ((Component)SlotButtonRoot).GetComponentsInChildren<MVGUIAvatarSlotButton>(true);
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
		}
	}

	private void UpdateSlotVisibility()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected Obj, but got Unknown
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		foreach (Transform item in SlotButtonRoot)
		{
			Transform val = item;
			MVGUIAvatarSlotButton component = ((Component)val).gameObject.GetComponent<MVGUIAvatarSlotButton>();
			float num = ((Component)component).transform.localPosition.x + ((Component)SlotButtonRoot).transform.localPosition.x;
			component.SetVisible(num >= 0f && num <= (float)VisibleButtons * SlotButtonSpacing);
		}
	}

	public void AddAvatarSlotButton(int index, bool purchased)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		Object val = Object.Instantiate(Resources.Load("Prefabs/GUI/AvatarSlotButton"));
		MVGUIAvatarSlotButton component = ((GameObject)((val is GameObject) ? val : null)).GetComponent<MVGUIAvatarSlotButton>();
		((Component)component).transform.parent = SlotButtonRoot;
		((Component)component).transform.localPosition = GetNextAvatarSlotButtonPos(index);
		((Component)component).transform.localScale = Vector3.one;
		((Component)component).transform.localRotation = Quaternion.identity;
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
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		SlotButtonRoot.localPosition = Vector3.left * SlotButtonSpacing * (float)index;
		UpdateAfterScroll();
	}

	private void ScrollAvatarSlots(int direction)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		Transform slotButtonRoot = SlotButtonRoot;
		slotButtonRoot.localPosition += Vector3.right * SlotButtonSpacing * (float)direction;
		UpdateAfterScroll();
	}

	private void UpdateAfterScroll()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		UpdateSlotVisibility();
		UpdateSelectionCursor();
		moveSlotsLeftButton.SetVisible(SlotButtonRoot.localPosition.x < 0f);
		int num = (int)(-1f * SlotButtonRoot.localPosition.x / SlotButtonSpacing);
		moveSlotsRightButton.SetVisible(num + VisibleButtons < SlotButtonRoot.childCount - 1);
	}

	public void UpdateSelectionCursor()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 nextAvatarSlotButtonPos = GetNextAvatarSlotButtonPos(CEController.CurrentBodyIndex);
		float num = nextAvatarSlotButtonPos.x + ((Component)SlotButtonRoot).transform.localPosition.x;
		selectionCursor.SetVisible(num >= 0f && num <= (float)VisibleButtons * SlotButtonSpacing);
		((Component)selectionCursor).transform.localPosition = nextAvatarSlotButtonPos + ((Component)SlotButtonRoot).transform.localPosition;
	}

	private Vector3 GetNextAvatarSlotButtonPos(int index)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.right * SlotButtonSpacing * (float)index;
	}

	private void OnAvatarSlotPick(int index)
	{
		CEController.SetActiveAvatar(AvatarBodies[index].Id);
		UpdateSelectionCursor();
	}
}
