using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using CodeStage.AntiCheat.ObscuredTypes;
using ExitGames.Client.Photon;
using MV.Common;
using MV.WorldObject;
using MV.WorldObject.RuntimeEvents;
using MV.WorldObject.Security;
using UnityEngine;
using UnityEngine.Events;

public class MVNetworkGame : IPhotonPeerListener
{
	private class EventHandling
	{
		private const float maxJoinTimeValue = 250000f;

		private bool cacheEvents;

		private Queue<EventData> cachedEvents = new Queue<EventData>();

		private MVNetworkGame networkGame;

		public bool CacheEvents
		{
			set
			{
				cacheEvents = value;
			}
		}

		public EventHandling(MVNetworkGame networkGame)
		{
			this.networkGame = networkGame;
		}

		public void UncacheEventsFromJoin()
		{
			cacheEvents = false;
			while (cachedEvents.Count > 0)
			{
				OnEvent(cachedEvents.Dequeue());
			}
		}

		public void OnEvent(EventData photonEvent)
		{
			if (cacheEvents)
			{
				cachedEvents.Enqueue(photonEvent);
				return;
			}
			if (MVGameControllerBase.JoinState != MVJoinState.Playing)
			{
				JoinUIUpdater.UpdateJoinStateForUI((MVEventCodes)photonEvent.Code);
			}
			MVEventCodes code = (MVEventCodes)photonEvent.Code;
			try
			{
				HandleEvent(code, photonEvent);
			}
			catch (Exception innerException)
			{
				Debug.LogException(new Exception("EventFailed Code: " + code, innerException));
			}
		}

		private void HandleEvent(MVEventCodes eventCode, EventData photonEvent)
		{
			switch (eventCode)
			{
			case MVEventCodes.Join:
			{
				int profileID3 = (int)photonEvent[11];
				int num5 = (int)photonEvent[254];
				string userName = (string)photonEvent[9];
				string regionCode = (string)photonEvent[154];
				BuildTarget buildTarget = (BuildTarget)(byte)photonEvent[188];
				MVTeam team2 = (MVTeam)(int)photonEvent[88];
				if (num5 == networkGame.LocalPlayerActorNumber)
				{
					Debug.LogError("Received join event for localPlayerActorNumber");
					break;
				}
				MVPlayer mVPlayer2 = new MVPlayer(num5, profileID3, userName, regionCode, buildTarget);
				mVPlayer2.Team = team2;
				networkGame.AddPlayer(mVPlayer2);
				break;
			}
			case MVEventCodes.GetDBTimeTicks:
			{
				networkGame.localTimeInMillisecondsOnDBTimeSync = networkGame.LocalTimeInMilliSeconds;
				long ticks = (long)photonEvent[132];
				networkGame.dbTimeBase = new DateTime(ticks);
				break;
			}
			case MVEventCodes.RequestMaterials:
				networkGame.OnRequestMaterialsResponse((Dictionary<object, object>)photonEvent[92]);
				break;
			case MVEventCodes.GetPlanetOwnershipTypes:
				networkGame.OnGetPlanetOwnershipTypes((Dictionary<object, object>)photonEvent[1]);
				break;
			case MVEventCodes.GetItemCategories:
				networkGame.OnGetItemCategories((Dictionary<object, object>)photonEvent[1]);
				break;
			case MVEventCodes.RequestStreamingAssetList:
			{
				Dictionary<object, object> list = (Dictionary<object, object>)photonEvent[107];
				networkGame.OnRequestStreamingAssetListResponse(list);
				break;
			}
			case MVEventCodes.CreateGameSnapshot:
			{
				WorldNetwork worldNetwork = networkGame.worldNetwork;
				worldNetwork.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Combine(worldNetwork.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(networkGame.WOCM_InitializedGameQueryDataHandler));
				networkGame.CreatePlayersFromUserList((Dictionary<object, object>)photonEvent[13]);
				networkGame.CreateTeamList((Dictionary<object, object>)photonEvent[89]);
				MVGameStateType gameStateType = (MVGameStateType)(int)photonEvent[63];
				int startTime = (int)photonEvent[65];
				int duration = (int)photonEvent[64];
				byte[] stats = (byte[])photonEvent[158];
				networkGame.gameStatCounterManager.SetStats(stats);
				networkGame.gameStatCounterManager.OnCounterTypeChanged += GameSessionCounterRules.OnCounterTypeChanged;
				networkGame.worldNetwork.WorldInventory.FineGrainedTerrainPrototypeID = (int)photonEvent[156];
				Debug.LogFormat("Initial Server frameCount {0}. Server timeStamp {1}.", (int)photonEvent[205], (int)photonEvent[33]);
				networkGame.networkGameStateListener.ChangeState(gameStateType, startTime, duration, fromGameSnapshot: true);
				networkGame.logicObjectManager = new LogicObjectManager((int)photonEvent[33], (int)photonEvent[205]);
				break;
			}
			case MVEventCodes.GameSnapshotData:
			{
				BytePacker bytePacker = new BytePacker((byte[])photonEvent[245]);
				QueryType queryType = (QueryType)(byte)photonEvent[133];
				bool dataLeft = (bool)photonEvent[99];
				networkGame.HandleGameSnapshotData(bytePacker, queryType, dataLeft);
				break;
			}
			case MVEventCodes.SetActorReady:
				MVGameControllerBase.JoinState = MVJoinState.Playing;
				HandleActorReadyMetric();
				networkGame.gameCoinManager.Reset(networkGame);
				break;
			case MVEventCodes.RequestStreamingAssetInventory:
				networkGame.OnRequestStreamingAssetInventoryResponse((Dictionary<object, object>)photonEvent[108]);
				break;
			case MVEventCodes.RequestFriends:
				networkGame.OnRequestFriendsResponse((Dictionary<object, object>)photonEvent[49]);
				break;
			case MVEventCodes.GetItemInventory:
				networkGame.OnInventoryResultSetResponse((Dictionary<object, object>)photonEvent[245]);
				break;
			case MVEventCodes.GetItemShopInventory:
				networkGame.OnShopInventoryResultSetResponse((Dictionary<object, object>)photonEvent[245], !(bool)photonEvent[7]);
				break;
			case MVEventCodes.GetBuiltInItemBusinessData:
				networkGame.OnGetBuiltInItemBusinessData((Dictionary<object, object>)photonEvent[131]);
				break;
			case MVEventCodes.LargeDBQueryAvatarShopInventory:
				networkGame.OnAvatarShopInventoryResultSetResponse((Dictionary<object, object>)photonEvent[245]);
				break;
			case MVEventCodes.InitializeAvatarEdit:
			{
				byte[] buffer3 = (byte[])photonEvent[164];
				networkGame.avatarMetaDataWoMap = new MvAvatarMetaDataWoMap(new BytePacker(buffer3));
				break;
			}
			case MVEventCodes.GetActiveAvatar:
				networkGame.OnGetActiveAvatarResponse((int)photonEvent[20]);
				break;
			case MVEventCodes.Leave:
			{
				int num4 = (int)photonEvent[254];
				if (num4 != networkGame.LocalPlayer.ActorNr)
				{
					MVPlayer mVPlayer = networkGame.Players[num4];
					Dictionary<object, object> dictionary5 = new Dictionary<object, object>();
					dictionary5[(byte)0] = num4;
					dictionary5[(byte)3] = mVPlayer.Username;
					dictionary5[(byte)6] = MVGameControllerBase.Game.Friends.IsFriend(mVPlayer.ProfileID);
					MVGameControllerBase.PostGameMsg(MVGameMsgType.UserLeft, dictionary5);
					networkGame.Players.Remove(num4);
					networkGame.gameStatCounterManager.RemoveStatsFromActor(num4);
					if (networkGame.onPlayerListChanged != null)
					{
						networkGame.onPlayerListChanged();
					}
				}
				else
				{
					Debug.LogError("Local player leave event");
				}
				break;
			}
			case MVEventCodes.NotificationEvent:
			{
				NotificationType type = (NotificationType)(int)photonEvent[199];
				Dictionary<object, object> data = (Dictionary<object, object>)photonEvent[200];
				networkGame.OnNotificationEventReceived(type, data);
				break;
			}
			case MVEventCodes.UnregisterWorldObject:
				networkGame.OnUnregisterWorldObjectEvent((int)photonEvent[20]);
				break;
			case MVEventCodes.UpdateWorldObject:
				networkGame.OnUpdateWorldObjectEvent(photonEvent);
				break;
			case MVEventCodes.TransferOwnership:
				networkGame.OnTransferOwnershipEvent(photonEvent);
				break;
			case MVEventCodes.UnregisterPrototype:
				networkGame.OnUnregisterPrototypeEvent((int)photonEvent[45]);
				break;
			case MVEventCodes.UpdatePrototype:
				networkGame.worldNetwork.WorldInventory.OnUpdatePrototypeEvent((int)photonEvent[45], (byte[])photonEvent[47]);
				break;
			case MVEventCodes.UpdatePrototypeScale:
				break;
			case MVEventCodes.UpdateWorldObjectData:
				networkGame.worldNetwork.WorldObjectClientManagerNetwork.OnUpdateWorldObjectDataEvent((int)photonEvent[20], (Dictionary<object, object>)photonEvent[16]);
				break;
			case MVEventCodes.UpdateWorldObjectDataPartial:
			{
				int worldObjectID8 = (int)photonEvent[20];
				Dictionary<object, object> worldObjectData = (Dictionary<object, object>)photonEvent[16];
				networkGame.worldNetwork.WorldObjectClientManagerNetwork.OnUpdateWorldObjectDataPartialEvent(worldObjectID8, worldObjectData);
				break;
			}
			case MVEventCodes.RemoveWorldObjectDataPartial:
			{
				int worldObjectID7 = (int)photonEvent[20];
				Dictionary<object, object> worldObjectDataToRemove = (Dictionary<object, object>)photonEvent[17];
				networkGame.worldNetwork.WorldObjectClientManagerNetwork.OnRemoveWorldObjectDataPartialEvent(worldObjectID7, worldObjectDataToRemove);
				break;
			}
			case MVEventCodes.UpdateWorldObjectRunTimeData:
				if ((int)photonEvent[254] != networkGame.LocalPlayer.ActorNr)
				{
					networkGame.worldNetwork.WorldObjectClientManagerNetwork.OnUpdateWorldObjectRunTimeDataEvent((int)photonEvent[20], (Dictionary<object, object>)photonEvent[68]);
				}
				break;
			case MVEventCodes.AddLink:
				networkGame.OnAddLinkEvent((int)photonEvent[55], (int)photonEvent[54], (int)photonEvent[56]);
				break;
			case MVEventCodes.RemoveLink:
				networkGame.OnRemoveLinkEvent((int)photonEvent[56]);
				break;
			case MVEventCodes.AddObjectLink:
				networkGame.OnAddObjectLinkEvent((int)photonEvent[55], (int)photonEvent[54], (int)photonEvent[56]);
				break;
			case MVEventCodes.RemoveObjectLink:
				networkGame.OnRemoveObjectLinkEvent((int)photonEvent[56]);
				break;
			case MVEventCodes.AddItemToInventory:
				networkGame.OnAddItemToInventoryEvent(photonEvent);
				break;
			case MVEventCodes.RemoveItemFromInventory:
				networkGame.OnRemoveItemFromInventory((int)photonEvent[38]);
				break;
			case MVEventCodes.FriendRequest:
			{
				int friendID2 = (int)photonEvent[50];
				int profileID2 = (int)photonEvent[11];
				int friendProfileID = (int)photonEvent[51];
				networkGame.OnFriendRequestEvent(friendID2, profileID2, friendProfileID);
				break;
			}
			case MVEventCodes.FriendUpdate:
			{
				int friendID = (int)photonEvent[50];
				int profileID = (int)photonEvent[11];
				FriendStatus status = (FriendStatus)(int)photonEvent[52];
				networkGame.OnFriendUpdateEvent(friendID, profileID, status);
				break;
			}
			case MVEventCodes.TriggerBoxEnter:
			{
				int worldObjectID6 = (int)photonEvent[20];
				int actorNr3 = (int)photonEvent[254];
				networkGame.OnTriggerBoxEnterEvent(actorNr3, worldObjectID6);
				break;
			}
			case MVEventCodes.TriggerBoxExit:
			{
				int worldObjectID5 = (int)photonEvent[20];
				int actorNr2 = (int)photonEvent[254];
				networkGame.OnTriggerBoxExitEvent(actorNr2, worldObjectID5);
				break;
			}
			case MVEventCodes.TriggerBoxStayBegin:
			{
				int worldObjectID4 = (int)photonEvent[20];
				int actorNr = (int)photonEvent[254];
				networkGame.OnTriggerBoxStayBegin(worldObjectID4, actorNr);
				break;
			}
			case MVEventCodes.TriggerBoxStayEnd:
			{
				int worldObjectID3 = (int)photonEvent[20];
				networkGame.OnTriggerBoxStayEnd(worldObjectID3);
				break;
			}
			case MVEventCodes.Ungroup:
				Debug.Log("Ungroup event...");
				networkGame.OnUngroupEvent(photonEvent);
				break;
			case MVEventCodes.LockHierarchy:
				networkGame.OnLockHierarchyEvent(photonEvent);
				break;
			case MVEventCodes.WoUniquePrototype:
				networkGame.OnWoUniquePrototypeEvent((int)photonEvent[20], (int)photonEvent[45]);
				break;
			case MVEventCodes.GameStateChange:
				Debug.Log("GameStateChange event " + (MVGameStateType)(int)photonEvent[63]);
				networkGame.networkGameStateListener.ChangeState((MVGameStateType)(int)photonEvent[63], (int)photonEvent[65], (int)photonEvent[64], fromGameSnapshot: false);
				break;
			case MVEventCodes.PropertiesChanged:
			{
				int num3 = (int)photonEvent[253];
				Dictionary<object, object> dictionary4 = (Dictionary<object, object>)photonEvent[251];
				Debug.Log("ACTOR-NR: " + num3);
				Debug.Log(networkGame.Players[num3].Username);
				{
					foreach (string key in dictionary4.Keys)
					{
						Debug.Log(key + ": " + dictionary4[key]);
					}
					break;
				}
			}
			case MVEventCodes.ResetLogicChunk:
				networkGame.OnResetLogicChunkEvent((int)photonEvent[20]);
				break;
			case MVEventCodes.PickupItemStateChange:
				networkGame.OnPickupItemStateChangeEvent((PickupItemState)(int)photonEvent[69], (int)photonEvent[20], (int)photonEvent[254]);
				break;
			case MVEventCodes.UpdateLineOfFire:
			{
				Vector3 camOrigin = new Vector3((float)photonEvent[72], (float)photonEvent[73], (float)photonEvent[74]);
				Vector3 camDir = new Vector3((float)photonEvent[75], (float)photonEvent[76], (float)photonEvent[77]);
				networkGame.OnUpdateLineOfFire((int)photonEvent[20], camOrigin, camDir);
				break;
			}
			case MVEventCodes.WorldObjectRPCEvent:
				networkGame.OnWorldObjectRPCEvent(photonEvent);
				break;
			case MVEventCodes.PostGameMsgEvent:
				MVGameControllerBase.PostGameMsg((MVGameMsgType)(int)photonEvent[86], (Dictionary<object, object>)photonEvent[87]);
				break;
			case MVEventCodes.SetTeam:
				networkGame.OnSetTeamEvent((int)photonEvent[254], (MVTeam)(int)Enum.ToObject(typeof(MVTeam), (int)photonEvent[88]));
				break;
			case MVEventCodes.AddTeam:
				networkGame.OnAddTeamEvent((MVTeam)(int)Enum.ToObject(typeof(MVTeam), (int)photonEvent[88]));
				break;
			case MVEventCodes.RemoveTeam:
				networkGame.OnRemoveTeamEvent((MVTeam)(int)Enum.ToObject(typeof(MVTeam), (int)photonEvent[88]), (Dictionary<object, object>)photonEvent[245]);
				break;
			case MVEventCodes.TransferWorldObjectsToGroup:
				networkGame.OnTransferWorldObjectsToGroup(photonEvent);
				break;
			case MVEventCodes.CloneWorldObjectTree:
				networkGame.OnCloneWorldObjectTree(photonEvent);
				break;
			case MVEventCodes.CloneWorldObjectTreeWithPosition:
				networkGame.OnCloneWorldObjectTreePosition(photonEvent);
				break;
			case MVEventCodes.CloneTempWorldObjectWithOriginalReferenceEvent:
				networkGame.OnCloneTempWorldObjectWithOriginalReferenceEvent(photonEvent);
				break;
			case MVEventCodes.GetGameBatch:
				networkGame.OnGetGameBatch(photonEvent);
				break;
			case MVEventCodes.GameQueryReady:
				networkGame.OnGameQueryReady(photonEvent);
				break;
			case MVEventCodes.PostWinnerReport:
				networkGame.OnPostWinnerReportEvent();
				break;
			case MVEventCodes.CollectiblePickedUp:
				networkGame.OnCollectiblePickedUp(photonEvent);
				break;
			case MVEventCodes.SetWorldObjectsToPurchasedEvent:
				networkGame.OnSetWorldObjectsToPurchasedEvent((int)photonEvent[11], (int)photonEvent[38]);
				break;
			case MVEventCodes.AchievementUnlockedEvent:
				Debug.Log($"Profile with ID {(int)photonEvent[11]} unlocked Achievement {(AchievementType)(int)photonEvent[128]}");
				break;
			case MVEventCodes.AttachWorldObjectToSeat:
			{
				Dictionary<object, object> dictionary3 = (Dictionary<object, object>)photonEvent[70];
				int seatOwnerWoID = (int)dictionary3[(byte)4];
				int worldObjectID2 = (int)dictionary3[(byte)0];
				networkGame.PlayerController.OnAttachWorldObjectToSeat((int)photonEvent[254], seatOwnerWoID, worldObjectID2, (byte)photonEvent[141]);
				break;
			}
			case MVEventCodes.DetachWorldObjectFromVehicle:
			{
				int id2 = (int)photonEvent[20];
				MVWorldObjectClient worldObjectClient2 = networkGame.WorldObjectClientManager.GetWorldObjectClient(id2);
				if (worldObjectClient2 != null && worldObjectClient2 is MVAvatar)
				{
					((MVAvatar)worldObjectClient2).OnLeaveVehicle();
				}
				break;
			}
			case MVEventCodes.SpawnVehicleWithDriver:
			{
				Dictionary<object, object> dictionary2 = (Dictionary<object, object>)photonEvent[70];
				int id = (int)dictionary2[(byte)1];
				int worldObjectID = (int)dictionary2[(byte)0];
				MVWorldObjectSpawnerVehicle mVWorldObjectSpawnerVehicle = (MVWorldObjectSpawnerVehicle)networkGame.WorldObjectClientManager.GetWorldObjectClient(id);
				int spawnWorldObjectID = mVWorldObjectSpawnerVehicle.SpawnWorldObjectID;
				int num2 = (int)dictionary2[(byte)3];
				int ownerActorNumber = (int)photonEvent[254];
				int cloneLinkId = (int)photonEvent[56];
				int cloneObjectLinkId = (int)photonEvent[91];
				int takeTime = (int)photonEvent[33];
				networkGame.worldNetwork.OnCloneWorldObjectTreeEvent(ownerActorNumber, 0, cloneToRootGroup: true, spawnWorldObjectID, num2, cloneLinkId, cloneObjectLinkId);
				MVWorldObjectClient worldObjectClient = networkGame.WorldObjectClientManager.GetWorldObjectClient(num2);
				MVWorldObjectClient.CallBackDelegate callBack = (MVWorldObjectClient wo) =>
				{
					wo.InteractionFlags = InteractionFlags.None;
				};
				worldObjectClient.TraverseRecursiveTail(callBack);
				networkGame.PlayerController.OnAttachWorldObjectToSeat((int)photonEvent[254], num2, worldObjectID, (byte)photonEvent[141]);
				mVWorldObjectSpawnerVehicle.Take(takeTime);
				break;
			}
			case MVEventCodes.Reward:
			{
				int num = (int)photonEvent[143];
				RewardReason rewardReason = (RewardReason)(byte)photonEvent[145];
				RewardType rewardType = (RewardType)(int)photonEvent[144];
				Debug.Log($"Amount {num}, rewardReason {rewardReason}, rewardType {rewardType} ");
				BrowserComm.ToJavaScript.ExternalCall("refreshCredentials");
				break;
			}
			case MVEventCodes.RuntimeEvent:
			{
				byte[] buffer2 = (byte[])photonEvent[245];
				networkGame.worldNetwork.RuntimeEventManagerNetwork.HandleRuntimeEvent(RuntimeEvent.Create(new BytePacker(buffer2)));
				break;
			}
			case MVEventCodes.ResetTerrainEvent:
				networkGame.worldNetwork.RuntimeEventManagerNetwork.ResetTerrain();
				break;
			case MVEventCodes.UpdateGameStat:
			{
				int actorNumber = (int)photonEvent[254];
				MVTeam team = (MVTeam)(int)photonEvent[88];
				GameStatCounterType counterType = (GameStatCounterType)(byte)photonEvent[159];
				int value = (int)photonEvent[160];
				int otherID = (int)photonEvent[161];
				bool includeTeamScore = (bool)photonEvent[162];
				if ((bool)photonEvent[163])
				{
					networkGame.gameStatCounterManager.Increment(counterType, team, actorNumber, value, otherID, includeTeamScore);
				}
				else
				{
					networkGame.gameStatCounterManager.Update(counterType, actorNumber, team, value, otherID, includeTeamScore);
				}
				break;
			}
			case MVEventCodes.UpdateGameStatType:
			{
				byte[] stat = (byte[])photonEvent[158];
				networkGame.gameStatCounterManager.SetStat(stat);
				break;
			}
			case MVEventCodes.UpdateAvatarMetaData:
			{
				int woID = (int)photonEvent[20];
				byte[] buffer = (byte[])photonEvent[165];
				MvAvatarMetaData mvAvatarMetaData = new MvAvatarMetaData(new BytePacker(buffer));
				Debug.Log(mvAvatarMetaData);
				networkGame.avatarMetaDataWoMap.Add(woID, mvAvatarMetaData);
				break;
			}
			case MVEventCodes.LevelChanged:
				networkGame.OnLevelChanged((int)photonEvent[254], (int)photonEvent[169]);
				break;
			case MVEventCodes.XPRewarded:
				networkGame.OnXPRewarded((int)photonEvent[254], (byte)photonEvent[84]);
				break;
			case MVEventCodes.GameBoostEvent:
			{
				int boostLeft = (int)photonEvent[179];
				bool boostEnabled = (bool)photonEvent[183];
				networkGame.gameCoinManager.OnGameBoostChanged(boostLeft, boostEnabled);
				break;
			}
			case MVEventCodes.PendingByteDataBatch:
				networkGame.OnGetGameBatch(photonEvent);
				break;
			case MVEventCodes.SyncronizePing:
				MVGameControllerBase.OperationRequests.SyncronizePing();
				break;
			case MVEventCodes.SwitchAvatar:
				networkGame.OnSwitchAvatar(photonEvent);
				break;
			case MVEventCodes.StartRewardCountDown:
			{
				int countDownTimeInMs = (int)photonEvent[194];
				int countDownStartTick = (int)photonEvent[33];
				RewardManager.SetCountDownValues(countDownTimeInMs, countDownStartTick);
				break;
			}
			case MVEventCodes.RewardIsReady:
				RewardManager.SetNumberOfPendingRewards((int)photonEvent[195]);
				break;
			case MVEventCodes.JoinNotification:
			{
				Debug.Log("MVEventCodes.JoinNotification");
				Dictionary<object, object> dictionary = new Dictionary<object, object>();
				dictionary.Add((byte)0, (int)photonEvent[254]);
				Dictionary<object, object> gameMsgData = dictionary;
				MVGameControllerBase.PostGameMsg(MVGameMsgType.UserJoined, gameMsgData);
				break;
			}
			case MVEventCodes.GodzillaEnter:
			{
				int[] array = (int[])photonEvent[70];
				if (MVGameControllerBase.WOCM.GetWorldObjectClient(array[0]) is GodzillaTrigger godzillaTrigger2)
				{
					godzillaTrigger2.OccupationChange(array[1]);
				}
				break;
			}
			case MVEventCodes.GodzillaExit:
				if (MVGameControllerBase.WOCM.GetWorldObjectClient((int)photonEvent[20]) is GodzillaTrigger godzillaTrigger)
				{
					godzillaTrigger.OccupationChange(-1);
				}
				break;
			case MVEventCodes.LogicFrame:
				networkGame.logicObjectManager.Update();
				break;
			case MVEventCodes.LogicObjectFiringStateChange:
				((IIsLogicObjectFiringEventHandler)MVGameControllerBase.WOCM.GetWorldObjectClient((int)photonEvent[20])).OnIsFiringChanged((bool)photonEvent[204]);
				break;
			case MVEventCodes.RandomBoxIndex:
				((MVRandomBox)MVGameControllerBase.WOCM.GetWorldObjectClient((int)photonEvent[20])).SetRandomIndex((int)photonEvent[206]);
				break;
			default:
				Debug.LogError("Unknown event: " + eventCode);
				break;
			}
		}

