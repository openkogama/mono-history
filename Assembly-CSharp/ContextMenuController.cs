using System;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContextMenuController : MonoBehaviour, IHandlePointerDownOnContextMenuButton, IEventSystemHandler
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
		PlayerInventoryRepository playerInventoryRepository = MVGameControllerBase.EditModeUI.PlayerInventoryRepository;
		playerInventoryRepository.OnFailedToAddItem = (Action)Delegate.Combine(playerInventoryRepository.OnFailedToAddItem, new Action(OnFailedToAddItem));
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
		if (!MVClientSettings.IsFlagSet(ClientSettingFlags.GamePassSilentReleaseEnabled) && worldObjectClient.HasInteractionFlag(InteractionFlags.CanUseGameRank))
		{
			contextMenu.AddButton(TM._("Game Tier"), ShowGameRankDialog);
		}
		if (worldObjectClient.HasInteractionFlag(InteractionFlags.HasSettings))
		{
			contextMenu.AddButton(TM._("Settings"), ShowSettingsDialog);
		}
		if (worldObjectClient.HasInteractionFlag(InteractionFlags.Sounds))
		{
			contextMenu.AddButton(TM._("Sounds"), ShowSoundsDialog);
		}
		if (worldObjectClient.HasInteractionFlag(InteractionFlags.GlobalSounds))
		{
			contextMenu.AddButton(TM._("Global Sounds"), ShowGlobalSoundsDialog);
		}
		if (worldObjectClient.HasInteractionFlag(InteractionFlags.CanEdit) && !worldObjectClient.HasInteractionFlag(InteractionFlags.IsPreview))
		{
			contextMenu.AddButton(TM._("Edit Model"), EnterCubeEdit);
		}
		if (worldObjectClient.HasInteractionFlag(InteractionFlags.CanResetLogic))
		{
			contextMenu.AddButton(TM._("Reset Logic"), ResetLogic);
		}
		if (worldObjectClient.HasInteractionFlag(InteractionFlags.CanEnterPlay))
		{
			contextMenu.AddButton(TM._("Play"), EnterPlay);
		}
		if (!MVClientSettings.IsFlagSet(ClientSettingFlags.GamePassSilentReleaseEnabled) && worldObjectClient.HasInteractionFlag(InteractionFlags.CanEarnGamePoints))
		{
			contextMenu.AddButton(TM._("Crystals"), ShowGamePointsDialog);
		}
		if (!MVClientSettings.IsFlagSet(ClientSettingFlags.GamePassSilentReleaseEnabled) && worldObjectClient.HasInteractionFlag(InteractionFlags.CanEarnGamePointsMinor))
		{
			contextMenu.AddButton(TM._("Crystals"), ShowMinorGamePointsDialog);
		}
		if (worldObjectClient.HasInteractionFlag(InteractionFlags.CanRespawn))
		{
			contextMenu.AddButton(TM._("Respawn"), ShowRespawnDialog);
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
		contextMenu.AddButton(TM._("Delete"), Delete);
		PopGizmos();
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
		PopGizmos();
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(contextMenu.gameObject, UIPushOption.None, OnContextMenuPop, UIGroupFlags.GameObjectUI);
		});
		contextMenu.InitializeLink(linkID, worldPos);
	}

	public void PopGizmos()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.GameObjectUI | UIGroupFlags.GameObjectUISubMenu);
		});
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

	private void ShowGameRankDialog()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.GameObjectUI);
		});
		settingsFactory.CreateSettingsDialog(woID, UseRequirementType.GameRank);
	}

	private void ShowGamePointsDialog()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.GameObjectUI);
		});
		settingsFactory.CreateGamePointsSettings(woID);
	}

	private void ShowMinorGamePointsDialog()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.GameObjectUI);
		});
		settingsFactory.CreateGamePointsMinorRewardSettings(woID);
	}

	private void ShowRespawnDialog()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.GameObjectUI);
		});
		settingsFactory.CreateRespawnSetting(woID);
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

	private void ShowGlobalSoundsDialog()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.GameObjectUI);
		});
		settingsFactory.CreateGlobalSoundsInventory(woID);
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
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create(TM._("Image upload is disabled in standalone. Reload game using the browser version to update image of model.\n"), OnClosedStandaloneError, TM._("Are you sure?"));
		});
	}

	private void EnterPlay()
	{
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woID);
		if (MVGameControllerBase.WOCM.GetWorldObjectClient(woID) is MVAvatarSpawnRoleCreator)
		{
			MVLocalPlayerBuilder.EnterPlayStateDataStruct enterPlayStateData = ((MVLocalPlayerBuilder)MVGameControllerBase.LocalPlayer).EnterPlayStateData;
			enterPlayStateData.selectedSpawnRoleCreator = woID;
			((MVLocalPlayerBuilder)MVGameControllerBase.LocalPlayer).EnterPlayStateData = enterPlayStateData;
			GamePassTier tier = ((MVAvatarSpawnRoleCreator)worldObjectClient).Tier;
			if ((int)tier > (int)GamePassesManager.PlayerPlanetData.gamePassTier)
			{
				MVGameControllerBase.OperationRequests.SetTier(tier);
			}
		}
		MVGameControllerBase.GameEventManager.GameState.OnDisableLobbyState();
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IEditModeController x, BaseEventData y) =>
		{
			x.EnterPlayMode();
		});
	}

	private void OnClosedStandaloneError(bool confirmed, ConfirmationPopup popup)
	{
		if (confirmed)
		{
			ItemImageUploaded(woID);
			return;
		}
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
	}

	private void ItemImageUploaded(int woId)
	{
		MVGameControllerBase.OperationRequests.AddWorldObjectToInventory(woId);
	}

	private void OnFailedToAddItem()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopToGroup(UIGroupFlags.MainUI);
		});
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
