using MV.Common;
using UnityEngine;

public class DesktopLobbyStateController : MonoBehaviour
{
	[SerializeField]
	private GameObject teamButton;

	[SerializeField]
	private GameObject respawnButton;

	[SerializeField]
	private GameObject gameCoinBoosterButton;

	[SerializeField]
	private GameObject avatarAccessoriesButton;

	[SerializeField]
	private GameObject touristRegisterButton;

	private readonly AccessoryMover accessoryMover = new AccessoryMover();

	private void Start()
	{
		touristRegisterButton.SetActive(MVGameControllerBase.IsTouristSession);
		gameCoinBoosterButton.SetActive(!MVGameControllerBase.IsTouristSession);
		avatarAccessoriesButton.SetActive(!MVGameControllerBase.IsTouristSession);
	}

	private void Update()
	{
		if (MVGameControllerBase.WOCM.AvatarLocal.AvatarRuntimeState != AvatarRuntimeState.Playing && respawnButton.gameObject.activeSelf)
		{
			respawnButton.gameObject.SetActive(value: false);
		}
		else if (MVGameControllerBase.WOCM.AvatarLocal.AvatarRuntimeState == AvatarRuntimeState.Playing && !respawnButton.gameObject.activeSelf)
		{
			respawnButton.gameObject.SetActive(value: true);
		}
		MVInputWrapper.IsInGameInputSuppressed = true;
		accessoryMover.MoveAccessory();
	}

	private void OnDisable()
	{
		accessoryMover.Destroy();
	}

	private void OnEnable()
	{
		accessoryMover.Activate();
		if (MVGameControllerBase.Game.TeamManager.TeamCount() > 1)
		{
			teamButton.SetActive(value: true);
		}
		else
		{
			teamButton.SetActive(value: false);
		}
	}
}
