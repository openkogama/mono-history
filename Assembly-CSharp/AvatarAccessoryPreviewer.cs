using System.Collections;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AvatarAccessoryPreviewer : MonoBehaviour, IDragHandler, IPointerDownHandler, IEventSystemHandler, IPointerClickHandler
{
	private const string mouseX = "Mouse X";

	private const string mouseY = "Mouse Y";

	private const float animationLoopTimes = 3f;

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
	private float zoomSpeed = 1.5f;

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

	private AccessoryAnimationHandler[] accessoryAnimationHandlers;

	private bool pickedAccessory;

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
		PickupItem[] componentsInChildren2 = bodyClone.GetComponentsInChildren<PickupItem>();
		for (int j = 0; j < componentsInChildren2.Length; j++)
		{
			componentsInChildren2[j].gameObject.SetActive(value: false);
		}
		AvatarModifier[] componentsInChildren3 = bodyClone.GetComponentsInChildren<AvatarModifier>();
		for (int k = 0; k < componentsInChildren3.Length; k++)
		{
			componentsInChildren3[k].gameObject.SetActive(value: false);
		}
		SelectionHelperAvatarAccessory[] componentsInChildren4 = bodyClone.GetComponentsInChildren<SelectionHelperAvatarAccessory>(includeInactive: true);
		for (int l = 0; l < componentsInChildren4.Length; l++)
		{
			componentsInChildren4[l].GetComponent<Collider>().enabled = true;
		}
		SelectionBox[] componentsInChildren5 = bodyClone.GetComponentsInChildren<SelectionBox>();
		for (int m = 0; m < componentsInChildren5.Length; m++)
		{
			Object.Destroy(componentsInChildren5[m].gameObject);
		}
		InvulnerabilityBubble componentInChildren = bodyClone.GetComponentInChildren<InvulnerabilityBubble>();
		if (componentInChildren != null)
		{
			componentInChildren.gameObject.SetActive(value: false);
		}
		SkinnedMeshOptimizer[] componentsInChildren6 = bodyClone.GetComponentsInChildren<SkinnedMeshOptimizer>();
		for (int n = 0; n < componentsInChildren6.Length; n++)
		{
			if (componentsInChildren6[n] != null)
			{
				componentsInChildren6[n].DisableOptimizer();
				componentsInChildren6[n].TurnOffMesh();
			}
		}
		MeshRenderer[] componentsInChildren7 = bodyClone.GetComponentsInChildren<MeshRenderer>();
		for (int num = 0; num < componentsInChildren7.Length; num++)
		{
			for (int num2 = 0; num2 < componentsInChildren7[num].materials.Length; num2++)
			{
				Color color = componentsInChildren7[num].materials[num2].color;
				color.a = 1f;
				componentsInChildren7[num].materials[num2].color = color;
			}
		}
		goAnimation = bodyClone.GetComponentInChildren<Animation>();
		goAnimation.Play(animations[currentAnimation]);
		accessoryAnimationHandlers = goAnimation.GetComponentsInChildren<AccessoryAnimationHandler>();
		for (int num3 = 0; num3 < accessoryAnimationHandlers.Length; num3++)
		{
			accessoryAnimationHandlers[num3].PlayAnimation(animations[currentAnimation]);
		}
		toPreviewer = Object.Instantiate(previewer);
		toPreviewer.Initialize(previewDimensionsX, previewDimensionsY, CameraClearFlags.Color, MVGameControllerBase.WOCM.AvatarLocal.PreviewLayerMask, new Vector3(0f, -0.5f, -1f), avatarResetToTransform, new Vector3(100f, 100f, 100f), "Avatar accessory preview", MVGameControllerBase.WOCM.AvatarLocal, bodyClone, new Vector3(15f, 0f, 0f));
		toPreviewer.previewCam.transform.position += new Vector3(0f, 1.22f, 0f);
		bodyClone.transform.rotation = rotation;
		bodyClone.SetLayerRecursively(LayerUtil.GetLayerNumber(LayerFlags.Hidden));
		toImage.texture = toPreviewer.PreviewTexture;
		GameObject gameObject = Object.Instantiate(dropShadowPlane);
		gameObject.transform.SetParent(avatarResetToTransform);
		gameObject.transform.position = toPreviewer.PreviewGameObject.transform.position + new Vector3(0f, -0.1f, 0f);
		imagesReady = true;
	}

	private bool PickAccessory(Ray ray, out GameObject gameObject, out RaycastHit raycastHit)
	{
		Debug.DrawRay(ray.origin, ray.direction, Color.red, 10f);
		RaycastHit[] array = Physics.RaycastAll(ray, float.PositiveInfinity, 1 << LayerMask.NameToLayer("Hidden"));
		for (int i = 0; i < array.Length; i++)
		{
			gameObject = array[i].collider.gameObject;
			raycastHit = array[i];
			if (gameObject.GetComponent<SelectionHelperAvatarAccessory>() != null)
			{
				return true;
			}
		}
		gameObject = null;
		raycastHit = default;
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

	public void OnDrag(PointerEventData data)
	{
		currentRotationSpeed = (0f - Input.GetAxis("Mouse X")) * rotationSensitivity;
		toPreviewer.previewCam.fieldOfView += Input.GetAxis("Mouse Y") * zoomSpeed * Time.deltaTime;
		toPreviewer.previewCam.fieldOfView = Mathf.Clamp(toPreviewer.previewCam.fieldOfView, 20f, 60f);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		currentRotationSpeed = 0f;
		Vector2 screenPoint = Input.mousePosition;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(toImage.rectTransform, screenPoint, null, out var localPoint);
		localPoint += new Vector2(toImage.rectTransform.rect.width / (1f / toImage.rectTransform.pivot.x), toImage.rectTransform.rect.height / (1f / toImage.rectTransform.pivot.y));
		Ray ray = toPreviewer.previewCam.ScreenPointToRay(localPoint);
		if (PickAccessory(ray, out var _, out var _))
		{
			pickedAccessory = true;
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		Vector2 screenPoint = Input.mousePosition;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(toImage.rectTransform, screenPoint, null, out var localPoint);
		localPoint += new Vector2(toImage.rectTransform.rect.width / (1f / toImage.rectTransform.pivot.x), toImage.rectTransform.rect.height / (1f / toImage.rectTransform.pivot.y));
		Ray ray = toPreviewer.previewCam.ScreenPointToRay(localPoint);
		if (pickedAccessory && PickAccessory(ray, out var gameObject, out var _))
		{
			SelectionHelperAvatarAccessory componentInChildren = gameObject.GetComponentInChildren<SelectionHelperAvatarAccessory>(includeInactive: true);
			AccessoryDataClient accessoryData = AccessoryDataManager.GetAccessoryDataByStreamingAssetId(componentInChildren.StreamingAssetsId);
			ExecuteEvents.ExecuteHierarchy(base.gameObject, null, (IAccessoryClicked x, BaseEventData y) =>
			{
				x.OpenAccessoryManagementScreen(accessoryData);
			});
		}
		pickedAccessory = false;
	}

	public void ChangeAnimation()
	{
		currentAnimation++;
		if (currentAnimation >= animations.Count)
		{
			currentAnimation = 1;
		}
		PlayAnimation();
	}

	public void OnRestartAnimation()
	{
		if (!(goAnimation == null))
		{
			accessoryAnimationHandlers = goAnimation.GetComponentsInChildren<AccessoryAnimationHandler>();
			PlayAnimation("Idle");
		}
	}

	private void PlayAnimation()
	{
		PlayAnimation(animations[currentAnimation]);
	}

	private void PlayAnimation(string animationName)
	{
		goAnimation.Play(animationName);
		for (int i = 0; i < accessoryAnimationHandlers.Length; i++)
		{
			accessoryAnimationHandlers[i].PlayAnimation(animationName);
		}
		StopAllCoroutines();
		AnimationState animationState = goAnimation[animationName];
		float resetDelay = animationState.length / animationState.speed * ((animationState.wrapMode != WrapMode.Loop) ? 1f : 3f);
		StartCoroutine(AnimationEndTrack(resetDelay));
	}

	private IEnumerator AnimationEndTrack(float resetDelay)
	{
		float startTime = Time.time;
		while (Time.time < startTime + resetDelay)
		{
			yield return null;
		}
		PlayAnimation("Idle");
	}
}
