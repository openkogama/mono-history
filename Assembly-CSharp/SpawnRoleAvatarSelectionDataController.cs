using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class SpawnRoleAvatarSelectionDataController : MonoBehaviour, IHandleSpawnRoleAvatarSelectionData, IEventSystemHandler
{
	private List<SpawnRoleAvatarSelectionData> avatarSelectionDataList;

	private UnityAction<List<SpawnRoleAvatarSelectionData>> onDataRecieved;

	public void TryGetSpawnRoleAvatarSelectionData(UnityAction<List<SpawnRoleAvatarSelectionData>> onDataReady)
	{
		if (avatarSelectionDataList == null)
		{
			MVGameControllerBase.Game.ReceivedAvatarBodiesFromQuery += GameOnReceivedAvatarBodiesFromQuery;
			onDataRecieved = (UnityAction<List<SpawnRoleAvatarSelectionData>>)Delegate.Combine(onDataRecieved, onDataReady);
			MVGameControllerBase.Game.OperationRequestSender.GetAvatarBodies();
		}
		else
		{
			onDataReady(avatarSelectionDataList);
		}
	}

	private void OnDestroy()
	{
		if (MVGameControllerBase.IsAlive)
		{
			MVGameControllerBase.Game.ReceivedAvatarBodiesFromQuery -= GameOnReceivedAvatarBodiesFromQuery;
		}
		if (avatarSelectionDataList != null)
		{
			for (int i = 0; i < avatarSelectionDataList.Count; i++)
			{
				avatarSelectionDataList[i].koGaMaPackageClientInventoryItem.Destroy();
			}
		}
	}

	private void GameOnReceivedAvatarBodiesFromQuery(object sender, ReceivedItemFromQueryEventArgs e)
	{
		Debug.LogWarning("GameOnReceivedAvatarBodiesFromQuery");
		MVGameControllerBase.Game.ReceivedAvatarBodiesFromQuery -= GameOnReceivedAvatarBodiesFromQuery;
		avatarSelectionDataList = new List<SpawnRoleAvatarSelectionData>();
		while (e.KoGaMaData.Length != e.KoGaMaData.Position)
		{
			KoGaMaPackageClient koGaMaPackageClient = new KoGaMaPackageClient(e.KoGaMaData, readRuntimeValues: true);
			MVWorldObjectClient mVWorldObjectClient = null;
			try
			{
				koGaMaPackageClient.InventoryInitialize();
				mVWorldObjectClient = koGaMaPackageClient.worldObjects[koGaMaPackageClient.worldObjectRoot];
				mVWorldObjectClient.GameObject.SetActive(value: false);
			}
			catch
			{
				Debug.LogWarning("Failed to initialize inventory for spawn role body selection element. This is most likely because the avatar is hacked and is missing a limb.");
				koGaMaPackageClient.Destroy();
				continue;
			}
			int avatarId = (ObscuredInt)koGaMaPackageClient.worldObjects[koGaMaPackageClient.worldObjectRoot].RunTimeData.GetObscuredType("DBId");
			SpawnRoleAvatarSelectionData item = new SpawnRoleAvatarSelectionData
			{
				avatar = mVWorldObjectClient,
				avatarId = avatarId,
				koGaMaPackageClientInventoryItem = koGaMaPackageClient
			};
			avatarSelectionDataList.Add(item);
		}
		if (onDataRecieved != null)
		{
			onDataRecieved(avatarSelectionDataList);
		}
		onDataRecieved = null;
	}
}
