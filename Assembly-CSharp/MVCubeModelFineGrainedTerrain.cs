using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MVCubeModelFineGrainedTerrain : MVCubeModelBase
{
	public MVCubeModelFineGrainedTerrain(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects, Dictionary<int, RuntimePrototypeCubeModel> prototypes)
		: base(data, worldObjects, prototypes)
	{
		interactionFlags = InteractionFlags.None;
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
}
