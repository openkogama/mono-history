public class FirstTimeActivatableRewardFirstLobby : FirstTimeActivatableSetEventOnShow
{
	public override void OnShow()
	{
		base.OnShow();
		gameObject.AddComponent<FirstTimeAvatarJumpAnimator>();
	}
}
