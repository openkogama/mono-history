using System;
using System.Collections.Generic;
using UnityEngine;

internal class ESTranslate : ESStateBase
{
	private const float _mouseSensitivity = 0.005f;

	private float gridSize;

	private float stickyModifier = 0.2f;

	private float completelyStuckLimit = 0.3f;

	private bool recalcLocalDirCamToObjects = true;

	private List<TranslateData> translateDatas = new List<TranslateData>();

	private List<MVWorldObjectClient> targets = new List<MVWorldObjectClient>();

	private float initialDistance;

	private Vector3 originPrevFrame = Vector3.zero;

	private bool playTranslateSounds = true;

	private float scrollMoveDistance;

	private bool fixedToYPlane = true;

	private TranslateMode translateMode;

	private HashSet<int> woIds;

	private bool moveWithAvatar;

	private ILaserPointer laser;

	public override void Enter(EditorStateMachine e)
	{
		if (MVGameControllerLegacyUI.EditorController.IsGridSnap())
		{
			gridSize = 1f;
		}
		else
		{
			gridSize = 0.0625f;
		}
		translateMode = (TranslateMode)(int)e.Data["translateMode"];
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
		e.CameraController.IgnoreInputTypes(IgnoreInputTypes.MouseScroll | IgnoreInputTypes.Avatar);
		if (!e.NetworkSelector.RequestOwnership(e.SelectedIDs))
		{
			Debug.Log("Ownership request failed");
			e.PopState();
			return;
		}
		laser = MVGameControllerBase.WOCM.AvatarLocal.LaserPointer;
		laser.ChangeState(LaserPointerState.Transforming);
		laser.LaserActive = true;
		foreach (int selectedID in e.SelectedIDs)
		{
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(selectedID);
			targets.Add(worldObjectClient);
			translateDatas.Add(new TranslateData(worldObjectClient, gridSize));
		}
		e.CameraController.TertiaryCameraActive = true;
		e.CameraController.TertiaryCamera.SetReplacementShader(e.CameraController.transparentMultiplyColor, string.Empty);
		woIds = new HashSet<int>();
		foreach (TranslateData translateData in translateDatas)
		{
			MVGameControllerBase.WOCM.GetAllWoIds(translateData.wo.Id, woIds);
			SharedCubeFunctions.SetLayerRecursively(translateData.wo.Transform, select: true);
		}
		Cursor.visible = false;
		originPrevFrame = MVGameControllerBase.WOCM.AvatarLocal.GameObject.transform.position;
		UXUtils.UXInputDispatcher.BlockGUIInput = true;
	}

