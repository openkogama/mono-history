using System.Collections.Generic;

public class MVCubeGun : MVPickupItemBase
{
	private CubeBullet cubeBullet;

	public MVCubeGun(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, worldObjects)
	{
		interactionFlags |= InteractionFlags.HasSettings;
		SetCubeMaterial();
	}

	private void SetCubeMaterial()
	{
		if (cubeBullet == null)
		{
			cubeBullet = gameObject.GetComponentInChildren<CubeBullet>();
		}
		Dictionary<object, object> dictionary = (Dictionary<object, object>)Data["itemData"];
		cubeBullet.SetCubeMaterial((byte)dictionary["material"]);
		gameObject.GetComponentInChildren<GreyOutObjectScript>().InitializeOriginalMaterials();
	}

	public override void OnDataUpdate()
	{
		base.OnDataUpdate();
		SetCubeMaterial();
	}
}
