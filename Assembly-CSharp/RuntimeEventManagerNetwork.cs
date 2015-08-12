using MV.Common;
using MV.WorldObject;
using MV.WorldObject.RuntimeEvents;
using UnityEngine;

public class RuntimeEventManagerNetwork : RuntimeEventManager
{
	public RuntimeEventManagerNetwork(MVCubeModelPrototypeTerrain cubeModelPrototypeTerrain, MVCubeModelFineGrainedTerrain cubeModelFineGrainedTerrain)
	{
		base.cubeModelPrototypeTerrain = cubeModelPrototypeTerrain;
		base.cubeModelFineGrainedTerrain = cubeModelFineGrainedTerrain;
	}

	public void DeserializeRuntimeEvents(BytePacker bytePacker)
	{
		int num = bytePacker.ReadInt32();
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		for (int i = 0; i < num; i++)
		{
			RuntimeEvent runtimeEvent = RuntimeEvent.Create(bytePacker);
			HandleRuntimeEvent(runtimeEvent);
		}
		Debug.Log($"NumRuntimeEvents: {num}, Time: {Time.realtimeSinceStartup - realtimeSinceStartup}");
		doEffects = true;
	}

	public void HandleRuntimeEvent(RuntimeEvent runtimeEvent)
	{
		switch (RuntimeEvent.GetRuntimeEventObjectType(runtimeEvent.RuntimeEventType))
		{
		case RuntimeEventObjectType.SingleCube:
			HandleEvent((SingleCubeFineGrainedEvent)runtimeEvent);
			break;
		case RuntimeEventObjectType.Explosion:
			HandleEvent((ExplosionEvent)runtimeEvent);
			break;
		}
	}
}
