using System.Collections;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class MVSentryGun : MVLogicObject
{
	private const string prefabPath = "Prefabs/Logic/SentryGunObject";

	private static readonly Dictionary<SentryGunBeamType, string> prefabMap = new Dictionary<SentryGunBeamType, string>
	{
		{
			SentryGunBeamType.IceBeam,
			"Prefabs/LaserBeam"
		},
		{
			SentryGunBeamType.FireBeam,
			"Prefabs/SentryGunFireBeam"
		}
	};

	private float laserRange = 20f;

	private float pushBackStrength = 5f;

	private ClientSideNPCInteractable interactable;

	private InteractionPackageType interactionType;

	private SentryGunScript sentryGunScript;

	private IntervalWithRandomSeed intervalWithRandomSeed = new IntervalWithRandomSeed(1f);

	private Dictionary<SentryGunBeamType, SentryGunBeam> prefabs = new Dictionary<SentryGunBeamType, SentryGunBeam>();

	private Dictionary<int, SentryGunBeam> woIdsBeamsMap = new Dictionary<int, SentryGunBeam>();

	private float glowFactor = 0.5f;

	private SentryGunBeamType beamType = SentryGunBeamType.IceBeam;

	private bool wasDead;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => false;

	public override Vector3 InputConnectorOffset
	{
		get
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			return new Vector3(-2f, 0f, 0f);
		}
	}

	public HashSet<int> RaycastIgnoreWorldObjectIds { get; set; }

	public SentryGunBeamType BeamType => beamType;

	public MVSentryGun(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/Logic/SentryGunObject", worldObjects)
	{
		sentryGunScript = gameObject.GetComponentInChildren<SentryGunScript>();
		sentryGunScript.SetLaserRange(laserRange);
		RaycastIgnoreWorldObjectIds = new HashSet<int> { id };
		PlayInteractionType = PlayInteractionType.HandlesHits;
	}

	public override void Initialize()
	{
		base.Initialize();
		interactable = GameObject.AddComponent<ClientSideNPCInteractable>();
		interactable.Init(ReceiveDamage);
		InitializeCommon();
		gameObject.AddComponent<ClientSideNPCInteractionHandler>();
		wasDead = interactable.IsDead();
		UpdateSentryState();
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		InitializeCommon();
	}

	public void InitializeCommon()
	{
		foreach (KeyValuePair<SentryGunBeamType, string> item in prefabMap)
		{
			prefabs[item.Key] = Resources.Load(item.Value, typeof(SentryGunBeam)) as SentryGunBeam;
		}
		if (Data.ContainsKey("beamType"))
		{
			beamType = (SentryGunBeamType)(byte)Data["beamType"];
		}
		sentryGunScript.SetSentryGunBeamType(beamType);
		sentryGunScript.Initialize();
		if (beamType == SentryGunBeamType.FireBeam)
		{
			interactionType = InteractionPackageType.SentryTowerFire;
		}
		if (beamType == SentryGunBeamType.IceBeam)
		{
			interactionType = InteractionPackageType.SentryTowerIce;
		}
	}

	public void ReceiveDamage(float damage)
	{
		if (interactable.IsDead())
		{
			sentryGunScript.Explode();
			sentryGunScript.SmokeEnabled = true;
			wasDead = true;
		}
		UpdateSentryState();
	}

	private void UpdateSentryState()
	{
		sentryGunScript.BlinkDamage();
		if (interactable.IsDead())
		{
			sentryGunScript.SetHealth(0f);
			return;
		}
		sentryGunScript.SmokeEnabled = false;
		sentryGunScript.SetHealth((float)RunTimeData["health"]);
	}

	public override void Select(Color color)
	{
		Selected = true;
	}

	public override void DeSelect()
	{
		Selected = false;
	}

	public override void Reset()
	{
		wasDead = false;
		interactable.Reset();
		UpdateSentryState();
	}

	private bool DoRespawn()
	{
		if (wasDead && !interactable.IsDead())
		{
			wasDead = false;
			return true;
		}
		return false;
	}

	protected override void OnUpdate()
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		base.OnUpdate();
		if (DoRespawn())
		{
			UpdateSentryState();
		}
		if ((InputState || InputLinkRefs.Count == 0) && !interactable.IsDead())
		{
			if (intervalWithRandomSeed.Update() && MVGameController.Instance.Game.IsPlaying)
			{
				HashSet<int> hashSet = new HashSet<int>();
				Collider[] array = Physics.OverlapSphere(gameObject.transform.position, laserRange, 1 << LayerMask.NameToLayer("Player"));
				Collider[] array2 = array;
				foreach (Collider val in array2)
				{
					MVWorldObjectClient mVObject = MVWorldObjectClientManager.GetMVObject(((Component)val).transform);
					if (mVObject == null)
					{
						continue;
					}
					int num = mVObject.Id;
					InteractionDataHandlerBase component = mVObject.GameObject.GetComponent<InteractionDataHandlerBase>();
					if ((Object)(object)component == (Object)null || !component.CanHandle(interactionType, interactionIsLocal: true))
					{
						continue;
					}
					mVObject = MVGameController.Instance.WOCM.GetWorldObjectClient(num);
					Vector3 targetPosition = mVObject.GetTargetPosition();
					Vector3 val2 = gameObject.transform.position;
					Vector3 val3 = targetPosition - gameObject.transform.position;
					Ray ray = new Ray(val2, val3.normalized);
					if (HitsTarget(ray, num) && !hashSet.Contains(num))
					{
						if (woIdsBeamsMap.TryGetValue(mVObject.Id, out var value) && (Object)(object)value != (Object)null)
						{
							value.RefreshTime();
						}
						else
						{
							woIdsBeamsMap.Remove(mVObject.Id);
							value = SentryGunBeam.Create(prefabs[beamType], beamType, this);
							woIdsBeamsMap.Add(mVObject.Id, value);
						}
						ApplyDamage(mVObject, component);
						hashSet.Add(mVObject.Id);
					}
				}
				List<int> list = new List<int>();
				foreach (KeyValuePair<int, SentryGunBeam> item in woIdsBeamsMap)
				{
					if (!hashSet.Contains(item.Key))
					{
						list.Add(item.Key);
					}
				}
				foreach (int item2 in list)
				{
					woIdsBeamsMap.Remove(item2);
				}
			}
			sentryGunScript.UpdateAnimation();
		}
		DoFrameDelete();
		foreach (KeyValuePair<int, SentryGunBeam> item3 in woIdsBeamsMap)
		{
			MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(item3.Key);
			Collider componentInChildren = worldObjectClient.GameObject.GetComponentInChildren<Collider>();
			SentryGunBeam value2 = item3.Value;
			Vector3 start = gameObject.transform.position;
			Bounds bounds = componentInChildren.bounds;
			value2.SetBeamPositions(start, bounds.center);
		}
		if (woIdsBeamsMap.Count > 0 && !gameObject.audio.isPlaying)
		{
			gameObject.audio.Play();
		}
		if (woIdsBeamsMap.Count == 0 && gameObject.audio.isPlaying)
		{
			gameObject.audio.Stop();
		}
		float num2 = ((woIdsBeamsMap.Count <= 0) ? 0.5f : 1f);
		if (interactable.IsDead())
		{
			num2 = 0f;
		}
		glowFactor = Mathf.Lerp(glowFactor, num2, Time.deltaTime * 2.5f);
		sentryGunScript.SetGlowFactor(glowFactor);
	}

	private void DoFrameDelete()
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, SentryGunBeam> item in woIdsBeamsMap)
		{
			MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(item.Key);
			if (worldObjectClient == null || (Object)(object)item.Value == (Object)null)
			{
				list.Add(item.Key);
				continue;
			}
			Vector3 val = worldObjectClient.GameObject.GetComponentInChildren<Collider>().ClosestPointOnBounds(gameObject.transform.position);
			if (Vector3.Distance(val, gameObject.transform.position) > laserRange)
			{
				list.Add(item.Key);
			}
		}
		foreach (int item2 in list)
		{
			woIdsBeamsMap.Remove(item2);
		}
	}

	private bool HitsTarget(Ray ray, int woID)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (CollisionDetection.MVHit(ray, out var voxelHit, laserRange, RaycastIgnoreWorldObjectIds))
		{
			return voxelHit.woId == woID;
		}
		return true;
	}

	private void ApplyDamage(MVWorldObjectClient wo, InteractionDataHandlerBase interactionDataHandlerBase)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		SentryGunBeamType btype = beamType;
		Vector3 val = wo.GetTargetPosition() - gameObject.transform.position;
		InteractionData interaction = BeamTypeToInteractionPackageType(btype, val.normalized * pushBackStrength);
		interactionDataHandlerBase.HandleInteraction(interaction, interactionIsLocal: true);
	}

	private static InteractionData BeamTypeToInteractionPackageType(SentryGunBeamType btype, Vector3 impulse)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		return btype switch
		{
			SentryGunBeamType.FireBeam => SentryTowerFirePackage.Create(impulse), 
			SentryGunBeamType.IceBeam => SentryTowerIcePackage.Create(impulse), 
			_ => SentryTowerFirePackage.Create(impulse), 
		};
	}

	public override bool CompareWithKoGaMaPackage(MVWorldObjectClient wo, KoGaMaPackageClient koGaMaPackageClient, ref int insertedBy)
	{
		MVSentryGun mVSentryGun = wo as MVSentryGun;
		SentryGunBeamType sentryGunBeamType = (SentryGunBeamType)(byte)Data["beamType"];
		SentryGunBeamType sentryGunBeamType2 = (SentryGunBeamType)(byte)mVSentryGun.Data["beamType"];
		return sentryGunBeamType2 == sentryGunBeamType;
	}
}
