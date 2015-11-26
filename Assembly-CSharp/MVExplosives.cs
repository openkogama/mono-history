using System.Collections.Generic;
using MV.Common;
using MV.WorldObject.RuntimeEvents;
using UnityEngine;

public class MVExplosives(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects) : MVLogicObject(data, "Prefabs/ExplosivesObject", worldObjects)
{
	private const string prefabPath = "Prefabs/ExplosivesObject";

	private float damageRadius = 10f;

	private float damageValue = 150f;

	private float shockwaveAcceleration = 3500f;

	private GameObject audioGO;

	private AudioLogicCube audioLC;

	private bool isInitialized;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => false;

	public override void Initialize()
	{
		base.Initialize();
		isInitialized = true;
	}

	public override void OnInputStateChanged()
	{
		if (InputState && isInitialized)
		{
			Explode();
		}
	}

	public void Explode()
	{
		ExplosionEvent explosionEvent = new ExplosionEvent(RuntimeEventType.Bazooka, gameObject.transform.position);
		SharedWorldObjectGameplayFunctions.Explosion.Explode("ParticleFX/Explosion", gameObject.transform.position, damageValue, damageRadius, shockwaveAcceleration, local: true, explosionEvent, new HashSet<int>());
	}
}
