using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SpawnRoleAvatarSelectionElement : MonoBehaviour
{
	[SerializeField]
	private RawImage avatarImage;

	[SerializeField]
	private Image noAvatarImage;

	[SerializeField]
	private SpawnRolePreviewer spawnRolePreviewerPrefab;

	[SerializeField]
	private int previewWidth;

	[SerializeField]
	private int previewHeight;

	private int elementIndex;

	private int avatarId;

	private SpawnRolePreviewer spawnRolePreviewer;

	private UnityAction<int> onSelectedCallback;

	public void Initialize(int elementIndex, int avatarId, UnityAction<int> onSelectedCallback)
	{
		this.elementIndex = elementIndex;
		this.avatarId = avatarId;
		this.onSelectedCallback = onSelectedCallback;
	}

	public void OnSelected()
	{
		onSelectedCallback(avatarId);
	}

	public void SetupPreviewImage(GameObject spawnRoleObject)
	{
		spawnRolePreviewer = Object.Instantiate(spawnRolePreviewerPrefab);
		GameObject gameObject = Object.Instantiate(spawnRoleObject);
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localPosition = Vector3.zero;
		Transform previewSpawnRoleRoot = new GameObject("Preview Root - TierShopItem").transform;
		Vector3 previewPosition = new Vector3(1500f, 1500f, 30f);
		Vector3 cameraOffset = new Vector3(0f, 1f, -4.5f);
		spawnRolePreviewer.Initialize(previewWidth, previewHeight, CameraClearFlags.Color, LayerFlags.Default | LayerFlags.CamRotateTarget, cameraOffset, previewSpawnRoleRoot, previewPosition, "SpawnRole", elementIndex, gameObject);
		noAvatarImage.gameObject.SetActive(value: false);
		avatarImage.gameObject.SetActive(value: true);
		avatarImage.texture = spawnRolePreviewer.PreviewTexture;
	}

	public void Activate()
	{
		spawnRolePreviewer.ActivatePreview();
	}

	public void Deactivate()
	{
		spawnRolePreviewer.DeactivatePreview();
	}

	private void OnDestroy()
	{
		Object.Destroy(spawnRolePreviewer);
	}
}
