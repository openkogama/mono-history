using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

namespace MV.WorldObject;

public abstract class MVWorldObject
{
	public delegate void CallBackDelegate(MVWorldObject wos);

	protected int id;

	protected int groupId;

	protected int itemId;

	internal bool inputState;

	internal List<Link> outputLinkRefs = new List<Link>();

	internal List<Link> inputLinkRefs = new List<Link>();

	internal List<ObjectLink> objectLinkRefs = new List<ObjectLink>();

	private int ownerActorNr;

	private int previewOwnerProfileId;

	private Vector3 position;

	private Quaternion rotation;

	private Vector3 scale;

	private WorldObjectType type;

	internal Dictionary<object, object> data;

	private Dictionary<object, object> runTimeData = new Dictionary<object, object>();

	private int timestamp;

	public Action<List<Link>> OnInputLinkChanged;

	public Action<List<Link>> OnOutputLinkChanged;

	public int Id => id;

	public int GroupId
	{
		get
		{
			return groupId;
		}
		set
		{
			groupId = value;
		}
	}

	public int ItemId
	{
		get
		{
			return itemId;
		}
		set
		{
			itemId = value;
		}
	}

	public int OwnerActorNr
	{
		get
		{
			return ownerActorNr;
		}
		set
		{
			ownerActorNr = value;
		}
	}

	public int PreviewOwnerProfileId
	{
		get
		{
			return previewOwnerProfileId;
		}
		set
		{
			previewOwnerProfileId = value;
		}
	}

	public virtual Vector3 Position
	{
		get
		{
			return position;
		}
		set
		{
			position = value;
		}
	}

	public virtual Quaternion Rotation
	{
		get
		{
			return rotation;
		}
		set
		{
			rotation = value;
		}
	}

	public virtual Vector3 Scale
	{
		get
		{
			return scale;
		}
		set
		{
			scale = value;
		}
	}

	public virtual Vector3 WorldPosition => Vector3.zero;

	public virtual Quaternion WorldRotation => Quaternion.identity;

	public WorldObjectType WorldObjectType
	{
		get
		{
			return type;
		}
		set
		{
			type = value;
		}
	}

	public Dictionary<object, object> Data
	{
		get
		{
			return data;
		}
		set
		{
			data = value;
		}
	}

	public virtual Dictionary<object, object> RunTimeData
	{
		get
		{
			return runTimeData;
		}
		set
		{
			runTimeData = value;
		}
	}

	public int Timestamp
	{
		get
		{
			return timestamp;
		}
		set
		{
			timestamp = value;
		}
	}

	public List<Link> InputLinkRefs => inputLinkRefs;

	public List<Link> OutputLinkRefs => outputLinkRefs;

	public List<ObjectLink> ObjectLinkRefs => objectLinkRefs;

	public bool InputState => inputState;

	public virtual bool HasOutputConnector => true;

	public virtual bool HasInputConnector => true;

	public virtual bool HasObjectConnector => true;

	public virtual void OnInputStateChanged()
	{
	}

	public virtual void Reset()
	{
	}

	public virtual void OnObjectLinkChanged()
	{
	}

	public virtual bool IsSingletonObject()
	{
		return false;
	}

	public MVWorldObject()
	{
	}

	public MVWorldObject(MVWorldObject wo)
	{
		id = wo.id;
		groupId = wo.groupId;
		inputState = wo.inputState;
		outputLinkRefs = wo.outputLinkRefs;
		inputLinkRefs = wo.inputLinkRefs;
		ownerActorNr = wo.ownerActorNr;
		previewOwnerProfileId = wo.previewOwnerProfileId;
		position = wo.position;
		rotation = wo.rotation;
		scale = wo.scale;
		type = wo.type;
		data = wo.data;
		runTimeData = wo.runTimeData;
		timestamp = wo.timestamp;
	}

	public virtual void TraverseRecursiveTail(CallBackDelegate callBack, Dictionary<int, MVWorldObject> wos)
	{
		callBack(this);
	}

	protected void ResetRunTimeData()
	{
		RuntimeVariablesRepository.SetupRuntimeVariable(WorldObjectType, RunTimeData);
	}

