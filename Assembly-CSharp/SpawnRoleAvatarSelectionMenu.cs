using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpawnRoleAvatarSelectionMenu : MonoBehaviour
{
	[SerializeField]
	private RectTransform avatarElementContainer;

	[SerializeField]
	private GameObject loadingWheel;

	[SerializeField]
	private Scrollbar scrollBar;

	[SerializeField]
	private SpawnRoleAvatarSelectionElement avatarSelectionElementPrefab;

	[SerializeField]
	private int maxSelectionElementsOnScreen = 10;

	private int spawnRoleId;

	private int currentSelectionStartIndex;

	private List<SpawnRoleAvatarSelectionData> avatarSelectionDataList;

	private List<SpawnRoleAvatarSelectionElement> selectionElements = new List<SpawnRoleAvatarSelectionElement>();

	public void Initialize(int spawnRoleId)
	{
		this.spawnRoleId = spawnRoleId;
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IHandleSpawnRoleAvatarSelectionData handler, BaseEventData data) =>
		{
			handler.TryGetSpawnRoleAvatarSelectionData(OnSpawnRoleAvatarDataRecieved);
		});
	}

	public void OnScrollValueChanged()
	{
		if (avatarSelectionDataList != null)
		{
			UpdateShownElements();
		}
	}

	private void OnDestroy()
	{
		if (avatarSelectionDataList != null)
		{
			for (int i = 0; i < avatarSelectionDataList.Count; i++)
			{
				avatarSelectionDataList[i].avatar.GameObject.SetActive(value: false);
			}
		}
	}

	private void OnSpawnRoleAvatarDataRecieved(List<SpawnRoleAvatarSelectionData> avatarSelectionDataList)
	{
		loadingWheel.SetActive(value: false);
		this.avatarSelectionDataList = avatarSelectionDataList;
		for (int i = 0; i < avatarSelectionDataList.Count; i++)
		{
			AddSelectionElement(i);
		}
		HideElements(0, maxSelectionElementsOnScreen, 0);
		ShowElements(0);
	}

	private void UpdateShownElements()
	{
		float value = scrollBar.value;
		int count = avatarSelectionDataList.Count;
		int num = Mathf.FloorToInt((float)count * value);
		int num2 = Mathf.FloorToInt((float)num - (float)maxSelectionElementsOnScreen / 2f);
		HideElements(currentSelectionStartIndex, selectionElements.Count, num2);
		ShowElements(num2);
	}

	private bool IsIndexWithinBounds(int index)
	{
		return index >= 0 && index < selectionElements.Count;
	}

	private void ShowElements(int startElementIndex)
	{
		currentSelectionStartIndex = startElementIndex;
		for (int i = startElementIndex; i < startElementIndex + maxSelectionElementsOnScreen; i++)
		{
			if (IsIndexWithinBounds(i))
			{
				selectionElements[i].Activate();
				avatarSelectionDataList[i].avatar.GameObject.SetActive(value: true);
			}
		}
	}

	private void HideElements(int previousStartElement, int amoutOfElements, int newStartElement)
	{
		for (int i = previousStartElement; i < previousStartElement + amoutOfElements; i++)
		{
			if (IsIndexWithinBounds(i) && (i < newStartElement || i > newStartElement + maxSelectionElementsOnScreen))
			{
				selectionElements[i].Deactivate();
				avatarSelectionDataList[i].avatar.GameObject.SetActive(value: false);
			}
		}
	}

	private void AddSelectionElement(int index)
	{
		avatarSelectionDataList[index].avatar.GameObject.SetActive(value: true);
		SpawnRoleAvatarSelectionElement spawnRoleAvatarSelectionElement = Object.Instantiate(avatarSelectionElementPrefab);
		spawnRoleAvatarSelectionElement.transform.SetParent(avatarElementContainer, worldPositionStays: false);
		spawnRoleAvatarSelectionElement.Initialize(index, avatarSelectionDataList[index].avatarId, OnAvatarSelected);
		spawnRoleAvatarSelectionElement.SetupPreviewImage(avatarSelectionDataList[index].avatar.GameObject);
		selectionElements.Add(spawnRoleAvatarSelectionElement);
	}

	private void OnAvatarSelected(int avatarId)
	{
		MVGameControllerBase.Game.OperationRequestSender.SetSpawnRoleBody(spawnRoleId, avatarId);
	}
}
