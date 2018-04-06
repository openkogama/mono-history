using UnityEngine;
using UnityEngine.UI;

public class TeamMenuLayoutFitter : MonoBehaviour
{
	private const int maxTeamCount = 4;

	[SerializeField]
	private VerticalLayoutGroup layoutGroup;

	private void Start()
	{
		FixLayout();
	}

	private void FixLayout()
	{
		LayoutElement[] componentsInChildren = GetComponentsInChildren<LayoutElement>();
		layoutGroup.CalculateLayoutInputVertical();
		float num = ((RectTransform)transform).rect.height / 4f;
		layoutGroup.spacing = layoutGroup.spacing / 1440f * (float)Screen.height;
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].minHeight = num;
			componentsInChildren[i].preferredHeight = num;
			layoutGroup.CalculateLayoutInputVertical();
		}
	}
}
