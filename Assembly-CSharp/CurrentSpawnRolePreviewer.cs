using UnityEngine;
using UnityEngine.UI;

public class CurrentSpawnRolePreviewer : MonoBehaviour
{
	[SerializeField]
	private RawImage previewImage;

	[SerializeField]
	private AvatarPreviewer previewerPrefab;

	[SerializeField]
	private GameObject dropShadowPlane;

	private Transform avatarResetToTransform;

	private MVBody avatarBody;

	private GameObject bodyClone;

	private Animation goAnimation;

	private AvatarPreviewer previewer;

	public void SetupPreviewer(int previewDimensionsX = 512, int previewDimensionsY = 1024)
	{
		avatarBody = ((MVAvatarLocal)MVGameControllerBase.WOCM.GetWorldObjectClient(MVGameControllerBase.SpawnRoleDataMediatorLocal.WoId)).Body;
		Quaternion rotation = Quaternion.identity * Quaternion.Euler(0f, 180f, 0f);
		if (bodyClone != null)
		{
			rotation = bodyClone.transform.rotation;
		}
		bodyClone = avatarBody.CreateClone();
		avatarBody.AccessoryMoveOverride = false;
		if (previewer != null)
		{
			Object.Destroy(previewer.gameObject);
		}
		if (avatarResetToTransform != null)
		{
			Object.Destroy(avatarResetToTransform.gameObject);
		}
		avatarResetToTransform = new GameObject().transform;
		previewImage.color = new Color(1f, 1f, 1f, 1f);
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
		AnimatedSpriteSheetTexture[] componentsInChildren4 = bodyClone.GetComponentsInChildren<AnimatedSpriteSheetTexture>(includeInactive: true);
		for (int l = 0; l < componentsInChildren4.Length; l++)
		{
			componentsInChildren4[l].enabled = true;
		}
		AnimatedTextureOffset[] componentsInChildren5 = bodyClone.GetComponentsInChildren<AnimatedTextureOffset>(includeInactive: true);
		for (int m = 0; m < componentsInChildren5.Length; m++)
		{
			componentsInChildren5[m].enabled = true;
		}
		SelectionBox[] componentsInChildren6 = bodyClone.GetComponentsInChildren<SelectionBox>();
		for (int n = 0; n < componentsInChildren6.Length; n++)
		{
			Object.Destroy(componentsInChildren6[n].gameObject);
		}
		InvulnerabilityBubble componentInChildren = bodyClone.GetComponentInChildren<InvulnerabilityBubble>();
		if (componentInChildren != null)
		{
			componentInChildren.gameObject.SetActive(value: false);
		}
		RemoveSkinnedMeshOptimizers();
		MeshRenderer[] componentsInChildren7 = bodyClone.GetComponentsInChildren<MeshRenderer>();
		for (int num = 0; num < componentsInChildren7.Length; num++)
		{
			for (int num2 = 0; num2 < componentsInChildren7[num].materials.Length; num2++)
			{
				if (componentsInChildren7[num].materials[num2].HasProperty("_Color"))
				{
					Color color = componentsInChildren7[num].materials[num2].color;
					color.a = 1f;
					componentsInChildren7[num].materials[num2].color = color;
				}
			}
		}
		goAnimation = bodyClone.GetComponentInChildren<Animation>();
		previewer = Object.Instantiate(previewerPrefab);
		previewer.Initialize(previewDimensionsX, previewDimensionsY, CameraClearFlags.Color, MVGameControllerBase.LocalPlayer.Body.PreviewLayerMask, new Vector3(0f, -0.5f, -1f), avatarResetToTransform, new Vector3(100f, 100f, 100f), "CurrentSpawnRole preview", MVGameControllerBase.LocalPlayer.Body, bodyClone, new Vector3(15f, 0f, 0f));
		previewer.previewCam.transform.position += new Vector3(0f, 1.22f, 0f);
		bodyClone.transform.rotation = rotation;
		bodyClone.SetLayerRecursively(LayerUtil.GetLayerNumber(LayerFlags.Hidden));
		previewImage.texture = previewer.PreviewTexture;
		GameObject gameObject = Object.Instantiate(dropShadowPlane);
		gameObject.transform.SetParent(avatarResetToTransform);
		gameObject.transform.position = previewer.PreviewGameObject.transform.position + new Vector3(0f, -0.1f, 0f);
	}

	public void ChangeAnimation(string NewAnimation)
	{
		goAnimation.Play(NewAnimation);
	}

	private void RemoveSkinnedMeshOptimizers()
	{
		SkinnedMeshOptimizer[] componentsInChildren = bodyClone.GetComponentsInChildren<SkinnedMeshOptimizer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i] != null)
			{
				componentsInChildren[i].DisableOptimizer();
				componentsInChildren[i].TurnOffMesh();
				Object.Destroy(componentsInChildren[i]);
			}
		}
	}

	private void OnDestroy()
	{
		if (avatarBody != null)
		{
			avatarBody.DestroyClone();
		}
		if (previewer != null)
		{
			Object.Destroy(previewer.gameObject);
		}
		if (avatarResetToTransform != null)
		{
			Object.Destroy(avatarResetToTransform.gameObject);
			avatarResetToTransform = null;
		}
	}
}
