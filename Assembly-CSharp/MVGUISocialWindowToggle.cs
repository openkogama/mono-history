using UnityEngine;

public class MVGUISocialWindowToggle : UXViewScript
{
	public UXView socialView;

	public UXIconButton socialIcon;

	private bool toggle;

	public override void OnInitialize()
	{
		socialIcon.OnClick = ToggleSocial;
	}

	private void ToggleSocial()
	{
		toggle = !toggle;
		if (toggle)
		{
			socialView.Show();
		}
		else
		{
			socialView.Hide();
		}
	}

	public override void OnShow()
	{
		((Component)socialIcon).gameObject.SetActiveRecursively(true);
	}

	public override void OnHide()
	{
		((Component)socialIcon).gameObject.SetActiveRecursively(false);
		socialView.Hide();
	}
}
