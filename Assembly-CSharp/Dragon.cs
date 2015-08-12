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
		p1Ref = worldObject.DragonHead.WorldPosition;
		p2Ref = worldObject.WorldPosition;
		p3Ref = -worldObject.DragonHead.Transform.forward * (p1Ref - p2Ref).magnitude * 1f + p1Ref;
	}

	private void OnDestroy()
	{
		if (worldObject != null)
		{
			Object.Destroy(worldObject.DragonTargetArea);
		}
	}

	public void Update()
	{
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
		if (State == DragonState.Alive && !gameObject.GetComponent<NPCHealth>().isAlive)
		{
			StopFiring();
			minHPos = HPos;
			maxHPos = 1.1f;
			worldObject.DragonTargetArea.GetComponentInChildren<MeshRenderer>().enabled = false;
			State = DragonState.Dying;
			MVGameController.AudioManager.Play("Killed dragon", dyingDragon, worldObject.DragonHead.WorldPosition, 1f, SoundRangeDistance.Long);
		}
		switch (State)
		{
		case DragonState.Alive:
		{
			Vector3? targetPosition = GetTargetPosition();
			float num = float.PositiveInfinity;
			if (targetPosition.HasValue)
			{
				lastKnownPosition = targetPosition.Value;
				searchPoint = null;
				Vector3 value = targetPosition.Value - worldObject.DragonTargetArea.transform.position;
				num = value.magnitude;
				if (Speed * Time.deltaTime < num)
				{
					value = Vector3.Normalize(value) * Speed * Time.deltaTime;
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
				Vector3 value = searchPoint.Value - worldObject.DragonTargetArea.transform.position;
				num = value.magnitude;
				if (Speed * Time.deltaTime < num)
				{
					value = Vector3.Normalize(value) * Speed * Time.deltaTime;
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
			p3Ref = -worldObject.DragonHead.Transform.forward * (p1Ref - p2Ref).magnitude * 1f + p1Ref;
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
		Vector3 vector = p1 + (p3 - p1) * time;
		Vector3 vector2 = p3 + (p2 - p3) * time;
		return vector + (vector2 - vector) * time;
	}

	private void StartFiring()
	{
		if (!isFiring)
		{
			isFiring = true;
			minHPos = 0f - headMovementsOnFire;
			maxHPos = HPos;
			worldObject.DragonHead.GameObject.GetComponent<DragonHead>().StartFiring();
			worldObject.DragonTargetArea.GetComponent<DragonTargetArea>().StartFiring();
			if (fireAudioClip != null)
			{
				fireAudio = MVGameController.AudioManager.Play("dragon fire", fireAudioClip, worldObject.DragonTargetArea.transform.position, 1f, SoundRangeDistance.Short);
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
			if (fireAudioClip != null && fireAudio != null)
			{
				fireAudio.audio.Stop();
			}
		}
	}

	private Vector3? GetTargetPosition()
	{
		Dictionary<int, MVPlayer>.KeyCollection keys = MVGameController.Game.Players.Keys;
		float num = MaxDistance;
		Vector3? result = null;
		Vector3 worldPosition = worldObject.WorldPosition;
		Vector3 worldPosition2 = worldObject.DragonHead.WorldPosition;
		foreach (int item in keys)
		{
			if (MVGameController.Game.Players[item].Avatar == null)
			{
				continue;
			}
			Vector3 worldPosition3 = MVGameController.Game.Players[item].Avatar.WorldPosition;
			float num2 = Vector3.Distance(worldPosition3, worldPosition);
			if (num2 <= num && num2 > MinDistance)
			{
				Vector3 normalized = (worldPosition3 - worldPosition2).normalized;
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
		LayerMask layerMask = 1 << LayerMask.NameToLayer("Default");
		layerMask = (int)layerMask | (1 << (LayerMask.NameToLayer("Player") & 0x1F));
		if (CollisionDetection.MVHit(ray, out var voxelHit, float.PositiveInfinity, new HashSet<int> { worldObject.Id }, layerMask))
		{
			hit.position = voxelHit.point;
			hit.normal = voxelHit.normal;
			hit.distance = Vector3.Distance(ray.origin, hit.position);
			MVWorldObjectClient worldObjectClient = MVGameController.WOCM.GetWorldObjectClient(voxelHit.woId);
			return worldObjectClient is MVAvatar;
		}
		return false;
	}

	protected void NeckPop()
	{
		if (neck.Count >= 1)
		{
			GameObject obj = neck[neck.Count - 1];
			neck.RemoveAt(neck.Count - 1);
			Object.Destroy(obj);
		}
	}

	public void UpdateNeck(bool fullUpdate = false)
	{
		if (worldObject == null || worldObject.DragonHead == null)
		{
			return;
		}
		worldObject.DragonHead.Transform.LookAt(worldObject.DragonTargetArea.transform.position);
		Vector3 worldPosition = worldObject.DragonHead.WorldPosition;
		Vector3 worldPosition2 = worldObject.WorldPosition;
		Vector3 p = -worldObject.DragonHead.Transform.forward * (worldPosition - worldPosition2).magnitude * 1f + worldPosition;
		float num = 0f;
		Vector3 vector = worldPosition;
		for (int i = 0; i < 20; i++)
		{
			float time = ((float)i + 0.5f) / 20f;
			Vector3 vector2 = CalcPoint(time, worldPosition, worldPosition2, p);
			num += (vector2 - vector).magnitude;
			vector = vector2;
		}
		int num2 = (int)(num * new Vector3(1f, 1f, 1f).magnitude / (2f * transform.localScale.magnitude));
		if (fullUpdate)
		{
			while (neck.Count > 1)
			{
				NeckPop();
			}
		}
		while (num2 - 1 > neck.Count)
		{
			if (worldObject.DragonNeck != null)
			{
				if (!neck.Contains(worldObject.DragonNeck.GameObject))
				{
					neck.Add(worldObject.DragonNeck.GameObject);
					continue;
				}
				GameObject gameObject = Object.Instantiate(worldObject.DragonNeck.GameObject);
				gameObject.name = "dragon neck clone";
				gameObject.GetComponentInChildren<MeshRenderer>().enabled = true;
				neck.Add(gameObject);
				gameObject.transform.parent = worldObject.GameObject.transform;
			}
			else
			{
				GameObject gameObject2 = Object.Instantiate(worldObject.DragonHead.GameObject);
				gameObject2.name = "dragon clone";
				gameObject2.GetComponentInChildren<MeshRenderer>().enabled = true;
				neck.Add(gameObject2);
				gameObject2.transform.parent = worldObject.GameObject.transform;
			}
		}
		while (num2 - 1 < neck.Count)
		{
			NeckPop();
		}
		vector = worldPosition;
		for (int j = 0; j < num2 - 1; j++)
		{
			float time2 = ((float)j + 0.5f) / (float)num2;
			Vector3 vector3 = CalcPoint(time2, worldPosition, worldPosition2, p);
			neck[j].transform.position = vector3;
			neck[j].transform.LookAt(vector);
			vector = vector3;
		}
	}
}
