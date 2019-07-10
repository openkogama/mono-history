using System;
using System.Collections.Generic;
using UnityEngine;

internal class ESTranslate : ESStateBase
{
	private float gridSize;

	private float stickyModifier = 0.2f;

	private float completelyStuckLimit = 0.3f;

	private bool recalcLocalDirCamToObjects = true;

	private List<TranslateData> translateDatas = new List<TranslateData>();

	private List<MVWorldObjectClient> targets = new List<MVWorldObjectClient>();

	private const float _mouseSensitivity = 0.005f;

	private float initialDistance;

	private Vector3 originPrevFrame = Vector3.zero;

	private bool playTranslateSounds = true;

	private float scrollMoveDistance;

	private TranslateMode translateMode;

	private HashSet<int> woIds;

	private bool fixedToYPlane = true;

	private bool moveWithAvatar;

	private bool enteredStateWithPointerSelectReleased;

	public override void Enter(EditorStateMachine e)
	{
		if (MVGameControllerBase.EditModeUI.IsGridSnap())
		{
			gridSize = 1f;
		}
		else
		{
			gridSize = 0.0625f;
		}
		translateMode = (TranslateMode)e.Data["translateMode"];
		moveWithAvatar = (bool)e.Data["moveWithAvatar"];
		float hitDistance = 0f;
		initialDistance = 0f;
		if (GetInitialAvatarMoveObjectHitDistance(e, ref hitDistance))
		{
			initialDistance = hitDistance;
		}
		else
		{
			initialDistance = GetInitialAvatarMoveObjectDistance(e);
		}
		recalcLocalDirCamToObjects = true;
		translateDatas = new List<TranslateData>();
		targets = new List<MVWorldObjectClient>();
		e.MainCameraManager.IgnoreInputTypes(IgnoreInputTypes.MouseScroll | IgnoreInputTypes.Avatar);
		if (!e.NetworkSelector.RequestOwnership(e.SelectedIDs))
		{
			Debug.Log("Ownership request failed");
			e.PopState();
			return;
		}
		MVGameControllerBase.GameEventManager.AvatarCommandsBuildMode.LaserCommands.ChangeState(LaserPointerState.Transforming);
		MVGameControllerBase.GameEventManager.AvatarCommandsBuildMode.LaserCommands.SetLaserActiveState(isActive: true);
		foreach (int selectedID in e.SelectedIDs)
		{
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(selectedID);
			targets.Add(worldObjectClient);
			translateDatas.Add(new TranslateData(worldObjectClient, gridSize));
		}
		e.MainCameraManager.TertiaryCameraActive = true;
		e.MainCameraManager.TertiaryCamera.SetReplacementShader(e.MainCameraManager.transparentMultiplyColor, string.Empty);
		woIds = new HashSet<int>();
		foreach (TranslateData translateData in translateDatas)
		{
			MVGameControllerBase.WOCM.GetAllWoIds(translateData.Wo.Id, woIds);
			SharedCubeFunctions.SetLayerRecursively(translateData.Wo.Transform, select: true);
		}
		Cursor.visible = false;
		originPrevFrame = MVGameControllerBase.SpawnRoleDataMediatorLocal.Position;
		enteredStateWithPointerSelectReleased = MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelect);
	}

	public override void Execute(EditorStateMachine e)
	{
		base.Execute(e);
		if (!IsValid())
		{
			e.PopState();
		}
		else if (enteredStateWithPointerSelectReleased || !MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelect))
		{
			Vector3 deltaMouse = GetDeltaMouse(e);
			if (MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelectAlt))
			{
				recalcLocalDirCamToObjects = true;
			}
			Vector3 vector = MVGameControllerBase.SpawnRoleDataMediatorLocal.Position - originPrevFrame;
			originPrevFrame = MVGameControllerBase.SpawnRoleDataMediatorLocal.Position;
			for (int i = 0; i < translateDatas.Count; i++)
			{
				if (moveWithAvatar)
				{
					translateDatas[i].ungridifiedPosition += vector;
				}
				if (MVInputWrapper.GetBooleanControl(KogamaControls.PointerSelectAlt))
				{
					RotateWithCamera(e, i);
				}
				else
				{
					translateDatas[i].ungridifiedPosition += deltaMouse;
				}
				translateDatas[i].gridifiedPosition = translateDatas[i].Wo.GetClosestGridPoint(gridSize, translateDatas[i].ungridifiedPosition);
				if (gridSize < (translateDatas[i].prevGridifiedPosition - translateDatas[i].ungridifiedPosition).magnitude)
				{
					translateDatas[i].Wo.SyncPos = translateDatas[i].gridifiedPosition;
					translateDatas[i].prevGridifiedPosition = translateDatas[i].gridifiedPosition;
					translateDatas[i].ungridifiedPosition = translateDatas[i].gridifiedPosition;
					if (playTranslateSounds)
					{
						AudioEventHandler.AddTranslateSoundData(0f, moveToGridPos: true, translateDatas[i].Wo.WorldPosition);
					}
					continue;
				}
				Vector3 vector2 = translateDatas[i].ungridifiedPosition - translateDatas[i].prevGridifiedPosition;
				float magnitude = vector2.magnitude;
				if (magnitude > completelyStuckLimit * gridSize)
				{
					vector2 *= stickyModifier;
					translateDatas[i].Wo.WorldPosition = translateDatas[i].prevGridifiedPosition + vector2;
				}
				if (playTranslateSounds)
				{
					AudioEventHandler.AddTranslateSoundData(magnitude, moveToGridPos: false, translateDatas[i].Wo.WorldPosition);
				}
			}
			UpdateLaserPosition(targets);
			enteredStateWithPointerSelectReleased = false;
		}
		else
		{
			e.PopState();
		}
	}

	private bool IsValid()
	{
		foreach (TranslateData translateData in translateDatas)
		{
			if (translateData.Wo == null)
			{
				return false;
			}
		}
		return true;
	}

	private void UpdateLaserPosition(List<MVWorldObjectClient> wos)
	{
		if (wos.Count == 1)
		{
			MVGameControllerBase.GameEventManager.AvatarCommandsBuildMode.LaserCommands.UpdatePosition(wos[0].WorldPivot);
			return;
		}
		List<Transform> list = new List<Transform>();
		foreach (MVWorldObjectClient wo in wos)
		{
			list.Add(wo.Transform);
		}
		MVGameControllerBase.GameEventManager.AvatarCommandsBuildMode.LaserCommands.UpdatePosition(SharedCubeFunctions.GetWorldCenter(list));
	}

	public override void Exit(EditorStateMachine e)
	{
		MVGameControllerBase.GameEventManager.AvatarCommandsBuildMode.LaserCommands.ChangeState(LaserPointerState.Idle);
		MVGameControllerBase.GameEventManager.AvatarCommandsBuildMode.LaserCommands.SetLaserActiveState(isActive: false);
		e.MainCameraManager.IgnoreInputTypes(IgnoreInputTypes.None);
		e.MainCameraManager.TertiaryCameraActive = false;
		e.MainCameraManager.TertiaryCamera.ResetReplacementShader();
		foreach (TranslateData translateData in translateDatas)
		{
			if (translateData.Wo != null)
			{
				SharedCubeFunctions.SetLayerRecursively(translateData.Wo.GameObject.transform, select: false);
				translateData.Wo.SyncPos = translateData.prevGridifiedPosition;
			}
		}
		Cursor.visible = true;
		e.NetworkSelector.RequestReleaseOwnership(e.SelectedIDs);
		e.Data.Add("FromTranslateState", true);
	}

	private bool GetInitialAvatarMoveObjectHitDistance(EditorStateMachine e, ref float hitDistance)
	{
		VoxelHit hit = default;
		if (EditModeObjectPicker.Pick(ref hit) && hit.woId != -1 && e.SelectedIDs.Contains(hit.woId))
		{
			hitDistance = (hit.point - MVGameControllerBase.SpawnRoleDataMediatorLocal.Position).magnitude;
			return true;
		}
		return false;
	}

	private float GetInitialAvatarMoveObjectDistance(EditorStateMachine e)
	{
		float num = 0f;
		int num2 = 0;
		Vector3 vector = MVGameControllerBase.SpawnRoleDataMediatorLocal.Position;
		foreach (MVWorldObjectClient selectedWO in e.SelectedWOs)
		{
			num += (selectedWO.WorldPosition - vector).magnitude;
			num2++;
		}
		return num / (float)num2;
	}

	private void RotateWithCamera(EditorStateMachine e, int targetIndex)
	{
		if (recalcLocalDirCamToObjects)
		{
			Matrix4x4 matrix4x = default;
			if (!fixedToYPlane)
			{
				matrix4x = Matrix4x4.TRS(Vector3.zero, e.MainCameraManager.transform.rotation, Vector3.one);
			}
			else
			{
				float y = MathFunctions.Yaw(e.MainCameraManager.transform.rotation * Vector3.forward);
				Vector3 eulerAngles = new Vector3(0f, y, 0f);
				Quaternion identity = Quaternion.identity;
				identity.eulerAngles = eulerAngles;
				matrix4x = Matrix4x4.TRS(Vector3.zero, identity, Vector3.one);
			}
			matrix4x = Matrix4x4.Inverse(matrix4x);
			for (int i = 0; i < translateDatas.Count; i++)
			{
				translateDatas[i].localDirCamToObject = matrix4x.MultiplyVector((translateDatas[i].Wo.WorldPosition - MVGameControllerBase.SpawnRoleDataMediatorLocal.Position).normalized);
			}
			recalcLocalDirCamToObjects = false;
		}
		float magnitude = (translateDatas[targetIndex].Wo.WorldPosition - MVGameControllerBase.SpawnRoleDataMediatorLocal.Position).magnitude;
		Matrix4x4 matrix4x2 = default;
		if (!fixedToYPlane)
		{
			matrix4x2 = Matrix4x4.TRS(Vector3.zero, e.MainCameraManager.transform.rotation, Vector3.one);
		}
		else
		{
			float y2 = MathFunctions.Yaw(e.MainCameraManager.transform.rotation * Vector3.forward);
			Vector3 eulerAngles2 = new Vector3(0f, y2, 0f);
			Quaternion identity2 = Quaternion.identity;
			identity2.eulerAngles = eulerAngles2;
			matrix4x2 = Matrix4x4.TRS(Vector3.zero, identity2, Vector3.one);
		}
		Vector3 vector = matrix4x2.MultiplyVector(translateDatas[targetIndex].localDirCamToObject);
		translateDatas[targetIndex].ungridifiedPosition = MVGameControllerBase.SpawnRoleDataMediatorLocal.Position + vector * magnitude;
	}

	private Vector3 GetDeltaMouse(EditorStateMachine e)
	{
		Vector3 v = e.MainCameraManager.transform.rotation * Vector3.forward;
		v.y = 0f;
		v.Normalize();
		float y = MathFunctions.SignedAngle(Vector3.forward, v, Vector3.up) * 57.29578f;
		Quaternion quaternion = Quaternion.Euler(0f, y, 0f);
		return quaternion * translateMode switch
		{
			TranslateMode.Y => new Vector3(0f, MVInputWrapper.GetAxisRawWithoutSensitivity("Mouse Y") * initialDistance * 0.005f, 0f), 
			TranslateMode.XZ => new Vector3(MVInputWrapper.GetAxisRawWithoutSensitivity("Mouse X") * initialDistance * 0.005f, 0f, MVInputWrapper.GetAxisRawWithoutSensitivity("Mouse Y") * initialDistance * 0.005f), 
			TranslateMode.XY => new Vector3(MVInputWrapper.GetAxisRawWithoutSensitivity("Mouse X") * initialDistance * 0.005f, MVInputWrapper.GetAxisRawWithoutSensitivity("Mouse Y") * initialDistance * 0.005f, 0f), 
			_ => throw new Exception("Failed to set translate mode"), 
		};
	}
}
