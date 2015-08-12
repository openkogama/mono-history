using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class MVCubeModelFineGrainedTerrain : MVCubeModelBase
{
	private TerrainLODComponent terrainLODComponent;

	public bool RequiresResetToEdit => prototypeCubeModel.CubeCount > 0;

	public MVCubeModelFineGrainedTerrain(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects, Dictionary<int, RuntimePrototypeCubeModel> prototypes)
		: base(data, worldObjects, prototypes)
	{
		interactionFlags = InteractionFlags.None;
		terrainLODComponent = new TerrainLODComponent(prototypeCubeModel, chunkInstances, new DynamicLODDistance(1f, 300f, 1500), Scale.x, debug: false);
	}

	public void ChangeLODTerrain()
	{
		terrainLODComponent.ChangeLODTerrain();
	}

	public override void Destroy()
	{
		prototypeCubeModel.RemoveInstance(id);
		base.Destroy();
	}

	public override void Select(Color color)
	{
	}

	public override void Reset()
	{
		PrototypeCubeModel.RemoveAllCubesLocal();
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