		private void HandleActorReadyMetric()
		{
			StatHatWrapper.Count(MVJoinState.Playing.ToString(), 1);
			double totalMilliseconds = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
			if (MVGameControllerBase.LoadStats.DOMReady > 0.0)
			{
				float num = (float)(totalMilliseconds - MVGameControllerBase.LoadStats.DOMReady);
				Debug.Log("CompleteJoinTime " + num);
				if (num > 0f && num < 250000f)
				{
					StatHatWrapper.Value("CompleteJoinTime", num);
					StatHatWrapper.Value("CompleteJoinTime." + MVGameControllerBase.GameMode, num);
				}
			}
			if (MVGameControllerBase.LoadStats.PluginInit > 0.0)
			{
				float num2 = (float)(totalMilliseconds - MVGameControllerBase.LoadStats.PluginInit);
				Debug.Log("JoinAndInitializationTime " + num2);
				if (num2 > 0f && num2 < 250000f)
				{
					StatHatWrapper.Value("JoinAndInitializationTime", num2);
					StatHatWrapper.Value("JoinAndInitializationTime." + MVGameControllerBase.GameMode, num2);
				}
			}
			if (MVGameControllerBase.LoadStats.GameStartTime > 0.0)
			{
				float num3 = (float)(totalMilliseconds - MVGameControllerBase.LoadStats.GameStartTime);
				if (num3 > 0f && num3 < 250000f)
				{
					StatHatWrapper.Value("JoinTime", num3);
					StatHatWrapper.Value("JoinTime." + MVGameControllerBase.GameMode, num3);
				}
			}
			if (MVGameControllerBase.IsTouristSession)
			{
				StatHatWrapper.Count("SessionType.Tourist" + networkGame.gameType, 1);
			}
			else
			{
				StatHatWrapper.Count("SessionType." + networkGame.gameType, 1);
			}
		}
	}

	private class GameDataQueryManager
	{
		public class GameDataQuery
		{
			private BytePacker bp;

			private int instigatorActorNumber;

			public QueryType QueryType { get; private set; }

			public int InstigatorActorNumber => instigatorActorNumber;

			public GameDataQuery(BytePacker bp, int instigatorActorNumber, QueryType queryType)
			{
				this.bp = bp;
				this.instigatorActorNumber = instigatorActorNumber;
				QueryType = queryType;
			}

			public override string ToString()
			{
				return $"[GameDataQuery: InstigatorActorNumber={InstigatorActorNumber}, BPLength={bp.Length}]";
			}

			public void AddGameDataQuery(GameDataQuery gameDataQuery)
			{
				bp.Position = bp.Length;
				bp.Write(gameDataQuery.GetBytePacker().ToArray());
			}

			public BytePacker GetBytePacker()
			{
				bp.Position = 0;
				return bp;
			}
		}

		private readonly Dictionary<int, GameDataQuery> gameDataQueries = new Dictionary<int, GameDataQuery>();

		public void HandleDataBatch(int instigator, int queryId, QueryType queryType, bool queryDataLeft, BytePacker bp)
		{
			Debug.Log("instigator " + instigator);
			GameDataQuery gameDataQuery = new GameDataQuery(bp, instigator, queryType);
			OnGetGameBatch(queryId, gameDataQuery);
			if (!queryDataLeft)
			{
				OnGameQueryReady(queryId);
			}
		}

		public void OnGameQueryReady(int queryId)
		{
			GameDataQuery gameDataQuery = gameDataQueries[queryId];
			InitializeGameQueryData(gameDataQuery);
		}

		private void OnGetGameBatch(int queryId, GameDataQuery gameDataQuery)
		{
			if (gameDataQueries.ContainsKey(queryId))
			{
				gameDataQueries[queryId].AddGameDataQuery(gameDataQuery);
			}
			else
			{
				gameDataQueries.Add(queryId, gameDataQuery);
			}
		}

		private void InitializeGameQueryData(GameDataQuery gameDataQuery)
		{
			switch (gameDataQuery.QueryType)
			{
			case QueryType.AddToGameWorld:
				MVGameControllerBase.Game.worldNetwork.AddGameQueryDataToGameWorld(gameDataQuery.GetBytePacker(), gameDataQuery.InstigatorActorNumber);
				break;
			case QueryType.Item:
				if (MVGameControllerBase.Game.ReceivedItemFromQuery != null)
				{
					ReceivedItemFromQueryEventArgs e = new ReceivedItemFromQueryEventArgs(gameDataQuery.GetBytePacker(), gameDataQuery.InstigatorActorNumber);
					MVGameControllerBase.Game.ReceivedItemFromQuery(this, e);
				}
				break;
			}
		}
	}

	public class OperationRequests
	{
		private OperationResponsePendingManager operationResponsePendingManager;

		private MVNetworkGame networkGame;

		private PhotonPeer peer;

		public OperationRequests(MVNetworkGame networkGame)
		{
			this.networkGame = networkGame;
			peer = networkGame.peer;
			operationResponsePendingManager = new OperationResponsePendingManager(peer);
		}

		public void UploadData(int id, byte[] uploadData)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(191, id);
			dictionary.Add(245, uploadData);
			peer.OpCustom(77, dictionary, sendReliable: true);
		}

		public void TryRemovePendingOperation(MVOperationCodes operationCode)
		{
			operationResponsePendingManager.TryRemovePendingOperation(operationCode);
		}

		public void Syncronize()
		{
			peer.OpCustom(68, new Dictionary<byte, object>(), sendReliable: true);
		}

		public void SyncronizePing()
		{
			peer.OpCustom(70, new Dictionary<byte, object>(), sendReliable: true);
		}

		public void GetRewardList()
		{
			peer.OpCustom(71, new Dictionary<byte, object>(), sendReliable: true);
		}

		public void GetRewardIndex()
		{
			peer.OpCustom(72, new Dictionary<byte, object>(), sendReliable: true);
		}

		public void ClaimReward()
		{
			peer.OpCustom(73, new Dictionary<byte, object>(), sendReliable: true);
		}

		public void GetOffer()
		{
			peer.OpCustom(74, new Dictionary<byte, object>(), sendReliable: true);
		}

		public void ClaimOffer()
		{
			peer.OpCustom(75, new Dictionary<byte, object>(), sendReliable: true);
		}

		public void AddObjectLink(ObjectLink link)
		{
			LogicObjectManager.ValidateObjectLinkStatus validateObjectLinkStatus = LogicObjectManager.ValidateObjectLink(link, MVGameControllerBase.WOCM, out var reportSeverity);
			if (validateObjectLinkStatus != LogicObjectManager.ValidateObjectLinkStatus.Ok)
			{
				if (reportSeverity == LogicObjectManager.ReportSeverity.Error)
				{
					Debug.LogError("Link invalid and rejected. Reason: " + validateObjectLinkStatus);
				}
				if (reportSeverity == LogicObjectManager.ReportSeverity.Info || reportSeverity == LogicObjectManager.ReportSeverity.Warning)
				{
					Debug.LogWarning("Link invalid and rejected. Reason: " + validateObjectLinkStatus);
				}
			}
			else
			{
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(55, link.objectConnectorWOID);
				dictionary.Add(54, link.objectWOID);
				peer.OpCustom(35, dictionary, sendReliable: true);
			}
		}

		public bool PublishPlanet(ref string errorText)
		{
			if (MVGameControllerBase.Game == null || MVGameControllerBase.JoinState != MVJoinState.Playing || MVGameControllerBase.GameSessionData.gameMode != MVGameMode.Edit)
			{
				errorText = TM._("Error publishing game, try again in a few moments.");
				return false;
			}
			if (IsOperationPending(MVOperationCodes.PublishPlanet))
			{
				Debug.LogWarning("Publish planet operation is pending. Aborting publish");
				errorText = TM._("You are already publishing planet, please wait.");
				return false;
			}
			if (networkGame.LocalPlayer.Level < networkGame.PublishLevel)
			{
				errorText = TM._("You can not publish game before reaching level: " + networkGame.PublishLevel);
				return false;
			}
			if (!networkGame.isPublished)
			{
				if (GenerateTextureData.IsCreatingScreenShot)
				{
					Debug.LogWarning("Texture is already being generated. Aborting publish");
					errorText = TM._("You are already publishing planet, please wait.");
					return false;
				}
				GeneratePlanetScreenShot(HandlePublishAndScreenShotData);
			}
			else
			{
				PublishPlanet(newImagePending: false);
			}
			return true;
		}

		private void HandlePublishAndScreenShotData(byte[] screenshot)
		{
			DataUploadManager.UploadData(screenshot, () =>
			{
				PublishPlanet(newImagePending: true);
			});
		}

