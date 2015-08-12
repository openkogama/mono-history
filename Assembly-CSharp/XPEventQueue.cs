using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public abstract class XPEventQueue : IUpdatecontrollerSubscriber
{
	private ObscuredInt waitInterval = 500;

	private ObscuredInt xpDataQueueLimit = 10;

	private ObscuredBool disabled = false;

	private Queue<string> xpDataQueue = new Queue<string>();

	private WaitForTicks waitForTicks = new WaitForTicks(0);

	protected XPProgress xpProgress;

	public XPProgressData XPProgressData => xpProgress.XPProgressData;

	public XPProgress.OnXPProgressDataDelegate OnXPProgressData
	{
		get
		{
			return xpProgress.OnXPProgressData;
		}
		set
		{
			xpProgress.OnXPProgressData = value;
		}
	}

	public void Initialize(MVLocalPlayer player, InitialLevelData initialLevelData)
	{
		xpProgress = new XPProgress(player, initialLevelData);
		UpdateController.AddUpdateObject(this, UpdatePriority.UPDATEBUCKET_STANDARD);
	}

	public void AddXp(string xpType)
	{
		if (!disabled)
		{
			xpDataQueue.Enqueue(xpType);
			if (xpDataQueue.Count > (int)xpDataQueueLimit)
			{
				xpDataQueue.Clear();
				disabled = true;
				Debug.LogWarning("To many xp requests. Disabling xp for session.");
			}
		}
	}

	public virtual void UpdateControllerUpdate()
	{
		if (LevelingManager.IsInitialized && waitForTicks.TimeIsUp && xpDataQueue.Count != 0 && XPManager.TryGetXPData(xpDataQueue.Dequeue(), out var xpData))
		{
			RequestXp(xpData);
			waitForTicks = new WaitForTicks(waitInterval);
		}
	}

	protected abstract void RequestXp(XPData xpData);

	public void UpdateControllerFixedUpdate()
	{
	}
}
