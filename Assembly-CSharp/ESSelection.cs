using System;
using System.Collections;
using System.Collections.Generic;
using Localize;
using MV.WorldObject;
using UnityEngine;

internal class ESSelection : ESStateBase
{
	private enum GizmoAction
	{
		None,
		Rotate,
		TranslateXZ,
		TranslateY,
		Open,
		ExitGroup
	}

	private class PickResult<T>
	{
		public readonly Vector3 mousePosition;

		public readonly VoxelHit hit;

		public readonly T data;

		public PickResult(Vector3 mousePosition, VoxelHit hit, T data)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			this.mousePosition = mousePosition;
			this.hit = hit;
			this.data = data;
		}
	}

	private MVGUISelectionGizmo translateGizmo;

	private MVGUIRightClickGizmo rightClickGizmo;

	private MVGUIEditModel exitButton;

	private GizmoAction gizmoAction;

	private EditorStateMachine editorStateMachine;

	private LinkObjectScript selectedLinkObject;

	private MVWorldObjectClient selectedWorldObject;

	private bool deselectAfterTranslate;

	private bool rightClickGizmoSelect;

	private PickResult<MVWorldObjectClient> pickedTarget;

	private PickResult<LinkObjectScript> pickedLink;

	private MVWorldObjectClientManager WOCM => MVGameController.Instance.WOCM;

	public ESSelection()
	{
		InitializeSelectionGizmo();
		InitializeRightClickMenuGizmo();
		exitButton = Object.FindObjectOfType(typeof(MVGUIEditModel)) as MVGUIEditModel;
	}

	private void InitializeSelectionGizmo()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		Object val = Object.Instantiate(Resources.Load("Prefabs/GUI/Gizmos/SelectionGizmo"));
		GameObject val2 = (GameObject)(object)((val is GameObject) ? val : null);
		val2.transform.localPosition = Vector3.zero;
		translateGizmo = val2.GetComponent<MVGUISelectionGizmo>();
		MVGUISelectionGizmo mVGUISelectionGizmo = translateGizmo;
		mVGUISelectionGizmo.OnRotate = (MVGUIGizmoBase.GizmoClickDelegate)Delegate.Combine(mVGUISelectionGizmo.OnRotate, (MVGUIGizmoBase.GizmoClickDelegate)(() =>
		{
			gizmoAction = GizmoAction.Rotate;
		}));
		MVGUISelectionGizmo mVGUISelectionGizmo2 = translateGizmo;
		mVGUISelectionGizmo2.OnXZtranslate = (MVGUIGizmoBase.GizmoClickDelegate)Delegate.Combine(mVGUISelectionGizmo2.OnXZtranslate, (MVGUIGizmoBase.GizmoClickDelegate)(() =>
		{
			gizmoAction = GizmoAction.TranslateXZ;
		}));
		MVGUISelectionGizmo mVGUISelectionGizmo3 = translateGizmo;
		mVGUISelectionGizmo3.OnYtranslate = (MVGUIGizmoBase.GizmoClickDelegate)Delegate.Combine(mVGUISelectionGizmo3.OnYtranslate, (MVGUIGizmoBase.GizmoClickDelegate)(() =>
		{
			gizmoAction = GizmoAction.TranslateY;
		}));
	}

	private void InitializeRightClickMenuGizmo()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		Object val = Object.Instantiate(Resources.Load("Prefabs/GUI/Gizmos/RightClickGizmo"));
		GameObject val2 = (GameObject)(object)((val is GameObject) ? val : null);
		val2.transform.localPosition = Vector3.zero;
		rightClickGizmo = val2.GetComponent<MVGUIRightClickGizmo>();
	}

	private void OnRightGlickGizmoMouseDown()
	{
		rightClickGizmoSelect = true;
	}

	private void ShowContextMenuGizmo()
	{
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		if (selectedWorldObject.HasInteractionFlag(InteractionFlags.HasSettings))
		{
			rightClickGizmo.AddButton(Localization.Instance.GetText(TextSlotIndex.Settings), () =>
			{
				MVGUIDialogBoxWrapper.Instance.ShowSettingsDialog(selectedWorldObject);
				rightClickGizmoSelect = false;
			});
		}
		if (selectedWorldObject.HasInteractionFlag(InteractionFlags.CanEdit) && !selectedWorldObject.HasInteractionFlag(InteractionFlags.IsPreview))
		{
			Debug.Log((object)"CanEdit");
			rightClickGizmo.AddButton(Localization.Instance.GetText(TextSlotIndex.EditModel), () =>
			{
				gizmoAction = GizmoAction.Open;
				rightClickGizmoSelect = false;
			});
		}
		if (selectedWorldObject.HasInteractionFlag(InteractionFlags.CanResetLogic))
		{
			rightClickGizmo.AddButton(Localization.Instance.GetText(TextSlotIndex.ResetLogic), () =>
			{
				MVGameController.Instance.Game.ResetLogicChunk(selectedWorldObject.Id);
				rightClickGizmoSelect = false;
			});
		}
		if (CanClone())
		{
			rightClickGizmo.AddButton(Localization.Instance.GetText(TextSlotIndex.Clone), () =>
			{
				deselectAfterTranslate = true;
				MVGameController.Instance.EditController.EditorWorldObjectCreation.CloneHierarchy(selectedWorldObject, cloneToRoot: false, setAsPreviewItem: false);
				rightClickGizmoSelect = false;
			});
		}
		if (selectedWorldObject.HasInteractionFlag(InteractionFlags.CanAddToInventory) && !selectedWorldObject.HasInteractionFlag(InteractionFlags.IsPreview))
		{
			rightClickGizmo.AddButton(Localization.Instance.GetText(TextSlotIndex.AddToInventory), () =>
			{
				MVGameController.Instance.EditController.AddToInventory(selectedWorldObject);
				rightClickGizmoSelect = false;
			});
		}
		if (selectedWorldObject.HasInteractionFlag(InteractionFlags.IsPreview) && selectedWorldObject.PreviewOwnerProfileId == MVGameController.Instance.Game.LocalPlayer.ProfileID)
		{
			rightClickGizmo.AddButton(Localization.Instance.GetText(TextSlotIndex.Purchase), () =>
			{
				MVItem shopItem = MVGameController.Instance.Game.ShopRepository.ShopInventory[selectedWorldObject.ItemId];
				UXDialogFactory uXDialogFactory = UXUtils.FindGUIObjectOfType<UXDialogFactory>();
				uXDialogFactory.CreateCustomDialog("Prefabs/GUI/Dialogs/BrightProductShopDialog", TextSlotIndex.Empty, noButtons: true).SetOnResultCallback(OnPurchaseDialogResult).SetValues(BuildDialogData(shopItem))
					.Show();
				MVGUIProductShopDialog mVGUIProductShopDialog = (MVGUIProductShopDialog)uXDialogFactory.CurrentDialogBox;
				mVGUIProductShopDialog.SetPrice(shopItem.priceGold, shopItem.priceSilver);
				mVGUIProductShopDialog.OnTryPurchaseProduct = () =>
				{
					MVGameController.Instance.Game.UnlockClientShopInventoryItem(shopItem.itemID);
				};
				rightClickGizmoSelect = false;
			});
		}
		rightClickGizmo.AddButton(Localization.Instance.GetText(TextSlotIndex.Delete), () =>
		{
			MVGameController.Instance.EditorController.Delete(editorStateMachine.SelectedWOs);
			rightClickGizmoSelect = false;
		});
		MVGUIRightClickGizmo mVGUIRightClickGizmo = rightClickGizmo;
		mVGUIRightClickGizmo.OnGizmoMouseDown = (MVGUIRightClickGizmo.OnGizmoMouseDownDelegate)Delegate.Combine(mVGUIRightClickGizmo.OnGizmoMouseDown, new MVGUIRightClickGizmo.OnGizmoMouseDownDelegate(OnRightGlickGizmoMouseDown));
		MVGUIRightClickGizmo mVGUIRightClickGizmo2 = rightClickGizmo;
		VoxelHit hit = pickedTarget.hit;
		mVGUIRightClickGizmo2.WorldPosition = hit.point;
		rightClickGizmo.Visible = true;
	}

	private bool CanClone()
	{
		foreach (MVWorldObjectClient selectedWO in editorStateMachine.SelectedWOs)
		{
			if (!selectedWO.HasInteractionFlag(InteractionFlags.CanClone) || selectedWO.HasInteractionFlag(InteractionFlags.IsPreview))
			{
				return false;
			}
			if (MVGameController.Instance.Game.ItemBusinessLogic.GetItem(selectedWO.ItemId) == null)
			{
				return false;
			}
		}
		return true;
	}

	private Dictionary<string, DialogData> BuildDialogData(MVItem item)
	{
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		dictionary.Add("ProductName", new TextData
		{
			text = item.name,
			useWordWrap = true
		});
		Object val = Object.Instantiate(Resources.Load("Prefabs/GUI/ShopPreview/ItemShopPreview"));
		MVGUIItemShopPreview component = ((GameObject)((val is GameObject) ? val : null)).GetComponent<MVGUIItemShopPreview>();
		component.BuildItemShopPreview(item, 12f, 12f);
		dictionary.Add("ProductPreview", new ProductPreviewData
		{
			productPreview = ((Component)component).gameObject
		});
		return dictionary;
	}

	private void OnPurchaseDialogResult(UXDialogBox dialogBox)
	{
		if (dialogBox.DialogResult == UXDialogResult.Positive)
		{
			Hashtable hashtable = (Hashtable)dialogBox.GetResult();
			if (hashtable.ContainsKey((byte)22))
			{
				int num = (int)hashtable[(byte)22];
				MVItem mVItem = MVGameController.Instance.Game.ShopRepository.ShopInventory[selectedWorldObject.ItemId];
				MVGameController.Instance.Game.PlayerRepository.PlayerInventory.Add(mVItem.itemID, mVItem);
				MVGameController.Instance.Game.PlayerRepository.itemIDToInventorySlotIndex.Add(mVItem.itemID, num);
				MVGameController.Instance.Game.PlayerRepository.NotifyRepositoryChange();
				MVGameController.Instance.Game.ShopRepository.RemoveItem(mVItem.itemID);
				MVGameController.Instance.Game.ShopRepository.ReorganizeItemsByItemType(notifyOfChange: true);
			}
			else
			{
				Debug.LogError((object)"Purchased product, but received no slot index to put it into");
			}
		}
	}

	private void ShowLinkMenuGizmo()
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		rightClickGizmo.AddButton(Localization.Instance.GetText(TextSlotIndex.Delete), () =>
		{
			if (selectedLinkObject.isObjectLink)
			{
				MVGameController.Instance.Game.RemoveObjectLink(selectedLinkObject.linkID);
			}
			else
			{
				MVGameController.Instance.Game.RemoveLink(selectedLinkObject.linkID);
			}
			rightClickGizmoSelect = false;
			editorStateMachine.DeSelectAll();
			selectedLinkObject = null;
			pickedLink = null;
		});
		MVGUIRightClickGizmo mVGUIRightClickGizmo = rightClickGizmo;
		mVGUIRightClickGizmo.OnGizmoMouseDown = (MVGUIRightClickGizmo.OnGizmoMouseDownDelegate)Delegate.Combine(mVGUIRightClickGizmo.OnGizmoMouseDown, new MVGUIRightClickGizmo.OnGizmoMouseDownDelegate(OnRightGlickGizmoMouseDown));
		MVGUIRightClickGizmo mVGUIRightClickGizmo2 = rightClickGizmo;
		VoxelHit hit = pickedLink.hit;
		mVGUIRightClickGizmo2.WorldPosition = hit.point;
		rightClickGizmo.Visible = true;
	}

	public override void Enter(EditorStateMachine e)
	{
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		bool flag = e.SingleSelectedWO != null && (e.SingleSelectedWO.InteractionFlags & InteractionFlags.DirectlySelectable) != 0;
		if (deselectAfterTranslate)
		{
			deselectAfterTranslate = false;
			if (e.ParentGroupIsRoot)
			{
				e.DeSelectAll();
				e.Event = EditorEvent.ESTerrainEdit;
				return;
			}
			if (flag)
			{
				SharedCubeFunctions.SetLayerRecursively(e.ParentGroup.GameObject.transform, select: false);
				e.ExitGroup();
				if (e.ParentGroupIsRoot)
				{
					e.Event = EditorEvent.ESTerrainEdit;
				}
			}
		}
		if (e.SelectedIDs.Count != 0 && e.SelectedIDs.Count > 1)
		{
			Debug.LogWarning((object)"Entering selection state with multiple items, lolwut?");
		}
		editorStateMachine = e;
		editorStateMachine.SelectionController.SelectedWorldObjectDeleted += SelectionController_SelectedWorldObjectDeletedHandler;
		if (!e.ParentGroupIsRoot)
		{
			exitButton.View.Show();
			exitButton.exitButton.OnClick = () =>
			{
				gizmoAction = GizmoAction.ExitGroup;
			};
			exitButton.exitText.OnClick = (UXMouseClickObject clickObject, Vector3 mousePositionWorld) =>
			{
				gizmoAction = GizmoAction.ExitGroup;
			};
			UXMouseClickObject exitText = exitButton.exitText;
			exitText.OnMouseDown = (UXMouseClickObject.OnMouseDownDelegate)Delegate.Combine(exitText.OnMouseDown, (UXMouseClickObject.OnMouseDownDelegate)((UXMouseClickObject clickObject, Vector3 mousePositionWorld) => true));
			if (!flag)
			{
				e.CameraController.SecondaryCameraActive = true;
				((Behaviour)((Component)e.CameraController).GetComponent<GrayscaleEffect>()).enabled = true;
			}
		}
		gizmoAction = GizmoAction.None;
		VoxelHit hit = default;
		if (MVGameController.Instance.WOCM.Pick(ref hit))
		{
			pickedTarget = new PickResult<MVWorldObjectClient>(Input.mousePosition, hit, MVGameController.Instance.WOCM.GetWorldObjectClient(hit.woId));
			selectedWorldObject = e.SingleSelectedWO;
		}
		LinkObjectScript linkHit = GetLinkHit(e, ref hit);
		pickedLink = ((!((Object)(object)linkHit != (Object)null)) ? null : new PickResult<LinkObjectScript>(Input.mousePosition, hit, linkHit));
	}

	public override void Execute(EditorStateMachine e)
	{
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_053c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_057f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		base.Execute(e);
		if ((Object)(object)UXUtils.FindGUIObjectOfType<UXDialogFactory>().CurrentDialogBox != (Object)null || MVGameController.Instance.EditController.WindowShown)
		{
			e.DeSelectAll();
			HideGizmos();
			if (e.ParentGroup == MVGameController.Instance.WOCM.RootGroup)
			{
				e.Event = EditorEvent.ESTerrainEdit;
			}
		}
		if (CheckAndHandleGizmoAction(e))
		{
			gizmoAction = GizmoAction.None;
		}
		else
		{
			if (rightClickGizmoSelect)
			{
				return;
			}
			if (MVInputWrapper.GetKeyUp((KeyCode)13))
			{
				if (e.SingleSelectedWO != null && EnterObject(e, e.SingleSelectedWO))
				{
					return;
				}
			}
			else if (MVInputWrapper.GetKeyDown((KeyCode)127))
			{
				MVGameController.Instance.EditorController.Delete(e.SelectedWOs);
				HideGizmos();
				return;
			}
			if (MVInputWrapper.GetKeyDown((KeyCode)27))
			{
				HandleEscape(e);
				return;
			}
			VoxelHit hit = default;
			bool flag = MVGameController.Instance.WOCM.Pick(ref hit);
			if (flag && (MVGameController.Instance.WOCM.IsType(hit.woId, WorldObjectType.CubeModelPrototypeTerrain) || hit.woId == -1))
			{
				flag = false;
			}
			TintObjectsOnMouseOver(e, flag, hit);
			if (pickedTarget != null)
			{
				MVWorldObjectClientManager wOCM = MVGameController.Instance.WOCM;
				VoxelHit hit2 = pickedTarget.hit;
				if (wOCM.GetWorldObjectClient(hit2.woId) == null)
				{
					pickedTarget = null;
				}
			}
			if (MVInputWrapper.GetKeyDown((KeyCode)323))
			{
				if (flag)
				{
					pickedTarget = new PickResult<MVWorldObjectClient>(Input.mousePosition, hit, MVGameController.Instance.WOCM.GetWorldObjectClient(hit.woId));
					HashSet<int> selectedIDs = e.SelectedIDs;
					VoxelHit hit3 = pickedTarget.hit;
					bool flag2 = selectedIDs.Contains(hit3.woId);
					if ((pickedTarget.data.InteractionFlags & InteractionFlags.NotUserTransformable) != InteractionFlags.NotUserTransformable)
					{
						bool addToSelection = flag2;
						selectedWorldObject = e.Select(pickedTarget.hit, addToSelection);
						Debug.Log((object)("SelectedWorldObject: " + selectedWorldObject));
					}
					HideGizmos();
				}
			}
			else if (MVInputWrapper.GetKey((KeyCode)323))
			{
				if (pickedTarget == null)
				{
					return;
				}
				Vector3 val = pickedTarget.mousePosition - Input.mousePosition;
				if (val.magnitude > 0.5f || MVInputWrapper.GetAxisRaw("Mouse ScrollWheel") != 0f)
				{
					if (e.SelectedIDs.Count == 1)
					{
						deselectAfterTranslate = true;
					}
					e.PushState(EditorEvent.ESTranslate);
				}
			}
			else if (MVInputWrapper.GetKeyUp((KeyCode)323))
			{
				if (selectedWorldObject != null && pickedTarget != null && IsMouseUpValid(pickedTarget.mousePosition))
				{
					bool flag3 = CheckAndExecuteOnClickHandler(e, pickedTarget);
					bool flag4 = !e.SelectedIDs.Contains(selectedWorldObject.Id);
					if (!flag3 && !flag4)
					{
						translateGizmo.rotate.buttonEnabled = selectedWorldObject.HasInteractionFlag(InteractionFlags.CanRotateX) || selectedWorldObject.HasInteractionFlag(InteractionFlags.CanRotateY) || selectedWorldObject.HasInteractionFlag(InteractionFlags.CanRotateZ);
						translateGizmo.xzTranslate.buttonEnabled = !selectedWorldObject.HasInteractionFlag(InteractionFlags.NotTranslatbleXZ);
						translateGizmo.yTranslateDown.buttonEnabled = (translateGizmo.yTranslateUp.buttonEnabled = !selectedWorldObject.HasInteractionFlag(InteractionFlags.NotTranslatbleY));
						MVGUISelectionGizmo mVGUISelectionGizmo = translateGizmo;
						VoxelHit hit4 = pickedTarget.hit;
						mVGUISelectionGizmo.WorldPosition = hit4.point;
						translateGizmo.Visible = true;
					}
				}
				else
				{
					e.DeSelectAll();
					HideGizmos();
					if (e.ParentGroup == MVGameController.Instance.WOCM.RootGroup)
					{
						e.Event = EditorEvent.ESTerrainEdit;
					}
				}
				pickedLink = null;
				pickedTarget = null;
			}
			else if (MVInputWrapper.GetKeyDown((KeyCode)324))
			{
				VoxelHit hit5 = default;
				LinkObjectScript linkHit = GetLinkHit(e, ref hit5);
				if ((Object)(object)linkHit != (Object)null)
				{
					pickedLink = new PickResult<LinkObjectScript>(Input.mousePosition, hit5, linkHit);
				}
				else if (flag)
				{
					MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(hit.woId);
					pickedTarget = new PickResult<MVWorldObjectClient>(Input.mousePosition, hit, worldObjectClient);
					if (!e.SelectedWOs.Contains(pickedTarget.data))
					{
						selectedWorldObject = e.Select(pickedTarget.hit, addToSelection: false);
						Debug.Log((object)("SelectedWorldObject: " + selectedWorldObject));
					}
				}
			}
			else
			{
				if (!MVInputWrapper.GetKeyUp((KeyCode)324))
				{
					return;
				}
				if (pickedLink != null && IsMouseUpValid(pickedLink.mousePosition))
				{
					HideGizmos();
					selectedLinkObject = pickedLink.data;
					ShowLinkMenuGizmo();
				}
				else if (pickedTarget != null && IsMouseUpValid(pickedTarget.mousePosition))
				{
					HideGizmos();
					if (!CheckAndExecuteOnClickHandler(e, pickedTarget))
					{
						if (e.SelectedIDs.Count == 1)
						{
							ShowContextMenuGizmo();
						}
						else
						{
							Debug.LogWarning((object)"There should be selected objects at this point - Martin");
						}
					}
				}
				else
				{
					e.DeSelectAll();
					HideGizmos();
					if (e.ParentGroupIsRoot)
					{
						e.Event = EditorEvent.ESTerrainEdit;
					}
				}
				pickedLink = null;
				pickedTarget = null;
			}
		}
	}

	public override void Exit(EditorStateMachine e)
	{
		if (e.ParentGroupIsRoot || (e.SingleSelectedWO != null && e.SingleSelectedWO.HasInteractionFlag(InteractionFlags.DirectlySelectable)))
		{
			e.CameraController.SecondaryCameraActive = false;
			((Behaviour)((Component)e.CameraController).GetComponent<GrayscaleEffect>()).enabled = false;
		}
		HideGizmos();
		exitButton.View.Hide();
		exitButton.exitButton.OnClick = null;
		editorStateMachine.SelectionController.SelectedWorldObjectDeleted -= SelectionController_SelectedWorldObjectDeletedHandler;
	}

	private bool IsMouseUpValid(Vector3 mousePosition)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = mousePosition - Input.mousePosition;
		return val.sqrMagnitude < 20f;
	}

	private bool CheckAndHandleGizmoAction(EditorStateMachine e)
	{
		switch (gizmoAction)
		{
		case GizmoAction.Open:
			EnterObject(e, e.SingleSelectedWO);
			return true;
		case GizmoAction.Rotate:
			if (e.SelectedIDs.Count == 1)
			{
				deselectAfterTranslate = true;
			}
			e.PushState(EditorEvent.Rotating);
			return true;
		case GizmoAction.TranslateY:
			if (e.SelectedIDs.Count == 1)
			{
				deselectAfterTranslate = true;
			}
			e.Data.Add("yOnlyTranslate", null);
			e.PushState(EditorEvent.ESTranslate);
			return true;
		case GizmoAction.TranslateXZ:
			if (e.SelectedIDs.Count == 1)
			{
				deselectAfterTranslate = true;
			}
			e.PushState(EditorEvent.ESTranslate);
			return true;
		case GizmoAction.ExitGroup:
			HandleEscape(e);
			return true;
		default:
			return false;
		}
	}

	private bool CheckAndExecuteOnClickHandler(EditorStateMachine e, PickResult<MVWorldObjectClient> pick)
	{
		MVWorldObjectClient data = pick.data;
		VoxelHit hit = pick.hit;
		return data.OnClickHandler(e, hit.collider);
	}

	private void SelectionController_SelectedWorldObjectDeletedHandler(object sender, WorldObjectDestroyedEventArgs e)
	{
		if (selectedWorldObject != null && selectedWorldObject.Id == e.WordObjectID)
		{
			HideGizmos();
			editorStateMachine.PopState();
		}
	}

	private LinkObjectScript GetLinkHit(EditorStateMachine e, ref VoxelHit hit)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		if (MVGameController.Instance.EditController.IsLogicRendered())
		{
			float num = float.PositiveInfinity;
			if (MVGameController.Instance.WOCM.Pick(ref hit))
			{
				num = hit.distance;
			}
			Ray val = ((Component)e.CameraController).camera.ScreenPointToRay(Input.mousePosition);
			int num2 = 1 << LayerMask.NameToLayer("Logic");
			RaycastHit val2 = default;
			Physics.Raycast(val, ref val2, float.PositiveInfinity, num2);
			if ((Object)(object)val2.collider != (Object)null && val2.distance < num)
			{
				hit.point = val2.point;
				return ((Component)val2.collider).gameObject.GetComponentInChildren<LinkObjectScript>();
			}
		}
		return null;
	}

	private bool EnterObject(EditorStateMachine e, MVWorldObjectClient selectedWo)
	{
		Debug.Log((object)"EnterObject");
		return selectedWo.OnEnterObject(e);
	}

	private void HandleEscape(EditorStateMachine e)
	{
		if (e.ParentGroupIsRoot)
		{
			e.CameraController.SecondaryCameraActive = false;
			e.DeSelectAll();
			e.Event = EditorEvent.ESTerrainEdit;
			return;
		}
		SharedCubeFunctions.SetLayerRecursively(e.ParentGroup.Transform, select: false);
		e.ExitGroup();
		if (e.ParentGroupIsRoot)
		{
			((Behaviour)((Component)e.CameraController).GetComponent<GrayscaleEffect>()).enabled = false;
			e.CameraController.SecondaryCameraActive = false;
			e.DeSelectAll();
			e.Event = EditorEvent.ESTerrainEdit;
		}
		else
		{
			EnterObject(e, e.ParentGroup);
		}
	}

	private void HideGizmos()
	{
		translateGizmo.Visible = false;
		rightClickGizmo.Visible = false;
		MVGUIRightClickGizmo mVGUIRightClickGizmo = rightClickGizmo;
		mVGUIRightClickGizmo.OnGizmoMouseDown = (MVGUIRightClickGizmo.OnGizmoMouseDownDelegate)Delegate.Remove(mVGUIRightClickGizmo.OnGizmoMouseDown, new MVGUIRightClickGizmo.OnGizmoMouseDownDelegate(OnRightGlickGizmoMouseDown));
	}
}
