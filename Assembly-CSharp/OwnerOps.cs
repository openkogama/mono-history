using UnityEngine;
using UnityEngine.EventSystems;

public static class OwnerOps
{
	public static void RevokeEditRightsAndKick(MonoBehaviour caller, MVPlayer player)
	{
		RevokeEditRightsAndKick(caller.gameObject, player);
	}

	public static void RevokeEditRightsAndKick(GameObject caller, MVPlayer player)
	{
		MVGameControllerBase.Game.OperationRequestSender.RevokeEditRights(player);
		ExecuteEvents.ExecuteHierarchy(caller, null, (IUIStack x, BaseEventData y) =>
		{
			x.PopGroups(UIGroupFlags.GameObjectUI);
		});
	}
}
