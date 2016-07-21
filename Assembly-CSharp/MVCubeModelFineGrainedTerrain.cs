using System.Collections.Generic;
using MV.WorldObject;

public class MVCubeModelFineGrainedTerrain : MVCubeModelBase
{
	private CullingTerrainManager cullingTerrainManager;

	public bool RequiresResetToEdit => prototypeCubeModel.CubeCount > 0;

	public MVCubeModelFineGrainedTerrain(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects, Dictionary<int, RuntimePrototypeCubeModel> prototypes)
		: base(data, worldObjects, prototypes)
	{
		interactionFlags = InteractionFlags.None;
	}

	public override void Initialize()
	{
		base.Initialize();
		cullingTerrainManager = new CullingTerrainManager(chunkInstances, this);
	}

	public override void Destroy()
	{
		prototypeCubeModel.RemoveInstance(id);
		base.Destroy();
	}

	public override void Reset()
	{
		PrototypeCubeModel.RemoveAllCubesLocal();
		cullingTerrainManager.Clear();
	}

	public override void RemoveCubeNetworkUpdate(IntVector pos)
	{
		prototypeCubeModel.RemoveCubeNetworkUpdate(pos, MeshGeneratePriority.Medium);
	}

	public override void AddCubeNetworkUpdate(IntVector pos, CubeBase cube)
	{
		Cube cube2 = new Cube(cube.ByteCorners, cube.FaceMaterials);
		prototypeCubeModel.AddCubeNetworkUpdate(pos, cube2, MeshGeneratePriority.Medium);
	}
}
