using UnityEngine;

public class AvatarLocalBuildMode : MonoBehaviour
{
	[SerializeField]
	private AvatarCamerasDesktopBuildMode avatarCamerasDesktopPrefab;

	private AvatarCamerasDesktopBuildMode avatarCamerasDesktop;

	[SerializeField]
	private AvatarEnabledChangeHandler enabledChangeHandler;

	public AvatarEnabledChangeHandler EnabledChangeHandler => enabledChangeHandler;

	public AvatarCamerasDesktopBuildMode AvatarCamerasDesktopBuildMode => avatarCamerasDesktop;

	public void Initialize(MVBuildModeAvatarLocal buildModeAvatar)
	{
		avatarCamerasDesktop = Object.Instantiate(avatarCamerasDesktopPrefab);
		avatarCamerasDesktop.Initialize(buildModeAvatar);
	}

	public void Activate()
	{
		avatarCamerasDesktop.ActivateCameraController();
	}
}
