using UnityEngine;
using UnityEngine.UI;

namespace LivelyChatBubbles;

[RequireComponent(typeof(RectTransform))]
[ExecuteInEditMode]
public class ChatBubble : MonoBehaviour
{
	private const float baseFadeWaitTime = 5f;

	private const float charactersPerSecond = 30f;

	private const int characerLimit = 130;

	[Tooltip("Text component in the tree used to display the bubble's message.")]
	public Text MessageComponent;

	[Multiline]
	public string MessageValue;

	[Tooltip("True if the bubble should be autosized according to the message content.")]
	public bool AutoSize;

	[Tooltip("Minimum size required.")]
	public Vector2 MessageMinimumSize = new Vector2(60f, 30f);

	[Tooltip("Maximum width before wrapping.")]
	public float MessageWrapWidth = 300f;

	[Tooltip("The image should be aligned to the top without any empty pixel rows/columns around it.")]
	public Image ExtenderComponent;

	[Tooltip("Configurable borders that define where the extender can travel.")]
	public ExtenderBorderInfo[] ExtenderBorderInfo;

	[Tooltip("Border to dock the extender to.")]
	public ExtenderBorderEnum ExtenderDock;

	[Tooltip("Canvas group")]
	public CanvasGroup CanvasGroup;

	[Tooltip("Offset of the extender along the docked edge.")]
	[Range(0f, 1f)]
	public float ExtenderPosition = 0.5f;

	private bool isActive = true;

	private RectTransform _rectTransform;

	private ChatAnchor anchor;

	private float currentFade;

	private float timeUntilFade;

	public static bool BindingsExpanded;

	public static bool ValuesExpanded;

	public static bool ExtenderExpanded;

	public bool IsActive => isActive;

	public RectTransform rectTransform
	{
		get
		{
			if (!_rectTransform)
			{
				_rectTransform = GetComponent<RectTransform>();
			}
			return _rectTransform;
		}
	}

	public ChatAnchor Anchor
	{
		set
		{
			anchor = value;
		}
	}

	public bool BindMessageValue(string value)
	{
		if (isActive)
		{
			if (value.Length > 130)
			{
				value = value.Substring(0, 130) + "...";
			}
			MessageValue = value;
			if (!MessageComponent || MessageComponent.text == value)
			{
				return false;
			}
			MessageComponent.text = value;
			if (AutoSize)
			{
				PerformAutoSize();
			}
			SetChatBubbleVisibility(shouldBeVisible: true);
			if (anchor != null)
			{
				anchor.SkipInterpolation();
				anchor.UpdateAttachedBubblePosition();
			}
			return true;
		}
		return false;
	}

	public bool BindExtenderDock(ExtenderBorderEnum value)
	{
		if (ExtenderDock == value)
		{
			return false;
		}
		ExtenderDock = value;
		PerformExtenderSnap();
		PerformExtenderPosition();
		return true;
	}

	public bool BindExtenderPosition(float value)
	{
		value = Mathf.Clamp01(value);
		if (ExtenderPosition == value)
		{
			return false;
		}
		ExtenderPosition = value;
		PerformExtenderPosition();
		return true;
	}

	private void Awake()
	{
		if (ExtenderBorderInfo == null)
		{
			ExtenderBorderInfo = new ExtenderBorderInfo[4];
			ExtenderBorderInfo[0] = new ExtenderBorderInfo
			{
				Border = ExtenderBorderEnum.Bottom
			};
			ExtenderBorderInfo[1] = new ExtenderBorderInfo
			{
				Border = ExtenderBorderEnum.Left
			};
			ExtenderBorderInfo[2] = new ExtenderBorderInfo
			{
				Border = ExtenderBorderEnum.Right
			};
			ExtenderBorderInfo[3] = new ExtenderBorderInfo
			{
				Border = ExtenderBorderEnum.Top
			};
		}
	}

	private void OnEnable()
	{
		isActive = true;
	}

	private void OnDisable()
	{
		isActive = false;
	}

	private void Update()
	{
		PerformExtenderPosition();
		if (anchor != null)
		{
			anchor.UpdateAttachedBubblePosition();
		}
		UpdateFading();
	}

