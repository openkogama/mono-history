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
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			if (Input.GetMouseButtonDown(0))
			{
				potentialDragObject = UXUtils.FindFirstWithComponent<UXDragObject>((ICollection<GameObject>)context.Objects);
			}
			if (Input.GetMouseButton(0) && (Object)(object)potentialDragObject != (Object)null)
			{
				if (!validDragObject)
				{
					dragStartPosition = Input.mousePosition;
					validDragObject = true;
				}
				Vector3 val = dragStartPosition - Input.mousePosition;
				if (val.sqrMagnitude > 0f)
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
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			bool keyDown = MVInputWrapper.GetKeyDown((KeyCode)323);
			bool key = MVInputWrapper.GetKey((KeyCode)323);
			bool keyUp = MVInputWrapper.GetKeyUp((KeyCode)323);
			if (!keyDown && !keyUp && !key)
			{
				return;
			}
			bool flag = false;
			bool flag2 = false;
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
							mouseDownObject = true;
							flag = true;
						}
					}
					if (key)
					{
						component.NotifyMouseDownMove(context.MousePositionWorld);
					}
					if (keyUp && mouseDownObject && !flag2)
					{
						component.NotifyMouseUp(context.MousePositionWorld);
						flag2 = true;
					}
				}
				if (keyDown && (!flag || (flag && Object.op_Implicit((Object)(object)@object) == flag)))
				{
					UXFocusObject component2 = @object.GetComponent<UXFocusObject>();
					if ((Object)(object)component2 != (Object)null)
					{
						context.InputDispatcher.focusManager.CurrentFocus = component2;
					}
				}
			}
			if (keyUp)
			{
				mouseDownObject = false;
			}
		}

		private void HandleMouseOver()
		{
			if (!Screen.showCursor)
			{
				return;
			}
			foreach (GameObject item in context.ObjectsExited)
			{
				if ((Object)(object)item != (Object)null)
				{
					UXMouseOverObject component = item.GetComponent<UXMouseOverObject>();
					if ((Object)(object)component != (Object)null)
					{
						component.NotifyOnMouseOverExit();
					}
				}
			}
			foreach (GameObject item2 in context.ObjectsEntered)
			{
				if ((Object)(object)item2 != (Object)null)
				{
					UXMouseOverObject component2 = item2.GetComponent<UXMouseOverObject>();
					if ((Object)(object)component2 != (Object)null)
					{
						component2.NotifyOnMouseOverEnter();
					}
				}
			}
			foreach (GameObject @object in context.Objects)
			{
				if ((Object)(object)@object != (Object)null)
				{
					UXMouseOverObject component3 = @object.GetComponent<UXMouseOverObject>();
					if ((Object)(object)component3 != (Object)null)
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
				if ((Object)(object)component != (Object)null && component.OnDragOverExit != null && (Object)(object)dragObject != (Object)null)
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
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
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
