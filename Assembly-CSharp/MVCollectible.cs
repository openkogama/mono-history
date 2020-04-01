using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class MVCollectible : MVGamePointRewardLogicObject
{
	public enum CollectibleClientState
	{
		Visible,
		PickedUp,
		ReShowing,
		Invisible,
		PickingUp
	}

	private CollectibleClientState state;

	private MVCollectibleObject collectibleObject;

	private bool isVisible = true;

	private float pickedUpStateDuration = 0.8f;

	private float reshowingStateDuration = 0.5f;

	private float pickedUpTime;

	private MVRuntimeDataVariable takenByListRunTimeVariable;

	private List<MVTeam> takenByTeamList;

	private bool initializedInWorld;

	public override MVWorldObjectDocumentationType DocumentationType => MVWorldObjectDocumentationType.Star;

	public override bool HasInputConnector => false;

	public override bool HasOutputConnector => false;

	protected override bool HasVisualsInPlaymode => true;

	public MVCollectible(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVCollectiblePrefab, worldObjects)
	{
		Create();
	}

	private void Create()
	{
		collectibleObject = (MVCollectibleObject)component;
		if (collectibleObject.TriggerBoxEvents != null)
		{
			collectibleObject.TriggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
		}
		else
		{
			Debug.LogError("A TriggerBoxEvents object is missing in PickupItem type: " + GetType().Name);
		}
		if (collectibleObject.AllWorldObjectTriggerBoxEvents != null)
		{
			collectibleObject.AllWorldObjectTriggerBoxEvents.TriggerEnter += allWorldObjectTriggerBoxEvents_TriggerEnter;
		}
		else
		{
			Debug.LogError("A AllWorldObjectTriggerBoxEvents object is missing in PickupItem type: " + GetType().Name);
		}
		SetVisible();
		interactionFlags |= InteractionFlags.CanEarnGamePointsMinor;
	}

	public override void Initialize()
	{
		base.Initialize();
		AllCollectiblesCollectedClient allCollectiblesCollectedClient = MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<AllCollectiblesCollectedClient>();
		if (allCollectiblesCollectedClient == null)
		{
			allCollectiblesCollectedClient = MVGameControllerBase.Game.WinningConditionManager.CreateWinnerCondition<AllCollectiblesCollectedClient>(new object[0]);
		}
		allCollectiblesCollectedClient.SetLimit(allCollectiblesCollectedClient.Limit + 1);
		initializedInWorld = true;
		SetupCulling(collectibleObject.PickupMesh);
		takenByListRunTimeVariable = RuntimeDataVariables.New("takenByList", 1f, writeThrough: false);
		takenByTeamList = new List<MVTeam>();
		if (MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState != MVGameStateType.RoundEnded)
		{
			Dictionary<object, object> dictionary = (Dictionary<object, object>)takenByListRunTimeVariable.Value;
			if (dictionary.ContainsKey(MVTeam.Blue.ToString()))
			{
				takenByTeamList.Add(MVTeam.Blue);
			}
			if (dictionary.ContainsKey(MVTeam.Red.ToString()))
			{
				takenByTeamList.Add(MVTeam.Red);
			}
			if (dictionary.ContainsKey(MVTeam.Green.ToString()))
			{
				takenByTeamList.Add(MVTeam.Green);
			}
			if (dictionary.ContainsKey(MVTeam.Yellow.ToString()))
			{
				takenByTeamList.Add(MVTeam.Yellow);
			}
		}
		OnTakenByListChange();
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnWinningConditionFulfilled = (Action<IWinningCondition>)Delegate.Combine(game.OnWinningConditionFulfilled, new Action<IWinningCondition>(OnWinningConditionFulfilled));
	}

	public override void Destroy()
	{
		base.Destroy();
		if (!initializedInWorld)
		{
			return;
		}
		AllCollectiblesCollectedClient singletonWinnerConditionByType = MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<AllCollectiblesCollectedClient>();
		if (singletonWinnerConditionByType == null)
		{
			throw new Exception("AllCollectiblesCollected not found.");
		}
		if (singletonWinnerConditionByType.Limit == 0)
		{
			throw new Exception("AllCollectiblesCollected limit is 0");
		}
		singletonWinnerConditionByType.SetLimit(singletonWinnerConditionByType.Limit - 1);
		if (MVGameControllerBase.IsAlive && MVGameControllerBase.Game != null)
		{
			if (singletonWinnerConditionByType.Limit == 0)
			{
				MVGameControllerBase.Game.WinningConditionManager.RemoveWinnerCondition(singletonWinnerConditionByType.ID);
			}
			MVNetworkGame game = MVGameControllerBase.Game;
			game.OnWinningConditionFulfilled = (Action<IWinningCondition>)Delegate.Remove(game.OnWinningConditionFulfilled, new Action<IWinningCondition>(OnWinningConditionFulfilled));
		}
	}

	private void SetVisible()
	{
		if (!isVisible)
		{
			collectibleObject.PickupItem.GreyIn();
			isVisible = true;
		}
		state = CollectibleClientState.Visible;
		collectibleObject.CollectibleEffects.SetState(state);
	}

	private void allWorldObjectTriggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		if (collectibleObject.WorldObjectEnableController.EnableState != EnableState.Enable)
		{
			return;
		}
		int num = MVGameControllerBase.WOCM.GetWorldObjectClient(e.instigatorWOID).OwnerActorNr;
		bool flag = MVGameControllerBase.Game.LocalPlayer.ActorNr == num;
		bool flag2 = num <= 0;
		if (takenByTeamList == null || !MVGameControllerBase.Game.MVPlayerContainer.TryGetValue(num, out var player))
		{
			return;
		}
		MVTeam team = player.Team;
		if ((flag && isVisible) || (!flag && !flag2 && !takenByTeamList.Contains(team)))
		{
			if ((bool)collectibleObject.AudioSource)
			{
				collectibleObject.AudioSource.Play();
			}
			collectibleObject.Particles.Play();
			if (MVGameControllerBase.Game.TeamManager.GetTeamList().Count > 1 && flag)
			{
				takenByTeamList.Add(team);
				OnTakenByListChange();
			}
		}
	}

	public virtual void OnPickup(int actorNr)
	{
		MVPlayer player;
		if (actorNr == MVGameControllerBase.Game.LocalPlayer.ActorNr && state == CollectibleClientState.PickingUp)
		{
			isVisible = false;
			collectibleObject.PickupItem.GreyOut();
			state = CollectibleClientState.PickedUp;
			collectibleObject.CollectibleEffects.SetState(state);
			pickedUpTime = Time.realtimeSinceStartup;
		}
		else if (MVGameControllerBase.Game.MVPlayerContainer.TryGetValue(actorNr, out player))
		{
			MVTeam team = player.Team;
			if (!takenByTeamList.Contains(team) && MVGameControllerBase.Game.TeamManager.GetTeamList().Count > 1)
			{
				takenByTeamList.Add(team);
				OnTakenByListChange();
			}
		}
	}

	public override void Reset()
	{
		SetVisible();
		MVTeam team = MVGameControllerBase.Game.LocalPlayer.Team;
		if (MVGameControllerBase.Game.TeamManager.GetTeamList().Count > 1 && takenByTeamList.Contains(team) && isVisible)
		{
			isVisible = false;
			collectibleObject.PickupItem.GreyOut();
		}
	}

	private void OnWinningConditionFulfilled(IWinningCondition winningCondition)
	{
		takenByTeamList.Clear();
		Reset();
	}

	protected override void OnUpdate()
	{
		if (state == CollectibleClientState.PickedUp)
		{
			if (Time.realtimeSinceStartup - pickedUpTime > pickedUpStateDuration)
			{
				state = CollectibleClientState.ReShowing;
				collectibleObject.CollectibleEffects.SetState(state);
			}
		}
		else if (state == CollectibleClientState.ReShowing)
		{
			float num = pickedUpTime + pickedUpStateDuration;
			if (Time.realtimeSinceStartup - num > reshowingStateDuration)
			{
				state = CollectibleClientState.Invisible;
				collectibleObject.CollectibleEffects.SetState(state);
			}
		}
	}

	private void triggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		if (collectibleObject.WorldObjectEnableController.EnableState == EnableState.Enable && state == CollectibleClientState.Visible)
		{
			state = CollectibleClientState.PickingUp;
			MVGameControllerBase.OperationRequests.TriggerBoxEnter(Id, e.instigatorWOID);
		}
	}

	public override Vector3 GetClosestGridPoint(float gridSize, Vector3 position)
	{
		Vector3 one = Vector3.one;
		one *= 2f;
		return SharedCubeFunctions.GetClosestGridPoint(position, gameObject.transform.rotation, gridSize, one);
	}

	private void OnTakenByListChange()
	{
		if (MVGameControllerBase.Game.TeamManager.TeamCount() > 1 && takenByTeamList.Contains(MVGameControllerBase.Game.LocalPlayer.Team))
		{
			isVisible = false;
			collectibleObject.PickupItem.GreyOut();
		}
	}
}
