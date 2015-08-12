using System;
using System.Collections.Generic;

public class MVGUISettingsDialogCubeGun
{
	private MVWorldObjectClient wo;

	private MVGUIMaterialSelectionWindow materialSelection;

	private byte currentMaterial;

	public MVGUISettingsDialogCubeGun()
	{
		MVMaterialRepository.AllowDestructibleMaterialSelection = true;
		currentMaterial = MVGameController.EditorController.EditorStateMachine.CubeModelingStateMachine.CurrentMaterialId;
		wo = MVGameController.EditorController.GetSettingsDialogSelectionWO();
		materialSelection = MVGameController.EditorController.ShowMaterialChangeWindow();
		MVGUIMaterialSelectionWindow mVGUIMaterialSelectionWindow = materialSelection;
		mVGUIMaterialSelectionWindow.OnMaterialSelection = (MVGUIMaterialSelectionWindow.OnMaterialSelectionDelegate)Delegate.Combine(mVGUIMaterialSelectionWindow.OnMaterialSelection, new MVGUIMaterialSelectionWindow.OnMaterialSelectionDelegate(SetCubeGunMaterial));
		UXView view = materialSelection.View;
		view.OnHide = (UXView.OnHideDelegate)Delegate.Combine(view.OnHide, new UXView.OnHideDelegate(OnHide));
	}

	private void SetCubeGunMaterial(byte materialId)
	{
		MVGameController.EditorController.EditorStateMachine.CubeModelingStateMachine.CurrentMaterialId = currentMaterial;
		Dictionary<object, object> data = wo.Data;
		((Dictionary<object, object>)data["itemData"])["material"] = materialId;
		MVGameController.Game.UpdateWorldObjectDataPartial(wo.Id, data);
		MVGUIMaterialSelectionWindow mVGUIMaterialSelectionWindow = materialSelection;
		mVGUIMaterialSelectionWindow.OnMaterialSelection = (MVGUIMaterialSelectionWindow.OnMaterialSelectionDelegate)Delegate.Remove(mVGUIMaterialSelectionWindow.OnMaterialSelection, new MVGUIMaterialSelectionWindow.OnMaterialSelectionDelegate(SetCubeGunMaterial));
		UXView view = materialSelection.View;
		view.OnHide = (UXView.OnHideDelegate)Delegate.Remove(view.OnHide, new UXView.OnHideDelegate(OnHide));
	}

	private void OnHide()
	{
		MVMaterialRepository.AllowDestructibleMaterialSelection = false;
		MVGUIMaterialSelectionWindow mVGUIMaterialSelectionWindow = materialSelection;
		mVGUIMaterialSelectionWindow.OnMaterialSelection = (MVGUIMaterialSelectionWindow.OnMaterialSelectionDelegate)Delegate.Remove(mVGUIMaterialSelectionWindow.OnMaterialSelection, new MVGUIMaterialSelectionWindow.OnMaterialSelectionDelegate(SetCubeGunMaterial));
		UXView view = materialSelection.View;
		view.OnHide = (UXView.OnHideDelegate)Delegate.Remove(view.OnHide, new UXView.OnHideDelegate(OnHide));
	}
}
