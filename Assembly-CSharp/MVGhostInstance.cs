using System;
using System.Collections.Generic;
using System.Linq;
using MV.Common;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.Events;

public class MVGhostInstance : MVWorldObjectClient, IUpdatecontrollerSubscriberUpdate, IUpdatecontrollerSubscriberFixedUpdate, IGameStateControllerSubscriber, IUpdatecontrollerSubscriberBase
{
	private enum GameEffect
	{
		DAMAGE_OVER_TIME,
		INSTANT_DEATH
	}

	private enum GhostMode
	{
		MarkerActive,
		InstanceActive
	}

	private SphereVolumeIndicator rangeVis;

	private float distance = 10f;

	private float speed = 5f;

	private Transform moveTarget;

	private Vector3 oscilPos = Vector3.zero;

	private float oscillationPeriod = 1f;

	private float damagePerSecond = 50f;

	private float turnSlerpFactor = 0.04f;

	private GameEffect gameEffect;

	private float patrolSpeed = 1f;

	private SmoothPhysicsMovement smoothPhysicsMovement;

	private GameObject _ghostInstance;

	private GameObject _ghostMarker;

	private Bounds localBounds;

	private CullingSubscriberBase cullingSubscriberBase;

	private bool isLODVisible;

	private GhostMode ghostMode;

	private Vector3 lodSphereOffset = Vector3.up;

	private int playerLayer = LayerMask.NameToLayer("Player");

	private List<MVWorldObjectClient> targetWos = new List<MVWorldObjectClient>();

	public override MVWorldObjectDocumentationType DocumentationType => MVWorldObjectDocumentationType.Ghost;

	public float Distance
	{
		get
		{
			return distance;
		}
		set
		{
			if (distance != value)
			{
				distance = value;
				Data["Distance"] = value;
				MVGameControllerBase.OperationRequests.UpdateWorldObjectDataPartial(Id, "Data\\Distance", value);
				if (MVGameControllerBase.GameMode == MVGameMode.Edit)
				{
					rangeVis.SetRadius(distance);
				}
			}
		}
	}

	public float Speed
	{
		get
		{
			return speed;
		}
		set
		{
			if (speed != value)
			{
				speed = value;
				Data["Speed"] = value;
				MVGameControllerBase.OperationRequests.UpdateWorldObjectDataPartial(Id, "Data\\Speed", value);
			}
		}
	}

	public MVGhostInstance(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVGhostInstancePrefab, worldObjects)
	{
		interactionFlags |= InteractionFlags.Selectable | InteractionFlags.CanClone;
		ReadWOData();
		_ghostInstance = UnityEngine.Object.Instantiate(base.gameObject);
		_ghostInstance.SetLayerRecursively(LayerMask.NameToLayer("Default"));
		UnityEngine.Object.Destroy(_ghostInstance.GetComponent<Collider>());
		_ghostInstance.transform.parent = base.gameObject.transform;
		_ghostInstance.transform.localPosition = Vector3.zero;
		Material ghostMarkerMaterial = PrefabPool.Instance.GhostMarkerMaterial;
		_ghostMarker = base.gameObject.transform.Find("Ghost").gameObject;
		_ghostMarker.GetComponentInChildren<MeshRenderer>().sharedMaterial = ghostMarkerMaterial;
		GameObject gameObject = new GameObject(_ghostInstance.name + " physics");
		gameObject.transform.parent = _ghostInstance.transform.parent;
		gameObject.transform.position = _ghostInstance.transform.position;
		gameObject.transform.rotation = _ghostInstance.transform.rotation;
		moveTarget = gameObject.transform;
		previewLayerMask |= LayerFlags.Logic;
		UnityEngine.Object.Destroy(base.gameObject.GetComponent<ParticleSystem>());
	}

	public override void Initialize()
	{
		base.Initialize();
		if (MVGameControllerBase.GameMode == MVGameMode.Edit)
		{
			rangeVis = UnityEngine.Object.Instantiate(PrefabPool.Instance.RangeVisualizationObject);
			rangeVis.transform.parent = gameObject.transform;
			rangeVis.transform.localPosition = Vector3.zero;
			rangeVis.SetRadius(distance);
		}
		UpdateController.AddFixedUpdateObject(this, UpdatePriority.PRE_UPDATEBUCKET_20, 10);
		UpdateController.AddUpdateObject(this, UpdatePriority.PRE_UPDATEBUCKET_20, 10);
		InitializeCommon();
		SetupCulling();
		smoothPhysicsMovement = _ghostInstance.AddComponent<SmoothPhysicsMovement>();
		smoothPhysicsMovement.Init(moveTarget, cullingSubscriberBase, null);
		MVGameControllerBase.Game.GameStateController.AddUpdateObject(this);
	}

