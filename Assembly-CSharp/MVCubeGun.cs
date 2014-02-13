using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MVCubeGun : MVPickupItemBase
{
	private CubeBullet cubeBullet;

	public MVCubeGun(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, worldObjects)
	{
		interactionFlags |= InteractionFlags.HasSettings;
		SetCubeMaterial();
	}

	private void SetCubeMaterial()
	{
		if ((Object)(object)cubeBullet == (Object)null)
		{
			cubeBullet = gameObject.GetComponentInChildren<CubeBullet>();
		}
		Hashtable hashtable = (Hashtable)Data["itemData"];
		cubeBullet.SetCubeMaterial((byte)hashtable["material"]);
		gameObject.GetComponentInChildren<PickupItemObjectScript>().InitializeOriginalMaterials();
	}

	public override void OnDataUpdate()
	{
		base.OnDataUpdate();
		SetCubeMaterial();
	}
}
