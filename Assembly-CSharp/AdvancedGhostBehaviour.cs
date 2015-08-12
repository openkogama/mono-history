using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.WorldObject;
using UnityEngine;

public class AdvancedGhostBehaviour : MonoBehaviour
{
	private interface IGhostBehaviourState
	{
		void Enter(AdvancedGhostBehaviour ghostBehaviour);

		Type Update(AdvancedGhostBehaviour ghostBehaviour);

		void Exit(AdvancedGhostBehaviour ghostBehaviour);
	}

	private abstract class GhostBehaviourState : IGhostBehaviourState
	{
		public abstract void Enter(AdvancedGhostBehaviour ghostBehaviour);

		public Type Update(AdvancedGhostBehaviour ghostBehaviour)
		{
			if (ghostBehaviour.isDead())
			{
				return typeof(Die);
			}
			return UpdateState(ghostBehaviour);
		}

		protected abstract Type UpdateState(AdvancedGhostBehaviour ghostBehaviour);

		public abstract void Exit(AdvancedGhostBehaviour ghostBehaviour);
	}

	private class Idle : GhostBehaviourState
	{
		private float idleRotationSpeed = 0.3f;

		public override void Enter(AdvancedGhostBehaviour ghostBehaviour)
		{
			ghostBehaviour.weapon.SetAttackValueFactor(0.3f);
			ghostBehaviour.GhostVisualization.SetRotationSpeed(idleRotationSpeed);
		}

		protected override Type UpdateState(AdvancedGhostBehaviour ghostBehaviour)
		{
			ghostBehaviour.SetDesiredPosition(ghostBehaviour.networkedValues.SyncPosition);
			if (ghostBehaviour.perception.TryGetNewTarget(out var _))
			{
				return typeof(Alert);
			}
			return ghostBehaviour.currentState.GetType();
		}

		public override void Exit(AdvancedGhostBehaviour ghostBehaviour)
		{
		}
	}

	private class Die : IGhostBehaviourState
	{
		private float dieTime = 0.5f;

		private float currentDieTime;

		private float deathRotationSpeed = 0.15f;

		public void Enter(AdvancedGhostBehaviour ghostBehaviour)
		{
			currentDieTime = dieTime;
			ghostBehaviour.GhostVisualization.PlayEffect(AdvancedGhostVisualizaton.Effect.Die, dieTime);
			ghostBehaviour.GhostVisualization.SetRotationSpeed(deathRotationSpeed);
		}

		public Type Update(AdvancedGhostBehaviour ghostBehaviour)
		{
			currentDieTime -= Time.deltaTime;
			if (currentDieTime <= 0f)
			{
				return typeof(Dead);
			}
			return ghostBehaviour.currentState.GetType();
		}

		public void Exit(AdvancedGhostBehaviour ghostBehaviour)
		{
		}
	}

	private class Dead : IGhostBehaviourState
	{
		public void Enter(AdvancedGhostBehaviour ghostBehaviour)
		{
			ghostBehaviour.GhostVisualization.gameObject.SetActive(value: false);
		}

		public Type Update(AdvancedGhostBehaviour ghostBehaviour)
		{
			if (!ghostBehaviour.isDead())
			{
				return typeof(Idle);
			}
			return ghostBehaviour.currentState.GetType();
		}

		public void Exit(AdvancedGhostBehaviour ghostBehaviour)
		{
			ghostBehaviour.respawn = true;
			ghostBehaviour.GhostVisualization.PlayEffect(AdvancedGhostVisualizaton.Effect.Respawn, 2f);
		}
	}

	private class ResetState : IGhostBehaviourState
	{
		public void Enter(AdvancedGhostBehaviour ghostBehaviour)
		{
			Debug.Log("Reset entered");
		}

		public Type Update(AdvancedGhostBehaviour ghostBehaviour)
		{
			return typeof(Idle);
		}

		public void Exit(AdvancedGhostBehaviour ghostBehaviour)
		{
		}
	}

	private class Alert : GhostBehaviourState
	{
		private float alertRotationSpeed = 0.7f;

		private float alertMultiplier = 0.8f;