	private void SetupCulling()
	{
		cullingSubscriberBase = new CullingSubscriberBase(3.4f, WorldPosition + lodSphereOffset, OnStateChange);
		cullingSubscriberBase.DistanceBandIndex = 3;
		PositionChanged = (UnityAction<MVWorldObjectClient, PositionChangedEventArgs>)Delegate.Combine(PositionChanged, new UnityAction<MVWorldObjectClient, PositionChangedEventArgs>(OnPositionChanged));
	}

	private void OnPositionChanged(MVWorldObjectClient wo, PositionChangedEventArgs positionChangedEventArgs)
	{
		if (ghostMode == GhostMode.MarkerActive)
		{
			UpdateMarkerPosition(positionChangedEventArgs.NewPos);
		}
	}

	private void UpdateMarkerPosition(Vector3 newPos)
	{
		cullingSubscriberBase.Position = newPos + lodSphereOffset;
	}

	private void OnStateChange(CullingGroupEvent cullingGroupEvent)
	{
		isLODVisible = CullingApiWrapper.Visible(cullingGroupEvent, cullingSubscriberBase.DistanceBandIndex);
		UpGhosts();
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		InitializeCommon();
	}

	public void InitializeCommon()
	{
		MeshRenderer componentInChildren = _ghostInstance.GetComponentInChildren<MeshRenderer>();
		localBounds = ComputeLocalBounds(gameObject.transform.position, new MeshRenderer[1] { componentInChildren });
	}

	private static Bounds ComputeLocalBounds(Vector3 origin, MeshRenderer[] meshRenderers)
	{
		Bounds result = new Bounds(Vector3.zero, Vector3.zero);
		if (meshRenderers.Length > 0)
		{
			Bounds bounds = meshRenderers[0].bounds;
			bounds.center -= origin;
			result = bounds;
			for (int i = 1; i < meshRenderers.Length; i++)
			{
				bounds = meshRenderers[i].bounds;
				bounds.center -= origin;
				result.Encapsulate(bounds);
			}
		}
		else
		{
			Debug.LogWarning("Mesh filters required for correct bounds");
		}
		return result;
	}

	public override Bounds GetLocalBounds(BoundsContext boundsContext)
	{
		return localBounds;
	}

	public override void Select()
	{
		AddSelectionBox();
		Selected = true;
	}

	public override void Select(Color color)
	{
		AddSelectionBox();
		Selected = true;
	}

	public override void DeSelect()
	{
		RemoveSelectionBox();
		Selected = false;
	}

	private void ReadWOData()
	{
		string empty = string.Empty;
		foreach (KeyValuePair<object, object> datum in Data)
		{
			empty = datum.Value.ToString();
			switch (datum.Key.ToString())
			{
			case "Speed":
				speed = Convert.ToSingle(empty);
				break;
			case "Distance":
				distance = Convert.ToSingle(empty);
				break;
			case "GameEffect":
				gameEffect = (GameEffect)Convert.ToInt32(empty);
				break;
			}
		}
	}

	public override void OnDataUpdate()
	{
		base.OnDataUpdate();
		ReadWOData();
	}

	public void GameStateChanged(UpdateCondition condition)
	{
		if (condition == UpdateCondition.EDITOR)
		{
			ghostMode = GhostMode.MarkerActive;
		}
		else
		{
			smoothPhysicsMovement.Reset();
			moveTarget.position = GetTargetPos(patrolling: true);
			_ghostInstance.transform.position = moveTarget.position;
			ghostMode = GhostMode.InstanceActive;
		}
		UpGhosts();
	}

	private void UpGhosts()
	{
		_ghostInstance.SetActive(value: false);
		_ghostMarker.SetActive(value: false);
		if (!isLODVisible)
		{
			return;
		}
		if (ghostMode == GhostMode.InstanceActive)
		{
			if (!_ghostInstance.activeSelf)
			{
				_ghostInstance.SetActive(value: true);
			}
		}
		else if (ghostMode == GhostMode.MarkerActive && !_ghostMarker.activeSelf)
		{
			_ghostMarker.SetActive(value: true);
			UpdateMarkerPosition(WorldPosition);
		}
	}

	public override void Destroy()
	{
		UpdateController.RemoveUpdateObject(this);
		UpdateController.RemoveFixedUpdateObject(this);
		MVGameControllerBase.Game.GameStateController.RemoveObject(this);
		base.Destroy();
		if (cullingSubscriberBase != null)
		{
			cullingSubscriberBase.Destroy();
			cullingSubscriberBase = null;
		}
	}

