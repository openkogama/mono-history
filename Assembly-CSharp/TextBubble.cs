using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TextBubble : MonoBehaviour
{
	[SerializeField]
	[Header("Configuration")]
	private float tailSize = 0.25f;

	[SerializeField]
	private float tailDistance = 1f;

	[SerializeField]
	[Header("Dependencies")]
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
			return tail.anchoredPosition;
		}
		set
		{
			tail.anchoredPosition = value;
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
		Vector2 vector = centerPoint / 2f - tail.anchoredPosition;
		if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
		{
			float num = tailSize * VerticalPadding;
			tail.sizeDelta = new Vector2(num, num);
			int num2 = ((!(vector.x > 0f)) ? 1 : (-1));
			num2 *= inside;
			tail.up = new Vector3(num2, 0f, 0f);
			Vector2 vector2 = new Vector2(tail.rect.height - tail.pivot.y * tail.rect.height, 0f) * num2;
			float num3 = Position.y / (float)Screen.height;
			BubbleTransform.pivot = new Vector2((num2 <= 0) ? 1 : 0, num3);
			BubbleTransform.localPosition = tail.anchoredPosition + vector2;
			BubbleTransform.Translate(tail.up * tailDistance);
			float num4 = num3 - 0.5f;
			BubbleTransform.Translate(new Vector3(0f, num4 * num, 0f));
			BubbleTransform.Translate(new Vector3(0f, (float)((!(num4 > 0f)) ? bubble.padding.bottom : bubble.padding.top) * num4, 0f));
		}
		else
		{
			float num5 = tailSize * HorizontalPadding;
			tail.sizeDelta = new Vector2(num5, num5);
			float num6 = ((!(vector.y > 0f)) ? 1 : (-1));
			num6 *= (float)inside;
			tail.up = new Vector3(0f, num6, 0f);
			Vector2 vector2 = new Vector2(0f, tail.rect.height - tail.pivot.y * tail.rect.height) * num6;
			float num7 = Position.x / (float)Screen.width;
			BubbleTransform.pivot = new Vector2(num7, (!(num6 > 0f)) ? 1 : 0);
			BubbleTransform.localPosition = tail.anchoredPosition + vector2;
			BubbleTransform.Translate(tail.up * tailDistance);
			float num8 = num7 - 0.5f;
			BubbleTransform.Translate(new Vector3(num8 * num5, 0f, 0f));
			BubbleTransform.Translate(new Vector3((float)((!(num8 > 0f)) ? bubble.padding.right : bubble.padding.left) * num8, 0f, 0f));
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
}
