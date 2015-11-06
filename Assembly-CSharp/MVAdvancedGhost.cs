using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class MVAdvancedGhost : MVBlueprintBase, IGameStateControllerSubscriber
{
	private const string _prefab = "Prefabs/AdvancedGhost/AdvancedGhost";

	private AdvancedGhostBehaviour advancedGhostBehaviour;

	private ClientSideNPCInteractable interactable;

	private AdvancedGhostCubeModelWrapper editableCubeModelWrapper;

	private AdvancedGhostIcon advancedGhostIcon;

	private float deathExplosionDamageValue = 20f;

	private float deathExplosionRadius = 5f;

	private float deathExplosionImpulse = 1000f;

	private bool visible;

	private float cullDistance = 100f;

	public override Vector3 WorldPivot => transform.position;

	public MVAdvancedGhost(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/AdvancedGhost/AdvancedGhost", worldObjects)
	{
		interactionFlags |= InteractionFlags.Selectable | InteractionFlags.CanRotateY | InteractionFlags.CanEdit | InteractionFlags.CanClone | InteractionFlags.HasSettings;
	}

	public override void Initialize()
	{
		base.Initialize();
		MVCubeModelBase mVCubeModelBase = (MVCubeModelBase)GetChild("BodyCubeModel");
		GameObject.AddComponent<ClientSideNPCInteractionHandler>();
		interactable = GameObject.AddComponent<ClientSideNPCInteractable>();
		interactable.Init(ReceiveDamage);
		AdvancedGhostMotor advancedGhostMotor = GameObject.AddComponent<AdvancedGhostMotor>();
		advancedGhostBehaviour = GameObject.GetComponentInChildren<AdvancedGhostBehaviour>();
		advancedGhostBehaviour.Init(mVCubeModelBase, advancedGhostMotor, interactable.IsDead, id);
		editableCubeModelWrapper = new AdvancedGhostCubeModelWrapper(mVCubeModelBase, advancedGhostBehaviour.GhostVisualization.transform);
		advancedGhostMotor.Init(advancedGhostBehaviour.gameObject, interactable);
		if (MVGameControllerBase.GameMode == MVGameMode.Edit)
		{
			SetupEditorIcon(mVCubeModelBase);
		}
		Hide();
		MVGameControllerBase.Game.GameStateController.AddUpdateObject(this);
		OnDataUpdate();
	}

	private void SetupEditorIcon(MVCubeModelBase cubeModelBody)
	{
		advancedGhostIcon = Object.Instantiate(Resources.Load("Prefabs/AdvancedGhost/GhostEditorIcon", typeof(AdvancedGhostIcon))) as AdvancedGhostIcon;
		advancedGhostIcon.transform.parent = transform;
		advancedGhostIcon.transform.localPosition = Vector3.zero;
		advancedGhostIcon.transform.localRotation = Quaternion.identity;
		advancedGhostIcon.Init(cubeModelBody);
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		advancedGhostBehaviour = GameObject.GetComponentInChildren<AdvancedGhostBehaviour>();
		MVCubeModelBase mVCubeModelBase = (MVCubeModelBase)GetChild("BodyCubeModel");
		SetupEditorIcon(mVCubeModelBase);
		mVCubeModelBase.GameObject.SetActive(value: false);
		advancedGhostIcon.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Default"));
		SetGameMode(isPlayMode: false);
	}

	private void SetGameMode(bool isPlayMode)
	{
		if (isPlayMode)
		{
			advancedGhostBehaviour.gameObject.SetActive(value: true);
			if (advancedGhostIcon != null)
			{
				advancedGhostIcon.gameObject.SetActive(value: false);
			}
			advancedGhostBehaviour.SetGameMode(isPlayMode);
			if (editableCubeModelWrapper.CubeModelIsBeingEdited)
			{
				Debug.LogWarning("Todo: Fix this hack. This is simply because OnExitObject is not called if user enters playmode while editing cube model");
				editableCubeModelWrapper.ExitEdit();
				editableCubeModelWrapper.CubeModel.GameObject.SetActive(value: true);
			}
		}
		else
		{
			advancedGhostBehaviour.gameObject.SetActive(value: false);
			advancedGhostIcon.gameObject.SetActive(value: true);
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
			HandleGameCounting(amount, damageDealer, damageType);
			HashSet<int> worldIDsRecursive = WorldIDsRecursive;
			SharedWorldObjectGameplayFunctions.Explosion.Explode("ParticleFX/Explosion", advancedGhostBehaviour.GhostVisualization.transform.position, deathExplosionDamageValue, deathExplosionRadius, deathExplosionImpulse, local: true, null, worldIDsRecursive);
		}
	}

	private void HandleGameCounting(float amount, MVPlayer damageDealer, PlayerKilledByType damageType)
	{
		if (damageDealer.ActorNr == MVGameControllerBase.Game.LocalPlayer.ActorNr && (damageDealer.Avatar.Transform.position - advancedGhostBehaviour.transform.position).magnitude < 10f)
		{
			GameSessionCounters.Increment(GameSessionCounterType.OculusKilledByLocalPlayerInCloseCombat);
			Debug.Log("Close combat");
		}
	}

	public override void Reset()
	{
		interactable.Reset();
		advancedGhostBehaviour.Reset();
	}

	public override void ChangeLOD(float distance)
	{
		advancedGhostBehaviour.LOD = distance / cullDistance;
		if (!visible && distance < cullDistance)
		{
			SetGameMode(MVGameControllerBase.Game.IsPlaying);
			visible = true;
		}
		else if (visible && distance >= cullDistance)
		{
			Hide();
		}
	}

	private void Hide()
	{
		SetVisible(visible: false);
		visible = false;
	}

	private void SetVisible(bool visible)
	{
		advancedGhostBehaviour.gameObject.SetActive(visible);
		if (advancedGhostIcon != null)
		{
			advancedGhostIcon.gameObject.SetActive(visible);
		}
	}

	public override void OnDataUpdate()
	{
		advancedGhostBehaviour.Speed = (float)Data["Speed"];
		advancedGhostBehaviour.Radius = (float)Data["Radius"];
		if (advancedGhostIcon != null)
		{
			advancedGhostIcon.Radius = (float)Data["Radius"];
		}
	}

	public void GameStateChanged(UpdateCondition condition)
	{
		if (visible)
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
}
