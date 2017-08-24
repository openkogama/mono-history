using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TextBubbleController : MonoBehaviour, IEventSystemHandler
{
	private class BubbleTracker
	{
		public float timeToLive;

		public float currentLifeTime;

		private TextBubble bubble;

		public TextBubble Bubble
		{
			get
			{
				return bubble;
			}
			set
			{
				if (bubble != null)
				{
					Debug.LogWarning("Bubble was not null");
					Debug.Log("bubble.gameObject.transform.parent.name " + bubble.gameObject.transform.parent.name);
					Object.Destroy(bubble.gameObject);
				}
				bubble = value;
			}
		}

		public void Clear()
		{
			Debug.Log("Clear");
			timeToLive = 0f;
			currentLifeTime = 0f;
			Bubble.SetTransparancy(0f);
			Bubble = null;
		}
	}

	private const int maxNumOfBubbles = 40;

	[SerializeField]
	[Header("Configuration")]
	private float bubbleLifeTime = 2f;

	[SerializeField]
	private float bubbleFadeIn = 0.2f;

	[SerializeField]
	[Header("Dependencies")]
	private TextBubble textBubblePrefab;

	[SerializeField]
	private Text textPrefab;

	private Dictionary<int, BubbleTracker> textBubbles = new Dictionary<int, BubbleTracker>();

	private int currentBubbleId;

	private List<int> removeList = new List<int>();

	public int ShowBubble3D(Vector3 worldPosition, string text, float lifeTime, Transform parentTransform, int bubbleId, bool positionUpdate = true, bool contentUpdate = false)
	{
		Text text2 = Object.Instantiate(textPrefab);
		text2.text = text;
		return ShowBubble3D(worldPosition, lifeTime, new List<RectTransform> { text2.rectTransform }, parentTransform, Vector2.one * 0.1f);
	}

	public int ShowBubble3D(Vector3 worldPosition, float lifeTime, List<RectTransform> content, Transform parentTransform, Vector2 offset)
	{
		Vector3 vector = Camera.main.WorldToScreenPoint(worldPosition);
		vector += CalculateOffSet(offset, vector);
		return ShowBubble2D(vector, Camera.main.WorldToScreenPoint(worldPosition) * 2f, lifeTime, content, parentTransform);
	}

	public void UpdatePosition3D(int bubbleId, Vector3 worldPosition, Vector2 offset)
	{
		Vector3 vector = Camera.main.WorldToScreenPoint(worldPosition);
		if (!(vector.z <= 0f))
		{
			vector += CalculateOffSet(offset, vector);
			UpdatePosition(bubbleId, vector, Camera.main.WorldToScreenPoint(worldPosition) * 2f);
		}
	}

	private Vector3 CalculateOffSet(Vector2 offset, Vector3 screenSpacePos)
	{
		Vector3 vector = new Vector2(Screen.width, Screen.height) / 2f;
		Vector3 vector2 = vector - screenSpacePos;
		if (Mathf.Abs(vector2.x) > Mathf.Abs(vector2.y))
		{
			return new Vector3(offset.x * Mathf.Sign(vector2.x), 0f);
		}
		return new Vector3(0f, offset.y * Mathf.Sign(vector2.y));
	}

	public int ShowBubble2D(Vector2 anchoredPosition, Vector2 targetCenterPoint, float lifeTime, List<RectTransform> content, Transform parentTransform)
	{
		Debug.Log("Show bubble 2d");
		int num = currentBubbleId;
		currentBubbleId++;
		TextBubble textBubble = Object.Instantiate(textBubblePrefab);
		for (int i = 0; i < content.Count; i++)
		{
			textBubble.Add(Object.Instantiate(content[i]));
		}
		textBubble.Initialize(targetCenterPoint, num);
		textBubble.Position = anchoredPosition;
		if (parentTransform != null)
		{
			textBubble.gameObject.transform.SetParent(parentTransform, worldPositionStays: true);
		}
		else
		{
			textBubble.gameObject.transform.SetParent(transform, worldPositionStays: false);
		}
		textBubbles.Add(num, new BubbleTracker());
		textBubbles[num].Bubble = textBubble;
		textBubbles[num].currentLifeTime = 0f;
		textBubbles[num].timeToLive = lifeTime;
		return num;
	}

	public void UpdateContent(int bubbleId, List<RectTransform> content)
	{
		TextBubble bubble = textBubbles[bubbleId].Bubble;
		bubble.ClearContent();
		for (int i = 0; i < content.Count; i++)
		{
			bubble.Add(Object.Instantiate(content[i]));
		}
	}

	public void UpdatePosition(int bubbleId, Vector2 anchoredPosition, Vector2 targetCenterPoint)
	{
		textBubbles[bubbleId].Bubble.Initialize(targetCenterPoint, bubbleId);
		textBubbles[bubbleId].Bubble.Position = anchoredPosition;
	}

	public void AddFirstElement(int bubbleId, RectTransform element)
	{
		TextBubble bubble = textBubbles[bubbleId].Bubble;
		bubble.Add(element);
		element.SetAsFirstSibling();
	}

	public void AddElement(int bubbleId, RectTransform element)
	{
		TextBubble bubble = textBubbles[bubbleId].Bubble;
		bubble.Add(element);
	}

	public void ClearBubblesWithId(int bubbleId)
	{
		textBubbles[bubbleId].timeToLive = bubbleLifeTime;
	}

	public void ClearBubblesOfTypeImmediately(int bubbleId)
	{
		textBubbles[bubbleId].timeToLive = 0f;
		UpdateBubble(textBubbles[bubbleId]);
		textBubbles[bubbleId].Bubble.OnRemoved();
		textBubbles[bubbleId].Clear();
		textBubbles.Remove(bubbleId);
	}

	protected void Update()
	{
		foreach (KeyValuePair<int, BubbleTracker> textBubble in textBubbles)
		{
			if (UpdateBubble(textBubble.Value))
			{
				removeList.Add(textBubble.Key);
			}
		}
		for (int num = removeList.Count - 1; num >= 0; num--)
		{
			textBubbles[removeList[num]].Bubble.OnRemoved();
			textBubbles[removeList[num]].Clear();
			textBubbles.Remove(removeList[num]);
		}
		removeList.Clear();
	}

	private bool UpdateBubble(BubbleTracker bubble)
	{
		bubble.currentLifeTime += Time.deltaTime;
		bubble.timeToLive -= Time.deltaTime;
		float transparancy = bubble.timeToLive / bubbleLifeTime;
		if (bubble.currentLifeTime <= bubbleFadeIn)
		{
			transparancy = bubble.currentLifeTime / bubbleFadeIn;
		}
		bubble.Bubble.SetTransparancy(transparancy);
		if (bubble.timeToLive <= 0f)
		{
			Debug.Log("Time out");
			return true;
		}
		return false;
	}
}
