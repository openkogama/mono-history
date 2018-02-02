using UnityEngine;

namespace LivelyChatBubbles;

[ExecuteInEditMode]
[RequireComponent(typeof(Transform))]
public class ChatAnchor : MonoBehaviour
{
	private const float screenEdgeOffset = 40f;

	[Tooltip("Chat Bubble that should be anchored at this transform.")]
	public ChatBubble AttachedBubble;

	[Tooltip("Radius in world units from the anchor transform to the bubble's pivot.")]
	public float AttachedRadius = 1f;

	[Range(-180f, 180f)]
	[Tooltip("Angle in degrees around the transform to the bubble's pivot.")]
	public float AttachedAngle = 90f;

	[Range(0f, 1f)]
	[Tooltip("Percentage to influence the bubble's angle towards the center of the screen.")]
	public float CenterInfluence;

	[Tooltip("Smoothing speed as the bubble follows the anchor transform.")]
	public float TrackingSpeed = 15f;

	[Tooltip("True if the bubble should stay within the screen bounds until the anchor position is no longer visible.")]
	public bool KeepInView = true;

	private bool snapTracking;

	private bool inViewport;

	private bool isLocal;

	private Avatar avatar;

	private float currentInterpolationProgress;

	private Vector3 previousAdjustedPosition;

	public bool BindAttachedBubble(ChatBubble value)
	{
		if (AttachedBubble == value)
		{
			return false;
		}
		AttachedBubble = value;
		AttachedBubble.Anchor = this;
		snapTracking = true;
		return true;
	}

	public void Initialize(bool isLocal, Avatar avatar)
	{
		this.isLocal = isLocal;
		this.avatar = avatar;
	}

	private void OnEnable()
	{
		snapTracking = true;
	}

	private void OnValidate()
	{
		if (AttachedRadius < 0f)
		{
			AttachedRadius = 0f;
		}
	}

	private void Update()
	{
		if (!AttachedBubble)
		{
			return;
		}
		if (!Application.isPlaying)
		{
			snapTracking = true;
		}
		if (isLocal && avatar.mvAvatar.CurrentPickup != null && avatar.mvAvatar.CurrentPickup.IsInFirstPersonMode)
		{
			AttachedBubble.SetChatBubbleVisibility(shouldBeVisible: false);
			return;
		}
		Camera main = Camera.main;
		Vector3 vector = new Vector3(main.pixelWidth / 2, main.pixelHeight / 2, 0f);
		float attachedAngle = AttachedAngle;
		if (CenterInfluence > 0f)
		{
			Vector3 vector2 = vector - main.WorldToScreenPoint(transform.position);
			float b = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f;
			attachedAngle = Mathf.LerpAngle(attachedAngle, b, vector2.magnitude / Mathf.Min(vector.x, vector.y) * CenterInfluence);
		}
		if (snapTracking)
		{
			snapTracking = false;
		}
		if (!AttachedBubble.ExtenderComponent)
		{
			return;
		}
		if (AttachedRadius == 0f)
		{
			AttachedBubble.BindExtenderDock(ExtenderBorderEnum.Bottom);
			AttachedBubble.BindExtenderPosition(0f);
			return;
		}
		Vector2 pivot = AttachedBubble.rectTransform.pivot;
		if ((double)pivot.x <= 0.1464466)
		{
			AttachedBubble.BindExtenderDock(ExtenderBorderEnum.Left);
		}
		else if ((double)pivot.x >= 0.8535534)
		{
			AttachedBubble.BindExtenderDock(ExtenderBorderEnum.Right);
		}
		else if ((double)pivot.y <= 0.1464466)
		{
			AttachedBubble.BindExtenderDock(ExtenderBorderEnum.Bottom);
		}
		else if ((double)pivot.y >= 0.8535534)
		{
			AttachedBubble.BindExtenderDock(ExtenderBorderEnum.Top);
		}
		Vector3 vector3 = main.WorldToScreenPoint(transform.position);
		Vector3 position = AttachedBubble.rectTransform.position;
		Rect rect = AttachedBubble.rectTransform.rect;
		ExtenderBorderInfo extenderBorderInfo = AttachedBubble.ExtenderBorderInfo[(int)AttachedBubble.ExtenderDock];
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		switch (extenderBorderInfo.Border)
		{
		case ExtenderBorderEnum.Bottom:
		case ExtenderBorderEnum.Top:
			num = position.x + rect.xMin + extenderBorderInfo.CutoffNear;
			num2 = position.x + rect.xMax - extenderBorderInfo.CutoffFar;
			num3 = vector3.x.Remap(num, num2, 0f, 1f);
			AttachedBubble.BindExtenderPosition(num3);
			break;
		case ExtenderBorderEnum.Left:
		case ExtenderBorderEnum.Right:
			num = position.y + rect.yMin + extenderBorderInfo.CutoffNear;
			num2 = position.y + rect.yMax - extenderBorderInfo.CutoffFar;
			num3 = vector3.y.Remap(num, num2, 0f, 1f);
			AttachedBubble.BindExtenderPosition(num3);
			break;
		}
	}

