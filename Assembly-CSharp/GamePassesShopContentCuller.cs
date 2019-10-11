using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePassesShopContentCuller : MonoBehaviour
{
	[SerializeField]
	private Scrollbar scrollbar;

	[SerializeField]
	private int maxSelectionElementsOnScreen;

	private int currentSelectionStartIndex;

	private List<IGamePassShopContent> gamePassShopContentList = new List<IGamePassShopContent>();

	public void AddContentElement(IGamePassShopContent gamePassContentElement)
	{
		gamePassShopContentList.Add(gamePassContentElement);
	}

	public void Initialize()
	{
		HideElements(0, gamePassShopContentList.Count, 0);
		ShowElements(0);
	}

	public void OnScrollValueChanged()
	{
		UpdateShownElements();
	}

	private void UpdateShownElements()
	{
		float value = scrollbar.value;
		int count = gamePassShopContentList.Count;
		int num = Mathf.FloorToInt((float)count * value);
		int num2 = Mathf.FloorToInt((float)num - (float)maxSelectionElementsOnScreen / 2f);
		HideElements(currentSelectionStartIndex, gamePassShopContentList.Count, num2);
		ShowElements(num2);
	}

	private bool IsIndexWithinBounds(int index)
	{
		return index >= 0 && index < gamePassShopContentList.Count;
	}

	private void ShowElements(int startElementIndex)
	{
		currentSelectionStartIndex = startElementIndex;
		for (int i = startElementIndex; i < startElementIndex + maxSelectionElementsOnScreen; i++)
		{
			if (IsIndexWithinBounds(i))
			{
				gamePassShopContentList[i].Activate();
			}
		}
	}

	private void HideElements(int previousStartElement, int amoutOfElements, int newStartElement)
	{
		for (int i = previousStartElement; i < previousStartElement + amoutOfElements; i++)
		{
			if (IsIndexWithinBounds(i) && (i < newStartElement || i > newStartElement + maxSelectionElementsOnScreen))
			{
				gamePassShopContentList[i].Deactivate();
			}
		}
	}
}
