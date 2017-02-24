using System.Collections.Generic;
using MV.Common;
using MV.WorldObject.RuntimeEvents;
using UnityEngine;

public class MVExplosives(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects) : MVLogicObject(data, PrefabPool.Instance.MVExplosivesPrefab, worldObjects), ILogicWorldObject
{
	private float damageRadius = 10f;

	private float damageValue = 150f;

	private float shockwaveAcceleration = 3500f;

	private GameObject audioGO;

	private AudioLogicCube audioLC;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => false;

	public IInputSignalReceiver InputSignalReceiver { get; private set; }

	public override void Initialize()
	{
		base.Initialize();
		SetupCulling(((MVExplosivesObject)component).VisualObject);
		InputSignalReceiver = LogicClientsideFactory.CreateStateChangeInputSignalReceiver(this, defaultInput: false, null, InputStateUpdateCallback);
	}

	private void InputStateUpdateCallback(LogicInputState logicInputState, LogicObjectManager logicObjectManager)
	{
		if (logicInputState == LogicInputState.FromColdToHot)
		{
			Explode();
		}
	}

	public void Explode()
	{
		Debug.Log("Explode " + MVGameControllerBase.Game.LogicObjectManager.TimeStamp);
		ExplosionEvent explosionEvent = new ExplosionEvent(RuntimeEventType.Bazooka, gameObject.transform.position);
		SharedWorldObjectGameplayFunctions.Explosion.Explode(PrefabPool.Instance.ParticleExplosion, gameObject.transform.position, damageValue, damageRadius, shockwaveAcceleration, local: true, explosionEvent, new HashSet<int>());
	}
}
