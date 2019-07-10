namespace MV.Common;

public enum RuntimeEventType : byte
{
	Undefined = 0,
	FineGrainedSingleCubeAdd = 1,
	FineGrainedSingleCubeRemove = 2,
	Bazooka = 3,
	AvatarImpact25 = 4,
	FineGrainedSingleCubeRemovedAddedFineGrainedCube = 5,
	VehicleImpact25 = 6,
	AvatarImpact50 = 7,
	AvatarImpact75 = 8,
	VehicleImpact50 = 9,
	VehicleImpact75 = 10,
	SwordTerrainDestroy = 15,
	ImpulseGunImpact = 16
}
