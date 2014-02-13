using System.Collections.Generic;
using UnityEngine;

internal class ESTranslate : ESStateBase
{
	private const float _mouseSensitivity = 0.05f;

	private const bool _moveWithAvatar = true;

	private const float minInitialDistance = 2.5f;

	private const float _moveWithSmallestGridSizeThres = 25f;

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

	private bool lockY;

	private HashSet<int> woIds;

	private ILaserPointer laser;

	public ESTranslate()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
	}

	public override void Enter(EditorStateMachine e)
	{
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		if (AEditController.IsGridSnap())
		{
			gridSize = 1f;
		}
		else
		{
			gridSize = 0.0625f;
		}
		lockY = false;
		if (e.Data.Contains("yOnlyTranslate"))
		{
			lockY = true;
		}
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
			Debug.Log((object)"Ownership request failed");
			e.PopState();
			return;
		}
		laser = MVGameController.Instance.WOCM.AvatarLocal.LaserPointer;
		laser.ChangeState(LaserPointerState.Transforming);
		laser.LaserActive = true;
		foreach (int selectedID in e.SelectedIDs)
		{
			MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(selectedID);
			targets.Add(worldObjectClient);
			translateDatas.Add(new TranslateData(worldObjectClient, gridSize));
		}
		e.CameraController.TertiaryCameraActive = true;
		e.CameraController.TertiaryCamera.SetReplacementShader(e.CameraController.transparentMultiplyColor, string.Empty);
		woIds = new HashSet<int>();
		foreach (TranslateData translateData in translateDatas)
		{
			MVGameController.Instance.WOCM.GetAllWoIds(translateData.wo.Id, woIds);
			SharedCubeFunctions.SetLayerRecursively(translateData.wo.Transform, select: true);
		}
		Screen.showCursor = false;
		originPrevFrame = MVGameController.Instance.WOCM.AvatarLocal.GameObject.transform.position;
		UXUtils.FindGUIObjectOfType<UXInputDispatcher>().BlockGUIInput = true;
	}

	public override void Execute(EditorStateMachine e)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		base.Execute(e);
		if (!MVInputWrapper.GetKeyUp((KeyCode)323))
		{
			Vector3 deltaMouse = GetDeltaMouse(e);
			if (MVInputWrapper.GetKeyUp((KeyCode)324))
			{
				recalcLocalDirCamToObjects = true;
			}
			Vector3 val = MVGameController.Instance.WOCM.AvatarLocal.GameObject.transform.position - originPrevFrame;
			originPrevFrame = MVGameController.Instance.WOCM.AvatarLocal.GameObject.transform.position;
			for (int i = 0; i < translateDatas.Count; i++)
			{
				TranslateData translateData = translateDatas[i];
				translateData.ungridifiedPosition += val;
				if (MVInputWrapper.GetKey((KeyCode)324))
				{
					RotateWithCamera(e, i);
				}
				else
				{
					TranslateData translateData2 = translateDatas[i];
					translateData2.ungridifiedPosition += deltaMouse;
				}
				translateDatas[i].gridifiedPosition = translateDatas[i].wo.GetClosestGridPoint(gridSize, translateDatas[i].ungridifiedPosition);
				float num = gridSize;
				Vector3 val2 = translateDatas[i].prevGridifiedPosition - translateDatas[i].ungridifiedPosition;
				if (num < val2.magnitude)
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
				Vector3 val3 = translateDatas[i].ungridifiedPosition - translateDatas[i].prevGridifiedPosition;
				float magnitude = val3.magnitude;
				if (magnitude > completelyStuckLimit * gridSize)
				{
					val3 *= stickyModifier;
					translateDatas[i].wo.WorldPosition = translateDatas[i].prevGridifiedPosition + val3;
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

	private void UpdateLaserPosition(List<MVWorldObjectClient> wos)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
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
		Screen.showCursor = true;
		e.NetworkSelector.RequestReleaseOwnership(e.SelectedIDs);
		UXUtils.FindGUIObjectOfType<UXInputDispatcher>().BlockGUIInput = false;
	}

	private bool GetInitialCamMoveObjectHitDistance(EditorStateMachine e, ref float hitDistance)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		VoxelHit hit = default;
		if (MVGameController.Instance.WOCM.Pick(ref hit) && hit.woId != -1)
		{
			Vector3 val = hit.point - ((Component)e.CameraController).transform.position;
			hitDistance = val.magnitude;
			return true;
		}
		return false;
	}

	private bool GetInitialAvatarMoveObjectHitDistance(EditorStateMachine e, ref float hitDistance)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		VoxelHit hit = default;
		if (MVGameController.Instance.WOCM.Pick(ref hit) && hit.woId != -1 && e.SelectedIDs.Contains(hit.woId))
		{
			Vector3 val = hit.point - MVGameController.Instance.WOCM.AvatarLocal.GameObject.transform.position;
			hitDistance = val.magnitude;
			return true;
		}
		return false;
	}

	private float GetInitialAvatarMoveObjectDistance(EditorStateMachine e)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		int num2 = 0;
		Vector3 position = MVGameController.Instance.WOCM.AvatarLocal.GameObject.transform.position;
		foreach (MVWorldObjectClient selectedWO in e.SelectedWOs)
		{
			float num3 = num;
			Vector3 val = selectedWO.WorldPosition - position;
			num = num3 + val.magnitude;
			num2++;
		}
		return num / (float)num2;
	}

	private void RotateWithCamera(EditorStateMachine e, int targetIndex)
	{
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		if (recalcLocalDirCamToObjects)
		{
			Matrix4x4 val = default;
			if (!fixedToYPlane)
			{
				val = Matrix4x4.TRS(Vector3.zero, ((Component)e.CameraController).transform.rotation, Vector3.one);
			}
			else
			{
				float num = MathFunctions.Yaw(((Component)e.CameraController).transform.rotation * Vector3.forward);
				Vector3 eulerAngles = new Vector3(0f, num, 0f);
				Quaternion identity = Quaternion.identity;
				identity.eulerAngles = eulerAngles;
				val = Matrix4x4.TRS(Vector3.zero, identity, Vector3.one);
			}
			val = Matrix4x4.Inverse(val);
			for (int i = 0; i < translateDatas.Count; i++)
			{
				TranslateData translateData = translateDatas[i];
				Vector3 val2 = translateDatas[i].wo.WorldPosition - MVGameController.Instance.WOCM.AvatarLocal.GameObject.transform.position;
				translateData.localDirCamToObject = val.MultiplyVector(val2.normalized);
			}
			recalcLocalDirCamToObjects = false;
		}
		Vector3 val3 = translateDatas[targetIndex].wo.WorldPosition - MVGameController.Instance.WOCM.AvatarLocal.GameObject.transform.position;
		float magnitude = val3.magnitude;
		Matrix4x4 val4 = default;
		if (!fixedToYPlane)
		{
			val4 = Matrix4x4.TRS(Vector3.zero, ((Component)e.CameraController).transform.rotation, Vector3.one);
		}
		else
		{
			float num2 = MathFunctions.Yaw(((Component)e.CameraController).transform.rotation * Vector3.forward);
			Vector3 eulerAngles2 = new Vector3(0f, num2, 0f);
			Quaternion identity2 = Quaternion.identity;
			identity2.eulerAngles = eulerAngles2;
			val4 = Matrix4x4.TRS(Vector3.zero, identity2, Vector3.one);
		}
		Vector3 val5 = val4.MultiplyVector(translateDatas[targetIndex].localDirCamToObject);
		translateDatas[targetIndex].ungridifiedPosition = MVGameController.Instance.WOCM.AvatarLocal.GameObject.transform.position + val5 * magnitude;
	}

	private Vector3 GetDeltaMouse(EditorStateMachine e)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		Vector3 v = ((Component)e.CameraController).transform.rotation * Vector3.forward;
		v.y = 0f;
		v.Normalize();
		float num = MathFunctions.SignedAngle(Vector3.forward, v, Vector3.up) * 57.29578f;
		Quaternion val = Quaternion.Euler(0f, num, 0f);
		Vector3 val2 = ((!lockY) ? new Vector3(Input.GetAxisRaw("Mouse X") * initialDistance * 0.05f, 0f, Input.GetAxisRaw("Mouse Y") * initialDistance * 0.05f) : new Vector3(0f, Input.GetAxisRaw("Mouse Y") * initialDistance * 0.05f, 0f));
		return val * val2;
	}
}
