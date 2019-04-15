using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupSlideshowController : MonoBehaviour
{
	[SerializeField]
	private List<GameObject> pages;

	private int currentPage;

	[SerializeField]
	private GameObject pageControls;

	[SerializeField]
	private GameObject pageRight;

	[SerializeField]
	private GameObject pageLeft;

	[SerializeField]
	private Text currentPageText;

	[SerializeField]
	private bool lastPageHasPageControls = true;

	public void Start()
	{
		currentPage = 1;
		PageTurned(0);
	}

	public void PageTurned(int dir)
	{
		int num = currentPage + dir - 1;
		pages[currentPage - 1].SetActive(value: false);
		pages[num].SetActive(value: true);
		currentPage = num + 1;
		currentPageText.text = num + 1 + "/" + pages.Count;
		if (!lastPageHasPageControls)
		{
			pageControls.SetActive(num != pages.Count - 1);
		}
		pageLeft.gameObject.SetActive(num > 0);
		pageRight.gameObject.SetActive(num < pages.Count - 1);
	}
}
