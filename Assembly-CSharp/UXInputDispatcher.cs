using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[AddComponentMenu("UX/Management/Input dispatcher")]
public class UXInputDispatcher : MonoBehaviour, IInputHandler
{
	public class StateContext
	{
		private UXInputDispatcher dispatcher;

		public Vector3 MousePositionWorld;

		public List<GameObject> Objects;

		public HashSet<GameObject> ObjectsEntered;

		public HashSet<GameObject> ObjectsExited;

		public UXInputDispatcher InputDispatcher => dispatcher;

		public InputState State
		{
			get
			{
				return dispatcher.State;
			}
			set
			{
				dispatcher.State = value;
			}
		}

		public StateContext(UXInputDispatcher dispatcher)
		{
			this.dispatcher = dispatcher;
		}
	}

	public interface InputState
	{
		void Enter();

		bool Update();

		void Exit();
	}

	public class NormalState : InputState
	{
		private StateContext context;

		private Vector3 dragStartPosition = Vector3.zero;

		private UXDragObject potentialDragObject;

		private bool sticky;

		public NormalState(StateContext context)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			this.context = context;
		}

		public void Enter()
		{
			potentialDragObject = null;
		}

		public bool Update()
		{
			HandleMouseOver();
			HandleMouseClick();
			HandleStartDrag();
			if (sticky && MVInputWrapper.GetKeyUp((KeyCode)323))
			{
				sticky = false;
				return true;
			}
			return sticky;
		}