		public override void Enter(AdvancedGhostBehaviour ghostBehaviour)
		{
			if (!ghostBehaviour.perception.TryGetNewTarget(out var worldObjectClient))
			{
				Debug.LogError("Entering alert without valid attack target");
			}
			ghostBehaviour.weapon.SetAttackValueFactor(0.5f);
			ghostBehaviour.GhostVisualization.SetRotationSpeed(alertRotationSpeed);
			ghostBehaviour.GhostVisualization.ghostEye.UpdateLookAtTarget(worldObjectClient.GetTargetPosition());
		}

		protected override Type UpdateState(AdvancedGhostBehaviour ghostBehaviour)
		{
			if (!ghostBehaviour.perception.TryGetNewTarget(out var worldObjectClient))
			{
				return typeof(Idle);
			}
			ghostBehaviour.SetDesiredPosition(ghostBehaviour.networkedValues.SyncPosition);
			ghostBehaviour.GhostVisualization.ghostEye.UpdateLookAtTarget(worldObjectClient.GetTargetPosition());
			if ((worldObjectClient.GameObject.transform.position - ghostBehaviour.transform.position).magnitude < ghostBehaviour.perceptionRadius * alertMultiplier)
			{
				return typeof(Attack);
			}
			return ghostBehaviour.currentState.GetType();
		}

		public override void Exit(AdvancedGhostBehaviour ghostBehaviour)
		{
			ghostBehaviour.GhostVisualization.ghostEye.ClearLookAtTarget();
		}
	}

	private class Attack : GhostBehaviourState
	{
		private float attackRotationSpeed = 1f;

		public override void Enter(AdvancedGhostBehaviour ghostBehaviour)
		{
			if (!ghostBehaviour.perception.TryGetCurrentTarget(out var worldObjectClient))
			{
				Debug.LogError("Entering attack without valid attack target");
			}
			ghostBehaviour.weapon.SetAttackValueFactor(1f);
			ghostBehaviour.GhostVisualization.SetRotationSpeed(attackRotationSpeed);
			ghostBehaviour.GhostVisualization.ghostEye.UpdateLookAtTarget(worldObjectClient.GetTargetPosition());
		}

		protected override Type UpdateState(AdvancedGhostBehaviour ghostBehaviour)
		{
			if (!ghostBehaviour.perception.TryGetCurrentTarget(out var worldObjectClient) && !ghostBehaviour.perception.TryGetNewTarget(out worldObjectClient))
			{
				return typeof(Idle);
			}
			Vector3 targetPosition = worldObjectClient.GetTargetPosition();
			ghostBehaviour.GhostVisualization.ghostEye.UpdateLookAtTarget(targetPosition);
			ghostBehaviour.SetDesiredPosition(worldObjectClient.GetTargetPosition());
			return ghostBehaviour.currentState.GetType();
		}

		public override void Exit(AdvancedGhostBehaviour ghostBehaviour)
		{
			ghostBehaviour.GhostVisualization.ghostEye.ClearLookAtTarget();
		}
	}

	private class AdvancedGhostPerception
	{
		private AdvancedGhostBehaviour ghostBehaviour;

		private OptimizedPerception perception;

		private int currentWoID = -1;

		private int perceptionIntervalMilliseconds = 1000;

		private DeterministicSyncedInterval syncedInterval;

		public AdvancedGhostPerception(AdvancedGhostBehaviour ghostBehaviour, int woID)
		{
			this.ghostBehaviour = ghostBehaviour;
			perception = new OptimizedPerception();
			syncedInterval = new DeterministicSyncedInterval(woID, perceptionIntervalMilliseconds);
		}

		public void Reset()
		{
			syncedInterval.Update();
		}

		public void Update()
		{
			if (syncedInterval.Update())
			{
				perception.Update(ghostBehaviour.networkedValues.SyncPosition, ghostBehaviour.perceptionRadius);
			}
		}

		public bool TryGetCurrentTarget(out MVWorldObjectClient worldObjectClient)
		{
			worldObjectClient = MVGameController.WOCM.GetWorldObjectClient(currentWoID);
			if (worldObjectClient == null)
			{
				return false;
			}
			if (!IsWithinRoamRadius(worldObjectClient.GetTargetPosition()))
			{
				return false;
			}
			if (!worldObjectClient.GameObject.GetComponent<InteractionDataHandlerBase>().enabled)
			{
				return false;
			}
			return true;
		}

