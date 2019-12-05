using MV.Common;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class DefaultSpawnRoleSelectionElement : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
{
	[SerializeField]
	protected RawImage spawnRoleImage;

	[SerializeField]
	protected SpawnRolePreviewer spawnRolePreviewerPrefab;

	[SerializeField]
	protected int previewWidth;

	[SerializeField]
	protected int previewHeight;

	protected bool isSelected;

	protected int spawnRoleIndex;

	protected int woId;

	protected UnityAction<int> onSelectedCallback;

	protected UnityAction<int> onActivatedCallback;

	protected SpawnRolePreviewer spawnRolePreviewer;

	protected GameObject spawnRolePreviewObject;

	protected bool isDragging;

	public bool IsDragging
	{
		set
		{
			isDragging = value;
		}
	}

	public int WOID => woId;

	public virtual GamePassTier Tier => GamePassTier.Tier0;

	public virtual void Initialize(int spawnRoleIndex, int woId, GamePassTier tierRequirement, MVTeam team, UnityAction<int> onSelectedCallback, UnityAction<int> onActivatedCallback)
	{
		this.spawnRoleIndex = spawnRoleIndex;
		this.woId = woId;
		this.onSelectedCallback = onSelectedCallback;
		this.onActivatedCallback = onActivatedCallback;
	}

	public void SetupPreviewImage(GameObject spawnRoleObject)
	{
		spawnRolePreviewObject = spawnRoleObject;
		spawnRolePreviewer = Object.Instantiate(spawnRolePreviewerPrefab);
		GameObject gameObject = Object.Instantiate(spawnRoleObject);
		gameObject.transform.localRotation = Quaternion.identity;
		Transform previewSpawnRoleRoot = new GameObject("Preview Root - TierShopItem").transform;
		Vector3 previewPosition = new Vector3(500f, 100f, 100f + 10f * (float)spawnRoleIndex);
		Vector3 cameraOffset = new Vector3(0f, 1.5f, -6f);
		spawnRolePreviewer.Initialize(previewWidth, previewHeight, CameraClearFlags.Color, LayerFlags.Default | LayerFlags.CamRotateTarget, cameraOffset, previewSpawnRoleRoot, previewPosition, "SpawnRole", spawnRoleIndex, gameObject);
		spawnRoleImage.texture = spawnRolePreviewer.PreviewTexture;
	}

	public virtual void Select()
	{
		if (!isSelected)
		{
			onSelectedCallback(spawnRoleIndex);
		}
	}

	public virtual void OnSelctionHighlight()
	{
		isSelected = true;
		spawnRolePreviewer.SetRenderGrey(shouldRenderAsGrey: false);
		spawnRolePreviewer.StartActiveAnimation();
	}

	public virtual void OnSelected()
	{
		isSelected = true;
		spawnRolePreviewer.SetRenderGrey(shouldRenderAsGrey: false);
		spawnRolePreviewer.StartActiveAnimation();
	}

	public virtual void OnUnSelected()
	{
		isSelected = false;
		spawnRolePreviewer.SetRenderGrey(shouldRenderAsGrey: true);
		spawnRolePreviewer.StartInactiveAnimation();
	}

	public virtual void UpdateButtonUI()
	{
	}

	public void OnPointerDown(PointerEventData eventData)
	{
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (!isDragging)
		{
			Select();
		}
	}

	public void Activate()
	{
		spawnRolePreviewer.ActivatePreview();
	}

	public void Deactivate()
	{
		spawnRolePreviewer.DeactivatePreview();
	}

	protected virtual void OnDestroy()
	{
		Object.Destroy(spawnRolePreviewer);
	}
}
