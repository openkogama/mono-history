using UnityEngine;

[RequireComponent(typeof(Transform))]
[ExecuteInEditMode]
public class ChatAnchor : MonoBehaviour
{
	private const float screenEdgeOffset = 40f;

	[Tooltip("Chat Bubble that should be anchored at this transform.")]
	public ChatBubble AttachedBubble;

	[Tooltip("Radius in world units from the anchor transform to the bubble's pivot.")]
	public float AttachedRadius = 1f;

	[Tooltip("Angle in degrees around the transform to the bubble's pivot.")]
	[Range(-180f, 180f)]
	public float AttachedAngle = 90f;

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

	private void OnDestroy()
	{
		if (AttachedBubble != null)
		{
			Object.Destroy(AttachedBubble.gameObject);
		}
	}

	private void Update()
	{
		if ((bool)AttachedBubble)
		{
			if (isLocal && avatar.mvAvatar.CurrentPickup != null && avatar.mvAvatar.CurrentPickup.IsInFirstPersonMode)
			{
				AttachedBubble.SetChatBubbleVisibility(shouldBeVisible: false);
			}
			else if (snapTracking)
			{
				snapTracking = false;
			}
		}
	}

	public void UpdateAttachedBubblePosition()
	{
		if ((bool)AttachedBubble)
		{
			if (!Application.isPlaying)
			{
				snapTracking = true;
			}
			Camera mainCamera = MVGameControllerBase.CameraController.MainCamera;
			Vector3 position = transform.position;
			position = mainCamera.WorldToScreenPoint(position);
			position.z = AttachedBubble.rectTransform.position.z;
			position = HandleOfScreenChatBubble(mainCamera, position);
			InterpolateToNewBubblePosition(position);
		}
	}

	private Vector3 HandleOfScreenChatBubble(Camera camera, Vector3 adjustedPosition)
	{
		float num = AttachedBubble.rectTransform.rect.width * AttachedBubble.rectTransform.lossyScale.x * AttachedBubble.rectTransform.pivot.x + 40f;
		float num2 = (float)camera.pixelWidth - AttachedBubble.rectTransform.rect.width * AttachedBubble.rectTransform.lossyScale.x * (1f - AttachedBubble.rectTransform.pivot.x) - 40f;
		float num3 = AttachedBubble.rectTransform.rect.height * AttachedBubble.rectTransform.lossyScale.y * AttachedBubble.rectTransform.pivot.y + 40f;
		float num4 = (float)camera.pixelHeight - AttachedBubble.rectTransform.rect.height * AttachedBubble.rectTransform.lossyScale.y * (1f - AttachedBubble.rectTransform.pivot.y) - 40f;
		if (KeepInView)
		{
			adjustedPosition.x = Mathf.Clamp(adjustedPosition.x, num, num2);
			adjustedPosition.y = Mathf.Clamp(adjustedPosition.y, num3, num4);
		}
		Vector3 vector = camera.WorldToViewportPoint(transform.position);
		if (!(vector.z > 0f) || !(vector.x > 0f) || !(vector.x < 1f) || !(vector.y > 0f) || !(vector.y < 1f))
		{
			switch (AttachedBubble.ExtenderDock)
			{
			case ExtenderBorderEnum.Bottom:
				adjustedPosition.y = num3;
				break;
			case ExtenderBorderEnum.Left:
				adjustedPosition.x = num;
				break;
			case ExtenderBorderEnum.Right:
				adjustedPosition.x = num2;
				break;
			case ExtenderBorderEnum.Top:
				adjustedPosition.y = num4;
				break;
			}
		}
		return adjustedPosition;
	}

	private void InterpolateToNewBubblePosition(Vector3 adjustedPosition)
	{
		previousAdjustedPosition.z = adjustedPosition.z;
		if (adjustedPosition != previousAdjustedPosition)
		{
			currentInterpolationProgress = 0f;
		}
		currentInterpolationProgress += Time.deltaTime * TrackingSpeed;
		AttachedBubble.rectTransform.position = Vector3.Lerp(AttachedBubble.rectTransform.position, adjustedPosition, (!snapTracking) ? currentInterpolationProgress : 1f);
		AttachedBubble.rectTransform.rotation = AttachedBubble.rectTransform.rotation;
		previousAdjustedPosition = adjustedPosition;
	}

	public void SkipInterpolation()
	{
		snapTracking = true;
	}

	public void HideChatBubble()
	{
		if (AttachedBubble != null)
		{
			AttachedBubble.HideBubble();
		}
	}
}
