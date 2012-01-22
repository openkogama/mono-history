using System;
using UnityEngine;

public class UXStaticMenu : MonoBehaviour
{
	public delegate void OnOpenDelegate();

	public delegate void OnCloseDelegate();

	public Vector3 slideInDirection = new Vector3(1f, 0f, 0f);

	public UXButton button;

	public UXWindow window;

	public float slideDistance;

	private bool isOpen;

	private float slideRatio;

	private float slideRatioTarget;

	public float slideDuration = 0.5f;

	public string headerText = "Menu";

	private string closedText;

	public string openText = "Close";

	public float slidingWindowDepth = 1f;

	public OnOpenDelegate OnOpen;

	public OnCloseDelegate OnClose;

	public UXStaticMenu()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
	}

	public void Awake()
	{
		closedText = headerText;
		button.text = ((!isOpen) ? closedText : openText);
		UXButton uXButton = button;
		uXButton.OnClick = (UXButton.OnClickDelegate)Delegate.Combine(uXButton.OnClick, new UXButton.OnClickDelegate(OpenCloseClick));
		SetTransitionRatio(slideRatio);
	}

	public void OpenCloseClick()
	{
		isOpen = !isOpen;
		Notify(isOpen);
		button.Text = ((!isOpen) ? closedText : openText);
		slideRatioTarget = ((!isOpen) ? 0f : 1f);
		((MonoBehaviour)this).StopAllCoroutines();
		((MonoBehaviour)this).StartCoroutine(pTween.To(slideDuration, slideRatio, slideRatioTarget, SetTransitionRatio));
	}

	private void SetTransitionRatio(float t)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		slideRatio = Mathf.SmoothStep(0f, 1f, t);
		((Component)window).transform.localPosition = slideInDirection * slideDistance * (slideRatio - 1f) + slidingWindowDepth * Vector3.forward;
	}

	private void Notify(bool isOpen)
	{
		if (isOpen)
		{
			if (OnOpen != null)
			{
				OnOpen();
			}
		}
		else if (OnClose != null)
		{
			OnClose();
		}
	}
}
