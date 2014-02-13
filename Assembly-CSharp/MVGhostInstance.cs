using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class MVGhostInstance : MVWorldObjectClient, IGameStateControllerSubscriber, IUpdatecontrollerSubscriber
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

	private GameObject _ghostInstance;

	private GameObject _ghostMarker;

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
				Data["Distance"] = value;
				MVGameController.Instance.Game.UpdateWorldObjectDataPartial(Id, "Data\\Distance", value);
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
				Data["Speed"] = value;
				MVGameController.Instance.Game.UpdateWorldObjectDataPartial(Id, "Data\\Speed", value);
			}
		}
	}

	public MVGhostInstance(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/GhostObject", worldObjects)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		interactionFlags |= InteractionFlags.Selectable | InteractionFlags.CanClone;
		ReadWOData();
		Object val = Object.Instantiate((Object)(object)gameObject);
		_ghostInstance = (GameObject)(object)((val is GameObject) ? val : null);
		_ghostInstance.SetLayerRecursively(LayerMask.NameToLayer("Default"));
		Object.Destroy((Object)(object)_ghostInstance.collider);
		Object val2 = Resources.Load("Materials/GhostMaterialMarker", typeof(Material));
		Material sharedMaterial = (Material)(object)((val2 is Material) ? val2 : null);
		_ghostMarker = ((Component)gameObject.transform.FindChild("Ghost")).gameObject;
		((Renderer)_ghostMarker.GetComponentInChildren<MeshRenderer>()).sharedMaterial = sharedMaterial;
		ghostBody = _ghostInstance.transform;
		ghostBody.parent = gameObject.transform;
		ghostBody.localPosition = Vector3.zero;
		previewLayerMask |= LayerFlags.Logic;
	}

	public override void Initialize()
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize();
		rangeVis = Object.Instantiate(Resources.Load("Prefabs/Effects/RangeVisualization", typeof(SphereVolumeIndicator))) as SphereVolumeIndicator;
		((Component)rangeVis).transform.parent = gameObject.transform;
		((Component)rangeVis).transform.localPosition = Vector3.zero;
		rangeVis.Radius = distance;
		MVGameController.Instance.UpdateController.AddFixedUpdateObject(this, UpdatePriority.PRE_UPDATEBUCKET_20, 10);
		InitializeCommon();
		MVGameController.Instance.Game.GameStateController.AddUpdateObject(this);
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		InitializeCommon();
	}

	public void InitializeCommon()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		MeshRenderer componentInChildren = _ghostInstance.GetComponentInChildren<MeshRenderer>();
		localBounds = ComputeLocalBounds(gameObject.transform.position, new MeshRenderer[1] { componentInChildren });
	}

	private static Bounds ComputeLocalBounds(Vector3 origin, MeshRenderer[] meshRenderers)
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
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
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
			Debug.LogWarning((object)"Mesh filters required for correct bounds");
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
		foreach (DictionaryEntry datum in Data)
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
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (condition == UpdateCondition.EDITOR)
		{
			_ghostMarker.SetActiveRecursively(true);
			((Component)_ghostMarker.transform.parent).gameObject.active = true;
			_ghostInstance.SetActiveRecursively(false);
		}
		else
		{
			ghostBody.position = GetTargetPos(patrolling: true);
			((Component)_ghostMarker.transform.parent).gameObject.active = false;
			_ghostMarker.SetActiveRecursively(false);
			_ghostInstance.SetActiveRecursively(true);
		}
	}

	public override void Destroy()
	{
		MVGameController.Instance.UpdateController.RemoveObject(this);
		MVGameController.Instance.Game.GameStateController.RemoveObject(this);
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
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		Vector3 val = TargetAvatar.WorldPosition + Vector3.up;
		Vector3 val2 = val - WorldPosition;
		if (val2.sqrMagnitude > Distance * Distance)
		{
			val = GetTargetPos(flag);
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
				goto IL_00e3;
			}
		}
		Vector3 val6 = GetTacticalPos() - (TargetAvatar.WorldPosition + Vector3.up);
		if (val6.sqrMagnitude < GetSpeed(flag) * Time.fixedDeltaTime)
		{
			val4 = Vector3.zero;
		}
		goto IL_00e3;
		IL_00e3:
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

	private Vector3 GetTargetPos(bool patrolling)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		float time = MVGameController.Instance.WOCM.MoveableController.time;
		return Vector3.up + WorldPosition + Quaternion.AngleAxis(time * 10f * GetSpeed(patrolling) / distance, Vector3.up) * (Vector3.forward * distance * Mathf.Sin(time * GetSpeed(patrolling) / (distance * 10f)));
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
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = GetTacticalPos() - (TargetAvatar.WorldPosition + Vector3.up);
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
