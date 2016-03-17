using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;

public class GizmoMenu : MonoBehaviour
{
	private Vector3 worldPosition;

	[SerializeField]
	private RectTransform rectTransform;

	[SerializeField]
	private GizmoButton rotate;

	[SerializeField]
	private GizmoButton xzTranslate;

	[SerializeField]
	private GizmoButton yTranslate;

	private int woID;

	public void Initialize(int woID, Vector3 worldPosition)
	{
		this.woID = woID;
		this.worldPosition = worldPosition;
		Setup(woID);
		SetToScreenPoint();
	}

	private void Update()
	{
		SetToScreenPoint();
	}

	private void Setup(int woID)
	{
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woID);
		MVGameControllerBase.WOCM.SubscribeWODestroyedEvent(woID, PopWoDestroyed);
		bool active = worldObjectClient.HasInteractionFlag(InteractionFlags.CanRotateX) || worldObjectClient.HasInteractionFlag(InteractionFlags.CanRotateY) || worldObjectClient.HasInteractionFlag(InteractionFlags.CanRotateZ);
		rotate.gameObject.SetActive(active);
		bool active2 = (!worldObjectClient.HasInteractionFlag(InteractionFlags.NotTranslatbleXZ) && MVGameControllerBase.Game.GameType != MVGameType.Platformer) || (worldObjectClient.HasInteractionFlag(InteractionFlags.TranslatbleXZ2D) && MVGameControllerBase.Game.GameType == MVGameType.Platformer);
		xzTranslate.gameObject.SetActive(active2);
		bool active3 = MVGameControllerBase.Game.GameType != MVGameType.Platformer && !worldObjectClient.HasInteractionFlag(InteractionFlags.NotTranslatbleY);
		yTranslate.gameObject.SetActive(active3);
	}

	private void SetToScreenPoint()
	{
		Vector3 position = MVGameControllerBase.CameraController.MainCamera.WorldToScreenPoint(worldPosition);
		rectTransform.transform.position = position;
	}

	private void LateUpdate()
	{
		if (MVInputWrapper.GetBooleanControlDown(KogamaControls.PointerSelect))
		{
			Pop();
		}
	}

	private void Pop()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.GameObjectUI);
		});
	}

	private void OnDestroy()
	{
		MVGameControllerBase.WOCM.UnsubscribeWODestroyedEvent(woID, PopWoDestroyed);
	}

	private void PopWoDestroyed(object obj, WorldObjectDestroyedEventArgs args)
	{
		Pop();
	}
}
