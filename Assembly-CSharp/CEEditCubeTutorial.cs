using System;
using System.Collections.Generic;
using MV.WorldObject;
using MV.WorldObject.MetaData;
using UnityEngine;
using UnityEngine.EventSystems;

public class CEEditCubeTutorial : ESStateBase
{
	private class ResettingBookkeeping
	{
		private int resettingBeginTime;

		public int resettingDelay;

		private bool doReset;

		public bool isResetting;

		public bool DoReset => doReset;

		public bool ReadyToReset => WaitForTicksLocal.Diff(resettingBeginTime) > resettingDelay;

		public void InitializeResetting(int resettingDelay)
		{
			Debug.Log("InitializeResetting");
			if (resettingDelay > this.resettingDelay)
			{
				this.resettingDelay = resettingDelay;
			}
			doReset = true;
			isResetting = false;
		}

		public void StartResetting()
		{
			doReset = false;
			isResetting = true;
			resettingBeginTime = WaitForTicksLocal.GetEnvironmentTick(0);
		}
	}

	private int targetCubeModelId = -1;

	private bool exiting;

	private IModelingConstraint constraint;

	private ConstraintVisualizer constraintVisualizer;

	private EditableCubeModelWrapper cubeModelWrapper;

	private static HashSet<FirstTimeEvent> successEvents = new HashSet<FirstTimeEvent>
	{
		FirstTimeEvent.BM_TryExtrudingCube,
		FirstTimeEvent.BM_TryTiltingCube,
		FirstTimeEvent.BM_TryTiltingCorner,
		FirstTimeEvent.BM_TryAddingCube,
		FirstTimeEvent.BM_CubeTutorialAddedCubes,
		FirstTimeEvent.BM_CubeTutorialPaintedCubes,
		FirstTimeEvent.BM_CubeTutorialDeletedCubes
	};

	private static HashSet<FirstTimeEvent> disableCubeModelingEvents = new HashSet<FirstTimeEvent>
	{
		FirstTimeEvent.BM_CubeTutorialAddedCubes,
		FirstTimeEvent.BM_CubeTutorialPaintedCubes
	};

	private static HashSet<FirstTimeEvent> enableCubeModelingEvents = new HashSet<FirstTimeEvent>
	{
		FirstTimeEvent.BM_ChangeToolToPaint,
		FirstTimeEvent.BM_ChangeToolToDelete
	};

	private float oneCubeDistance = 4f;

	private float multiCubeDistance = 16f;

	private Vector3 focusOffset = (Vector3.right + Vector3.up + Vector3.back).normalized;

	private FirstTimeCubeModelBlinker blinker;

	private bool hasExited;

	private IntVector zeroPos = new IntVector(0, 0, 0);

	private Dictionary<EditCubeChange, bool> firstTimeEventChangeCheck = new Dictionary<EditCubeChange, bool>();

	private bool bordersExpanded;

	private MVCubeModelInstance selectedInstance;

	private CubeModelingStateMachine CMSM;

	private int mainCameraDefaultMask;

	private ResettingBookkeeping resettingBookkeeping = new ResettingBookkeeping();

	private bool disableCubeModeling;

	private bool enableCubemodeling = true;

	private MVCubeModelBase TargetCubeModel
	{
		get
		{
			if (targetCubeModelId == -1)
			{
				return null;
			}
			return (MVCubeModelBase)MVGameControllerBase.WOCM.GetWorldObjectClient(targetCubeModelId);
		}
		set
		{
			if (value != null)
			{
				targetCubeModelId = value.Id;
			}
			else
			{
				targetCubeModelId = -1;
			}
		}
	}

