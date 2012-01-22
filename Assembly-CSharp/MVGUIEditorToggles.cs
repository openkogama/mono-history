using UnityEngine;

public class MVGUIEditorToggles : UXViewScript
{
	public UXToggleIconButton workplaneToggle;

	public UXToggleIconButton cloakeToggle;

	public UXToggleIconButton camToggle;

	public UXToggleIconButton jetPackToggle;

	public UXToggleIconButton logicRenderingToggle;

	public UXToggleIconButton gridSnapToggle;

	public override void OnShow()
	{
		((Component)workplaneToggle).gameObject.SetActiveRecursively(true);
		((Component)cloakeToggle).gameObject.SetActiveRecursively(true);
		((Component)camToggle).gameObject.SetActiveRecursively(true);
		((Component)jetPackToggle).gameObject.SetActiveRecursively(true);
		((Component)logicRenderingToggle).gameObject.SetActiveRecursively(true);
		((Component)gridSnapToggle).gameObject.SetActiveRecursively(true);
	}

	public override void OnHide()
	{
		((Component)workplaneToggle).gameObject.SetActiveRecursively(false);
		((Component)cloakeToggle).gameObject.SetActiveRecursively(false);
		((Component)camToggle).gameObject.SetActiveRecursively(false);
		((Component)jetPackToggle).gameObject.SetActiveRecursively(false);
		((Component)logicRenderingToggle).gameObject.SetActiveRecursively(false);
		((Component)gridSnapToggle).gameObject.SetActiveRecursively(false);
	}
}
