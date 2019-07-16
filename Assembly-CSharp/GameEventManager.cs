using System;
using MV.Common;
using MV.WorldObject.MetaData;
using UnityEngine;

public class GameEventManager
{
	private class GameEventSubscribableVariable<T> : SubscribableVariableBase<T>
	{
		public T ValueSet
		{
			set
			{
				base.value = value;
				Notify();
			}
		}

		public GameEventSubscribableVariable(T value)
			: base(value)
		{
		}
	}

	public class GameStateManager
	{
		public Action OnEnableLobbyState;

		private GameEventSubscribableVariable<MVGameStateType> gameStateType = new GameEventSubscribableVariable<MVGameStateType>(MVGameStateType.None);

		public SubscribableVariableBase<MVGameStateType> GameStateType => gameStateType;

		public void NotifyGameStateType(MVGameStateType gameState)
		{
			gameStateType.ValueSet = gameState;
		}
	}

	public class AvatarCommandsPlayModeManager
	{
		public event Action OnKillSelf;

		public event Action OnSetRespawnWhenPossible;

		public event Action OnReadyScreenShot;

		public event Action OnSpawn;

		public event Action OnSetToSpawnPoint;

		public event Action<WinningConditionType> OnWinningConditionIntermediateDebriefing;

		public event Action OnRemoveFromGame;

		public void KillSelf()
		{
			if (OnKillSelf != null)
			{
				OnKillSelf();
			}
		}

		public void Spawn()
		{
			if (OnSpawn != null)
			{
				OnSpawn();
			}
		}

		public void SetToSpawnPoint()
		{
			if (OnSetToSpawnPoint != null)
			{
				OnSetToSpawnPoint();
			}
		}

		public void SetRespawnWhenPossible()
		{
			if (OnSetRespawnWhenPossible != null)
			{
				OnSetRespawnWhenPossible();
			}
		}

		public void SetIntermediateDebriefing(WinningConditionType winningConditionType)
		{
			if (OnWinningConditionIntermediateDebriefing != null)
			{
				OnWinningConditionIntermediateDebriefing(winningConditionType);
			}
		}

		public void RemoveFromGame()
		{
			if (OnRemoveFromGame != null)
			{
				OnRemoveFromGame();
			}
		}

		public void ReadyScreenShot()
		{
			if (OnReadyScreenShot != null)
			{
				OnReadyScreenShot();
			}
		}
	}

	public class AvatarCommandsBuildModeManager
	{
		public class LaserCommandsManager
		{
			public event Action<bool> OnLaserActiveChanged;

			public event Action<byte> OnCubeMaterialChanged;

			public event Action<LaserPointerState> OnChangeState;

			public event Action<Vector3> OnUpdatePosition;

			public event Action<float> OnActivateLaserForDuration;

			public void SetLaserActiveState(bool isActive)
			{
				if (OnLaserActiveChanged != null)
				{
					OnLaserActiveChanged(isActive);
				}
			}

			public void SetCurrentCubeMaterial(byte cubeMaterial)
			{
				if (OnCubeMaterialChanged != null)
				{
					OnCubeMaterialChanged(cubeMaterial);
				}
			}

			public void ChangeState(LaserPointerState newState)
			{
				if (OnChangeState != null)
				{
					OnChangeState(newState);
				}
			}

			public void UpdatePosition(Vector3 to)
			{
				if (OnUpdatePosition != null)
				{
					OnUpdatePosition(to);
				}
			}

			public void ActivateLaserForDuration(float duration)
			{
				if (OnActivateLaserForDuration != null)
				{
					OnActivateLaserForDuration(duration);
				}
			}
		}

		public readonly LaserCommandsManager LaserCommands = new LaserCommandsManager();

		public event Action<EditorEvent, object> OnEnterBuildStateEvent;

		public event Action<EditorEvent, object> OnExitBuildStateEvent;

		public event Action<Vector3, Quaternion> OnSetSpawn;

		public event Action OnSetToEditMode;

		public void SetSpawn(Vector3 position, Quaternion rotation)
		{
			if (OnSetSpawn != null)
			{
				OnSetSpawn(position, rotation);
			}
		}

		public void SetToEditMode()
		{
			if (OnSetToEditMode != null)
			{
				OnSetToEditMode();
			}
		}

		public void EnterBuildStateEvent(EditorEvent editorEvent, object eventData)
		{
			if (OnEnterBuildStateEvent != null)
			{
				OnEnterBuildStateEvent(editorEvent, eventData);
			}
		}

		public void ExitBuildStateEvent(EditorEvent editorEvent, object eventData)
		{
			if (OnExitBuildStateEvent != null)
			{
				OnExitBuildStateEvent(editorEvent, eventData);
			}
		}
	}

	private GameEventSubscribableVariable<int> gameEventSubscribableVariable;

	public readonly AvatarCommandsPlayModeManager AvatarCommandsPlayMode = new AvatarCommandsPlayModeManager();

	public readonly AvatarCommandsBuildModeManager AvatarCommandsBuildMode = new AvatarCommandsBuildModeManager();

	public readonly GameStateManager GameState = new GameStateManager();

	public event Action<FirstTimeEvent> OnFirstTimeEvent;

	public event Action<int> OnXPRewarded;

	public void NotifyFirstTimeEvent(FirstTimeEvent firstTimeEvent)
	{
		if (OnFirstTimeEvent != null)
		{
			OnFirstTimeEvent(firstTimeEvent);
		}
	}

	public void NotifyXPDeltaAmount(int xp)
	{
		if (OnXPRewarded != null)
		{
			OnXPRewarded(xp);
		}
	}
}