	public void UpdateAttachedBubblePosition()
	{
		if (!AttachedBubble)
		{
			return;
		}
		if (!Application.isPlaying)
		{
			snapTracking = true;
		}
		Camera mainCamera = MVGameControllerBase.CameraController.MainCamera;
		Vector3 vector = new Vector3(mainCamera.pixelWidth / 2, mainCamera.pixelHeight / 2, 0f);
		Vector3 vector2 = mainCamera.WorldToViewportPoint(transform.position);
		float attachedAngle = AttachedAngle;
		if (CenterInfluence > 0f)
		{
			Vector3 vector3 = vector - mainCamera.WorldToScreenPoint(transform.position);
			float b = Mathf.Atan2(vector3.y, vector3.x) * 57.29578f;
			attachedAngle = Mathf.LerpAngle(attachedAngle, b, vector3.magnitude / Mathf.Min(vector.x, vector.y) * CenterInfluence);
		}
		Vector3 position = transform.position;
		position = mainCamera.WorldToScreenPoint(position);
		position.z = AttachedBubble.rectTransform.position.z;
		float num = AttachedBubble.rectTransform.rect.width * AttachedBubble.rectTransform.lossyScale.x * AttachedBubble.rectTransform.pivot.x + 40f;
		float num2 = (float)mainCamera.pixelWidth - AttachedBubble.rectTransform.rect.width * AttachedBubble.rectTransform.lossyScale.x * (1f - AttachedBubble.rectTransform.pivot.x) - 40f;
		float num3 = AttachedBubble.rectTransform.rect.height * AttachedBubble.rectTransform.lossyScale.y * AttachedBubble.rectTransform.pivot.y + 40f;
		float num4 = (float)mainCamera.pixelHeight - AttachedBubble.rectTransform.rect.height * AttachedBubble.rectTransform.lossyScale.y * (1f - AttachedBubble.rectTransform.pivot.y) - 40f;
		if (KeepInView)
		{
			position.x = Mathf.Clamp(position.x, num, num2);
			position.y = Mathf.Clamp(position.y, num3, num4);
		}
		if (vector2.z < 0f)
		{
			switch (AttachedBubble.ExtenderDock)
			{
			case ExtenderBorderEnum.Bottom:
				position.y = num3;
				break;
			case ExtenderBorderEnum.Left:
				position.x = num;
				break;
			case ExtenderBorderEnum.Right:
				position.x = num2;
				break;
			case ExtenderBorderEnum.Top:
				position.y = num4;
				break;
			}
		}
		previousAdjustedPosition.z = position.z;
		if (position != previousAdjustedPosition)
		{
			currentInterpolationProgress = 0f;
		}
		currentInterpolationProgress += Time.deltaTime * TrackingSpeed;
		AttachedBubble.rectTransform.position = Vector3.Lerp(AttachedBubble.rectTransform.position, position, (!snapTracking) ? currentInterpolationProgress : 1f);
		AttachedBubble.rectTransform.rotation = AttachedBubble.rectTransform.rotation;
		previousAdjustedPosition = position;
	}

	public void SkipInterpolation()
	{
		snapTracking = true;
	}
}
