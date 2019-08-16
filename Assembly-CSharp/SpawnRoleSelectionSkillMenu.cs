using System.Collections.Generic;
using MV.Common;
using MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings.AttributeSettingTypes;
using UnityEngine;
using UnityEngine.UI;

public class SpawnRoleSelectionSkillMenu : MonoBehaviour
{
	[SerializeField]
	private Transform skillElementContainer;

	[SerializeField]
	private GameObject leftBorder;

	[SerializeField]
	private GameObject noSkillsText;

	[SerializeField]
	private RawImage spawnRolePreviewImage;

	[SerializeField]
	private Text spawnRoleCost;

	[SerializeField]
	private GameObject backgroundTier1;

	[SerializeField]
	private GameObject backgroundTier2;

	[SerializeField]
	private GameObject backgroundTier3;

	[SerializeField]
	private SpawnRoleSelectionSkillElement skillElementPrefab;

	[SerializeField]
	private SpawnRolePreviewer spawnRolePreviewerPrefab;

	[SerializeField]
	private SpawnRolesSkillDataManager skillDataManagerPrefab;

	[SerializeField]
	protected int previewWidth;

	[SerializeField]
	protected int previewHeight;

	private SpawnRolePreviewer spawnRolePreviewer;

	public void Initialize(int spawnRoleId, GamePassTier tierRequirement, GameObject spawnRolePreviewObject)
	{
		SpawnRolesSkillDataManager skillDataManager = Object.Instantiate(skillDataManagerPrefab);
		ChangeBackground(tierRequirement);
		spawnRolePreviewer = Object.Instantiate(spawnRolePreviewerPrefab);
		SetupPreviewImage(spawnRolePreviewObject);
		MVAvatarSpawnRoleCreator mVAvatarSpawnRoleCreator = (MVAvatarSpawnRoleCreator)MVGameControllerBase.WOCM.GetWorldObject(spawnRoleId);
		AttributeSettingsManager attributeSettingsManagerAvatar = mVAvatarSpawnRoleCreator.AttributeSettingsManagerAvatar;
		KogamaSettingsCollectionBase kogamaSettingsCollectionBase = (KogamaSettingsCollectionBase)attributeSettingsManagerAvatar.Settings;
		bool flag = false;
		if (kogamaSettingsCollectionBase != null)
		{
			foreach (KeyValuePair<string, KogamaSettingWrapperBase> child in kogamaSettingsCollectionBase.Children)
			{
				flag = true;
				SpawnRoleSelectionSkillElement spawnRoleSelectionSkillElement = Object.Instantiate(skillElementPrefab);
				spawnRoleSelectionSkillElement.Initialize(child.Key, skillDataManager, (KogamaSettingValueWrapperBase)child.Value);
				spawnRoleSelectionSkillElement.transform.SetParent(skillElementContainer);
			}
			int skillCost = CalculateTotalCostOfSkills(kogamaSettingsCollectionBase);
			spawnRoleCost.text = skillCost.ToString();
			spawnRoleCost.color = SpawnRolesSkillDataManager.GetCostColor(skillCost);
		}
		leftBorder.transform.SetAsFirstSibling();
		leftBorder.SetActive(flag);
		noSkillsText.SetActive(!flag);
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
		GameObject gameObject = Object.Instantiate(spawnRolePreviewObject);
		gameObject.transform.localRotation = Quaternion.identity;
		Transform previewSpawnRoleRoot = new GameObject("Preview Root - TierShopItem").transform;
		Vector3 previewPosition = new Vector3(500f, 500f, 0f);
		Vector3 cameraOffset = new Vector3(0f, 1f, -4.5f);
		spawnRolePreviewer.Initialize(previewWidth, previewHeight, CameraClearFlags.Color, LayerFlags.Default | LayerFlags.CamRotateTarget, cameraOffset, previewSpawnRoleRoot, previewPosition, "SpawnRole", 0, gameObject);
		spawnRolePreviewImage.texture = spawnRolePreviewer.PreviewTexture;
	}

	private int CalculateTotalCostOfSkills(KogamaSettingsCollectionBase subSettingData)
	{
		int num = 0;
		foreach (KeyValuePair<string, KogamaSettingWrapperBase> child in subSettingData.Children)
		{
			num += ((IAttributeSetting)child.Value).AttributeValue;
		}
		return num;
	}
}
