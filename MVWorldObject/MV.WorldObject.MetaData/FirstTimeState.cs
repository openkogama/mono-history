using System.Collections;
using System.Text;

namespace MV.WorldObject.MetaData;

public class FirstTimeState
{
	private BitArray bitArray = new BitArray(0);

	public byte[] ByteArray
	{
		get
		{
			byte[] array = new byte[bitArray.Length / 8 + ((bitArray.Length % 8 != 0) ? 1 : 0)];
			bitArray.CopyTo(array, 0);
			return array;
		}
		set
		{
			bitArray = new BitArray(value);
		}
	}

	public void SetFirstTimeEvent(FirstTimeEvent firstTimeEvent)
	{
		if (bitArray.Length <= (int)firstTimeEvent)
		{
			bitArray.Length = (int)(firstTimeEvent + 1);
		}
		bitArray[(int)firstTimeEvent] = true;
	}

	public void OverrideFirstTimeEvent(FirstTimeEvent firstTimeEvent, bool value)
	{
		if (bitArray.Length <= (int)firstTimeEvent)
		{
			bitArray.Length = (int)(firstTimeEvent + 1);
		}
		bitArray[(int)firstTimeEvent] = value;
	}

	public bool HasFirstTimeEventOccured(FirstTimeEvent firstTimeEvent)
	{
		if (firstTimeEvent == FirstTimeEvent.NoEvent)
		{
			return true;
		}
		if (bitArray.Length <= (int)firstTimeEvent)
		{
			return false;
		}
		return bitArray[(int)firstTimeEvent];
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < bitArray.Count; i++)
		{
			char value = (bitArray[i] ? '1' : '0');
			stringBuilder.Append(value);
		}
		return stringBuilder.ToString();
	}
}
