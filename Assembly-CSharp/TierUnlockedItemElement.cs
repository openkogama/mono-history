using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.UI;

public class TierUnlockedItemElement : MonoBehaviour
{
	[SerializeField]
	private InventoryItemPreviewer objectPreviewerPrefab;

	[SerializeField]
	private RawImage previewImage;

	[SerializeField]
	private int previewWidth;

	[SerializeField]
	private int previewHeight;

	[SerializeField]
	private Text itemAmountText;

	private InventoryItemPreviewer objectPreviewer;

	private Transform rootTransform;

	private MVWorldObjectClient previewObject;

	private Vector3 cameraOffset = new Vector3(0f, 0f, 0f);

	public void Initialize(List<MVWorldObjectClient> tierShopItemData, int itemIndex)
	{
		objectPreviewer = Object.Instantiate(objectPreviewerPrefab);
		previewObject = tierShopItemData[0];
		if (InventoryItem.localItemDescriptionOverride.ContainsKey(previewObject.DocumentationType))
		{
			cameraOffset = InventoryItem.localItemDescriptionOverride[previewObject.DocumentationType].CameraPreviewerOffset;
		}
		GameObject gameObject = CreatePreviewObjectClone();
		gameObject.transform.localRotation = Quaternion.identity;
		rootTransform = new GameObject("Preview Root - TierShopItem").transform;
		Vector3 previewPosition = new Vector3(300f, 300f, 10f * (float)itemIndex);
		objectPreviewer.Initialize(previewWidth, previewHeight, CameraClearFlags.Color, previewObject.PreviewLayerMask, cameraOffset, rootTransform, previewPosition, previewObject.WorldObjectType.ToString(), previewObject, gameObject);
		previewImage.texture = objectPreviewer.PreviewTexture;
		itemAmountText.text = "x" + tierShopItemData.Count;
	}

	private GameObject CreatePreviewObjectClone()
	{
		previewObject.SetupTierInventory();
		bool activeSelf = previewObject.GameObject.activeSelf;
		previewObject.GameObject.SetActive(value: false);
		bool flag = false;
		GreyOutObjectScript[] componentsInChildren = previewObject.GameObject.GetComponentsInChildren<GreyOutObjectScript>();
		GreyOutObjectScript[] array = componentsInChildren;
		foreach (GreyOutObjectScript greyOutObjectScript in array)
		{
			if (!greyOutObjectScript.IsGreyedIn)
			{
				greyOutObjectScript.GreyIn();
				flag = true;
			}
		}
		GameObject gameObject = Object.Instantiate(previewObject.GameObject);
		previewObject.GameObject.SetActive(activeSelf);
		JetPackVisualization jetPackVisualization = null;
		MonoBehaviour[] componentsInChildren2 = gameObject.GetComponentsInChildren<MonoBehaviour>();
		MonoBehaviour[] array2 = componentsInChildren2;
		foreach (MonoBehaviour monoBehaviour in array2)
		{
			monoBehaviour.enabled = false;
			if (monoBehaviour is TintObject)
			{
				((TintObject)monoBehaviour).TeamTint(MVTeam.None);
			}
			if (monoBehaviour is MVPickupOwner)
			{
				((MVPickupOwner)monoBehaviour).findWorldObjectParent = false;
			}
			if (monoBehaviour is JetPackVisualization)
			{
				jetPackVisualization = (JetPackVisualization)monoBehaviour;
			}
		}
		ParticleSystem[] componentsInChildren3 = gameObject.GetComponentsInChildren<ParticleSystem>();
		ParticleSystem[] array3 = componentsInChildren3;
		foreach (ParticleSystem particleSystem in array3)
		{
			particleSystem.Stop();
			particleSystem.gameObject.SetActive(value: false);
		}
		UseInteratorVisualization[] componentsInChildren4 = gameObject.GetComponentsInChildren<UseInteratorVisualization>();
		UseInteratorVisualization[] array4 = componentsInChildren4;
		foreach (UseInteratorVisualization useInteratorVisualization in array4)
		{
			useInteratorVisualization.Disable();
		}
		if (jetPackVisualization == null)
		{
			RotateLocal rotateLocal = gameObject.AddComponent<RotateLocal>();
			rotateLocal.rotationSpeed = 70f;
		}
		else
		{
			RotateLocal rotateLocal2 = jetPackVisualization.gameObject.AddComponent<RotateLocal>();
			rotateLocal2.rotationSpeed = 70f;
			jetPackVisualization.transform.localPosition = Vector3.zero;
			cameraOffset.x += 0.075f;
			cameraOffset.z -= 0.5f;
		}
		gameObject.SetActive(value: true);
		if (flag)
		{
			GreyOutObjectScript[] array5 = componentsInChildren;
			foreach (GreyOutObjectScript greyOutObjectScript2 in array5)
			{
				greyOutObjectScript2.GreyOut();
			}
		}
		return gameObject;
	}

	private void OnDestroy()
	{
		if (rootTransform != null)
		{
			Object.Destroy(rootTransform.gameObject);
		}
		rootTransform = null;
		previewObject.UnSetupTierInventory();
	}
}