	public void UpdateControllerUpdate()
	{
		smoothPhysicsMovement.SmoothMove();
		if (ghostMode != GhostMode.InstanceActive)
		{
		}
	}

	public void UpdateControllerFixedUpdate()
	{
		targetWos = MVGameControllerBase.WOCM.GetWorldObjectsByType(WorldObjectType.PlayModeAvatar);
		for (int num = targetWos.Count - 1; num >= 0; num--)
		{
			if (targetWos[num].GameObject.layer != playerLayer || !targetWos[num].GameObject.activeInHierarchy)
			{
				targetWos.RemoveAt(num);
			}
		}
		MVWorldObjectClient mVWorldObjectClient = targetWos.OrderBy((MVWorldObjectClient a) => (a.WorldPosition - WorldPosition).sqrMagnitude).FirstOrDefault();
		if (mVWorldObjectClient != null)
		{
			targetWos.Clear();
			MoveGhost(mVWorldObjectClient);
			InteractionDataHandlerBase interactionDataHandlerBase = mVWorldObjectClient.InteractionDataHandlerBase;
			if (interactionDataHandlerBase != null)
			{
				ApplyGameEffect(mVWorldObjectClient, interactionDataHandlerBase);
			}
			UpdateVisualEffects(IsTouchingAvatar(mVWorldObjectClient));
		}
	}

	private void MoveGhost(MVWorldObjectClient TargetAvatar)
	{
		bool flag = false;
		Vector3 vector = TargetAvatar.WorldPosition + Vector3.up;
		if ((vector - WorldPosition).sqrMagnitude > Distance * Distance)
		{
			vector = GetTargetPos(flag);
			flag = true;
		}
		Vector3 vector2 = vector - GetTacticalPos();
		Vector3 vector3 = vector2.normalized * GetSpeed(flag) * Time.fixedDeltaTime;
		if (flag && (GetTacticalPos() - vector).sqrMagnitude < GetSpeed(flag) * Time.fixedDeltaTime)
		{
			vector3 = Vector3.zero;
		}
		else if ((GetTacticalPos() - (TargetAvatar.WorldPosition + Vector3.up)).sqrMagnitude < GetSpeed(flag) * Time.fixedDeltaTime)
		{
			vector3 = Vector3.zero;
		}
		Vector3 vector4 = oscilPos;
		oscilPos = Vector3.up * Mathf.Sin(Time.realtimeSinceStartup * GetSpeed(flag) / oscillationPeriod);
		vector4 = oscilPos - vector4;
		moveTarget.position += vector3 + vector4;
		Vector3 forward = vector2;
		if (flag)
		{
			forward.y = 0f;
		}
		forward.Normalize();
		if (forward.sqrMagnitude > 0.01f)
		{
			moveTarget.rotation = Quaternion.Slerp(moveTarget.rotation, Quaternion.LookRotation(forward), turnSlerpFactor * GetSpeed(flag));
		}
	}

	private Vector3 GetTargetPos(bool patrolling)
	{
		float time = MVGameControllerBase.WOCM.MoveableController.time;
		return Vector3.up + WorldPosition + Quaternion.AngleAxis(time * 10f * GetSpeed(patrolling) / distance, Vector3.up) * (Vector3.forward * distance * Mathf.Sin(time * GetSpeed(patrolling) / (distance * 10f)));
	}

	private void ApplyGameEffect(MVWorldObjectClient targetAvatar, InteractionDataHandlerBase interactionHandler)
	{
		switch (gameEffect)
		{
		case GameEffect.DAMAGE_OVER_TIME:
			if (IsTouchingAvatar(targetAvatar))
			{
				interactionHandler.HandleInteraction(ProximityDamageAndImpulse.Create(damagePerSecond * Time.fixedDeltaTime, Vector3.zero, PlayerKilledByType.Ghost), interactionIsLocal: true);
			}
			break;
		case GameEffect.INSTANT_DEATH:
			Debug.LogError("GameEffect.INSTANT_DEATH out commented. Is not expected to be used by anything.");
			break;
		}
	}

	private bool IsTouchingAvatar(MVWorldObjectClient TargetAvatar)
	{
		return (GetTacticalPos() - (TargetAvatar.WorldPosition + Vector3.up)).sqrMagnitude < 0.3f;
	}

	private void UpdateVisualEffects(bool touchingAvatar)
	{
	}

	private Vector3 GetTacticalPos()
	{
		return moveTarget.position - oscilPos;
	}

	private float GetSpeed(bool patrolling)
	{
		if (patrolling)
		{
			return patrolSpeed;
		}
		return speed;
	}
}