	public override void Enter(EditorStateMachine e)
	{
		exiting = false;
		hasExited = false;
		if (e.SingleSelectedWO == null)
		{
			Debug.LogError("ESEditCubeTutorial must not be entered with no selected WorldObject");
			e.PopState();
			return;
		}
		firstTimeEventChangeCheck = GetFirstTimeEventCheck();
		selectedInstance = e.SingleSelectedWO as MVCubeModelInstance;
		TargetCubeModel = (MVCubeModelBase)e.SingleSelectedWO;
		CMSM = e.CubeModelingStateMachine;
		CubeModelTool.OnEditCubeChange = (Action<int, EditCubeChange>)Delegate.Combine(CubeModelTool.OnEditCubeChange, new Action<int, EditCubeChange>(OnEditCubeChange));
		FirstTimeEventManager.SubscribeToFirstTimeState(SetFirstTimeEventsHappened);
		if (!bordersExpanded)
		{
			cubeModelWrapper = new EditableCubeModelWrapper(selectedInstance, new IntVector(-1, -1, -1), new IntVector(1, 1, 1), 27);
		}
		((MVAvatarLocal.JetPackMode)MVGameControllerBase.WOCM.AvatarLocal.CurrentMode).ModifySpeed(Mathf.Min(1f, 1f * e.SingleSelectedWO.Scale.x), Mathf.Min(1f, 1f * e.SingleSelectedWO.Scale.x));
		DrawPlane.HideDrawPlane();
		MVAvatarLocal.JetPackMode jetPackMode = (MVAvatarLocal.JetPackMode)MVGameControllerBase.WOCM.AvatarLocal.CurrentMode;
		jetPackMode.SetMoveConstraint(new Vector3(0f, 0f, 0f), 25f);
		ExecuteEvents.ExecuteHierarchy(e.GameObject, null, (IHandleCubeEditTutorial handler, BaseEventData data) =>
		{
			handler.PushCubeEditCubeTutorialTools(OnClosed);
		});
		tintedWo = null;
		if (constraintVisualizer == null)
		{
			CreateConstraint();
		}
		TargetCubeModel.RemoveSelectionBox();
		if (!e.ParentGroupIsRoot)
		{
			SharedCubeFunctions.SetLayerRecursively(MVGameControllerBase.WOCM.GetWorldObjectClient(e.ParentGroupID).Transform, select: false);
		}
		SharedCubeFunctions.SetLayerRecursively(TargetCubeModel.Transform, select: true);
		CMSM.StartEdit(TargetCubeModel, constraint);
		mainCameraDefaultMask = e.CameraController.MainCamera.cullingMask;
		e.CameraController.MainCamera.cullingMask = 0;
		e.CameraController.BlueModeEnabled = true;
		SetupBlinker();
	}

	public override void Execute(EditorStateMachine e)
	{
		base.Execute(e);
		exiting = exiting || TargetCubeModel == null;
		if (exiting)
		{
			e.Event = EditorEvent.ESLeaveCubeTutorial;
			hasExited = true;
		}
		else
		{
			if (hasExited)
			{
				return;
			}
			if ((resettingBookkeeping.DoReset || disableCubeModeling) && Cursor.visible)
			{
				if (resettingBookkeeping.DoReset)
				{
					resettingBookkeeping.StartResetting();
				}
				CMSM.CursorVisible = false;
				disableCubeModeling = false;
				Debug.Log("Hiding cursor");
			}
			if (resettingBookkeeping.isResetting)
			{
				if (DoReset())
				{
					resettingBookkeeping.isResetting = false;
				}
			}
			else if (enableCubemodeling)
			{
				e.CubeModelingStateMachine.Update();
			}
			else
			{
				Cursor.visible = true;
			}
		}
	}

	private void HandleCubeModelingEnabling(FirstTimeEvent firstTimeEvent)
	{
		if (enableCubeModelingEvents.Contains(firstTimeEvent))
		{
			disableCubeModeling = false;
			enableCubemodeling = true;
			CMSM.CursorVisible = true;
		}
		if (disableCubeModelingEvents.Contains(firstTimeEvent))
		{
			enableCubemodeling = false;
			disableCubeModeling = true;
		}
	}

	public override void Exit(EditorStateMachine esm)
	{
		base.Exit(esm);
		esm.CameraController.MainCamera.cullingMask = mainCameraDefaultMask;
		MVAvatarLocal.JetPackMode jetPackMode = (MVAvatarLocal.JetPackMode)MVGameControllerBase.WOCM.AvatarLocal.CurrentMode;
		jetPackMode.MovementConstrained = false;
		string err = string.Empty;
		if (!TargetCubeModel.Delete(MVGameControllerBase.WOCM, ref err))
		{
			ExecuteEvents.ExecuteHierarchy(esm.GameObject, null, (IModalPopupCreator handler, BaseEventData data) =>
			{
				handler.CreateErrorNotificationPopup(err);
			});
		}
		UnityEngine.Object.Destroy(constraintVisualizer.gameObject);
		MVGameControllerBase.WOCM.AvatarLocal.LaserPointer.ChangeState(LaserPointerState.Idle);
		MVGameControllerBase.WOCM.AvatarLocal.SetMode(AvatarRuntimeState.Edit);
		((MVAvatarLocal.JetPackMode)MVGameControllerBase.WOCM.AvatarLocal.CurrentMode).ModifySpeed(1f, 1f);
		MVGameControllerBase.WOCM.AvatarLocal.SetToSpawnTransform();
		esm.CubeModelingStateMachine.RemoveCursors();
		esm.CubeModelingStateMachine.EndEdit();
		TargetCubeModel = null;
		selectedInstance = null;
		esm.DeSelectAll();
		FirstTimeEventManager.UnSubscribeToFirstTimeState(SetFirstTimeEventsHappened);
		CubeModelTool.OnEditCubeChange = (Action<int, EditCubeChange>)Delegate.Remove(CubeModelTool.OnEditCubeChange, new Action<int, EditCubeChange>(OnEditCubeChange));
		blinker = null;
	}

