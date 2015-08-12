using System;
using System.Collections.Generic;

namespace MV.WorldObject;

public class MVPrototype
{
	protected int id;

	protected int itemID;

	protected int typeID;

	protected string name = "";

	protected Dictionary<object, object> data;

	protected float scale;

	protected int insertedInWorldByProfileID;

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

	public bool IsEmpty => ((Dictionary<IntVector, byte[]>)Data[(byte)48]).Count == 0;

	public int InsertedInWorldByProfileID
	{
		get
		{
			return insertedInWorldByProfileID;
		}
		set
		{
			insertedInWorldByProfileID = value;
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

	public static byte[] GetPrototypeData(Dictionary<IntVector, byte[]> cubeDict)
	{
		BytePacker bytePacker = new BytePacker();
		int num = 0;
		bytePacker.Write(cubeDict.Count);
		foreach (KeyValuePair<IntVector, byte[]> item in cubeDict)
		{
			if (CubeDataPacker.GetCubesInRow(item.Value[0]) != 0)
			{
				bytePacker.Write(item.Key.x);
				bytePacker.Write(item.Key.y);
				bytePacker.Write(item.Key.z);
				bytePacker.Write(item.Value);
				num++;
			}
		}
		bytePacker.Position = 0;
		bytePacker.Write(num);
		return bytePacker.ToArray();
	}

	public virtual MVPrototype ShallowCopy()
	{
		return (MVPrototype)MemberwiseClone();
	}

	public virtual MVPrototype DeepCopy()
	{
		MVPrototype mVPrototype = ShallowCopy();
		Dictionary<IntVector, byte[]> dictionary = (Dictionary<IntVector, byte[]>)Data[(byte)48];
		Dictionary<IntVector, byte[]> dictionary2 = new Dictionary<IntVector, byte[]>();
		foreach (KeyValuePair<IntVector, byte[]> item in dictionary)
		{
			byte[] array = new byte[item.Value.Length];
			for (int i = 0; i < item.Value.Length; i++)
			{
				array[i] = item.Value[i];
			}
			dictionary2.Add(item.Key, array);
		}
		mVPrototype.Data[(byte)48] = dictionary2;
		return mVPrototype;
	}
}