		private bool PublishPlanet(bool newImagePending)
		{
			if (newImagePending)
			{
				MVGameControllerBase.Game.MaterialRepository.Validate();
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(62, newImagePending);
			return operationResponsePendingManager.AddOperationCodeToPending(MVOperationCodes.PublishPlanet, dictionary);
		}

		public void UploadGameScreenShot()
		{
			if (MVGameControllerBase.Game.LocalPlayer.PlanetOwnershipTypeID != 2)
			{
				Debug.LogWarning("No screen shot when not planet owner");
			}
			else if (GenerateTextureData.IsCreatingScreenShot)
			{
				Debug.LogWarning("Texture is already being generated. Aborting UploadGameScreenShot");
			}
			else
			{
				GeneratePlanetScreenShot(HandleUploadScreenShotData);
			}
		}

		private void HandleUploadScreenShotData(byte[] screenshot)
		{
			DataUploadManager.UploadData(screenshot, () =>
			{
				UploadScreenshot(ImageType.Planet);
			});
		}

		public bool IsOperationPending(MVOperationCodes operationCode)
		{
			return operationResponsePendingManager.IsOperationPending(operationCode);
		}

		public void RequestAcceptFriendShip(int friendID)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(50, friendID);
			peer.OpCustom(17, dictionary, sendReliable: true);
		}

