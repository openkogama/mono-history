using MV.Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TierUnlockedPopupContentSpawnRole : TierUnlockedPopupContentBase
{
	[SerializeField]
	private RawImage spawnRolePreviewImage;

	[SerializeField]
	private SpawnRolePreviewer spawnRolePreviewPrefab;

	[SerializeField]
	protected int previewWidth;

	[SerializeField]
	protected int previewHeight;

	private SpawnRolePreviewer spawnRolePreviewer;

	public override void Initialize(GamePassTier unlockedGamePassTier, UnityAction onDisplayDoneCallback)
	{
		base.Initialize(unlockedGamePassTier, onDisplayDoneCallback);
	}

	public void SetupPreviewImage(GameObject spawnRolePreviewObject)
	{
		spawnRolePreviewer = Object.Instantiate(spawnRolePreviewPrefab);
		GameObject gameObject = Object.Instantiate(spawnRolePreviewObject);
		gameObject.transform.localRotation = Quaternion.identity;
		Transform previewSpawnRoleRoot = new GameObject("Preview Root - TierShopItem").transform;
		Vector3 previewPosition = new Vector3(500f, 500f, 0f);
		Vector3 cameraOffset = new Vector3(0f, 1f, -4.5f);
		spawnRolePreviewer.Initialize(previewWidth, previewHeight, CameraClearFlags.Color, LayerFlags.Default | LayerFlags.CamRotateTarget, cameraOffset, previewSpawnRoleRoot, previewPosition, "SpawnRole", 0, gameObject);
		spawnRolePreviewImage.texture = spawnRolePreviewer.PreviewTexture;
	}
}
