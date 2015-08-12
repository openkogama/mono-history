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

		private bool validDragObject;

		private Vector3 dragStartPosition = Vector3.zero;

		private UXDragObject potentialDragObject;

		private bool mouseDownObject;

		private bool sticky;

		public NormalState(StateContext context)
		{
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
			if (sticky && MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelect))
			{
				sticky = false;
				return true;
			}
			return sticky;
		}

		private void HandleStartDrag()
		{
			if (MVInputWrapper.GetBooleanControlDown(KogamaControls.PointerSelect))
			{
				potentialDragObject = UXUtils.FindFirstWithComponent<UXDragObject>(context.Objects);
			}
			if (MVInputWrapper.GetBooleanControl(KogamaControls.PointerSelect) && potentialDragObject != null)
			{
				if (!validDragObject)
				{
					dragStartPosition = MVInputWrapper.GetPointerPosition();
					validDragObject = true;
				}
				if ((dragStartPosition - MVInputWrapper.GetPointerPosition()).sqrMagnitude > 0f)
				{
					if (potentialDragObject.OnDragStart(context.MousePositionWorld))
					{
						context.InputDispatcher.State = new Dragging(context, dragStartPosition, potentialDragObject);
					}
					validDragObject = false;
				}
			}
			else
			{
				validDragObject = false;
			}
		}

		private void HandleMouseClick()
		{
			bool booleanControlDown = MVInputWrapper.GetBooleanControlDown(KogamaControls.PointerSelect);
			bool booleanControl = MVInputWrapper.GetBooleanControl(KogamaControls.PointerSelect);
			bool booleanControlUp = MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelect);
			if (!booleanControlDown && !booleanControlUp && !booleanControl)
			{
				return;
			}
			bool flag = false;
			bool flag2 = false;
			foreach (GameObject @object in context.Objects)
			{
				UXMouseClickObject component = @object.GetComponent<UXMouseClickObject>();
				if (component != null)
				{
					if (booleanControlDown)
					{
						sticky = component.NotifyMouseDown(context.MousePositionWorld);
						if (!flag)
						{
							component.NotifyOnClick(context.MousePositionWorld);
							mouseDownObject = true;
							flag = true;
						}
					}
					if (booleanControl)
					{
						component.NotifyMouseDownMove(context.MousePositionWorld);
					}
					if (booleanControlUp && mouseDownObject && !flag2)
					{
						component.NotifyMouseUp(context.MousePositionWorld);
						flag2 = true;
					}
				}
				if (booleanControlDown && (!flag || (flag && (bool)@object == flag)))
				{
					UXFocusObject component2 = @object.GetComponent<UXFocusObject>();
					if (component2 != null)
					{
						context.InputDispatcher.focusManager.CurrentFocus = component2;
					}
				}
			}
			if (booleanControlUp)
			{
				mouseDownObject = false;
			}
		}

		private void HandleMouseOver()
		{
			if (!Cursor.visible)
			{
				return;
			}
			foreach (GameObject item in context.ObjectsExited)
			{
				if (item != null)
				{
					UXMouseOverObject component = item.GetComponent<UXMouseOverObject>();
					if (component != null)
					{
						component.NotifyOnMouseOverExit();
					}
				}
			}
			foreach (GameObject item2 in context.ObjectsEntered)
			{
				if (item2 != null)
				{
					UXMouseOverObject component2 = item2.GetComponent<UXMouseOverObject>();
					if (component2 != null)
					{
						component2.NotifyOnMouseOverEnter();
					}
				}
			}
			foreach (GameObject @object in context.Objects)
			{
				if (@object != null)
				{
					UXMouseOverObject component3 = @object.GetComponent<UXMouseOverObject>();
					if (component3 != null)
					{
						component3.NotifyOnMouseOver();
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
			foreach (GameObject item in context.ObjectsExited)
			{
				UXDropObject component = item.GetComponent<UXDropObject>();
				if (component != null && component.OnDragOverExit != null)
				{
					component.OnDragOverExit(dragObject.gameObject);
				}
			}
			foreach (GameObject item2 in context.ObjectsEntered)
			{
				UXDropObject component2 = item2.GetComponent<UXDropObject>();
				if (component2 != null && component2.OnDragOverEnter != null)
				{
					component2.OnDragOverEnter(dragObject.gameObject);
				}
			}
			if (!MVInputWrapper.GetBooleanControl(KogamaControls.PointerSelect))
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
				if (component3 != null && component3.OnDragOver != null)
				{
					component3.OnDragOver(dragObject.gameObject);
				}
			}
		}

		private void HandleDrop()
		{
			if (!MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelect))
			{
				return;
			}
			UXDropObject uXDropObject = UXUtils.FindFirstWithComponent<UXDropObject>(context.Objects);
			bool isDrop = false;
			if (uXDropObject != null && uXDropObject.AcceptDrop != null && uXDropObject.AcceptDrop(dragObject.gameObject))
			{
				if (uXDropObject.OnDrop != null)
				{
					uXDropObject.OnDrop(dragObject.gameObject);
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
				if (component != null && component.OnDragOverExit != null && dragObject != null)
				{
					component.OnDragOverExit(dragObject.gameObject);
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

	public bool BlockGUIInput { get; set; }

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
		focusManager = UXUtils.FindGUIObjectOfType<UXFocusManager>();
		uiLayerMask = 1 << LayerMask.NameToLayer("UXElement");
		state = NORMAL_STATE;
		UXUtils.FindObjectOfType<MVInputHandlerPrioritizer>().Register(this);
	}

	public bool HandleInput()
	{
		List<GameObject> list = objects;
		objects = FindHitObjects();
		HashSet<GameObject> hashSet = new HashSet<GameObject>(objects);
		hashSet.ExceptWith(list);
		HashSet<GameObject> hashSet2 = new HashSet<GameObject>(list);
		hashSet2.ExceptWith(objects);
		context.MousePositionWorld = GetComponent<Camera>().ScreenToWorldPoint(MVInputWrapper.GetPointerPosition());
		context.Objects = objects;
		context.ObjectsEntered = hashSet;
		context.ObjectsExited = hashSet2;
		HandleNonUIElementEvents();
		bool result = !BlockGUIInput && State.Update();
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
			UnityEngine.Object.Destroy(item);
		}
		objectsForCleanup.Clear();
	}

	private List<GameObject> FindHitObjects()
	{
		Ray ray = GetComponent<Camera>().ScreenPointToRay(MVInputWrapper.GetPointerPosition());
		RaycastHit[] array = Physics.RaycastAll(ray, float.PositiveInfinity, uiLayerMask);
		Array.Sort(array, (RaycastHit a, RaycastHit b) => a.distance.CompareTo(b.distance));
		UXView uXView = ((modalViews.Count <= 0) ? null : modalViews.Peek());
		List<GameObject> list = new List<GameObject>();
		if (uXView == null)
		{
			list.AddRange(array.Select((RaycastHit hit) => hit.collider.gameObject));
		}
		else
		{
			foreach (GameObject item in array.Select((RaycastHit hit) => hit.collider.gameObject))
			{
				UXView uXView2 = FindParentView(item);
				if (uXView2 == null || uXView2 == uXView)
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
		if (uXView != null)
		{
			anchestorView[gameObject] = uXView;
			return uXView;
		}
		return null;
	}

	private void HandleNonUIElementEvents()
	{
		bool booleanControlDown = MVInputWrapper.GetBooleanControlDown(KogamaControls.PointerSelect);
		bool booleanControl = MVInputWrapper.GetBooleanControl(KogamaControls.PointerSelect);
		bool booleanControlUp = MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelect);
		if (booleanControlDown && context.InputDispatcher.OnMouseButtonDown != null)
		{
			context.InputDispatcher.OnMouseButtonDown(context.MousePositionWorld);
		}
		if (booleanControl && context.InputDispatcher.OnMouseButton != null)
		{
			context.InputDispatcher.OnMouseButton(context.MousePositionWorld);
		}
		if (booleanControlUp && context.InputDispatcher.OnMouseButtonUp != null)
		{
			context.InputDispatcher.OnMouseButtonUp(context.MousePositionWorld);
		}
	}

	public void DestroyUIElement(GameObject gameObject)
	{
		UXUtils.VisitSubtree(gameObject.transform, objectsForCleanup.Enqueue);
	}
}
