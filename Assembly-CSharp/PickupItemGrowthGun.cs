using MV.Common;
using MV.WorldObject;

public class PickupItemGrowthGun : SizeGunBase
{
	public override AvatarItemType Type => AvatarItemType.GrowthGun;

	protected override InteractionData GetPackageData()
	{
		return GrowthGunHitPackage.Create();
	}
}
