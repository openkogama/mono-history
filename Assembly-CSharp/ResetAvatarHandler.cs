using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResetAvatarHandler : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
{
	[SerializeField]
	private AvatarScreenShooter screenShooter;

	[SerializeField]
	private PleaseWaitPopup pleaseWaitPopupPrefab;

	[SerializeField]
	private GameObject invisibleBlocker;

	[SerializeField]
	private AvatarPreviewer previewer;

	[SerializeField]
	private RawImage fromImage;

	[SerializeField]
	private RawImage toImage;

	[SerializeField]
	private int previewDimensions;

	[SerializeField]
	private float defaultRotationSpeed = 15f;

	[SerializeField]
	private float rotationSensitivity = 15f;

	private float currentRotationSpeed;

	private Action OnReset;

	private MVBody avatarBody;

	private Transform avatarResetToTransform;

	private AvatarPreviewer fromPreviewer;

	private AvatarPreviewer toPreviewer;

	private const string mouseX = "Mouse X";

	private bool imagesReady;

	private bool isDown;

	public void ResetAvatar(MVBody currentBody, Action onReset)
	{
		OnReset = onReset;
		avatarBody = currentBody;
		if (avatarResetToTransform == null)
		{
			avatarResetToTransform = new GameObject().transform;
		}
		GetResetAvatarData(currentBody.Id);
	}

	private void GetResetAvatarData(int id)
	{
		MVGameControllerBase.Game.ReceivedItemFromQuery += GameOnReceivedItemFromQuery;
		MVGameControllerBase.Game.OperationRequestSender.GetResetAvatar(id);
	}

	private void GameOnReceivedItemFromQuery(object sender, ReceivedItemFromQueryEventArgs receivedItemFromQueryEventArgs)
	{
		MVGameControllerBase.Game.ReceivedItemFromQuery -= GameOnReceivedItemFromQuery;
		KoGaMaPackageClient koGaMaPackageClient = new KoGaMaPackageClient(receivedItemFromQueryEventArgs.KoGaMaData, readRuntimeValues: false);
		MVWorldObjectClient mVWorldObjectClient = koGaMaPackageClient.worldObjects[koGaMaPackageClient.worldObjectRoot];
		mVWorldObjectClient.InventoryInitialize();
		toImage.color = new Color(1f, 1f, 1f, 1f);
		fromImage.color = new Color(1f, 1f, 1f, 1f);
		toPreviewer = UnityEngine.Object.Instantiate(previewer);
		toPreviewer.Initialize(previewDimensions, previewDimensions, CameraClearFlags.Color, mVWorldObjectClient.PreviewLayerMask, default, avatarResetToTransform, new Vector3(100f, 100f, 100f), "Avatar reset-to", mVWorldObjectClient, mVWorldObjectClient.GameObject, default);
		toPreviewer.PreviewGameObject.transform.Rotate(0f, 180f, 0f);
		GameObject woGameObjectCopy = UnityEngine.Object.Instantiate(avatarBody.GameObject);
		woGameObjectCopy.SetLayerRecursively(LayerUtil.GetLayerNumber(LayerFlags.Hidden));
		fromPreviewer = UnityEngine.Object.Instantiate(previewer);
		fromPreviewer.Initialize(previewDimensions, previewDimensions, CameraClearFlags.Color, avatarBody.PreviewLayerMask, new Vector3(0f, 0f, 0f), avatarResetToTransform, new Vector3(100f, 100f, 100f), "Avatar reset-to", avatarBody, woGameObjectCopy, default);
		fromPreviewer.PreviewGameObject.transform.Rotate(0f, 180f, 0f);
		fromImage.texture = fromPreviewer.PreviewTexture;
		toImage.texture = toPreviewer.PreviewTexture;
		imagesReady = true;
	}

	public void OnDrag(PointerEventData data)
	{
		currentRotationSpeed = (0f - Input.GetAxis("Mouse X")) * rotationSensitivity;
		isDown = true;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		currentRotationSpeed = 0f;
		isDown = true;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		isDown = false;
	}

	private void Update()
	{
		if (imagesReady)
		{
			MVInputWrapper.SuppressInGameInput();
			MVInputWrapper.SuppressAllInput();
			fromPreviewer.UpdateRotation(currentRotationSpeed);
			toPreviewer.UpdateRotation(currentRotationSpeed);
			currentRotationSpeed = ((!isDown) ? defaultRotationSpeed : 0f);
		}
	}

	private void OnDestroy()
	{
		MVGameControllerBase.Game.ReceivedItemFromQuery -= GameOnReceivedItemFromQuery;
		if (avatarResetToTransform != null)
		{
			UnityEngine.Object.Destroy(avatarResetToTransform.gameObject);
			avatarResetToTransform = null;
		}
	}

	public void OnAcceptReset()
	{
		PleaseWaitPopup popup = UnityEngine.Object.Instantiate(pleaseWaitPopupPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(popup.gameObject, UIPushOption.Blocking, null, UIGroupFlags.Popup);
		});
		OnReset();
	}
}
