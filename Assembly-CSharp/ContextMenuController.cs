using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContextMenuController : MonoBehaviour, IEventSystemHandler, IHandlePointerDownOnContextMenuButton
{
	private int woID;

	private EditorStateMachine editorStateMachine;

	private bool rightClickGizmoSelect;

	[SerializeField]
	private ContextMenu contextMenuPrefab;

	[SerializeField]
	private SettingsFactory settingsFactory;

	public bool MouseDown => rightClickGizmoSelect;

	public void Initialize(EditorStateMachine editorStateMachine)
	{
		this.editorStateMachine = editorStateMachine;
		PlayerInventoryRepository playerInventoryRepository = MVGameControllerBase.IEditModeUI.PlayerInventoryRepository;
		playerInventoryRepository.OnInventoryItemAdded = (Action<int, int>)Delegate.Combine(playerInventoryRepository.OnInventoryItemAdded, new Action<int, int>(OnFinishedAddingItem));
	}

	public void ShowContextMenu(int woID, Vector3 worldPos)
	{
		this.woID = woID;
		ContextMenu contextMenu = UnityEngine.Object.Instantiate(contextMenuPrefab);
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woID);
		if (worldObjectClient.HasInteractionFlag(InteractionFlags.CanUseStars))
		{
			contextMenu.AddButton(TM._("Stars"), ShowStarsDialog);
		}
		if (worldObjectClient.HasInteractionFlag(InteractionFlags.CanUseTeam))
		{
			contextMenu.AddButton(TM._("Team"), ShowTeamDialog);
		}
		if (worldObjectClient.HasInteractionFlag(InteractionFlags.CanUseGameCoins))
		{
			contextMenu.AddButton(TM._("Game Coins"), ShowGameCoinsDialog);
		}
		if (worldObjectClient.HasInteractionFlag(InteractionFlags.CanUseLevel))
		{
			contextMenu.AddButton(TM._("Levels"), ShowLevelsDialog);
		}
		if (worldObjectClient.HasInteractionFlag(InteractionFlags.HasSettings))
		{
			contextMenu.AddButton(TM._("Settings"), ShowSettingsDialog);
		}
		if (worldObjectClient.HasInteractionFlag(InteractionFlags.Sounds))
		{
			contextMenu.AddButton(TM._("Sounds"), ShowSoundsDialog);
		}
		if (worldObjectClient.HasInteractionFlag(InteractionFlags.CanEdit) && !worldObjectClient.HasInteractionFlag(InteractionFlags.IsPreview))
		{
			contextMenu.AddButton(TM._("Edit Model"), EnterCubeEdit);
		}
		if (worldObjectClient.HasInteractionFlag(InteractionFlags.CanResetLogic))
		{
			contextMenu.AddButton(TM._("Reset Logic"), ResetLogic);
		}
		if (CanClone())
		{
			contextMenu.AddButton(TM._("Clone"), Clone);
		}
		if (CanCloneRoot())
		{
			contextMenu.AddButton(TM._("Clone"), CloneRoot);
		}
		if (worldObjectClient.HasInteractionFlag(InteractionFlags.CanAddToInventory) && !worldObjectClient.HasInteractionFlag(InteractionFlags.IsPreview))
		{
			contextMenu.AddButton(TM._("Add To Inventory"), AddToInventory);
		}
		if (worldObjectClient.HasInteractionFlag(InteractionFlags.IsPreview) && worldObjectClient.PreviewOwnerProfileId == MVGameControllerBase.Game.LocalPlayer.ProfileID)
		{
			contextMenu.AddButton(TM._("Purchase"), ShowClientShopInventory);
		}
		contextMenu.AddButton(TM._("Delete"), Delete);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.GameObjectUI | UIGroupFlags.GameObjectUISubMenu);
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(contextMenu.gameObject, UIPushOption.None, OnContextMenuPop, UIGroupFlags.GameObjectUI);
		});
		contextMenu.Initialize(worldObjectClient.Id, worldPos);
	}

	public void ShowContextMenuLink(int linkID, bool isObjectLink, Vector3 worldPos)
	{
		ContextMenu contextMenu = UnityEngine.Object.Instantiate(contextMenuPrefab);
		contextMenu.AddButton(TM._("Delete"), () =>
		{
			DeleteLink(linkID, isObjectLink);
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.GameObjectUI);
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(contextMenu.gameObject, UIPushOption.None, OnContextMenuPop, UIGroupFlags.GameObjectUI);
		});
		contextMenu.InitializeLink(linkID, worldPos);
	}

	private void DeleteLink(int linkID, bool isObjectLink)
	{
		if (isObjectLink)
		{
			MVGameControllerBase.OperationRequests.RemoveObjectLink(linkID);
		}
		else
		{
			MVGameControllerBase.OperationRequests.RemoveLink(linkID);
		}
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.GameObjectUI | UIGroupFlags.GameObjectUISubMenu);
		});
	}

	private void ShowStarsDialog()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.GameObjectUI);
		});
		settingsFactory.CreateSettingsDialog(woID, UseRequirementType.Star);
	}

	private void ShowTeamDialog()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.GameObjectUI);
		});
		settingsFactory.CreateSettingsDialog(woID, UseRequirementType.Team);
	}

	private void ShowGameCoinsDialog()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.GameObjectUI);
		});
		settingsFactory.CreateSettingsDialog(woID, UseRequirementType.GameCoin);
	}

	private void ShowLevelsDialog()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.GameObjectUI);
		});
		settingsFactory.CreateSettingsDialog(woID, UseRequirementType.Level);
	}

	private void ShowSettingsDialog()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.GameObjectUI);
		});
		settingsFactory.CreateSettingsDialog(woID);
	}

	private void ShowSoundsDialog()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.GameObjectUI);
		});
		settingsFactory.CreateSoundsInventory(woID);
	}

	private void EnterCubeEdit()
	{
		MVGameControllerBase.WOCM.GetWorldObjectClient(woID).OnEnterObject(editorStateMachine);
	}

	private void ResetLogic()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.GameObjectUI);
		});
		MVGameControllerBase.OperationRequests.ResetLogicChunk(woID);
	}

	private void ShowClientShopInventory()
	{
	}

	private void Clone()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.GameObjectUI);
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (ICloneHandler handler, BaseEventData data) =>
		{
			handler.Clone(MVGameControllerBase.WOCM.GetWorldObjectClient(woID), cloneToRoot: false, setAsPreviewItem: false);
		});
	}

	private void CloneRoot()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.GameObjectUI);
		});
		MVWorldObjectClient root = MVGameControllerBase.WOCM.GetWorldObjectClientRoot(woID);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (ICloneHandler handler, BaseEventData data) =>
		{
			handler.Clone(root, cloneToRoot: false, setAsPreviewItem: false);
		});
	}

	private void AddToInventory()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create();
		});
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woID);
		Action<byte[]> callback = (byte[] imageData) =>
		{
			DataUploadManager.UploadData(imageData, () =>
			{
				ItemImageUploaded(woID);
			});
		};
		StartCoroutine(ImageGenerator.CreateTextureFromData(worldObjectClient, callback));
	}

	private void ItemImageUploaded(int woId)
	{
		MVGameControllerBase.OperationRequests.AddWorldObjectToInventory(woId);
	}

	private void OnFinishedAddingItem(int category, int slotPosition)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopToBottom();
		});
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add((byte)13, category);
		dictionary.Add((byte)14, slotPosition);
		NotificationController.PushNotification(NotificationType.OpenInventory, NotificationsManager.eNotificationPanel.primary, dictionary, (NotificationLifetime)4);
	}

	private void Delete()
	{
		editorStateMachine.DeSelectAll();
		string errorText = string.Empty;
		if (!MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Delete(MVGameControllerBase.WOCM, ref errorText))
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.CreateErrorNotificationPopup(errorText);
			});
		}
		else
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
			{
				handler.Pop();
			});
		}
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

	private bool CanCloneRoot()
	{
		foreach (MVWorldObjectClient selectedWO in editorStateMachine.SelectedWOs)
		{
			if (!selectedWO.HasInteractionFlag(InteractionFlags.CanCloneRoot) || selectedWO.HasInteractionFlag(InteractionFlags.IsPreview))
			{
				return false;
			}
		}
		return true;
	}

	public void PointerIsDown()
	{
		rightClickGizmoSelect = true;
	}

	private void OnContextMenuPop()
	{
		rightClickGizmoSelect = false;
	}
}