	public Dictionary<object, object> DeepCopyWorldObjectDataParameters()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add(WorldObjectDataParameters.WorldObjectType, WorldObjectType);
		dictionary.Add(WorldObjectDataParameters.Id, Id);
		dictionary.Add(WorldObjectDataParameters.GroudId, GroupId);
		dictionary.Add(WorldObjectDataParameters.ItemId, ItemId);
		dictionary.Add(WorldObjectDataParameters.OwnerActorNumber, OwnerActorNr);
		dictionary.Add(WorldObjectDataParameters.PreviewOwnerProfileId, PreviewOwnerProfileId);
		dictionary.Add(WorldObjectDataParameters.Position, Position);
		dictionary.Add(WorldObjectDataParameters.Rotation, Rotation);
		dictionary.Add(WorldObjectDataParameters.Scale, Scale);
		dictionary.Add(WorldObjectDataParameters.Data, HashtableFunctions.DeepCopyHashTable(Data));
		dictionary.Add(WorldObjectDataParameters.RuntimeData, HashtableFunctions.DeepCopyHashTable(RunTimeData));
		return dictionary;
	}

	protected void GetLinksForClone(List<int> links)
	{
		foreach (Link inputLinkRef in inputLinkRefs)
		{
			if (!links.Contains(inputLinkRef.id))
			{
				links.Add(inputLinkRef.id);
			}
		}
		foreach (Link outputLinkRef in outputLinkRefs)
		{
			if (!links.Contains(outputLinkRef.id))
			{
				links.Add(outputLinkRef.id);
			}
		}
	}

	protected void GetObjectLinksForClone(List<int> objectLinks)
	{
		foreach (ObjectLink objectLinkRef in objectLinkRefs)
		{
			if (!objectLinks.Contains(objectLinkRef.id))
			{
				objectLinks.Add(objectLinkRef.id);
			}
		}
	}

	public void AddOutputLink(Link link)
	{
		outputLinkRefs.Add(link);
		link.outputWOID = id;
		if (OnOutputLinkChanged != null)
		{
			OnOutputLinkChanged(outputLinkRefs);
		}
	}

	public void AddInputLink(Link link)
	{
		inputLinkRefs.Add(link);
		link.inputWOID = id;
		if (OnInputLinkChanged != null)
		{
			OnInputLinkChanged(inputLinkRefs);
		}
	}

	public bool AddObjectLink(ObjectLink link)
	{
		objectLinkRefs.Add(link);
		OnObjectLinkChanged();
		return true;
	}

	public bool RemoveOutputLink(Link link)
	{
		bool result = outputLinkRefs.Remove(link);
		if (OnOutputLinkChanged != null)
		{
			OnOutputLinkChanged(outputLinkRefs);
		}
		return result;
	}

	public bool RemoveInputLink(Link link)
	{
		bool result = inputLinkRefs.Remove(link);
		OnInputLinkChanged(inputLinkRefs);
		return result;
	}

	public bool RemoveObjectLink(ObjectLink link)
	{
		int indexOfObjectLink = GetIndexOfObjectLink(link);
		if (indexOfObjectLink == -1)
		{
			return false;
		}
		objectLinkRefs.RemoveAt(indexOfObjectLink);
		OnObjectLinkChanged();
		return true;
	}

	public bool ContainObjectLink(ObjectLink link)
	{
		if (GetIndexOfObjectLink(link) == -1)
		{
			return false;
		}
		return true;
	}

	private int GetIndexOfObjectLink(ObjectLink link)
	{
		for (int i = 0; i < objectLinkRefs.Count; i++)
		{
			if (objectLinkRefs[i].objectConnectorWOID == link.objectConnectorWOID && objectLinkRefs[i].objectWOID == link.objectWOID)
			{
				return i;
			}
		}
		return -1;
	}

	public virtual MVWorldObject ShallowCopy()
	{
		return (MVWorldObject)MemberwiseClone();
	}

	public virtual MVWorldObject DeepCopy()
	{
		MVWorldObject mVWorldObject = ShallowCopy();
		mVWorldObject.outputLinkRefs = new List<Link>();
		foreach (Link outputLinkRef in outputLinkRefs)
		{
			mVWorldObject.outputLinkRefs.Add(new Link(outputLinkRef.id, outputLinkRef.inputWOID, outputLinkRef.inputWOID, outputLinkRef.isSet));
		}
		mVWorldObject.inputLinkRefs = new List<Link>();
		foreach (Link inputLinkRef in inputLinkRefs)
		{
			mVWorldObject.inputLinkRefs.Add(new Link(inputLinkRef.id, inputLinkRef.inputWOID, inputLinkRef.inputWOID, inputLinkRef.isSet));
		}
		mVWorldObject.data = HashtableFunctions.DeepCopyHashTable(data);
		mVWorldObject.runTimeData = HashtableFunctions.DeepCopyHashTable(runTimeData);
		return mVWorldObject;
	}

	public override string ToString()
	{
		return string.Concat(WorldObjectType, " id: ", id, " groupId: ", groupId, " itemId: ", itemId);
	}

	public virtual void PartialUpdateWOData(Dictionary<object, object> newWOData)
	{
		CommonUtils.PartialUpdateHashtable(data, newWOData);
	}

	public virtual void PartialRemoveFromWOData(Dictionary<object, object> dataToRemove)
	{
		CommonUtils.PartialRemoveFromHashtable(data, dataToRemove);
	}
}
