using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinningConditionBriefingLayoutFitter : MonoBehaviour
{
	[Serializable]
	private class LayoutGroupAspectFitterDef
	{
		public LayoutGroup layoutGroup;

		public LayoutElement group;

		public List<LayoutElement> elements;

		public float elementAspectRatio = 1f;
	}

	[SerializeField]
	private VerticalLayoutGroup layoutGroup;

	[SerializeField]
	private List<LayoutGroupAspectFitterDef> elements;

	[SerializeField]
	private float groupAspectRatio = 1f;

	private Vector2 desktopReferenceRes = new Vector2(1920f, 1440f);

	private float defaultSpacing = 20f;

	private Vector2 referenceRes;

	private void Start()
	{
		referenceRes = desktopReferenceRes;
		defaultSpacing = layoutGroup.spacing;
	}

	public void FixAspectRatio()
	{
		layoutGroup.CalculateLayoutInputVertical();
		layoutGroup.spacing = defaultSpacing / referenceRes.y * (float)Screen.height;
		layoutGroup.CalculateLayoutInputVertical();
		for (int i = 0; i < elements.Count; i++)
		{
			elements[i].layoutGroup.CalculateLayoutInputHorizontal();
			LayoutGroupAspectFitterDef layoutGroupAspectFitterDef = elements[i];
			AdjustElement(layoutGroupAspectFitterDef);
			elements[i] = layoutGroupAspectFitterDef;
		}
	}

	private void AdjustElement(LayoutGroupAspectFitterDef layoutElement)
	{
		layoutElement.group.minHeight = (((RectTransform)layoutElement.group.transform).sizeDelta.x - (layoutElement.layoutGroup as HorizontalLayoutGroup).spacing) / groupAspectRatio;
		(layoutElement.layoutGroup as HorizontalLayoutGroup).spacing = defaultSpacing / referenceRes.y * (float)Screen.height;
		for (int i = 0; i < layoutElement.elements.Count; i++)
		{
			layoutElement.elements[i].minHeight = Mathf.Min(((RectTransform)layoutElement.elements[i].transform).sizeDelta.x / layoutElement.elementAspectRatio, layoutElement.group.minHeight);
		}
	}
}
