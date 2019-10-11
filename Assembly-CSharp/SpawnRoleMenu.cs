using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using MV.WorldObject.GamePassSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpawnRoleMenu : LobbyFlowMenu, IDragInputReciever
{
	[SerializeField]
	private RectTransform elementContainer;

	[SerializeField]
	private Scrollbar scrollbar;

	[SerializeField]
	private DragInputHandler dragInputReciever;

	[SerializeField]
	private SpawnRoleSelectionButtonController buttonController;

	[SerializeField]
	private GameObject backButton;

	[SerializeField]
	private DefaultSpawnRoleSelectionElement defaultSelectionElementPrefab;

	[SerializeField]
	private SpawnRoleSelectionElement selectionElementPrefab;

	[SerializeField]
	private TierUnlockDetailsPopup tierUnlockPopupPrefab;

	[SerializeField]
	private TierTestDetailsPopup tierTestPopupPrefab;

	[SerializeField]
	private TierLockedDetailsPopup tierLockedPopupPrefab;

	[SerializeField]
	private float selectionElementWidth;

	[SerializeField]
	private float elementSpacing;

	[SerializeField]
	private int maxSelectionElementsOnScreen = 10;

	private List<DefaultSpawnRoleSelectionElement> SelectionElementsList = new List<DefaultSpawnRoleSelectionElement>();

	private int selectedSpawnRole;

	private float interpolateToPositionX;

	private float interpolationStartTime;

	private bool shouldInterpolate;

	private float menuHalfWidth;

	private float dragStartPositionX;

	private int currentSelectionStartIndex;

	private bool awaitingSpawn;

	private MVTeam shownTeam;

	public static Action<int> OnNewSpawnRoleSelected;

	protected override LobbyFlowMenuType MenuType => LobbyFlowMenuType.SpawnRoleSelect;

	public void OnBeginDrag()
	{
		shouldInterpolate = false;
		dragStartPositionX = elementContainer.localPosition.x;
		for (int i = 0; i < SelectionElementsList.Count; i++)
		{
			SelectionElementsList[i].IsDragging = true;
		}
	}

	public void OnEndDrag()
	{
		OnSpawnRoleSelected(CalculateNewSelectedItem());
		for (int i = 0; i < SelectionElementsList.Count; i++)
		{
			SelectionElementsList[i].IsDragging = false;
		}
	}

	public void OnDrag()
	{
		int num = CalculateNewSelectedItem();
		if (num != selectedSpawnRole)
		{
			SelectionElementsList[selectedSpawnRole].OnUnSelected();
			selectedSpawnRole = num;
			SelectionElementsList[num].OnSelctionHighlight();
		}
	}

	public void OnScrollValueChange()
	{
		UpdateShownElements();
	}

	private void UpdateShownElements()
	{
		float value = scrollbar.value;
		int count = SelectionElementsList.Count;
		int num = Mathf.FloorToInt((float)count * value);
		int num2 = Mathf.FloorToInt((float)num - (float)maxSelectionElementsOnScreen / 2f);
		HideElements(currentSelectionStartIndex, SelectionElementsList.Count, num2);
		ShowElements(num2);
	}

	private bool IsIndexWithinBounds(int index)
	{
		return index >= 0 && index < SelectionElementsList.Count;
	}

	private void ShowElements(int startElementIndex)
	{
		currentSelectionStartIndex = startElementIndex;
		for (int i = startElementIndex; i < startElementIndex + maxSelectionElementsOnScreen; i++)
		{
			if (IsIndexWithinBounds(i))
			{
				SelectionElementsList[i].Activate();
			}
		}
	}

	private void HideElements(int previousStartElement, int amoutOfElements, int newStartElement)
	{
		for (int i = previousStartElement; i < previousStartElement + amoutOfElements; i++)
		{
			if (IsIndexWithinBounds(i) && (i < newStartElement || i > newStartElement + maxSelectionElementsOnScreen))
			{
				SelectionElementsList[i].Deactivate();
			}
		}
	}

	public void OnSelectButtonPressed()
	{
		int wOID = SelectionElementsList[selectedSpawnRole].WOID;
		bool flag = wOID == MVGameControllerBase.LocalPlayer.SpawnRoleDataMediator.WoId;
		if (MVGameControllerBase.Game.TeamManager.TeamCount() > 1)
		{
			HandleTeamSwitching(wOID);
		}
		if (flag)
		{
			Close();
			StartPlaying();
			return;
		}
		AwaitSpawnThenClose();
		if (wOID == MVGameControllerBase.LocalPlayer.DefaultSpawnRoleId)
		{
			MVGameControllerBase.Game.LocalPlayer.SetActiveSpawnRole(wOID);
			if (OnNewSpawnRoleSelected != null)
			{
				OnNewSpawnRoleSelected(MVGameControllerBase.LocalPlayer.DefaultSpawnRoleId);
			}
		}
		else
		{
			MVGameControllerBase.Game.LocalPlayer.CreateSpawnRole(wOID);
			if (OnNewSpawnRoleSelected != null)
			{
				OnNewSpawnRoleSelected(wOID);
			}
		}
		PrepareForSpawnRoleActivating();
	}

	public void OpenTierShopButtonPressed()
	{
		if (MVGameControllerBase.GameSessionData.gameMode == MVGameMode.Edit)
		{
			ShowTestTier();
		}
		else
		{
			ShowTierPurchase();
		}
	}

	public void LockedButtonPressed()
	{
		if (MVGameControllerBase.GameSessionData.gameMode == MVGameMode.Edit)
		{
			ShowTestTier();
		}
		else
		{
			ShowLockedTierPurchase();
		}
	}

	public void HideBackButton()
	{
		backButton.SetActive(value: false);
	}

	public override void Start()
	{
		base.Start();
		GamePassesManager.OnPlayerPlanetDataUpdated = (Action)Delegate.Combine(GamePassesManager.OnPlayerPlanetDataUpdated, new Action(OnPlayerPlanetDataUpdated));
	}

	public void Initialize(MVTeam team)
	{
		shownTeam = team;
		dragInputReciever.AddInputReciever(this);
		MVGameControllerBase.MainCameraManager.CamMaskMode = MaskMode.SkyBoxOnly;
		List<MVWorldObjectClient> worldObjectsByType = MVGameControllerBase.Game.WorldObjectClientManager.GetWorldObjectsByType(WorldObjectType.AvatarSpawnRoleCreator);
		List<ISpawnRolePreviewObject> list = new List<ISpawnRolePreviewObject>();
		List<MVWorldObjectClient> list2 = new List<MVWorldObjectClient>();
		for (int i = 0; i < worldObjectsByType.Count; i++)
		{
			ISpawnRolePreviewObject spawnRolePreviewObject = (ISpawnRolePreviewObject)worldObjectsByType[i];
			if (spawnRolePreviewObject.GetTeamRequirement() == MVTeam.None || spawnRolePreviewObject.GetTeamRequirement() == shownTeam)
			{
				list.Add(spawnRolePreviewObject);
				list2.Add(worldObjectsByType[i]);
			}
		}
		List<ISpawnRolePreviewObject> sortedList = GetSortedList(list);
		List<MVWorldObjectClient> sortedWorldObjectList = GetSortedWorldObjectList(list2, list);
		bool flag = MVGameControllerBase.Game.TeamManager.TeamHasSpawnPoints(shownTeam);
		int startIndex = 0;
		if (flag)
		{
			startIndex = 1;
		}
		for (int j = 0; j < sortedList.Count; j++)
		{
			CreateSpawnRoleSelectionElement(startIndex, j, sortedList, sortedWorldObjectList);
		}
		if (flag)
		{
			CreateDefaultAvatarElement();
		}
		LayoutRebuilder.ForceRebuildLayoutImmediate(elementContainer);
		menuHalfWidth = elementContainer.rect.width / 2f;
		OnSpawnRoleSelected(0);
		elementContainer.localPosition = new Vector3(interpolateToPositionX, elementContainer.localPosition.y);
		HideElements(0, SelectionElementsList.Count, 0);
		ShowElements(0);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		GamePassesManager.OnPlayerPlanetDataUpdated = (Action)Delegate.Remove(GamePassesManager.OnPlayerPlanetDataUpdated, new Action(OnPlayerPlanetDataUpdated));
		if (awaitingSpawn)
		{
			MVGameControllerBase.LocalPlayer.SpawnRolesManager.OnSpawnRoleActivated -= Close;
		}
	}

	private void Update()
	{
		if (shouldInterpolate)
		{
			float x = Mathf.Lerp(elementContainer.localPosition.x, interpolateToPositionX, Time.time - interpolationStartTime);
			elementContainer.localPosition = new Vector3(x, elementContainer.localPosition.y);
		}
	}

	private void CreateSpawnRoleSelectionElement(int startIndex, int index, List<ISpawnRolePreviewObject> sortedSpawnRoles, List<MVWorldObjectClient> sortedWorldObjects)
	{
		SpawnRoleSelectionElement spawnRoleSelectionElement = UnityEngine.Object.Instantiate(selectionElementPrefab);
		spawnRoleSelectionElement.Initialize(startIndex + index, sortedWorldObjects[index].Id, sortedSpawnRoles[index].GetTierRequirement(), OnSpawnRoleSelected, OnSpawnRoleActivated);
		spawnRoleSelectionElement.SetupPreviewImage(sortedSpawnRoles[index].GetSpawnRolePreviewObject());
		spawnRoleSelectionElement.OnUnSelected();
		spawnRoleSelectionElement.transform.SetParent(elementContainer, worldPositionStays: false);
		SelectionElementsList.Add(spawnRoleSelectionElement);
	}

	private void CreateDefaultAvatarElement()
	{
		DefaultSpawnRoleSelectionElement defaultSpawnRoleSelectionElement = UnityEngine.Object.Instantiate(defaultSelectionElementPrefab);
		defaultSpawnRoleSelectionElement.Initialize(0, MVGameControllerBase.LocalPlayer.DefaultSpawnRoleId, GamePassTier.Tier0, OnSpawnRoleSelected, OnSpawnRoleActivated);
		defaultSpawnRoleSelectionElement.SetupPreviewImage(MVGameControllerBase.LocalPlayer.Body.GameObject);
		defaultSpawnRoleSelectionElement.OnUnSelected();
		defaultSpawnRoleSelectionElement.transform.SetParent(elementContainer, worldPositionStays: false);
		defaultSpawnRoleSelectionElement.transform.SetAsFirstSibling();
		SelectionElementsList.Insert(0, defaultSpawnRoleSelectionElement);
	}

	private void OnSpawnRoleSelected(int newSelectedSpawnRole)
	{
		SelectionElementsList[selectedSpawnRole].OnUnSelected();
		selectedSpawnRole = newSelectedSpawnRole;
		SelectionElementsList[newSelectedSpawnRole].OnSelected();
		RecalculateInterpolation(selectedSpawnRole);
		buttonController.OnNewSelectedSpawnRole(SelectionElementsList[selectedSpawnRole].Tier);
	}

	private void OnSpawnRoleActivated(int newSelectedSpawnRole)
	{
		GamePassTier tier = SelectionElementsList[selectedSpawnRole].Tier;
		GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		if ((int)tier <= (int)gamePassTier)
		{
			OnSelectButtonPressed();
		}
		else if (tier == gamePassTier + 1)
		{
			OpenTierShopButtonPressed();
		}
		else
		{
			LockedButtonPressed();
		}
	}

	private void RecalculateInterpolation(int index)
	{
		float num = CalculateElementPosition(index);
		interpolateToPositionX = menuHalfWidth - num;
		interpolationStartTime = Time.time;
		shouldInterpolate = true;
	}

	private float CalculateElementPosition(int index)
	{
		return (float)(index + 1) * elementSpacing + (float)index * selectionElementWidth + selectionElementWidth / 2f;
	}

	private int CalculateNewSelectedItem()
	{
		if (SelectionElementsList.Count == 1)
		{
			return 0;
		}
		int count = SelectionElementsList.Count;
		float num = CalculateElementPosition(count - 1);
		float x = elementContainer.localPosition.x;
		float num2 = ((!(dragStartPositionX < x)) ? 1f : (-1f));
		x = menuHalfWidth - elementContainer.localPosition.x - CalculateElementPosition(0) / 2f;
		float num3 = x / num;
		float num4 = num3 * (float)(count - 1);
		num4 = ((!(num2 > 0f)) ? (num4 + 0.2f) : (num4 + 0.8f));
		num4 = Mathf.Clamp(num4, 0f, count - 1);
		return Mathf.FloorToInt(num4);
	}

	protected override void StartPlaying()
	{
		if (!FirstTimePressPlayController.HaveBeenPressed)
		{
			FirstTimePressPlayController.OnFirstTimePlayIsPressed();
		}
		MVGameControllerDesktop.LockCursorManager.CursorLock = true;
		if (MVGameControllerBase.GameSessionData.gameMode != MVGameMode.Edit || (MVGameControllerBase.GameSessionData.gameMode == MVGameMode.Edit && MVGameControllerBase.EditModeUI.IsInPlayInEditMode))
		{
			MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.SetToSpawnPoint();
		}
	}

	private void PrepareForSpawnRoleActivating()
	{
		if (!FirstTimePressPlayController.HaveBeenPressed)
		{
			FirstTimePressPlayController.OnFirstTimePlayIsPressed();
		}
		MVGameControllerDesktop.LockCursorManager.CursorLockWithoutCallback = true;
	}

	private List<ISpawnRolePreviewObject> GetSortedList(List<ISpawnRolePreviewObject> unsortedList)
	{
		List<ISpawnRolePreviewObject> list = new List<ISpawnRolePreviewObject>();
		for (int i = 0; i < unsortedList.Count; i++)
		{
			bool flag = false;
			for (int j = 0; j < list.Count; j++)
			{
				if ((int)unsortedList[i].GetTierRequirement() < (int)list[j].GetTierRequirement())
				{
					list.Insert(j, unsortedList[i]);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				list.Add(unsortedList[i]);
			}
		}
		return list;
	}

	private List<MVWorldObjectClient> GetSortedWorldObjectList(List<MVWorldObjectClient> wos, List<ISpawnRolePreviewObject> unsortedList)
	{
		List<ISpawnRolePreviewObject> list = new List<ISpawnRolePreviewObject>();
		List<MVWorldObjectClient> list2 = new List<MVWorldObjectClient>();
		for (int i = 0; i < unsortedList.Count; i++)
		{
			bool flag = false;
			for (int j = 0; j < list.Count; j++)
			{
				if ((int)unsortedList[i].GetTierRequirement() < (int)list[j].GetTierRequirement())
				{
					list.Insert(j, unsortedList[i]);
					list2.Insert(j, wos[i]);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				list.Add(unsortedList[i]);
				list2.Add(wos[i]);
			}
		}
		return list2;
	}

	private void HandleTeamSwitching(int spawnRoleId)
	{
		if (shownTeam != MVGameControllerBase.Game.LocalPlayer.Team)
		{
			MVGameControllerBase.OperationRequests.SetTeam(shownTeam);
			MVGameControllerBase.Game.GameStatCounterManager.RemoveTeamScoreOnActorLeave(MVGameControllerBase.Game.LocalPlayer.ActorNr, MVGameControllerBase.Game.LocalPlayer.Team);
			MVGameControllerBase.Game.LocalPlayer.ResetCheckpoint();
			MVGameControllerBase.Game.LocalPlayer.Team = shownTeam;
		}
	}

	private void ShowTierPurchase()
	{
		GamePassTier tier = SelectionElementsList[selectedSpawnRole].Tier;
		int progressionGamePoints = GamePassesManager.PlayerPlanetData.progressionGamePoints;
		GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		Dictionary<GamePassTier, PlayerTierState> tierPricingState = GamePassesManager.playerTierStateCalculator.GetTierPricingState(progressionGamePoints, gamePassTier);
		int remainingGoldPriceRequired = tierPricingState[tier].remainingGoldPriceRequired;
		TierUnlockDetailsPopup tierPurchasePopup = UnityEngine.Object.Instantiate(tierUnlockPopupPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(tierPurchasePopup.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
		tierPurchasePopup.Initialize(tier, remainingGoldPriceRequired, OnPurchaseGamePassTier);
	}

	private void ShowLockedTierPurchase()
	{
		GamePassTier tier = SelectionElementsList[selectedSpawnRole].Tier;
		TierLockedDetailsPopup tierLockedPopup = UnityEngine.Object.Instantiate(tierLockedPopupPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(tierLockedPopup.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
		tierLockedPopup.Initialize(tier, OnPurchaseGamePassTier);
	}

	private void ShowTestTier()
	{
		GamePassTier tier = SelectionElementsList[selectedSpawnRole].Tier;
		int progressionGamePoints = GamePassesManager.PlayerPlanetData.progressionGamePoints;
		GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		Dictionary<GamePassTier, PlayerTierState> tierPricingState = GamePassesManager.playerTierStateCalculator.GetTierPricingState(progressionGamePoints, gamePassTier);
		int remainingGoldPriceRequired = tierPricingState[tier].remainingGoldPriceRequired;
		TierTestDetailsPopup tierTestPopup = UnityEngine.Object.Instantiate(tierTestPopupPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(tierTestPopup.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
		tierTestPopup.Initialize(tier, remainingGoldPriceRequired);
	}

	private void OnPurchaseGamePassTier()
	{
		buttonController.OnNewSelectedSpawnRole(SelectionElementsList[selectedSpawnRole].Tier);
	}

	private void OnPlayerPlanetDataUpdated()
	{
		buttonController.OnNewSelectedSpawnRole(SelectionElementsList[selectedSpawnRole].Tier);
	}

	private void AwaitSpawnThenClose()
	{
		awaitingSpawn = true;
		MVGameControllerBase.LocalPlayer.SpawnRolesManager.OnSpawnRoleActivated += Close;
	}

	private void Close(int spawnRoleID = 0)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.Pop();
		});
		MVGameControllerBase.MainCameraManager.CamMaskMode = MaskMode.Default;
	}
}