		public void RequestRejectFriendShip(int friendID)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(50, friendID);
			peer.OpCustom(18, dictionary, sendReliable: true);
		}

		public void RequestWoUniquePrototype(int woId)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(20, woId);
			peer.OpCustom(25, dictionary, sendReliable: true);
		}

		public void JoinGame()
		{
			networkGame.ConnState = MVConnState.Joining;
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			if (MVGameControllerBase.GameSessionData.gameMode == MVGameMode.CharacterEditor)
			{
				Debug.Log("Setting planetId to -1 as GameMode CharacterEditor is not using a planet");
			}
			dictionary.Add(byte.MaxValue, MVGameControllerBase.GameSessionData.planetName);
			dictionary.Add(85, MVGameControllerBase.GameSessionData.planetID);
			dictionary.Add(115, MVGameControllerBase.GameSessionData.gameMode);
			dictionary.Add(154, MVGameControllerBase.GameSessionData.language);
			dictionary.Add(167, MVGameControllerBase.GameSessionData.token);
			dictionary.Add(171, MVGameControllerBase.GameSessionData.newToken);
			dictionary.Add(172, MVGameControllerBase.GameSessionData.newPlanetName);
			dictionary.Add(188, MVGameControllerBase.BuildTarget);
			peer.OpCustom(byte.MaxValue, dictionary, sendReliable: true);
		}

		public bool AddLink(Link link)
		{
			LogicObjectManager.ValidateLinkStatus validateLinkStatus = LogicObjectManager.ValidateLink(link.outputWOID, link.inputWOID, MVGameControllerBase.WOCM, out var reportSeverity);
			if (validateLinkStatus != LogicObjectManager.ValidateLinkStatus.Ok)
			{
				if (reportSeverity == LogicObjectManager.ReportSeverity.Error)
				{
					Debug.LogError("Link invalid and rejected. Reason: " + validateLinkStatus);
				}
				if (reportSeverity == LogicObjectManager.ReportSeverity.Info || reportSeverity == LogicObjectManager.ReportSeverity.Warning)
				{
					Debug.LogWarning("Link invalid and rejected. Reason: " + validateLinkStatus);
				}
				return false;
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(55, link.outputWOID);
			dictionary.Add(54, link.inputWOID);
			peer.OpCustom(9, dictionary, sendReliable: true);
			return true;
		}

		public void UpdateWorldObject(int id, Vector3 position, byte[] rotation, TransformPackageType packageType)
		{
			if (MVGameControllerBase.JoinState == MVJoinState.Playing)
			{
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(20, id);
				dictionary.Add(33, networkGame.ServerTimeInMilliSeconds);
				TransformHelper.SetPosition(position, dictionary);
				dictionary.Add(157, rotation);
				dictionary.Add(34, (byte)packageType);
				bool sendReliable = TransformPackageType.Stop == packageType;
				peer.OpCustom(2, dictionary, sendReliable);
			}
		}

		public void UpdateLineOfFire(int worldObjectIDPickupOwner, Vector3 camDir, Vector3 camOrigin)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(20, worldObjectIDPickupOwner);
			dictionary.Add(72, camOrigin.x);
			dictionary.Add(73, camOrigin.y);
			dictionary.Add(74, camOrigin.z);
			dictionary.Add(75, camDir.x);
			dictionary.Add(76, camDir.y);
			dictionary.Add(77, camDir.z);
			peer.OpCustom(30, dictionary, sendReliable: false);
		}

		public void TransferWorldObjectsToGroup(int groupId, int[] worldObjects)
		{
			if (worldObjects.Contains(groupId))
			{
				Debug.LogError("Trying to transfer WO " + groupId + " to itself !");
				return;
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(20, groupId);
			dictionary.Add(70, worldObjects);
			peer.OpCustom(37, dictionary, sendReliable: true);
		}

		public void TransferOwnership(int worldObjectID, int ownerActorNr, Transform t)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(20, worldObjectID);
			dictionary.Add(18, ownerActorNr);
			if (t == null)
			{
				dictionary.Add(82, false);
			}
			else
			{
				dictionary.Add(82, true);
				TransformHelper.SetPosition(t.localPosition, dictionary);
				TransformHelper.SetRotation(t.localRotation, dictionary);
			}
			peer.OpCustom(6, dictionary, sendReliable: true);
		}

		public void PostGameMsg(MVGameMsgType gameMsgType, Dictionary<object, object> gameMsgData)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(86, (int)gameMsgType);
			dictionary.Add(87, gameMsgData);
			peer.OpCustom(33, dictionary, sendReliable: true);
		}

		public void PostNotificationOperation(NotificationType type, Dictionary<object, object> notificationData)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(199, type);
			dictionary.Add(200, notificationData);
			Dictionary<byte, object> customOpParameters = dictionary;
			peer.OpCustom(78, customOpParameters, sendReliable: true);
		}

		public void LockHierarchy(int worldObjectID, bool lockHierarchy)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(20, worldObjectID);
			dictionary.Add(61, lockHierarchy);
			peer.OpCustom(22, dictionary, sendReliable: true);
		}

		public bool RequestFriendShipByName(string name, ref string errorText)
		{
			if (name != networkGame.LocalPlayer.Username)
			{
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(9, name);
				peer.OpCustom(15, dictionary, sendReliable: true);
				return true;
			}
			errorText = TM._("Can't request friendship from yourself!");
			return false;
		}

		public void UpdateWorldObjectData(int worldObjectID, Dictionary<object, object> worldObjectData)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(20, worldObjectID);
			dictionary.Add(16, worldObjectData);
			peer.OpCustom(3, dictionary, sendReliable: true);
		}

		public void UpdateWorldObjectDataPartial(int worldObjectID, string keyPath, object value)
		{
			string[] array = keyPath.Split(new char[1] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length == 0)
			{
				Debug.LogError("Trying to update WO with an empty path key");
			}
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add(array[array.Length - 1], value);
			int num = array.Length - 2;
			while (0 <= num)
			{
				Dictionary<object, object> dictionary2 = new Dictionary<object, object>();
				dictionary2[array[num]] = dictionary;
				dictionary = dictionary2;
				num--;
			}
			Dictionary<byte, object> dictionary3 = new Dictionary<byte, object>();
			dictionary3.Add(20, worldObjectID);
			dictionary3.Add(16, dictionary);
			peer.OpCustom(4, dictionary3, sendReliable: true);
		}

		public void UpdateWorldObjectDataPartial(int worldObjectID, Dictionary<object, object> woData)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(20, worldObjectID);
			dictionary.Add(16, woData);
			peer.OpCustom(4, dictionary, sendReliable: true);
		}

		public void RemoveWorldObjectDataPartial(int worldObjectID, string keyPath)
		{
			string[] array = keyPath.Split(new char[1] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length == 0)
			{
				Debug.LogError("Trying to remoe WO data with an empty path key");
			}
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			Dictionary<object, object> dictionary2 = dictionary;
			for (int i = 0; i < array.Length; i++)
			{
				if (i < array.Length - 1)
				{
					Dictionary<object, object> dictionary3 = new Dictionary<object, object>();
					dictionary2[array[i]] = dictionary3;
					dictionary2 = dictionary3;
				}
				else
				{
					dictionary2[array[i]] = string.Empty;
				}
			}
			Dictionary<byte, object> dictionary4 = new Dictionary<byte, object>();
			dictionary4.Add(20, worldObjectID);
			dictionary4.Add(17, dictionary);
			peer.OpCustom(5, dictionary4, sendReliable: true);
		}

		public void RemoveWorldObjectDataPartial(int worldObjectID, Dictionary<object, object> woDataToRemove)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(20, worldObjectID);
			dictionary.Add(17, woDataToRemove);
			peer.OpCustom(5, dictionary, sendReliable: true);
		}

		public void WorldObjectRPC(int worldObjectID, Dictionary<object, object> dataPackage)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(20, worldObjectID);
			dictionary.Add(81, dataPackage);
			peer.OpCustom(31, dictionary, sendReliable: true);
		}

		public void UpdateWorldObjectRunTimeData(int worldObjectID, Dictionary<object, object> worldObjectRunTimeData)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(20, worldObjectID);
			dictionary.Add(68, worldObjectRunTimeData);
			peer.OpCustom(28, dictionary, sendReliable: true);
		}

		public void RegisterWorldObject(WorldObjectType type, int groupId, Dictionary<object, object> woData, Vector3 position, Quaternion rotation, Vector3 scale, bool localOwner, bool transferOwnershipToServerOnLeave)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(15, type);
			dictionary.Add(21, groupId);
			dictionary.Add(16, woData);
			dictionary.Add(18, localOwner ? networkGame.LocalPlayerActorNumber : 0);
			dictionary.Add(37, transferOwnershipToServerOnLeave);
			TransformHelper.SetPosition(position, dictionary);
			TransformHelper.SetRotation(rotation, dictionary);
			TransformHelper.SetScale(scale, dictionary);
			peer.OpCustom(0, dictionary, sendReliable: true);
		}

		public void RequestBuiltInItem(BuiltInItem builtInItem, int groupId, Dictionary<object, object> customData, Vector3 position, Quaternion rotation, Vector3 scale, bool localOwner, bool transferOwnershipToServerOnLeave)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(114, builtInItem);
			dictionary.Add(21, groupId);
			dictionary.Add(245, customData);
			dictionary.Add(18, localOwner ? networkGame.LocalPlayerActorNumber : 0);
			dictionary.Add(37, transferOwnershipToServerOnLeave);
			TransformHelper.SetPosition(position, dictionary);
			TransformHelper.SetRotation(rotation, dictionary);
			TransformHelper.SetScale(scale, dictionary);
			peer.OpCustom(45, dictionary, sendReliable: true);
		}

		public void AddItemToWorld(int itemId, int groupId, Vector3 position, Quaternion rotation, Vector3 scale, bool localOwner, bool transferOwnershipToServerOnLeave, bool isPreviewItem)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(38, itemId);
			dictionary.Add(21, groupId);
			dictionary.Add(18, localOwner ? networkGame.LocalPlayerActorNumber : 0);
			dictionary.Add(37, transferOwnershipToServerOnLeave);
			TransformHelper.SetPosition(position, dictionary);
			TransformHelper.SetRotation(rotation, dictionary);
			TransformHelper.SetScale(scale, dictionary);
			dictionary.Add(124, isPreviewItem);
			peer.OpCustom(46, dictionary, sendReliable: true);
		}

		public void CloneWorldObjectTree(MVWorldObjectClient root, bool localOwner, bool setAsPreviewItem, bool cloneToRootGroup)
		{
			Dictionary<byte, object> customOpParameters = CreateBasicCloneData(root, localOwner, setAsPreviewItem, cloneToRootGroup);
			peer.OpCustom(38, customOpParameters, sendReliable: true);
		}

		public void CloneWorldObjectTreeWithPosition(MVWorldObjectClient root, Vector3 position, Quaternion rotation, bool localOwner, bool setAsPreviewItem, bool cloneToRootGroup, bool isTempObject)
		{
			Dictionary<byte, object> customOpParameters = CreateWithPositionCloneData(root, position, rotation, localOwner, setAsPreviewItem, cloneToRootGroup, isTempObject);
			peer.OpCustom(79, customOpParameters, sendReliable: true);
		}

		public void CloneTempWorldObjectWithOriginalReference(MVWorldObjectClient root, Vector3 position, Quaternion rotation)
		{
			Dictionary<byte, object> customOpParameters = CreateWithPositionCloneData(root, position, rotation, localOwner: true, setAsPreviewItem: false, cloneToRootGroup: true, isTempObject: true);
			peer.OpCustom(80, customOpParameters, sendReliable: true);
		}

		private Dictionary<byte, object> CreateBasicCloneData(MVWorldObjectClient root, bool localOwner, bool setAsPreviewItem, bool cloneToRootGroup)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(20, root.Id);
			dictionary.Add(18, localOwner ? networkGame.LocalPlayerActorNumber : 0);
			dictionary.Add(100, cloneToRootGroup);
			dictionary.Add(126, setAsPreviewItem);
			return dictionary;
		}

		private Dictionary<byte, object> CreateWithPositionCloneData(MVWorldObjectClient root, Vector3 position, Quaternion rotation, bool localOwner, bool setAsPreviewItem, bool cloneToRootGroup, bool isTempObject)
		{
			Dictionary<byte, object> dictionary = CreateBasicCloneData(root, localOwner, setAsPreviewItem, cloneToRootGroup);
			dictionary.Add(203, isTempObject);
			TransformHelper.SetPosition(position, dictionary);
			TransformHelper.SetRotation(rotation, dictionary);
			return dictionary;
		}

		public void AddPlanetToPlanet(int planetId, int subtreeId)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(85, planetId);
			dictionary.Add(20, subtreeId);
			peer.OpCustom(39, dictionary, sendReliable: true);
		}

		public void UnregisterWorldObject(int worldObjectID)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(20, worldObjectID);
			peer.OpCustom(1, dictionary, sendReliable: true);
		}

		public void LocalPlayerLevelChanged(int level)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(169, level);
			peer.OpCustom(64, dictionary, sendReliable: true);
		}

		public void JoinNotification()
		{
			if (!MVGameControllerBase.Game.LocalPlayer.IsAnonymous)
			{
				Dictionary<object, object> dictionary = new Dictionary<object, object>();
				dictionary.Add((byte)9, MVGameControllerBase.Game.LocalPlayer.ActorNr);
				dictionary.Add((byte)12, MVGameControllerBase.Game.LocalPlayer.RegionCode);
				Dictionary<object, object> value = dictionary;
				Dictionary<byte, object> dictionary2 = new Dictionary<byte, object>();
				dictionary2.Add(199, NotificationType.PlayerJoined);
				dictionary2.Add(200, value);
				Dictionary<byte, object> customOpParameters = dictionary2;
				peer.OpCustom(78, customOpParameters, sendReliable: true);
			}
		}

		public void WonRareReward(IActorRewardClient reward, int rewardAmount)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add((byte)9, MVGameControllerBase.Game.LocalPlayer.ActorNr);
			dictionary.Add((byte)11, (byte)reward.RewardRarity);
			dictionary.Add((byte)5, reward.RewardType);
			dictionary.Add((byte)4, rewardAmount);
			Dictionary<object, object> value = dictionary;
			Dictionary<byte, object> dictionary2 = new Dictionary<byte, object>();
			dictionary2.Add(199, NotificationType.WonRareSpinReward);
			dictionary2.Add(200, value);
			Dictionary<byte, object> customOpParameters = dictionary2;
			peer.OpCustom(78, customOpParameters, sendReliable: true);
		}

		public void Ban(CheatType cheatType)
		{
			if (!MVGameControllerBase.IsTouristSession)
			{
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(178, (byte)cheatType);
				peer.OpCustom(66, dictionary, sendReliable: true);
				peer.SendOutgoingCommands();
			}
		}

		public void AutoRegisterLocalPrototype(int woId, int worldInventoryID)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(45, worldInventoryID);
			dictionary.Add(20, woId);
			peer.OpCustom(25, dictionary, sendReliable: true);
		}

		public void UpdatePrototype(int worldInventoryID, byte[] prototypeData)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(45, worldInventoryID);
			dictionary.Add(47, prototypeData);
			peer.OpCustom(7, dictionary, sendReliable: true);
		}

		public void UpdatePrototypeScale(int worldInventoryID, float scale)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(45, worldInventoryID);
			dictionary.Add(32, scale);
			peer.OpCustom(8, dictionary, sendReliable: true);
		}

		public void AddWorldObjectToInventory(int worldObjectID)
		{
			MVGameControllerBase.Game.MaterialRepository.Validate();
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(20, worldObjectID);
			peer.OpCustom(47, dictionary, sendReliable: true);
		}

		public void RemoveItemFromInventory(int itemID)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(38, itemID);
			peer.OpCustom(13, dictionary, sendReliable: true);
		}

		public void UpdateInventorySlots(Dictionary<object, object> itemIdToSlotIndexTable)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(44, itemIdToSlotIndexTable);
			peer.OpCustom(14, dictionary, sendReliable: true);
		}

		public void SwitchAvatar()
		{
			peer.OpCustom(69, new Dictionary<byte, object>(), sendReliable: true);
		}

		public void SendClientLog(string logString, string stackTrace, LogType type, Dictionary<string, object> extraSentryData, Dictionary<string, string> tags)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(146, logString);
			dictionary.Add(147, stackTrace);
			dictionary.Add(148, (byte)type);
			dictionary.Add(153, extraSentryData);
			dictionary.Add(187, tags);
			peer.OpCustom(58, dictionary, sendReliable: true);
			peer.SendOutgoingCommands();
		}

		public void Ungroup(int worldObjectID)
		{
			if (!networkGame.worldNetwork.WorldObjectClientManagerNetwork.Ungroup(worldObjectID))
			{
				Debug.LogWarning("Ungroup failed");
				return;
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(20, worldObjectID);
			peer.OpCustom(21, dictionary, sendReliable: true);
		}

		public void ReportCaptureFlag()
		{
			peer.OpCustom(26, new Dictionary<byte, object>(), sendReliable: true);
		}

		public void ResetLogicChunk(int worldObjectID)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(20, worldObjectID);
			peer.OpCustom(27, dictionary, sendReliable: true);
		}

		public bool RequestFriendShipByID(int id, ref string errorText)
		{
			if (id != networkGame.LocalPlayer.ProfileID)
			{
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(51, id);
				peer.OpCustom(16, dictionary, sendReliable: true);
				return true;
			}
			errorText = TM._("Can't request friendship from yourself!");
			return false;
		}

		public void RequestMarketPlaceItem(int itemID)
		{
			Debug.Log("MVOperationCodes.GetMarketPlaceItem");
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(38, itemID);
			peer.OpCustom(51, dictionary, sendReliable: true);
		}

		public void RequestAddItemToMarketPlace(int itemID, string itemName, string itemDescription, int silverPrice)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(38, itemID);
			dictionary.Add(40, itemName);
			dictionary.Add(134, itemDescription);
			dictionary.Add(67, silverPrice);
			peer.OpCustom(52, dictionary, sendReliable: true);
		}

		public void RequestRemoveItemFromMarketPlace(int itemID)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(38, itemID);
			peer.OpCustom(53, dictionary, sendReliable: true);
		}

		public void RequestLargeDBQuery(MVOperationCodes operationCode, DBQuery query, Dictionary<object, object> inData, int numRowsPerReturn)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(0, (byte)query);
			dictionary.Add(2, inData);
			dictionary.Add(6, numRowsPerReturn);
			peer.OpCustom((byte)operationCode, dictionary, sendReliable: true);
		}

		public void RequestResetTerrain()
		{
			Dictionary<byte, object> customOpParameters = new Dictionary<byte, object>();
			peer.OpCustom(61, customOpParameters, sendReliable: true);
		}

		public void SendRuntimeEventOperation(RuntimeEvent runtimeEvent)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(245, runtimeEvent.Data);
			peer.OpCustom(60, dictionary, sendReliable: true);
		}

		public void SetTeam(MVTeam team)
		{
			if (MVGameControllerBase.Game.TeamManager.IsTeamActive(team))
			{
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(88, (int)team);
				peer.OpCustom(34, dictionary, sendReliable: true);
			}
		}

		public void AttachWorldObjectToSeat(int seatOwnerWoID, int worldObjectID, VehicleSeatBase seatBase)
		{
			Dictionary<byte, object> attachWorldObjectToSeatData = networkGame.GetAttachWorldObjectToSeatData(seatBase);
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add((byte)4, seatOwnerWoID);
			dictionary.Add((byte)0, worldObjectID);
			attachWorldObjectToSeatData.Add(70, dictionary);
			peer.OpCustom(55, attachWorldObjectToSeatData, sendReliable: true);
		}

		public void AddAvatarToAvatarShopInventory(int worldObjectId, int priceSilver, string name)
		{
			networkGame.MaterialRepository.Validate();
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(20, worldObjectId);
			dictionary.Add(130, priceSilver);
			dictionary.Add(166, name);
			peer.OpCustom(62, dictionary, sendReliable: true);
		}

		public void DeleteAvatarFromShopInventory(int worldObjectId)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(20, worldObjectId);
			peer.OpCustom(63, dictionary, sendReliable: true);
		}

		public void SpawnVehicleWithDriver(int worldObjectSpawnerVehicleID, int worldObjectID, VehicleSeatBase seatBase)
		{
			Dictionary<byte, object> attachWorldObjectToSeatData = networkGame.GetAttachWorldObjectToSeatData(seatBase);
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add((byte)1, worldObjectSpawnerVehicleID);
			dictionary.Add((byte)0, worldObjectID);
			attachWorldObjectToSeatData.Add(70, dictionary);
			peer.OpCustom(57, attachWorldObjectToSeatData, sendReliable: true);
		}

		public void DetachWorldObjectFromVehicle(int worldObjectID)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(20, worldObjectID);
			peer.OpCustom(56, dictionary, sendReliable: true);
		}

		public void SetAvatarAccessorySlot(int avatarBodyWoID, int accessoryInventoryID, AvatarAccessorySlot slot, float slotOffset)
		{
			Debug.Log("Set accessory slot");
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary[125] = avatarBodyWoID;
			dictionary[110] = accessoryInventoryID;
			dictionary[112] = slot;
			dictionary[113] = slotOffset;
			peer.OpCustom(54, dictionary, sendReliable: true);
		}

		public void ResetAvatar(int AvatarID)
		{
			Debug.Log("Reset ActiveAvatar  called");
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(125, AvatarID);
			peer.OpCustom(50, dictionary, sendReliable: true);
		}

		public void SetActiveAvatar(int AvatarID)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(125, AvatarID);
			peer.OpCustom(49, dictionary, sendReliable: true);
		}

		public void ExpireAvatarAccessory(int avatarAccessoryInventoryID, int bodyWoID = 0)
		{
			Debug.LogWarning("Expire accessory " + avatarAccessoryInventoryID + " on body " + bodyWoID);
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary[(byte)105] = StreamingAssetType.AvatarAccessory;
			if (bodyWoID != 0)
			{
				dictionary[(byte)20] = bodyWoID;
			}
			ExpireProduct(MVProductType.StreamingAsset, avatarAccessoryInventoryID, dictionary);
		}

		public void ExpireStreamingAsset(StreamingAssetType assetType, int streamingAssetInventoryID)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add((byte)105, assetType);
			Dictionary<object, object> expireProductData = dictionary;
			ExpireProduct(MVProductType.StreamingAsset, streamingAssetInventoryID, expireProductData);
		}

		public void UpdateAvatarAccessoryOffset(int bodyWoID, AvatarAccessorySlot slot, float offset)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary[125] = bodyWoID;
			dictionary[112] = (int)slot;
			dictionary[113] = offset;
			peer.OpCustom(59, dictionary, sendReliable: true);
		}

		public void RentAvatarAccessory(int streamingAssetID, int bodyWoID, AvatarAccessorySlot slot, float offset)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary[(byte)105] = StreamingAssetType.AvatarAccessory;
			dictionary[(byte)104] = streamingAssetID;
			dictionary[(byte)20] = bodyWoID;
			dictionary[(byte)112] = slot;
			dictionary[(byte)113] = offset;
			RentProduct(MVProductType.StreamingAsset, dictionary);
		}

		public void RentAvatarAccessory(int streamingAssetID)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary[(byte)105] = StreamingAssetType.AvatarAccessory;
			dictionary[(byte)104] = streamingAssetID;
			RentProduct(MVProductType.StreamingAsset, dictionary);
		}

		public void ExtendRentAvatarAccessory(int streamingAssetID, int streamingAssetInventoryID)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary[(byte)105] = StreamingAssetType.AvatarAccessory;
			dictionary[(byte)104] = streamingAssetID;
			dictionary[(byte)110] = streamingAssetInventoryID;
			RentProduct(MVProductType.StreamingAsset, dictionary);
		}

		public void SetGameCoinBoostState(bool gameCoinBoosterEnabled)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(183, gameCoinBoosterEnabled);
			peer.OpCustom(67, dictionary, sendReliable: true);
		}

		public void PurchaseMysteryBoxSpins(int numberOfSpins)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add((byte)143, numberOfSpins);
			Dictionary<byte, object> dictionary2 = new Dictionary<byte, object>();
			dictionary2.Add(93, 7);
			dictionary2.Add(94, dictionary);
			peer.OpCustom(40, dictionary2, sendReliable: true);
		}

		public void ExtendRentAvatarAccessory(int streamingAssetID, int streamingAssetInventoryID, int bodyWoID, AvatarAccessorySlot slot, float offset)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary[(byte)105] = StreamingAssetType.AvatarAccessory;
			dictionary[(byte)104] = streamingAssetID;
			dictionary[(byte)110] = streamingAssetInventoryID;
			dictionary[(byte)20] = bodyWoID;
			dictionary[(byte)112] = slot;
			dictionary[(byte)113] = offset;
			RentProduct(MVProductType.StreamingAsset, dictionary);
		}

		public void RentStreamingAsset(StreamingAssetType assetType, int assetID, int streamingAssetInventoryID = 0)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary[(byte)105] = assetType;
			dictionary[(byte)104] = assetID;
			dictionary[(byte)110] = streamingAssetInventoryID;
			RentProduct(MVProductType.StreamingAsset, dictionary);
		}

		public void PurchaseStreamingAsset(StreamingAssetType assetType, int streamingAssetID)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary[(byte)104] = streamingAssetID;
			dictionary[(byte)105] = assetType;
			PurchaseProduct(MVProductType.StreamingAsset, dictionary);
		}

		public void UnlockMaterial(int materialID)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add((byte)102, materialID);
			PurchaseProduct(MVProductType.MaterialUnlock, dictionary);
		}

		public void UnlockClientShopInventoryItem(int itemId)
		{
			Debug.Log("Purchase item with id: " + itemId);
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add((byte)9, itemId);
			PurchaseProduct(MVProductType.Item, dictionary);
		}

		public void PurchaseAvatar(int avatarId)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add((byte)125, avatarId);
			PurchaseProduct(MVProductType.Avatar, dictionary);
		}

		public void PurchaseRespawnNow()
		{
			Debug.Log("Purchase respawn now");
			Dictionary<object, object> productData = new Dictionary<object, object>();
			PurchaseProduct(MVProductType.RespawnNow, productData);
		}

		public void PurchaseGameCoinBooster()
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add((byte)185, PricesManager.GetPrice("GameCoinBoost").ToArray());
			PurchaseProduct(MVProductType.GameCoinBooster, dictionary);
		}

		public void RemoveLink(int linkID)
		{
			if (!networkGame.worldNetwork.LinksContains(linkID))
			{
				Debug.LogWarning("RemoveLink: Link not found");
				return;
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(56, linkID);
			peer.OpCustom(10, dictionary, sendReliable: true);
		}

		public void RemoveObjectLink(int objectLinkID)
		{
			if (!networkGame.worldNetwork.ObjectLinksContains(objectLinkID))
			{
				Debug.LogWarning("RemoveObjectLink: ObjectLink not found");
				return;
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(56, objectLinkID);
			peer.OpCustom(36, dictionary, sendReliable: true);
		}

		public void TriggerBoxEnter(int triggerBoxOwnerId, int triggerInstigatorId)
		{
			if (MVGameControllerBase.JoinState == MVJoinState.Playing)
			{
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(20, new int[2] { triggerBoxOwnerId, triggerInstigatorId });
				peer.OpCustom(19, dictionary, sendReliable: true);
			}
		}

		public void TriggerBoxExit(int triggerBoxOwnerId, int triggerInstigatorId)
		{
			if (MVGameControllerBase.JoinState == MVJoinState.Playing)
			{
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(20, new int[2] { triggerBoxOwnerId, triggerInstigatorId });
				peer.OpCustom(20, dictionary, sendReliable: true);
			}
		}

		public bool UploadScreenshot(ImageType imageType)
		{
			MVGameControllerBase.Game.MaterialRepository.Validate();
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(116, (byte)imageType);
			return operationResponsePendingManager.AddOperationCodeToPending(MVOperationCodes.UploadScreenshot, dictionary);
		}

		public void PurchaseItem(int itemID, int worldObjectID)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(38, itemID);
			dictionary.Add(20, worldObjectID);
			peer.OpCustom(29, dictionary, sendReliable: true);
		}

		public void PurchaseAvatarAccessory(int streamingAssetID)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary[(byte)105] = StreamingAssetType.AvatarAccessory;
			dictionary[(byte)104] = streamingAssetID;
			PurchaseProduct(MVProductType.StreamingAsset, dictionary);
		}

		public void LogicActivateRequest(int woID, bool activate)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(20, woID);
			dictionary.Add(204, activate);
			peer.OpCustom(81, dictionary, sendReliable: true);
		}

		private void PurchaseProduct(MVProductType productTypeID, Dictionary<object, object> productData)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(93, (int)productTypeID);
			dictionary.Add(94, productData);
			peer.OpCustom(40, dictionary, sendReliable: true);
		}

		private void RentProduct(MVProductType productTypeID, Dictionary<object, object> productData)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(93, (int)productTypeID);
			dictionary.Add(95, productData);
			peer.OpCustom(41, dictionary, sendReliable: true);
		}

		private void ExpireProduct(MVProductType productTypeID, int productInventoryID, Dictionary<object, object> expireProductData)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary[93] = (int)productTypeID;
			dictionary[136] = productInventoryID;
			dictionary[96] = expireProductData;
			peer.OpCustom(42, dictionary, sendReliable: true);
		}
	}

	private class OperationResponseHandling
	{
		private MVNetworkGame networkGame;

		public OperationResponseHandling(MVNetworkGame networkGame)
		{
			this.networkGame = networkGame;
		}

		public void HandleOperationResponse(OperationResponse operationResponse)
		{
			short returnCode = operationResponse.ReturnCode;
			MVOperationCodes operationCode = (MVOperationCodes)operationResponse.OperationCode;
			Dictionary<byte, object> parameters = operationResponse.Parameters;
			try
			{
				ExecuteOperationResponse(operationCode, parameters, returnCode);
			}
			catch (Exception innerException)
			{
				Debug.LogException(new Exception("OperationResponse failed. OperationCode: " + operationCode, innerException));
			}
		}

		private void ExecuteOperationResponse(MVOperationCodes opCode, Dictionary<byte, object> returnValues, short returnCode)
		{
			switch (opCode)
			{
			case MVOperationCodes.Join:
				networkGame.ConnState = MVConnState.Joined;
				switch (returnCode)
				{
				case 0:
					networkGame.OnJoinResponse(returnValues);
					break;
				case -12:
					if (MVGameControllerBase.TryReauth())
					{
						break;
					}
					goto default;
				default:
					Debug.Log("Quiting from join because of of ping not ok and out of reauth");
					MVGameControllerBase.ApplicationQuit(new QuitConnectionError());
					break;
				}
				break;
			case MVOperationCodes.PublishPlanet:
			{
				string empty = string.Empty;
				switch (returnCode)
				{
				case 0:
					empty = TM._("Successfully published planet");
					networkGame.isPublished = true;
					break;
				case -1:
					empty = TM._("Undefined fail!");
					break;
				case -2:
					empty = TM._("You are not authorized to publish this planet");
					break;
				default:
					empty = TM._("Unhandled returnCode");
					break;
				}
				if (networkGame.OnPublishedPlanet != null)
				{
					networkGame.OnPublishedPlanet(empty);
				}
				if (!string.IsNullOrEmpty(MVGameControllerBase.GameSessionData.gamePublishedURL))
				{
					WWWForm wWWForm = new WWWForm();
					wWWForm.AddField("token", MVGameControllerBase.GameSessionData.token);
					wWWForm.AddField("profile_id", MVGameControllerBase.GameSessionData.profileID);
					wWWForm.AddField("planet_id", MVGameControllerBase.GameSessionData.planetID);
					AsyncWWWManager.WWWRequest(new PostRequest(MVGameControllerBase.GameSessionData.gamePublishedURL, wWWForm, null, WWWRequestPriority.ExecuteWhileSyncronizing));
				}
				else
				{
					Debug.LogWarning("s.gamePublishedURL is null or empty string");
				}
				break;
			}
			case MVOperationCodes.UnregisterWorldObject:
				if (returnCode != 0)
				{
					Debug.LogWarning("Failed to unregister worldObject");
				}
				else
				{
					networkGame.OnUnregisterWorldObjectResponse((int)returnValues[20]);
				}
				break;
			case MVOperationCodes.UpdateWorldObject:
				break;
			case MVOperationCodes.UpdateWorldObjectData:
				if (returnCode != 0)
				{
					Debug.LogError("UpdateWorldObjectData FAILED on server!");
				}
				break;
			case MVOperationCodes.UpdateWorldObjectRunTimeData:
				if (returnCode != 0)
				{
					Debug.LogError("UpdateWorldObjectRunTimeData FAILED on server!");
				}
				break;
			case MVOperationCodes.TransferOwnership:
				networkGame.OnTransferOwnershipResponse(returnValues, returnCode);
				break;
			case MVOperationCodes.AddWorldObjectToInventory:
				if (networkGame.OnAddWorldObjectToInventoryResponse != null)
				{
					networkGame.OnAddWorldObjectToInventoryResponse(returnCode, (int)returnValues[67], (int)returnValues[38], (int)returnValues[20]);
				}
				else
				{
					Debug.LogWarning("OnAddWorldObjectToInventoryResponse called with no subscribers, this is unhandled but possibly ok.");
				}
				break;
			case MVOperationCodes.AddWorldObjectToInventoryDev:
				if (returnCode == 0)
				{
					networkGame.OnAddWorldObjectToInventoryResponseDev(returnCode, (int)returnValues[20], (int)returnValues[38]);
				}
				break;
			case MVOperationCodes.RequestFriendshipByName:
				networkGame.OnRequestFriendshipResponse(returnCode);
				break;
			case MVOperationCodes.RequestFriendshipByProfileID:
				networkGame.OnRequestFriendshipResponse(returnCode);
				break;
			case MVOperationCodes.Ungroup:
				networkGame.OnUngroupResponse(returnCode == 0);
				break;
			case MVOperationCodes.LockHierarchy:
				networkGame.OnLockHierarchyResponse(returnValues, returnCode);
				break;
			case MVOperationCodes.RequestWoUniquePrototype:
				if (returnCode != 0)
				{
					networkGame.OnRequestWoUniquePrototypeFailed(returnValues);
				}
				break;
			case MVOperationCodes.PurchaseItem:
				if (networkGame.OnPurchaseItemResponse != null)
				{
					networkGame.OnPurchaseItemResponse(returnCode);
				}
				break;
			case MVOperationCodes.TransferWorldObjectsToGroup:
				networkGame.worldNetwork.WorldObjectClientManagerNetwork.HandleTransferWorldObjectsToGroup(returnCode == 0);
				break;
			case MVOperationCodes.PurchaseProduct:
			{
				Dictionary<object, object> purchaseResponseData = null;
				if (returnValues.ContainsKey(94))
				{
					purchaseResponseData = (Dictionary<object, object>)returnValues[94];
				}
				networkGame.OnPurchaseProductResponse(returnCode, purchaseResponseData);
				break;
			}
			case MVOperationCodes.RentProduct:
			{
				Dictionary<object, object> arg = null;
				if (returnValues.ContainsKey(95))
				{
					arg = (Dictionary<object, object>)returnValues[95];
				}
				networkGame.PurchaseProductResponseHandler(returnCode, arg);
				break;
			}
			case MVOperationCodes.ExpireProduct:
				networkGame.OnExpireProductResponse(returnCode, returnValues);
				break;
			case MVOperationCodes.CloneWorldObjectTree:
			case MVOperationCodes.CloneWorldObjectTreeWithPosition:
			{
				int rootId = (int)returnValues[20];
				networkGame.worldNetwork.WorldObjectClientManagerNetwork.OnCloneWorldObjectTreeResponse(returnCode == 0, rootId);
				break;
			}
			case MVOperationCodes.RequestStreamingAssetInventoryItems:
				networkGame.OnRequestStreamingAssetInventoryItemsResponse(returnCode, returnValues);
				break;
			case MVOperationCodes.UploadScreenshot:
				if (networkGame.ScreenshotUploaded != null)
				{
					networkGame.ScreenshotUploaded(this, new ScreenshotUploadedEventArgs(returnCode == 0));
				}
				break;
			case MVOperationCodes.AddItemToMarketPlace:
				if (returnCode == 0)
				{
					int itemID = (int)returnValues[38];
					int shopInventoryID = (int)returnValues[135];
					MVGameControllerBase.IEditModeUI.PlayerInventoryRepository.UpdateShopInventoryID(itemID, shopInventoryID);
				}
				else
				{
					Debug.LogWarning("Failed to add item to marketPlace");
				}
				if (networkGame.OnMarketPlaceActionComplete != null)
				{
					networkGame.OnMarketPlaceActionComplete(returnCode == 0);
				}
				break;
			case MVOperationCodes.RemoveItemFromMarketPlace:
				if (networkGame.OnMarketPlaceActionComplete != null)
				{
					networkGame.OnMarketPlaceActionComplete(returnCode == 0);
				}
				break;
			case MVOperationCodes.SetAvatarAccessorySlot:
				if (networkGame.OnSetAvatarAccessoryResponse != null)
				{
					networkGame.OnSetAvatarAccessoryResponse(returnCode == 0);
				}
				if (returnCode != 0)
				{
					Debug.LogError("SetAvatarAccessorySlot operation failed");
				}
				break;
			case MVOperationCodes.AttachWorldObjectToSeat:
				networkGame.PlayerController.HandleAttachWorldObjectToSeat(returnCode == 0);
				break;
			case MVOperationCodes.DetachWorldObjectFromVehicle:
				networkGame.PlayerController.HandleDetachWorldObjectFromVehicle(returnCode == 0);
				break;
			case MVOperationCodes.SpawnVehicleWithDriver:
				networkGame.PlayerController.HandleAttachWorldObjectToSeat(returnCode == 0);
				break;
			case MVOperationCodes.AddItemToWorld:
				if (networkGame.OnItemAddedToWorld != null)
				{
					networkGame.OnItemAddedToWorld(returnCode == -1);
				}
				break;
			case MVOperationCodes.AddAvatarToAvatarShopInventory:
				if (networkGame.OnMarketPlaceActionComplete != null)
				{
					networkGame.OnMarketPlaceActionComplete(returnCode == 0);
				}
				break;
			case MVOperationCodes.DeleteAvatarFromShopInventory:
				if (networkGame.OnMarketPlaceActionComplete != null)
				{
					networkGame.OnMarketPlaceActionComplete(returnCode == 0);
				}
				break;
			case MVOperationCodes.GetRewardList:
			{
				int spinPrice = (int)returnValues[185];
				int[] array = (int[])returnValues[192];
				string[] array2 = (string[])returnValues[193];
				List<KeyValuePair<int, string>> list = new List<KeyValuePair<int, string>>();
				for (int i = 0; i < array.Length; i++)
				{
					list.Add(new KeyValuePair<int, string>(array[i], array2[i]));
				}
				RewardManager.Initialize(list, spinPrice);
				break;
			}
			case MVOperationCodes.GetRewardIndex:
				RewardManager.SetRewardIndex((int)returnValues[196]);
				break;
			case MVOperationCodes.ClaimReward:
				if (returnCode == 0)
				{
					RewardManager.OnClaimReward();
				}
				else
				{
					Debug.LogError("Failed to claim");
				}
				break;
			case MVOperationCodes.GetActorOffer:
			{
				Debug.Log("MVOperationCodes.GetActorOffer");
				ActorOfferType actorOfferType = (ActorOfferType)(byte)returnValues[197];
				string jsonData = (string)returnValues[198];
				OffersManager.UpdateCurrentOffer(actorOfferType, jsonData);
				break;
			}
			case MVOperationCodes.UploadBytes:
				DataUploadManager.OnUploadBytes();
				break;
			default:
				Debug.LogWarning("Unhandled operation code " + opCode);
				break;
			}
		}
	}

	private class StatusChangedHandling
	{
		private bool registeredFatalStatusCodeInStatHat;

		private MVNetworkGame networkGame;

		public StatusChangedHandling(MVNetworkGame networkGame)
		{
			this.networkGame = networkGame;
		}

		private void HandleDisconnectMetric(StatusCode returnCode)
		{
			if (!registeredFatalStatusCodeInStatHat && ((returnCode != StatusCode.Connect && returnCode != StatusCode.QueueIncomingReliableWarning && returnCode != StatusCode.QueueIncomingUnreliableWarning && returnCode != StatusCode.QueueOutgoingAcksWarning && returnCode != StatusCode.QueueOutgoingReliableWarning && returnCode != StatusCode.QueueOutgoingUnreliableWarning && returnCode != StatusCode.QueueSentWarning && returnCode != StatusCode.Disconnect) || (returnCode == StatusCode.Disconnect && !MVGameControllerBase.DisconnectIsOk)))
			{
				registeredFatalStatusCodeInStatHat = true;
				StatHatWrapper.Count("StatusCode." + returnCode, 1);
				DebugLogHandler.ForceExtraErrorReport();
				Debug.LogError("Client disconnected " + returnCode);
			}
		}

		public void OnStatusChanged(StatusCode returnCode)
		{
			Debug.Log("PeerStatusCallback():" + returnCode);
			HandleDisconnectMetric(returnCode);
			switch (returnCode)
			{
			case StatusCode.Connect:
				MVGameControllerBase.OperationRequests.JoinGame();
				break;
			case StatusCode.Disconnect:
				networkGame.ConnState = MVConnState.DisconnectedByUser;
				Debug.LogWarning("Expecting that this disconnect is done by quiting");
				break;
			case StatusCode.SecurityExceptionOnConnect:
			case StatusCode.ExceptionOnConnect:
			case StatusCode.Exception:
			case StatusCode.SendError:
			case StatusCode.ExceptionOnReceive:
			case StatusCode.TimeoutDisconnect:
			case StatusCode.DisconnectByServer:
			case StatusCode.DisconnectByServerUserLimit:
			case StatusCode.DisconnectByServerLogic:
				Debug.Log("Disconnected bacause: " + returnCode);
				if (networkGame.ConnState != MVConnState.DisconnectedByUser)
				{
					networkGame.ConnState = MVConnState.Disconnected;
					Coroutines.Start(WaitForFrames.Frames(5, () =>
					{
						MVGameControllerBase.ApplicationQuit(new QuitConnectionError());
					}));
				}
				break;
			default:
				Debug.LogWarning("Unhandled PeerStatusCallback, returnCode: " + returnCode);
				break;
			}
		}
	}

	public delegate void OnPlayerListChangedDelegate();

	public delegate void OnReceivedChatMessageDelegate(MVPlayer sender, string message);

	public delegate void OnReceivedXPDelegate(byte xpId, int actorNumber);

	public delegate void OnMarketPlaceActionCompleteDelegate(bool success);

	private const string appName = "MVGameServer";

	private const float serviceCallInterval = 0.07f;

	private const float ExpirationStateCheckPeriod = 1f;

	private const float ExpirationDataFetchCheckPeriod = 15f;

	private PhotonPeer peer;

	private MVConnState connState;

	private MVGameType gameType;

	private SessionTime sessionTime;

	private MVItemBusinessLogic itemBusinessLogic = new MVItemBusinessLogic();

	private MVNetworkGameStateListener networkGameStateListener;

	private ItemCategories itemCategories;

	private bool isPublished;

	private GameDataQueryManager gameDataQueryManager = new GameDataQueryManager();

	private TransformNetworkManager transformNetworkManager = new TransformNetworkManager();

	private LogicObjectManager logicObjectManager;

	private MVGameCoinManager gameCoinManager;

	private int lastFrameServerTimeUpdate = -1;

	private int lastFrameLocalTimeUpdate = -1;

	private int serverTimeInMilliseconds;

	private int localTimeInMilliseconds;

	private DateTime dbTimeBase;

	private int localTimeInMillisecondsOnDBTimeSync;

	public Action<int, Dictionary<object, object>> PurchaseProductResponseHandler;

	public Action<IWinningCondition> OnWinningCondition;

	public Action<int> OnActiveAvatar;

	public Action<bool> OnItemAddedToWorld;

	public Action<int, int, int, int> OnAddWorldObjectToInventoryResponse;

	public UnityAction<int> OnPurchaseItemResponse;

	public UnityAction<string> OnPublishedPlanet;

	public UnityAction<string> OnAddWorldObjectToInventoryCallbackDev;

	public UnityAction OnFinishedLoadingPlayers;

	public Action<bool> OnSetAvatarAccessoryResponse;

	public OnPlayerListChangedDelegate onPlayerListChanged;

	public OnReceivedChatMessageDelegate OnReceivedChatMessage;

	public OnReceivedXPDelegate OnReceivedXP;

	public OnMarketPlaceActionCompleteDelegate OnMarketPlaceActionComplete;

	private RuntimeVariableNetworkManager runtimeVariableNetworkManager = new RuntimeVariableNetworkManager();

	private float prevServiceCallTime;

	private float lastExpirationStateCheck;

	private float lastDataFetchCheck;

	private HashSet<int> ownedRequestedIDs = new HashSet<int>();

	private HashSet<int> ownedRequestFailedIDs = new HashSet<int>();

	private HashSet<int> expiredStreamingAssetIDs = new HashSet<int>();

	private MVLocalObjectController playerController;

	private PlayerRepository playerRepository;

	private ShopRepository shopRepository;

	private AvatarRepository avatarShopRepository;

	private Dictionary<int, MVPlayer> players;

	private MVTeamManager teamManager = new MVTeamManager();

	private GameStatCounterManager gameStatCounterManager = new GameStatCounterManager();

	private WinningConditionManager winningConditionManager;

	private FriendList friends;

	private MVMaterialRepository materialRepository;

	private int localPlayerActorNumber = -1;

	private WorldNetwork worldNetwork;

	private MvAvatarMetaDataWoMap avatarMetaDataWoMap;

	private GameDataQueryManager.GameDataQuery gameDataQuery;

	private EventHandling eventHandling;

	private OperationRequests operationRequests;

	private OperationResponseHandling operationResponseHandling;

	private StatusChangedHandling statusChangedHandling;

	public LogicObjectManager LogicObjectManager => logicObjectManager;

	public MVGameType GameType => gameType;

	public MVItemBusinessLogic ItemBusinessLogic => itemBusinessLogic;

	public MVGameCoinManager GameCoinManager => gameCoinManager;

	public ItemCategories ItemCategories => itemCategories;

	public SessionTime SessionTime => sessionTime;

	public MVConnState ConnState
	{
		get
		{
			return connState;
		}
		set
		{
			connState = value;
		}
	}

	public bool IsPlaying => MVGameControllerBase.IsPlaying;

	public MVNetworkGameStateListener NetworkGameStateListener => networkGameStateListener;

	public PhotonPeer Peer => peer;

	public ObscuredString XpKey { get; private set; }

	public int MarketPlaceLevel { get; private set; }

	public int PublishLevel { get; private set; }

	public int ServerTimeInMilliSeconds
	{
		get
		{
			if (lastFrameServerTimeUpdate != Time.frameCount)
			{
				serverTimeInMilliseconds = Peer.ServerTimeInMilliSeconds;
				lastFrameServerTimeUpdate = Time.frameCount;
			}
			return serverTimeInMilliseconds;
		}
	}

	public int LocalTimeInMilliSeconds
	{
		get
		{
			if (lastFrameLocalTimeUpdate != Time.frameCount)
			{
				localTimeInMilliseconds = SupportClass.GetTickCount();
				lastFrameLocalTimeUpdate = Time.frameCount;
			}
			return localTimeInMilliseconds;
		}
	}

	public DateTime DBTime => dbTimeBase.AddMilliseconds(LocalTimeInMilliSeconds - localTimeInMillisecondsOnDBTimeSync);

	public Dictionary<int, StreamingAssetInfo> StreamingAssetInfoMap { get; private set; }

	public StreamingAssetShopInventory StreamingAssetShopInventory { get; private set; }

	public StreamingAssetInventory StreamingAssetInventory { get; private set; }

	public InventoryExpirationChecker StreamingAssetExpirationChecker { get; private set; }

	public TransformNetworkManager TransformNetworkManager => transformNetworkManager;

	public OperationRequests OperationRequestSender => operationRequests;

	public RuntimeVariableNetworkManager RuntimeVariableNetworkManager => runtimeVariableNetworkManager;

	public MVLocalPlayer LocalPlayer
	{
		get
		{
			players.TryGetValue(localPlayerActorNumber, out var value);
			return (MVLocalPlayer)value;
		}
	}

	public int LocalPlayerActorNumber
	{
		get
		{
			return localPlayerActorNumber;
		}
		set
		{
			if (localPlayerActorNumber == -1)
			{
				localPlayerActorNumber = value;
			}
		}
	}

	public MVMaterialRepository MaterialRepository => materialRepository;

	public PlayerRepository PlayerRepository => playerRepository;

	public ShopRepository ShopRepository => shopRepository;

	public AvatarRepository AvatarShopRepository => avatarShopRepository;

	public MvAvatarMetaDataWoMap AvatarMetaDataWoMap => avatarMetaDataWoMap;

	public MVTeamManager TeamManager => teamManager;

	public MVGameModeChangeNotifier GameStateController { get; private set; }

	public FriendList Friends => friends;

	public MVLocalObjectController PlayerController => playerController;

	public Dictionary<int, MVPlayer> Players => players;

	public GameStatCounterManager GameStatCounterManager => gameStatCounterManager;

	public WinningConditionManager WinningConditionManager => winningConditionManager;

	public MVWorldObjectClientManager WorldObjectClientManager
	{
		get
		{
			if (worldNetwork == null)
			{
				return null;
			}
			return worldNetwork.WorldObjectClientManager;
		}
	}

	public World World => worldNetwork;

	public event EventHandler<ScreenshotUploadedEventArgs> ScreenshotUploaded;

	public event EventHandler<ProductsExpiringEventArgs> StreamingAssetsExpired;

	public event EventHandler<ReceivedItemFromQueryEventArgs> ReceivedItemFromQuery;

	public MVNetworkGame()
	{
		MVGameControllerBase.JoinState = MVJoinState.Joining;
		peer = new PhotonPeer(this, ConnectionProtocol.Udp);
		peer.DisconnectTimeout = 30000;
		peer.DebugOut = DebugLevel.WARNING;
		CreatePrivateClasses();
		networkGameStateListener = new MVNetworkGameStateListener();
		networkGameStateListener.OnGameStateChanged += networkGameStateListener_OnGameStateChanged;
		StreamingAssetExpirationChecker = new InventoryExpirationChecker();
		StreamingAssetInfoMap = new Dictionary<int, StreamingAssetInfo>();
		StreamingAssetShopInventory = new StreamingAssetShopInventory();
		StreamingAssetInventory = new StreamingAssetInventory(StreamingAssetExpirationChecker);
	}

	private void CreatePrivateClasses()
	{
		eventHandling = new EventHandling(this);
		operationResponseHandling = new OperationResponseHandling(this);
		statusChangedHandling = new StatusChangedHandling(this);
		operationRequests = new OperationRequests(this);
	}

	private void networkGameStateListener_OnGameStateChanged(object sender, GameStateChangeEventArgs e)
	{
		switch (networkGameStateListener.CurrentGameState)
		{
		case MVGameStateType.RoundEnded:
			MVGameControllerBase.Game.LocalPlayer.ResetCheckpoint();
			if (IsPlaying)
			{
				MVGameControllerBase.WOCM.AvatarLocal.SetMode(AvatarRuntimeState.Hidden);
			}
			break;
		case MVGameStateType.Round:
			logicObjectManager.Reset();
			worldNetwork.WorldObjectClientManagerNetwork.ResetWorld();
			winningConditionManager.Reset();
			break;
		}
	}

	public void Update()
	{
		try
		{
			UpdateGame();
		}
		catch (Exception ex)
		{
			if (Application.isEditor)
			{
				throw;
			}
			Debug.LogError("Exception in update loop: " + ex.ToString());
		}
	}

	public void Cleanup()
	{
		if (worldNetwork != null && worldNetwork.WorldObjectClientManagerNetwork != null)
		{
			worldNetwork.WorldObjectClientManagerNetwork.Cleanup();
		}
	}

	private void UpdateGame()
	{
		if (peer != null)
		{
			Service();
			if (MVGameControllerBase.JoinState == MVJoinState.Playing)
			{
				CheckStreamingAsssetExpiration();
				runtimeVariableNetworkManager.SendRuntimeData();
				transformNetworkManager.Update(this);
				worldNetwork.Update(this);
				gameCoinManager.Update(this);
			}
			networkGameStateListener.Update(this);
		}
	}

	public void Service()
	{
		if (peer != null && Time.realtimeSinceStartup - prevServiceCallTime >= 0.07f)
		{
			peer.Service();
			prevServiceCallTime = Time.realtimeSinceStartup;
		}
	}

	public bool Join()
	{
		ConnState = MVConnState.Connecting;
		try
		{
			return peer.Connect(MVGameControllerBase.GameSessionData.serverIP, "MVGameServer");
		}
		catch (SecurityException ex)
		{
			Debug.LogError("Security Exception (check policy file): " + ex);
			return false;
		}
		catch (Exception ex2)
		{
			Debug.LogError("Unknown exception during connect: '" + ex2.ToString() + "', Message: '" + ex2.Message + "'");
			return false;
		}
	}

	private void CheckStreamingAsssetExpiration()
	{
		bool flag = lastExpirationStateCheck <= Time.realtimeSinceStartup / 1f;
		bool flag2 = lastDataFetchCheck <= Time.realtimeSinceStartup / 15f;
		if (flag)
		{
			lastExpirationStateCheck++;
			StreamingAssetExpirationChecker.CheckExpiration();
		}
		HashSet<int> iDs = StreamingAssetInventory.GetIDs();
		HashSet<int> hashSet = new HashSet<int>(iDs);
		HashSet<int> hashSet2 = null;
		HashSet<int> hashSet3 = null;
		HashSet<int> hashSet4 = null;
		MVBody body = WorldObjectClientManager.AvatarLocal.Body;
		if (flag2 || StreamingAssetExpirationChecker.ContainsExpired())
		{
			lastDataFetchCheck++;
			if (MVGameControllerBase.GameSessionData.gameMode != MVGameMode.CharacterEditor && body.AccessoriesLoaded)
			{
				hashSet2 = body.GetAccessoryIDs();
				hashSet.UnionWith(hashSet2);
				hashSet3 = new HashSet<int>(hashSet2);
				hashSet3.ExceptWith(iDs);
				hashSet3.ExceptWith(ownedRequestedIDs);
			}
			hashSet4 = StreamingAssetExpirationChecker.GetExpiringInfoIDs(StreamingAssetInfo.ExpiringFetchThreshold);
			if (hashSet3 != null && hashSet3.Count != 0)
			{
				hashSet3.IntersectWith(hashSet4);
				hashSet3.ExceptWith(ownedRequestFailedIDs);
				if (hashSet3.Count != 0)
				{
					RequestStreamingAssetInventoryItems(hashSet3.ToArray());
					ownedRequestedIDs.UnionWith(hashSet3);
				}
			}
			if (hashSet4.Count == 0)
			{
			}
		}
		if (hashSet4 == null || hashSet4.Count == 0 || ownedRequestedIDs.Count != 0 || StreamingAssetsExpired == null)
		{
			return;
		}
		List<InventoryExpirationInfo> list = new List<InventoryExpirationInfo>();
		foreach (int item in hashSet4)
		{
			InventoryExpirationInfo expirationInfo = StreamingAssetExpirationChecker.GetExpirationInfo(item);
			if (expirationInfo.IsExpired && !expiredStreamingAssetIDs.Contains(expirationInfo.InventoryID))
			{
				expiredStreamingAssetIDs.Add(expirationInfo.InventoryID);
				list.Add(expirationInfo);
				expirationInfo.Renewed += InventoryExpirationInfo_Renewed;
			}
		}
		if (0 < list.Count)
		{
			Debug.Log("Expired assets " + list.BuildString(eachEntryNewLine: false));
			ProductsExpiringEventArgs e = new ProductsExpiringEventArgs(list);
			StreamingAssetsExpired(this, e);
		}
	}

	private void InventoryExpirationInfo_Renewed(object sender, ProductRenewedEventArgs e)
	{
		expiredStreamingAssetIDs.Remove(e.InvetoryID);
	}

	private static void GeneratePlanetScreenShot(Action<byte[]> callback)
	{
		GameObject gameObject = new GameObject("GenerateTexture");
		GenerateTextureData generateTextureData = gameObject.AddComponent<GenerateTextureData>();
		generateTextureData.GenerateTextureDataCameraView(callback);
	}

	public void OnUnregisterWorldObjectResponse(int worldObjectID)
	{
		if (!worldNetwork.OnUnregisterWorldObject(worldObjectID))
		{
			Debug.LogWarning("OnUnregisterWorldObjectResponse failed!");
		}
	}

	public void OnResetLogicChunkEvent(int worldObjectID)
	{
		LogicObjectManager.ResetChunk(worldObjectID, MVGameControllerBase.WOCM);
	}

	public void OnPickupItemStateChangeEvent(PickupItemState state, int worldObjectID, int instigatorActorNr)
	{
		if (WorldObjectClientManager.GetWorldObjectClient(worldObjectID) != null)
		{
			if (!(WorldObjectClientManager.GetWorldObjectClient(worldObjectID) is IPickupStateHandler))
			{
				Debug.LogError("PickUpItemStateChangeEvent failed, since WOID is not derived from MVPickupItemBase");
				return;
			}
			IPickupStateHandler pickupStateHandler = (IPickupStateHandler)WorldObjectClientManager.GetWorldObjectClient(worldObjectID);
			pickupStateHandler.HandleStateChange(state);
		}
		else
		{
			Debug.LogWarning("OnPickupItemStateChangeEvent failed, since WorldObjectID does not exist. WOID: " + worldObjectID);
		}
	}

	public void OnUpdateLineOfFire(int worldObjectID, Vector3 camOrigin, Vector3 camDir)
	{
		MVWorldObjectClient worldObjectClient = WorldObjectClientManager.GetWorldObjectClient(worldObjectID);
		MVPickupOwner component = worldObjectClient.GameObject.GetComponent<MVPickupOwner>();
		if (component == null)
		{
			Debug.LogError("Pickup owner not found");
		}
		else
		{
			component.SetLineOfFire(camOrigin, camDir);
		}
	}

	public List<CommonOverlapArg> GetWOIdsWithinRadius(float radius, Vector3 worldPos)
	{
		int num = Physics.OverlapSphereNonAlloc(worldPos, radius, CollisionDetectionGlobalBuffers.colliderBuffer);
		List<CommonOverlapArg> list = new List<CommonOverlapArg>();
		HashSet<int> hashSet = new HashSet<int>();
		for (int i = 0; i < num; i++)
		{
			MVWorldObjectClient mVObject = MVWorldObjectClientManager.GetMVObject(CollisionDetectionGlobalBuffers.colliderBuffer[i].transform);
			if (mVObject is MVCubeModelBase)
			{
				CommonOverlapArg item = new CommonOverlapArg(mVObject);
				list.Add(item);
				hashSet.Add(mVObject.Id);
			}
		}
		return list;
	}

	public void OnNotificationEventReceived(NotificationType type, Dictionary<object, object> data)
	{
		if (MVGameControllerBase.OnReceivedNotification != null)
		{
			MVGameControllerBase.OnReceivedNotification(type, data);
		}
	}

	private void OnRequestFriendshipResponse(int returnCode)
	{
		string text = string.Empty;
		switch (returnCode)
		{
		case -1:
			text = "Undefined fail during friend request";
			break;
		case -2:
			text = "User does not exist";
			break;
		case -3:
			text = "You already have a pending request with that user";
			break;
		case -4:
			text = "You are already friends with that user";
			break;
		case -5:
			text = "You have blocked that user";
			break;
		case -6:
			text = "User has sent you request! Accept?";
			break;
		case -7:
			text = "That user has blocked you";
			break;
		}
		if (text != string.Empty)
		{
			MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, text);
		}
	}

	public void OnPurchaseProductResponse(int returnCode, Dictionary<object, object> purchaseResponseData)
	{
		Debug.Log("PurchaseProductResponse");
		if (PurchaseProductResponseHandler != null)
		{
			PurchaseProductResponseHandler(returnCode, purchaseResponseData);
			BrowserComm.ToJavaScript.ExternalCall("refreshCredentials");
		}
	}

	private void OnExpireProductResponse(int returnCode, Dictionary<byte, object> returnValues)
	{
		MVProductType mVProductType = (MVProductType)(int)returnValues[93];
		int num = (int)returnValues[136];
		if (returnCode != 0)
		{
			Debug.LogError(string.Concat("Expiring product ", mVProductType, " with inventoryID ", num, " failed on server"));
		}
		else if (mVProductType == MVProductType.StreamingAsset)
		{
			InventoryExpirationInfo expirationInfo = StreamingAssetExpirationChecker.GetExpirationInfo(num);
			if (expirationInfo != null)
			{
				expirationInfo.ExpirePermanently();
				StreamingAssetInventory.Remove(num);
			}
			else
			{
				Debug.LogError("Cant find expiration infor for expired streaming asset with inventoryID " + num);
			}
		}
	}

	public void AddCloneToWorldObjects(MVWorldObjectClient wo)
	{
		worldNetwork.WorldObjectClientManagerNetwork.AddToWorldObjects(wo);
	}

	private Dictionary<byte, object> GetAttachWorldObjectToSeatData(VehicleSeatBase seatBase)
	{
		Vector3 localPosition = seatBase.gameObject.transform.localPosition;
		Quaternion localRotation = seatBase.gameObject.transform.localRotation;
		int seatID = seatBase.SeatID;
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		TransformHelper.SetPosition(localPosition, dictionary);
		TransformHelper.SetRotation(localRotation, dictionary);
		dictionary.Add(141, (byte)seatID);
		dictionary.Add(142, (byte)seatBase.SeatType);
		return dictionary;
	}

	private void OnJoinResponse(Dictionary<byte, object> returnValues)
	{
		DebugLogHandler.SetupSentryClient((string)returnValues[201]);
		Dictionary<object, object> prices = (Dictionary<object, object>)returnValues[182];
		PricesManager.Init(prices);
		gameCoinManager = new MVGameCoinManager((int)returnValues[179]);
		MarketPlaceLevel = (int)returnValues[181];
		PublishLevel = (int)returnValues[184];
		XpKey = SecurityHelper.Decrypt((string)returnValues[177]);
		sessionTime = new SessionTime();
		if (!MVGameControllerBase.UsingDevSessionData)
		{
			new SessionLocatorPing();
		}
		gameType = (MVGameType)(int)returnValues[170];
		InitializeManagers();
		int actorNumber = (int)returnValues[254];
		int planetOwnershipTypeID = (int)returnValues[14];
		LocalPlayerActorNumber = actorNumber;
		MVLocalPlayer mVLocalPlayer = ((!MVGameControllerBase.IsTouristSession) ? ((MVLocalPlayer)new MVLocalPlayerRegistered(actorNumber, MVGameControllerBase.GameSessionData.profileID, (string)returnValues[9], MVGameControllerBase.GameSessionData.language)) : ((MVLocalPlayer)new MVLocalPlayerTourist(actorNumber, MVGameControllerBase.GameSessionData.profileID, (string)returnValues[9], MVGameControllerBase.GameSessionData.language)));
		mVLocalPlayer.PlanetOwnershipTypeID = planetOwnershipTypeID;
		mVLocalPlayer.Team = (MVTeam)(int)returnValues[88];
		AddPlayer(mVLocalPlayer);
		MVClientSettings.ClientSettingFlags = (ClientSettingFlags)(int)returnValues[168];
		isPublished = (bool)returnValues[80];
		MVGameControllerBase.JoinState = MVJoinState.LoadGUI;
		LoadModeGui();
		string apiUrl = (string)returnValues[174];
		string streamingAssetsUrl = (string)returnValues[103];
		if (Application.isEditor)
		{
			streamingAssetsUrl = (string)returnValues[186];
		}
		Urls.Init(apiUrl, streamingAssetsUrl);
		TM.LoadLanguage(MVGameControllerBase.GameSessionData.language);
	}

	private void InitializeManagers()
	{
		worldNetwork = new WorldNetwork();
		playerController = new MVLocalObjectController(worldNetwork.WorldObjectClientManagerNetwork, gameType);
		materialRepository = new MVMaterialRepository();
		playerRepository = new PlayerRepository();
		shopRepository = new ShopRepository();
		avatarShopRepository = new AvatarRepository();
		players = new Dictionary<int, MVPlayer>();
		friends = new FriendList();
		GameStateController = new MVGameModeChangeNotifier();
		teamManager.OnTeamAdded += gameStatCounterManager.OnTeamAdded;
		teamManager.OnTeamRemoved += gameStatCounterManager.OnTeamRemoved;
		winningConditionManager = new WinningConditionManagerClient(gameStatCounterManager);
	}

	private void OnRequestMaterialsResponse(Dictionary<object, object> materialList)
	{
		MaterialButtonTextureGenerator materialButtonTextureGenerator = UnityEngine.Object.Instantiate(PrefabPool.Instance.MaterialButtonTextureGenerator);
		foreach (byte key in materialList.Keys)
		{
			Dictionary<object, object> dictionary = (Dictionary<object, object>)materialList[key];
			string name = (string)dictionary[(byte)51];
			string description = (string)dictionary[(byte)52];
			string path = (string)dictionary[(byte)53];
			int materialSound = (int)dictionary[(byte)54];
			int modifierPackageType = (int)dictionary[(byte)55];
			int priceGold = (int)dictionary[(byte)57];
			int priceSilver = (int)dictionary[(byte)58];
			bool isUnlocked = (bool)dictionary[(byte)59];
			float[] physicalProperties = (float[])dictionary[(byte)115];
			MaterialRepository.AddMaterial(name, description, path, (MaterialSound)materialSound, (AvatarModifierPackageType)modifierPackageType, priceGold, priceSilver, isUnlocked, physicalProperties, materialButtonTextureGenerator);
		}
		UnityEngine.Object.Destroy(materialButtonTextureGenerator.gameObject);
		if (MaterialRepository.GetMaterial(21).IsDestructible || !MaterialRepository.GetMaterial(21).isUnlocked)
		{
			throw new Exception("Default material is invalid");
		}
		MVGameControllerBase.RegisterOverrideMaterials();
	}

	private void CreatePlayersFromUserList(Dictionary<object, object> userList)
	{
		string text = "Users in user list:\n";
		if (userList != null)
		{
			foreach (int key in userList.Keys)
			{
				Dictionary<byte, object> dictionary = (Dictionary<byte, object>)userList[key];
				string text2 = text;
				text = string.Concat(text2, dictionary[9], " ", key, "\n");
				if (key != LocalPlayerActorNumber)
				{
					int profileID = (int)dictionary[11];
					int team = (int)dictionary[88];
					string userName = (string)dictionary[9];
					int level = (int)dictionary[169];
					string regionCode = (string)dictionary[154];
					BuildTarget buildTarget = (BuildTarget)(byte)dictionary[188];
					MVPlayer mVPlayer = new MVPlayer(key, profileID, userName, level, regionCode, buildTarget);
					mVPlayer.Team = (MVTeam)team;
					AddPlayer(mVPlayer);
				}
			}
			return;
		}
		Debug.LogError("UserList is null");
	}

	private void OnGetBuiltInItemBusinessData(Dictionary<object, object> builtInItemBusinessData)
	{
		foreach (KeyValuePair<object, object> builtInItemBusinessDatum in builtInItemBusinessData)
		{
			MVItem mVItem = new MVItem();
			mVItem.itemID = (int)builtInItemBusinessDatum.Key;
			Dictionary<object, object> dictionary = (Dictionary<object, object>)builtInItemBusinessDatum.Value;
			mVItem.itemCategoryID = (int)dictionary[(byte)116];
			mVItem.itemTypeID = (int)dictionary[(byte)15];
			mVItem.name = (string)dictionary[(byte)10];
			mVItem.resellable = (bool)dictionary[(byte)104];
			itemBusinessLogic.AddItem(mVItem);
		}
	}

	private void OnRequestFriendsResponse(Dictionary<object, object> friendsList)
	{
		if (friendsList != null)
		{
			foreach (int key in friendsList.Keys)
			{
				Dictionary<object, object> dictionary = (Dictionary<object, object>)friendsList[key];
				int profileID = (int)dictionary[(byte)0];
				int friendProfileID = (int)dictionary[(byte)26];
				FriendStatus status = (FriendStatus)(int)dictionary[(byte)28];
				Friends.AddFriend(key, profileID, friendProfileID, status);
			}
			return;
		}
		Debug.LogWarning("Friendslist is null");
	}

	private void WOCM_InitializedGameQueryDataHandler(object sender, InitializedGameQueryDataEventArgs e)
	{
		WorldNetwork worldNetwork = this.worldNetwork;
		worldNetwork.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Remove(worldNetwork.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(WOCM_InitializedGameQueryDataHandler));
		if (e.RootWO is MVGroup)
		{
			WorldObjectClientManager.RootGroup = (MVGroup)e.RootWO;
		}
		else
		{
			Debug.LogError("RootGroup is not found!");
		}
		if (MVGameControllerBase.OnPostGameInit != null)
		{
			MVGameControllerBase.OnPostGameInit();
		}
	}

	private void TransferBodyResponseHandler(object sender, OnTransferWosResponseEventArgs e)
	{
		MVWorldObjectClientManager worldObjectClientManager = WorldObjectClientManager;
		worldObjectClientManager.OnTransferWosResponse = (EventHandler<OnTransferWosResponseEventArgs>)Delegate.Remove(worldObjectClientManager.OnTransferWosResponse, new EventHandler<OnTransferWosResponseEventArgs>(TransferBodyResponseHandler));
		Debug.Log("TransferWosResponseHandler");
		if (!e.success)
		{
			Debug.LogError("Body transfer failed!");
		}
	}

	private void OnTransferOwnershipResponse(Dictionary<byte, object> returnValues, int returnCode)
	{
		int id = (int)returnValues[20];
		int ownerActorNr = (int)returnValues[18];
		if (returnCode == 0)
		{
			worldNetwork.WorldObjectClientManagerNetwork.TransferOwnershipResponse(id, ownerActorNr, success: true);
		}
		else
		{
			worldNetwork.WorldObjectClientManagerNetwork.TransferOwnershipResponse(id, ownerActorNr, success: false);
		}
	}

	private void OnLockHierarchyResponse(Dictionary<byte, object> returnValues, int returnCode)
	{
		int id = (int)returnValues[20];
		bool lockObject = (bool)returnValues[61];
		if (returnCode == 0)
		{
		}
		worldNetwork.WorldObjectClientManagerNetwork.LockHierarchyResponse(id, lockObject, returnCode == 0);
	}

	private void OnRequestWoUniquePrototypeFailed(Dictionary<byte, object> returnValues)
	{
		Debug.LogWarning("OnRequestWoUniquePrototypeFailed");
		int woId = (int)returnValues[20];
		worldNetwork.WorldInventory.UnpendRuntimePrototype(woId);
	}

	private void CreateTeamList(Dictionary<object, object> teamList)
	{
		Dictionary<object, object> dictionary = (Dictionary<object, object>)teamList[0];
		Dictionary<object, object> dictionary2 = (Dictionary<object, object>)teamList[1];
		Dictionary<object, object> dictionary3 = (Dictionary<object, object>)teamList[2];
		Dictionary<object, object> dictionary4 = (Dictionary<object, object>)teamList[3];
		if ((bool)dictionary[(byte)0])
		{
			OnAddTeamEvent(MVTeam.Blue);
		}
		if ((bool)dictionary2[(byte)0])
		{
			OnAddTeamEvent(MVTeam.Red);
		}
		if ((bool)dictionary3[(byte)0])
		{
			OnAddTeamEvent(MVTeam.Green);
		}
		if ((bool)dictionary4[(byte)0])
		{
			OnAddTeamEvent(MVTeam.Yellow);
		}
	}

	private void OnUngroupResponse(bool success)
	{
		worldNetwork.WorldObjectClientManagerNetwork.UngroupResponse(success);
	}

	private void OnLockHierarchyEvent(EventData eventData)
	{
		worldNetwork.WorldObjectClientManagerNetwork.LockHierarchyProxy((int)eventData[20], (int)eventData[18]);
	}

	private void OnUngroupEvent(EventData eventData)
	{
		worldNetwork.WorldObjectClientManagerNetwork.UngroupProxy((int)eventData[20]);
	}

	private void OnUnregisterWorldObjectEvent(int worldObjectID)
	{
		worldNetwork.OnUnregisterWorldObject(worldObjectID);
	}

	private void OnUpdateWorldObjectEvent(EventData photonEvent)
	{
		if (MVGameControllerBase.JoinState == MVJoinState.Playing)
		{
			int woID = (int)photonEvent[20];
			NetworkTransformPackage networkTransformPackage = new NetworkTransformPackage();
			networkTransformPackage.position = TransformHelper.GetPosition(photonEvent.Parameters);
			networkTransformPackage.rotation = QuaternionCompression.ToQuaternion((byte[])photonEvent[157]);
			networkTransformPackage.timestamp = (int)photonEvent[33];
			networkTransformPackage.packageType = (TransformPackageType)(byte)photonEvent[34];
			transformNetworkManager.AddTransformPackage(woID, networkTransformPackage);
		}
	}

	private void OnWorldObjectRPCEvent(EventData photonEvent)
	{
		if (MVGameControllerBase.JoinState == MVJoinState.Playing)
		{
			int id = (int)photonEvent[20];
			if (WorldObjectClientManager.GetWorldObjectClient(id) == null)
			{
				Debug.LogError("Attempt to update world object, but object not registered in world");
				return;
			}
			int key = (int)photonEvent[254];
			MVPlayer p = Players[key];
			MVWorldObjectClient worldObjectClient = WorldObjectClientManager.GetWorldObjectClient(id);
			worldObjectClient.ReceivePackage(p, (Dictionary<object, object>)photonEvent[81]);
		}
	}

	private void OnTransferOwnershipEvent(EventData photonEvent)
	{
		bool flag = (bool)photonEvent[82];
		int num = (int)photonEvent[20];
		int ownerActorNr = (int)photonEvent[18];
		worldNetwork.WorldObjectClientManagerNetwork.TransferOwnershipProxy(num, ownerActorNr);
		if (flag)
		{
			MVWorldObjectClient worldObjectClient = WorldObjectClientManager.GetWorldObjectClient(num);
			if (worldObjectClient != null)
			{
				Vector3 position = TransformHelper.GetPosition(photonEvent.Parameters);
				Quaternion rotation = TransformHelper.GetRotation(photonEvent.Parameters);
				transformNetworkManager.RemoveNetworkObject(num);
				worldObjectClient.Position = position;
				worldObjectClient.Rotation = rotation;
			}
		}
	}

	private void OnUnregisterPrototypeEvent(int worldInventoryID)
	{
		worldNetwork.WorldInventory.RemovePrototype(worldInventoryID);
	}

	private void OnFriendRequestEvent(int friendID, int profileID, int friendProfileID)
	{
		Friends.AddFriend(friendID, profileID, friendProfileID, FriendStatus.Pending);
	}

	private void OnFriendUpdateEvent(int friendID, int profileID, FriendStatus status)
	{
		Friends.UpdateFriend(friendID, profileID, status);
	}

	private void OnAddLinkEvent(int fromID, int toID, int linkID)
	{
		Link link = new Link();
		link.outputWOID = fromID;
		link.inputWOID = toID;
		link.id = linkID;
		worldNetwork.AddLink(link);
		int num = LogicObjectManager.ResetChunk(link.inputWOID, MVGameControllerBase.WOCM);
		Debug.Log("reset count " + num);
	}

	private void OnRemoveLinkEvent(int linkID)
	{
		Link link = worldNetwork.RemoveLink(linkID);
		if (link != null)
		{
			int num = logicObjectManager.OnLinkRemoved(link, MVGameControllerBase.WOCM);
			Debug.Log("reset count " + num);
		}
	}

	private void OnAddObjectLinkEvent(int fromID, int toID, int linkID)
	{
		ObjectLink objectLink = new ObjectLink();
		objectLink.objectConnectorWOID = fromID;
		objectLink.objectWOID = toID;
		objectLink.id = linkID;
		worldNetwork.AddObjectLink(objectLink);
	}

	private void OnRemoveObjectLinkEvent(int linkID)
	{
		Debug.Log("Remove objectLink!");
		worldNetwork.RemoveObjectLink(linkID);
	}

	private void OnTriggerBoxEnterEvent(int actorNr, int worldObjectID)
	{
		if (WorldObjectClientManager.GetWorldObjectClient(worldObjectID) == null)
		{
			Debug.LogError("OnTriggerBoxEnterEvent received, but worldObjectID: " + worldObjectID + " does not exist");
		}
		else if (WorldObjectClientManager.GetWorldObjectClient(worldObjectID) is MVTriggerBox)
		{
			MVTriggerBox mVTriggerBox = (MVTriggerBox)WorldObjectClientManager.GetWorldObjectClient(worldObjectID);
			mVTriggerBox.OnEnter(Players[actorNr]);
		}
		else
		{
			Debug.LogError("OnTriggerBoxStayBegin received, but worldObjectID: " + worldObjectID + " is not a triggerbox or a togglebox");
		}
	}

	private void OnTriggerBoxExitEvent(int actorNr, int worldObjectID)
	{
		if (WorldObjectClientManager.GetWorldObjectClient(worldObjectID) == null)
		{
			Debug.LogError("OnTriggerBoxExitEvent received, but worldObjectID: " + worldObjectID + " does not exist");
		}
		else if (WorldObjectClientManager.GetWorldObjectClient(worldObjectID) is MVTriggerBox)
		{
			MVTriggerBox mVTriggerBox = (MVTriggerBox)WorldObjectClientManager.GetWorldObjectClient(worldObjectID);
			mVTriggerBox.OnExit(Players[actorNr]);
		}
		else
		{
			Debug.LogError("OnTriggerBoxStayBegin received, but worldObjectID: " + worldObjectID + " is not a triggerbox or a togglebox");
		}
	}

	private void OnTriggerBoxStayBegin(int worldObjectID, int actorNr)
	{
		MVWorldObjectClient worldObjectClient = WorldObjectClientManager.GetWorldObjectClient(worldObjectID);
		if (worldObjectClient == null)
		{
			Debug.LogError("OnTriggerBoxStayBegin received, but worldObjectID: " + worldObjectID + " does not exist");
		}
		else if (worldObjectClient is ITriggerBoxEventsHandler triggerBoxEventsHandler)
		{
			triggerBoxEventsHandler.Enter(actorNr);
		}
		else
		{
			Debug.LogError("OnTriggerBoxStayBegin received, but worldObjectID: " + worldObjectID + " is not a triggerbox or a togglebox");
		}
	}

	private void OnTriggerBoxStayEnd(int worldObjectID)
	{
		MVWorldObjectClient worldObjectClient = WorldObjectClientManager.GetWorldObjectClient(worldObjectID);
		if (worldObjectClient == null)
		{
			Debug.LogError("OnTriggerBoxStayEnd received, but worldObjectID: " + worldObjectID + " does not exist");
		}
		else if (worldObjectClient is ITriggerBoxEventsHandler triggerBoxEventsHandler)
		{
			triggerBoxEventsHandler.Exit();
		}
		else
		{
			Debug.LogError("OnTriggerBoxStayEnd received, but worldObjectID: " + worldObjectID + " is not a triggerbox or a togglebox");
		}
	}

	private void OnAddItemToInventoryEvent(EventData data)
	{
		int num = (int)data[254];
		int id = (int)data[20];
		if (num == LocalPlayerActorNumber)
		{
			InventoryItem inventoryItem = new InventoryItem(data);
			MVGameControllerBase.IEditModeUI.PlayerInventoryRepository.AddItem(inventoryItem);
			itemBusinessLogic.AddItemWithNoData(inventoryItem.itemID, inventoryItem.resellable, inventoryItem.itemCategoryID, inventoryItem.itemTypeID, inventoryItem.name);
		}
		else
		{
			MVWorldObjectClient worldObjectClient = WorldObjectClientManager.GetWorldObjectClient(id);
			if (worldObjectClient == null)
			{
				Debug.LogWarning("Attempted to assign itemId to worldObject failed. This is probably because the worldObject was deleted");
				return;
			}
		}
		int itemID = (int)data[38];
		MVWorldObjectClient.CallBackDelegate callBack = (MVWorldObjectClient wo) =>
		{
			wo.ItemId = itemID;
		};
		MVWorldObjectClient worldObjectClient2 = WorldObjectClientManager.GetWorldObjectClient(id);
		worldObjectClient2.TraverseRecursiveTail(callBack);
	}

	private void OnRemoveItemFromInventory(int itemID)
	{
		MVGameControllerBase.IEditModeUI.PlayerInventoryRepository.RemoveItem(itemID);
	}

	private void OnWoUniquePrototypeEvent(int woId, int worldInventoryId)
	{
		worldNetwork.WorldInventory.OnReplaceWoPrototype(woId, worldInventoryId);
	}

	private void AddPlayer(MVPlayer player)
	{
		if (Players.ContainsKey(player.ActorNr))
		{
			Debug.LogWarning("Duplicate player " + player.Username + " " + player.ActorNr);
		}
		else
		{
			Players.Add(player.ActorNr, player);
			if (onPlayerListChanged != null)
			{
				onPlayerListChanged();
			}
		}
	}

	public void ResetPlayer()
	{
		MVGameControllerBase.WOCM.AvatarLocal.SetMode(AvatarRuntimeState.Hidden);
		worldNetwork.WorldObjectClientManagerNetwork.ResetLocalWorldObject();
		gameCoinManager.Reset(this);
		LocalPlayer.ResetCheckpoint();
		GameStatCounterManager.RemoveStatsFromActor(LocalPlayer.ActorNr);
	}

	public void OnAddTeamEvent(MVTeam team)
	{
		TeamManager.AddTeam(team);
	}

	public void OnRemoveTeamEvent(MVTeam team, Dictionary<object, object> actorsWithNewTeam)
	{
		Debug.Log("Team removed");
		foreach (KeyValuePair<object, object> item in actorsWithNewTeam)
		{
			int num = (int)item.Key;
			MVTeam mVTeam = (MVTeam)(int)item.Value;
			Debug.Log(num);
			Debug.Log(mVTeam);
			players[num].Team = mVTeam;
		}
		TeamManager.RemoveTeam(team);
	}

	public void OnSetWorldObjectsToPurchasedEvent(int purchaseProfileId, int itemId)
	{
		worldNetwork.WorldObjectClientManagerNetwork.OnSetWorldObjectsToPurchasedEvent(purchaseProfileId, itemId);
	}

	public void OnTransferWorldObjectsToGroup(EventData eventData)
	{
		int groupId = (int)eventData[20];
		int[] worldObjectsToGroup = (int[])eventData[70];
		worldNetwork.WorldObjectClientManagerNetwork.OnTransferWorldObjectsToGroupEvent(groupId, worldObjectsToGroup);
	}

	public MVWorldObjectClient OnCloneWorldObjectTree(EventData eventData)
	{
		int[] array = (int[])eventData[70];
		int ownerActorNumber = (int)eventData[18];
		int cloneLinkId = (int)eventData[56];
		int cloneObjectLinkId = (int)eventData[91];
		bool cloneToRootGroup = (bool)eventData[100];
		int previewProfileOwnerId = (int)eventData[127];
		return worldNetwork.OnCloneWorldObjectTreeEvent(ownerActorNumber, previewProfileOwnerId, cloneToRootGroup, array[0], array[1], cloneLinkId, cloneObjectLinkId);
	}

	public MVWorldObjectClient OnCloneWorldObjectTreePosition(EventData eventData)
	{
		MVWorldObjectClient mVWorldObjectClient = OnCloneWorldObjectTree(eventData);
		mVWorldObjectClient.Position = TransformHelper.GetPosition(eventData.Parameters);
		mVWorldObjectClient.Rotation = TransformHelper.GetRotation(eventData.Parameters);
		return mVWorldObjectClient;
	}

	public void OnCloneTempWorldObjectWithOriginalReferenceEvent(EventData eventData)
	{
		int[] array = (int[])eventData[70];
		int num = array[0];
		MVWorldObjectClient worldObjectClient = WorldObjectClientManager.GetWorldObjectClient(num);
		bool flag = true;
		if (worldObjectClient.RunTimeData.ContainsObscuredKey("OriginalId"))
		{
			flag = false;
		}
		else
		{
			worldObjectClient.RunTimeData.SetObscuredType("OriginalId", (ObscuredInt)num);
		}
		OnCloneWorldObjectTreePosition(eventData);
		if (flag)
		{
			worldObjectClient.RunTimeData.RemoveObscuredKey("OriginalId");
		}
	}

	public void OnGetGameBatch(EventData eventData)
	{
		if (!eventData.Parameters.ContainsKey(245))
		{
			Debug.LogError("!eventData.Contains((byte)MVParameterKeys.Data");
			return;
		}
		int instigator = (int)eventData[254];
		BytePacker bp = new BytePacker((byte[])eventData[245]);
		QueryType queryType = (QueryType)(byte)eventData[133];
		Debug.Log(((byte[])eventData[245]).Length);
		int queryId = -1;
		bool queryDataLeft = false;
		if (eventData.Parameters.ContainsKey(98))
		{
			queryId = (int)eventData[98];
		}
		if (eventData.Parameters.ContainsKey(99))
		{
			queryDataLeft = (bool)eventData[99];
		}
		gameDataQueryManager.HandleDataBatch(instigator, queryId, queryType, queryDataLeft, bp);
	}

	private void OnGameQueryReady(EventData eventData)
	{
		int queryId = (int)eventData[98];
		gameDataQueryManager.OnGameQueryReady(queryId);
	}

	private void OnPostWinnerReportEvent()
	{
		IWinningCondition obj = null;
		if (winningConditionManager.WinningConditionFound)
		{
			List<IWinningCondition> forfilledWinningConditions = winningConditionManager.GetForfilledWinningConditions();
			if (forfilledWinningConditions.Count == 0)
			{
				Debug.LogError("No winning condition found even though server reported game ended");
				return;
			}
			if (forfilledWinningConditions.Count > 1)
			{
				Debug.LogError("Only 1 winning condition currently supported");
				return;
			}
			obj = forfilledWinningConditions[0];
		}
		else
		{
			Debug.LogError("Did not find winner condition");
		}
		if (OnWinningCondition != null)
		{
			OnWinningCondition(obj);
		}
	}

	private void OnCollectiblePickedUp(EventData photonEvent)
	{
		int actorNr = (int)photonEvent[254];
		int id = (int)photonEvent[20];
		MVWorldObjectClient worldObjectClient = WorldObjectClientManager.GetWorldObjectClient(id);
		if (worldObjectClient != null)
		{
			if (worldObjectClient is MVCollectible)
			{
				(worldObjectClient as MVCollectible).OnPickup(actorNr);
			}
			else
			{
				Debug.LogError("Attempt to call WO that is not collectible, OnCollectiblePickedUpEvent");
			}
		}
	}

	private void OnRequestStreamingAssetListResponse(Dictionary<object, object> list)
	{
		HashSet<int> hashSet = new HashSet<int>();
		foreach (int key in list.Keys)
		{
			Dictionary<object, object> dictionary = (Dictionary<object, object>)list[key];
			StreamingAssetInfo streamingAssetInfo = new StreamingAssetInfo();
			streamingAssetInfo.ProductID = key;
			streamingAssetInfo.StreamedAssetType = (StreamingAssetType)(int)dictionary[(byte)64];
			streamingAssetInfo.CategoryID = (int)dictionary[(byte)66];
			streamingAssetInfo.Name = (string)dictionary[(byte)67];
			streamingAssetInfo.Desc = (string)dictionary[(byte)68];
			streamingAssetInfo.AssetPath = (string)dictionary[(byte)69];
			StreamingAssetInfoMap.Add(streamingAssetInfo.ProductID, streamingAssetInfo);
			hashSet.Add((int)streamingAssetInfo.StreamedAssetType);
			ProductShopInfo productShopInfo = null;
			if (dictionary.ContainsKey((byte)76))
			{
				productShopInfo = new ProductShopInfo();
				productShopInfo.PriceGold = (int)dictionary[(byte)76];
				productShopInfo.PriceSilver = (int)dictionary[(byte)77];
				productShopInfo.IsBuyable = productShopInfo.PriceGold != 0 || productShopInfo.PriceSilver != 0;
				productShopInfo.RentExpireSeconds = (int)dictionary[(byte)80];
				streamingAssetInfo.ShopInfo = productShopInfo;
			}
			if (streamingAssetInfo.ShopInfo != null)
			{
				StreamingAssetShopInventory.Add(streamingAssetInfo);
			}
		}
		StreamingAssetShopInventory.NotifyProductShopInventoryChange();
	}

	private void OnRequestStreamingAssetInventoryResponse(Dictionary<object, object> list)
	{
		foreach (int key in list.Keys)
		{
			Dictionary<object, object> dictionary = (Dictionary<object, object>)list[key];
			int num2 = key;
			DateTime purchaseTime = new DateTime((long)dictionary[(byte)83]);
			bool isRented = (bool)dictionary[(byte)81];
			int num3 = (int)dictionary[(byte)62];
			StreamingAssetInfo value = null;
			StreamingAssetInfoMap.TryGetValue(num3, out value);
			if (value == null)
			{
				Debug.LogError("Missing asset info " + num3 + " for asset " + num2);
			}
			else
			{
				ProductInventoryInfo invInfo = new ProductInventoryInfo(num2, value, purchaseTime, isRented);
				StreamingAssetInventory.Add(invInfo);
			}
		}
		StreamingAssetInventory.NotifyProductInventoryChange();
	}

	public void RequestStreamingAssetInventoryItems(int[] inventoryIDs)
	{
		Debug.LogWarning("Request SA inventory items " + inventoryIDs.BuildString(null, eachEntryNewLine: false));
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(111, inventoryIDs);
		peer.OpCustom(44, dictionary, sendReliable: true);
	}

	private void OnRequestStreamingAssetInventoryItemsResponse(int returnCode, Dictionary<byte, object> returnValues)
	{
		int[] array = (int[])returnValues[111];
		int[] array2 = array;
		foreach (int item in array2)
		{
			ownedRequestedIDs.Remove(item);
		}
		if (returnCode != 0)
		{
			Debug.LogError("RequestStreamingAssetInventoryItem failed. returnCode: " + returnCode);
			int[] array3 = array;
			foreach (int item2 in array3)
			{
				ownedRequestFailedIDs.Add(item2);
			}
			return;
		}
		Dictionary<object, object> dictionary = (Dictionary<object, object>)returnValues[109];
		int[] array4 = array;
		foreach (int num in array4)
		{
			if (!dictionary.ContainsKey(num))
			{
				ownedRequestFailedIDs.Add(num);
				continue;
			}
			Dictionary<object, object> dictionary2 = (Dictionary<object, object>)dictionary[num];
			StreamingAssetInfo streamingAssetInfo = new StreamingAssetInfo();
			streamingAssetInfo.ProductID = (int)dictionary2[(byte)62];
			streamingAssetInfo.StreamedAssetType = (StreamingAssetType)(int)dictionary2[(byte)64];
			streamingAssetInfo.CategoryID = (int)dictionary2[(byte)66];
			streamingAssetInfo.Name = (string)dictionary2[(byte)67];
			streamingAssetInfo.Desc = (string)dictionary2[(byte)68];
			streamingAssetInfo.AssetPath = (string)dictionary2[(byte)69];
			if (!StreamingAssetInfoMap.ContainsKey(streamingAssetInfo.ProductID))
			{
				StreamingAssetInfoMap.Add(streamingAssetInfo.ProductID, streamingAssetInfo);
			}
			ProductShopInfo productShopInfo = null;
			if (dictionary2.ContainsKey((byte)76))
			{
				productShopInfo = new ProductShopInfo();
				productShopInfo.PriceGold = (int)dictionary2[(byte)76];
				productShopInfo.PriceSilver = (int)dictionary2[(byte)77];
				productShopInfo.IsBuyable = productShopInfo.PriceGold != 0 || productShopInfo.PriceSilver != 0;
				productShopInfo.RentExpireSeconds = (int)dictionary2[(byte)80];
				streamingAssetInfo.ShopInfo = productShopInfo;
			}
			if (streamingAssetInfo.ShopInfo != null && !StreamingAssetShopInventory.Contains(streamingAssetInfo.ProductID))
			{
				StreamingAssetShopInventory.Add(streamingAssetInfo);
			}
			if (!StreamingAssetInventory.Contains(num))
			{
				DateTime purchaseTime = new DateTime((long)dictionary2[(byte)83]);
				bool isRented = (bool)dictionary2[(byte)81];
				ProductInventoryInfo invInfo = new ProductInventoryInfo(num, streamingAssetInfo, purchaseTime, isRented);
				StreamingAssetInventory.Add(invInfo);
			}
			ownedRequestedIDs.Remove(num);
		}
	}

	private void OnGetActiveAvatarResponse(int woid)
	{
		if (OnActiveAvatar != null)
		{
			OnActiveAvatar(woid);
		}
	}

	public void OnSetTeamEvent(int actorNr, MVTeam team)
	{
		if (!Players.ContainsKey(actorNr))
		{
			Debug.LogError("!Players.ContainsKey(actorNr): " + actorNr);
		}
		Players[actorNr].Team = team;
		GameStatCounterManager.RemoveStatsFromActor(actorNr);
		if (LocalPlayer.ActorNr == actorNr && MVGameControllerBase.Game.IsPlaying)
		{
			MVGameControllerBase.Game.ResetPlayer();
		}
		if (OnFinishedLoadingPlayers != null)
		{
			OnFinishedLoadingPlayers();
		}
		if (onPlayerListChanged != null)
		{
			onPlayerListChanged();
		}
	}

	public void OnGetItemCategories(Dictionary<object, object> outData)
	{
		if (outData == null)
		{
			Debug.LogError("OnDBQueryResponse: outData is null");
		}
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		foreach (object key in outData.Keys)
		{
			dictionary.Add((string)outData[(int)key], (int)key);
		}
		itemCategories = new ItemCategories(dictionary);
	}

	public void OnGetPlanetOwnershipTypes(Dictionary<object, object> outData)
	{
		if (outData == null)
		{
			Debug.LogError("OnDBQueryResponse: outData is null");
		}
		foreach (object key in outData.Keys)
		{
			PlayerRepository.PlanetOwnershipTypes.Add((int)key, (string)outData[(int)key]);
		}
	}

	private void OnInventoryResultSetResponse(Dictionary<object, object> outData)
	{
		if (MVGameControllerBase.IEditModeUI.PlayerInventoryRepository == null)
		{
			MVGameControllerBase.IEditModeUI.PlayerInventoryRepository = new PlayerInventoryRepository();
		}
		foreach (int key in outData.Keys)
		{
			Dictionary<object, object> dictionary = (Dictionary<object, object>)outData[key];
			InventoryItem inventoryItem = new InventoryItem(key, dictionary);
			if (!inventoryItem.isDeleted)
			{
				inventoryItem.slotPosition = (int)dictionary[(byte)22];
				MVGameControllerBase.IEditModeUI.PlayerInventoryRepository.AddItem(inventoryItem);
			}
			itemBusinessLogic.AddItemWithNoData(key, inventoryItem.resellable, inventoryItem.itemCategoryID, inventoryItem.itemTypeID, inventoryItem.name);
		}
	}

	private void OnShopInventoryResultSetResponse(Dictionary<object, object> outData, bool isDone)
	{
		if (MVGameControllerBase.IEditModeUI.ClientShopRepository == null)
		{
			MVGameControllerBase.IEditModeUI.ClientShopRepository = new ClientShopRepository();
		}
		foreach (int key in outData.Keys)
		{
			ShopItem item = new ShopItem(key, outData);
			MVGameControllerBase.IEditModeUI.ClientShopRepository.AddItem(item);
		}
		if (isDone)
		{
			MVGameControllerBase.IEditModeUI.ClientShopRepository.ReorganizeBySlotPositions();
		}
	}

	private void OnAvatarShopInventoryResultSetResponse(Dictionary<object, object> outData)
	{
		foreach (int key in outData.Keys)
		{
			AvatarRepositoryItem item = new AvatarRepositoryItem(outData, key);
			AvatarShopRepository.AddItem(item);
		}
	}

	public void OnAddWorldObjectToInventoryResponseDev(int returnCode, int worldObjectID, int itemID)
	{
		string empty = string.Empty;
		empty = ((returnCode != 0) ? "Item not added to inventory" : ("Successfully added model to your inventory. ItemID is: " + itemID));
		if (OnAddWorldObjectToInventoryCallbackDev != null)
		{
			OnAddWorldObjectToInventoryCallbackDev(empty);
		}
	}

	public void OnOperationResponse(OperationResponse operationResponse)
	{
		operationResponseHandling.HandleOperationResponse(operationResponse);
		operationRequests.TryRemovePendingOperation((MVOperationCodes)operationResponse.OperationCode);
	}

	public void OnStatusChanged(StatusCode statusCode)
	{
		statusChangedHandling.OnStatusChanged(statusCode);
	}

	public void OnEvent(EventData eventData)
	{
		eventHandling.OnEvent(eventData);
	}

	private void HandleGameSnapshotData(BytePacker bytePacker, QueryType queryType, bool dataLeft)
	{
		if (gameDataQuery == null)
		{
			gameDataQuery = new GameDataQueryManager.GameDataQuery(bytePacker, LocalPlayer.ActorNr, queryType);
		}
		else
		{
			gameDataQuery.AddGameDataQuery(new GameDataQueryManager.GameDataQuery(bytePacker, LocalPlayer.ActorNr, queryType));
		}
		if (!dataLeft)
		{
			eventHandling.CacheEvents = true;
			StatHatWrapper.Count("GameSnapshotDataReceived", 1);
			Coroutines.Start(WaitForFrames.Frames(3, CreateGame));
		}
	}

	private void CreateGame()
	{
		WorldNetwork worldNetwork = this.worldNetwork;
		worldNetwork.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Combine(worldNetwork.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(OnGameCreated));
		this.worldNetwork.CreateGameWorldFromQueryData(gameDataQuery.GetBytePacker(), gameDataQuery.InstigatorActorNumber);
		gameDataQuery = null;
	}

	private void OnGameCreated(object sender, InitializedGameQueryDataEventArgs initializedGameQueryDataEventArgs)
	{
		WorldNetwork worldNetwork = this.worldNetwork;
		worldNetwork.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Remove(worldNetwork.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(OnGameCreated));
		Coroutines.Start(WaitForFrames.Frames(1, eventHandling.UncacheEventsFromJoin));
	}

	private void OnSwitchAvatar(EventData eventData)
	{
		int[] array = (int[])eventData[70];
		int num = array[0];
		int num2 = array[1];
		Debug.Log("prevBody " + num);
		Debug.Log("curBody " + num2);
		int num3 = (int)eventData[254];
		OnUnregisterWorldObjectEvent(num);
		if (MVGameControllerBase.Game.LocalPlayer.ActorNr == num3)
		{
			MVGameControllerBase.WOCM.AvatarLocal.ResetMode();
			if (MVGameControllerBase.GameMode == MVGameMode.Play)
			{
				MVGameControllerBase.WOCM.AvatarLocal.Body.AccessoryMoveOverride = true;
			}
		}
	}

	private void OnXPRewarded(int actorNr, byte xpId)
	{
		if (players.ContainsKey(actorNr))
		{
			if (OnReceivedXP != null)
			{
				OnReceivedXP(xpId, actorNr);
			}
			Debug.Log("Got xpId " + xpId);
		}
		else
		{
			Debug.LogError("Could not find player");
		}
	}

	private void OnLevelChanged(int actorNr, int level)
	{
		Debug.Log("MVNetworkGame.OnLevelChanged");
		if (players.ContainsKey(actorNr))
		{
			players[actorNr].Level = level;
		}
		else
		{
			Debug.LogError("Could not find player");
		}
	}

	private void LoadModeGui()
	{
		MVGameControllerBase.LevelLoader.LoadScenes(MVGameControllerBase.GameMode, gameType, MVGameControllerBase.IsTouristSession, operationRequests.Syncronize);
	}

	public void DebugReturn(DebugLevel level, string debug)
	{
		if (level == DebugLevel.ERROR)
		{
			Debug.LogError("DebugReturn: " + debug);
			if (debug.Contains("Exiting receive thread (inside loop) due to error"))
			{
				Debug.LogError("Leave hack. Fix this!!");
				MVGameControllerBase.ApplicationQuit(new QuitConnectionError());
			}
		}
		Debug.Log(debug);
	}
}
