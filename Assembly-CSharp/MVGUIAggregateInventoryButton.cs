public class MVGUIAggregateInventoryButton : UXViewScript
{
	public MVGUIAggregateInventory aggregateInventory;

	public UXIconButton inventoryIcon;

	public void ShowInventory()
	{
		MVGameController.EditorController.ShowInventory();
	}

	public override void OnShow()
	{
		base.OnShow();
		inventoryIcon.gameObject.SetActive(value: true);
	}

	public override void OnHide()
	{
		base.OnHide();
		inventoryIcon.gameObject.SetActive(value: false);
		aggregateInventory.View.Hide();
	}

	public override void OnInitialize()
	{
		inventoryIcon.OnClick = ShowInventory;
	}
}
