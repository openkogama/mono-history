using MV.Common;
using MV.WorldObject;

public class PickupItemGrowthGun : MouseGun
{
	public override AvatarItemType Type => AvatarItemType.GrowthGun;

	protected override InteractionData GetPackageData()
	{
		return GrowthGunHitPackage.Create();
	}
}
