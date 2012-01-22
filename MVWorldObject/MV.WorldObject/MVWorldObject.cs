using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MV.WorldObject;

public abstract class MVWorldObject
{
	protected int id;

	protected int groupId;

	internal bool inputState;

	private MVWorldObjectState state;

	protected List<OldLink> oldLinks = new List<OldLink>();

	protected List<OldLink> oldLinkReferences = new List<OldLink>();

	internal List<Link> outputLinkRefs = new List<Link>();

	internal List<Link> inputLinkRefs = new List<Link>();

	private int ownerActorNr;

	private Vector3 position;

	private Quaternion rotation;

	private Vector3 scale;

	private WorldObjectType type;

	internal Hashtable data;

	internal Hashtable runTimeData;

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

	public Vector3 Position
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

	public Quaternion Rotation
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

	public Vector3 Scale
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

	public List<OldLink> OldLinks => oldLinks;

	public List<OldLink> OldLinkReferences => oldLinkReferences;

	public List<Link> InputLinkRefs => inputLinkRefs;

	public List<Link> OutputLinkRefs => outputLinkRefs;

	public bool InputState => inputState;

	public virtual bool HasOutputConnector => true;

	public virtual bool HasInputConnector => true;

	public virtual void OnInputStateChanged()
	{
	}

	public virtual void ResetLogic()
	{
	}

	public virtual void OnInputLinkChanged()
	{
	}

	public virtual void OnOutputLinkChanged()
	{
	}

	public MVWorldObject()
	{
		state = MVWorldObjectState.Created;
	}

	public MVWorldObject(MVWorldObject wo)
	{
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		id = wo.id;
		groupId = wo.groupId;
		inputState = wo.inputState;
		state = wo.state;
		oldLinks = wo.oldLinks;
		oldLinkReferences = wo.OldLinkReferences;
		outputLinkRefs = wo.outputLinkRefs;
		inputLinkRefs = wo.inputLinkRefs;
		ownerActorNr = wo.ownerActorNr;
		position = wo.position;
		rotation = wo.rotation;
		scale = wo.scale;
		type = wo.type;
		data = wo.data;
		runTimeData = wo.runTimeData;
		timestamp = wo.timestamp;
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
}
