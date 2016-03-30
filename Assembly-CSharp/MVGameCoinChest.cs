using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class MVGameCoinChest : MVLogicObject
{
	public enum GameCoinChestClientState
	{
		Closed,
		Opening,
		Open
	}

	private static readonly UseGUIResult purchaseOptions = UseGUIResult.CanAfford | UseGUIResult.CannotAfford;

	private GameCoinChestClientState state;

	private UseInteractor useInteractor;

	private ObjectParticleEmitterScript particles;

	private GameCoinChestModelSelector modelSelector;

	private TriggerBoxEvents triggerBoxEvents;

	private AudioSource aSource;

	public override bool HasInputConnector => false;

	public override bool HasOutputConnector => false;

	public override bool Visible
	{
		get
		{
			return modelSelector.IsVisible();
		}
		set
		{
			if (state == GameCoinChestClientState.Closed)
			{
				modelSelector.Close();
			}
			else
			{
				modelSelector.Open();
			}
		}
	}

	public MVGameCoinChest(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVGameCoinChestPrefab, worldObjects)
	{
		particles = gameObject.GetComponent<ObjectParticleEmitterScript>();
		modelSelector = gameObject.GetComponent<GameCoinChestModelSelector>();
		triggerBoxEvents = gameObject.GetComponentInChildren<TriggerBoxEvents>();
		aSource = gameObject.GetComponent<AudioSource>();
		if (triggerBoxEvents != null)
		{
			triggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
		}
		else
		{
			Debug.LogError("A TriggerBoxEvents object is missing in PickupItem type: " + GetType().Name);
		}
		useInteractor = new UseInteractor(Id, gameObject, reset: false, triggerBoxEvents.Collider, OpenChest, IsUsable);
		LevelBasedUseRequirement useRequirement = new LevelBasedUseRequirement(gameObject, hasUseButtonWhenFree: false);
		useInteractor.AddRequirement(useRequirement);
		triggerBoxEvents.TriggerEnter += useInteractor.triggerBoxEvents_TriggerEnter;
		triggerBoxEvents.TriggerExit += useInteractor.triggerBoxEvents_TriggerExit;
		interactionFlags |= InteractionFlags.HasSettings;
		interactionFlags |= InteractionFlags.CanUseLevel;
		MVGameControllerBase.Game.GameCoinManager.ReportPickupChangeInEditor();
	}

	public override void OnDataUpdate()
	{
		useInteractor.UpdateData(Data);
		base.OnDataUpdate();
	}

	public override void InitializeInventory()
	{
		useInteractor.UpdateData(Data);
		base.InitializeInventory();
		modelSelector.Close();
	}

	protected override void OnUpdate()
	{
		if (state == GameCoinChestClientState.Opening)
		{
			OpenChest(MVGameControllerBase.WOCM.AvatarLocal.Id);
		}
	}

	public bool IsUsable(MVInteractableBase avatarInteractable)
	{
		if (state != GameCoinChestClientState.Closed)
		{
			return false;
		}
		return true;
	}

	private bool OpenChest(int instigatorID)
	{
		modelSelector.Open();
		if ((bool)aSource)
		{
			aSource.Play();
		}
		particles.Play();
		MVGameControllerBase.Game.GameCoinManager.GameCoinChestCollect((int)Data["gameCoinAmount"]);
		state = GameCoinChestClientState.Open;
		return true;
	}

	private void triggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		if (state == GameCoinChestClientState.Closed && (useInteractor.EvaluateRequirementsUsability() & purchaseOptions) == 0)
		{
			state = GameCoinChestClientState.Opening;
		}
	}

	public override void Reset()
	{
		base.Reset();
		DoOpen();
	}

	private void DoOpen()
	{
		if (state == GameCoinChestClientState.Open || state == GameCoinChestClientState.Opening)
		{
			modelSelector.Close();
			state = GameCoinChestClientState.Closed;
		}
	}

	public override void ChangeLOD(float distance)
	{
		if (disabledByLod && distance < cullDistance)
		{
			disabledByLod = false;
			if (state == GameCoinChestClientState.Closed)
			{
				modelSelector.Close();
			}
			else
			{
				modelSelector.Open();
			}
		}
		else if (!disabledByLod && distance >= cullDistance)
		{
			disabledByLod = true;
			MeshRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<MeshRenderer>();
			MeshRenderer[] array = componentsInChildren;
			foreach (MeshRenderer meshRenderer in array)
			{
				meshRenderer.enabled = false;
			}
		}
	}

	public override MVWorldObjectClient Clone(int ownerActorNumber, int cloneGroupId, CloneBookkeeping cloneBookkeeping, Dictionary<int, MVWorldObjectClient> worldObjects, Dictionary<int, RuntimePrototypeCubeModel> prototypes)
	{
		MVGameControllerBase.Game.GameCoinManager.ReportPickupChangeInEditor();
		return base.Clone(ownerActorNumber, cloneGroupId, cloneBookkeeping, worldObjects, prototypes);
	}

	public override MVWorldObject DeepCopy()
	{
		MVGameControllerBase.Game.GameCoinManager.ReportPickupChangeInEditor();
		return base.DeepCopy();
	}

	public override void Initialize()
	{
		MVGameControllerBase.Game.GameCoinManager.ReportPickupChangeInEditor();
		base.Initialize();
	}

	public override void Destroy()
	{
		MVGameControllerBase.Game.GameCoinManager.ReportPickupChangeInEditor();
		triggerBoxEvents.TriggerEnter -= useInteractor.triggerBoxEvents_TriggerEnter;
		triggerBoxEvents.TriggerExit -= useInteractor.triggerBoxEvents_TriggerExit;
		useInteractor.OnDestroy(Data);
		base.Destroy();
	}

	public override Vector3 GetClosestGridPoint(float gridSize, Vector3 position)
	{
		Vector3 one = Vector3.one;
		one *= 2.6f;
		return SharedCubeFunctions.GetClosestGridPoint(position, gameObject.transform.rotation, gridSize, one);
	}
}
