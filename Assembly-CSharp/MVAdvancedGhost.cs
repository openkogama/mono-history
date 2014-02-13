using System.Collections;
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

	public override Vector3 WorldPivot
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return transform.position;
		}
	}

	public MVAdvancedGhost(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
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
		editableCubeModelWrapper = new AdvancedGhostCubeModelWrapper(mVCubeModelBase, ((Component)advancedGhostBehaviour.GhostVisualization).transform);
		advancedGhostMotor.Init(((Component)advancedGhostBehaviour).transform, interactable);
		if (MVGameController.Instance.GameMode == MVGameMode.Edit)
		{
			SetupEditorIcon(mVCubeModelBase);
		}
		Hide();
		MVGameController.Instance.Game.GameStateController.AddUpdateObject(this);
		OnDataUpdate();
	}

	private void SetupEditorIcon(MVCubeModelBase cubeModelBody)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		advancedGhostIcon = Object.Instantiate(Resources.Load("Prefabs/AdvancedGhost/GhostEditorIcon", typeof(AdvancedGhostIcon))) as AdvancedGhostIcon;
		((Component)advancedGhostIcon).transform.parent = transform;
		((Component)advancedGhostIcon).transform.localPosition = Vector3.zero;
		((Component)advancedGhostIcon).transform.localRotation = Quaternion.identity;
		advancedGhostIcon.Init(cubeModelBody);
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		advancedGhostBehaviour = GameObject.GetComponentInChildren<AdvancedGhostBehaviour>();
		MVCubeModelBase mVCubeModelBase = (MVCubeModelBase)GetChild("BodyCubeModel");
		SetupEditorIcon(mVCubeModelBase);
		mVCubeModelBase.GameObject.SetActiveRecursively(false);
		((Component)advancedGhostIcon).gameObject.SetLayerRecursively(LayerMask.NameToLayer("Default"));
		SetGameMode(isPlayMode: false);
	}

	private void SetGameMode(bool isPlayMode)
	{
		if (isPlayMode)
		{
			((Component)advancedGhostBehaviour).gameObject.SetActiveRecursively(true);
			if ((Object)(object)advancedGhostIcon != (Object)null)
			{
				((Component)advancedGhostIcon).gameObject.SetActiveRecursively(false);
			}
			advancedGhostBehaviour.SetGameMode(isPlayMode);
			if (editableCubeModelWrapper.CubeModelIsBeingEdited)
			{
				Debug.LogWarning((object)"Todo: Fix this hack. This is simply because OnExitObject is not called if user enters playmode while editing cube model");
				editableCubeModelWrapper.ExitEdit();
				editableCubeModelWrapper.CubeModel.GameObject.SetActiveRecursively(true);
			}
		}
		else
		{
			((Component)advancedGhostBehaviour).gameObject.SetActiveRecursively(false);
			((Component)advancedGhostIcon).gameObject.SetActiveRecursively(true);
		}
	}

	public override bool OnEnterObject(EditorStateMachine e)
	{
		((Component)advancedGhostIcon).gameObject.SetActiveRecursively(false);
		return editableCubeModelWrapper.OnEnterObject(e, transform);
	}

	public override bool OnExitObject(EditorStateMachine e)
	{
		Debug.Log((object)"On exit object");
		((Component)advancedGhostIcon).gameObject.SetActiveRecursively(true);
		return editableCubeModelWrapper.OnExitObject(e);
	}

	public override Bounds GetLocalBounds(BoundsContext boundsContext)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
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
		MVGameController.Instance.Game.GameStateController.RemoveObject(this);
		base.Destroy();
	}

	public override Vector3 GetTargetPosition()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)advancedGhostBehaviour).transform.position + Vector3.up;
	}

	private void ReceiveDamage(float damage)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		advancedGhostBehaviour.ReceivedDamage();
		if (interactable.IsDead())
		{
			HashSet<int> worldIDsRecursive = WorldIDsRecursive;
			MVGameController.Instance.WOCM.SharedWorldObjectGameplayFunctions.ExplosionCreator.Explode(((Component)advancedGhostBehaviour.GhostVisualization).transform.position, deathExplosionDamageValue, deathExplosionRadius, deathExplosionImpulse, worldIDsRecursive);
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
			SetGameMode(MVGameController.Instance.Game.IsPlaying);
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
		((Component)advancedGhostBehaviour).gameObject.SetActiveRecursively(visible);
		if ((Object)(object)advancedGhostIcon != (Object)null)
		{
			((Component)advancedGhostIcon).gameObject.SetActiveRecursively(visible);
		}
	}

	public override void OnDataUpdate()
	{
		advancedGhostBehaviour.Speed = (float)Data["Speed"];
		advancedGhostBehaviour.Radius = (float)Data["Radius"];
		if ((Object)(object)advancedGhostIcon != (Object)null)
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
