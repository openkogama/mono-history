using System.Collections.Generic;
using UnityEngine;

public class Dragon : MonoBehaviour
{
	private struct Hit
	{
		public Vector3 position;

		public Vector3 normal;

		public float distance;
	}

	public enum DragonState
	{
		Alive,
		Dying,
		Dead
	}

	private MVDragon worldObject;

	public List<GameObject> neck;

	public DragonState State;

	public bool isFiring;

	public bool isInventory;

	private float MinDistance = 50f;

	private float MaxDistance = 100f;

	private float Speed = 15f;

	private Vector3 curSpeed = Vector3.zero;

	private float headMovementsOnFire = 0.15f;

	private float fireOnDistance = 15f;

	private float fireOffDistance = 17f;

	private Vector3? searchPoint;

	private Vector3? lastKnownPosition;

	private float timeoutToTurnOffOnLastKnownPosition = 5f;

	private float timeToTurnOffOnLastKnownPosition = -1f;

	private float radiousLastKnownPosition = 10f;

	private float firingProbWithoutTarget = 0.3f;

	private Vector3 p1Ref;

	private Vector3 p2Ref;

	private Vector3 p3Ref;

	private float minHPos;

	private float maxHPos;

	private float HPos;

	private float HPosV;

	public AudioClip dyingDragon;

	public AudioClip fireAudioClip;

	private AudioManager.Sound fireAudio;

