using System;
using System.Collections;

namespace MV.WorldObject;

public class MVPrototype
{
	protected int id;

	protected int itemID;

	protected int typeID;

	protected string name;

	protected Hashtable data;

	protected float scale;

	protected int authorProfileID;

	protected int authorPlanetID;

	protected int instanceCounter;

	public int ID
	{
		get
		{
			return id;
		}
		set
		{
			id = value;
		}
	}

	public int ItemID
	{
		get
		{
			return itemID;
		}
		set
		{
			itemID = value;
		}
	}

	public int TypeID
	{
		get
		{
			return typeID;
		}
		set
		{
			typeID = value;
		}
	}

	public string Name
	{
		get
		{
			return name;
		}
		set
		{
			name = value;
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

	public float Scale
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

	public int AuthorProfileID
	{
		get
		{
			return authorProfileID;
		}
		set
		{
			authorProfileID = value;
		}
	}

	public int AuthorPlanetID
	{
		get
		{
			return authorPlanetID;
		}
		set
		{
			authorPlanetID = value;
		}
	}

	public int InstanceCounter
	{
		get
		{
			return instanceCounter;
		}
		set
		{
			instanceCounter = value;
			if (instanceCounter <= 0 && LastInstanceRemoved != null)
			{
				LastInstanceRemoved(this, new LastInstanceRemovedEventArgs(ID));
			}
		}
	}

	public event EventHandler<LastInstanceRemovedEventArgs> LastInstanceRemoved;
}
