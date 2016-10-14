using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameMeterVisuals;

public class SlideOnClick : GameMeterVisualEffect, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField]
	private Vector3 targetPos;

	[SerializeField]
	private RectTransform rectTransform;

	[SerializeField]
	private float lerpSpeed;

	[SerializeField]
	private float waitBeforeMoveBack;

	private Vector3 startPos;

	private bool readyForSlide = true;

	private bool holding;

	private void Start()
	{
		startPos = rectTransform.anchoredPosition;
	}

	public override void ExecuteEffect()
	{
		if (gameObject.activeInHierarchy && readyForSlide)
		{
			readyForSlide = false;
			StartCoroutine(SlideTowardsPosition());
		}
	}

	private IEnumerator SlideTowardsPosition()
	{
		for (float i = 0f; i < 1f; i += Time.deltaTime * lerpSpeed)
		{
			rectTransform.anchoredPosition = Vector3.Lerp(startPos, targetPos, i);
			yield return null;
		}
		rectTransform.anchoredPosition = targetPos;
		for (float i2 = 0f; i2 < waitBeforeMoveBack; i2 += Time.deltaTime)
		{
			yield return null;
		}
		while (holding)
		{
			yield return null;
		}
		for (float i3 = 0f; i3 < 1f; i3 += Time.deltaTime * lerpSpeed)
		{
			rectTransform.anchoredPosition = Vector3.Lerp(targetPos, startPos, i3);
			yield return null;
		}
		rectTransform.anchoredPosition = startPos;
		readyForSlide = true;
	}

	private void OnDisable()
	{
		StopAllCoroutines();
		readyForSlide = true;
		rectTransform.anchoredPosition = startPos;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		holding = true;
		if (readyForSlide)
		{
			readyForSlide = false;
			StartCoroutine(SlideTowardsPosition());
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		holding = false;
	}
}
