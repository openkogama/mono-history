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

	private const string prefabPath = "Prefabs/GameCoinChestObject";

	private ObjectParticleEmitterScript particles;

	private GameCoinChestModelSelector modelSelector;

	private GameCoinChestClientState state;

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
		: base(data, "Prefabs/GameCoinChestObject", worldObjects)
	{
		particles = gameObject.GetComponent<ObjectParticleEmitterScript>();
		modelSelector = gameObject.GetComponent<GameCoinChestModelSelector>();
		TriggerBoxEvents componentInChildren = gameObject.GetComponentInChildren<TriggerBoxEvents>();
		if (componentInChildren != null)
		{
			componentInChildren.TriggerEnter += triggerBoxEvents_TriggerEnter;
		}
		else
		{
			Debug.LogError("A TriggerBoxEvents object is missing in PickupItem type: " + GetType().Name);
		}
		interactionFlags |= InteractionFlags.HasSettings;
		MVGameControllerBase.Game.GameCoinManager.ReportPickupChangeInEditor();
	}

	public override Vector3 GetClosestGridPoint(float gridSize, Vector3 position)
	{
		Vector3 one = Vector3.one;
		one *= 2f;
		return SharedCubeFunctions.GetClosestGridPoint(position, gameObject.transform.rotation, gridSize, one);
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		modelSelector.Close();
	}

	protected override void OnUpdate()
	{
		if (state == GameCoinChestClientState.Opening)
		{
			modelSelector.Open();
			if ((bool)gameObject.GetComponent<AudioSource>())
			{
				gameObject.GetComponent<AudioSource>().Play();
			}
			particles.Play();
			MVGameControllerBase.Game.GameCoinManager.GameCoinChestCollect((int)Data["gameCoinAmount"]);
			state = GameCoinChestClientState.Open;
		}
	}

	private void triggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		Debug.Log("GameCoinChest trigger-enter");
		if (state == GameCoinChestClientState.Closed)
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
			Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				renderer.enabled = false;
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
		base.Destroy();
	}
}
