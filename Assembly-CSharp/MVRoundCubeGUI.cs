public class MVRoundCubeGUI : UXViewScript
{
	public bool wantToShow;

	public UXText gameMsgs;

	public override void OnShow()
	{
		if (wantToShow)
		{
			base.OnShow();
		}
	}

	public void Update()
	{
		if (!wantToShow)
		{
			View.Hide();
		}
		if (View.isVisible && wantToShow)
		{
			View.Show();
		}
	}
}