	public override void Execute(EditorStateMachine e)
	{
		base.Execute(e);
		if (!IsValid())
		{
			e.PopState();
		}
		if (!MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelect))
		{
			Vector3 deltaMouse = GetDeltaMouse(e);
			if (MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelectAlt))
			{
				recalcLocalDirCamToObjects = true;
			}
			Vector3 vector = MVGameControllerBase.WOCM.AvatarLocal.GameObject.transform.position - originPrevFrame;
			originPrevFrame = MVGameControllerBase.WOCM.AvatarLocal.GameObject.transform.position;
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
				translateDatas[i].gridifiedPosition = translateDatas[i].wo.GetClosestGridPoint(gridSize, translateDatas[i].ungridifiedPosition);
				if (gridSize < (translateDatas[i].prevGridifiedPosition - translateDatas[i].ungridifiedPosition).magnitude)
				{
					translateDatas[i].wo.SyncPos = translateDatas[i].gridifiedPosition;
					translateDatas[i].prevGridifiedPosition = translateDatas[i].gridifiedPosition;
					translateDatas[i].ungridifiedPosition = translateDatas[i].gridifiedPosition;
					if (playTranslateSounds)
					{
						AudioEventHandler.AddTranslateSoundData(0f, moveToGridPos: true, translateDatas[i].wo.WorldPosition);
					}
					continue;
				}
				Vector3 vector2 = translateDatas[i].ungridifiedPosition - translateDatas[i].prevGridifiedPosition;
				float magnitude = vector2.magnitude;
				if (magnitude > completelyStuckLimit * gridSize)
				{
					vector2 *= stickyModifier;
					translateDatas[i].wo.WorldPosition = translateDatas[i].prevGridifiedPosition + vector2;
				}
				if (playTranslateSounds)
				{
					AudioEventHandler.AddTranslateSoundData(magnitude, moveToGridPos: false, translateDatas[i].wo.WorldPosition);
				}
			}
			UpdateLaserPosition(targets);
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
			if (translateData.wo == null)
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
			laser.UpdatePosition(wos[0].WorldPivot);
			return;
		}
		List<Transform> list = new List<Transform>();
		foreach (MVWorldObjectClient wo in wos)
		{
			list.Add(wo.Transform);
		}
		laser.UpdatePosition(SharedCubeFunctions.GetWorldCenter(list));
	}

	public override void Exit(EditorStateMachine e)
	{
		laser.ChangeState(LaserPointerState.Idle);
		laser.LaserActive = false;
		e.CameraController.IgnoreInputTypes(IgnoreInputTypes.None);
		e.CameraController.TertiaryCameraActive = false;
		e.CameraController.TertiaryCamera.ResetReplacementShader();
		foreach (TranslateData translateData in translateDatas)
		{
			SharedCubeFunctions.SetLayerRecursively(translateData.wo.GameObject.transform, select: false);
			translateData.wo.SyncPos = translateData.prevGridifiedPosition;
		}
		Cursor.visible = true;
		e.NetworkSelector.RequestReleaseOwnership(e.SelectedIDs);
		UXUtils.UXInputDispatcher.BlockGUIInput = false;
	}

	private bool GetInitialAvatarMoveObjectHitDistance(EditorStateMachine e, ref float hitDistance)
	{
		VoxelHit hit = default;
		if (MVGameControllerLegacyUI.Pick(ref hit) && hit.woId != -1 && e.SelectedIDs.Contains(hit.woId))
		{
			hitDistance = (hit.point - MVGameControllerBase.WOCM.AvatarLocal.GameObject.transform.position).magnitude;
			return true;
		}
		return false;
	}

	private float GetInitialAvatarMoveObjectDistance(EditorStateMachine e)
	{
		float num = 0f;
		int num2 = 0;
		Vector3 position = MVGameControllerBase.WOCM.AvatarLocal.GameObject.transform.position;
		foreach (MVWorldObjectClient selectedWO in e.SelectedWOs)
		{
			num += (selectedWO.WorldPosition - position).magnitude;
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
				matrix4x = Matrix4x4.TRS(Vector3.zero, e.CameraController.transform.rotation, Vector3.one);
			}
			else
			{
				float y = MathFunctions.Yaw(e.CameraController.transform.rotation * Vector3.forward);
				Vector3 eulerAngles = new Vector3(0f, y, 0f);
				Quaternion identity = Quaternion.identity;
				identity.eulerAngles = eulerAngles;
				matrix4x = Matrix4x4.TRS(Vector3.zero, identity, Vector3.one);
			}
			matrix4x = Matrix4x4.Inverse(matrix4x);
			for (int i = 0; i < translateDatas.Count; i++)
			{
				translateDatas[i].localDirCamToObject = matrix4x.MultiplyVector((translateDatas[i].wo.WorldPosition - MVGameControllerBase.WOCM.AvatarLocal.GameObject.transform.position).normalized);
			}
			recalcLocalDirCamToObjects = false;
		}
		float magnitude = (translateDatas[targetIndex].wo.WorldPosition - MVGameControllerBase.WOCM.AvatarLocal.GameObject.transform.position).magnitude;
		Matrix4x4 matrix4x2 = default;
		if (!fixedToYPlane)
		{
			matrix4x2 = Matrix4x4.TRS(Vector3.zero, e.CameraController.transform.rotation, Vector3.one);
		}
		else
		{
			float y2 = MathFunctions.Yaw(e.CameraController.transform.rotation * Vector3.forward);
			Vector3 eulerAngles2 = new Vector3(0f, y2, 0f);
			Quaternion identity2 = Quaternion.identity;
			identity2.eulerAngles = eulerAngles2;
			matrix4x2 = Matrix4x4.TRS(Vector3.zero, identity2, Vector3.one);
		}
		Vector3 vector = matrix4x2.MultiplyVector(translateDatas[targetIndex].localDirCamToObject);
		translateDatas[targetIndex].ungridifiedPosition = MVGameControllerBase.WOCM.AvatarLocal.GameObject.transform.position + vector * magnitude;
	}

	private Vector3 GetDeltaMouse(EditorStateMachine e)
	{
		Vector3 v = e.CameraController.transform.rotation * Vector3.forward;
		v.y = 0f;
		v.Normalize();
		float y = MathFunctions.SignedAngle(Vector3.forward, v, Vector3.up) * 57.29578f;
		Quaternion quaternion = Quaternion.Euler(0f, y, 0f);
		return quaternion * translateMode switch
		{
			TranslateMode.Y => new Vector3(0f, MVInputWrapper.GetAxisRaw("Mouse Y") * initialDistance * 0.005f, 0f), 
			TranslateMode.XZ => new Vector3(MVInputWrapper.GetAxisRaw("Mouse X") * initialDistance * 0.005f, 0f, MVInputWrapper.GetAxisRaw("Mouse Y") * initialDistance * 0.005f), 
			TranslateMode.XY => new Vector3(MVInputWrapper.GetAxisRaw("Mouse X") * initialDistance * 0.005f, MVInputWrapper.GetAxisRaw("Mouse Y") * initialDistance * 0.005f, 0f), 
			_ => throw new Exception("Failed to set translate mode"), 
		};
	}
}
