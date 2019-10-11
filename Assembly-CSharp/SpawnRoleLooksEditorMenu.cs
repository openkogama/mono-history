using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpawnRoleLooksEditorMenu : MonoBehaviour
{
	[SerializeField]
	private RawImage spawnRoleAvatarPreviewImage;

	[SerializeField]
	private SpawnRoleAvatarSelectionMenu avatarSelectionMenuPrefab;

	[SerializeField]
	private SpawnRolePreviewer spawnRolePreviewerPrefab;

	[SerializeField]
	private int previewWidth;

	[SerializeField]
	private int previewHeight;

	private int spawnRoleId;

	private SpawnRolePreviewer spawnRolePreviewer;

	private MVAvatarSpawnRoleCreator spawnRole;

	private int renewPreviewerFrameDelay = -1;

	public void Initialize(int spawnRoleId, MVAvatarSpawnRoleCreator spawnRole)
	{
		this.spawnRoleId = spawnRoleId;
		this.spawnRole = spawnRole;
		SetupPreviewImage(spawnRole.GetSpawnRolePreviewObject());
		spawnRole.OnBodyUpdate = (Action)Delegate.Combine(spawnRole.OnBodyUpdate, new Action(OnSpawnRoleBodyUpdate));
	}

	public void OnAvatarChangeButtonPressed()
	{
		SpawnRoleAvatarSelectionMenu avatarSelectionMenu = UnityEngine.Object.Instantiate(avatarSelectionMenuPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(avatarSelectionMenu.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
		avatarSelectionMenu.Initialize(spawnRoleId);
	}

	private void Update()
	{
		if (renewPreviewerFrameDelay >= 0)
		{
			renewPreviewerFrameDelay--;
			if (renewPreviewerFrameDelay == -1)
			{
				SetupPreviewImage(spawnRole.GetSpawnRolePreviewObject());
			}
		}
	}

	private void SetupPreviewImage(GameObject spawnRolePreviewObject)
	{
		if (spawnRolePreviewer != null)
		{
			UnityEngine.Object.Destroy(spawnRolePreviewer);
		}
		SharedCubeFunctions.SetLayerRecursively(spawnRolePreviewObject.transform, select: false);
		spawnRolePreviewer = UnityEngine.Object.Instantiate(spawnRolePreviewerPrefab);
		GameObject gameObject = UnityEngine.Object.Instantiate(spawnRolePreviewObject);
		gameObject.transform.localRotation = Quaternion.identity;
		Transform previewSpawnRoleRoot = new GameObject("Preview Root - TierShopItem").transform;
		Vector3 previewPosition = new Vector3(-300f, -500f, -700f);
		Vector3 cameraOffset = new Vector3(0f, 1f, -4.5f);
		spawnRolePreviewer.Initialize(previewWidth, previewHeight, CameraClearFlags.Color, LayerFlags.Default | LayerFlags.CamRotateTarget, cameraOffset, previewSpawnRoleRoot, previewPosition, "SpawnRole", 0, gameObject);
		spawnRoleAvatarPreviewImage.texture = spawnRolePreviewer.PreviewTexture;
		SharedCubeFunctions.SetLayerRecursively(spawnRolePreviewObject.transform, select: true);
	}

	private void OnSpawnRoleBodyUpdate()
	{
		renewPreviewerFrameDelay = 3;
	}

	private void OnDestroy()
	{
		UnityEngine.Object.Destroy(spawnRolePreviewer);
		MVAvatarSpawnRoleCreator mVAvatarSpawnRoleCreator = spawnRole;
		mVAvatarSpawnRoleCreator.OnBodyUpdate = (Action)Delegate.Remove(mVAvatarSpawnRoleCreator.OnBodyUpdate, new Action(OnSpawnRoleBodyUpdate));
	}
}
