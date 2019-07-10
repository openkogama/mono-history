using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TerrainCubeModelingControllerTutorial : MonoBehaviour
{
	private MaterialsController materialsController;

	[SerializeField]
	private RawImage materialsButtonImage;

	[SerializeField]
	private DesktopCubeModelingToolsController desktopCubeModelingController;

	public void Initialize(CubeModelingStateMachine cubeModelingStateMachine, MaterialsController materialsController)
	{
		this.materialsController = materialsController;
		desktopCubeModelingController.Initialize(cubeModelingStateMachine);
		materialsController.materialChange = (UnityAction<byte>)Delegate.Combine(materialsController.materialChange, new UnityAction<byte>(SetMaterial));
		SetMaterial(cubeModelingStateMachine.CurrentMaterialId);
		MVGameControllerBase.MainCameraManager.IsLogicRendered = false;
	}

	private void SetMaterial(byte materialId)
	{
		materialsButtonImage.texture = MVGameControllerBase.Game.MaterialRepository.GetMaterial(materialId).ButtonTexture;
		MVGameControllerBase.GameEventManager.AvatarCommandsBuildMode.LaserCommands.SetCurrentCubeMaterial(materialId);
	}

	private void OnDestroy()
	{
		MaterialsController materialsController = this.materialsController;
		materialsController.materialChange = (UnityAction<byte>)Delegate.Remove(materialsController.materialChange, new UnityAction<byte>(SetMaterial));
		MVGameControllerBase.MainCameraManager.IsLogicRendered = true;
	}
}
