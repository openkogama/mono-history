using System;
using System.Collections.Generic;
using UnityEngine;

public class MVGUIHelp : UXViewScript
{
	public GameObject ColliderBoxBlocker;

	public UXIconButton PreviousPage;

	public UXIconButton NextPage;

	public UXText pageText;

	public List<UXGroup> Pages;

	private int currentIndex;

	public override void OnShow()
	{
		base.OnShow();
		UXFullscreenColliderBox.Instance.AddBlockingObject(ColliderBoxBlocker);
		UXFullscreenColliderBox instance = UXFullscreenColliderBox.Instance;
		instance.OnClick = (Action)Delegate.Combine(instance.OnClick, (Action)(() =>
		{
			View.Hide();
		}));
	}

	public override void OnHide()
	{
		base.OnHide();
		UXFullscreenColliderBox.Instance.RemoveBlockingObject(ColliderBoxBlocker);
	}

	public override void OnInitialize()
	{
		UXIconButton previousPage = PreviousPage;
		previousPage.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(previousPage.OnClick, new UXBaseButton.OnClickDelegate(HandleOnClickPrevious));
		UXIconButton nextPage = NextPage;
		nextPage.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(nextPage.OnClick, new UXBaseButton.OnClickDelegate(HandleOnClickNext));
		GoToPage(currentIndex);
	}

	private void HandleOnClickNext()
	{
		currentIndex++;
		if (currentIndex > Pages.Count - 1)
		{
			currentIndex = Pages.Count - 1;
		}
		GoToPage(currentIndex);
	}

	private void HandleOnClickPrevious()
	{
		currentIndex--;
		if (currentIndex < 0)
		{
			currentIndex = 0;
		}
		GoToPage(currentIndex);
	}

	private void GoToPage(int index)
	{
		pageText.Text = index + 1 + "/" + Pages.Count;
		foreach (UXGroup page in Pages)
		{
			page.Hide();
			if (Pages.IndexOf(page) == currentIndex)
			{
				page.Show();
			}
		}
	}
}
