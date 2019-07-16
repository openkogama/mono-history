using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings.AttributeSettingTypes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpawnRoleEditorMenu : MonoBehaviour
{
	[SerializeField]
	private Image TeamImage;

	[SerializeField]
	private Text spawnRoleCostText;

	[SerializeField]
	private SpawnRoleTierEditorMenu tierEditorMenu;

	[SerializeField]
	private SpawnRoleTeamEditor teamEditorPrefab;

	[SerializeField]
	private SpawnRoleLooksEditorMenu looksEditorMenuPrefab;

	[SerializeField]
	private SpawnRoleSkillsEditor skillsEditorMenuPrefab;

	private int spawnRoleWoId;

	private MVAvatarSpawnRoleCreator spawnRole;

	private AttributeSettingsManager spawnRoleAttributeSettingsManager;

	private SpawnRoleSkillsEditor skillsEditorMenu;

	public void OnTeamEditPressed()
	{
		SpawnRoleTeamEditor teamEditor = UnityEngine.Object.Instantiate(teamEditorPrefab);
		teamEditor.Initialize(spawnRole.Team, OnChangeTeam);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(teamEditor.gameObject, UIPushOption.None, null, UIGroupFlags.InventoryUI);
		});
	}

	public void OnLooksEditPressed()
	{
		SpawnRoleLooksEditorMenu looksEditorMenu = UnityEngine.Object.Instantiate(looksEditorMenuPrefab);
		looksEditorMenu.Initialize(spawnRoleWoId, spawnRole);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(looksEditorMenu.gameObject, UIPushOption.None, null, UIGroupFlags.InventoryUI);
		});
	}

	public void OnSkillsEditPressed()
	{
		skillsEditorMenu = UnityEngine.Object.Instantiate(skillsEditorMenuPrefab);
		skillsEditorMenu.Initialize(CalculateSpawnRoleCost(), spawnRole.Tier, spawnRoleAttributeSettingsManager, UpdateSpawnRoleCost);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(skillsEditorMenu.gameObject, UIPushOption.None, null, UIGroupFlags.InventoryUI);
		});
	}

	public void Initialize(int spawnRoleWoId)
	{
		this.spawnRoleWoId = spawnRoleWoId;
		spawnRole = (MVAvatarSpawnRoleCreator)MVGameControllerBase.WOCM.GetWorldObjectClient(spawnRoleWoId);
		spawnRoleAttributeSettingsManager = spawnRole.AttributeSettingsManagerAvatar;
		SharedCubeFunctions.SetLayerRecursively(spawnRole.Transform, select: true);
		Vector3 avatarOffset = CalculatePreviewOffset(spawnRole);
		Vector3 cameraOffset = spawnRole.Transform.right * 1f;
		MVGameControllerBase.MainCameraManager.CurrentCamera.FocusOnObject(spawnRole, 2f, avatarOffset, cameraOffset);
		ChangeTeamImageColor(spawnRole.Team);
		int newspawnRoleCost = CalculateSpawnRoleCost();
		tierEditorMenu.Initialize(spawnRole.Tier, newspawnRoleCost, OnChangeTier);
		UpdateSpawnRoleCost();
	}

	private void Update()
	{
		if (!MVGameControllerBase.MainCameraManager.BlueModeEnabled)
		{
			MVGameControllerBase.MainCameraManager.BlueModeEnabled = true;
		}
	}

	private Vector3 CalculatePreviewOffset(MVWorldObjectClient spawnRole)
	{
		float num = spawnRole.ComputeObjectRadius();
		float num2 = MVGameControllerBase.MainCameraManager.MainCamera.fieldOfView * 0.5f * 0.7f;
		float num3 = num / Mathf.Tan(num2 * ((float)Math.PI / 180f));
		float num4 = num3;
		Vector3 vector = 2f * Vector3.up;
		Vector3 worldPivot = spawnRole.WorldPivot;
		Vector3 vector2 = MVGameControllerBase.Game.LocalPlayer.SpawnRoleDataMediator.Position;
		Vector3 vector3 = worldPivot - (vector2 + vector);
		Vector3 vector4 = vector2;
		vector4 += vector3.normalized * (vector3.magnitude - num4);
		Vector3 vector5 = spawnRole.Position + spawnRole.Transform.forward * 3f + spawnRole.Transform.up * -1f;
		return vector5 - vector4;
	}

	private void UpdateSpawnRoleCost()
	{
		int num = CalculateSpawnRoleCost();
		spawnRoleCostText.text = num.ToString();
		spawnRoleCostText.color = SpawnRolesSkillDataManager.GetCostColor(num);
		tierEditorMenu.UpdateSpawnRoleCost(num);
		if (skillsEditorMenu != null)
		{
			skillsEditorMenu.UpdateSpawnRoleCost(num);
		}
	}

	private int CalculateSpawnRoleCost()
	{
		int num = 0;
		KogamaSettingsCollectionBase kogamaSettingsCollectionBase = (KogamaSettingsCollectionBase)spawnRoleAttributeSettingsManager.Settings;
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

	private void OnDestroy()
	{
		if (MVGameControllerBase.IsAlive)
		{
			if (MVGameControllerBase.MainCameraManager != null)
			{
				MVGameControllerBase.MainCameraManager.BlueModeEnabled = false;
			}
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(spawnRoleWoId);
			if (worldObjectClient != null)
			{
				SharedCubeFunctions.SetLayerRecursively(worldObjectClient.Transform, select: false);
			}
		}
	}

	private void OnChangeTeam(MVTeam newTeam)
	{
		spawnRole.Team = newTeam;
		ChangeTeamImageColor(newTeam);
	}

	private void ChangeTeamImageColor(MVTeam team)
	{
		switch (team)
		{
		case MVTeam.Blue:
			TeamImage.color = Styles.GetColor(ColorStyle.TeamBlue);
			break;
		case MVTeam.Red:
			TeamImage.color = Styles.GetColor(ColorStyle.TeamRed);
			break;
		case MVTeam.Green:
			TeamImage.color = Styles.GetColor(ColorStyle.TeamGreen);
			break;
		case MVTeam.Yellow:
			TeamImage.color = Styles.GetColor(ColorStyle.TeamYellow);
			break;
		default:
			TeamImage.color = Styles.GetColor(ColorStyle.OffGray);
			break;
		}
	}

	private void OnChangeTier(GamePassTier newTier)
	{
		spawnRole.Tier = newTier;
		if (skillsEditorMenu != null)
		{
			skillsEditorMenu.UpdateSpawnRoleTier(newTier);
		}
	}
}
