namespace MV.Common;

public enum RuntimeEventType : byte
{
	Undefined,
	FineGrainedSingleCubeAdd,
	FineGrainedSingleCubeRemove,
	Bazooka,
	AvatarImpact25,
	FineGrainedSingleCubeRemovedAddedFineGrainedCube,
	VehicleImpact25,
	AvatarImpact50,
	AvatarImpact75,
	VehicleImpact50,
	VehicleImpact75,
	GodzillaLaserImpactS,
	GodzillaLaserImpactM,
	GodzillaLaserImpactL,
	GodzillaLaserImpactXL
}
