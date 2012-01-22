using System.Collections.Generic;
using UnityEngine;

internal class ESTranslate : ESStateBase
{
	private const float _mouseSensitivity = 2.5f;

	private const bool _moveWithAvatar = true;

	private const float minInitialDistance = 2.5f;

	private const float _moveWithSmallestGridSizeThres = 25f;

	private float gridSize;

	private float stickyModifier = 0.2f;

	private float completelyStuckLimit = 0.3f;

	private bool recalcLocalDirCamToObjects = true;

	private List<TranslateData> translateDatas = new List<TranslateData>();

	private float initialDistance;

	private Vector3 originPrevFrame = Vector3.zero;

	private bool playTranslateSounds = true;

	private float scrollMoveDistance;

	private bool lockY;

	private HashSet<int> woIds;

	public ESTranslate()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
	}

	public override void Enter(EditorStateMachine e)
	{
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		if (MVGameController.Instance.EditorController.GridSnap)
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
		if (initialDistance < 25f)
		{
			Debug.Log((object)"is moving with smallest size");
		}
		recalcLocalDirCamToObjects = true;
		translateDatas = new List<TranslateData>();
		e.WeCamera.IgnoreInputTypes(IgnoreInputTypes.MouseScroll | IgnoreInputTypes.Avatar);
		if (!e.NetworkSelector.RequestOwnership(e.Selected))
		{
			e.PopState();
			return;
		}
		foreach (int item in e.Selected)
		{
			MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(item);
			translateDatas.Add(new TranslateData(worldObjectClient, gridSize));
		}
		e.WeCamera.TertiaryCameraActive = true;
		e.WeCamera.TertiaryCamera.SetReplacementShader(e.WeCamera.transparentMultiplyColor, string.Empty);
		woIds = new HashSet<int>();
		foreach (TranslateData translateData in translateDatas)
		{
			MVGameController.Instance.WOCM.GetAllWoIds(translateData.wo.Id, woIds);
			SharedCubeFunctions.SetLayerRecursively(translateData.wo.GameObject.transform, select: true);
		}
		Screen.showCursor = false;
		originPrevFrame = MVGameController.Instance.WOCM.WoAvatar.GameObject.transform.position;
	}

	public override void Execute(EditorStateMachine e)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		base.Execute(e);
		if (!MVInputWrapper.GetKeyUp((KeyCode)323))
		{
			Vector3 deltaMouse = GetDeltaMouse(e);
			if (MVInputWrapper.GetKeyUp((KeyCode)324))
			{
				recalcLocalDirCamToObjects = true;
			}
			Vector3 val = MVGameController.Instance.WOCM.WoAvatar.GameObject.transform.position - originPrevFrame;
			originPrevFrame = MVGameController.Instance.WOCM.WoAvatar.GameObject.transform.position;
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
					translateDatas[i].wo.GameObject.transform.position = translateDatas[i].gridifiedPosition;
					translateDatas[i].wo.SyncPos = translateDatas[i].wo.GameObject.transform.position;
					translateDatas[i].prevGridifiedPosition = translateDatas[i].gridifiedPosition;
					translateDatas[i].ungridifiedPosition = translateDatas[i].gridifiedPosition;
					if (playTranslateSounds)
					{
						AudioEventHandler.AddTranslateSoundData(0f, moveToGridPos: true, translateDatas[i].wo.GameObject.transform.position);
					}
					continue;
				}
				Vector3 val3 = translateDatas[i].ungridifiedPosition - translateDatas[i].prevGridifiedPosition;
				float magnitude = val3.magnitude;
				if (magnitude > completelyStuckLimit * gridSize)
				{
					val3 *= stickyModifier;
					translateDatas[i].wo.GameObject.transform.position = translateDatas[i].prevGridifiedPosition + val3;
				}
				if (playTranslateSounds)
				{
					AudioEventHandler.AddTranslateSoundData(magnitude, moveToGridPos: false, translateDatas[i].wo.GameObject.transform.position);
				}
			}
		}
		else
		{
			e.PopState();
		}
	}

	public override void Exit(EditorStateMachine e)
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		Debug.Log((object)("leaving " + GetType()));
		e.WeCamera.IgnoreInputTypes(IgnoreInputTypes.None);
		e.WeCamera.TertiaryCameraActive = false;
		e.WeCamera.TertiaryCamera.ResetReplacementShader();
		foreach (TranslateData translateData in translateDatas)
		{
			SharedCubeFunctions.SetLayerRecursively(translateData.wo.GameObject.transform, select: false);
			translateData.wo.GameObject.transform.position = translateData.prevGridifiedPosition;
			translateData.wo.SyncPos = translateData.prevGridifiedPosition;
		}
		Screen.showCursor = true;
		e.NetworkSelector.RequestReleaseOwnership(e.Selected);
	}

	private bool GetInitialCamMoveObjectHitDistance(EditorStateMachine e, ref float hitDistance)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		VoxelHit hit = default;
		if (MVGameController.Instance.WOCM.Pick(ref hit) && hit.woId != -1)
		{
			Vector3 val = hit.point - ((Component)e.WeCamera).transform.position;
			hitDistance = val.magnitude;
			return true;
		}
		return false;
	}

	private bool GetInitialAvatarMoveObjectHitDistance(EditorStateMachine e, ref float hitDistance)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		VoxelHit hit = default;
		if (MVGameController.Instance.WOCM.Pick(ref hit) && hit.woId != -1 && e.Selected.Contains(hit.woId))
		{
			Vector3 val = hit.point - MVGameController.Instance.WOCM.WoAvatar.GameObject.transform.position;
			hitDistance = val.magnitude;
			return true;
		}
		return false;
	}

	private float GetInitialAvatarMoveObjectDistance(EditorStateMachine e)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		int num2 = 0;
		Vector3 position = MVGameController.Instance.WOCM.WoAvatar.GameObject.transform.position;
		foreach (MVWorldObjectClient selectedWO in e.SelectedWOs)
		{
			float num3 = num;
			Vector3 val = selectedWO.GameObject.transform.position - position;
			num = num3 + val.magnitude;
			num2++;
		}
		return num / (float)num2;
	}

	private void RotateWithCamera(EditorStateMachine e, int targetIndex)
	{
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		if (recalcLocalDirCamToObjects)
		{
			Matrix4x4 val = default;
			if (!MVGameController.Instance.fixedToYPlane)
			{
				val = Matrix4x4.TRS(Vector3.zero, ((Component)e.WeCamera).transform.rotation, Vector3.one);
			}
			else
			{
				float num = MathFunctions.Yaw(((Component)e.WeCamera).transform.rotation * Vector3.forward);
				Vector3 eulerAngles = new Vector3(0f, num, 0f);
				Quaternion identity = Quaternion.identity;
				identity.eulerAngles = eulerAngles;
				val = Matrix4x4.TRS(Vector3.zero, identity, Vector3.one);
			}
			val = Matrix4x4.Inverse(val);
			for (int i = 0; i < translateDatas.Count; i++)
			{
				TranslateData translateData = translateDatas[i];
				Vector3 val2 = translateDatas[i].wo.GameObject.transform.position - MVGameController.Instance.WOCM.WoAvatar.GameObject.transform.position;
				translateData.localDirCamToObject = val.MultiplyVector(val2.normalized);
			}
			recalcLocalDirCamToObjects = false;
		}
		Vector3 val3 = translateDatas[targetIndex].wo.GameObject.transform.position - MVGameController.Instance.WOCM.WoAvatar.GameObject.transform.position;
		float magnitude = val3.magnitude;
		Matrix4x4 val4 = default;
		if (!MVGameController.Instance.fixedToYPlane)
		{
			val4 = Matrix4x4.TRS(Vector3.zero, ((Component)e.WeCamera).transform.rotation, Vector3.one);
		}
		else
		{
			float num2 = MathFunctions.Yaw(((Component)e.WeCamera).transform.rotation * Vector3.forward);
			Vector3 eulerAngles2 = new Vector3(0f, num2, 0f);
			Quaternion identity2 = Quaternion.identity;
			identity2.eulerAngles = eulerAngles2;
			val4 = Matrix4x4.TRS(Vector3.zero, identity2, Vector3.one);
		}
		Vector3 val5 = val4.MultiplyVector(translateDatas[targetIndex].localDirCamToObject);
		translateDatas[targetIndex].ungridifiedPosition = MVGameController.Instance.WOCM.WoAvatar.GameObject.transform.position + val5 * magnitude;
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
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		Vector3 v = ((Component)e.WeCamera).transform.rotation * Vector3.forward;
		v.y = 0f;
		v.Normalize();
		float num = MathFunctions.SignedAngle(Vector3.forward, v, Vector3.up) * 57.29578f;
		Quaternion val = Quaternion.Euler(0f, num, 0f);
		Vector3 val2 = ((!MVInputWrapper.GetKey((KeyCode)304) && !MVInputWrapper.GetKey((KeyCode)303) && !lockY) ? new Vector3(Input.GetAxis("Mouse X") * initialDistance * 2.5f * Mathf.Clamp(Time.deltaTime, 0f, 0.03f), 0f, Input.GetAxis("Mouse Y") * initialDistance * 2.5f * Mathf.Clamp(Time.deltaTime, 0f, 0.03f)) : new Vector3(0f, Input.GetAxis("Mouse Y") * initialDistance * 2.5f * Mathf.Clamp(Time.deltaTime, 0f, 0.03f), 0f));
		return val * val2;
	}
}
