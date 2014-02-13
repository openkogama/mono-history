using System;
using System.Collections;
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

	private const string prefabPath = "Prefabs/GhostObject";

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
				MVGameController.Instance.Game.UpdateWorldObjectDataPartial(Id, "BlueprintData\\Distance", value);
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
				MVGameController.Instance.Game.UpdateWorldObjectDataPartial(Id, "BlueprintData\\Speed", value);
			}
		}
	}

	public MVGhost(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/GhostObject", worldObjects)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		interactionFlags |= InteractionFlags.CanClone;
	}

	public override void Initialize()
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		Debug.LogWarning((object)("Ghost " + id + " init"));
		base.Initialize();
		ReadWOData();
		MVGameController.Instance.UpdateController.AddFixedUpdateObject(this, UpdatePriority.PRE_UPDATEBUCKET_20, 10);
		Object val = Object.Instantiate((Object)(object)gameObject);
		GameObject val2 = (GameObject)(object)((val is GameObject) ? val : null);
		val2.layer = LayerMask.NameToLayer("Default");
		ghostBody = val2.transform;
		ghostBody.parent = gameObject.transform;
		ghostBody.localPosition = Vector3.zero;
		rangeVis = Object.Instantiate(Resources.Load("Prefabs/Effects/RangeVisualization", typeof(SphereVolumeIndicator))) as SphereVolumeIndicator;
		((Component)rangeVis).transform.parent = gameObject.transform;
		((Component)rangeVis).transform.localPosition = Vector3.zero;
		rangeVis.Radius = distance;
		MeshRenderer componentInChildren = val2.GetComponentInChildren<MeshRenderer>();
		localBounds = ComputeLocalBounds(gameObject.transform.position, new MeshRenderer[1] { componentInChildren });
	}

	protected Bounds ComputeLocalBounds(Vector3 origin, MeshRenderer[] meshRenderers)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		Bounds result = new Bounds(Vector3.zero, Vector3.zero);
		if (meshRenderers.Length > 0)
		{
			Bounds bounds = ((Renderer)meshRenderers[0]).bounds;
			bounds.center -= origin;
			result = bounds;
			for (int i = 1; i < meshRenderers.Length; i++)
			{
				bounds = ((Renderer)meshRenderers[i]).bounds;
				bounds.center -= origin;
				result.Encapsulate(bounds);
			}
		}
		else
		{
			Debug.LogWarning((object)"Mesh renderers required for correct bounds", (Object)(object)GameObject);
		}
		return result;
	}

	public override Bounds GetLocalBounds(BoundsContext boundsContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
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
		foreach (DictionaryEntry blueprintDatum in blueprintData)
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

	public override void Destroy()
	{
		base.Destroy();
	}

	public void UpdateControllerUpdate()
	{
	}

	public void UpdateControllerFixedUpdate()
	{
		MVWorldObjectClient mVWorldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectsByType(WorldObjectType.Avatar).OrderBy((MVWorldObjectClient a) =>
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = a.WorldPosition - WorldPosition;
			return val.sqrMagnitude;
		}).FirstOrDefault();
		MoveGhost(mVWorldObjectClient);
		InteractionDataHandlerBase component = mVWorldObjectClient.GameObject.GetComponent<InteractionDataHandlerBase>();
		if ((Object)(object)component != (Object)null && mVWorldObjectClient is MVAvatarLocal)
		{
			ApplyGameEffect((MVAvatarLocal)mVWorldObjectClient, component);
		}
		UpdateVisualEffects(IsTouchingAvatar(mVWorldObjectClient));
	}

	private void MoveGhost(MVWorldObjectClient TargetAvatar)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		Vector3 val = TargetAvatar.WorldPosition;
		Vector3 val2 = val - WorldPosition;
		if (val2.sqrMagnitude > Distance * Distance)
		{
			float time = MVGameController.Instance.WOCM.MoveableController.time;
			val = WorldPosition + Quaternion.AngleAxis(time * 10f * GetSpeed(flag) / distance, Vector3.up) * (Vector3.forward * distance * Mathf.Sin(time * GetSpeed(flag) / (distance * 10f)));
			flag = true;
		}
		Vector3 val3 = val - GetTacticalPos();
		Vector3 val4 = val3.normalized * GetSpeed(flag) * Time.fixedDeltaTime;
		if (flag)
		{
			Vector3 val5 = GetTacticalPos() - val;
			if (val5.sqrMagnitude < GetSpeed(flag) * Time.fixedDeltaTime)
			{
				val4 = Vector3.zero;
				goto IL_0140;
			}
		}
		Vector3 val6 = GetTacticalPos() - TargetAvatar.WorldPosition;
		if (val6.sqrMagnitude < GetSpeed(flag) * Time.fixedDeltaTime)
		{
			val4 = Vector3.zero;
		}
		goto IL_0140;
		IL_0140:
		Vector3 val7 = oscilPos;
		oscilPos = Vector3.up * Mathf.Sin(Time.realtimeSinceStartup * GetSpeed(flag) / oscillationPeriod);
		val7 = oscilPos - val7;
		Transform val8 = ghostBody;
		val8.position += val4 + val7;
		Vector3 val9 = val3;
		if (flag)
		{
			val9.y = 0f;
		}
		val9.Normalize();
		if (val9.sqrMagnitude > 0.01f)
		{
			ghostBody.rotation = Quaternion.Slerp(ghostBody.rotation, Quaternion.LookRotation(val9), turnSlerpFactor * GetSpeed(flag));
		}
	}

	private void ApplyGameEffect(MVAvatarLocal TargetAvatar, InteractionDataHandlerBase interactionHandler)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
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
				interactionHandler.HandleInteraction(ProximityDamageAndImpulse.Create(MVGameController.Instance.WOCM.AvatarLocal.Health.Value, Vector3.zero, PlayerKilledByType.Ghost), interactionIsLocal: true);
			}
			break;
		}
	}

	private bool IsTouchingAvatar(MVWorldObjectClient TargetAvatar)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = GetTacticalPos() - TargetAvatar.WorldPosition;
		return val.sqrMagnitude < 0.3f;
	}

	private void UpdateVisualEffects(bool touchingAvatar)
	{
	}

	private Vector3 GetTacticalPos()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
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
