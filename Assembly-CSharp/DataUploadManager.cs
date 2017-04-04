using MV.WorldObject;
using UnityEngine;
using UnityEngine.Events;

public static class DataUploadManager
{
	private static int id;

	private static BytePacker bytePacker;

	private static int chunkSize = 5000;

	private static UnityAction doneNotification;

	public static void UploadData(byte[] data, UnityAction doneNotification)
	{
		if (bytePacker == null)
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

	private static void SendChunk()
	{
		int num = chunkSize;
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
}