	public Dragon()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
	}

	public void Initialize(MVDragon owner)
	{
		InitializeCommon(owner);
	}

	public void InitializeInventory(MVDragon owner)
	{
		InitializeCommon(owner);
		UpdateNeck();
		isInventory = true;
	}

	private void InitializeCommon(MVDragon owner)
	{
		neck = new List<GameObject>();
		worldObject = owner;
		worldObject.PositionChanged += PositionChanged;
		worldObject.DragonHead.PositionChanged += PositionChanged;
		PositionChanged(null, null);
	}

	private void PositionChanged(object sender, PositionChangedEventArgs e)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		p1Ref = worldObject.DragonHead.WorldPosition;
		p2Ref = worldObject.WorldPosition;
		Vector3 val = -worldObject.DragonHead.Transform.forward;
		Vector3 val2 = p1Ref - p2Ref;
		p3Ref = val * val2.magnitude * 1f + p1Ref;
	}

	private void OnDestroy()
	{
		if (worldObject != null)
		{
			Object.Destroy((Object)(object)worldObject.DragonTargetArea);
		}
	}

	public void Update()
	{
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_0528: Unknown result type (might be due to invalid IL or missing references)
		//IL_052e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_055b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0561: Unknown result type (might be due to invalid IL or missing references)
		//IL_0567: Unknown result type (might be due to invalid IL or missing references)
		//IL_056c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_0468: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
		if (worldObject == null)
		{
			return;
		}
		if (isInventory)
		{
			worldObject.DragonHead.GameObject.GetComponent<DragonHead>().StartFiring();
			return;
		}
		if (!worldObject.IsPlayMode)
		{
			UpdateNeck();
			return;
		}
		if (State == DragonState.Alive && !((Component)this).gameObject.GetComponent<NPCHealth>().isAlive)
		{
			StopFiring();
			minHPos = HPos;
			maxHPos = 1.1f;
			((Renderer)worldObject.DragonTargetArea.GetComponentInChildren<MeshRenderer>()).enabled = false;
			State = DragonState.Dying;
			MVGameController.Instance.AudioManager.Play("Killed dragon", dyingDragon, worldObject.DragonHead.WorldPosition, 1f, SoundRangeDistance.Long);
		}
		switch (State)
		{
		case DragonState.Alive:
		{
			Vector3? targetPosition = GetTargetPosition();
			float num = float.PositiveInfinity;
			Vector3 val;
			if (targetPosition.HasValue)
			{
				lastKnownPosition = targetPosition.Value;
				searchPoint = null;
				val = targetPosition.Value - worldObject.DragonTargetArea.transform.position;
				num = val.magnitude;
				if (Speed * Time.deltaTime < num)
				{
					val = Vector3.Normalize(val) * Speed * Time.deltaTime;
				}
				worldObject.DragonTargetArea.transform.position = Vector3.SmoothDamp(worldObject.DragonTargetArea.transform.position, targetPosition.Value, ref curSpeed, 3f, Speed);
				if (num < fireOnDistance)
				{
					StartFiring();
				}
				if (num > fireOffDistance)
				{
					StopFiring();
				}
			}
			else
			{
				if (!searchPoint.HasValue)
				{
					if (lastKnownPosition.HasValue)
					{
						searchPoint = lastKnownPosition.Value + new Vector3(Random.Range(0f - radiousLastKnownPosition, radiousLastKnownPosition), 0f, Random.Range(0f - radiousLastKnownPosition, radiousLastKnownPosition));
						if (Random.value < firingProbWithoutTarget)
						{
							StartFiring();
						}
					}
					else
					{
						float num2 = MaxDistance / 4f;
						searchPoint = new Vector3(Random.Range(0f - num2, num2), 0f, Random.Range(0f - num2, num2));
					}
				}
				val = searchPoint.Value - worldObject.DragonTargetArea.transform.position;
				num = val.magnitude;
				if (Speed * Time.deltaTime < num)
				{
					val = Vector3.Normalize(val) * Speed * Time.deltaTime;
				}
				worldObject.DragonTargetArea.transform.position = Vector3.SmoothDamp(worldObject.DragonTargetArea.transform.position, searchPoint.Value, ref curSpeed, 3f, Speed);
				if (num < fireOnDistance)
				{
					if (timeToTurnOffOnLastKnownPosition < 0f)
					{
						timeToTurnOffOnLastKnownPosition = Time.time + timeoutToTurnOffOnLastKnownPosition;
					}
					else if (timeToTurnOffOnLastKnownPosition < Time.time)
					{
						timeToTurnOffOnLastKnownPosition = -1f;
						StopFiring();
						searchPoint = null;
					}
				}
			}
			Vector3 val2 = -worldObject.DragonHead.Transform.forward;
			Vector3 val3 = p1Ref - p2Ref;
			p3Ref = val2 * val3.magnitude * 1f + p1Ref;
			if (isFiring)
			{
				HPos = Mathf.SmoothDamp(HPos, minHPos, ref HPosV, 0.5f);
				worldObject.DragonHead.GameObject.transform.position = CalcPoint(HPos, p1Ref, p2Ref, p3Ref);
			}
			if (!isFiring)
			{
				HPos = Mathf.SmoothDamp(HPos, maxHPos, ref HPosV, 1.5f);
				worldObject.DragonHead.GameObject.transform.position = CalcPoint(HPos, p1Ref, p2Ref, p3Ref);
			}
			UpdateNeck();
			break;
		}
		case DragonState.Dying:
			HPos = Mathf.SmoothDamp(HPos, maxHPos, ref HPosV, 2.5f);
			worldObject.DragonHead.GameObject.transform.position = CalcPoint(HPos, p1Ref, p2Ref, p3Ref);
			worldObject.DragonTargetArea.transform.position = CalcPoint(HPos - 0.3f, p1Ref, p2Ref, p3Ref);
			if (maxHPos - HPos < 0.001f)
			{
				State = DragonState.Dead;
			}
			UpdateNeck();
			break;
		case DragonState.Dead:
			break;
		}
	}

	private Vector3 CalcPoint(float time, Vector3 p1, Vector3 p2, Vector3 p3)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = p1 + (p3 - p1) * time;
		Vector3 val2 = p3 + (p2 - p3) * time;
		return val + (val2 - val) * time;
	}

	private void StartFiring()
	{
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		if (!isFiring)
		{
			isFiring = true;
			minHPos = 0f - headMovementsOnFire;
			maxHPos = HPos;
			worldObject.DragonHead.GameObject.GetComponent<DragonHead>().StartFiring();
			worldObject.DragonTargetArea.GetComponent<DragonTargetArea>().StartFiring();
			if ((Object)(object)fireAudioClip != (Object)null)
			{
				fireAudio = MVGameController.Instance.AudioManager.Play("dragon fire", fireAudioClip, worldObject.DragonTargetArea.transform.position, 1f, SoundRangeDistance.Short);
				fireAudio.audio.loop = true;
			}
		}
	}

	private void StopFiring()
	{
		if (isFiring)
		{
			isFiring = false;
			minHPos = HPos;
			maxHPos = headMovementsOnFire;
			worldObject.DragonHead.GameObject.GetComponent<DragonHead>().StopFiring();
			worldObject.DragonTargetArea.GetComponent<DragonTargetArea>().StopFiring();
			if ((Object)(object)fireAudioClip != (Object)null && fireAudio != null)
			{
				fireAudio.audio.Stop();
			}
		}
	}

	private Vector3? GetTargetPosition()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<int, MVPlayer>.KeyCollection keys = MVGameController.Instance.Game.Players.Keys;
		float num = MaxDistance;
		Vector3? result = null;
		Vector3 worldPosition = worldObject.WorldPosition;
		Vector3 worldPosition2 = worldObject.DragonHead.WorldPosition;
		foreach (int item in keys)
		{
			if (MVGameController.Instance.Game.Players[item].Avatar == null)
			{
				continue;
			}
			Vector3 worldPosition3 = MVGameController.Instance.Game.Players[item].Avatar.WorldPosition;
			float num2 = Vector3.Distance(worldPosition3, worldPosition);
			if (num2 <= num && num2 > MinDistance)
			{
				Vector3 val = worldPosition3 - worldPosition2;
				Vector3 normalized = val.normalized;
				Ray ray = new Ray(worldPosition2 + normalized * 5f, normalized);
				Hit hit = default;
				if (Raycast(ray, ref hit))
				{
					num = num2;
					result = worldPosition3;
				}
			}
		}
		return result;
	}

	private bool Raycast(Ray ray, ref Hit hit)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		LayerMask val = LayerMask.op_Implicit(1 << LayerMask.NameToLayer("Default"));
		val = LayerMask.op_Implicit(LayerMask.op_Implicit(val) | (1 << (LayerMask.NameToLayer("Player") & 0x1F)));
		if (CollisionDetection.MVHit(ray, out var voxelHit, float.PositiveInfinity, new HashSet<int> { worldObject.Id }, LayerMask.op_Implicit(val)))
		{
			hit.position = voxelHit.point;
			hit.normal = voxelHit.normal;
			hit.distance = Vector3.Distance(ray.origin, hit.position);
			MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(voxelHit.woId);
			return worldObjectClient is MVAvatar;
		}
		return false;
	}

	protected void NeckPop()
	{
		if (neck.Count >= 1)
		{
			GameObject val = neck[neck.Count - 1];
			neck.RemoveAt(neck.Count - 1);
			Object.Destroy((Object)(object)val);
		}
	}

	public void UpdateNeck(bool fullUpdate = false)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Expected Obj, but got Unknown
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Expected Obj, but got Unknown
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		if (worldObject == null || worldObject.DragonHead == null)
		{
			return;
		}
		worldObject.DragonHead.Transform.LookAt(worldObject.DragonTargetArea.transform.position);
		Vector3 worldPosition = worldObject.DragonHead.WorldPosition;
		Vector3 worldPosition2 = worldObject.WorldPosition;
		Vector3 val = -worldObject.DragonHead.Transform.forward;
		Vector3 val2 = worldPosition - worldPosition2;
		Vector3 p = val * val2.magnitude * 1f + worldPosition;
		float num = 0f;
		Vector3 val3 = worldPosition;
		for (int i = 0; i < 20; i++)
		{
			float time = ((float)i + 0.5f) / 20f;
			Vector3 val4 = CalcPoint(time, worldPosition, worldPosition2, p);
			float num2 = num;
			Vector3 val5 = val4 - val3;
			num = num2 + val5.magnitude;
			val3 = val4;
		}
		float num3 = num;
		Vector3 val6 = new Vector3(1f, 1f, 1f);
		float num4 = num3 * val6.magnitude;
		Vector3 localScale = ((Component)this).transform.localScale;
		int num5 = (int)(num4 / (2f * localScale.magnitude));
		if (fullUpdate)
		{
			while (neck.Count > 1)
			{
				NeckPop();
			}
		}
		while (num5 - 1 > neck.Count)
		{
			if (worldObject.DragonNeck != null)
			{
				if (!neck.Contains(worldObject.DragonNeck.GameObject))
				{
					neck.Add(worldObject.DragonNeck.GameObject);
					continue;
				}
				GameObject val7 = (GameObject)Object.Instantiate((Object)(object)worldObject.DragonNeck.GameObject);
				((Object)val7).name = "dragon neck clone";
				((Renderer)val7.GetComponentInChildren<MeshRenderer>()).enabled = true;
				neck.Add(val7);
				val7.transform.parent = worldObject.GameObject.transform;
			}
			else
			{
				GameObject val8 = (GameObject)Object.Instantiate((Object)(object)worldObject.DragonHead.GameObject);
				((Object)val8).name = "dragon clone";
				((Renderer)val8.GetComponentInChildren<MeshRenderer>()).enabled = true;
				neck.Add(val8);
				val8.transform.parent = worldObject.GameObject.transform;
			}
		}
		while (num5 - 1 < neck.Count)
		{
			NeckPop();
		}
		val3 = worldPosition;
		for (int j = 0; j < num5 - 1; j++)
		{
			float time2 = ((float)j + 0.5f) / (float)num5;
			Vector3 val9 = CalcPoint(time2, worldPosition, worldPosition2, p);
			neck[j].transform.position = val9;
			neck[j].transform.LookAt(val3);
			val3 = val9;
		}
	}
}
