using System;
using System.Collections.Generic;
using MV.WorldObject;

public class LogicObjectManager
{
	public enum ValidateLinkStatus
	{
		LoopDetected,
		InputObjectIsNull,
		OutputObjectIsNull,
		BothInputAndOutputIsNull,
		LinkAlreadyExists,
		InputObjectDoesNotHaveConnector,
		OutputObjectDoesNotHaveConnector,
		InputObjectIdIsZeroOrLess,
		OutputObjectIdIsZeroOrLess,
		Ok
	}

	public enum ValidateObjectLinkStatus
	{
		ObjectConnectorWOIDIsZeroOrLess,
		ObjectWOIDIsZeroOrLess,
		BothObjectConnectorAndObjectWOIsNull,
		ObjectConnector,
		ObjectWO,
		ObjectConnectorHasNoObjectConnector,
		ObjectConnectorContainsLinkAlready,
		ObjectWOContainsLinkAlready,
		Ok
	}

	public enum ReportSeverity
	{
		Info,
		Warning,
		Error
	}

	public const int updateInterval = 100;

	public const int stepInterval = 1000;

	public const int updatesBeforeStepSend = 10;

	private Dictionary<int, IInputSignalReceiver> logicWorldObjects = new Dictionary<int, IInputSignalReceiver>();

	public readonly bool trackLoops;

	private HashSet<int> updatedIds = new HashSet<int>();

	public int TimeStamp { get; private set; }

	public int FrameCount => TimeStamp / 100;

	public LogicObjectManager(int timeStamp, bool trackLoops)
	{
		this.trackLoops = trackLoops;
		TimeStamp = timeStamp;
	}

	public void Reset()
	{
		TimeStamp = 0;
	}

	public int OnLinkAdded(Link link, IWorldObjectManager worldObjectManager)
	{
		return ResetChunk(link.inputWOID, worldObjectManager);
	}

	public int OnLinkRemoved(Link link, IWorldObjectManager worldObjectManager)
	{
		int num = ResetChunk(link.inputWOID, worldObjectManager);
		int num2 = ResetChunk(link.outputWOID, worldObjectManager);
		return num + num2;
	}

	public void AddLogicObjectToUpdate(int worldObjectID, IInputSignalReceiver logicWorldObject)
	{
		logicWorldObjects.Add(worldObjectID, logicWorldObject);
	}

	public void RemoveLogicObjectFromUpdate(int worldObjectID)
	{
		if (logicWorldObjects.ContainsKey(worldObjectID))
		{
			logicWorldObjects.Remove(worldObjectID);
		}
	}

	public void Update()
	{
		foreach (IInputSignalReceiver value in logicWorldObjects.Values)
		{
			value.UpdateSignal(value.DefaultInput);
		}
		TimeStamp += 100;
		if (trackLoops)
		{
			ClearDebugIds();
		}
	}

	public static int ResetChunk(int woID, IWorldObjectManager worldObjectManager)
	{
		HashSet<int> hashSet = new HashSet<int>();
		ResetNode(woID, hashSet, worldObjectManager);
		return hashSet.Count;
	}

	public static ValidateObjectLinkStatus ValidateObjectLink(ObjectLink objectLink, IWorldObjectManager worldObjectManager, out ReportSeverity reportSeverity)
	{
		if (objectLink.objectConnectorWOID <= 0)
		{
			reportSeverity = ReportSeverity.Error;
			return ValidateObjectLinkStatus.ObjectConnectorWOIDIsZeroOrLess;
		}
		if (objectLink.objectWOID <= 0)
		{
			reportSeverity = ReportSeverity.Error;
			return ValidateObjectLinkStatus.ObjectWOIDIsZeroOrLess;
		}
		bool flag = worldObjectManager.TryGetWorldObject(objectLink.objectConnectorWOID, out var worldObject);
		bool flag2 = worldObjectManager.TryGetWorldObject(objectLink.objectWOID, out var worldObject2);
		if (!flag && !flag2)
		{
			reportSeverity = ReportSeverity.Info;
			return ValidateObjectLinkStatus.BothObjectConnectorAndObjectWOIsNull;
		}
		if (!flag)
		{
			reportSeverity = ReportSeverity.Info;
			return ValidateObjectLinkStatus.ObjectConnector;
		}
		if (!flag2)
		{
			reportSeverity = ReportSeverity.Info;
			return ValidateObjectLinkStatus.ObjectWO;
		}
		if (!worldObject.HasObjectConnector)
		{
			reportSeverity = ReportSeverity.Error;
			return ValidateObjectLinkStatus.ObjectConnectorHasNoObjectConnector;
		}
		if (worldObject.ContainObjectLink(objectLink))
		{
			reportSeverity = ReportSeverity.Info;
			return ValidateObjectLinkStatus.ObjectConnectorContainsLinkAlready;
		}
		if (worldObject2.ContainObjectLink(objectLink))
		{
			reportSeverity = ReportSeverity.Info;
			return ValidateObjectLinkStatus.ObjectWOContainsLinkAlready;
		}
		reportSeverity = ReportSeverity.Info;
		return ValidateObjectLinkStatus.Ok;
	}

