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

	private float defaultSpacing = 20f;

	private void Start()
	{
		defaultSpacing = layoutGroup.spacing;
	}

	public void FixAspectRatio()
	{
		layoutGroup.CalculateLayoutInputVertical();
		layoutGroup.spacing = defaultSpacing / 1440f * (float)Screen.height;
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
		(layoutElement.layoutGroup as HorizontalLayoutGroup).spacing = defaultSpacing / 1440f * (float)Screen.height;
		for (int i = 0; i < layoutElement.elements.Count; i++)
		{
			layoutElement.elements[i].minHeight = Mathf.Min(((RectTransform)layoutElement.elements[i].transform).sizeDelta.x / layoutElement.elementAspectRatio, layoutElement.group.minHeight);
		}
	}
}
