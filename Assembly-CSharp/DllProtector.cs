using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MV.WorldObject.AntiCheat;
using UnityEngine;

public static class DllProtector
{
	[DllImport("DLLProtector")]
	private static extern uint GetCrcData(byte[] buffer, int len);

	private static int FirstIndexOf(byte[] data, int startIndex, byte val)
	{
		int num = startIndex;
		int num2 = num;
		while (data[num] != val)
		{
			num++;
			num2++;
		}
		return num2;
	}

	public static List<FileData> GetCRCData()
	{
		byte[] array = new byte[10240];
		uint crcData = GetCrcData(array, 10240);
		List<FileData> list = null;
		if (crcData <= 10240)
		{
			list = ParseCrcData(array, crcData);
		}
		else
		{
			Debug.LogError("Buffer is too small, causing undefined behavior in DllProtector.");
			DebugLogHandler.ReportError("Buffer is too small, causing undefined behavior in DllProtector.", string.Empty, LogType.Error);
		}
		Debug.Log("Dll returned " + list.Count + " dlls.");
		foreach (FileData item in list)
		{
			Debug.Log(item.ToString() + "\n");
		}
		return list;
	}

	private static List<FileData> ParseCrcData(byte[] crcData, uint numOfWrittenBytes)
	{
		List<FileData> list = new List<FileData>(16);
		int num = 0;
		while (num < numOfWrittenBytes)
		{
			ushort num2 = BitConverter.ToUInt16(crcData, num);
			num += 2;
			byte[] array = new byte[num2];
			Array.Copy(crcData, num, array, 0, num2);
			num += num2;
			uint crc = BitConverter.ToUInt32(crcData, num);
			num += 4;
			list.Add(new FileData(array, crc));
		}
		return list;
	}
}
