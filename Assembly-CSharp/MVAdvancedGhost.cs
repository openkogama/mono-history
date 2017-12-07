using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.Events;

public class MVAdvancedGhost : MVBlueprintBase, ITeamInteractorNPC, IGameStateControllerSubscriber
{
	private const float deathExplosionDamageValue = 20f;

	private const float deathExplosionRadius = 5f;

	private const float deathExplosionImpulse = 1000f;

	private AdvancedGhostBehaviour advancedGhostBehaviour;

	private ClientSideNPCInteractable interactable;

	private AdvancedGhostCubeModelWrapper editableCubeModelWrapper;

	private AdvancedGhostIcon advancedGhostIcon;

	private AdvancedGhostObject advGhostObject;

	private ClientSideNPCInteractionHandler interactionHandler;

	public override MVWorldObjectDocumentationType DocumentationType => MVWorldObjectDocumentationType.Oculus;

	private MVTeam Team => (!Data.ContainsKey("team")) ? MVTeam.Server : ((MVTeam)(int)Data["team"]);

	public override Vector3 WorldPivot => transform.position;

	public MVAdvancedGhost(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVAdvancedGhostPrefab, worldObjects)
	{
		interactionFlags |= InteractionFlags.Selectable | InteractionFlags.CanRotateY | InteractionFlags.CanEdit | InteractionFlags.CanClone | InteractionFlags.HasSettings | InteractionFlags.CanUseTeam;
		advGhostObject = (AdvancedGhostObject)component;
	}

	public override void Initialize()
	{
		base.Initialize();
		MVCubeModelInstance mVCubeModelInstance = (MVCubeModelInstance)GetChild("BodyCubeModel");
		interactionHandler = GameObject.GetComponent<ClientSideNPCInteractionHandler>();
		interactionHandler.FindWorldObjectParent();
		interactable = GameObject.AddComponent<ClientSideNPCInteractable>();
		interactable.Init(ReceiveDamage);
		AdvancedGhostMotor advancedGhostMotor = GameObject.AddComponent<AdvancedGhostMotor>();
		advancedGhostBehaviour = GameObject.GetComponentInChildren<AdvancedGhostBehaviour>();
		advancedGhostBehaviour.Init(mVCubeModelInstance, advancedGhostMotor, interactable.IsDead, id);
		editableCubeModelWrapper = new AdvancedGhostCubeModelWrapper(mVCubeModelInstance, advancedGhostBehaviour.GhostVisualization.transform);
		advancedGhostMotor.Init(advancedGhostBehaviour.gameObject, interactable, advancedGhostBehaviour.CullingSubscriberBase);
		if (MVGameControllerBase.GameMode == MVGameMode.Edit)
		{
			SetupEditorIcon(mVCubeModelInstance, enableCulling: true);
			PositionChanged = (UnityAction<MVWorldObjectClient, PositionChangedEventArgs>)Delegate.Combine(PositionChanged, new UnityAction<MVWorldObjectClient, PositionChangedEventArgs>(OnPositionChanged));
		}
		MVGameControllerBase.Game.GameStateController.AddUpdateObject(this);
		OnDataUpdate();
	}

	private void OnPositionChanged(MVWorldObjectClient arg0, PositionChangedEventArgs positionChangedEventArgs)
	{
		advancedGhostBehaviour.EditModeUpdateCulling();
	}

	private void SetupEditorIcon(MVCubeModelBase cubeModelBody, bool enableCulling)
	{
		advancedGhostIcon = UnityEngine.Object.Instantiate(PrefabPool.Instance.GhostEditorIconObject);
		advancedGhostIcon.transform.parent = transform;
		advancedGhostIcon.transform.localPosition = Vector3.zero;
		advancedGhostIcon.transform.localRotation = Quaternion.identity;
		advancedGhostIcon.Init(this, cubeModelBody, enableCulling, Team);
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		advancedGhostBehaviour = GameObject.GetComponentInChildren<AdvancedGhostBehaviour>();
		MVCubeModelBase mVCubeModelBase = (MVCubeModelBase)GetChild("BodyCubeModel");
		SetupEditorIcon(mVCubeModelBase, enableCulling: false);
		mVCubeModelBase.GameObject.SetActive(value: false);
		advancedGhostIcon.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Default"));
		advancedGhostBehaviour.gameObject.SetActive(value: false);
	}