		public bool TryGetNewTarget(out MVWorldObjectClient worldObjectClient)
		{
			List<MVWorldObjectClient> targets = perception.GetTargets();
			if (!TryGetTarget(targets, out worldObjectClient))
			{
				return false;
			}
			currentWoID = worldObjectClient.Id;
			return true;
		}

		private bool TryGetTarget(List<MVWorldObjectClient> targets, out MVWorldObjectClient target)
		{
			MVWorldObjectClient mVWorldObjectClient = null;
			float num = ghostBehaviour.RoamRadius;
			foreach (MVWorldObjectClient target2 in targets)
			{
				if (target2.WorldObjectType == WorldObjectType.AdvancedGhost)
				{
					continue;
				}
				Vector3 targetPosition = target2.GetTargetPosition();
				if (CanSense(targetPosition))
				{
					float num2 = DistanceToTargetPosition(targetPosition);
					if (num2 < num)
					{
						mVWorldObjectClient = target2;
						num = num2;
					}
				}
			}
			target = mVWorldObjectClient;
			return target != null;
		}

		private bool CanSense(Vector3 targetPosition)
		{
			if (IsWithinRoamRadius(targetPosition) && IsWithinPerceptionRadius(targetPosition))
			{
				return true;
			}
			return false;
		}

		private bool IsWithinRoamRadius(Vector3 targetPosition)
		{
			if ((ghostBehaviour.transformParent.position - targetPosition).magnitude > ghostBehaviour.RoamRadius)
			{
				return false;
			}
			return true;
		}

		private bool IsWithinPerceptionRadius(Vector3 targetPosition)
		{
			if ((ghostBehaviour.networkedValues.SyncPosition - targetPosition).magnitude > ghostBehaviour.perceptionRadius)
			{
				return false;
			}
			return true;
		}

		private float DistanceToTargetPosition(Vector3 targetPosition)
		{
			return (ghostBehaviour.networkedValues.SyncPosition - targetPosition).magnitude;
		}
	}

	private class NetworkedValues
	{
		private const float pi2 = (float)Math.PI * 2f;

		private const float idleTargetPosMoveSpeedFactor = 0.2f;

		private Vector3 lookDir;

		private float minLookDeltaOffset = 0.1f;

		private AdvancedGhostBehaviour ghostBehaviour;

		private Vector3 nextPosition;

		private Func<int, float, float, Transform, Vector3> patrolPattern;

		private int prevServertime;

		private bool didMeasure;

		public Vector3 SyncPosition => nextPosition;

		public Vector3 SyncLookDir => lookDir;

		public NetworkedValues(AdvancedGhostBehaviour ghostBehaviour)
		{
			this.ghostBehaviour = ghostBehaviour;
			patrolPattern = EaseInEaseOutBackAndForward;
			Update();
		}

		public void Update()
		{
			nextPosition = patrolPattern(WaitForTicks.GetEnvironmentTick(0), ghostBehaviour.speed, ghostBehaviour.radius, ghostBehaviour.transform.parent);
			GetNextLookAt(nextPosition, WaitForTicks.GetEnvironmentTick(0));
		}

		public Vector3 GetPosition(float delta)
		{
			int deltaMilliseconds = (int)(delta * 1000f);
			return patrolPattern(WaitForTicks.GetEnvironmentTick(deltaMilliseconds), ghostBehaviour.speed, ghostBehaviour.radius, ghostBehaviour.transform.parent);
		}

		private void GetNextLookAt(Vector3 curPosition, int serverTimeInMilliSeconds)
		{
			float num = minLookDeltaOffset * (1f / (float)ghostBehaviour.speed);
			int arg = serverTimeInMilliSeconds + (int)(num * 1000f);
			Vector3 vector = patrolPattern(arg, ghostBehaviour.speed, ghostBehaviour.radius, ghostBehaviour.transform.parent);
			lookDir = (vector - curPosition).normalized;
		}

