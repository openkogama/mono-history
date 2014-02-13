public class MVGUIShopView : UXViewScript
{
	public UXGroup shopElements;

	public UXText silverMoney;

	public UXText goldMoney;

	public UXIconButton shopButton;

	public MVGUIAggregateInventory shopInventory;

	public override void OnInitialize()
	{
		base.OnInitialize();
		shopButton.OnClick = ShowInventory;
	}

	public void ShowInventory()
	{
		MVGameController.Instance.EditController.ShowShopInventory();
	}

	public override void OnShow()
	{
		shopElements.Show();
	}

	public override void OnHide()
	{
		shopElements.Hide();
	}
}
