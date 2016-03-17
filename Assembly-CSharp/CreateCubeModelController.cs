using System;
using UnityEngine;
using UnityEngine.Events;

public class CreateCubeModelController : MonoBehaviour
{
	[SerializeField]
	private CreateNewCubeModel cubeModelButton;

	public void Initialize(MaterialsController materialsController, Transform parent, byte materialId)
	{
		cubeModelButton = UnityEngine.Object.Instantiate(cubeModelButton);
		cubeModelButton.transform.SetParent(parent, worldPositionStays: false);
		cubeModelButton.UpdateButtonTextures(materialId);
		materialsController.materialChange = (UnityAction<byte>)Delegate.Combine(materialsController.materialChange, new UnityAction<byte>(cubeModelButton.UpdateButtonTextures));
	}
}
