using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AvatarAccessoryPreviewer : MonoBehaviour, IDragHandler, IPointerDownHandler, IEventSystemHandler, IPointerClickHandler
{
	private const string mouseX = "Mouse X";

	[SerializeField]
	private AvatarPreviewer previewer;

	[SerializeField]
	private RawImage toImage;

	[SerializeField]
	private int previewDimensionsX = 512;

	[SerializeField]
	private int previewDimensionsY = 1024;

	[SerializeField]
	private float rotationSensitivity = 15f;

	[SerializeField]
	private GameObject dropShadowPlane;

	private float currentRotationSpeed;

	private Transform avatarResetToTransform;

	private AvatarPreviewer toPreviewer;

	private bool imagesReady;

	private int currentAnimation;

	private List<string> animations = new List<string> { "Idle", "Walk", "Jump", "Swim", "Dead" };

	private MVBody avatarBody;

	private GameObject bodyClone;

	private Animation goAnimation;

	public void SetupPreviewer(MVBody avatarBody)
	{
		this.avatarBody = avatarBody;
		Quaternion rotation = Quaternion.identity * Quaternion.Euler(0f, 180f, 0f);
		if (bodyClone != null)
		{
			rotation = bodyClone.transform.rotation;
		}
		bodyClone = avatarBody.CreateClone();
		if (avatarResetToTransform != null)
		{
			Object.Destroy(avatarResetToTransform.gameObject);
		}
		if (toPreviewer != null)
		{
			Object.Destroy(toPreviewer.gameObject);
		}
		avatarResetToTransform = new GameObject().transform;
		toImage.color = new Color(1f, 1f, 1f, 1f);
		avatarBody.AccessoryMoveOverride = true;
		MonoBehaviour[] componentsInChildren = bodyClone.GetComponentsInChildren<MonoBehaviour>();
		MonoBehaviour[] array = componentsInChildren;
		foreach (MonoBehaviour monoBehaviour in array)
		{
			monoBehaviour.enabled = false;
		}
		SelectionHelperAvatarAccessory[] componentsInChildren2 = bodyClone.GetComponentsInChildren<SelectionHelperAvatarAccessory>(includeInactive: true);
		for (int j = 0; j < componentsInChildren2.Length; j++)
		{
			componentsInChildren2[j].GetComponent<Collider>().enabled = true;
		}
		SelectionBox[] componentsInChildren3 = bodyClone.GetComponentsInChildren<SelectionBox>();
		for (int k = 0; k < componentsInChildren3.Length; k++)
		{
			Object.Destroy(componentsInChildren3[k].gameObject);
		}
		InvulnerabilityBubble componentInChildren = bodyClone.GetComponentInChildren<InvulnerabilityBubble>();
		if (componentInChildren != null)
		{
			componentInChildren.gameObject.SetActive(value: false);
		}
		SkinnedMeshOptimizer[] componentsInChildren4 = bodyClone.GetComponentsInChildren<SkinnedMeshOptimizer>();
		for (int l = 0; l < componentsInChildren4.Length; l++)
		{
			if (componentsInChildren4[l] != null)
			{
				componentsInChildren4[l].TurnOffMesh();
			}
		}
		MeshRenderer[] componentsInChildren5 = bodyClone.GetComponentsInChildren<MeshRenderer>();
		for (int m = 0; m < componentsInChildren5.Length; m++)
		{
			for (int n = 0; n < componentsInChildren5[m].materials.Length; n++)
			{
				Color color = componentsInChildren5[m].materials[n].color;
				color.a = 1f;
				componentsInChildren5[m].materials[n].color = color;
			}
		}
		goAnimation = bodyClone.GetComponentInChildren<Animation>();
		foreach (AnimationState item in goAnimation)
		{
			item.wrapMode = WrapMode.Loop;
		}
		goAnimation.Play(animations[currentAnimation]);
		toPreviewer = Object.Instantiate(previewer);
		toPreviewer.Initialize(previewDimensionsX, previewDimensionsY, CameraClearFlags.Color, MVGameControllerBase.WOCM.AvatarLocal.PreviewLayerMask, new Vector3(0f, -0.5f, -1f), avatarResetToTransform, new Vector3(100f, 100f, 100f), "Avatar accessory preview", MVGameControllerBase.WOCM.AvatarLocal, bodyClone, new Vector3(-2.4f, -16.3f, 0f));
		bodyClone.transform.rotation = rotation;
		bodyClone.SetLayerRecursively(LayerUtil.GetLayerNumber(LayerFlags.Hidden));
		toImage.texture = toPreviewer.PreviewTexture;
		GameObject gameObject = Object.Instantiate(dropShadowPlane);
		gameObject.transform.SetParent(avatarResetToTransform);
		gameObject.transform.position = toPreviewer.PreviewGameObject.transform.position + new Vector3(0f, -0.03f, 0f);
		imagesReady = true;
	}

	private bool PickAccessory(Ray ray, out GameObject gameObject, out RaycastHit raycastHit)
	{
		if (Physics.Raycast(ray, out raycastHit, float.PositiveInfinity, 1 << LayerMask.NameToLayer("Hidden")))
		{
			gameObject = raycastHit.collider.gameObject;
			if (gameObject.GetComponent<SelectionHelperAvatarAccessory>() != null)
			{
				return true;
			}
		}
		gameObject = null;
		return false;
	}

	private void Start()
	{
		if (MVGameControllerBase.GameMode == MVGameMode.CharacterEditor)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IGetCurrentBody x, BaseEventData y) =>
			{
				x.GetCurrentBody(SetupPreviewer);
			});
		}
		else
		{
			SetupPreviewer(MVGameControllerBase.WOCM.AvatarLocal.Body);
		}
	}

	public void OnDrag(PointerEventData data)
	{
		currentRotationSpeed = (0f - Input.GetAxis("Mouse X")) * rotationSensitivity;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		currentRotationSpeed = 0f;
	}

	private void Update()
	{
		if (imagesReady)
		{
			MVInputWrapper.SuppressInGameInput();
			MVInputWrapper.SuppressAllInput();
			toPreviewer.UpdateRotation(currentRotationSpeed);
			currentRotationSpeed = 0f;
		}
	}

	private void OnDestroy()
	{
		avatarBody.DestroyClone();
		if (toPreviewer != null)
		{
			Object.Destroy(toPreviewer.gameObject);
		}
		if (avatarResetToTransform != null)
		{
			Object.Destroy(avatarResetToTransform.gameObject);
			avatarResetToTransform = null;
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		Vector2 screenPoint = Input.mousePosition;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(toImage.rectTransform, screenPoint, null, out var localPoint);
		localPoint += new Vector2(toImage.rectTransform.rect.width / (1f / toImage.rectTransform.pivot.x), toImage.rectTransform.rect.height / (1f / toImage.rectTransform.pivot.y));
		Ray ray = toPreviewer.previewCam.ScreenPointToRay(localPoint);
		if (PickAccessory(ray, out var gameObject, out var _))
		{
			SelectionHelperAvatarAccessory componentInChildren = gameObject.GetComponentInChildren<SelectionHelperAvatarAccessory>(includeInactive: true);
			Debug.Log(componentInChildren.StreamingAssetsId);
			AccessoryDataClient accessoryData = AccessoryDataManager.GetAccessoryDataByStreamingAssetId(componentInChildren.StreamingAssetsId);
			ExecuteEvents.ExecuteHierarchy(base.gameObject, null, (IAccessoryClicked x, BaseEventData y) =>
			{
				x.OpenAccessoryManagementScreen(accessoryData);
			});
		}
	}

	public void ChangeAnimation()
	{
		currentAnimation++;
		if (currentAnimation >= animations.Count)
		{
			currentAnimation = 0;
		}
		goAnimation.Play(animations[currentAnimation]);
	}
}
