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

	private EditorStateMachine editorStateMachine;

	public void Initialize(int woID, Vector3 worldPosition, EditorStateMachine esm)
	{
		this.woID = woID;
		editorStateMachine = esm;
		this.worldPosition = worldPosition;
		Setup(woID);
		SetToScreenPoint();
	}

	private void Update()
	{
		SetToScreenPoint();
		HandleCloningHotkey();
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

	private void HandleCloningHotkey()
	{
		if (Input.GetKeyUp(KeyCode.Q))
		{
			if (CanClone())
			{
				Clone();
			}
			else if (CanCloneRoot())
			{
				CloneRoot();
			}
		}
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

	private void Clone()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.GameObjectUI);
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (ICloneHandler handler, BaseEventData data) =>
		{
			handler.Clone(MVGameControllerBase.WOCM.GetWorldObjectClient(woID), cloneToRoot: false, setAsPreviewItem: false);
		});
	}

	private void CloneRoot()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.GameObjectUI);
		});
		MVWorldObjectClient root = MVGameControllerBase.WOCM.GetWorldObjectClientRoot(woID);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (ICloneHandler handler, BaseEventData data) =>
		{
			handler.Clone(root, cloneToRoot: false, setAsPreviewItem: false);
		});
	}

	private bool CanClone()
	{
		foreach (MVWorldObjectClient selectedWO in editorStateMachine.SelectedWOs)
		{
			if (!selectedWO.HasInteractionFlag(InteractionFlags.CanClone) || selectedWO.HasInteractionFlag(InteractionFlags.IsPreview))
			{
				return false;
			}
		}
		return true;
	}

	private bool CanCloneRoot()
	{
		foreach (MVWorldObjectClient selectedWO in editorStateMachine.SelectedWOs)
		{
			if (!selectedWO.HasInteractionFlag(InteractionFlags.CanCloneRoot) || selectedWO.HasInteractionFlag(InteractionFlags.IsPreview))
			{
				return false;
			}
		}
		return true;
	}
}
