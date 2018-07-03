using System;

public class MVLocalPlayerTourist : MVLocalPlayer
{
	public override bool IsAdmin
	{
		get
		{
			throw new NotImplementedException("This is a tourist. The IsAdmin-property should not exist in this class. Until proper refactoring can be done, this override is to prevent access otherwise provided by the inheritance.");
		}
	}

	public MVLocalPlayerTourist(int actorNumber, int profileID, string userName, string regionCode, int planetOwnershipTypeID)
		: base(actorNumber, profileID, userName, regionCode, planetOwnershipTypeID, isAdmin: false, 0)
	{
	}
}
