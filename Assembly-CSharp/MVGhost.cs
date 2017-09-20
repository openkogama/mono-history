using System;
using System.Collections.Generic;
using System.Linq;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class MVGhost : MVBlueprintBase, IUpdatecontrollerSubscriber
{
	private enum GameEffect
	{
		DAMAGE_OVER_TIME,
		INSTANT_DEATH
	}

	private SphereVolumeIndicator rangeVis;

	private float distance = 10f;

	private float speed = 5f;

	private Transform ghostBody;

	private Vector3 oscilPos = Vector3.zero;

	private float oscillationPeriod = 1f;

	private float damagePerSecond = 50f;

	private float turnSlerpFactor = 0.04f;

	private GameEffect gameEffect;

	private float patrolSpeed = 1f;

	private Bounds localBounds;

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
				blueprintData["Distance"] = value;
				MVGameControllerBase.OperationRequests.UpdateWorldObjectDataPartial(Id, "BlueprintData\\Distance", value);
				rangeVis.Radius = distance;
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
				blueprintData["Speed"] = value;
				MVGameControllerBase.OperationRequests.UpdateWorldObjectDataPartial(Id, "BlueprintData\\Speed", value);
			}
		}
	}

	public MVGhost(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVGhostPrefab, worldObjects)
	{
		interactionFlags |= InteractionFlags.CanClone;
	}

	public override void Initialize()
	{
		Debug.LogWarning("Ghost " + id + " init");
		base.Initialize();
		ReadWOData();
		UpdateController.AddFixedUpdateObject(this, UpdatePriority.PRE_UPDATEBUCKET_20, 10);
		GameObject gameObject = UnityEngine.Object.Instantiate(base.gameObject);
		gameObject.layer = LayerMask.NameToLayer("Default");
		ghostBody = gameObject.transform;
		ghostBody.parent = base.gameObject.transform;
		ghostBody.localPosition = Vector3.zero;
		rangeVis = UnityEngine.Object.Instantiate(PrefabPool.Instance.RangeVisualizationObject);
		rangeVis.transform.parent = base.gameObject.transform;
		rangeVis.transform.localPosition = Vector3.zero;
		rangeVis.Initialize(Id);
		rangeVis.Radius = distance;
		MeshRenderer componentInChildren = gameObject.GetComponentInChildren<MeshRenderer>();
		localBounds = ComputeLocalBounds(base.gameObject.transform.position, new MeshRenderer[1] { componentInChildren });
	}

	protected Bounds ComputeLocalBounds(Vector3 origin, MeshRenderer[] meshRenderers)
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
			Debug.LogWarning("Mesh renderers required for correct bounds", GameObject);
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
		foreach (KeyValuePair<object, object> blueprintDatum in blueprintData)
		{
			empty = blueprintDatum.Value.ToString();
			switch (blueprintDatum.Key.ToString())
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

	public void UpdateControllerUpdate()
	{
	}

	public void UpdateControllerFixedUpdate()
	{
		MVWorldObjectClient mVWorldObjectClient = (from a in MVGameControllerBase.WOCM.GetWorldObjectsByType(WorldObjectType.Avatar)
			orderby (a.WorldPosition - WorldPosition).sqrMagnitude
			select a).FirstOrDefault();
		MoveGhost(mVWorldObjectClient);
		InteractionDataHandlerBase interactionDataHandlerBase = mVWorldObjectClient.InteractionDataHandlerBase;
		if (interactionDataHandlerBase != null && mVWorldObjectClient is MVAvatarLocal)
		{
			ApplyGameEffect((MVAvatarLocal)mVWorldObjectClient, interactionDataHandlerBase);
		}
		UpdateVisualEffects(IsTouchingAvatar(mVWorldObjectClient));
	}

	private void MoveGhost(MVWorldObjectClient TargetAvatar)
	{
		bool flag = false;
		Vector3 vector = TargetAvatar.WorldPosition;
		if ((vector - WorldPosition).sqrMagnitude > Distance * Distance)
		{
			float time = MVGameControllerBase.WOCM.MoveableController.time;
			vector = WorldPosition + Quaternion.AngleAxis(time * 10f * GetSpeed(flag) / distance, Vector3.up) * (Vector3.forward * distance * Mathf.Sin(time * GetSpeed(flag) / (distance * 10f)));
			flag = true;
		}
		Vector3 vector2 = vector - GetTacticalPos();
		Vector3 vector3 = vector2.normalized * GetSpeed(flag) * Time.fixedDeltaTime;
		if (flag && (GetTacticalPos() - vector).sqrMagnitude < GetSpeed(flag) * Time.fixedDeltaTime)
		{
			vector3 = Vector3.zero;
		}
		else if ((GetTacticalPos() - TargetAvatar.WorldPosition).sqrMagnitude < GetSpeed(flag) * Time.fixedDeltaTime)
		{
			vector3 = Vector3.zero;
		}
		Vector3 vector4 = oscilPos;
		oscilPos = Vector3.up * Mathf.Sin(Time.realtimeSinceStartup * GetSpeed(flag) / oscillationPeriod);
		vector4 = oscilPos - vector4;
		ghostBody.position += vector3 + vector4;
		Vector3 forward = vector2;
		if (flag)
		{
			forward.y = 0f;
		}
		forward.Normalize();
		if (forward.sqrMagnitude > 0.01f)
		{
			ghostBody.rotation = Quaternion.Slerp(ghostBody.rotation, Quaternion.LookRotation(forward), turnSlerpFactor * GetSpeed(flag));
		}
	}

	private void ApplyGameEffect(MVAvatarLocal TargetAvatar, InteractionDataHandlerBase interactionHandler)
	{
		switch (gameEffect)
		{
		case GameEffect.DAMAGE_OVER_TIME:
			if (IsTouchingAvatar(TargetAvatar))
			{
				interactionHandler.HandleInteraction(ProximityDamageAndImpulse.Create(damagePerSecond * Time.fixedDeltaTime, Vector3.zero, PlayerKilledByType.Ghost), interactionIsLocal: true);
			}
			break;
		case GameEffect.INSTANT_DEATH:
			if (IsTouchingAvatar(TargetAvatar))
			{
				interactionHandler.HandleInteraction(ProximityDamageAndImpulse.Create(MVGameControllerBase.WOCM.AvatarLocal.Health.Value, Vector3.zero, PlayerKilledByType.Ghost), interactionIsLocal: true);
			}
			break;
		}
	}

	private bool IsTouchingAvatar(MVWorldObjectClient TargetAvatar)
	{
		return (GetTacticalPos() - TargetAvatar.WorldPosition).sqrMagnitude < 0.3f;
	}

	private void UpdateVisualEffects(bool touchingAvatar)
	{
	}

	private Vector3 GetTacticalPos()
	{
		return ghostBody.position - oscilPos;
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