		private void HandleStartDrag()
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			if (Input.GetMouseButtonDown(0))
			{
				potentialDragObject = UXUtils.FindFirstWithComponent<UXDragObject>((ICollection<GameObject>)context.Objects);
			}
			if (Input.GetMouseButton(0) && (Object)(object)potentialDragObject != (Object)null)
			{
				Vector3 val = dragStartPosition - Input.mousePosition;
				if (val.sqrMagnitude > 0f && potentialDragObject.OnDragStart(context.MousePositionWorld))
				{
					context.InputDispatcher.State = new Dragging(context, dragStartPosition, potentialDragObject);
				}
			}
		}

		private void HandleMouseClick()
		{
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			bool keyDown = MVInputWrapper.GetKeyDown((KeyCode)323, useKey: false);
			bool key = MVInputWrapper.GetKey((KeyCode)323, useKey: false);
			bool keyUp = MVInputWrapper.GetKeyUp((KeyCode)323, useKey: false);
			if (!keyDown && !keyUp && !key)
			{
				return;
			}
			bool flag = false;
			foreach (GameObject @object in context.Objects)
			{
				UXMouseClickObject component = @object.GetComponent<UXMouseClickObject>();
				if ((Object)(object)component != (Object)null)
				{
					if (keyDown)
					{
						sticky = component.NotifyMouseDown(context.MousePositionWorld);
						if (!flag)
						{
							component.NotifyOnClick(context.MousePositionWorld);
							flag = true;
						}
					}
					if (key)
					{
						component.NotifyMouseDownMove(context.MousePositionWorld);
					}
					if (keyUp)
					{
						component.NotifyMouseUp(context.MousePositionWorld);
					}
				}
				if (keyDown)
				{
					UXFocusObject component2 = @object.GetComponent<UXFocusObject>();
					if ((Object)(object)component2 != (Object)null)
					{
						context.InputDispatcher.focusManager.CurrentFocus = component2;
					}
				}
			}
		}

		private void HandleMouseOver()
		{
			foreach (GameObject item in context.ObjectsEntered)
			{
				if ((Object)(object)item != (Object)null)
				{
					UXMouseOverObject component = item.GetComponent<UXMouseOverObject>();
					if ((Object)(object)component != (Object)null)
					{
						component.NotifyOnMouseOverEnter();
					}
				}
			}
			foreach (GameObject item2 in context.ObjectsExited)
			{
				if ((Object)(object)item2 != (Object)null)
				{
					UXMouseOverObject component2 = item2.GetComponent<UXMouseOverObject>();
					if ((Object)(object)component2 != (Object)null)
					{
						component2.NotifyOnMouseOverExit();
					}
				}
			}
		}

		public void Exit()
		{
		}
	}

	private class Dragging : InputState
	{
		private StateContext context;

		private UXDragObject dragObject;

		private Vector3 dragStartPosition;

		public Dragging(StateContext context, Vector3 dragStartPosition, UXDragObject dragObject)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			this.context = context;
			this.dragObject = dragObject;
			this.dragStartPosition = dragStartPosition;
		}

		public void Enter()
		{
		}

		public bool Update()
		{
			HandleDrag();
			HandleDrop();
			return true;
		}

		private void HandleDrag()
		{
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			foreach (GameObject item in context.ObjectsExited)
			{
				UXDropObject component = item.GetComponent<UXDropObject>();
				if ((Object)(object)component != (Object)null && component.OnDragOverExit != null)
				{
					component.OnDragOverExit(((Component)dragObject).gameObject);
				}
			}
			foreach (GameObject item2 in context.ObjectsEntered)
			{
				UXDropObject component2 = item2.GetComponent<UXDropObject>();
				if ((Object)(object)component2 != (Object)null && component2.OnDragOverEnter != null)
				{
					component2.OnDragOverEnter(((Component)dragObject).gameObject);
				}
			}
			if (!Input.GetMouseButton(0))
			{
				return;
			}
			if (dragObject.OnDrag != null)
			{
				dragObject.OnDrag(context.MousePositionWorld);
			}
			foreach (GameObject @object in context.Objects)
			{
				UXDropObject component3 = @object.GetComponent<UXDropObject>();
				if ((Object)(object)component3 != (Object)null && component3.OnDragOver != null)
				{
					component3.OnDragOver(((Component)dragObject).gameObject);
				}
			}
		}

		private void HandleDrop()
		{
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			if (!Input.GetMouseButtonUp(0))
			{
				return;
			}
			UXDropObject uXDropObject = UXUtils.FindFirstWithComponent<UXDropObject>((ICollection<GameObject>)context.Objects);
			bool isDrop = false;
			if ((Object)(object)uXDropObject != (Object)null && uXDropObject.AcceptDrop != null && uXDropObject.AcceptDrop(((Component)dragObject).gameObject))
			{
				if (uXDropObject.OnDrop != null)
				{
					uXDropObject.OnDrop(((Component)dragObject).gameObject);
				}
				isDrop = true;
			}
			if (dragObject.OnDragStop != null)
			{
				dragObject.OnDragStop(context.MousePositionWorld, isDrop);
			}
			context.State = context.InputDispatcher.NORMAL_STATE;
		}

		public void Exit()
		{
			foreach (GameObject @object in context.Objects)
			{
				UXDropObject component = @object.GetComponent<UXDropObject>();
				if ((Object)(object)component != (Object)null && component.OnDragOverExit != null)
				{
					component.OnDragOverExit(((Component)dragObject).gameObject);
				}
			}
		}
	}

	public delegate void OnMouseButtonDownDelegate(Vector3 mousePositionWorld);

	public delegate void OnMouseButtonDelegate(Vector3 mousePositionWorld);

	public delegate void OnMouseButtonUpDelegate(Vector3 mousePositionWorld);

	private int uiLayerMask;

	private List<GameObject> objects = new List<GameObject>(10);

	private Queue<GameObject> objectsForCleanup = new Queue<GameObject>();

	private UXFocusManager focusManager;

	private InputState state;

	private readonly StateContext context;

	private readonly NormalState NORMAL_STATE;

	private Stack<UXView> modalViews = new Stack<UXView>();

	private Dictionary<GameObject, UXView> anchestorView = new Dictionary<GameObject, UXView>();

	public OnMouseButtonDownDelegate OnMouseButtonDown;

	public OnMouseButtonDelegate OnMouseButton;

	public OnMouseButtonUpDelegate OnMouseButtonUp;

	private Queue<InputState> stateQueue = new Queue<InputState>();

	public int Priority => InputHandlerPriority.UI;

	public InputState State
	{
		get
		{
			while (stateQueue.Count > 0)
			{
				state.Exit();
				state = stateQueue.Dequeue();
				state.Enter();
			}
			return state;
		}
		set
		{
			stateQueue.Enqueue(value);
		}
	}

	public UXInputDispatcher()
	{
		context = new StateContext(this);
		NORMAL_STATE = new NormalState(context);
	}

	public void Awake()
	{
		focusManager = UXUtils.FindObjectOfType<UXFocusManager>();
		uiLayerMask = 1 << LayerMask.NameToLayer("UXElement");
		state = NORMAL_STATE;
		UXUtils.FindObjectOfType<MVInputHandlerPrioritizer>().Register(this);
	}

	public bool HandleInput()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		List<GameObject> list = objects;
		objects = FindHitObjects();
		HashSet<GameObject> hashSet = new HashSet<GameObject>(objects);
		hashSet.ExceptWith(list);
		HashSet<GameObject> hashSet2 = new HashSet<GameObject>(list);
		hashSet2.ExceptWith(objects);
		context.MousePositionWorld = ((Component)this).camera.ScreenToWorldPoint(Input.mousePosition);
		context.Objects = objects;
		context.ObjectsEntered = hashSet;
		context.ObjectsExited = hashSet2;
		HandleNonUIElementEvents();
		bool result = State.Update();
		CleanupObjects();
		return result;
	}

	public bool HandleLateUpdate()
	{
		return false;
	}

	public void PushModalView(UXView view)
	{
		modalViews.Push(view);
	}

	public UXView PeekModalView()
	{
		return modalViews.Peek();
	}

	public UXView PopModalView()
	{
		return modalViews.Pop();
	}

	private void CleanupObjects()
	{
		foreach (GameObject item in objectsForCleanup)
		{
			objects.Remove(item);
			Object.Destroy((Object)(object)item);
		}
		objectsForCleanup.Clear();
	}

	private List<GameObject> FindHitObjects()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		Ray val = ((Component)this).camera.ScreenPointToRay(Input.mousePosition);
		RaycastHit[] array = Physics.RaycastAll(val, float.PositiveInfinity, uiLayerMask);
		Array.Sort(array, (RaycastHit a, RaycastHit b) => a.distance.CompareTo(b.distance));
		UXView uXView = ((modalViews.Count <= 0) ? null : modalViews.Peek());
		List<GameObject> list = new List<GameObject>();
		if ((Object)(object)uXView == (Object)null)
		{
			list.AddRange(array.Select((RaycastHit hit) => ((Component)hit.collider).gameObject));
		}
		else
		{
			foreach (GameObject item in array.Select((RaycastHit hit) => ((Component)hit.collider).gameObject))
			{
				UXView uXView2 = FindParentView(item);
				if ((Object)(object)uXView2 == (Object)null || (Object)(object)uXView2 == (Object)(object)uXView)
				{
					list.Add(item);
				}
			}
		}
		return list;
	}

	private UXView FindParentView(GameObject gameObject)
	{
		if (anchestorView.TryGetValue(gameObject, out var value))
		{
			return value;
		}
		UXView uXView = UXUtils.FindComponentInParents(typeof(UXView), gameObject.transform) as UXView;
		if ((Object)(object)uXView != (Object)null)
		{
			anchestorView[gameObject] = uXView;
			return uXView;
		}
		return null;
	}

	private void HandleNonUIElementEvents()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		bool keyDown = MVInputWrapper.GetKeyDown((KeyCode)323);
		bool key = MVInputWrapper.GetKey((KeyCode)323);
		bool keyUp = MVInputWrapper.GetKeyUp((KeyCode)323);
		if (keyDown && context.InputDispatcher.OnMouseButtonDown != null)
		{
			context.InputDispatcher.OnMouseButtonDown(context.MousePositionWorld);
		}
		if (key && context.InputDispatcher.OnMouseButton != null)
		{
			context.InputDispatcher.OnMouseButton(context.MousePositionWorld);
		}
		if (keyUp && context.InputDispatcher.OnMouseButtonUp != null)
		{
			context.InputDispatcher.OnMouseButtonUp(context.MousePositionWorld);
		}
	}

	public void DestroyUIElement(GameObject gameObject)
	{
		UXUtils.VisitSubtree(gameObject.transform, objectsForCleanup.Enqueue);
	}
}
