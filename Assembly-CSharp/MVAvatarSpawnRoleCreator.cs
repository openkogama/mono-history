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

	private UseInteractor useInteractor;

	private readonly SettingsReporter settingsReporter;

	private CullingSubscriberDynamic cullingSubscriberDynamic;

	public Action OnBodyUpdate;

	public const string teamKey = "team";

	public const string tierKey = "RequiredRank";

	private int AvatarRuntimePrototypeRoot { get; set; }

	public override MVWorldObjectDocumentationType DocumentationType => MVWorldObjectDocumentationType.AvatarClass;

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
		InteractionFlags |= InteractionFlags.CanEnterPlay;
		spawnRoleCreatorObject = (MVAvatarSpawnRoleCreatorObject)component;
		spawnRoleCreatorObject.SpawnPlate.TeamTint(Team);
		useInteractor = new UseInteractor(this, spawnRoleCreatorObject.useInteractionRotator, reset: false, null, null);
		GameRankRequirement useRequirement = new GameRankRequirement(spawnRoleCreatorObject.useInteractionRotator, this, hasUseButtonWhenFree: false)
		{
			ShouldDeleteWhenTier0 = false
		};
		useInteractor.AddRequirement(useRequirement);
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
		MVGameControllerBase.Game.TeamManager.OnAddSpawnPoint(Id, Team);
		HideBody();
		useInteractor.UpdateData(Data);
		spawnRoleCreatorObject.useInteractionRotator.transform.SetLayerRecursively(LayerMask.NameToLayer("Logic"));
		cullingSubscriberDynamic = new CullingSubscriberDynamic(4f, 3, spawnRoleCreatorObject.gameObject);
		AvatarPrototype.SpawnRoleCreatorId = Id;
	}

	public override void InitializeInventory()
	{
		base.Initialize();
		base.InitializeInventory();
		TryShowBody();
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
		if (MVGameControllerBase.MainCameraManager.BlueModeEnabled)
		{
			SharedCubeFunctions.SetLayerRecursively(AvatarPrototype.Transform, select: true);
			SharedCubeFunctions.SetLayerRecursively(spawnRoleCreatorObject.SpawnPlate.transform, select: true);
		}
		HideBody();
		AvatarPrototype.SpawnRoleCreatorId = Id;
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

	public override void OnDataUpdate()
	{
		base.OnDataUpdate();
		useInteractor.UpdateData(Data);
		spawnRoleCreatorObject.useInteractionRotator.transform.SetLayerRecursively(LayerMask.NameToLayer("Logic"));
		if (MVGameControllerBase.MainCameraManager.BlueModeEnabled)
		{
			SharedCubeFunctions.SetLayerRecursively(spawnRoleCreatorObject.useInteractionRotator.transform, select: true);
		}
		spawnRoleCreatorObject.useInteractionRotator.SetActive(value: false);
		spawnRoleCreatorObject.useInteractionRotator.SetActive(value: true);
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
		if (cullingSubscriberDynamic != null)
		{
			cullingSubscriberDynamic.Destroy();
			cullingSubscriberDynamic = null;
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

	public override Vector3 GetClosestGridPoint(float gridSize, Vector3 position)
	{
		Vector3 vector = Vector3.one * 2.2f;
		return SharedCubeFunctions.GetClosestGridPoint(position, gameObject.transform.rotation, gridSize, vector);
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

	private void TryShowBody()
	{
		int num = (int)childIdMap["bodyId"];
		MVWorldObjectClient mVWorldObjectClient = (MVWorldObjectClient)MVGameControllerBase.WOCM.GetWorldObject(num);
		if (mVWorldObjectClient != null && mVWorldObjectClient is MVGroup)
		{
			ShowBody(mVWorldObjectClient);
			return;
		}
		foreach (MVWorldObjectClient child in Children)
		{
			if (child is MVBody)
			{
				ShowBody(this);
				break;
			}
		}
	}

	private void ShowBody(MVWorldObjectClient originalBody)
	{
		foreach (MVWorldObjectClient child in ((MVGroup)originalBody).Children)
		{
			if (child is MVBody)
			{
				((MVBody)child).LayerToSetTo = "Hidden";
				break;
			}
		}
		Transform.SetLayerRecursively(LayerMask.NameToLayer("Hidden"));
	}
}
