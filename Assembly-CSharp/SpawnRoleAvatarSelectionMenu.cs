using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class SpawnRoleAvatarSelectionMenu : MonoBehaviour
{
	[SerializeField]
	private Transform avatarElementContainer;

	[SerializeField]
	private GameObject loadingWheel;

	[SerializeField]
	private SpawnRoleAvatarSelectionElement avatarSelectionElementPrefab;

	private int spawnRoleId;

	private List<MVWorldObjectClient> avatarList = new List<MVWorldObjectClient>();

	public void Initialize(int spawnRoleId)
	{
		this.spawnRoleId = spawnRoleId;
		MVGameControllerBase.Game.ReceivedAvatarBodiesFromQuery += GameOnReceivedAvatarBodiesFromQuery;
		MVGameControllerBase.Game.OperationRequestSender.GetAvatarBodies();
	}

	private void OnDestroy()
	{
		MVGameControllerBase.Game.ReceivedAvatarBodiesFromQuery -= GameOnReceivedAvatarBodiesFromQuery;
		for (int i = 0; i < avatarList.Count; i++)
		{
			Object.Destroy(avatarList[i].GameObject);
		}
	}

	private void GameOnReceivedAvatarBodiesFromQuery(object sender, ReceivedItemFromQueryEventArgs e)
	{
		int num = 0;
		loadingWheel.SetActive(value: false);
		while (e.KoGaMaData.Length != e.KoGaMaData.Position)
		{
			KoGaMaPackageClient koGaMaPackageClient = new KoGaMaPackageClient(e.KoGaMaData, readRuntimeValues: true);
			MVWorldObjectClient mVWorldObjectClient = koGaMaPackageClient.worldObjects[koGaMaPackageClient.worldObjectRoot];
			mVWorldObjectClient.InventoryInitialize();
			avatarList.Add(mVWorldObjectClient);
			int avatarId = (ObscuredInt)koGaMaPackageClient.worldObjects[koGaMaPackageClient.worldObjectRoot].RunTimeData.GetObscuredType("DBId");
			SpawnRoleAvatarSelectionElement spawnRoleAvatarSelectionElement = Object.Instantiate(avatarSelectionElementPrefab);
			spawnRoleAvatarSelectionElement.transform.SetParent(avatarElementContainer, worldPositionStays: false);
			spawnRoleAvatarSelectionElement.Initialize(num, avatarId, OnAvatarSelected);
			spawnRoleAvatarSelectionElement.SetupPreviewImage(mVWorldObjectClient.GameObject);
			num++;
		}
	}

	private void OnAvatarSelected(int avatarId)
	{
		MVGameControllerBase.Game.OperationRequestSender.SetSpawnRoleBody(spawnRoleId, avatarId);
	}
}
