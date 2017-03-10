namespace MV.WorldObject;

public static class SharedLinkFunctions
{
	public static void EventSetOutput(IWorldObjectManager manager, MVWorldObject wo)
	{
		foreach (Link outputLinkRef in wo.OutputLinkRefs)
		{
			MVWorldObject worldObject = manager.GetWorldObject(outputLinkRef.inputWOID);
			if (worldObject != null && EvaluateLinks(worldObject))
			{
				EventSetOutput(manager, worldObject);
			}
		}
	}

	public static bool EvaluateLinks(MVWorldObject wo)
	{
		bool flag = false;
		WorldObjectType worldObjectType = wo.WorldObjectType;
		flag = ((worldObjectType != WorldObjectType.And) ? OrInputEval(wo) : AndInputEval(wo));
		if (flag != wo.inputState)
		{
			wo.inputState = flag;
			UpdateOutputLinks(wo);
			wo.OnInputStateChanged();
			return true;
		}
		return false;
	}

	public static void UpdateOutputLinks(MVWorldObject wo)
	{
		switch (wo.WorldObjectType)
		{
		case WorldObjectType.And:
			MimicInput(wo);
			break;
		case WorldObjectType.Battery:
			AlwaysOn(wo);
			break;
		case WorldObjectType.Negate:
			NegateInput(wo);
			break;
		case WorldObjectType.ToggleBox:
			break;
		}
	}

	private static bool OrInputEval(MVWorldObject wo)
	{
		if (wo.inputLinkRefs.Count <= 0)
		{
			return false;
		}
		foreach (Link inputLinkRef in wo.inputLinkRefs)
		{
			if (inputLinkRef.isSet)
			{
				return true;
			}
		}
		return false;
	}

	private static bool AndInputEval(MVWorldObject wo)
	{
		if (wo.inputLinkRefs.Count <= 0)
		{
			return false;
		}
		foreach (Link inputLinkRef in wo.inputLinkRefs)
		{
			if (!inputLinkRef.isSet)
			{
				return false;
			}
		}
		return true;
	}

	private static void MimicInput(MVWorldObject wo)
	{
		foreach (Link outputLinkRef in wo.outputLinkRefs)
		{
			outputLinkRef.isSet = wo.inputState;
		}
	}

	private static void NegateInput(MVWorldObject wo)
	{
		foreach (Link outputLinkRef in wo.outputLinkRefs)
		{
			outputLinkRef.isSet = !wo.inputState;
		}
	}

	private static void AlwaysOn(MVWorldObject wo)
	{
		foreach (Link outputLinkRef in wo.outputLinkRefs)
		{
			outputLinkRef.isSet = true;
		}
	}
}
