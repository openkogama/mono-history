using UnityEngine;

public class MVGUIAggregateInventoryButton : UXViewScript
{
	public MVGUIAggregateInventory aggregateInventory;

	public UXIconButton inventoryIcon;

	public void ShowInventory()
	{
		MVGameController.Instance.EditController.ShowInventory();
	}

	public override void OnShow()
	{
		base.OnShow();
		((Component)inventoryIcon).gameObject.SetActiveRecursively(true);
	}

	public override void OnHide()
	{
		base.OnHide();
		((Component)inventoryIcon).gameObject.SetActiveRecursively(false);
		aggregateInventory.View.Hide();
	}

	public override void OnInitialize()
	{
		inventoryIcon.OnClick = ShowInventory;
	}
}
