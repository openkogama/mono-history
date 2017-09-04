using UnityEngine;

public class FirstTimeActivatablePointerPlayerList : FirstTimeActivatableGotItPointer
{
	[SerializeField]
	private int playersRequiredForShowingPlayerList;

	public override bool CanShow
	{
		get
		{
			if (MVGameControllerBase.Game.MVPlayerContainer.Count >= playersRequiredForShowingPlayerList)
			{
				foreach (MVPlayer value in MVGameControllerBase.Game.MVPlayerContainer.Values)
				{
					if (!value.IsAnonymous)
					{
						return base.CanShow;
					}
				}
			}
			return false;
		}
	}
}