	private void UpdateFading()
	{
		if (timeUntilFade < Time.time)
		{
			currentFade -= Time.deltaTime;
			CanvasGroup.alpha = currentFade;
		}
	}

	public void SetChatBubbleVisibility(bool shouldBeVisible)
	{
		if (shouldBeVisible)
		{
			currentFade = 1f;
			CanvasGroup.alpha = currentFade;
			float num = Time.time + 5f + (float)MessageValue.Length / 30f;
			if (timeUntilFade < num)
			{
				timeUntilFade = num;
			}
		}
		else
		{
			currentFade = 0f;
			CanvasGroup.alpha = currentFade;
			timeUntilFade = 0f;
		}
	}

	public Vector2 PerformManualSize(string value)
	{
		Vector2 messageMinimumSize = MessageMinimumSize;
		if ((bool)MessageComponent)
		{
			string text = MessageComponent.text;
			MessageComponent.text = value;
			PerformAutoSize();
			MessageMinimumSize = MessageComponent.rectTransform.rect.size;
			MessageComponent.text = text;
		}
		return messageMinimumSize;
	}

	public void PerformAutoSize()
	{
		if ((bool)MessageComponent)
		{
			PerformAutoHeight();
			if (PerformAutoWidth())
			{
				PerformAutoHeight();
			}
		}
	}