	private void SetFocus(float distance)
	{
		Vector3 avatarPosition = TargetCubeModel.Transform.position + TargetCubeModel.Transform.rotation * focusOffset * distance;
		JetPackCamera jetPackCamera = (JetPackCamera)MVGameControllerBase.CameraController.CurCamera;
		jetPackCamera.FocusOnPointFromAvatarPosition(TargetCubeModel.WorldPivot, avatarPosition);
	}

	private void SetupBlinker()
	{
		blinker = TargetCubeModel.GameObject.AddComponent<FirstTimeCubeModelBlinker>();
		blinker.Initialize(new Material(PrefabPool.Instance.BlinkerDefaultMaterial), MVGameControllerBase.CameraController.SecondaryCamera, TargetCubeModel);
		blinker.Visible = true;
	}

	private void CreateConstraint()
	{
		GameObject gameObject = new GameObject("ConstrainVisualizer");
		constraintVisualizer = gameObject.AddComponent<ConstraintVisualizer>();
		constraint = TargetCubeModel.ModelingConstraintBuilder();
		constraintVisualizer.Init(TargetCubeModel, constraint);
		CMSM.SetConstraint(constraint);
	}

	private void OnClosed()
	{
		exiting = true;
	}

	private void OnEditCubeChange(int cubeCount, EditCubeChange editCubeChange)
	{
		if (!bordersExpanded)
		{
			resettingBookkeeping.InitializeResetting(400);
		}
	}

	private Dictionary<EditCubeChange, bool> GetFirstTimeEventCheck()
	{
		bool value = !FirstTimeEventManager.HasFirstTimeEventOccured(FirstTimeEvent.BM_TryExtrudingCube);
		bool value2 = !FirstTimeEventManager.HasFirstTimeEventOccured(FirstTimeEvent.BM_TryTiltingCube);
		bool value3 = !FirstTimeEventManager.HasFirstTimeEventOccured(FirstTimeEvent.BM_TryTiltingCorner);
		bool value4 = !FirstTimeEventManager.HasFirstTimeEventOccured(FirstTimeEvent.BM_TryAddingCube);
		Dictionary<EditCubeChange, bool> dictionary = new Dictionary<EditCubeChange, bool>();
		dictionary.Add(EditCubeChange.FaceMoved, value);
		dictionary.Add(EditCubeChange.EdgeMoved, value2);
		dictionary.Add(EditCubeChange.VertexMoved, value3);
		dictionary.Add(EditCubeChange.CubeAdded, value4);
		return dictionary;
	}

	private void SetFirstTimeEventsHappened(FirstTimeState firstTimeState, FirstTimeEvent firstTimeEvent)
	{
		HandleCubeModelingEnabling(firstTimeEvent);
		if (successEvents.Contains(firstTimeEvent))
		{
			blinker.StartBlinking(BlinkType.OnBoardingCubeModelSuccess, 2f);
		}
		Dictionary<EditCubeChange, bool> dictionary = firstTimeEventChangeCheck;
		firstTimeEventChangeCheck = GetFirstTimeEventCheck();
		bool flag = true;
		foreach (KeyValuePair<EditCubeChange, bool> item in dictionary)
		{
			if (firstTimeEventChangeCheck[item.Key] != item.Value)
			{
				resettingBookkeeping.InitializeResetting(2000);
			}
			if (firstTimeEventChangeCheck[item.Key])
			{
				flag = false;
			}
		}
		if (flag && !bordersExpanded)
		{
			bordersExpanded = true;
			cubeModelWrapper = new EditableCubeModelWrapper(selectedInstance, new IntVector(-15, -15, -15), new IntVector(15, 15, 15), 1);
			SetFocus(multiCubeDistance);
			MVGameControllerBase.CameraController.StartTransitionCam(1f);
			if (constraintVisualizer != null)
			{
				UnityEngine.Object.Destroy(constraintVisualizer.gameObject);
			}
			CreateConstraint();
		}
		else if (!bordersExpanded)
		{
			SetFocus(oneCubeDistance);
		}
	}

	private bool DoReset()
	{
		if (!resettingBookkeeping.ReadyToReset)
		{
			return false;
		}
		resettingBookkeeping.resettingDelay = 0;
		CMSM.CursorVisible = true;
		byte material = cubeModelWrapper.CubeModel.GetCube(zeroPos).FaceMaterials[0];
		cubeModelWrapper.CubeModel.RemoveCube(zeroPos);
		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				for (int k = -1; k <= 1; k++)
				{
					cubeModelWrapper.CubeModel.RemoveCube(new IntVector(i, j, k));
				}
			}
		}
		cubeModelWrapper.CubeModel.AddCube(zeroPos, new Cube(CubeDataPacker.CornersToByteArray(CubeBase.IdentityCorners), Cube.CreateMaterialArray(material)));
		return true;
	}
}