	public static ValidateLinkStatus ValidateLink(int linkOutputWoId, int linkInputWoId, IWorldObjectManager worldObjectManager, out ReportSeverity reportSeverity)
	{
		if (linkInputWoId <= 0)
		{
			reportSeverity = ReportSeverity.Error;
			return ValidateLinkStatus.InputObjectIdIsZeroOrLess;
		}
		if (linkOutputWoId <= 0)
		{
			reportSeverity = ReportSeverity.Error;
			return ValidateLinkStatus.OutputObjectIdIsZeroOrLess;
		}
		MVWorldObject worldObject = worldObjectManager.GetWorldObject(linkOutputWoId);
		MVWorldObject worldObject2 = worldObjectManager.GetWorldObject(linkInputWoId);
		if (worldObject == null && worldObject2 == null)
		{
			reportSeverity = ReportSeverity.Info;
			return ValidateLinkStatus.BothInputAndOutputIsNull;
		}
		if (worldObject == null)
		{
			reportSeverity = ReportSeverity.Info;
			return ValidateLinkStatus.OutputObjectIsNull;
		}
		if (worldObject2 == null)
		{
			reportSeverity = ReportSeverity.Info;
			return ValidateLinkStatus.InputObjectIsNull;
		}
		if (!worldObject2.HasInputConnector)
		{
			reportSeverity = ReportSeverity.Error;
			return ValidateLinkStatus.InputObjectDoesNotHaveConnector;
		}
		if (!worldObject.HasOutputConnector)
		{
			reportSeverity = ReportSeverity.Error;
			return ValidateLinkStatus.OutputObjectDoesNotHaveConnector;
		}
		if (DoesLinkExist(worldObject, linkInputWoId))
		{
			reportSeverity = ReportSeverity.Info;
			return ValidateLinkStatus.LinkAlreadyExists;
		}
		bool loopDetected = false;
		ValidateLink(linkOutputWoId, linkInputWoId, worldObjectManager, ref loopDetected);
		if (loopDetected)
		{
			reportSeverity = ReportSeverity.Error;
			return ValidateLinkStatus.LoopDetected;
		}
		reportSeverity = ReportSeverity.Info;
		return ValidateLinkStatus.Ok;
	}

	private static bool DoesLinkExist(MVWorldObject outputWo, int inputWOID)
	{
		foreach (Link outputLinkRef in outputWo.OutputLinkRefs)
		{
			if (outputLinkRef.inputWOID == inputWOID)
			{
				return true;
			}
		}
		return false;
	}

	private static void ValidateLink(int woIdFrom, int woIdTo, IWorldObjectManager worldObjectManager, ref bool loopDetected)
	{
		if (woIdFrom == woIdTo)
		{
			loopDetected = true;
			return;
		}
		MVWorldObject worldObject = worldObjectManager.GetWorldObject(woIdTo);
		foreach (Link outputLinkRef in worldObject.OutputLinkRefs)
		{
			ValidateLink(woIdFrom, outputLinkRef.inputWOID, worldObjectManager, ref loopDetected);
			if (loopDetected)
			{
				break;
			}
		}
	}

	private static void ResetNode(int woID, HashSet<int> resetNodes, IWorldObjectManager worldObjectManager)
	{
		if (resetNodes.Contains(woID))
		{
			return;
		}
		MVWorldObject worldObject = worldObjectManager.GetWorldObject(woID);
		((ILogicWorldObject)worldObject).InputSignalReceiver.Reset();
		worldObject.Reset();
		resetNodes.Add(woID);
		foreach (Link inputLinkRef in worldObject.InputLinkRefs)
		{
			ResetNode(inputLinkRef.outputWOID, resetNodes, worldObjectManager);
		}
		foreach (Link outputLinkRef in worldObject.OutputLinkRefs)
		{
			ResetNode(outputLinkRef.inputWOID, resetNodes, worldObjectManager);
		}
	}

	public void DebugAddId(int id)
	{
		if (updatedIds.Contains(id))
		{
			throw new Exception("Id already evaluated. This is a loop issue");
		}
		updatedIds.Add(id);
	}

	private void ClearDebugIds()
	{
		updatedIds.Clear();
	}

	public override string ToString()
	{
		return $"logicWorldObjects.Count {logicWorldObjects.Values.Count} ";
	}
}
