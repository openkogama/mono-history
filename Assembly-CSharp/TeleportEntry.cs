using UnityEngine;

public class TeleportEntry
{
	private const float speed = 50f;

	private int woID;

	private int actorNr;

	private Vector3 fromPos;

	private Vector3 toPos;

	private float startTime;

	private float interval;

	public TeleportEntry(int woID, MVTeleporter from, MVWorldObjectClient to)
	{
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		this.woID = woID;
		actorNr = -1;
		foreach (MVPlayer value in MVGameController.Instance.WOCM.Players.Values)
		{
			if (value.Avatar.Id == woID)
			{
				actorNr = value.ActorNr;
				break;
			}
		}
		if (actorNr == -1)
		{
			Debug.LogWarning((object)"Could not find actor - not an actor trying to teleport...");
			interval = 0f;
			return;
		}
		fromPos = from.GameObject.transform.position;
		toPos = to.GameObject.transform.position;
		startTime = Time.time;
		interval = Vector3.Distance(fromPos, toPos) / 50f;
	}

	public bool Update()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (Time.time - startTime >= interval)
		{
			return false;
		}
		float num = (Time.time - startTime) / interval;
		MVGameController.Instance.WOCM.WorldObjects[woID].GameObject.transform.position = Vector3.Lerp(fromPos, toPos, num);
		return true;
	}
}
