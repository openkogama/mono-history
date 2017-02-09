using System.Collections.Generic;
using MV.Common;
using UnityEngine;

namespace MV.WorldObject.RuntimeEvents;

public class ExplosionEvent : RuntimeEvent
{
	public struct ExplosionValues
	{
		private readonly float radius;

		private readonly float centerDamage;

		private readonly DamageFallOffType damageFallOffType;

		public float Radius => radius;

		public float CenterDamage => centerDamage;

		public DamageFallOffType DamageFallOffType => damageFallOffType;

		public ExplosionValues(float radius, float centerDamage, DamageFallOffType damageFallOffType)
		{
			this.radius = radius;
			this.centerDamage = centerDamage;
			this.damageFallOffType = damageFallOffType;
		}
	}

	private const float GodzillaPowerS = 4f;

	private const float GodzillaPowerM = 8f;

	private const float GodzillaPowerL = 20f;

	private const float GodzillaPowerXL = 40f;

	private const float GodzillaLaserImpactBaseDamage = 9f;

	private const float GodzillaLaserImpactBaseRadius = 0.375f;

	private static Dictionary<RuntimeEventType, ExplosionValues> explosionValues = new Dictionary<RuntimeEventType, ExplosionValues>
	{
		{
			RuntimeEventType.Bazooka,
			new ExplosionValues(5f, 100f, DamageFallOffType.Linear)
		},
		{
			RuntimeEventType.AvatarImpact25,
			new ExplosionValues(0.5f, 25.1f, DamageFallOffType.Linear)
		},
		{
			RuntimeEventType.AvatarImpact50,
			new ExplosionValues(0.6f, 50.1f, DamageFallOffType.Linear)
		},
		{
			RuntimeEventType.AvatarImpact75,
			new ExplosionValues(0.7f, 75.1f, DamageFallOffType.Linear)
		},
		{
			RuntimeEventType.VehicleImpact25,
			new ExplosionValues(1f, 25.1f, DamageFallOffType.Linear)
		},
		{
			RuntimeEventType.VehicleImpact50,
			new ExplosionValues(1.2f, 50.1f, DamageFallOffType.Linear)
		},
		{
			RuntimeEventType.VehicleImpact75,
			new ExplosionValues(1.4f, 75.1f, DamageFallOffType.Linear)
		},
		{
			RuntimeEventType.GodzillaLaserImpactS,
			new ExplosionValues(1.5f, 36f, DamageFallOffType.Linear)
		},
		{
			RuntimeEventType.GodzillaLaserImpactM,
			new ExplosionValues(3f, 72f, DamageFallOffType.Linear)
		},
		{
			RuntimeEventType.GodzillaLaserImpactL,
			new ExplosionValues(7.5f, 180f, DamageFallOffType.Linear)
		},
		{
			RuntimeEventType.GodzillaLaserImpactXL,
			new ExplosionValues(15f, 360f, DamageFallOffType.Linear)
		},
		{
			RuntimeEventType.SwordTerrainDestroy,
			new ExplosionValues(0.3f, 100f, DamageFallOffType.Linear)
		}
	};

	public ExplosionValues ExplosionValuesStruct => explosionValues[RuntimeEventType];

	public override byte[] Data
	{
		get
		{
			BytePacker bytePacker = new BytePacker();
			bytePacker.Write((byte)RuntimeEventType);
			bytePacker.Write(position.x);
			bytePacker.Write(position.y);
			bytePacker.Write(position.z);
			return bytePacker.ToArray();
		}
	}

	public static ExplosionValues GetExplosionValuesStruct(RuntimeEventType runtimeEventType)
	{
		return explosionValues[runtimeEventType];
	}

	public ExplosionEvent(RuntimeEventType runtimeEventType, byte[] data)
		: this(runtimeEventType, new BytePacker(data))
	{
	}

	public ExplosionEvent(RuntimeEventType runtimeEventType, BytePacker bytePacker)
	{
		RuntimeEventType = runtimeEventType;
		short x = bytePacker.ReadInt16();
		short y = bytePacker.ReadInt16();
		short z = bytePacker.ReadInt16();
		position = new IntVector(x, y, z);
	}

	public ExplosionEvent(RuntimeEventType runtimeEventType, Vector3 worldPosition, Vector3 normal)
	{
		RuntimeEventType = runtimeEventType;
		position = CubeMathFunctions.WorldPosToFineGrainedLocalPos(worldPosition, normal);
	}

	public ExplosionEvent(RuntimeEventType runtimeEventType, Vector3 worldPosition)
	{
		RuntimeEventType = runtimeEventType;
		position = CubeMathFunctions.WorldPosToFineGrainedLocalPos(worldPosition);
	}

	public override string ToString()
	{
		return $"RuntimeEventType: {RuntimeEventType}, Position: {position}";
	}
}
