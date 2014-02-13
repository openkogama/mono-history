public class MVGUIEditorTools : UXViewScript
{
	public UXGroup editingTools;

	public MVGUICurrentSelectedMaterialCube currentSelectedMaterialCube;

	public UXIconButton newModelButton;

	public UXIconButton aggregateInventoryButton;

	public override void OnShow()
	{
		editingTools.Show();
	}

	public override void OnHide()
	{
		editingTools.Hide();
	}
}
