public class FirstTimeActivatablePopupTimedParentToGameobject : FirstTimeActivatablePopupTimed
{
	public override void OnShow()
	{
		if (!isShown)
		{
			isShown = true;
			CreatePopup();
			ParentPopupToGameObject();
		}
	}
}
