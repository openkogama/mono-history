using System;
using System.Collections.Generic;
using MV.WorldObject;

public class MVPulseBox : MVLogicObject
{
	private const string prefabPath = "Prefabs/PulseBoxObject";

	private bool currentlyHot;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => true;

	public MVPulseBox(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/PulseBoxObject", worldObjects)
	{
		interactionFlags |= InteractionFlags.HasSettings;
	}

	public override void Initialize()
	{
		base.Initialize();
	}

	public override void OnInputLinkChanged()
	{
		SetOutput(IsActive());
	}

	public override void OnOutputLinkChanged()
	{
	}

	public override void OnInputStateChanged()
	{
		SetOutput(IsActive());
	}

	protected override void OnUpdate()
	{
		if (IsActive())
		{
			int num = (int)((float)Data["intervalOn"] * 1000f);
			int num2 = (int)((float)Data["intervalOff"] * 1000f);
			int num3 = Math.Abs(MVGameControllerBase.Game.Peer.ServerTimeInMilliSeconds) % (num + num2);
			if (num3 > num)
			{
				SetOutput(output: false);
			}
			else
			{
				SetOutput(output: true);
			}
		}
	}

	private bool IsActive()
	{
		if (InputLinkRefs.Count == 0)
		{
			return true;
		}
		return InputState;
	}

	private void SetOutput(bool output)
	{
		if (output)
		{
			if (currentlyHot)
			{
				return;
			}
			foreach (Link outputLinkRef in OutputLinkRefs)
			{
				outputLinkRef.isSet = true;
			}
			currentlyHot = true;
		}
		else
		{
			if (!currentlyHot)
			{
				return;
			}
			foreach (Link outputLinkRef2 in OutputLinkRefs)
			{
				outputLinkRef2.isSet = false;
			}
			currentlyHot = false;
		}
	}
}
