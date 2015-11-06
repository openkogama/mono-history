using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityStandardAssets.CrossPlatformInput;

public class FireButton : MonoBehaviour, IPointerUpHandler, IEventSystemHandler, IPointerDownHandler, IUpdatecontrollerSubscriber
{
	private enum State
	{
		NotActive,
		PointerDown,
		DragShooting,
		Shooting
	}

	private const float timeBeforeShooting = 0.06f;

	private const float angleDiffBeforeDragShooting = 3f;

	private const float angleDiffBeforeDragModeShooting = 0.7f;

	private const int maxDragPositionsInShootingMode = 5;

	private State state;

	private int pointerId = -1;

	private float pointerDownTime;

	private bool fired;

	private float xFactor;

	private readonly List<Vector3> firedDirections = new List<Vector3>();

	private readonly List<Vector3> dragPositions = new List<Vector3>();

	[SerializeField]
	private FireDirection fireDirection;

	[SerializeField]
	private string buttonName;

	private void Awake()
	{
		UpdateController.AddUpdateObject(this, UpdatePriority.PRE_UPDATEBUCKET_20);
	}

	private void OnEnable()
	{
		fireDirection.gameObject.SetActive(value: true);
		fireDirection.SetToDir(MVGameControllerBase.IPlayModeUI.GetCrossHair().Direction);
	}

	private void OnDisable()
	{
		fireDirection.gameObject.SetActive(value: false);
		Reset();
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (state == State.DragShooting && firedDirections.Count == 1)
		{
			Debug.Log("Set to firedirection");
			fireDirection.SetToFixedDir(firedDirections[firedDirections.Count - 1]);
		}
		if (!fired)
		{
			CrossPlatformInputManager.SetButtonDown(buttonName);
		}
		Reset();
	}

	private void Reset()
	{
		fired = false;
		state = State.NotActive;
		dragPositions.Clear();
		CrossPlatformInputManager.SetButtonUp(buttonName);
		firedDirections.Clear();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		dragPositions.Add(eventData.position);
		state = State.PointerDown;
		pointerId = eventData.pointerId;
		pointerDownTime = Time.time;
	}

	public void UpdateControllerUpdate()
	{
		Vector3 item = MVGameControllerBase.IPlayModeUI.GetCrossHair().Direction;
		if (state != State.NotActive)
		{
			dragPositions.Add(GetCurrentPointerPos());
		}
		switch (state)
		{
		case State.NotActive:
			HandleSnapToMoveDirection();
			return;
		case State.PointerDown:
			if (IsDragging())
			{
				state = State.DragShooting;
				UpdateControllerUpdate();
			}
			else if (Time.time - pointerDownTime > 0.06f)
			{
				state = State.Shooting;
			}
			break;
		case State.DragShooting:
			fireDirection.SetToFixedDir(dragPositions[dragPositions.Count - 1] - dragPositions[0]);
			item = dragPositions[dragPositions.Count - 1] - dragPositions[0];
			if (Time.time - pointerDownTime > 0.06f || AngleDiff(dragPositions[0], dragPositions[dragPositions.Count - 1]) > 3f)
			{
				Fire();
			}
			break;
		case State.Shooting:
			while (dragPositions.Count > 5)
			{
				dragPositions.RemoveAt(0);
			}
			if (IsDragging())
			{
				state = State.DragShooting;
			}
			HandleSnapToMoveDirection();
			break;
		}
		if (MVGameControllerBase.IPlayModeUI.GetCrossHair().FiredThisFrame)
		{
			Debug.Log("Fired " + Time.frameCount);
			firedDirections.Add(item);
		}
	}

	private void HandleSnapToMoveDirection()
	{
		float nextXFactor = GetNextXFactor(xFactor);
		if (nextXFactor != xFactor)
		{
			Vector3 direction = MVGameControllerBase.IPlayModeUI.GetCrossHair().Direction;
			if (Mathf.Sign(nextXFactor) != Mathf.Sign(direction.x))
			{
				direction = new Vector3(0f - direction.x, direction.y, direction.z);
				Debug.Log("Snap " + Time.frameCount);
				fireDirection.SetToDir(direction);
			}
		}
		xFactor = nextXFactor;
	}

	private static float GetNextXFactor(float prevXFactor)
	{
		if (MVInputWrapper.GetBooleanControl(KogamaControls.MoveLeft) && !MVInputWrapper.GetBooleanControl(KogamaControls.MoveRight))
		{
			return -1f;
		}
		if (MVInputWrapper.GetBooleanControl(KogamaControls.MoveRight) && !MVInputWrapper.GetBooleanControl(KogamaControls.MoveLeft))
		{
			return 1f;
		}
		return prevXFactor;
	}

	private bool Fire()
	{
		if (fired)
		{
			return false;
		}
		CrossPlatformInputManager.SetButtonDown(buttonName);
		fired = true;
		return true;
	}

	private bool IsDragging()
	{
		float num = AngleDiff(dragPositions[0], dragPositions[dragPositions.Count - 1]);
		if (num > 0.7f)
		{
			return true;
		}
		return false;
	}

	private Vector3 GetCurrentPointerPos()
	{
		if (Input.touchCount >= pointerId + 1 && pointerId != -1)
		{
			return Input.touches[pointerId].position;
		}
		return Input.mousePosition;
	}

	private float AngleDiff(Vector3 firstPos, Vector3 currentPos)
	{
		Vector3 direction = Camera.main.ScreenPointToRay(firstPos).direction;
		Vector3 direction2 = Camera.main.ScreenPointToRay(currentPos).direction;
		return Vector3.Angle(direction, direction2);
	}

	public void UpdateControllerFixedUpdate()
	{
	}
}
