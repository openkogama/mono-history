using MV.Common;

namespace MV.WorldObject.RuntimeEvents;

public abstract class RuntimeEvent
{
	protected IntVector position;

	public RuntimeEventType RuntimeEventType { get; protected set; }

	public abstract byte[] Data { get; }

	public IntVector Position => position;

	public static RuntimeEventObjectType GetRuntimeEventObjectType(RuntimeEventType runtimeEventType)
	{
		switch (runtimeEventType)
		{
		case RuntimeEventType.FineGrainedSingleCubeAdd:
		case RuntimeEventType.FineGrainedSingleCubeRemove:
		case RuntimeEventType.FineGrainedSingleCubeRemovedAddedFineGrainedCube:
			return RuntimeEventObjectType.SingleCube;
		case RuntimeEventType.Bazooka:
		case RuntimeEventType.AvatarImpact25:
		case RuntimeEventType.VehicleImpact25:
		case RuntimeEventType.AvatarImpact50:
		case RuntimeEventType.AvatarImpact75:
		case RuntimeEventType.VehicleImpact50:
		case RuntimeEventType.VehicleImpact75:
		case RuntimeEventType.GodzillaLaserImpactS:
		case RuntimeEventType.GodzillaLaserImpactM:
		case RuntimeEventType.GodzillaLaserImpactL:
		case RuntimeEventType.GodzillaLaserImpactXL:
		case RuntimeEventType.SwordTerrainDestroy:
		case RuntimeEventType.ImpulseGunImpact:
			return RuntimeEventObjectType.Explosion;
		default:
			return RuntimeEventObjectType.Undefined;
		}
	}

	public static RuntimeEvent Create(BytePacker bytePacker)
	{
		RuntimeEventType runtimeEventType = (RuntimeEventType)bytePacker.ReadByte();
		return Create(runtimeEventType, bytePacker);
	}

	public static RuntimeEvent Create(byte[] bytes)
	{
		return Create(new BytePacker(bytes));
	}

	private static RuntimeEvent Create(RuntimeEventType runtimeEventType, BytePacker bytePacker)
	{
		return GetRuntimeEventObjectType(runtimeEventType) switch
		{
			RuntimeEventObjectType.SingleCube => (RuntimeEvent)new SingleCubeFineGrainedEvent(runtimeEventType, bytePacker), 
			RuntimeEventObjectType.Explosion => new ExplosionEvent(runtimeEventType, bytePacker), 
			_ => null, 
		};
	}
}