		private void Test(double serverTimeNormalizedToPeriod)
		{
			if (serverTimeNormalizedToPeriod > 5000.0 && !didMeasure)
			{
				Debug.Log(WaitForTicks.Diff(prevServertime));
				prevServertime = WaitForTicks.GetEnvironmentTick(0);
				didMeasure = true;
			}
			if (serverTimeNormalizedToPeriod < 5000.0)
			{
				didMeasure = false;
			}
		}

		private Vector3 EaseInEaseOutBackAndForward(int serverTimeInMilliSeconds, float speed, float radius, Transform transform)
		{
			speed *= 0.2f;
			float num = radius * ((float)Math.PI * 2f);
			float num2 = speed / num;
			float num3 = 1f / num2;
			int num4 = (int)(1000f * num3);
			int num5 = serverTimeInMilliSeconds % num4;
			float num6 = (float)num5 / (float)num4;
			float serverTimeWithSpeedFactor = (float)Math.PI * 2f * num6;
			Vector3 position = new Vector3(GetX(serverTimeWithSpeedFactor, radius), 0f, 0f);
			return transform.TransformPoint(position);
		}

		private Vector3 Circle(int serverTimeInMilliSeconds, float speed, float radius, Transform transform)
		{
			speed *= 0.2f;
			float num = radius * ((float)Math.PI * 2f);
			float num2 = speed / num;
			float num3 = 1f / num2;
			int num4 = (int)(1000f * num3);
			int num5 = serverTimeInMilliSeconds % num4;
			float num6 = (float)num5 / (float)num4;
			float serverTimeWithSpeedFactor = (float)Math.PI * 2f * num6;
			Vector3 position = new Vector3(GetX(serverTimeWithSpeedFactor, radius), 0f, GetY(serverTimeWithSpeedFactor, radius));
			return transform.TransformPoint(position);
		}

		private float GetX(float serverTimeWithSpeedFactor, float radius)
		{
			return radius * Mathf.Cos(serverTimeWithSpeedFactor);
		}

		private float GetY(float serverTimeWithSpeedFactor, float radius)
		{
			return radius * Mathf.Sin(serverTimeWithSpeedFactor);
		}

		private Vector3 BackAndForward(int serverTimeInMilliSeconds, float speed, float radius, Transform transform)
		{
			float num = radius * 2f;
			float num2 = radius * 2f * 2f;
			float num3 = speed * 0.2f;
			float num4 = num2 / num3;
			int num5 = (int)(num4 * 1000f);
			int num6 = serverTimeInMilliSeconds % num5;
			float num7 = (float)num6 / (float)num5;
			float num8 = num7 * num2;
			Vector3 vector = Vector3.back * radius;
			if (num8 > num)
			{
				num8 %= num;
				vector = -vector;
			}
			Vector3 vector2 = -vector.normalized;
			Debug.DrawLine(transform.TransformPoint(vector), transform.TransformPoint(vector) + vector2, Color.blue, 0.5f);
			Debug.DrawLine(transform.TransformPoint(vector), transform.TransformPoint(vector) + Vector3.right, Color.blue, 0.5f);
			Vector3 position = vector + vector2 * num8;
			return transform.TransformPoint(position);
		}
	}

	private IGhostBehaviourState currentState;

	private AdvancedGhostMotor advancedGhostMotor;

	private NetworkedValues networkedValues;

	private AdvancedGhostPerception perception;

	private Transform transformParent;

	private Vector3 nextPosition;

	private Func<bool> isDead;

	private ObscuredFloat speed = 10f;

	private float radius = 10f;

	private float perceptionRadius = 15f;

	private float minPerceptionRadius = 15f;

	private float speedPerceptionFactor = 0.3f;

	private bool respawn;

	private bool clearEffectsBecauseOfReset;

	private AdvancedGhostBodyRotateWeapon weapon;

	public AdvancedGhostVisualizaton GhostVisualization;

	private float lod;

	private float lodPercentageForRotationUpdate = 0.5f;

	private float RoamRadius => radius + perceptionRadius;

	public float Speed
	{
		set
		{
			speed = value;
			perceptionRadius = minPerceptionRadius + (float)speed * speedPerceptionFactor;
		}
	}

	public float Radius
	{
		set
		{
			radius = value;
		}
	}

	public float LOD
	{
		set
		{
			lod = value;
		}
	}

