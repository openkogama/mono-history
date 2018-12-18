using MV.Common;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.Events;

public static class DataUploadManager
{
	private const int chunkSize = 5000;

	private static int id;

	private static BytePacker bytePacker;

	private static UnityAction doneNotification;

	public static void UploadData(byte[] data, UnityAction doneNotification)
	{
		if (MVGameControllerBase.BuildTarget == BuildTarget.StandAlone)
		{
			Debug.LogWarning("UploadData called even though user is in standalone.");
		}
		else if (bytePacker == null)
		{
			if (MVGameControllerBase.TextureIntegrityChecker.VerifyTextureIntegrity())
			{
				DataUploadManager.doneNotification = doneNotification;
				bytePacker = new BytePacker(data);
				SendChunk();
			}
		}
		else
		{
			Debug.LogError("DataUpload already in progress");
		}
	}

	public static void OnUploadBytes()
	{
		if (bytePacker.Length == bytePacker.Position)
		{
			HandleDone();
		}
		else
		{
			SendChunk();
		}
	}

	public static void Reset()
	{
		id = 0;
		bytePacker = null;
		doneNotification = null;
	}

	private static void SendChunk()
	{
		int num = 5000;
		if (bytePacker.Length - bytePacker.Position < num)
		{
			num = bytePacker.Length - bytePacker.Position;
		}
		MVGameControllerBase.OperationRequests.UploadData(id, bytePacker.ReadBytes(num));
	}

	private static void HandleDone()
	{
		id++;
		bytePacker = null;
		if (doneNotification != null)
		{
			doneNotification();
			doneNotification = null;
		}
	}
}