	private bool PerformAutoHeight()
	{
		float height = MessageComponent.rectTransform.rect.height;
		float num = Mathf.Max(MessageMinimumSize.y, LayoutUtility.GetPreferredHeight(MessageComponent.rectTransform));
		if (height == num)
		{
			return false;
		}
		rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rectTransform.rect.height + (num - height));
		return true;
	}

	private bool PerformAutoWidth()
	{
		float width = MessageComponent.rectTransform.rect.width;
		float num = Mathf.Max(MessageMinimumSize.x, Mathf.Min(MessageWrapWidth, LayoutUtility.GetPreferredWidth(MessageComponent.rectTransform)));
		if (width == num)
		{
			return false;
		}
		rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rectTransform.rect.width + (num - width));
		return true;
	}

	public void PerformExtenderSnap()
	{
		if ((bool)ExtenderComponent)
		{
			ExtenderComponent.rectTransform.pivot = new Vector2(0.5f, 1f);
			switch (ExtenderDock)
			{
			case ExtenderBorderEnum.Bottom:
			{
				RectTransform rectTransform4 = ExtenderComponent.rectTransform;
				Vector2 vector = new Vector2(0.5f, 0f);
				ExtenderComponent.rectTransform.anchorMax = vector;
				rectTransform4.anchorMin = vector;
				ExtenderComponent.rectTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
				break;
			}
			case ExtenderBorderEnum.Left:
			{
				RectTransform rectTransform3 = ExtenderComponent.rectTransform;
				Vector2 vector = new Vector2(0f, 0.5f);
				ExtenderComponent.rectTransform.anchorMax = vector;
				rectTransform3.anchorMin = vector;
				ExtenderComponent.rectTransform.localRotation = Quaternion.Euler(0f, 0f, -90f);
				break;
			}
			case ExtenderBorderEnum.Right:
			{
				RectTransform rectTransform2 = ExtenderComponent.rectTransform;
				Vector2 vector = new Vector2(1f, 0.5f);
				ExtenderComponent.rectTransform.anchorMax = vector;
				rectTransform2.anchorMin = vector;
				ExtenderComponent.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 90f);
				break;
			}
			case ExtenderBorderEnum.Top:
			{
				RectTransform rectTransform = ExtenderComponent.rectTransform;
				Vector2 vector = new Vector2(0.5f, 1f);
				ExtenderComponent.rectTransform.anchorMax = vector;
				rectTransform.anchorMin = vector;
				ExtenderComponent.rectTransform.rotation = Quaternion.Euler(0f, 0f, 180f);
				break;
			}
			}
			if (ExtenderComponent.enabled && !ExtenderBorderInfo[(int)ExtenderDock].Enabled)
			{
				ExtenderComponent.enabled = false;
			}
			else if (!ExtenderComponent.enabled && ExtenderBorderInfo[(int)ExtenderDock].Enabled)
			{
				ExtenderComponent.enabled = true;
			}
		}
	}

	public void PerformExtenderPosition()
	{
		if (!ExtenderComponent)
		{
			return;
		}
		Camera main = Camera.main;
		if (main != null && anchor != null)
		{
			if (main.WorldToViewportPoint(anchor.transform.position).z >= 0f)
			{
				BindExtenderDock(ExtenderBorderEnum.Bottom);
			}
			else
			{
				Vector3 position = main.transform.position;
				float sqrMagnitude = (position + main.transform.up).sqrMagnitude;
				float sqrMagnitude2 = (position + main.transform.up * -1f).sqrMagnitude;
				float num;
				ExtenderBorderEnum value;
				if (sqrMagnitude < sqrMagnitude2)
				{
					num = sqrMagnitude;
					value = ExtenderBorderEnum.Top;
				}
				else
				{
					num = sqrMagnitude2;
					value = ExtenderBorderEnum.Bottom;
				}
				float sqrMagnitude3 = (position + main.transform.right).sqrMagnitude;
				float sqrMagnitude4 = (position + main.transform.right * -1f).sqrMagnitude;
				float num2;
				ExtenderBorderEnum value2;
				if (sqrMagnitude3 < sqrMagnitude4)
				{
					num2 = sqrMagnitude3;
					value2 = ExtenderBorderEnum.Right;
				}
				else
				{
					num2 = sqrMagnitude4;
					value2 = ExtenderBorderEnum.Left;
				}
				if (num2 < num)
				{
					BindExtenderDock(value2);
				}
				else
				{
					BindExtenderDock(value);
				}
			}
		}
		Vector3 v = Vector3.zero;
		Vector3 v2 = Vector3.zero;
		ExtenderBorderInfo extenderBorderInfo = ExtenderBorderInfo[(int)ExtenderDock];
		CalculateExtenderBorderVertices(extenderBorderInfo, ref v, ref v2);
		switch (ExtenderDock)
		{
		case ExtenderBorderEnum.Bottom:
			ExtenderComponent.rectTransform.anchoredPosition = new Vector3(ExtenderComponent.rectTransform.anchoredPosition.x, extenderBorderInfo.Margin, 0f);
			break;
		case ExtenderBorderEnum.Left:
			ExtenderComponent.rectTransform.anchoredPosition = new Vector3(extenderBorderInfo.Margin, ExtenderComponent.rectTransform.anchoredPosition.y, 0f);
			break;
		case ExtenderBorderEnum.Right:
			ExtenderComponent.rectTransform.anchoredPosition = new Vector3(0f - extenderBorderInfo.Margin, ExtenderComponent.rectTransform.anchoredPosition.y, 0f);
			break;
		case ExtenderBorderEnum.Top:
			ExtenderComponent.rectTransform.anchoredPosition = new Vector3(ExtenderComponent.rectTransform.anchoredPosition.x, 0f - extenderBorderInfo.Margin, 0f);
			break;
		}
	}

	public void CalculateExtenderBorderVertices(ExtenderBorderInfo info, ref Vector3 v1, ref Vector3 v2)
	{
		v1.z = (v2.z = rectTransform.localPosition.z);
		Vector2 vector = rectTransform.sizeDelta / 2f;
		switch (info.Border)
		{
		case ExtenderBorderEnum.Bottom:
			v1.x = 0f - vector.x + info.CutoffNear;
			v2.x = vector.x - info.CutoffFar;
			v1.y = (v2.y = 0f - vector.y + info.Margin);
			break;
		case ExtenderBorderEnum.Left:
			v1.y = 0f - vector.y + info.CutoffNear;
			v2.y = vector.y - info.CutoffFar;
			v1.x = (v2.x = 0f - vector.x + info.Margin);
			break;
		case ExtenderBorderEnum.Right:
			v1.y = 0f - vector.y + info.CutoffNear;
			v2.y = vector.y - info.CutoffFar;
			v1.x = (v2.x = vector.x - info.Margin);
			break;
		case ExtenderBorderEnum.Top:
			v1.x = 0f - vector.x + info.CutoffNear;
			v2.x = vector.x - info.CutoffFar;
			v1.y = (v2.y = vector.y - info.Margin);
			break;
		}
	}
}