	public void Init(MVCubeModelBase body, AdvancedGhostMotor advancedGhostMotor, Func<bool> isDead, int woID)
	{
		transformParent = transform.parent;
		perception = new AdvancedGhostPerception(this, woID);
		networkedValues = new NetworkedValues(this);
		nextPosition = networkedValues.SyncPosition;
		transform.position = nextPosition;
		this.advancedGhostMotor = advancedGhostMotor;
		this.isDead = isDead;
		InitBody(body);
		SetInitialState();
	}

	private void InitBody(MVCubeModelBase body)
	{
		body.GameObject.SetLayerRecursively(LayerMask.NameToLayer("Player"));
		GhostVisualization.ghostBody = body.GameObject.AddComponent<GhostBody>();
		weapon = body.GameObject.AddComponent<AdvancedGhostBodyRotateWeapon>();
		weapon.Init(GhostVisualization.weaponHitSound, body);
	}

	public void SetGameMode(bool isPlayMode)
	{
		if (isPlayMode)
		{
			SetInitialState();
			perception.Reset();
		}
	}

	public void Reset()
	{
		respawn = true;
		clearEffectsBecauseOfReset = true;
		SetCurrentState(typeof(Idle));
	}

	public void ReceivedDamage()
	{
		GhostVisualization.ReceivedDamage();
	}

	private void Update()
	{
		if (perception == null)
		{
			Debug.LogWarning("This is neccessary because of image generation");
			return;
		}
		perception.Update();
		networkedValues.Update();
		UpdateBehaviourState();
		advancedGhostMotor.FrameUpdate();
		if (respawn)
		{
			DoRespawn();
		}
	}

	private void FixedUpdate()
	{
		if (advancedGhostMotor == null)
		{
			Debug.LogWarning("This is neccessary because of image generation");
		}
		else
		{
			UpdatePositionAndRotation();
		}
	}

	private void DoRespawn()
	{
		if (clearEffectsBecauseOfReset)
		{
			GhostVisualization.PlayEffect(AdvancedGhostVisualizaton.Effect.None, 0f);
		}
		transform.position = networkedValues.GetPosition(0f - Time.deltaTime);
		nextPosition = networkedValues.SyncPosition;
		advancedGhostMotor.Reset(GetMoveVector(nextPosition));
		respawn = false;
		clearEffectsBecauseOfReset = false;
	}

	private void SetInitialState()
	{
		if (isDead())
		{
			SetCurrentState(typeof(Dead));
		}
		else
		{
			Reset();
		}
	}

	private void UpdatePositionAndRotation()
	{
		Vector3 moveVector = GetMoveVector(nextPosition);
		advancedGhostMotor.MoveDirection = moveVector;
		advancedGhostMotor.FixedUpdateFunction();
		if (lod < lodPercentageForRotationUpdate)
		{
			advancedGhostMotor.FixedUpdateRotation();
		}
	}

	private void UpdateBehaviourState()
	{
		Type type = currentState.Update(this);
		if (currentState.GetType() != type)
		{
			SetCurrentState(type);
		}
	}

	private void SetCurrentState(Type type)
	{
		if (currentState != null)
		{
			currentState.Exit(this);
		}
		IGhostBehaviourState ghostBehaviourState = (IGhostBehaviourState)Activator.CreateInstance(type);
		ghostBehaviourState.Enter(this);
		currentState = ghostBehaviourState;
	}

	private Vector3 GetMoveVector(Vector3 targetPos)
	{
		Debug.DrawLine(targetPos, targetPos + Vector3.up, Color.cyan, 0.3f);
		Debug.DrawLine(targetPos, targetPos + Vector3.right, Color.cyan, 0.3f);
		Debug.DrawLine(transform.position, transform.position + Vector3.up, Color.red, 0.3f);
		Debug.DrawLine(transform.position, transform.position + Vector3.right, Color.red, 0.3f);
		Debug.DrawLine(transform.position, targetPos, Color.yellow, 0.3f);
		Vector3 result = (targetPos - transform.position) / Time.deltaTime;
		float magnitude = result.magnitude;
		if (magnitude > (float)speed)
		{
			result = result.normalized * speed;
		}
		return result;
	}

	private void SetDesiredPosition(Vector3 position)
	{
		nextPosition = position;
	}
}
