using System;
using System.Collections;

public class MVGUISettingsDialogCubeGun
{
	private MVWorldObjectClient wo;

	private MVGUIMaterialSelectionWindow materialSelection;

	private byte currentMaterial;

	public MVGUISettingsDialogCubeGun()
	{
		currentMaterial = MVGameController.Instance.EditorController.EditorStateMachine.CubeModelingStateMachine.CurrentMaterialId;
		wo = MVGameController.Instance.EditorController.GetSettingsDialogSelectionWO();
		materialSelection = MVGameController.Instance.EditorController.ShowMaterialChangeWindow();
		MVGUIMaterialSelectionWindow mVGUIMaterialSelectionWindow = materialSelection;
		mVGUIMaterialSelectionWindow.OnMaterialSelection = (MVGUIMaterialSelectionWindow.OnMaterialSelectionDelegate)Delegate.Combine(mVGUIMaterialSelectionWindow.OnMaterialSelection, new MVGUIMaterialSelectionWindow.OnMaterialSelectionDelegate(SetCubeGunMaterial));
		UXView view = materialSelection.View;
		view.OnHide = (UXView.OnHideDelegate)Delegate.Combine(view.OnHide, new UXView.OnHideDelegate(OnHide));
	}

	private void SetCubeGunMaterial(byte materialId)
	{
		MVGameController.Instance.EditorController.EditorStateMachine.CubeModelingStateMachine.CurrentMaterialId = currentMaterial;
		Hashtable data = wo.Data;
		((Hashtable)data["itemData"])["material"] = materialId;
		MVGameController.Instance.Game.UpdateWorldObjectDataPartial(wo.Id, data);
		MVGUIMaterialSelectionWindow mVGUIMaterialSelectionWindow = materialSelection;
		mVGUIMaterialSelectionWindow.OnMaterialSelection = (MVGUIMaterialSelectionWindow.OnMaterialSelectionDelegate)Delegate.Remove(mVGUIMaterialSelectionWindow.OnMaterialSelection, new MVGUIMaterialSelectionWindow.OnMaterialSelectionDelegate(SetCubeGunMaterial));
		UXView view = materialSelection.View;
		view.OnHide = (UXView.OnHideDelegate)Delegate.Remove(view.OnHide, new UXView.OnHideDelegate(OnHide));
	}

	private void OnHide()
	{
		MVGUIMaterialSelectionWindow mVGUIMaterialSelectionWindow = materialSelection;
		mVGUIMaterialSelectionWindow.OnMaterialSelection = (MVGUIMaterialSelectionWindow.OnMaterialSelectionDelegate)Delegate.Remove(mVGUIMaterialSelectionWindow.OnMaterialSelection, new MVGUIMaterialSelectionWindow.OnMaterialSelectionDelegate(SetCubeGunMaterial));
		UXView view = materialSelection.View;
		view.OnHide = (UXView.OnHideDelegate)Delegate.Remove(view.OnHide, new UXView.OnHideDelegate(OnHide));
	}
}
