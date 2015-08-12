using System;
using System.Collections.Generic;
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

	private MVWorldObjectClientManager WOCM => MVGameController.WOCM;

	public ESSelection()
	{
		InitializeSelectionGizmo();
		InitializeRightClickMenuGizmo();
		exitButton = UXUtils.FindGUIObjectOfType<MVGUIEditModel>();
	}

	private void InitializeSelectionGizmo()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/Gizmos/SelectionGizmo")) as GameObject;
		gameObject.transform.localPosition = Vector3.zero;
		translateGizmo = gameObject.GetComponent<MVGUISelectionGizmo>();
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
		GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/Gizmos/RightClickGizmo")) as GameObject;
		gameObject.transform.localPosition = Vector3.zero;
		rightClickGizmo = gameObject.GetComponent<MVGUIRightClickGizmo>();
	}

	private void OnRightGlickGizmoMouseDown()
	{
		rightClickGizmoSelect = true;
	}

	private void ShowContextMenuGizmo()
	{
		if (selectedWorldObject.HasInteractionFlag(InteractionFlags.CanUseGameCoins))
		{
			rightClickGizmo.AddButton(TM._("Game Coins"), () =>
			{
				MVGUIDialogBoxWrapper.Instance.ShowGameCoinsDialog(selectedWorldObject);
				rightClickGizmoSelect = false;
			});
		}
		if (selectedWorldObject.HasInteractionFlag(InteractionFlags.HasSettings))
		{
			rightClickGizmo.AddButton(TM._("Settings"), () =>
			{
				MVGUIDialogBoxWrapper.Instance.ShowSettingsDialog(selectedWorldObject);
				rightClickGizmoSelect = false;
			});
		}
		if (selectedWorldObject.HasInteractionFlag(InteractionFlags.CanEdit) && !selectedWorldObject.HasInteractionFlag(InteractionFlags.IsPreview))
		{
			Debug.Log("CanEdit");
			rightClickGizmo.AddButton(TM._("Edit Model"), () =>
			{
				gizmoAction = GizmoAction.Open;
				rightClickGizmoSelect = false;
			});
		}
		if (selectedWorldObject.HasInteractionFlag(InteractionFlags.CanResetLogic))
		{
			rightClickGizmo.AddButton(TM._("Reset Logic"), () =>
			{
				MVGameController.Game.ResetLogicChunk(selectedWorldObject.Id);
				rightClickGizmoSelect = false;
			});
		}
		if (CanClone())
		{
			rightClickGizmo.AddButton(TM._("Clone"), () =>
			{
				deselectAfterTranslate = true;
				MVGameController.EditorController.EditorWorldObjectCreation.CloneHierarchy(selectedWorldObject, cloneToRoot: false, setAsPreviewItem: false);
				rightClickGizmoSelect = false;
			});
		}
		if (selectedWorldObject.HasInteractionFlag(InteractionFlags.CanAddToInventory) && !selectedWorldObject.HasInteractionFlag(InteractionFlags.IsPreview))
		{
			rightClickGizmo.AddButton(TM._("Add To Inventory"), () =>
			{
				MVGameController.EditorController.AddToInventory(selectedWorldObject);
				rightClickGizmoSelect = false;
			});
		}
		if (selectedWorldObject.HasInteractionFlag(InteractionFlags.IsPreview) && selectedWorldObject.PreviewOwnerProfileId == MVGameController.Game.LocalPlayer.ProfileID)
		{
			rightClickGizmo.AddButton(TM._("Purchase"), () =>
			{
				MVItem shopItem = MVGameController.Game.ShopRepository.ShopInventory[selectedWorldObject.ItemId];
				UXDialogFactory uXDialogFactory = UXUtils.UXDialogFactory;
				uXDialogFactory.CreateCustomDialog("Prefabs/GUI/Dialogs/BrightProductShopDialog", string.Empty, noButtons: true).SetOnResultCallback(OnPurchaseDialogResult).SetValues(BuildDialogData(shopItem))
					.Show();
				MVGUIProductShopDialog mVGUIProductShopDialog = (MVGUIProductShopDialog)uXDialogFactory.CurrentDialogBox;
				mVGUIProductShopDialog.SetPrice(shopItem.priceGold, shopItem.priceSilver);
				mVGUIProductShopDialog.OnTryPurchaseProduct = () =>
				{
					MVGameController.Game.UnlockClientShopInventoryItem(shopItem.itemID);
				};
				rightClickGizmoSelect = false;
			});
		}
		rightClickGizmo.AddButton(TM._("Delete"), () =>
		{
			MVGameController.EditorController.Delete(editorStateMachine.SelectedWOs);
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
		MVGUIItemShopPreview component = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/ShopPreview/ItemShopPreview")) as GameObject).GetComponent<MVGUIItemShopPreview>();
		component.BuildItemShopPreview(item, 12f, 12f);
		dictionary.Add("ProductPreview", new ProductPreviewData
		{
			productPreview = component.gameObject
		});
		return dictionary;
	}

	private void OnPurchaseDialogResult(UXDialogBox dialogBox)
	{
		if (dialogBox.DialogResult == UXDialogResult.Positive)
		{
			Dictionary<object, object> dictionary = (Dictionary<object, object>)dialogBox.GetResult();
			if (dictionary.ContainsKey((byte)22))
			{
				int num = (int)dictionary[(byte)22];
				MVItem mVItem = MVGameController.Game.ShopRepository.ShopInventory[selectedWorldObject.ItemId];
				MVGameController.Game.PlayerRepository.PlayerInventory.Add(mVItem.itemID, mVItem);
				MVGameController.Game.PlayerRepository.itemIDToInventorySlotIndex.Add(mVItem.itemID, num);
				MVGameController.Game.PlayerRepository.NotifyRepositoryChange();
				MVGameController.Game.ShopRepository.RemoveItem(mVItem.itemID);
				MVGameController.Game.ShopRepository.ReorganizeItemsByItemType(notifyOfChange: true);
			}
			else
			{
				Debug.LogError("Purchased product, but received no slot index to put it into");
			}
		}
	}

	private void ShowLinkMenuGizmo()
	{
		rightClickGizmo.AddButton(TM._("Delete"), () =>
		{
			if (selectedLinkObject.isObjectLink)
			{
				MVGameController.Game.RemoveObjectLink(selectedLinkObject.linkID);
			}
			else
			{
				MVGameController.Game.RemoveLink(selectedLinkObject.linkID);
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
			Debug.LogWarning("Entering selection state with multiple items, lolwut?");
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
				e.CameraController.GetComponent<GrayscaleEffect>().enabled = true;
			}
		}
		gizmoAction = GizmoAction.None;
		VoxelHit hit = default;
		if (MVGameController.WOCM.Pick(ref hit))
		{
			pickedTarget = new PickResult<MVWorldObjectClient>(MVInputWrapper.GetPointerPosition(), hit, MVGameController.WOCM.GetWorldObjectClient(hit.woId));
			selectedWorldObject = e.SingleSelectedWO;
		}
		LinkObjectScript linkHit = GetLinkHit(e, ref hit);
		pickedLink = ((!(linkHit != null)) ? null : new PickResult<LinkObjectScript>(MVInputWrapper.GetPointerPosition(), hit, linkHit));
	}

	public override void Execute(EditorStateMachine e)
	{
		base.Execute(e);
		if (UXUtils.UXDialogFactory.CurrentDialogBox != null || MVGameController.EditController.WindowShown)
		{
			e.DeSelectAll();
			HideGizmos();
			if (e.ParentGroup == MVGameController.WOCM.RootGroup)
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
			if (MVInputWrapper.GetBooleanControlUp(KogamaControls.EnterObject))
			{
				if (e.SingleSelectedWO != null && EnterObject(e, e.SingleSelectedWO))
				{
					return;
				}
			}
			else if (MVInputWrapper.GetBooleanControlDown(KogamaControls.DeleteObject))
			{
				MVGameController.EditorController.Delete(e.SelectedWOs);
				HideGizmos();
				return;
			}
			if (MVInputWrapper.GetBooleanControlDown(KogamaControls.LeaveObject))
			{
				HandleEscape(e);
				return;
			}
			VoxelHit hit = default;
			bool flag = MVGameController.WOCM.Pick(ref hit);
			if (flag && (MVGameController.WOCM.IsType(hit.woId, WorldObjectType.CubeModelPrototypeTerrain) || hit.woId == -1))
			{
				flag = false;
			}
			TintObjectsOnMouseOver(e, flag, hit);
			if (pickedTarget != null)
			{
				MVWorldObjectClientManager wOCM = MVGameController.WOCM;
				VoxelHit hit2 = pickedTarget.hit;
				if (wOCM.GetWorldObjectClient(hit2.woId) == null)
				{
					pickedTarget = null;
				}
			}
			if (MVInputWrapper.GetBooleanControlDown(KogamaControls.PointerSelect))
			{
				if (flag)
				{
					pickedTarget = new PickResult<MVWorldObjectClient>(MVInputWrapper.GetPointerPosition(), hit, MVGameController.WOCM.GetWorldObjectClient(hit.woId));
					HashSet<int> selectedIDs = e.SelectedIDs;
					VoxelHit hit3 = pickedTarget.hit;
					bool flag2 = selectedIDs.Contains(hit3.woId);
					if ((pickedTarget.data.InteractionFlags & InteractionFlags.NotUserTransformable) != InteractionFlags.NotUserTransformable)
					{
						bool addToSelection = flag2;
						selectedWorldObject = e.Select(pickedTarget.hit, addToSelection);
						Debug.Log("SelectedWorldObject: " + selectedWorldObject);
					}
					HideGizmos();
				}
			}
			else if (MVInputWrapper.GetBooleanControl(KogamaControls.PointerSelect))
			{
				if (pickedTarget != null && ((pickedTarget.mousePosition - MVInputWrapper.GetPointerPosition()).magnitude > 0.5f || MVInputWrapper.GetAxisRaw("Mouse ScrollWheel") != 0f))
				{
					if (e.SelectedIDs.Count == 1)
					{
						deselectAfterTranslate = true;
					}
					e.PushState(EditorEvent.ESTranslate);
				}
			}
			else if (MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelect))
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
					if (e.ParentGroup == MVGameController.WOCM.RootGroup)
					{
						e.Event = EditorEvent.ESTerrainEdit;
					}
				}
				pickedLink = null;
				pickedTarget = null;
			}
			else if (MVInputWrapper.GetBooleanControlDown(KogamaControls.PointerSelectAlt))
			{
				VoxelHit hit5 = default;
				LinkObjectScript linkHit = GetLinkHit(e, ref hit5);
				if (linkHit != null)
				{
					pickedLink = new PickResult<LinkObjectScript>(MVInputWrapper.GetPointerPosition(), hit5, linkHit);
				}
				else if (flag)
				{
					MVWorldObjectClient worldObjectClient = MVGameController.WOCM.GetWorldObjectClient(hit.woId);
					pickedTarget = new PickResult<MVWorldObjectClient>(MVInputWrapper.GetPointerPosition(), hit, worldObjectClient);
					if (!e.SelectedWOs.Contains(pickedTarget.data))
					{
						selectedWorldObject = e.Select(pickedTarget.hit, addToSelection: false);
						Debug.Log("SelectedWorldObject: " + selectedWorldObject);
					}
				}
			}
			else
			{
				if (!MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelectAlt))
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
							Debug.LogWarning("There should be selected objects at this point - Martin");
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
			e.CameraController.GetComponent<GrayscaleEffect>().enabled = false;
		}
		HideGizmos();
		exitButton.View.Hide();
		exitButton.exitButton.OnClick = null;
		editorStateMachine.SelectionController.SelectedWorldObjectDeleted -= SelectionController_SelectedWorldObjectDeletedHandler;
	}

	private bool IsMouseUpValid(Vector3 mousePosition)
	{
		return (mousePosition - MVInputWrapper.GetPointerPosition()).sqrMagnitude < 20f;
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
		if (MVGameController.EditorController.IsLogicRendered())
		{
			float num = float.PositiveInfinity;
			if (MVGameController.WOCM.Pick(ref hit))
			{
				num = hit.distance;
			}
			Ray ray = e.CameraController.GetComponent<Camera>().ScreenPointToRay(MVInputWrapper.GetPointerPosition());
			int layerMask = 1 << LayerMask.NameToLayer("Logic");
			Physics.Raycast(ray, out var hitInfo, float.PositiveInfinity, layerMask);
			if (hitInfo.collider != null && hitInfo.distance < num)
			{
				hit.point = hitInfo.point;
				return hitInfo.collider.gameObject.GetComponentInChildren<LinkObjectScript>();
			}
		}
		return null;
	}

	private bool EnterObject(EditorStateMachine e, MVWorldObjectClient selectedWo)
	{
		Debug.Log("EnterObject");
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
			e.CameraController.GetComponent<GrayscaleEffect>().enabled = false;
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
