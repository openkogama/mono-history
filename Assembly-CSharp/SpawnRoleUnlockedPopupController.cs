using System;
using MV.Common;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpawnRoleUnlockedPopupController : MonoBehaviour
{
	[SerializeField]
	private Image Background;

	[SerializeField]
	private Text titleText;

	[SerializeField]
	private RectTransform spawnRoleContentTransform;

	[SerializeField]
	private RawImage spawnRolePreviewImage;

	[SerializeField]
	private ContinueButtonHandler continueButtonHandler;

	[SerializeField]
	private ContinueButtonHandler backgroundContinueButtonHandler;

	[SerializeField]
	private SpawnRolePreviewer spawnRolePreviewPrefab;

	[SerializeField]
	private TierUnlockedPopupController TierUnlockedPopupControllerPrefab;

	[SerializeField]
	protected int previewWidth;

	[SerializeField]
	protected int previewHeight;

	[SerializeField]
	private float bounceEffectDuration;

	[SerializeField]
	private AnimationCurve bounceEffect;

	private SpawnRolePreviewer spawnRolePreviewer;

	private GamePassTier unlockedTier;

	private bool wasPurchased;

	private bool wasTempUnlocked;

	private int spawnRoleWoId;

	private MVTeam team;

	private float bounceEffectStartTime;

	private bool awaitingSpawn;

	public void Initialize(GamePassTier unlockedTier, bool wasPurchased, bool wasTempUnlocked, int spawnRoleWoId)
	{
		this.unlockedTier = unlockedTier;
		this.wasPurchased = wasPurchased;
		this.wasTempUnlocked = wasTempUnlocked;
		this.spawnRoleWoId = spawnRoleWoId;
		StartEffect();
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.Game.WorldObjectClientManager.GetWorldObjectClient(spawnRoleWoId);
		MVAvatarSpawnRoleCreator mVAvatarSpawnRoleCreator = (MVAvatarSpawnRoleCreator)worldObjectClient;
		SetupPreviewImage(mVAvatarSpawnRoleCreator.GetSpawnRolePreviewObject());
		SetupColor(mVAvatarSpawnRoleCreator.Team);
		string text = TM._("NEW CLASS UNLOCKED!");
		if (wasTempUnlocked)
		{
			text = TM._("NEW CLASS IS NOW UNLOCKED UNTIL THE NEXT TIME YOU RESPAWN. ENJOY!");
		}
		titleText.text = text;
		team = mVAvatarSpawnRoleCreator.Team;
		ContinueButtonHandler continueButtonHandler = this.continueButtonHandler;
		continueButtonHandler.OnClick = (Action)Delegate.Combine(continueButtonHandler.OnClick, new Action(OnPressedPlay));
		ContinueButtonHandler continueButtonHandler2 = backgroundContinueButtonHandler;
		continueButtonHandler2.OnClick = (Action)Delegate.Combine(continueButtonHandler2.OnClick, new Action(OnPressedPlay));
	}

	public void OnPressedPlay()
	{
		if (MVGameControllerBase.Game.TeamManager.TeamCount() > 1)
		{
			HandleTeamSwitching();
		}
		bool flag = spawnRoleWoId == MVGameControllerBase.LocalPlayer.SpawnRoleDataMediator.WoId;
		if (!FirstTimePressPlayController.HaveBeenPressed)
		{
			FirstTimePressPlayController.OnFirstTimePlayIsPressed();
		}
		if (flag)
		{
			Close();
			StartPlaying();
		}
		else
		{
			AwaitSpawnThenClose();
			MVGameControllerBase.Game.LocalPlayer.CreateSpawnRole(spawnRoleWoId);
			MVGameControllerDesktop.LockCursorManager.CursorLockWithoutCallback = true;
		}
	}

	public void SeeTierReward()
	{
		TierUnlockedPopupController tierUnlockedPopupController = UnityEngine.Object.Instantiate(TierUnlockedPopupControllerPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(tierUnlockedPopupController.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
		tierUnlockedPopupController.Initialize(unlockedTier, wasPurchased, wasTempUnlocked);
	}

	private void SetupPreviewImage(GameObject spawnRolePreviewObject)
	{
		spawnRolePreviewer = UnityEngine.Object.Instantiate(spawnRolePreviewPrefab);
		GameObject gameObject = UnityEngine.Object.Instantiate(spawnRolePreviewObject);
		gameObject.transform.localRotation = Quaternion.identity;
		Transform previewSpawnRoleRoot = new GameObject("Preview Root - TierShopItem").transform;
		Vector3 previewPosition = new Vector3(500f, 500f, 0f);
		Vector3 cameraOffset = new Vector3(0f, 1f, -4.5f);
		spawnRolePreviewer.Initialize(previewWidth, previewHeight, CameraClearFlags.Color, LayerFlags.Default | LayerFlags.CamRotateTarget, cameraOffset, previewSpawnRoleRoot, previewPosition, "SpawnRole", 0, gameObject);
		spawnRolePreviewImage.texture = spawnRolePreviewer.PreviewTexture;
	}

	private void SetupColor(MVTeam spawnRoleTeam)
	{
		Background.color = GetColor(spawnRoleTeam);
	}

	private Color GetColor(MVTeam spawnRoleTeam)
	{
		if (MVGameControllerBase.Game.TeamManager.GetTeamList().Count <= 1)
		{
			return Styles.GetColor(ColorStyle.OffGray);
		}
		return Styles.GetTeamColor(spawnRoleTeam);
	}

	private void StartEffect()
	{
		bounceEffectStartTime = Time.time;
	}

	private void HandleTeamSwitching()
	{
		if (team != MVGameControllerBase.Game.LocalPlayer.Team)
		{
			MVGameControllerBase.OperationRequests.SetTeam(team);
			MVGameControllerBase.Game.GameStatCounterManager.RemoveTeamScoreOnActorLeave(MVGameControllerBase.Game.LocalPlayer.ActorNr, MVGameControllerBase.Game.LocalPlayer.Team);
			MVGameControllerBase.Game.LocalPlayer.ResetCheckpoint();
			MVGameControllerBase.Game.LocalPlayer.Team = team;
		}
	}

	private void Close(int spawnRoleID = 0)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.Pop();
		});
		MVGameControllerBase.MainCameraManager.CamMaskMode = MaskMode.Default;
	}

	private void AwaitSpawnThenClose()
	{
		awaitingSpawn = true;
		MVGameControllerBase.LocalPlayer.SpawnRolesManager.OnSpawnRoleActivated += Close;
	}

	private void StartPlaying()
	{
		MVGameControllerDesktop.LockCursorManager.CursorLock = true;
		if (MVGameControllerBase.GameSessionData.gameMode != MVGameMode.Edit || (MVGameControllerBase.GameSessionData.gameMode == MVGameMode.Edit && MVGameControllerBase.EditModeUI.IsInPlayInEditMode))
		{
			MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.SetToSpawnPoint();
		}
	}

	private void Update()
	{
		float num = bounceEffect.Evaluate((Time.time - bounceEffectStartTime) / bounceEffectDuration);
		spawnRoleContentTransform.localScale = new Vector3(num, num, 1f);
	}

	private void OnDestroy()
	{
		UnityEngine.Object.Destroy(spawnRolePreviewer);
		if (awaitingSpawn)
		{
			MVGameControllerBase.LocalPlayer.SpawnRolesManager.OnSpawnRoleActivated -= Close;
		}
		ContinueButtonHandler continueButtonHandler = this.continueButtonHandler;
		continueButtonHandler.OnClick = (Action)Delegate.Remove(continueButtonHandler.OnClick, new Action(OnPressedPlay));
		ContinueButtonHandler continueButtonHandler2 = backgroundContinueButtonHandler;
		continueButtonHandler2.OnClick = (Action)Delegate.Remove(continueButtonHandler2.OnClick, new Action(OnPressedPlay));
	}
}
