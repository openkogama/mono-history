using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings.AttributeSettingTypes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GamePassesSpawnRoleRewardInfo : MonoBehaviour
{
	[SerializeField]
	private Image spawnRoleTeamImage;

	[SerializeField]
	private RawImage spawnRoleImage;

	[SerializeField]
	private Text spawnRoleCostAmount;

	[SerializeField]
	private GameObject spawnRoleEditButton;

	[SerializeField]
	private SpawnRoleEditorMenu spawnRoleEditorMenuPrefab;

	[SerializeField]
	private SpawnRolePreviewer spawnRolePreviewerPrefab;

	[SerializeField]
	private SpawnRoleSelectionSkillMenu skillMenuPrefab;

	[SerializeField]
	private GameObject backgroundTier1;

	[SerializeField]
	private GameObject backgroundTier2;

	[SerializeField]
	private GameObject backgroundTier3;

	[SerializeField]
	private int previewWidth;

	[SerializeField]
	private int previewHeight;

	private SpawnRolePreviewer spawnRolePreviewer;

	private int spawnRoleIndex;

	private MVAvatarSpawnRoleCreator spawnRole;

	private int woid;

	private GamePassTier tierRequirment;

	private GameObject spawnRolePreviewObject;

	private void OnDestroy()
	{
		MVAvatarSpawnRoleCreator mVAvatarSpawnRoleCreator = spawnRole;
		mVAvatarSpawnRoleCreator.OnBodyUpdate = (Action)Delegate.Remove(mVAvatarSpawnRoleCreator.OnBodyUpdate, new Action(OnSpawnRoleBodyUpdate));
		if (spawnRolePreviewer != null)
		{
			UnityEngine.Object.Destroy(spawnRolePreviewer);
		}
	}

	public void Initialize(int spawnRoleIndex, GameObject spawnRolePreviewObject, MVAvatarSpawnRoleCreator spawnRole, GamePassTier tierRequirment)
	{
		this.spawnRoleIndex = spawnRoleIndex;
		this.spawnRole = spawnRole;
		this.tierRequirment = tierRequirment;
		this.spawnRolePreviewObject = spawnRolePreviewObject;
		woid = spawnRole.Id;
		spawnRoleTeamImage.color = GetTeamRequirementColor(spawnRole.Team);
		ChangeBackground(tierRequirment);
		SetupPreviewImage(spawnRolePreviewObject);
		if (MVGameControllerBase.GameSessionData.gameMode != MVGameMode.Edit || MVGameControllerBase.EditModeUI.IsInPlayInEditMode)
		{
			spawnRoleEditButton.SetActive(value: false);
		}
		int skillCost = CalculateSpawnRoleCost();
		spawnRoleCostAmount.text = skillCost.ToString();
		spawnRoleCostAmount.color = SpawnRolesSkillDataManager.GetCostColor(skillCost);
		spawnRole.OnBodyUpdate = (Action)Delegate.Combine(spawnRole.OnBodyUpdate, new Action(OnSpawnRoleBodyUpdate));
	}

	public void OnPressed()
	{
		SpawnRoleSelectionSkillMenu skillMenu = UnityEngine.Object.Instantiate(skillMenuPrefab);
		skillMenu.Initialize(woid, tierRequirment, spawnRolePreviewObject);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(skillMenu.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
	}

	public void OnEditPressed()
	{
		if (MVGameControllerBase.GameSessionData.gameMode == MVGameMode.Edit && !MVGameControllerBase.EditModeUI.IsInPlayInEditMode)
		{
			SpawnRoleEditorMenu spawnRoleMenu = UnityEngine.Object.Instantiate(spawnRoleEditorMenuPrefab);
			spawnRoleMenu.Initialize(spawnRole.Id);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(spawnRoleMenu.gameObject, UIPushOption.HideAll | UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
			});
		}
	}

	private Color GetTeamRequirementColor(MVTeam team)
	{
		return team switch
		{
			MVTeam.Blue => Styles.GetColor(ColorStyle.TeamBlue), 
			MVTeam.Red => Styles.GetColor(ColorStyle.TeamRed), 
			MVTeam.Green => Styles.GetColor(ColorStyle.TeamGreen), 
			MVTeam.Yellow => Styles.GetColor(ColorStyle.TeamYellow), 
			_ => Styles.GetColor(ColorStyle.TeamNone), 
		};
	}

	private void ChangeBackground(GamePassTier tier)
	{
		bool flag = tier == GamePassTier.Tier1;
		bool flag2 = tier == GamePassTier.Tier2;
		bool flag3 = tier == GamePassTier.Tier3;
		if (backgroundTier1.activeSelf != flag)
		{
			backgroundTier1.SetActive(flag);
		}
		if (backgroundTier2.activeSelf != flag2)
		{
			backgroundTier2.SetActive(flag2);
		}
		if (backgroundTier3.activeSelf != flag3)
		{
			backgroundTier3.SetActive(flag3);
		}
	}

	private void SetupPreviewImage(GameObject spawnRolePreviewObject)
	{
		if (spawnRolePreviewer != null)
		{
			UnityEngine.Object.Destroy(spawnRolePreviewer);
		}
		spawnRolePreviewer = UnityEngine.Object.Instantiate(spawnRolePreviewerPrefab);
		GameObject gameObject = UnityEngine.Object.Instantiate(spawnRolePreviewObject);
		gameObject.transform.localRotation = Quaternion.identity;
		Transform previewSpawnRoleRoot = new GameObject("Preview Root - TierShopItem").transform;
		Vector3 previewPosition = new Vector3(500f, 500f, 10f * (float)spawnRoleIndex);
		Vector3 cameraOffset = new Vector3(0f, 1f, -4.5f);
		spawnRolePreviewer.Initialize(previewWidth, previewHeight, CameraClearFlags.Color, LayerFlags.Default | LayerFlags.CamRotateTarget, cameraOffset, previewSpawnRoleRoot, previewPosition, "SpawnRole", spawnRoleIndex, gameObject);
		spawnRoleImage.texture = spawnRolePreviewer.PreviewTexture;
	}

	private void OnSpawnRoleBodyUpdate()
	{
		SetupPreviewImage(spawnRole.GetSpawnRolePreviewObject());
	}

	private int CalculateSpawnRoleCost()
	{
		int num = 0;
		KogamaSettingsCollectionBase kogamaSettingsCollectionBase = (KogamaSettingsCollectionBase)spawnRole.AttributeSettingsManagerAvatar.Settings;
		if (kogamaSettingsCollectionBase == null)
		{
			return num;
		}
		foreach (KeyValuePair<string, KogamaSettingWrapperBase> child in kogamaSettingsCollectionBase.Children)
		{
			num += ((IAttributeSetting)child.Value).AttributeValue;
		}
		return num;
	}
}
