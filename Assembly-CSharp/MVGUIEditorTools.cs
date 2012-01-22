using UnityEngine;

public class MVGUIEditorTools : UXViewScript
{
	public MVGUICurrentSelectedMaterialCube currentSelectedMaterialCube;

	public UXMouseClickObject toolboxIcon;

	public UXMouseClickObject pickupIcon;

	public UXToggleIconButton play;

	public UXIconButton newModelButton;

	public UXGroup editingTools;

	public override void OnShow()
	{
		((Component)currentSelectedMaterialCube).gameObject.SetActiveRecursively(true);
		((Component)toolboxIcon).gameObject.SetActiveRecursively(true);
		((Component)pickupIcon).gameObject.SetActiveRecursively(true);
		((Component)play).gameObject.SetActiveRecursively(true);
		((Component)newModelButton).gameObject.SetActiveRecursively(true);
	}

	public override void OnHide()
	{
		((Component)currentSelectedMaterialCube).gameObject.SetActiveRecursively(false);
		((Component)toolboxIcon).gameObject.SetActiveRecursively(false);
		((Component)pickupIcon).gameObject.SetActiveRecursively(false);
		((Component)play).gameObject.SetActiveRecursively(false);
		((Component)newModelButton).gameObject.SetActiveRecursively(false);
	}

	public void HideEditingTools()
	{
		((Component)newModelButton).gameObject.SetActiveRecursively(false);
		((Component)currentSelectedMaterialCube).gameObject.SetActiveRecursively(false);
		((Component)toolboxIcon).gameObject.SetActiveRecursively(false);
		((Component)pickupIcon).gameObject.SetActiveRecursively(false);
		editingTools.Hide();
	}

	public void ShowEditingTools()
	{
		((Component)newModelButton).gameObject.SetActiveRecursively(true);
		((Component)currentSelectedMaterialCube).gameObject.SetActiveRecursively(true);
		((Component)toolboxIcon).gameObject.SetActiveRecursively(true);
		((Component)pickupIcon).gameObject.SetActiveRecursively(true);
		editingTools.Show();
	}
}