	private void SetGameMode(bool isPlayMode)
	{
		if (advancedGhostIcon != null)
		{
			advancedGhostIcon.SetGameMode(isPlayMode);
		}
		advancedGhostBehaviour.SetGameMode(isPlayMode);
		if (isPlayMode && editableCubeModelWrapper.CubeModelIsBeingEdited)
		{
			Debug.LogWarning("Todo: Fix this hack. This is simply because OnExitObject is not called if user enters playmode while editing cube model");
			editableCubeModelWrapper.ExitEdit();
			editableCubeModelWrapper.CubeModel.GameObject.SetActive(value: true);
		}
	}

	public override bool OnEnterObject(EditorStateMachine e)
	{
		advancedGhostIcon.gameObject.SetActive(value: false);
		return editableCubeModelWrapper.OnEnterObject(e, transform);
	}

	public override bool OnExitObject(EditorStateMachine e)
	{
		Debug.Log("On exit object");
		advancedGhostIcon.gameObject.SetActive(value: true);
		return editableCubeModelWrapper.OnExitObject(e);
	}

	public override Bounds GetLocalBounds(BoundsContext boundsContext)
	{
		return new Bounds(Vector3.up, 3f * Vector3.one);
	}

	public override void Select(Color color)
	{
		AddSelectionBox();
	}

	public override void DeSelect()
	{
		RemoveSelectionBox();
	}

	public override void Destroy()
	{
		MVGameControllerBase.Game.GameStateController.RemoveObject(this);
		base.Destroy();
	}

	public override Vector3 GetTargetPosition()
	{
		return advancedGhostBehaviour.transform.position + Vector3.up;
	}

	private void ReceiveDamage(float amount, MVPlayer damageDealer, PlayerKilledByType damageType)
	{
		advancedGhostBehaviour.ReceivedDamage();
		if (interactable.IsDead())
		{
			HashSet<int> worldIDsRecursive = WorldIDsRecursive;
			SharedWorldObjectGameplayFunctions.Explosion.Explode(PrefabPool.Instance.ParticleExplosion, advancedGhostBehaviour.GhostVisualization.transform.position, 20f, 5f, 1000f, local: true, null, worldIDsRecursive);
		}
	}

	public override void Reset()
	{
		interactable.Reset();
		advancedGhostBehaviour.Reset();
	}

	public bool IsOnSameTeam(MVTeam team)
	{
		return advancedGhostBehaviour.Team == team;
	}

	public override void OnDataUpdate()
	{
		advancedGhostBehaviour.Speed = (float)Data["Speed"];
		advancedGhostBehaviour.Radius = (float)Data["Radius"];
		if (advancedGhostIcon != null)
		{
			advancedGhostIcon.Radius = (float)Data["Radius"];
		}
		if (Team == MVTeam.None)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add("team", 0);
			MVGameControllerBase.OperationRequests.RemoveWorldObjectDataPartial(id, dictionary);
		}
		else
		{
			SetTeam(Team);
		}
		advancedGhostBehaviour.Lives = -1;
		if (Data.ContainsKey("Lives"))
		{
			advancedGhostBehaviour.Lives = (int)Data["Lives"];
		}
	}

	private void SetTeam(MVTeam team)
	{
		advGhostObject.TintObject.TeamTint(team);
		advancedGhostBehaviour.Team = team;
		interactionHandler.SetTeam(team);
		if (MVGameControllerBase.GameMode == MVGameMode.Edit)
		{
			SetTeam_Edit(team);
		}
	}

	private void SetTeam_Edit(MVTeam team)
	{
		advancedGhostIcon.Team = team;
	}

	public void GameStateChanged(UpdateCondition condition)
	{
		if (condition == UpdateCondition.EDITOR)
		{
			SetGameMode(isPlayMode: false);
		}
		else
		{
			SetGameMode(isPlayMode: true);
		}
	}
}
