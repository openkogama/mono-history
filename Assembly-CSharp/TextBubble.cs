using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TextBubble : MonoBehaviour
{
	[Header("Dependencies")]
	[SerializeField]
	private LayoutGroup bubble;

	[SerializeField]
	private RectTransform tail;

	[SerializeField]
	private CanvasGroup fadeGroup;

	[NonSerialized]
	public new Transform transform;

	private List<UnityEngine.Object> content = new List<UnityEngine.Object>();

	[SerializeField]
	private Vector2 centerPoint;

	private int bubbleId = -1;

	public Vector2 Position
	{
		private get
		{
			return tail.position;
		}
		set
		{
			tail.position = value;
			RecalcPositionWithScreenCollision();
		}
	}

	private RectTransform BubbleTransform => (RectTransform)bubble.transform;

	private float HorizontalPadding => bubble.padding.left + bubble.padding.right;

	private float VerticalPadding => bubble.padding.top + bubble.padding.bottom;

	private void Start()
	{
		SetTransparancy(0f);
	}

	private void OnDestroy()
	{
		if (bubbleId != -1)
		{
			Debug.LogWarning("Bubble not removed from controller");
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (TextBubbleController x, BaseEventData y) =>
			{
				x.ClearBubblesOfTypeImmediately(bubbleId);
			});
		}
	}

	public void Initialize(Vector2 center, int bubbleId)
	{
		this.bubbleId = bubbleId;
		centerPoint = center;
	}

	public void OnRemoved()
	{
		Debug.Log("OnRemoved");
		bubbleId = -1;
	}

	public void Add(RectTransform transformContent)
	{
		content.Add(transformContent.gameObject);
		transformContent.SetParent(BubbleTransform, worldPositionStays: false);
	}

	public void ClearContent()
	{
		for (int i = 0; i < content.Count; i++)
		{
			UnityEngine.Object.Destroy(content[i]);
		}
		content.Clear();
	}

	private void RecalcPositionWithScreenCollision()
	{
		RecalcPositionAndSize(1);
		Rect rect = new Rect(0f, 0f, Screen.width, Screen.height);
		Vector3[] array = new Vector3[4];
		BubbleTransform.GetWorldCorners(array);
		if (!rect.Contains(array[0]) || !rect.Contains(array[1]) || !rect.Contains(array[2]) || !rect.Contains(array[3]))
		{
			RecalcPositionAndSize(-1);
		}
	}

	public void RecalcPositionAndSize(int inside)
	{
		Vector2 vector = centerPoint / 2f - (Vector2)tail.position;
		Vector2 vector2;
		if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
		{
			int num = ((!(vector.x > 0f)) ? 1 : (-1));
			tail.up = new Vector3(num, 0f, 0f);
			float num2 = Position.y / (float)Screen.height;
			float y = CalculatePivotNearEdgeOffset(num2);
			vector2 = new Vector2(tail.rect.height * (float)num, y);
			BubbleTransform.pivot = new Vector2((num <= 0) ? 1 : 0, num2);
			BubbleTransform.localPosition = (Vector2)tail.localPosition + vector2;
		}
		else
		{
			float num3 = ((!(vector.y > 0f)) ? 1 : (-1));
			tail.up = new Vector3(0f, num3, 0f);
			float num4 = Position.x / (float)Screen.width;
			float num5 = CalculatePivotNearEdgeOffset(num4);
			vector2 = new Vector2(tail.rect.height * num3, num5);
			vector2 = new Vector2(num5, tail.rect.height * num3);
			BubbleTransform.pivot = new Vector2(num4, (!(num3 > 0f)) ? 1 : 0);
			BubbleTransform.localPosition = (Vector2)tail.localPosition + vector2;
		}
	}

	public void SetTransparancy(float a)
	{
		fadeGroup.alpha = a;
	}

	protected void OnValidate()
	{
		((RectTransform)base.transform).anchoredPosition = Vector2.zero;
	}

	private float CalculatePivotNearEdgeOffset(float pivot)
	{
		float result = 0f;
		float num = ((pivot > 0.5f) ? 1 : (-1));
		float num2 = (pivot - 0.5f) * num;
		if (num2 > 0.25f)
		{
			result = tail.rect.height * (0.25f / num2) * num;
		}
		return result;
	}
}
