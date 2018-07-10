using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SplashController : StreamedAudioClip.IReceiver
{
	private class ObjectData
	{
		public float TimeSinceLastRingEmission { get; set; }

		public Vector3 LastRingPosition { get; set; }

		public int LastFrameInWater { get; set; }

		public bool IsInWater { get; set; }

		public ObjectData()
		{
			TimeSinceLastRingEmission = float.PositiveInfinity;
			IsInWater = false;
		}
	}

	[SerializeField]
	[Header("Rings")]
	private ParticleSystem waterRingParticles;

	[SerializeField]
	private float timeBeforeNewRingIsEmitted = 1.2f;

	[SerializeField]
	private float distanceBeforeNewRingIsEmitted = 1.5f;

	[SerializeField]
	[Header("Splash")]
	private ParticleSystem waterSplashParticles;

	[SerializeField]
	[Tooltip("Actual number is based off avatar speed.")]
	private float baseNumberOfSplashParticles = 1f;

	[SerializeField]
	[Range(0f, 4f)]
	[Tooltip("Actual number is based off avatar speed.")]
	private float baseSplashParticlesSpeed = 0.5f;

	[SerializeField]
	private Color splashTint;

	[Header("Pillar")]
	[SerializeField]
	private ParticleSystem waterPillarParticles;

	[SerializeField]
	[Range(0f, 10f)]
	private float waterPillarDensity = 0.5f;

	[SerializeField]
	private Color pillarTint;

	[Range(0f, 1f)]
	[SerializeField]
	[Header("Sound")]
	private float splashSoundVolume = 0.5f;

	[SerializeField]
	private StreamedAudioClip streamedSplashSound;

	private AudioClip splashSound;

	private static int currentObjectID = 0;

	private static readonly Dictionary<int, ObjectData> objectIDToData = new Dictionary<int, ObjectData>(8);

	public int NewObjectID
	{
		get
		{
			currentObjectID++;
			return currentObjectID;
		}
	}

	public void Initialize()
	{
		streamedSplashSound.Initialize(this);
	}

	public void CleanUpInactiveObjectIDs()
	{
		int[] array = new int[objectIDToData.Keys.Count];
		objectIDToData.Keys.CopyTo(array, 0);
		int[] array2 = array;
		foreach (int key in array2)
		{
			ObjectData objectData = objectIDToData[key];
			if (Time.frameCount - objectData.LastFrameInWater > 1)
			{
				objectIDToData.Remove(key);
			}
		}
	}

	public void WaterSplash(Bounds bounds, Vector3 velocity, int objectID)
	{
		Vector3 vector = BoundsToPosition(bounds);
		float waterLevel = MVGameControllerBase.WaterPlaneManager.WaterLevel;
		if (!(vector.y <= waterLevel))
		{
			return;
		}
		ObjectData objectData = GetObjectData(objectID);
		float y = bounds.size.y;
		if (vector.y + y > waterLevel)
		{
			Vector3 vector2 = new Vector3(vector.x, waterLevel, vector.z);
			if (objectData.TimeSinceLastRingEmission > timeBeforeNewRingIsEmitted || Vector3.Distance(objectData.LastRingPosition, vector2) > distanceBeforeNewRingIsEmitted)
			{
				EmitWaterRing(vector2);
				objectData.TimeSinceLastRingEmission = 0f;
				objectData.LastRingPosition = vector2;
			}
			if (!objectData.IsInWater)
			{
				objectData.IsInWater = true;
				MVGameControllerBase.AudioManager.Play("AvatarWaterSplashSound", splashSound, vector2, CalcSplashSoundVolume(velocity), SoundRangeDistance.Long);
				EmitWaterPillar(vector2, velocity);
				EmitWaterSplash(vector2, velocity);
			}
		}
	}

	private float CalcSplashSoundVolume(Vector3 velocity)
	{
		float num = Mathf.Clamp(0f - velocity.y, 0f, 30f) / 30f;
		return splashSoundVolume * num;
	}

	private static ObjectData GetObjectData(int objectID)
	{
		if (!objectIDToData.ContainsKey(objectID))
		{
			objectIDToData[objectID] = new ObjectData();
		}
		ObjectData objectData = objectIDToData[objectID];
		objectData.LastFrameInWater = Time.frameCount;
		objectData.TimeSinceLastRingEmission += Time.deltaTime;
		return objectData;
	}

	private static Vector3 BoundsToPosition(Bounds b)
	{
		Vector3 center = b.center;
		center.y -= b.extents.y;
		return center;
	}

	private void EmitWaterRing(Vector3 position)
	{
		waterRingParticles.transform.position = position;
		waterRingParticles.Emit(1);
	}

	private void EmitWaterSplash(Vector3 position, Vector3 velocity)
	{
		waterSplashParticles.transform.position = position;
		waterSplashParticles.startSpeed = velocity.magnitude * baseSplashParticlesSpeed;
		Color waterColor = MVGameControllerBase.WaterPlaneManager.WaterColor;
		waterSplashParticles.startColor = (waterColor + splashTint) / 2f;
		waterSplashParticles.Emit((int)(velocity.magnitude * baseNumberOfSplashParticles));
	}

	private void EmitWaterPillar(Vector3 position, Vector3 impactVelocity)
	{
		waterPillarParticles.transform.position = position;
		float magnitude = impactVelocity.magnitude;
		float num = waterPillarDensity * magnitude;
		for (int i = 1; (float)i < num; i++)
		{
			waterPillarParticles.startSpeed = num - (float)i;
			Color waterColor = MVGameControllerBase.WaterPlaneManager.WaterColor;
			waterPillarParticles.startColor = (waterColor + pillarTint) / 2f;
			waterPillarParticles.Emit(1);
		}
	}

	public void OnAudioReceived(AudioClip a)
	{
		splashSound = a;
	}
}
