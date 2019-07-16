using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using MV.WorldObject.KogamaSettings.KogamaSettingsCore.Client;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings.AttributePrototypeSettings;
using MV.WorldObject.SpawnRoles;
using UnityEngine;

public class MVAvatarSpawnRoleCreator : MVBlueprintBase, ISpawnRolePreviewObject
{
	private MVAvatarSpawnRoleCreatorObject spawnRoleCreatorObject;

	private bool isInWorld;

	private readonly SettingsReporter settingsReporter;

	public Action OnBodyUpdate;

	public const string teamKey = "team";

	public const string tierKey = "RequiredRank";

	private int AvatarRuntimePrototypeRoot { get; set; }

	private MVPreviewAvatar AvatarPrototype => (MVPreviewAvatar)children[AvatarRuntimePrototypeRoot];

	public AttributeSettingsManager AttributeSettingsManagerAvatar => new AttributeSettingsManager(Data, AttributeSettingWoType.Avatar, settingsReporter);

	public MVTeam Team
	{
		get
		{
			return (MVTeam)(int)Data["team"];
		}
		set
		{
			MVGameControllerBase.OperationRequests.UpdateWorldObjectDataPartial(Id, "team", (int)value);
		}
	}

	public GamePassTier Tier
	{
		get
		{
			return (GamePassTier)(int)Data["RequiredRank"];
		}
		set
		{
			Data["RequiredRank"] = (int)value;
			PartialDataUpdate(Id, new Dictionary<object, object> { 
			{
				"RequiredRank",
				Data["RequiredRank"]
			} });
		}
	}

	public MVAvatarSpawnRoleCreator(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVAvatarSpawnRoleCreatorPrefab, worldObjects)
	{
		settingsReporter = new SettingsReporter(this, PartialDataUpdate, PartialDataRemove);
		settingsReporter.OnValueChangedLocal += SettingsReporterOnOnValueChangedLocal;
		settingsReporter.OnValueRemovedLocal += SettingsReporterOnOnValueRemovedLocal;
		InteractionFlags |= InteractionFlags.HasSettings;
		spawnRoleCreatorObject = (MVAvatarSpawnRoleCreatorObject)component;
		spawnRoleCreatorObject.SpawnPlate.TeamTint(Team);
	}

	public override void Initialize()
	{
		base.Initialize();
		isInWorld = true;
		transform.SetLayerRecursively(LayerMask.NameToLayer("Logic"));
		bool flag = false;
		foreach (MVWorldObjectClient child in Children)
		{
			if (child.WorldObjectType == WorldObjectType.PlayModeAvatar)
			{
				if (flag)
				{
					throw new Exception("Multiple playmode avatars detected");
				}
				AvatarRuntimePrototypeRoot = child.Id;
				flag = true;
			}
		}
		Debug.Log("AvatarRuntimePrototypeRoot " + AvatarRuntimePrototypeRoot);
		MVGameControllerBase.Game.TeamManager.OnAddSpawnPoint(Id, Team);
		HideBody();
	}

	public void UpdateAvatarBody(SpawnRoleBodySwitchData spawnRoleBodySwitchData)
	{
		childIdMap["bodyId"] = spawnRoleBodySwitchData.addedBodyWoId;
		Debug.Log("Children");
		foreach (MVWorldObjectClient child in AvatarPrototype.Children)
		{
			Debug.Log(child);
		}
		if (OnBodyUpdate != null)
		{
			OnBodyUpdate();
		}
		transform.SetLayerRecursively(LayerMask.NameToLayer("Logic"));
		SharedCubeFunctions.SetLayerRecursively(AvatarPrototype.Transform, select: true);
	}

	public GameObject GetSpawnRolePreviewObject()
	{
		return AvatarPrototype.GameObject;
	}

	public MVTeam GetTeamRequirement()
	{
		return Team;
	}

	public GamePassTier GetTierRequirement()
	{
		return Tier;
	}

	public override void PartialUpdateWOData(Dictionary<object, object> woData)
	{
		MVTeam team = Team;
		Debug.Log("prevTeam " + team);
		base.PartialUpdateWOData(woData);
		Debug.Log("Team " + Team);
		AvatarPrototype.PartialUpdateWOData(woData);
		if (team != Team)
		{
			MVGameControllerBase.Game.TeamManager.OnAddSpawnPoint(Id, Team);
			MVGameControllerBase.Game.TeamManager.OnRemoveSpawnPoint(Id, team);
			spawnRoleCreatorObject.SpawnPlate.TeamTint(Team);
		}
	}

	public override void PartialRemoveFromWOData(Dictionary<object, object> entriesToRemove)
	{
		base.PartialRemoveFromWOData(entriesToRemove);
		AvatarPrototype.PartialRemoveFromWOData(entriesToRemove);
	}

	public override void Destroy()
	{
		base.Destroy();
		if (isInWorld && !MVGameControllerBase.Quitting)
		{
			MVGameControllerBase.Game.TeamManager.OnRemoveSpawnPoint(Id, Team);
		}
	}

	public override bool Delete(MVWorldObjectClientManager worldObjectClientManager, ref string errorText)
	{
		if (MVGameControllerBase.Game.TeamManager.NumSpawnPoint <= 1)
		{
			errorText = "You cannot delete the last spawn-point.\nAll Projects must have at least one";
			return false;
		}
		return base.Delete(worldObjectClientManager, ref errorText);
	}

	private void SettingsReporterOnOnValueRemovedLocal(Dictionary<object, object> obj)
	{
		Debug.Log("Removing setting");
		AvatarPrototype.PartialRemoveFromWOData(obj);
	}

	private void SettingsReporterOnOnValueChangedLocal(Dictionary<object, object> obj)
	{
		Debug.Log("Adding setting");
		AvatarPrototype.PartialUpdateWOData(obj);
	}

	private void PartialDataRemove(int arg1, Dictionary<object, object> arg2)
	{
		MVGameControllerBase.OperationRequests.RemoveWorldObjectDataPartial(arg1, arg2);
	}

	private void PartialDataUpdate(int arg1, Dictionary<object, object> arg2)
	{
		MVGameControllerBase.OperationRequests.UpdateWorldObjectDataPartial(arg1, arg2);
		spawnRoleCreatorObject.SpawnPlate.TeamTint(Team);
	}

	private void HideBody()
	{
		int num = (int)childIdMap["bodyId"];
		((MVWorldObjectClient)MVGameControllerBase.WOCM.GetWorldObject(num))?.GameObject.SetActive(value: false);
	}
}
