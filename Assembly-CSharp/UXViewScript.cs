using System;
using UnityEngine;

[RequireComponent(typeof(UXView))]
public class UXViewScript : MonoBehaviour
{
	private UXView view;

	private bool isInitialized;

	public UXView View => view;

	public virtual void Awake()
	{
		Initialize();
	}

	public void Initialize()
	{
		if (!isInitialized)
		{
			isInitialized = true;
			view = GetComponent<UXView>();
			UXView uXView = view;
			uXView.OnShow = (UXView.OnShowDelegate)Delegate.Combine(uXView.OnShow, new UXView.OnShowDelegate(OnShow));
			UXView uXView2 = view;
			uXView2.OnHide = (UXView.OnHideDelegate)Delegate.Combine(uXView2.OnHide, new UXView.OnHideDelegate(OnHide));
			OnInitialize();
		}
	}

	public virtual void OnInitialize()
	{
	}

	public void ToggleShow()
	{
		View.ToggleVisibility();
	}

	public virtual void OnShow()
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).gameObject.SetActive(value: true);
		}
	}

	public virtual void OnHide()
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).gameObject.SetActive(value: false);
		}
	}

	public void OnDestroy()
	{
		if (!(view == null))
		{
			UXView uXView = view;
			uXView.OnShow = (UXView.OnShowDelegate)Delegate.Remove(uXView.OnShow, new UXView.OnShowDelegate(OnShow));
			UXView uXView2 = view;
			uXView2.OnHide = (UXView.OnHideDelegate)Delegate.Remove(uXView2.OnHide, new UXView.OnHideDelegate(OnHide));
		}
	}
}
