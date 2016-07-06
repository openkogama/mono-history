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

	private MVGameCoinChestObject chestObject;

	public override bool HasInputConnector => false;

	public override bool HasOutputConnector => false;

	public override bool Visible
	{
		get
		{
			return chestObject.ModelSelector.IsVisible();
		}
		set
		{
			if (state == GameCoinChestClientState.Closed)
			{
				chestObject.ModelSelector.Close();
			}
			else
			{
				chestObject.ModelSelector.Open();
			}
		}
	}

	public MVGameCoinChest(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVGameCoinChestPrefab, worldObjects)
	{
		chestObject = (MVGameCoinChestObject)component;
		if (chestObject.TriggerBoxEvents != null)
		{
			chestObject.TriggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
		}
		else
		{
			Debug.LogError("A TriggerBoxEvents object is missing in PickupItem type: " + GetType().Name);
		}
		useInteractor = new UseInteractor(Id, gameObject, reset: false, chestObject.TriggerBoxEvents.Collider, OpenChest, IsUsable);
		LevelBasedUseRequirement useRequirement = new LevelBasedUseRequirement(gameObject, hasUseButtonWhenFree: false);
		useInteractor.AddRequirement(useRequirement);
		chestObject.TriggerBoxEvents.TriggerEnter += useInteractor.triggerBoxEvents_TriggerEnter;
		chestObject.TriggerBoxEvents.TriggerExit += useInteractor.triggerBoxEvents_TriggerExit;
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
		base.InitializeInventory();
		chestObject.ModelSelector.Close();
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
		chestObject.ModelSelector.Open();
		if ((bool)chestObject.AudioSource)
		{
			chestObject.AudioSource.Play();
		}
		chestObject.Particles.Play();
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
			chestObject.ModelSelector.Close();
			state = GameCoinChestClientState.Closed;
		}
	}

	public override void ChangeLOD(float distance)
	{
		if (disabledByLod && distance < cullDistance)
		{
			disabledByLod = false;
			MeshRenderer[] meshRenderers = component.MeshRenderers;
			MeshRenderer[] array = meshRenderers;
			foreach (MeshRenderer meshRenderer in array)
			{
				meshRenderer.enabled = true;
			}
			if (state == GameCoinChestClientState.Closed)
			{
				chestObject.ModelSelector.Close();
			}
			else
			{
				chestObject.ModelSelector.Open();
			}
		}
		else if (!disabledByLod && distance >= cullDistance)
		{
			disabledByLod = true;
			MeshRenderer[] meshRenderers2 = component.MeshRenderers;
			MeshRenderer[] array2 = meshRenderers2;
			foreach (MeshRenderer meshRenderer2 in array2)
			{
				meshRenderer2.enabled = false;
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
		useInteractor.UpdateData(Data);
		MVGameControllerBase.Game.GameCoinManager.ReportPickupChangeInEditor();
		base.Initialize();
	}

	public override void Destroy()
	{
		MVGameControllerBase.Game.GameCoinManager.ReportPickupChangeInEditor();
		chestObject.TriggerBoxEvents.TriggerEnter -= useInteractor.triggerBoxEvents_TriggerEnter;
		chestObject.TriggerBoxEvents.TriggerExit -= useInteractor.triggerBoxEvents_TriggerExit;
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
