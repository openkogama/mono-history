using System;
using System.Collections.Generic;
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
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
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
			((Component)ghostBehaviour.GhostVisualization).gameObject.SetActiveRecursively(false);
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
			Debug.Log((object)"Reset entered");
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
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			if (!ghostBehaviour.perception.TryGetNewTarget(out var worldObjectClient))
			{
				Debug.LogError((object)"Entering alert without valid attack target");
			}
			ghostBehaviour.weapon.SetAttackValueFactor(0.5f);
			ghostBehaviour.GhostVisualization.SetRotationSpeed(alertRotationSpeed);
			ghostBehaviour.GhostVisualization.ghostEye.UpdateLookAtTarget(worldObjectClient.GetTargetPosition());
		}

		protected override Type UpdateState(AdvancedGhostBehaviour ghostBehaviour)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			if (!ghostBehaviour.perception.TryGetNewTarget(out var worldObjectClient))
			{
				return typeof(Idle);
			}
			ghostBehaviour.SetDesiredPosition(ghostBehaviour.networkedValues.SyncPosition);
			ghostBehaviour.GhostVisualization.ghostEye.UpdateLookAtTarget(worldObjectClient.GetTargetPosition());
			Vector3 val = worldObjectClient.GameObject.transform.position - ((Component)ghostBehaviour).transform.position;
			if (val.magnitude < ghostBehaviour.perceptionRadius * alertMultiplier)
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
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			if (!ghostBehaviour.perception.TryGetCurrentTarget(out var worldObjectClient))
			{
				Debug.LogError((object)"Entering attack without valid attack target");
			}
			ghostBehaviour.weapon.SetAttackValueFactor(1f);
			ghostBehaviour.GhostVisualization.SetRotationSpeed(attackRotationSpeed);
			ghostBehaviour.GhostVisualization.ghostEye.UpdateLookAtTarget(worldObjectClient.GetTargetPosition());
		}

		protected override Type UpdateState(AdvancedGhostBehaviour ghostBehaviour)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
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
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			if (syncedInterval.Update())
			{
				perception.Update(ghostBehaviour.networkedValues.SyncPosition, ghostBehaviour.perceptionRadius);
			}
		}

		public bool TryGetCurrentTarget(out MVWorldObjectClient worldObjectClient)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(currentWoID);
			if (worldObjectClient == null)
			{
				return false;
			}
			if (!IsWithinRoamRadius(worldObjectClient.GetTargetPosition()))
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
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
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
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			if (IsWithinRoamRadius(targetPosition) && IsWithinPerceptionRadius(targetPosition))
			{
				return true;
			}
			return false;
		}

		private bool IsWithinRoamRadius(Vector3 targetPosition)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = ghostBehaviour.transformParent.position - targetPosition;
			if (val.magnitude > ghostBehaviour.RoamRadius)
			{
				return false;
			}
			return true;
		}

		private bool IsWithinPerceptionRadius(Vector3 targetPosition)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = ghostBehaviour.networkedValues.SyncPosition - targetPosition;
			if (val.magnitude > ghostBehaviour.perceptionRadius)
			{
				return false;
			}
			return true;
		}

		private float DistanceToTargetPosition(Vector3 targetPosition)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = ghostBehaviour.networkedValues.SyncPosition - targetPosition;
			return val.magnitude;
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

		public Vector3 SyncPosition
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return nextPosition;
			}
		}

		public Vector3 SyncLookDir
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return lookDir;
			}
		}

		public NetworkedValues(AdvancedGhostBehaviour ghostBehaviour)
		{
			this.ghostBehaviour = ghostBehaviour;
			patrolPattern = EaseInEaseOutBackAndForward;
			Update();
		}

		public void Update()
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			nextPosition = patrolPattern(WaitForTicks.GetEnvironmentTick(0), ghostBehaviour.speed, ghostBehaviour.radius, ((Component)ghostBehaviour).transform.parent);
			GetNextLookAt(nextPosition, WaitForTicks.GetEnvironmentTick(0));
		}

		public Vector3 GetPosition(float delta)
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			int deltaMilliseconds = (int)(delta * 1000f);
			return patrolPattern(WaitForTicks.GetEnvironmentTick(deltaMilliseconds), ghostBehaviour.speed, ghostBehaviour.radius, ((Component)ghostBehaviour).transform.parent);
		}

		private void GetNextLookAt(Vector3 curPosition, int serverTimeInMilliSeconds)
		{
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			float num = minLookDeltaOffset * (1f / ghostBehaviour.speed);
			int arg = serverTimeInMilliSeconds + (int)(num * 1000f);
			Vector3 val = patrolPattern(arg, ghostBehaviour.speed, ghostBehaviour.radius, ((Component)ghostBehaviour).transform.parent);
			Vector3 val2 = val - curPosition;
			lookDir = val2.normalized;
		}

		private void Test(double serverTimeNormalizedToPeriod)
		{
			if (serverTimeNormalizedToPeriod > 5000.0 && !didMeasure)
			{
				Debug.Log((object)WaitForTicks.Diff(prevServertime));
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
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			speed *= 0.2f;
			float num = radius * ((float)Math.PI * 2f);
			float num2 = speed / num;
			float num3 = 1f / num2;
			int num4 = (int)(1000f * num3);
			int num5 = serverTimeInMilliSeconds % num4;
			float num6 = (float)num5 / (float)num4;
			float serverTimeWithSpeedFactor = (float)Math.PI * 2f * num6;
			Vector3 val = new Vector3(GetX(serverTimeWithSpeedFactor, radius), 0f, 0f);
			return transform.TransformPoint(val);
		}

		private Vector3 Circle(int serverTimeInMilliSeconds, float speed, float radius, Transform transform)
		{
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			speed *= 0.2f;
			float num = radius * ((float)Math.PI * 2f);
			float num2 = speed / num;
			float num3 = 1f / num2;
			int num4 = (int)(1000f * num3);
			int num5 = serverTimeInMilliSeconds % num4;
			float num6 = (float)num5 / (float)num4;
			float serverTimeWithSpeedFactor = (float)Math.PI * 2f * num6;
			Vector3 val = new Vector3(GetX(serverTimeWithSpeedFactor, radius), 0f, GetY(serverTimeWithSpeedFactor, radius));
			return transform.TransformPoint(val);
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
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			float num = radius * 2f;
			float num2 = radius * 2f * 2f;
			float num3 = speed * 0.2f;
			float num4 = num2 / num3;
			int num5 = (int)(num4 * 1000f);
			int num6 = serverTimeInMilliSeconds % num5;
			float num7 = (float)num6 / (float)num5;
			float num8 = num7 * num2;
			Vector3 val = Vector3.back * radius;
			if (num8 > num)
			{
				num8 %= num;
				val = -val;
			}
			Vector3 val2 = -val.normalized;
			Debug.DrawLine(transform.TransformPoint(val), transform.TransformPoint(val) + val2, Color.blue, 0.5f);
			Debug.DrawLine(transform.TransformPoint(val), transform.TransformPoint(val) + Vector3.right, Color.blue, 0.5f);
			Vector3 val3 = val + val2 * num8;
			return transform.TransformPoint(val3);
		}
	}

	private IGhostBehaviourState currentState;

	private AdvancedGhostMotor advancedGhostMotor;

	private NetworkedValues networkedValues;

	private AdvancedGhostPerception perception;

	private Transform transformParent;

	private Vector3 prevLocalPosition;

	private Vector3 nextPosition;

	private float minDeltaPos = 0.01f;

	private Func<bool> isDead;

	private float speed = 10f;

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
			perceptionRadius = minPerceptionRadius + speed * speedPerceptionFactor;
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
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		transformParent = ((Component)this).transform.parent;
		perception = new AdvancedGhostPerception(this, woID);
		networkedValues = new NetworkedValues(this);
		nextPosition = networkedValues.SyncPosition;
		((Component)this).transform.position = nextPosition;
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
			Debug.LogWarning((object)"This is neccessary because of image generation");
			return;
		}
		perception.Update();
		networkedValues.Update();
		UpdateBehaviourState();
		if (respawn)
		{
			DoRespawn();
		}
	}

	private void FixedUpdate()
	{
		if ((Object)(object)advancedGhostMotor == (Object)null)
		{
			Debug.LogWarning((object)"This is neccessary because of image generation");
		}
		else
		{
			UpdatePositionAndRotation();
		}
	}

	private void DoRespawn()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if (clearEffectsBecauseOfReset)
		{
			GhostVisualization.PlayEffect(AdvancedGhostVisualizaton.Effect.None, 0f);
		}
		((Component)this).transform.position = networkedValues.GetPosition(0f - Time.deltaTime);
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
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Vector3 moveVector = GetMoveVector(nextPosition);
		advancedGhostMotor.MoveDirection = moveVector;
		advancedGhostMotor.UpdateFunction();
		if (lod < lodPercentageForRotationUpdate)
		{
			UpdateRotation();
		}
	}

	private void UpdateRotation()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		Vector3 localPosition = ((Component)this).transform.localPosition;
		Vector3 val = localPosition - prevLocalPosition;
		if (val.sqrMagnitude > minDeltaPos * minDeltaPos)
		{
			Vector3 val2 = localPosition - prevLocalPosition;
			Vector3 normalized = val2.normalized;
			prevLocalPosition = localPosition;
			if (Mathf.Abs(normalized.y) < 0.5f)
			{
				((Component)this).transform.localRotation = Quaternion.Slerp(((Component)this).transform.localRotation, Quaternion.LookRotation(normalized), 0.1f);
			}
		}
	}

	private void UpdateBehaviourState()
	{
		Type type = currentState.Update(this);
		if ((object)currentState.GetType() != type)
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
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		Debug.DrawLine(targetPos, targetPos + Vector3.up, Color.cyan, 0.3f);
		Debug.DrawLine(targetPos, targetPos + Vector3.right, Color.cyan, 0.3f);
		Debug.DrawLine(((Component)this).transform.position, ((Component)this).transform.position + Vector3.up, Color.red, 0.3f);
		Debug.DrawLine(((Component)this).transform.position, ((Component)this).transform.position + Vector3.right, Color.red, 0.3f);
		Debug.DrawLine(((Component)this).transform.position, targetPos, Color.yellow, 0.3f);
		Vector3 result = (targetPos - ((Component)this).transform.position) / Time.deltaTime;
		float magnitude = result.magnitude;
		if (magnitude > speed)
		{
			result = result.normalized * speed;
		}
		return result;
	}

	private void SetDesiredPosition(Vector3 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		nextPosition = position;
	}
}
