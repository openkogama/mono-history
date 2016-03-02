using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class MVSentryGun : MVLogicObject
{
	private static readonly Dictionary<SentryGunBeamType, SentryGunBeam> prefabMap = new Dictionary<SentryGunBeamType, SentryGunBeam>
	{
		{
			SentryGunBeamType.IceBeam,
			PrefabPool.Instance.IceBeamObject
		},
		{
			SentryGunBeamType.FireBeam,
			PrefabPool.Instance.FireBeamObject
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

	private List<int> deleteList = new List<int>(8);

	private float glowFactor = 0.5f;

	private SentryGunBeamType beamType = SentryGunBeamType.IceBeam;

	private AudioSource audioSource;

	private bool wasDead;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => false;

	public override Vector3 InputConnectorOffset => new Vector3(-2f, 0f, 0f);

	public HashSet<int> RaycastIgnoreWorldObjectIds { get; set; }

	public SentryGunBeamType BeamType => beamType;

	public MVSentryGun(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVSentryGunPrefab, worldObjects)
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
		audioSource = gameObject.GetComponent<AudioSource>();
		wasDead = interactable.IsDead();
		UpdateSentryState();
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		InitializeCommon();
		sentryGunScript.glowPlane.gameObject.SetActive(value: false);
	}

	public void InitializeCommon()
	{
		foreach (KeyValuePair<SentryGunBeamType, SentryGunBeam> item in prefabMap)
		{
			prefabs[item.Key] = item.Value;
		}
		if (Data.ContainsKey("beamType"))
		{
			beamType = (SentryGunBeamType)(byte)Data["beamType"];
		}
		sentryGunScript.Initialize();
		sentryGunScript.SetSentryGunBeamType(beamType);
		if (beamType == SentryGunBeamType.FireBeam)
		{
			interactionType = InteractionPackageType.SentryTowerFire;
		}
		if (beamType == SentryGunBeamType.IceBeam)
		{
			interactionType = InteractionPackageType.SentryTowerIce;
		}
	}

	public void ReceiveDamage(float amount, MVPlayer damageDealer, PlayerKilledByType damageType)
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
		sentryGunScript.SetHealth((ObscuredFloat)RunTimeData.GetObscuredType("health"));
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
		base.OnUpdate();
		if (DoRespawn())
		{
			UpdateSentryState();
		}
		if ((InputState || InputLinkRefs.Count == 0) && !interactable.IsDead())
		{
			if (intervalWithRandomSeed.Update() && MVGameControllerBase.Game.IsPlaying)
			{
				HashSet<int> hashSet = new HashSet<int>();
				Collider[] array = Physics.OverlapSphere(gameObject.transform.position, laserRange, 1 << LayerMask.NameToLayer("Player"));
				Collider[] array2 = array;
				foreach (Collider collider in array2)
				{
					MVWorldObjectClient mVObject = MVWorldObjectClientManager.GetMVObject(collider.transform);
					if (mVObject == null)
					{
						continue;
					}
					int num = mVObject.Id;
					InteractionDataHandlerBase interactionDataHandlerBase = mVObject.InteractionDataHandlerBase;
					if (interactionDataHandlerBase == null || !interactionDataHandlerBase.CanHandle(interactionType, interactionIsLocal: true))
					{
						continue;
					}
					mVObject = MVGameControllerBase.WOCM.GetWorldObjectClient(num);
					Vector3 targetPosition = mVObject.GetTargetPosition();
					Ray ray = new Ray(gameObject.transform.position, (targetPosition - gameObject.transform.position).normalized);
					if (HitsTarget(ray, num) && !hashSet.Contains(num))
					{
						if (woIdsBeamsMap.TryGetValue(mVObject.Id, out var value) && value != null)
						{
							value.RefreshTime();
						}
						else
						{
							woIdsBeamsMap.Remove(mVObject.Id);
							value = SentryGunBeam.Create(prefabs[beamType], beamType, this);
							woIdsBeamsMap.Add(mVObject.Id, value);
						}
						ApplyDamage(mVObject, interactionDataHandlerBase);
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
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(item3.Key);
			Collider componentInChildren = worldObjectClient.GameObject.GetComponentInChildren<Collider>();
			item3.Value.SetBeamPositions(gameObject.transform.position, componentInChildren.bounds.center);
		}
		if (woIdsBeamsMap.Count > 0 && !audioSource.isPlaying)
		{
			audioSource.Play();
		}
		if (woIdsBeamsMap.Count == 0 && audioSource.isPlaying)
		{
			audioSource.Stop();
		}
		float b = ((woIdsBeamsMap.Count <= 0) ? 0.5f : 1f);
		if (interactable.IsDead())
		{
			b = 0f;
		}
		glowFactor = Mathf.Lerp(glowFactor, b, Time.deltaTime * 2.5f);
		sentryGunScript.SetGlowFactor(glowFactor);
	}

	private void DoFrameDelete()
	{
		foreach (KeyValuePair<int, SentryGunBeam> item in woIdsBeamsMap)
		{
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(item.Key);
			if (worldObjectClient == null || item.Value == null)
			{
				deleteList.Add(item.Key);
				continue;
			}
			if (worldObjectClient.Collider == null)
			{
				deleteList.Add(item.Key);
				continue;
			}
			Vector3 a = worldObjectClient.Collider.ClosestPointOnBounds(gameObject.transform.position);
			if (Vector3.Distance(a, gameObject.transform.position) > laserRange)
			{
				deleteList.Add(item.Key);
			}
		}
		int count = deleteList.Count;
		for (int i = 0; i < count; i++)
		{
			woIdsBeamsMap.Remove(deleteList[i]);
		}
		deleteList.Clear();
	}

	private bool HitsTarget(Ray ray, int woID)
	{
		if (CollisionDetection.MVHit(ray, out var voxelHit, laserRange, RaycastIgnoreWorldObjectIds))
		{
			return voxelHit.woId == woID;
		}
		return true;
	}

	private void ApplyDamage(MVWorldObjectClient wo, InteractionDataHandlerBase interactionDataHandlerBase)
	{
		InteractionData interaction = BeamTypeToInteractionPackageType(beamType, (wo.GetTargetPosition() - gameObject.transform.position).normalized * pushBackStrength);
		interactionDataHandlerBase.HandleInteraction(interaction, interactionIsLocal: true);
	}

	private static InteractionData BeamTypeToInteractionPackageType(SentryGunBeamType btype, Vector3 impulse)
	{
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
