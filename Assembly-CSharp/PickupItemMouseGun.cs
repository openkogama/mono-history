using MV.Common;
using MV.WorldObject;

public class PickupItemMouseGun : SizeGunBase
{
	public override AvatarItemType Type => AvatarItemType.MouseGun;

	protected override InteractionData GetPackageData()
	{
		return MouseGunHitPackage.Create();
	}
}
