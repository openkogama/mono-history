using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class MVGameControllerLegacyUI : MVGameControllerBase, IInputHandler
{
	public int Priority => InputHandlerPriority.GAME;

	public static AIngameController IngameController { get; private set; }

	public static CharacterEditorController CharacterEditorController
	{
		get
		{
			if (MVGameControllerBase.GameSessionData.gameMode != MVGameMode.CharacterEditor)
			{
				return null;
			}
			return (CharacterEditorController)IngameController;
		}
	}

	public static EditorController EditorController => (EditorController)IngameController;

	public static ICubeModelingEditMode CubeModelingEditMode => (ICubeModelingEditMode)IngameController;

	public static PlayControllerBase PlayController
	{
		get
		{
			if (MVGameControllerBase.GameSessionData.gameMode == MVGameMode.Play)
			{
				return IngameController as PlayControllerBase;
			}
			if (MVGameControllerBase.GameSessionData.gameMode == MVGameMode.Edit)
			{
				return ((EditorController)IngameController).PlayController;
			}
			Debug.LogError("PlayController does not exist");
			return null;
		}
	}

	protected override bool IsPlayingInternal
	{
		get
		{
			if (MVGameControllerBase.JoinState != MVJoinState.Playing)
			{
				return false;
			}
			return MVGameControllerBase.GameSessionData.gameMode == MVGameMode.Play || (MVGameControllerBase.GameSessionData.gameMode == MVGameMode.Edit && EditorController.PlayInEditor);
		}
	}

	protected override void CreateInGameController()
	{
		switch (MVGameControllerBase.GameSessionData.gameMode)
		{
		case MVGameMode.Play:
			IngameController = (AIngameController)(MVGameControllerBase.playModeUI = ((!MVGameControllerBase.IsTouristSession) ? ((PlayControllerBase)new PlayController()) : ((PlayControllerBase)new PlayControllerTourist())));
			break;
		case MVGameMode.Edit:
			if (MVGameControllerBase.Game.GameType == MVGameType.Classic)
			{
				IngameController = new EditorController3D();
			}
			else if (MVGameControllerBase.Game.GameType == MVGameType.Platformer)
			{
				IngameController = new EditorController2D();
			}
			MVGameControllerBase.playModeUI = EditorController;
			MVGameControllerBase.editModeUI = EditorController;
			break;
		case MVGameMode.CharacterEditor:
			IngameController = new CharacterEditorController();
			break;
		}
	}

	public static bool Pick(ref VoxelHit hit, HashSet<int> ignoreWoIds = null, int layerMask = -5)
	{
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(MVInputWrapper.GetPointerPosition().x, MVInputWrapper.GetPointerPosition().y));
		float num = 0f;
		bool flag = false;
		if (IngameController != null && CubeModelingEditMode.DrawPlaneController.IsDrawPlaneActive)
		{
			Vector3 hit2 = Vector3.zero;
			if (CubeModelingEditMode.DrawPlaneController.Pick(ref hit2))
			{
				num = (hit2 - ray.origin).magnitude;
				flag = true;
			}
		}
		List<VoxelHit> list = CollisionDetection.MVHitAll(ray, float.PositiveInfinity, ignoreWoIds, layerMask);
		if (list.Count == 0)
		{
			return false;
		}
		float num2 = float.PositiveInfinity;
		bool result = false;
		foreach (VoxelHit item in list)
		{
			if ((item.distance < num || !flag) && item.transform.gameObject.activeInHierarchy && (item.transform.gameObject.layer != LayerMask.NameToLayer("Logic") || MVGameControllerBase.CameraController.IsLogicRendered || IsHitPickup(item)))
			{
				float num3 = Vector3.Distance(ray.origin, item.point);
				if (num3 < num2)
				{
					num2 = num3;
					hit = item;
					result = true;
				}
			}
		}
		return result;
	}

	private static bool IsHitPickup(VoxelHit hit)
	{
		Transform parent = hit.transform.parent;
		return parent.GetComponent<GreyOutObjectScript>() != null;
	}

	protected override void InitWebPlayer(bool developmentMode)
	{
		base.InitWebPlayer(developmentMode);
		UXScreen uXScreen = UXUtils.FindGUIObjectOfType<UXScreen>();
		uXScreen.Init(Screen.width, Screen.height);
	}

	protected override void InitStandAlone(bool developmentMode)
	{
		base.InitStandAlone(developmentMode);
		UXScreen uXScreen = UXUtils.FindGUIObjectOfType<UXScreen>();
		uXScreen.Fullscreen = false;
		Screen.SetResolution(940, 482, fullscreen: false);
		uXScreen.Init(940, 482);
	}

	protected override void UpdateInternal()
	{
		if (!MVGameControllerBase.isInitialized)
		{
			if (MVGameControllerBase.GameMode == MVGameMode.CharacterEditor)
			{
				MVInputWrapper.SetInputMap(new DesktopPlayMode());
			}
			else if (MVGameControllerBase.Game.GameType == MVGameType.Classic)
			{
				MVInputWrapper.SetInputMap(new DesktopPlayMode());
			}
			else if (MVGameControllerBase.Game.GameType == MVGameType.Platformer)
			{
				MVInputWrapper.SetInputMap(new Desktop2DPlayMode());
			}
			IngameController.Initialize();
			Initialize();
		}
		IngameController.Update();
	}

	public bool HandleInput()
	{
		if (MVGameControllerBase.Game != null && MVGameControllerBase.JoinState == MVJoinState.Playing && IngameController.IsInitialized)
		{
			IngameController.HandleInput();
		}
		return false;
	}

	protected override void CleanUp()
	{
		base.CleanUp();
		IngameController = null;
	}
}
