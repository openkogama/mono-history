using System.Collections;
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

	private MVWorldObjectState state;

	internal List<Link> outputLinkRefs = new List<Link>();

	internal List<Link> inputLinkRefs = new List<Link>();

	internal List<ObjectLink> objectLinkRefs = new List<ObjectLink>();

	private int ownerActorNr;

	private int previewOwnerProfileId;

	private Vector3 position;

	private Quaternion rotation;

	private Vector3 scale;

	private WorldObjectType type;

	internal Hashtable data;

	internal Hashtable runTimeData = new Hashtable();

	private int timestamp;

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

	public MVWorldObjectState State
	{
		get
		{
			return state;
		}
		set
		{
			state = value;
		}
	}

	public virtual Vector3 Position
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return position;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			position = value;
		}
	}

	public virtual Quaternion Rotation
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return rotation;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			rotation = value;
		}
	}

	public virtual Vector3 Scale
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return scale;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			scale = value;
		}
	}

	public virtual Vector3 WorldPosition
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return Vector3.zero;
		}
	}

	public virtual Quaternion WorldRotation
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return Quaternion.identity;
		}
	}

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

	public Hashtable Data
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

	public Hashtable RunTimeData
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

	public virtual void OnInputLinkChanged()
	{
	}

	public virtual void OnOutputLinkChanged()
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
		state = MVWorldObjectState.Created;
	}

	public MVWorldObject(MVWorldObject wo)
	{
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		id = wo.id;
		groupId = wo.groupId;
		inputState = wo.inputState;
		state = wo.state;
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
		RunTimeData = RuntimeVariablesRepository.GetRuntimeVariables(WorldObjectType);
	}

	public Hashtable DeepCopyWorldObjectDataParameters()
	{
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		Hashtable hashtable = new Hashtable();
		hashtable.Add(WorldObjectDataParameters.WorldObjectType, WorldObjectType);
		hashtable.Add(WorldObjectDataParameters.Id, Id);
		hashtable.Add(WorldObjectDataParameters.GroudId, GroupId);
		hashtable.Add(WorldObjectDataParameters.ItemId, ItemId);
		hashtable.Add(WorldObjectDataParameters.OwnerActorNumber, OwnerActorNr);
		hashtable.Add(WorldObjectDataParameters.PreviewOwnerProfileId, PreviewOwnerProfileId);
		hashtable.Add(WorldObjectDataParameters.Position, Position);
		hashtable.Add(WorldObjectDataParameters.Rotation, Rotation);
		hashtable.Add(WorldObjectDataParameters.Scale, Scale);
		hashtable.Add(WorldObjectDataParameters.Data, HashtableFunctions.DeepCopyHashTable(Data));
		hashtable.Add(WorldObjectDataParameters.RuntimeData, HashtableFunctions.DeepCopyHashTable(RunTimeData));
		return hashtable;
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

	public bool AddOutputLink(Link link)
	{
		if (!HasOutputConnector)
		{
			return false;
		}
		if (outputLinkRefs.Contains(link))
		{
			return false;
		}
		outputLinkRefs.Add(link);
		link.outputWOID = id;
		SharedLinkFunctions.UpdateOutputLinks(this);
		SharedLinkFunctions.EvaluateLinks(this);
		OnOutputLinkChanged();
		return true;
	}

	public bool AddInputLink(Link link)
	{
		if (!HasInputConnector)
		{
			return false;
		}
		if (inputLinkRefs.Contains(link))
		{
			return false;
		}
		inputLinkRefs.Add(link);
		link.inputWOID = id;
		SharedLinkFunctions.EvaluateLinks(this);
		OnInputLinkChanged();
		return true;
	}

	public bool AddObjectLink(ObjectLink link)
	{
		if (!HasObjectConnector)
		{
			return false;
		}
		if (GetIndexOfObjectLink(link) != -1)
		{
			return false;
		}
		objectLinkRefs.Add(link);
		OnObjectLinkChanged();
		return true;
	}

	public bool RemoveOutputLink(Link link)
	{
		if (!HasOutputConnector)
		{
			return false;
		}
		if (!outputLinkRefs.Contains(link))
		{
			return false;
		}
		bool result = outputLinkRefs.Remove(link);
		SharedLinkFunctions.UpdateOutputLinks(this);
		SharedLinkFunctions.EvaluateLinks(this);
		OnOutputLinkChanged();
		return result;
	}

	public bool RemoveInputLink(Link link)
	{
		if (!HasInputConnector)
		{
			return false;
		}
		if (!inputLinkRefs.Contains(link))
		{
			return false;
		}
		bool result = inputLinkRefs.Remove(link);
		SharedLinkFunctions.EvaluateLinks(this);
		OnInputLinkChanged();
		return result;
	}

	public bool RemoveObjectLink(ObjectLink link)
	{
		if (!HasObjectConnector)
		{
			return false;
		}
		int indexOfObjectLink = GetIndexOfObjectLink(link);
		if (indexOfObjectLink == -1)
		{
			return false;
		}
		OnObjectLinkChanged();
		objectLinkRefs.RemoveAt(indexOfObjectLink);
		return true;
	}

	private int GetIndexOfObjectLink(ObjectLink link)
	{
		int num = -1;
		for (int i = 0; i < objectLinkRefs.Count; i++)
		{
			if (objectLinkRefs[i].objectWOID == link.objectWOID && num == -1)
			{
				num = i;
			}
		}
		return num;
	}

	public bool ValidateLink(Link link)
	{
		if (link.inputWOID == Id)
		{
			foreach (Link inputLinkRef in inputLinkRefs)
			{
				if (inputLinkRef.outputWOID == link.outputWOID)
				{
					return false;
				}
			}
			return true;
		}
		if (link.outputWOID == Id)
		{
			foreach (Link outputLinkRef in outputLinkRefs)
			{
				if (outputLinkRef.inputWOID == link.inputWOID)
				{
					return false;
				}
			}
			return true;
		}
		return false;
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
		return string.Concat(new object[7] { WorldObjectType, " id: ", id, " groupId: ", groupId, " itemId: ", itemId });
	}

	public virtual void PartialUpdateWOData(Hashtable newWOData)
	{
		CommonUtils.PartialUpdateHashtable(data, newWOData);
	}

	public virtual void PartialRemoveFromWOData(Hashtable dataToRemove)
	{
		CommonUtils.PartialRemoveFromHashtable(data, dataToRemove);
	}
}
