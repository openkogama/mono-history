using System;
using MV.Common;
using UnityEngine;

public class PlayModeController : ModeControllerBase, IPlayModeUI
{
	private bool showingEquipableUI;

	private bool inLobbyState = true;

	[SerializeField]
	private GameObject eventSystem;

	[SerializeField]
	private RectTransform lobbyState;

	[SerializeField]
	private RectTransform mobileControlRig;

	[SerializeField]
	private RectTransform use;

	[SerializeField]
	private RectTransform fire;

	[SerializeField]
	private RectTransform dropWeapon;

	[SerializeField]
	private RectTransform leaveVehicle;

	[SerializeField]
	private CrossHair crossHair;

	private Action<bool> OnLobbyStateChange;

	public bool InLobbyState
	{
		get
		{
			return inLobbyState;
		}
		set
		{
			inLobbyState = value;
			if (OnLobbyStateChange != null)
			{
				OnLobbyStateChange(inLobbyState);
			}
		}
	}

	private void Awake()
	{
		MVGameControllerTouch.RegisterPlayModeController(this);
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		UnityEngine.Object.DontDestroyOnLoad(eventSystem);
		OnLobbyStateChange = (Action<bool>)Delegate.Combine(OnLobbyStateChange, new Action<bool>(LobbyStateChange));
	}

	public override void Initialize()
	{
		base.Initialize();
		if (MVGameControllerBase.Game.GameType == MVGameType.Classic)
		{
			MVInputWrapper.SetInputMap(new MobileInputMap());
		}
		else if (MVGameControllerBase.Game.GameType == MVGameType.Platformer)
		{
			MVInputWrapper.SetInputMap(new Android2DPlayMode());
		}
		Debug.Log("Initialized");
	}

	private void Update()
	{
		if (MVGameControllerBase.IsInitialized)
		{
			if (MVInputWrapper.GetBooleanControlDown(KogamaControls.Respawn))
			{
				RespawnAvatar();
			}
			if (PickupGUI.ShowEquipableUI != showingEquipableUI)
			{
				fire.gameObject.SetActive(PickupGUI.ShowEquipableUI);
				dropWeapon.gameObject.SetActive(PickupGUI.ShowEquipableUI);
				showingEquipableUI = PickupGUI.ShowEquipableUI;
			}
			if (MVGameControllerBase.WOCM.AvatarLocal.IsSeated != leaveVehicle.gameObject.activeInHierarchy)
			{
				leaveVehicle.gameObject.SetActive(MVGameControllerBase.WOCM.AvatarLocal.IsSeated);
			}
		}
	}

	private void RespawnAvatar()
	{
		MVGameControllerBase.WOCM.AvatarLocal.Respawn();
	}

	private void LobbyStateChange(bool inLobbyState)
	{
		lobbyState.gameObject.SetActive(inLobbyState);
		mobileControlRig.gameObject.SetActive(!inLobbyState);
	}

	public void Play()
	{
		InLobbyState = false;
		MVGameControllerBase.WOCM.AvatarLocal.SetMode(AvatarRuntimeState.Playing);
	}

	public void ShowEUseIcon(ShowUseOption option, int levelRequirement = 0)
	{
		use.gameObject.SetActive(value: true);
	}

	public void HideEUseIcon()
	{
		use.gameObject.SetActive(value: false);
	}

	public IGUICrossHair GetCrossHair()
	{
		return crossHair;
	}
}
