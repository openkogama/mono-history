using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeStage.AntiCheat.ObscuredTypes;
using ExitGames.Client.Photon;
using MV.Common;
using MV.WorldObject;
using MV.WorldObject.AntiCheat;
using MV.WorldObject.GamePassSystem;
using MV.WorldObject.GamePassSystem.GamePassEarnings;
using MV.WorldObject.MetaData;
using MV.WorldObject.RuntimeEvents;
using MV.WorldObject.Security;
using MV.WorldObject.SpawnRoles;
using MV.WorldObject.Subscription;
using MV.WorldObject.ThemesData;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

public class MVNetworkGame : IPhotonPeerListener
{
	public delegate void OnReceivedChatMessageDelegate(MVPlayer sender, string message);

	public delegate void OnMarketPlaceActionCompleteDelegate(bool success);

	private class EventHandling
	{
		private class DynamicEventCallbackManager
		{
			private class EventCallback
			{
				public event Action<EventData> OnEventData;

				public void Notify(EventData eventData)
				{
					if (OnEventData != null)
					{
						OnEventData(eventData);
					}
					else
					{
						Debug.LogError("No subscribers to event data");
					}
				}

				public void Subscribe(Action<EventData> callback)
				{
					OnEventData += callback;
				}

				public bool UnSubscribe(Action<EventData> callback)
				{
					OnEventData -= callback;
					if (OnEventData == null)
					{
						return false;
					}
					return true;
				}
			}

			private Dictionary<MVEventCodes, EventCallback> eventCallbacks = new Dictionary<MVEventCodes, EventCallback>();

			private readonly HashSet<MVEventCodes> eventsHandledByDynamicEventCallbackManager = new HashSet<MVEventCodes> { MVEventCodes.XPRewardedAdReady };

			public bool IsDynamicEvent(MVEventCodes eventCode)
			{
				return eventsHandledByDynamicEventCallbackManager.Contains(eventCode);
			}

			public void Notify(MVEventCodes eventCode, EventData eventData)
			{
				eventCallbacks[eventCode].Notify(eventData);
			}

			public void SubscribeToEvent(MVEventCodes eventCode, Action<EventData> callback)
			{
				if (!eventsHandledByDynamicEventCallbackManager.Contains(eventCode))
				{
					throw new Exception("Event not handled by dynamic event callback manager");
				}
				if (!eventCallbacks.ContainsKey(eventCode))
				{
					eventCallbacks.Add(eventCode, new EventCallback());
				}
				eventCallbacks[eventCode].Subscribe(callback);
			}

			public void UnSubscribeToEvent(MVEventCodes eventCode, Action<EventData> callback)
			{
				if (!eventCallbacks[eventCode].UnSubscribe(callback))
				{
					eventCallbacks.Remove(eventCode);
				}
			}
		}

		private DynamicEventCallbackManager dynamicEventCallbackManager = new DynamicEventCallbackManager();

		private bool cacheEvents;

		private Queue<EventData> cachedEvents = new Queue<EventData>();

		private MVNetworkGame networkGame;

		private const float maxJoinTimeValue = 250000f;

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
				int profileID = (int)photonEvent[11];
				int num3 = (int)photonEvent[254];
				string regionCode = (string)photonEvent[154];
				BuildTarget buildTarget = (BuildTarget)photonEvent[188];
				MVTeam team2 = (MVTeam)(int)photonEvent[89];
				UserProfileData userProfileData = JsonConvert.DeserializeObject<UserProfileData>((string)photonEvent[224]);
				if (num3 == networkGame.LocalPlayer.ActorNr)
				{
					Debug.LogError("Received join event for localPlayerActorNumber");
					break;
				}
				MVPlayer mVPlayer = new MVPlayer(num3, profileID, regionCode, buildTarget, userProfileData, isReady: false);
				mVPlayer.Team = team2;
				networkGame.MVPlayerContainer.Add(mVPlayer);
				break;
			}
			case MVEventCodes.RequestMaterials:
				networkGame.OnRequestMaterialsResponse((Dictionary<object, object>)photonEvent[93]);
				break;
			case MVEventCodes.GetPlanetOwnershipTypes:
				networkGame.OnGetPlanetOwnershipTypes((Dictionary<object, object>)photonEvent[1]);
				break;
			case MVEventCodes.GetItemCategories:
				networkGame.OnGetItemCategories((Dictionary<object, object>)photonEvent[1]);
				break;
			case MVEventCodes.SetupUserAvatarEdit:
				networkGame.AllModesSetup(photonEvent);
				break;
			case MVEventCodes.SetupUserPlayMode:
				networkGame.AllModesSetup(photonEvent);
				networkGame.PlayModeSetup(photonEvent);
				break;
			case MVEventCodes.SetupUserBuildMode:
				networkGame.AllModesSetup(photonEvent);
				networkGame.PlayModeSetup(photonEvent);
				networkGame.BuildModeSetup(photonEvent);
				break;
			case MVEventCodes.GameSnapshotData:
			{
				BytePacker bytePacker = new BytePacker((byte[])photonEvent[245]);
				QueryType queryType = (QueryType)photonEvent[133];
				bool dataLeft = (bool)photonEvent[100];
				networkGame.HandleGameSnapshotData(bytePacker, queryType, dataLeft);
				break;
			}
			case MVEventCodes.SetActorReady:
				if ((int)photonEvent[254] == MVGameControllerBase.Game.LocalPlayer.ActorNr)
				{
					MVGameControllerBase.JoinState = MVJoinState.Playing;
					HandleActorReadyMetric();
					networkGame.GameCoinManager.Reset(networkGame);
					networkGame.OperationRequestSender.StartSessionTime();
				}
				MVGameControllerBase.Game.MVPlayerContainer.SetPlayerReady((int)photonEvent[254]);
				break;
			case MVEventCodes.RequestFriends:
				networkGame.OnRequestFriendsResponse((Dictionary<object, object>)photonEvent[51]);
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
				byte[] buffer = (byte[])photonEvent[164];
				networkGame.AvatarMetaDataWoMap = new MvAvatarMetaDataWoMap(new BytePacker(buffer));
				break;
			}
			case MVEventCodes.GetActiveAvatar:
				networkGame.OnGetActiveAvatarResponse((int)photonEvent[22]);
				break;
			case MVEventCodes.Leave:
			{
				int num4 = (int)photonEvent[254];
				if (num4 != networkGame.LocalPlayer.ActorNr)
				{
					if (networkGame.MVPlayerContainer.ContainsKey(num4))
					{
						MVPlayer mVPlayer2 = networkGame.MVPlayerContainer[num4];
						Dictionary<object, object> dictionary6 = new Dictionary<object, object>();
						dictionary6[(byte)0] = num4;
						dictionary6[(byte)3] = mVPlayer2.UserProfileData.UserName;
						dictionary6[(byte)6] = MVGameControllerBase.Game.Friends.IsFriend(mVPlayer2.ProfileID);
						MVGameControllerBase.PostGameMsg(MVGameMsgType.UserLeft, dictionary6);
						networkGame.gameStatCounterManager.RemoveTeamScoreOnActorLeave(num4, mVPlayer2.Team);
						networkGame.gameStatCounterManager.RemoveStatsFromActor(num4);
					}
					networkGame.MVPlayerContainer.Remove(num4);
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
				networkGame.OnUnregisterWorldObjectEvent((int)photonEvent[22]);
				break;
			case MVEventCodes.UpdateWorldObject:
				networkGame.OnUpdateWorldObjectEvent(photonEvent);
				break;
			case MVEventCodes.TransferOwnership:
				networkGame.OnTransferOwnershipEvent(photonEvent);
				break;
			case MVEventCodes.UnregisterPrototype:
				networkGame.OnUnregisterPrototypeEvent((int)photonEvent[47]);
				break;
			case MVEventCodes.UpdatePrototype:
				networkGame.worldNetwork.WorldInventory.OnUpdatePrototypeEvent((int)photonEvent[47], (byte[])photonEvent[49]);
				break;
			case MVEventCodes.UpdatePrototypeScale:
				break;
			case MVEventCodes.UpdateWorldObjectData:
				networkGame.worldNetwork.WorldObjectClientManagerNetwork.OnUpdateWorldObjectDataEvent((int)photonEvent[22], (Dictionary<object, object>)photonEvent[18]);
				break;
			case MVEventCodes.UpdateWorldObjectDataPartial:
			{
				int worldObjectID8 = (int)photonEvent[22];
				Dictionary<object, object> worldObjectData = (Dictionary<object, object>)photonEvent[18];
				networkGame.worldNetwork.WorldObjectClientManagerNetwork.OnUpdateWorldObjectDataPartialEvent(worldObjectID8, worldObjectData);
				break;
			}
			case MVEventCodes.RemoveWorldObjectDataPartial:
			{
				int worldObjectID7 = (int)photonEvent[22];
				Dictionary<object, object> worldObjectDataToRemove = (Dictionary<object, object>)photonEvent[19];
				networkGame.worldNetwork.WorldObjectClientManagerNetwork.OnRemoveWorldObjectDataPartialEvent(worldObjectID7, worldObjectDataToRemove);
				break;
			}
			case MVEventCodes.UpdateWorldObjectRunTimeData:
				if ((int)photonEvent[254] != networkGame.LocalPlayer.ActorNr)
				{
					networkGame.worldNetwork.WorldObjectClientManagerNetwork.OnUpdateWorldObjectRunTimeDataEvent((int)photonEvent[22], (Dictionary<object, object>)photonEvent[70]);
				}
				break;
			case MVEventCodes.AddLink:
				networkGame.OnAddLinkEvent((int)photonEvent[57], (int)photonEvent[56], (int)photonEvent[58]);
				break;
			case MVEventCodes.RemoveLink:
				networkGame.OnRemoveLinkEvent((int)photonEvent[58]);
				break;
			case MVEventCodes.AddObjectLink:
				networkGame.OnAddObjectLinkEvent((int)photonEvent[57], (int)photonEvent[56], (int)photonEvent[58]);
				break;
			case MVEventCodes.RemoveObjectLink:
				networkGame.OnRemoveObjectLinkEvent((int)photonEvent[58]);
				break;
			case MVEventCodes.RemoveItemFromInventory:
				networkGame.OnRemoveItemFromInventory((int)photonEvent[40]);
				break;
			case MVEventCodes.FriendRequest:
			{
				int friendID2 = (int)photonEvent[52];
				int profileID3 = (int)photonEvent[11];
				int friendProfileID = (int)photonEvent[53];
				networkGame.OnFriendRequestEvent(friendID2, profileID3, friendProfileID);
				break;
			}
			case MVEventCodes.FriendUpdate:
			{
				int friendID = (int)photonEvent[52];
				int profileID2 = (int)photonEvent[11];
				FriendStatus status = (FriendStatus)photonEvent[54];
				networkGame.OnFriendUpdateEvent(friendID, profileID2, status);
				break;
			}
			case MVEventCodes.TriggerBoxEnter:
			{
				int worldObjectID6 = (int)photonEvent[22];
				int actorNr2 = (int)photonEvent[254];
				networkGame.OnTriggerBoxEnterEvent(actorNr2, worldObjectID6);
				break;
			}
			case MVEventCodes.TriggerBoxExit:
			{
				int worldObjectID5 = (int)photonEvent[22];
				int actorNr = (int)photonEvent[254];
				networkGame.OnTriggerBoxExitEvent(actorNr, worldObjectID5);
				break;
			}
			case MVEventCodes.TriggerBoxStayBegin:
			{
				int worldObjectID4 = (int)photonEvent[22];
				int instigatorId = (int)photonEvent[254];
				networkGame.OnTriggerBoxStayBegin(worldObjectID4, instigatorId);
				break;
			}
			case MVEventCodes.TriggerBoxStayEnd:
			{
				int worldObjectID3 = (int)photonEvent[22];
				networkGame.OnTriggerBoxStayEnd(worldObjectID3);
				break;
			}
			case MVEventCodes.LockHierarchy:
				networkGame.OnLockHierarchyEvent(photonEvent);
				break;
			case MVEventCodes.WoUniquePrototype:
				networkGame.OnWoUniquePrototypeEvent((int)photonEvent[22], (int)photonEvent[47]);
				break;
			case MVEventCodes.GameStateChange:
				networkGame.NetworkGameStateListener.ChangeState((MVGameStateType)(int)photonEvent[65], (int)photonEvent[67], (int)photonEvent[66], fromGameSnapshot: false);
				break;
			case MVEventCodes.PropertiesChanged:
			{
				Dictionary<object, object> dictionary5 = (Dictionary<object, object>)photonEvent[251];
				{
					foreach (string key in dictionary5.Keys)
					{
						Debug.Log(key + ": " + dictionary5[key]);
					}
					break;
				}
			}
			case MVEventCodes.ResetLogicChunk:
				networkGame.OnResetLogicChunkEvent((int)photonEvent[22]);
				break;
			case MVEventCodes.PickupItemStateChange:
				networkGame.OnPickupItemStateChangeEvent((PickupItemState)(int)photonEvent[71], (int)photonEvent[22], (int)photonEvent[254]);
				break;
			case MVEventCodes.UpdateLineOfFire:
			{
				Vector3 camOrigin = new Vector3((float)photonEvent[74], (float)photonEvent[75], (float)photonEvent[76]);
				Vector3 camDir = new Vector3((float)photonEvent[77], (float)photonEvent[78], (float)photonEvent[79]);
				networkGame.OnUpdateLineOfFire((int)photonEvent[22], camOrigin, camDir);
				break;
			}
			case MVEventCodes.WorldObjectRPCEvent:
				networkGame.OnWorldObjectRPCEvent(photonEvent);
				break;
			case MVEventCodes.PostGameMsgEvent:
				MVGameControllerBase.PostGameMsg((MVGameMsgType)(int)photonEvent[87], (Dictionary<object, object>)photonEvent[88]);
				break;
			case MVEventCodes.SetTeam:
				networkGame.OnSetTeamEvent((int)photonEvent[254], (MVTeam)Enum.ToObject(typeof(MVTeam), (int)photonEvent[89]));
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
				networkGame.OnSetWorldObjectsToPurchasedEvent((int)photonEvent[11], (int)photonEvent[40]);
				break;
			case MVEventCodes.AchievementUnlockedEvent:
				Debug.Log($"Profile with ID {(int)photonEvent[11]} unlocked Achievement {(AchievementType)photonEvent[129]}");
				break;
			case MVEventCodes.AttachWorldObjectToSeat:
			{
				Dictionary<object, object> dictionary4 = (Dictionary<object, object>)photonEvent[72];
				int seatOwnerWoID = (int)dictionary4[(byte)4];
				int worldObjectID2 = (int)dictionary4[(byte)0];
				networkGame.PlayerController.OnAttachWorldObjectToSeat((int)photonEvent[254], seatOwnerWoID, worldObjectID2, (byte)photonEvent[141]);
				break;
			}
			case MVEventCodes.DetachWorldObjectFromVehicle:
			{
				Debug.LogWarning("Should probably be behind an interface on MVPlayer?");
				int id2 = (int)photonEvent[22];
				MVWorldObjectClient worldObjectClient4 = networkGame.WorldObjectClientManager.GetWorldObjectClient(id2);
				if (worldObjectClient4 != null && worldObjectClient4 is MVAvatar)
				{
					((MVAvatar)worldObjectClient4).OnLeaveVehicle();
				}
				break;
			}
			case MVEventCodes.ForceDetachWorldObjectFromVehicle:
			{
				int[] array = (int[])photonEvent[72];
				MVWorldObjectClient worldObjectClient2 = networkGame.WorldObjectClientManager.GetWorldObjectClient(array[0]);
				MVWorldObjectClient worldObjectClient3 = networkGame.WorldObjectClientManager.GetWorldObjectClient(array[1]);
				Debug.Log("MVEventCodes.ForceDetachWorldObjectFromVehicle");
				if (worldObjectClient2 != null)
				{
					Debug.Log("vehicle != null");
					if (worldObjectClient3.GroupId == worldObjectClient2.Id)
					{
						Debug.Log("attachedObject.GroupId == vehicle.GroupId");
						((MVAvatarLocal)worldObjectClient3).LeaveVehicle(leaveBecauseOfServer: true);
						networkGame.PlayerController.HandleDetachWorldObjectFromVehicle(success: true);
					}
				}
				break;
			}
			case MVEventCodes.SpawnVehicleWithDriver:
			{
				Dictionary<object, object> dictionary3 = (Dictionary<object, object>)photonEvent[72];
				int id = (int)dictionary3[(byte)1];
				int worldObjectID = (int)dictionary3[(byte)0];
				MVWorldObjectSpawnerVehicle mVWorldObjectSpawnerVehicle = (MVWorldObjectSpawnerVehicle)networkGame.WorldObjectClientManager.GetWorldObjectClient(id);
				int spawnWorldObjectID = mVWorldObjectSpawnerVehicle.SpawnWorldObjectID;
				int num2 = (int)dictionary3[(byte)3];
				int ownerActorNumber = (int)photonEvent[254];
				int cloneLinkId = (int)photonEvent[58];
				int cloneObjectLinkId = (int)photonEvent[92];
				int takeTime = (int)photonEvent[35];
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
				RewardReason rewardReason = (RewardReason)photonEvent[145];
				RewardType rewardType = (RewardType)photonEvent[144];
				Debug.Log($"Amount {num}, rewardReason {rewardReason}, rewardType {rewardType} ");
				BrowserComm.ToJavaScript.ExternalCall("refreshCredentials");
				break;
			}
			case MVEventCodes.RuntimeEvent:
			{
				byte[] buffer3 = (byte[])photonEvent[245];
				networkGame.worldNetwork.RuntimeEventManagerNetwork.HandleRuntimeEvent(RuntimeEvent.Create(new BytePacker(buffer3)));
				break;
			}
			case MVEventCodes.ResetTerrainEvent:
				networkGame.worldNetwork.RuntimeEventManagerNetwork.ResetTerrain();
				break;
			case MVEventCodes.UpdateGameStat:
			{
				int actorNumber = (int)photonEvent[254];
				MVTeam team = (MVTeam)(int)photonEvent[89];
				GameStatCounterType counterType = (GameStatCounterType)(byte)photonEvent[159];
				int value2 = (int)photonEvent[160];
				int otherID = (int)photonEvent[161];
				bool includeTeamScore = (bool)photonEvent[162];
				if ((bool)photonEvent[163])
				{
					networkGame.gameStatCounterManager.Increment(counterType, team, actorNumber, value2, otherID, includeTeamScore);
				}
				else
				{
					networkGame.gameStatCounterManager.Update(counterType, actorNumber, team, value2, otherID, includeTeamScore);
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
				int woID = (int)photonEvent[22];
				byte[] buffer2 = (byte[])photonEvent[165];
				MvAvatarMetaData mvAvatarMetaData = new MvAvatarMetaData(new BytePacker(buffer2));
				Debug.Log(mvAvatarMetaData);
				networkGame.AvatarMetaDataWoMap.Add(woID, mvAvatarMetaData);
				break;
			}
			case MVEventCodes.LevelChanged:
				networkGame.OnLevelChanged((int)photonEvent[254], (int)photonEvent[169]);
				break;
			case MVEventCodes.GameBoostEvent:
			{
				bool boostEnabled = (bool)photonEvent[183];
				networkGame.GameCoinManager.OnGameBoostChanged(boostEnabled);
				break;
			}
			case MVEventCodes.PendingByteDataBatch:
				networkGame.OnGetGameBatch(photonEvent);
				break;
			case MVEventCodes.SyncronizePing:
				MVGameControllerBase.OperationRequests.SyncronizePing();
				break;
			case MVEventCodes.JoinNotification:
			{
				Debug.Log("MVEventCodes.JoinNotification");
				Dictionary<object, object> dictionary2 = new Dictionary<object, object>();
				dictionary2.Add((byte)0, (int)photonEvent[254]);
				Dictionary<object, object> gameMsgData = dictionary2;
				MVGameControllerBase.PostGameMsg(MVGameMsgType.UserJoined, gameMsgData);
				break;
			}
			case MVEventCodes.SetSayChatBubbleVisible:
			{
				Dictionary<object, object> dictionary = (Dictionary<object, object>)photonEvent[245];
				bool visible = (bool)dictionary["V"];
				networkGame.OnSetSayChatBubbleVisible((int)photonEvent[254], visible);
				break;
			}
			case MVEventCodes.LogicFrame:
				networkGame.logicObjectManagerClientWrapper.Step();
				break;
			case MVEventCodes.LogicFastForward:
				Debug.Log("Fast forward");
				networkGame.logicObjectManagerClientWrapper.FastForward((int)photonEvent[35]);
				break;
			case MVEventCodes.LogicFastForwardEventImmediate:
				networkGame.logicObjectManagerClientWrapper.FastForwardImmediately((int)photonEvent[35]);
				break;
			case MVEventCodes.LogicObjectFiringStateChange:
			case MVEventCodes.CollectTheItemDropOff:
				networkGame.logicObjectManagerClientWrapper.EnqueueLogicEvent(photonEvent);
				break;
			case MVEventCodes.XPReceivedEvent:
				Debug.Log("MVEventCodes.XPReceivedEvent");
				break;
			case MVEventCodes.XPReward:
				networkGame.LocalPlayer.AddXp((int)photonEvent[220], (XPRewardType)(byte)photonEvent[219], (int)photonEvent[85], (int)photonEvent[209]);
				break;
			case MVEventCodes.GetProfileMetaData:
				FirstTimeEventManager.GetProfileMetaDataOk = (bool)photonEvent[208];
				if (FirstTimeEventManager.GetProfileMetaDataOk)
				{
					ProfileMetaData profileMetaData = JsonConvert.DeserializeObject<ProfileMetaData>((string)photonEvent[207]);
					StatHatWrapper.Count("FirstTime.Success", 1);
					FirstTimeEventManager.Initialize(profileMetaData.FirstTimeState);
					HighlightManager.Init((string)photonEvent[245]);
					MVInputWrapper.MouseSensitivityModifier = profileMetaData.MS;
					MVGameControllerBase.GoldRewardManager.Initialize((bool)photonEvent[196]);
					Debug.Log("(bool)photonEvent[(byte)MVParameterKeys.GoldRewardedGame] " + (bool)photonEvent[196]);
				}
				break;
			case MVEventCodes.ServerError:
				MVGameControllerBase.PostGameMsg(MVGameMsgType.Warning, "Server error: " + (string)photonEvent[245]);
				break;
			case MVEventCodes.GoldRewardedForLevel:
			{
				GoldRewardedForLevelCollection goldRewardedForLevelCollection = JsonConvert.DeserializeObject<GoldRewardedForLevelCollection>((string)photonEvent[245]);
				Dictionary<int, int> levelGoldRewards = goldRewardedForLevelCollection.levelGoldRewards;
				networkGame.LevelRewardsManager.AddClaimedLevelRewards(levelGoldRewards);
				break;
			}
			case MVEventCodes.NextLevelGoldReward:
			{
				GoldRewardedForLevelData goldRewardedForLevelData = JsonConvert.DeserializeObject<GoldRewardedForLevelData>((string)photonEvent[245]);
				networkGame.LevelRewardsManager.SetNextLevelReward(goldRewardedForLevelData.level, goldRewardedForLevelData.goldReward);
				break;
			}
			case MVEventCodes.GetPublishedPlanetProfileData:
			{
				string value = (string)photonEvent[245];
				if (!string.IsNullOrEmpty(value))
				{
					PlayerGamePassProgressionPackage playerGamePassProgressionPackage = JsonConvert.DeserializeObject<PlayerGamePassProgressionPackage>(value);
					GamePassesManager.PlayerPlanetData = playerGamePassProgressionPackage.playerPlanetData;
					GamePassesManager.playerTierStateCalculator = playerGamePassProgressionPackage.playerTierStateCalculator;
				}
				break;
			}
			case MVEventCodes.PlayerPlanetData:
			{
				PlayerPlanetData playerPlanetData = JsonConvert.DeserializeObject<PlayerPlanetData>((string)photonEvent[245]);
				Debug.Log(playerPlanetData);
				GamePassesManager.UpdatePlayerPlanetData(playerPlanetData);
				networkGame.MVPlayerContainer.LocalPlayer.PlayerPlanetData = playerPlanetData;
				break;
			}
			case MVEventCodes.PlayerPlanetRemote:
			{
				PlayerPlanetDataRemote playerPlanetDataRemote = JsonConvert.DeserializeObject<PlayerPlanetDataRemote>((string)photonEvent[245]);
				Debug.Log(playerPlanetDataRemote);
				networkGame.MVPlayerContainer[(int)photonEvent[254]].PlayerPlanetDataRemote = playerPlanetDataRemote;
				break;
			}
			case MVEventCodes.HighScores:
			{
				HighScoreDatas highScoreDatas2 = JsonConvert.DeserializeObject<HighScoreDatas>((string)photonEvent[245]);
				GamePassesHighScoreUpdateManager.UpdateHigscore(highScoreDatas2);
				Debug.Log(highScoreDatas2);
				break;
			}
			case MVEventCodes.PlayerTierStateCalculatorChanged:
			{
				PlayerTierStateCalculator playerTierStateCalculator = JsonConvert.DeserializeObject<PlayerTierStateCalculator>((string)photonEvent[245]);
				Debug.Log(playerTierStateCalculator);
				GamePassesManager.playerTierStateCalculator = playerTierStateCalculator;
				break;
			}
			case MVEventCodes.GetProjectEarnings:
			{
				ProjectEarningsReport newProjectEarningReport = JsonConvert.DeserializeObject<ProjectEarningsReport>((string)photonEvent[245]);
				GamePassesProjectEarningsManager.UpdateProjectEarningReport(newProjectEarningReport);
				break;
			}
			case MVEventCodes.TopHighScores:
			{
				HighScoreDatas highScoreDatas = JsonConvert.DeserializeObject<HighScoreDatas>((string)photonEvent[245]);
				GamePassesHighScoreUpdateManager.UpdateHigscore(highScoreDatas);
				Debug.Log(highScoreDatas);
				break;
			}
			case MVEventCodes.GetKogamaVat:
			{
				KogamaVatValues vatValues = JsonConvert.DeserializeObject<KogamaVatValues>((string)photonEvent[245]);
				SubscriberRewardDataManager.VatValues = vatValues;
				break;
			}
			case MVEventCodes.SetActiveSpawnRole:
			{
				Vector3 position = TransformHelper.GetPosition(photonEvent.Parameters);
				Quaternion rotation = TransformHelper.GetRotation(photonEvent.Parameters);
				MVGameControllerBase.Game.MVPlayerContainer[(int)photonEvent[254]].SpawnRolesManager.ActivateSpawnRole((int)photonEvent[191], position, rotation);
				break;
			}
			case MVEventCodes.ReplicateSpawnRoleData:
			{
				SpawnRolesRuntimeData spawnRolesRuntimeData = JsonConvert.DeserializeObject<SpawnRolesRuntimeData>((string)photonEvent[245]);
				SpawnRoleChangeHandlerRemote spawnRoleChangeHandler = new SpawnRoleChangeHandlerRemote();
				MVGameControllerBase.Game.MVPlayerContainer.GetPlayerUnsafe((int)photonEvent[254]).SetupSpawnRoleManager(spawnRoleChangeHandler, spawnRolesRuntimeData);
				break;
			}
			case MVEventCodes.GetSubscriptionPerksData:
				SubscriberRewardDataManager.SetBaseXPBonus((int)photonEvent[245]);
				break;
			case MVEventCodes.SetSpawnRoleBody:
			{
				SpawnRoleBodySwitchData spawnRoleBodySwitchData = JsonConvert.DeserializeObject<SpawnRoleBodySwitchData>((string)photonEvent[245]);
				networkGame.OnUnregisterWorldObjectEvent(spawnRoleBodySwitchData.deletedProtoBodyWoId);
				networkGame.OnUnregisterWorldObjectEvent(spawnRoleBodySwitchData.deletedBodyWoId);
				MVAvatarSpawnRoleCreator mVAvatarSpawnRoleCreator = (MVAvatarSpawnRoleCreator)MVGameControllerBase.WOCM.GetWorldObjectClient(spawnRoleBodySwitchData.spawnRoleCreatorWoId);
				mVAvatarSpawnRoleCreator.UpdateAvatarBody(spawnRoleBodySwitchData);
				break;
			}
			default:
				if (dynamicEventCallbackManager.IsDynamicEvent(eventCode))
				{
					dynamicEventCallbackManager.Notify(eventCode, photonEvent);
				}
				else
				{
					Debug.LogError("Unknown event: " + eventCode);
				}
				break;
			}
		}

		public void SubscribeToEvent(MVEventCodes eventCode, Action<EventData> callback)
		{
			dynamicEventCallbackManager.SubscribeToEvent(eventCode, callback);
		}

		public void UnSubscribeToEvent(MVEventCodes eventCode, Action<EventData> callback)
		{
			dynamicEventCallbackManager.UnSubscribeToEvent(eventCode, callback);
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
				StatHatWrapper.Count("SessionType.Tourist" + networkGame.GameType, 1);
				if (MVGameControllerBase.GameSessionData.embedded)
				{
					StatHatWrapper.Count("SessionType.TouristEmbedded" + networkGame.GameType, 1);
				}
			}
			else
			{
				StatHatWrapper.Count("SessionType." + networkGame.GameType, 1);
				if (MVGameControllerBase.GameSessionData.embedded)
				{
					StatHatWrapper.Count("SessionType.Embedded" + networkGame.GameType, 1);
				}
			}
			MVGameControllerBase.OperationRequests.IncrementStatRequest(IncrementStatRequestType.JoinCompleted);
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
					ReceivedItemFromQueryEventArgs e2 = new ReceivedItemFromQueryEventArgs(gameDataQuery.GetBytePacker(), gameDataQuery.InstigatorActorNumber);
					MVGameControllerBase.Game.ReceivedItemFromQuery(this, e2);
				}
				break;
			case QueryType.Bodies:
				if (MVGameControllerBase.Game.ReceivedAvatarBodiesFromQuery != null)
				{
					ReceivedItemFromQueryEventArgs e = new ReceivedItemFromQueryEventArgs(gameDataQuery.GetBytePacker(), gameDataQuery.InstigatorActorNumber);
					MVGameControllerBase.Game.ReceivedAvatarBodiesFromQuery(this, e);
				}
				break;
			case QueryType.AccessoryUserData:
			{
				byte[] bytes = gameDataQuery.GetBytePacker().ToArray();
				string obj = Encoding.ASCII.GetString(bytes);
				MVGameControllerBase.Game.ReceivedAccessoryData(obj);
				break;
			}
			}
		}
	}

	private class LogicEventQueue
	{
		private readonly Dictionary<int, Queue<EventData>> logicEvents = new Dictionary<int, Queue<EventData>>();

		public int Count => logicEvents.Count;

		public void Enqueue(EventData eventData)
		{
			int key = (int)eventData[35];
			if (!logicEvents.ContainsKey(key))
			{
				logicEvents.Add(key, new Queue<EventData>());
			}
			logicEvents[key].Enqueue(eventData);
		}

		public void Dequeue(int timestamp)
		{
			if (logicEvents.ContainsKey(timestamp))
			{
				Queue<EventData> queue = logicEvents[timestamp];
				logicEvents.Remove(timestamp);
				while (queue.Count > 0)
				{
					HandleEvent(queue.Dequeue());
				}
			}
		}

		private void HandleEvent(EventData photonEvent)
		{
			MVEventCodes code = (MVEventCodes)photonEvent.Code;
			switch (code)
			{
			case MVEventCodes.LogicObjectFiringStateChange:
				((IIsLogicObjectFiringEventHandler)MVGameControllerBase.WOCM.GetWorldObjectClient((int)photonEvent[22])).OnIsFiringChanged((bool)photonEvent[204]);
				break;
			case MVEventCodes.CollectTheItemDropOff:
			{
				int[] array = (int[])photonEvent[72];
				int instigatorWoID = array[0];
				int id = array[1];
				((CollectTheItemDropOff)MVGameControllerBase.WOCM.GetWorldObjectClient(id)).DropWoId(instigatorWoID);
				break;
			}
			default:
				Debug.LogError("Unknown logic event: " + code);
				break;
			}
		}
	}

	private class LogicObjectManagerClientWrapper
	{
		private LogicEventQueue logicEventQueue;

		private readonly MVNetworkGame networkGame;

		private UpdateEvaluator updateEvaluatorStep = new UpdateEvaluator(100);

		private UpdateEvaluator fastFordwardUpdateEvaluator = new UpdateEvaluator(10);

		public int StepTimeStamp => updateEvaluatorStep.StepTimestamp;

		public LogicObjectManagerClientWrapper(MVNetworkGame networkGame, int stepTimestamp)
		{
			this.networkGame = networkGame;
			updateEvaluatorStep.StepTimestamp = stepTimestamp;
			logicEventQueue = new LogicEventQueue();
		}

		public void EnqueueLogicEvent(EventData eventData)
		{
			logicEventQueue.Enqueue(eventData);
		}

		public void Step()
		{
			ExecuteRemainingFrames();
			updateEvaluatorStep.StepTimestamp += 1000;
		}

		public void FastForward(int timestamp)
		{
			fastFordwardUpdateEvaluator.StepTimestamp = timestamp;
		}

		public void FastForwardImmediately(int timestamp)
		{
			while (networkGame.LogicObjectManager.TimeStamp < timestamp)
			{
				UpdateLogicObjectManager();
			}
		}

		public void Reset()
		{
			updateEvaluatorStep.StepTimestamp += 1000;
			ExecuteRemainingFrames();
			if (logicEventQueue.Count != 0)
			{
				Debug.LogError("logic event queue not cleared on reset");
			}
			networkGame.LogicObjectManager.Reset();
			updateEvaluatorStep.StepTimestamp = 0;
			fastFordwardUpdateEvaluator.StepTimestamp = 0;
		}

		public void Update()
		{
			while (fastFordwardUpdateEvaluator.DoUpdate(networkGame.LogicObjectManager))
			{
				UpdateLogicObjectManager();
			}
			while (updateEvaluatorStep.DoUpdate(networkGame.LogicObjectManager))
			{
				UpdateLogicObjectManager();
			}
		}

		private void ExecuteRemainingFrames()
		{
			while (networkGame.LogicObjectManager.TimeStamp < updateEvaluatorStep.StepTimestamp)
			{
				UpdateLogicObjectManager();
			}
		}

		private void UpdateLogicObjectManager()
		{
			logicEventQueue.Dequeue(networkGame.LogicObjectManager.TimeStamp);
			networkGame.LogicObjectManager.Update();
		}
	}

	private class UpdateEvaluator
	{
		private int lastUpdateTick;

		private int accumulatedTime;

		private int stepTimestamp;

		private readonly int updateInterval;

		public int StepTimestamp
		{
			get
			{
				return stepTimestamp;
			}
			set
			{
				stepTimestamp = value;
				lastUpdateTick = WaitForTicksLocal.GetEnvironmentTick(0);
				accumulatedTime = 0;
			}
		}

		public int UpdateInterval => updateInterval;

		public UpdateEvaluator(int updateInterval)
		{
			this.updateInterval = updateInterval;
		}

		public bool DoUpdate(LogicObjectManager logicObjectManager)
		{
			int num = WaitForTicksLocal.Diff(lastUpdateTick);
			lastUpdateTick = WaitForTicksLocal.GetEnvironmentTick(0);
			accumulatedTime += num;
			if (accumulatedTime >= updateInterval && logicObjectManager.TimeStamp < stepTimestamp)
			{
				accumulatedTime -= updateInterval;
				return true;
			}
			return false;
		}
	}

	public class OperationRequests
	{
		private OperationResponsePendingManager operationResponsePendingManager;

		private MVNetworkGame networkGame;

		private PhotonPeer peer;

		private bool gamepointWelcomeClaimed;

		public OperationRequests(MVNetworkGame networkGame)
		{
			this.networkGame = networkGame;
			peer = networkGame.Peer;
			operationResponsePendingManager = new OperationResponsePendingManager(peer);
		}

		public void UploadData(int id, byte[] uploadData)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(191, id);
			dictionary.Add(245, uploadData);
			peer.SendOperation(62, dictionary, SendOptions.SendReliable);
		}

		public void TryRemovePendingOperation(MVOperationCodes operationCode)
		{
			operationResponsePendingManager.TryRemovePendingOperation(operationCode);
		}

		public void Syncronize()
		{
			peer.SendOperation(58, new Dictionary<byte, object>(), SendOptions.SendReliable);
		}

		public void ClaimRewardedAdXP(bool success)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(208, success);
			peer.SendOperation(117, dictionary, SendOptions.SendReliable);
		}

		public void SyncronizePing()
		{
			peer.SendOperation(60, new Dictionary<byte, object>(), SendOptions.SendReliable);
		}

		public void SetSpawnRoleBody(int avatarCreatorWoId, int avatarBodyDbId)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(22, avatarCreatorWoId);
			dictionary.Add(191, avatarBodyDbId);
			peer.SendOperation(115, dictionary, SendOptions.SendReliable);
		}

		public void TogglePreviewTier()
		{
			peer.SendOperation(116, new Dictionary<byte, object>(), SendOptions.SendReliable);
		}

		public void IncrementStatRequest(IncrementStatRequestType statRequestType, int value = 0)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(191, (short)statRequestType);
			if (value != 0)
			{
				dictionary.Add(160, value);
			}
			peer.SendOperation(118, dictionary, SendOptions.SendReliable);
		}

		public void ClaimPlayingNewGameRewardedGold()
		{
			peer.SendOperation(87, new Dictionary<byte, object>(), SendOptions.SendReliable);
		}

		public void GetHighScoreList()
		{
			peer.SendOperation(104, new Dictionary<byte, object>(), SendOptions.SendReliable);
		}

		public void GetTopHighScoreList()
		{
			peer.SendOperation(108, new Dictionary<byte, object>(), SendOptions.SendReliable);
		}

		public void CustomDevCommands()
		{
			peer.SendOperation(112, new Dictionary<byte, object>(), SendOptions.SendReliable);
		}

		public void GetAvatarBodies()
		{
			peer.SendOperation(114, new Dictionary<byte, object>(), SendOptions.SendReliable);
		}

		public void CreateSpawnRole(int avatarSpawnerWoId)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(22, avatarSpawnerWoId);
			peer.SendOperation(113, dictionary, SendOptions.SendReliable);
		}

		public void ClaimGamePointWelcomeReward()
		{
			if (!gamepointWelcomeClaimed)
			{
				gamepointWelcomeClaimed = true;
				peer.SendOperation(109, new Dictionary<byte, object>(), SendOptions.SendReliable);
			}
			else
			{
				Debug.LogError("ClaimGamePointWelcomeReward being called twice on the client side");
			}
		}

		public void AddObjectLink(ObjectLink link)
		{
			LogicObjectManager.ValidateObjectLinkStatus validateObjectLinkStatus = global::LogicObjectManager.ValidateObjectLink(link, MVGameControllerBase.WOCM, out var reportSeverity);
			if (validateObjectLinkStatus != global::LogicObjectManager.ValidateObjectLinkStatus.Ok)
			{
				if (reportSeverity == global::LogicObjectManager.ReportSeverity.Error)
				{
					Debug.LogError("Link invalid and rejected. Reason: " + validateObjectLinkStatus);
				}
				if (reportSeverity == global::LogicObjectManager.ReportSeverity.Info || reportSeverity == global::LogicObjectManager.ReportSeverity.Warning)
				{
					Debug.LogWarning("Link invalid and rejected. Reason: " + validateObjectLinkStatus);
				}
			}
			else
			{
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(57, link.objectConnectorWOID);
				dictionary.Add(56, link.objectWOID);
				peer.SendOperation(30, dictionary, SendOptions.SendReliable);
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
				PublishPlanet(newImagePending: false);
				NotificationController.PushNoticationInstruction(TM._("Remember that you need to play in the browser to update your game's image!"));
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
			if (!newImagePending || MVGameControllerBase.MaterialLoader.CheckAtlasIntegrity())
			{
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(64, newImagePending);
				return operationResponsePendingManager.AddOperationCodeToPending(MVOperationCodes.PublishPlanet, dictionary);
			}
			return true;
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
			dictionary.Add(52, friendID);
			peer.SendOperation(16, dictionary, SendOptions.SendReliable);
		}

		public void ResetPlayerPlanetData()
		{
			peer.SendOperation(103, new Dictionary<byte, object>(), SendOptions.SendReliable);
		}

		public void RequestRejectFriendShip(int friendID)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(52, friendID);
			peer.SendOperation(17, dictionary, SendOptions.SendReliable);
		}

		public void StartSessionTime()
		{
			Dictionary<byte, object> operationParameters = new Dictionary<byte, object>();
			peer.SendOperation(102, operationParameters, SendOptions.SendReliable);
		}

		public void RequestWoUniquePrototype(int woId)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(22, woId);
			peer.SendOperation(22, dictionary, SendOptions.SendReliable);
		}

		public void JoinGame()
		{
			networkGame.ConnState = MVConnState.Joining;
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			if (MVGameControllerBase.GameSessionData.gameMode == MVGameMode.CharacterEditor)
			{
				Debug.Log("Setting planetId to -1 as GameMode CharacterEditor is not using a planet");
			}
			dictionary.Add(86, MVGameControllerBase.GameSessionData.planetID);
			dictionary.Add(116, MVGameControllerBase.GameSessionData.gameMode);
			dictionary.Add(154, MVGameControllerBase.GameSessionData.language);
			dictionary.Add(167, MVGameControllerBase.GameSessionData.token);
			dictionary.Add(171, MVGameControllerBase.GameSessionData.newToken);
			dictionary.Add(172, MVGameControllerBase.GameSessionData.newPlanetName);
			dictionary.Add(188, MVGameControllerBase.BuildTarget);
			dictionary.Add(209, MVGameControllerBase.ReAuthTries);
			dictionary.Add(217, MVGameControllerBase.KoGaMaSettings.VersionString);
			JoinSessionFlags joinSessionFlags = JoinSessionFlags.None;
			if (PlayerPrefsManager.IsFirstTimeSession)
			{
				joinSessionFlags |= JoinSessionFlags.IsFirstTimeSession;
			}
			if (PlayerPrefsManager.IsReturningPlayer)
			{
				joinSessionFlags |= JoinSessionFlags.IsReturning;
			}
			if (PlayerPrefsManager.IsReturningAsSignedUp)
			{
				joinSessionFlags |= JoinSessionFlags.IsReturningAsSignedUp;
			}
			if (!MVGameControllerBase.GameSessionData.embedded)
			{
				joinSessionFlags |= JoinSessionFlags.IsOnSite;
			}
			dictionary.Add(207, (int)joinSessionFlags);
			List<FileData> cRCData = DllProtector.GetCRCData();
			dictionary.Add(218, JsonConvert.SerializeObject(cRCData));
			peer.SendOperation(byte.MaxValue, dictionary, SendOptions.SendReliable);
		}

		public bool AddLink(Link link)
		{
			LogicObjectManager.ValidateLinkStatus validateLinkStatus = global::LogicObjectManager.ValidateLink(link.outputWOID, link.inputWOID, MVGameControllerBase.WOCM, out var reportSeverity);
			switch (validateLinkStatus)
			{
			case global::LogicObjectManager.ValidateLinkStatus.LoopDetected:
				Debug.LogWarning("Link invalid and rejected. Reason: " + validateLinkStatus);
				return false;
			default:
				if (reportSeverity == global::LogicObjectManager.ReportSeverity.Error)
				{
					Debug.LogError("Link invalid and rejected. Reason: " + validateLinkStatus);
				}
				if (reportSeverity == global::LogicObjectManager.ReportSeverity.Info || reportSeverity == global::LogicObjectManager.ReportSeverity.Warning)
				{
					Debug.LogWarning("Link invalid and rejected. Reason: " + validateLinkStatus);
				}
				return false;
			case global::LogicObjectManager.ValidateLinkStatus.Ok:
			{
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(57, link.outputWOID);
				dictionary.Add(56, link.inputWOID);
				peer.SendOperation(9, dictionary, SendOptions.SendReliable);
				return true;
			}
			}
		}

		public void UpdateWorldObject(int id, Vector3 position, byte[] rotation, TransformPackageType packageType)
		{
			if (MVGameControllerBase.JoinState == MVJoinState.Playing)
			{
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(22, id);
				dictionary.Add(35, networkGame.ServerTimeInMilliSeconds);
				TransformHelper.SetPosition(position, dictionary);
				dictionary.Add(157, rotation);
				dictionary.Add(36, (byte)packageType);
				bool reliability = TransformPackageType.Stop == packageType;
				SendOptions sendOptions = new SendOptions
				{
					Reliability = reliability
				};
				peer.SendOperation(2, dictionary, sendOptions);
			}
		}

		public void UpdateLineOfFire(int worldObjectIDPickupOwner, Vector3 camDir, Vector3 camOrigin)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(22, worldObjectIDPickupOwner);
			dictionary.Add(74, camOrigin.x);
			dictionary.Add(75, camOrigin.y);
			dictionary.Add(76, camOrigin.z);
			dictionary.Add(77, camDir.x);
			dictionary.Add(78, camDir.y);
			dictionary.Add(79, camDir.z);
			peer.SendOperation(26, dictionary, SendOptions.SendUnreliable);
		}

		public void TransferWorldObjectsToGroup(int groupId, int[] worldObjects)
		{
			if (worldObjects.Contains(groupId))
			{
				Debug.LogError("Trying to transfer WO " + groupId + " to itself !");
				return;
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(22, groupId);
			dictionary.Add(72, worldObjects);
			peer.SendOperation(32, dictionary, SendOptions.SendReliable);
		}

		public void TransferOwnership(int worldObjectID, int ownerActorNr, Transform t)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(22, worldObjectID);
			dictionary.Add(20, ownerActorNr);
			if (t == null)
			{
				dictionary.Add(84, false);
			}
			else
			{
				dictionary.Add(84, true);
				TransformHelper.SetPosition(t.localPosition, dictionary);
				TransformHelper.SetRotation(t.localRotation, dictionary);
			}
			peer.SendOperation(6, dictionary, SendOptions.SendReliable);
		}

		public void PostGameMsg(MVGameMsgType gameMsgType, Dictionary<object, object> gameMsgData)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(87, (int)gameMsgType);
			dictionary.Add(88, gameMsgData);
			peer.SendOperation(28, dictionary, SendOptions.SendReliable);
		}

		public void PostChatMsg(Dictionary<object, object> gameMsgData, MVGameMsgType chatMsgType)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(87, (int)chatMsgType);
			dictionary.Add(88, gameMsgData);
			peer.SendOperation(88, dictionary, SendOptions.SendReliable);
		}

		public void PostNotificationOperation(NotificationType type, Dictionary<object, object> notificationData)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(199, type);
			dictionary.Add(200, notificationData);
			Dictionary<byte, object> operationParameters = dictionary;
			peer.SendOperation(63, operationParameters, SendOptions.SendReliable);
		}

		public void LockHierarchy(int worldObjectID, bool lockHierarchy)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(22, worldObjectID);
			dictionary.Add(63, lockHierarchy);
			peer.SendOperation(20, dictionary, SendOptions.SendReliable);
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
			dictionary3.Add(22, worldObjectID);
			dictionary3.Add(18, dictionary);
			peer.SendOperation(4, dictionary3, SendOptions.SendReliable);
		}

		public void UpdateWorldObjectDataPartial(int worldObjectID, Dictionary<object, object> woData)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(22, worldObjectID);
			dictionary.Add(18, woData);
			peer.SendOperation(4, dictionary, SendOptions.SendReliable);
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
			dictionary4.Add(22, worldObjectID);
			dictionary4.Add(19, dictionary);
			peer.SendOperation(5, dictionary4, SendOptions.SendReliable);
		}

		public void RemoveWorldObjectDataPartial(int worldObjectID, Dictionary<object, object> woDataToRemove)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(22, worldObjectID);
			dictionary.Add(19, woDataToRemove);
			peer.SendOperation(5, dictionary, SendOptions.SendReliable);
		}

		public void WorldObjectRPC(int worldObjectID, Dictionary<object, object> dataPackage)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(22, worldObjectID);
			dictionary.Add(83, dataPackage);
			peer.SendOperation(27, dictionary, SendOptions.SendReliable);
		}

		public void UpdateWorldObjectRunTimeData(int worldObjectID, Dictionary<object, object> worldObjectRunTimeData)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(22, worldObjectID);
			dictionary.Add(70, worldObjectRunTimeData);
			peer.SendOperation(25, dictionary, SendOptions.SendReliable);
		}

		public void RegisterWorldObject(WorldObjectType type, int groupId, Dictionary<object, object> woData, Vector3 position, Quaternion rotation, Vector3 scale, bool localOwner, bool transferOwnershipToServerOnLeave)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(17, type);
			dictionary.Add(23, groupId);
			dictionary.Add(18, woData);
			dictionary.Add(20, localOwner ? networkGame.LocalPlayer.ActorNr : 0);
			dictionary.Add(39, transferOwnershipToServerOnLeave);
			TransformHelper.SetPosition(position, dictionary);
			TransformHelper.SetRotation(rotation, dictionary);
			TransformHelper.SetScale(scale, dictionary);
			peer.SendOperation(0, dictionary, SendOptions.SendReliable);
		}

		public void RequestBuiltInItem(BuiltInItem builtInItem, int groupId, Dictionary<object, object> customData, Vector3 position, Quaternion rotation, Vector3 scale, bool localOwner, bool transferOwnershipToServerOnLeave)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(115, builtInItem);
			dictionary.Add(23, groupId);
			dictionary.Add(245, customData);
			dictionary.Add(20, localOwner ? networkGame.LocalPlayer.ActorNr : 0);
			dictionary.Add(39, transferOwnershipToServerOnLeave);
			TransformHelper.SetPosition(position, dictionary);
			TransformHelper.SetRotation(rotation, dictionary);
			TransformHelper.SetScale(scale, dictionary);
			peer.SendOperation(37, dictionary, SendOptions.SendReliable);
		}

		public void AddItemToWorld(int itemId, int groupId, Vector3 position, Quaternion rotation, bool localOwner, bool transferOwnershipToServerOnLeave, bool isPreviewItem)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(40, itemId);
			dictionary.Add(23, groupId);
			dictionary.Add(20, localOwner ? networkGame.LocalPlayer.ActorNr : 0);
			dictionary.Add(39, transferOwnershipToServerOnLeave);
			TransformHelper.SetPosition(position, dictionary);
			TransformHelper.SetRotation(rotation, dictionary);
			dictionary.Add(125, isPreviewItem);
			Debug.Log("AddItemToWorld");
			peer.SendOperation(38, dictionary, SendOptions.SendReliable);
		}

		public void CloneWorldObjectTree(MVWorldObjectClient root, bool localOwner, bool setAsPreviewItem, bool cloneToRootGroup)
		{
			Dictionary<byte, object> operationParameters = CreateBasicCloneData(root, localOwner, setAsPreviewItem, cloneToRootGroup);
			peer.SendOperation(33, operationParameters, SendOptions.SendReliable);
		}

		public void CloneWorldObjectTreeWithPosition(MVWorldObjectClient root, Vector3 position, Quaternion rotation, bool localOwner, bool setAsPreviewItem, bool cloneToRootGroup, bool isTempObject)
		{
			Dictionary<byte, object> operationParameters = CreateWithPositionCloneData(root, position, rotation, localOwner, setAsPreviewItem, cloneToRootGroup, isTempObject);
			peer.SendOperation(64, operationParameters, SendOptions.SendReliable);
		}

		public void CloneTempWorldObjectWithOriginalReference(MVWorldObjectClient root, Vector3 position, Quaternion rotation)
		{
			Dictionary<byte, object> operationParameters = CreateWithPositionCloneData(root, position, rotation, localOwner: true, setAsPreviewItem: false, cloneToRootGroup: true, isTempObject: true);
			peer.SendOperation(65, operationParameters, SendOptions.SendReliable);
		}

		private Dictionary<byte, object> CreateBasicCloneData(MVWorldObjectClient root, bool localOwner, bool setAsPreviewItem, bool cloneToRootGroup)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(22, root.Id);
			dictionary.Add(20, localOwner ? networkGame.LocalPlayer.ActorNr : 0);
			dictionary.Add(101, cloneToRootGroup);
			dictionary.Add(127, setAsPreviewItem);
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
			dictionary.Add(86, planetId);
			dictionary.Add(22, subtreeId);
			peer.SendOperation(34, dictionary, SendOptions.SendReliable);
		}

		public void UnregisterWorldObject(int worldObjectID)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(22, worldObjectID);
			peer.SendOperation(1, dictionary, SendOptions.SendReliable);
		}

		public void LocalPlayerLevelChanged(int level)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(169, level);
			peer.SendOperation(56, dictionary, SendOptions.SendReliable);
		}

		public void JoinNotification()
		{
			if (!MVGameControllerBase.Game.LocalPlayer.IsTourist)
			{
				Dictionary<object, object> dictionary = new Dictionary<object, object>();
				dictionary.Add((byte)9, MVGameControllerBase.Game.LocalPlayer.ActorNr);
				dictionary.Add((byte)12, MVGameControllerBase.Game.LocalPlayer.RegionCode);
				Dictionary<object, object> value = dictionary;
				Dictionary<byte, object> dictionary2 = new Dictionary<byte, object>();
				dictionary2.Add(199, NotificationType.PlayerJoined);
				dictionary2.Add(200, value);
				Dictionary<byte, object> operationParameters = dictionary2;
				peer.SendOperation(63, operationParameters, SendOptions.SendReliable);
				if (MVGameControllerBase.Game.LocalPlayer.SubscriptionRules.HasBenefit(SubscriptionBenefit.XPBoost))
				{
					dictionary2 = new Dictionary<byte, object>();
					dictionary2.Add(199, NotificationType.SubscriberJoined);
					dictionary2.Add(200, value);
					Dictionary<byte, object> operationParameters2 = dictionary2;
					peer.SendOperation(63, operationParameters2, SendOptions.SendReliable);
				}
			}
		}

		public void Ban(CheatType cheatType)
		{
			if (!MVGameControllerBase.IsTouristSession)
			{
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(178, (byte)cheatType);
				peer.SendOperation(57, dictionary, SendOptions.SendReliable);
				peer.SendOutgoingCommands();
			}
		}

		public void AutoRegisterLocalPrototype(int woId, int worldInventoryID)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(47, worldInventoryID);
			dictionary.Add(22, woId);
			peer.SendOperation(22, dictionary, SendOptions.SendReliable);
		}

		public void UpdatePrototype(int worldInventoryID, byte[] prototypeData)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(47, worldInventoryID);
			dictionary.Add(49, prototypeData);
			peer.SendOperation(7, dictionary, SendOptions.SendReliable);
		}

		public void UpdatePrototypeScale(int worldInventoryID, float scale)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(47, worldInventoryID);
			dictionary.Add(34, scale);
			peer.SendOperation(8, dictionary, SendOptions.SendReliable);
		}

		public void AddWorldObjectToInventory(int worldObjectID)
		{
			if (MVGameControllerBase.MaterialLoader.CheckAtlasIntegrity())
			{
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(22, worldObjectID);
				peer.SendOperation(39, dictionary, SendOptions.SendReliable);
			}
		}

		public void RemoveItemFromInventory(int itemID)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(40, itemID);
			peer.SendOperation(13, dictionary, SendOptions.SendReliable);
		}

		public void UpdateInventorySlots(Dictionary<object, object> itemIdToSlotIndexTable)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(46, itemIdToSlotIndexTable);
			peer.SendOperation(14, dictionary, SendOptions.SendReliable);
		}

		public void SendClientLog(string logString, string stackTrace, LogType type, Dictionary<string, object> extraSentryData, Dictionary<string, string> tags)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(146, logString);
			dictionary.Add(147, stackTrace);
			dictionary.Add(148, (byte)type);
			dictionary.Add(153, extraSentryData);
			dictionary.Add(187, tags);
			peer.SendOperation(50, dictionary, SendOptions.SendReliable);
			peer.SendOutgoingCommands();
		}

		public void ReportCaptureFlag(int woid)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(191, woid);
			peer.SendOperation(23, dictionary, SendOptions.SendReliable);
		}

		public void ReportReachedTimeAttackFlag(int captureTime, int woid)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(35, captureTime);
			dictionary.Add(191, woid);
			peer.SendOperation(94, dictionary, SendOptions.SendReliable);
		}

		public void ResetLogicChunk(int worldObjectID)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(22, worldObjectID);
			peer.SendOperation(24, dictionary, SendOptions.SendReliable);
		}

		public bool RequestFriendShipByID(int id, ref string errorText)
		{
			if (id != networkGame.LocalPlayer.ProfileID)
			{
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(53, id);
				peer.SendOperation(15, dictionary, SendOptions.SendReliable);
				return true;
			}
			errorText = TM._("Can't request friendship from yourself!");
			return false;
		}

		public void RequestMarketPlaceItem(int itemID)
		{
			Debug.Log("MVOperationCodes.GetMarketPlaceItem");
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(40, itemID);
			peer.SendOperation(43, dictionary, SendOptions.SendReliable);
		}

		public void RequestAddItemToMarketPlace(int itemID, string itemName, string itemDescription)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(40, itemID);
			dictionary.Add(42, itemName);
			dictionary.Add(134, itemDescription);
			peer.SendOperation(44, dictionary, SendOptions.SendReliable);
		}

		public void RequestRemoveItemFromMarketPlace(int itemID)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(40, itemID);
			peer.SendOperation(45, dictionary, SendOptions.SendReliable);
		}

		public void RequestLargeDBQuery(MVOperationCodes operationCode, DBQuery query, Dictionary<object, object> inData, int numRowsPerReturn)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(0, (byte)query);
			dictionary.Add(2, inData);
			dictionary.Add(6, numRowsPerReturn);
			peer.SendOperation((byte)operationCode, dictionary, SendOptions.SendReliable);
		}

		public void RequestResetTerrain()
		{
			Dictionary<byte, object> operationParameters = new Dictionary<byte, object>();
			peer.SendOperation(53, operationParameters, SendOptions.SendReliable);
		}

		public void SendRuntimeEventOperation(RuntimeEvent runtimeEvent)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(245, runtimeEvent.Data);
			peer.SendOperation(52, dictionary, SendOptions.SendReliable);
		}

		public void SetTeam(MVTeam team)
		{
			if (MVGameControllerBase.Game.TeamManager.IsTeamActive(team))
			{
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(89, (int)team);
				peer.SendOperation(29, dictionary, SendOptions.SendReliable);
			}
		}

		public void AttachWorldObjectToSeat(int seatOwnerWoID, int worldObjectID, VehicleSeatBase seatBase)
		{
			Dictionary<byte, object> attachWorldObjectToSeatData = networkGame.GetAttachWorldObjectToSeatData(seatBase);
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add((byte)4, seatOwnerWoID);
			dictionary.Add((byte)0, worldObjectID);
			attachWorldObjectToSeatData.Add(72, dictionary);
			peer.SendOperation(47, attachWorldObjectToSeatData, SendOptions.SendReliable);
		}

		public void AddAvatarToAvatarShopInventory(int worldObjectId, string name)
		{
			if (MVGameControllerBase.MaterialLoader.CheckAtlasIntegrity())
			{
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(22, worldObjectId);
				dictionary.Add(166, name);
				peer.SendOperation(54, dictionary, SendOptions.SendReliable);
			}
		}

		public void DeleteAvatarFromShopInventory(int worldObjectId)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(22, worldObjectId);
			peer.SendOperation(55, dictionary, SendOptions.SendReliable);
		}

		public void SpawnVehicleWithDriver(int worldObjectSpawnerVehicleID, int worldObjectID, VehicleSeatBase seatBase)
		{
			Dictionary<byte, object> attachWorldObjectToSeatData = networkGame.GetAttachWorldObjectToSeatData(seatBase);
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add((byte)1, worldObjectSpawnerVehicleID);
			dictionary.Add((byte)0, worldObjectID);
			attachWorldObjectToSeatData.Add(72, dictionary);
			peer.SendOperation(49, attachWorldObjectToSeatData, SendOptions.SendReliable);
		}

		public void DetachWorldObjectFromVehicle(int worldObjectID)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(22, worldObjectID);
			peer.SendOperation(48, dictionary, SendOptions.SendReliable);
		}

		public void SetAvatarAccessorySlot(int avatarBodyWoID, int streamingAssetId, float offset, float scale)
		{
			Debug.Log("Set accessory slot");
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary[126] = avatarBodyWoID;
			dictionary[105] = streamingAssetId;
			dictionary[114] = offset;
			dictionary[34] = scale;
			peer.SendOperation(46, dictionary, SendOptions.SendReliable);
		}

		public void UnEquipAccessory(int avatarBodyWoID, AccessorySlotType accessorySlotType)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary[126] = avatarBodyWoID;
			dictionary[191] = (int)accessorySlotType;
			peer.SendOperation(96, dictionary, SendOptions.SendReliable);
		}

		public void ResetAvatar(int AvatarID)
		{
			Debug.Log("Reset ActiveAvatar  called");
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(126, AvatarID);
			peer.SendOperation(42, dictionary, SendOptions.SendReliable);
		}

		public void SetActiveAvatar(int AvatarID)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(126, AvatarID);
			peer.SendOperation(41, dictionary, SendOptions.SendReliable);
		}

		public void UpdateAvatarAccessoryOffset(int bodyWoID, AccessorySlotType slot, float offset)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary[126] = bodyWoID;
			dictionary[113] = (int)slot;
			dictionary[114] = offset;
			peer.SendOperation(51, dictionary, SendOptions.SendReliable);
		}

		public void UpdateAvatarAccessoryScale(int bodyWoID, AccessorySlotType slot, float scale)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary[126] = bodyWoID;
			dictionary[113] = (int)slot;
			dictionary[34] = scale;
			peer.SendOperation(99, dictionary, SendOptions.SendReliable);
		}

		public void PurchaseSwitchTheme(int themeId, Dictionary<object, object> themeSettingsData)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add((byte)191, themeId);
			dictionary.Add((byte)207, themeSettingsData);
			PurchaseProduct(MVProductType.Theme, dictionary);
		}

		public void UnlockMaterial(int materialID)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add((byte)103, materialID);
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
			dictionary.Add((byte)126, avatarId);
			PurchaseProduct(MVProductType.Avatar, dictionary);
		}

		public void RemoveLink(int linkID)
		{
			if (!networkGame.worldNetwork.LinksContains(linkID))
			{
				Debug.LogWarning("RemoveLink: Link not found");
				return;
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(58, linkID);
			peer.SendOperation(10, dictionary, SendOptions.SendReliable);
		}

		public void RemoveObjectLink(int objectLinkID)
		{
			if (!networkGame.worldNetwork.ObjectLinksContains(objectLinkID))
			{
				Debug.LogWarning("RemoveObjectLink: ObjectLink not found");
				return;
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(58, objectLinkID);
			peer.SendOperation(31, dictionary, SendOptions.SendReliable);
		}

		public void TriggerBoxEnter(int triggerBoxOwnerId, int triggerInstigatorId)
		{
			if (MVGameControllerBase.JoinState == MVJoinState.Playing)
			{
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(22, new int[2] { triggerBoxOwnerId, triggerInstigatorId });
				peer.SendOperation(18, dictionary, SendOptions.SendReliable);
			}
		}

		public void TriggerBoxExit(int triggerBoxOwnerId, int triggerInstigatorId)
		{
			if (MVGameControllerBase.JoinState == MVJoinState.Playing)
			{
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(22, new int[2] { triggerBoxOwnerId, triggerInstigatorId });
				peer.SendOperation(19, dictionary, SendOptions.SendReliable);
			}
		}

		public bool UploadScreenshot(ImageType imageType)
		{
			return false;
		}

		public void PurchaseAvatarAccessory(int streamingAssetID)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary[(byte)105] = streamingAssetID;
			PurchaseProduct(MVProductType.Accessory, dictionary);
		}

		public void PurchaseTier(GamePassTier gamePassTier)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary[(byte)245] = (int)gamePassTier;
			PurchaseProduct(MVProductType.GamePassTier, dictionary);
		}

		public void PurchaseGameBooster(string gameBooster)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary[(byte)245] = gameBooster;
			PurchaseProduct(MVProductType.GameBooster, dictionary);
		}

		public void PurchaseAvatarAccessoryBundle(int bundleId)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary[(byte)191] = bundleId;
			PurchaseProduct(MVProductType.AccessoryBundle, dictionary);
		}

		public void LogicActivateRequest(int woID, bool activate)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(22, woID);
			dictionary.Add(204, activate);
			peer.SendOperation(66, dictionary, SendOptions.SendReliable);
		}

		public void SetFirstTimeEvent(FirstTimeEvent firstTimeEvent)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(191, (int)firstTimeEvent);
			peer.SendOperation(84, dictionary, SendOptions.SendReliable);
		}

		public void SetTier(GamePassTier gamePassTier)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(245, (byte)gamePassTier);
			peer.SendOperation(105, dictionary, SendOptions.SendReliable);
		}

		public void SetGamePassTierToSeenOperation(GamePassTier gamePassTier)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(245, (byte)gamePassTier);
			peer.SendOperation(106, dictionary, SendOptions.SendReliable);
		}

		public void SetHighlightToSeen(int highlightId)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(191, highlightId);
			peer.SendOperation(97, dictionary, SendOptions.SendReliable);
		}

		public void OverrideFirstTimeEvent(FirstTimeEvent firstTimeEvent, bool overrideValue)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(191, (int)firstTimeEvent);
			dictionary.Add(208, overrideValue);
			peer.SendOperation(85, dictionary, SendOptions.SendReliable);
		}

		public void ResetFirstTimeEvents(bool overrideValue)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(208, overrideValue);
			peer.SendOperation(83, dictionary, SendOptions.SendReliable);
		}

		public void SetMouseSensitivity(float newMouseSensitivity)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(216, newMouseSensitivity);
			peer.SendOperation(101, dictionary, SendOptions.SendReliable);
		}

		public void GetResetAvatar(int avatarWoID)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(22, avatarWoID);
			peer.SendOperation(86, dictionary, SendOptions.SendReliable);
		}

		public void RevokeEditRights(MVPlayer target)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(222, (byte)0);
			dictionary.Add(11, target.ProfileID);
			Dictionary<byte, object> operationParameters = dictionary;
			peer.SendOperation(68, operationParameters, SendOptions.SendReliable);
		}

		public void Kick(MVPlayer target, string reason)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(222, (byte)0);
			dictionary.Add(143, 0);
			dictionary.Add(11, target.ProfileID);
			dictionary.Add(88, reason);
			Dictionary<byte, object> operationParameters = dictionary;
			peer.SendOperation(67, operationParameters, SendOptions.SendReliable);
		}

		public void Ban(int hours, MVPlayer target, string reason)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(222, (byte)1);
			dictionary.Add(143, hours);
			dictionary.Add(11, target.ProfileID);
			dictionary.Add(88, reason);
			Dictionary<byte, object> operationParameters = dictionary;
			peer.SendOperation(67, operationParameters, SendOptions.SendReliable);
		}

		public void Expel(MVPlayer target, string reason)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(222, (byte)2);
			dictionary.Add(143, 0);
			dictionary.Add(11, target.ProfileID);
			dictionary.Add(88, reason);
			Dictionary<byte, object> operationParameters = dictionary;
			peer.SendOperation(67, operationParameters, SendOptions.SendReliable);
		}

		public void SetSayChatBubbleVisible(bool shouldShow)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add("V", shouldShow);
			Dictionary<byte, object> dictionary2 = new Dictionary<byte, object>();
			dictionary2.Add(245, dictionary);
			peer.SendOperation(93, dictionary2, SendOptions.SendReliable);
		}

		public void GetThemesData()
		{
			peer.SendOperation(100, new Dictionary<byte, object>(), SendOptions.SendReliable);
		}

		public void ResetHighlights()
		{
			peer.SendOperation(98, new Dictionary<byte, object>(), SendOptions.SendReliable);
		}

		public void RequestAccessoryData()
		{
			MVGameControllerBase.Game.ReceivedAccessoryData += AccessoryDataManager.SetAccessoryData;
			peer.SendOperation(95, new Dictionary<byte, object>(), SendOptions.SendReliable);
		}

		public void SetEarningsReportToSeenOperation()
		{
			peer.SendOperation(107, new Dictionary<byte, object>(), SendOptions.SendReliable);
		}

		public void SetActiveSpawnRole(int woID)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(191, woID);
			Dictionary<byte, object> operationParameters = dictionary;
			peer.SendOperation(111, operationParameters, SendOptions.SendReliable);
		}

		public void RequestUpdateGoldResponse()
		{
			peer.SendOperation(110, new Dictionary<byte, object>(), SendOptions.SendReliable);
		}

		private void PurchaseProduct(MVProductType productTypeID, Dictionary<object, object> productData)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(94, (int)productTypeID);
			dictionary.Add(95, productData);
			peer.SendOperation(35, dictionary, SendOptions.SendReliable);
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
					networkGame.OnUnregisterWorldObjectResponse((int)returnValues[22]);
				}
				break;
			case MVOperationCodes.UpdateWorldObjectData:
				if (returnCode != 0)
				{
					Debug.LogError("UpdateWorldObjectData FAILED on server!");
				}
				break;
			case MVOperationCodes.TransferOwnership:
				networkGame.OnTransferOwnershipResponse(returnValues, returnCode);
				break;
			case MVOperationCodes.AddWorldObjectToInventory:
				networkGame.OnAddItemToInventory(returnValues, returnCode);
				break;
			case MVOperationCodes.AddWorldObjectToInventoryDev:
				networkGame.OnAddItemToInventory(returnValues, returnCode);
				networkGame.OnAddWorldObjectToInventoryResponseDev(returnCode, (int)returnValues[22], (int)returnValues[40]);
				break;
			case MVOperationCodes.RequestFriendshipByProfileID:
				networkGame.OnRequestFriendshipResponse(returnCode);
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
			case MVOperationCodes.TransferWorldObjectsToGroup:
				networkGame.worldNetwork.WorldObjectClientManagerNetwork.HandleTransferWorldObjectsToGroup(returnCode == 0);
				break;
			case MVOperationCodes.PurchaseProduct:
			{
				Dictionary<object, object> purchaseResponseData = null;
				if (returnValues.ContainsKey(95))
				{
					purchaseResponseData = (Dictionary<object, object>)returnValues[95];
				}
				networkGame.OnPurchaseProductResponse(returnCode, purchaseResponseData);
				if (returnCode == 0)
				{
					MVGameControllerBase.Game.LocalPlayer.UserProfileData.Gold = (int)returnValues[130];
					if (MVGameControllerBase.Game.LocalPlayer.OnGoldAmountChange != null)
					{
						MVGameControllerBase.Game.LocalPlayer.OnGoldAmountChange();
					}
				}
				break;
			}
			case MVOperationCodes.UpdateGold:
				if (returnValues.ContainsKey(130))
				{
					MVGameControllerBase.Game.LocalPlayer.UserProfileData.Gold = (int)returnValues[130];
					if (MVGameControllerBase.Game.LocalPlayer.OnGoldAmountChange != null)
					{
						MVGameControllerBase.Game.LocalPlayer.OnGoldAmountChange();
					}
				}
				break;
			case MVOperationCodes.CloneWorldObjectTree:
			case MVOperationCodes.CloneWorldObjectTreeWithPosition:
			{
				int rootId = (int)returnValues[22];
				networkGame.worldNetwork.WorldObjectClientManagerNetwork.OnCloneWorldObjectTreeResponse(returnCode == 0, rootId);
				break;
			}
			case MVOperationCodes.UploadScreenshot:
				if (networkGame.ScreenshotUploaded != null)
				{
					networkGame.ScreenshotUploaded(this, new ScreenshotUploadedEventArgs(returnCode == 0));
				}
				break;
			case MVOperationCodes.AddItemToMarketPlace:
				if (returnCode == 0)
				{
					int itemID = (int)returnValues[40];
					int shopInventoryID = (int)returnValues[135];
					MVGameControllerBase.EditModeUI.PlayerInventoryRepository.UpdateShopInventoryID(itemID, shopInventoryID);
				}
				else
				{
					Debug.LogWarning("Failed to add item to shop");
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
			case MVOperationCodes.UploadBytes:
				DataUploadManager.OnUploadBytes();
				break;
			case MVOperationCodes.SetActiveAvatar:
				if (MVGameControllerBase.Game.OnActiveAvatarSet != null)
				{
					MVGameControllerBase.Game.OnActiveAvatarSet();
				}
				break;
			case MVOperationCodes.SetFirstTimeEvent:
				FirstTimeEventManager.OnFirstTimeEventResponse((FirstTimeEvent)(int)returnValues[191], (XPRewardType)(byte)returnValues[219]);
				break;
			case MVOperationCodes.GetThemesData:
			{
				List<ThemeData> list = JsonConvert.DeserializeObject<List<ThemeData>>((string)returnValues[207]);
				if (ThemeSelection.CallbackHandler.OnThemeDataReceived != null)
				{
					ThemeSelection.CallbackHandler.OnThemeDataReceived(list.ToArray());
				}
				break;
			}
			case MVOperationCodes.UnEquipAccessory:
				if (networkGame.OnAccessoryUnequipped != null)
				{
					networkGame.OnAccessoryUnequipped();
				}
				break;
			case MVOperationCodes.CreateSpawnRole:
				if (returnCode == -1)
				{
					MVGameControllerBase.LocalPlayer.CreateSpawnRoleFailed();
				}
				break;
			case MVOperationCodes.ClaimPlayingNewGameRewardedGold:
				if (returnCode == -1)
				{
					Debug.LogError("Failed to claim gold");
				}
				else
				{
					Debug.Log("Gold claimed. Marcus: Handle this.");
				}
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
			if (!registeredFatalStatusCodeInStatHat && ((returnCode != StatusCode.Connect && returnCode != StatusCode.Disconnect) || (returnCode == StatusCode.Disconnect && !MVGameControllerBase.DisconnectIsOk)))
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
				DebugLogHandler.DidConnectToGameServer = true;
				MVGameControllerBase.OperationRequests.JoinGame();
				break;
			case StatusCode.Disconnect:
				if (networkGame.ConnState != MVConnState.Disconnected)
				{
					networkGame.ConnState = MVConnState.DisconnectedByUser;
					Debug.LogWarning("Expecting that this disconnect is done by quiting");
				}
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
			case StatusCode.DisconnectByServerReasonUnknown:
				Debug.Log("Disconnected because: " + returnCode);
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

	private MVConnState connState;

	private const string appName = "MVGameServer";

	private MVItemBusinessLogic itemBusinessLogic = new MVItemBusinessLogic();

	private bool isPublished;

	private GameDataQueryManager gameDataQueryManager = new GameDataQueryManager();

	private TransformNetworkManager transformNetworkManager = new TransformNetworkManager();

	public readonly GameEventManager GameEventManager = new GameEventManager();

	private readonly Dictionary<Region, float> timeZoneMap = new Dictionary<Region, float>
	{
		{
			Region.dev,
			2f
		},
		{
			Region.test,
			2f
		},
		{
			Region.friends,
			-5.5f
		},
		{
			Region.brazil,
			-3f
		},
		{
			Region.www,
			2.5f
		}
	};

	private int lastFrameServerTimeUpdate = -1;

	private int lastFrameLocalTimeUpdate = -1;

	private int serverTimeInMilliseconds;

	private int localTimeInMilliseconds;

	private MVTeamManager teamManager = new MVTeamManager();

	private GameStatCounterManager gameStatCounterManager = new GameStatCounterManager();

	private LevelRewardsManager levelRewardsManager = new LevelRewardsManager();

	private WorldNetwork worldNetwork;

	public Action<int, Dictionary<object, object>> PurchaseProductResponseHandler;

	public Action<IWinningCondition> OnWinningConditionFulfilled;

	public Action<int> OnActiveAvatar;

	public Action<bool> OnItemAddedToWorld;

	public UnityAction<string> OnPublishedPlanet;

	public UnityAction<string> OnAddWorldObjectToInventoryCallbackDev;

	public Action<bool> OnSetAvatarAccessoryResponse;

	public OnReceivedChatMessageDelegate OnReceivedChatMessage;

	public OnMarketPlaceActionCompleteDelegate OnMarketPlaceActionComplete;

	public Action OnActiveAvatarSet;

	public Action OnAccessoryUnequipped;

	private readonly MVPlayerContainer playerContainer = new MVPlayerContainer();

	private LogicObjectManagerClientWrapper logicObjectManagerClientWrapper;

	private RuntimeVariableNetworkManager runtimeVariableNetworkManager = new RuntimeVariableNetworkManager();

	private float prevServiceCallTime;

	private const float serviceCallInterval = 0.07f;

	private GameDataQueryManager.GameDataQuery gameDataQuery;

	private EventHandling eventHandling;

	private OperationRequests operationRequests;

	private OperationResponseHandling operationResponseHandling;

	private StatusChangedHandling statusChangedHandling;

	public LogicObjectManagerClient LogicObjectManager { get; private set; }

	public MVGameType GameType { get; private set; }

	public Region Region { get; private set; }

	public float TimeZone => timeZoneMap[Region];

	public MVItemBusinessLogic ItemBusinessLogic => itemBusinessLogic;

	public MVGameCoinManager GameCoinManager { get; private set; }

	public ItemCategories ItemCategories { get; private set; }

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

	public MVNetworkGameStateListener NetworkGameStateListener { get; private set; }

	public PhotonPeer Peer { get; private set; }

	public ObscuredString XpKey { get; private set; }

	public int MarketPlaceLevel { get; private set; }

	public int PublishLevel { get; private set; }

	public string AdConsentEndpointURL { get; private set; }

	public string KogamaMainpageURL { get; private set; }

	public CreySettings CreySettings { get; private set; }

	public ElitePromotionSettings EliteSettings { get; private set; }

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

	public int StepTimeStamp => logicObjectManagerClientWrapper.StepTimeStamp;

	public MVMaterialRepository MaterialRepository { get; private set; }

	public PlayerRepository PlayerRepository { get; private set; }

	public ShopRepository ShopRepository { get; private set; }

	public GameTierShopRepository GameTierShopRepository { get; private set; }

	public AvatarRepository AvatarShopRepository { get; private set; }

	public MvAvatarMetaDataWoMap AvatarMetaDataWoMap { get; private set; }

	public LevelRewardsManager LevelRewardsManager => levelRewardsManager;

	public MVTeamManager TeamManager => teamManager;

	public MVGameModeChangeNotifier GameStateController { get; private set; }

	public FriendList Friends { get; private set; }

	public MVLocalObjectController PlayerController { get; private set; }

	public GameStatCounterManager GameStatCounterManager => gameStatCounterManager;

	public WinningConditionManager WinningConditionManager { get; private set; }

	public World World => worldNetwork;

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

	public OperationRequests OperationRequestSender => operationRequests;

	public TransformNetworkManager TransformNetworkManager => transformNetworkManager;

	public MVPlayerContainer MVPlayerContainer => playerContainer;

	public MVLocalPlayer LocalPlayer => playerContainer.LocalPlayer;

	public RuntimeVariableNetworkManager RuntimeVariableNetworkManager => runtimeVariableNetworkManager;

	public event EventHandler<ReceivedItemFromQueryEventArgs> ReceivedItemFromQuery;

	public event EventHandler<ReceivedItemFromQueryEventArgs> ReceivedAvatarBodiesFromQuery;

	public event Action<string> ReceivedAccessoryData;

	public event EventHandler<ScreenshotUploadedEventArgs> ScreenshotUploaded;

	public MVNetworkGame()
	{
		MVGameControllerBase.JoinState = MVJoinState.Joining;
		Peer = new PhotonPeer(this, MVGameControllerBase.GameSessionData.ConnectionProtocol);
		Peer.DisconnectTimeout = 20000;
		Peer.SentCountAllowance = 8;
		Peer.DebugOut = DebugLevel.WARNING;
		CreatePrivateClasses();
		NetworkGameStateListener = new MVNetworkGameStateListener();
		NetworkGameStateListener.OnGameStateChanged += networkGameStateListener_OnGameStateChanged;
	}

	public void SubscribeToEvent(MVEventCodes eventCode, Action<EventData> callback)
	{
		eventHandling.SubscribeToEvent(eventCode, callback);
	}

	public void UnSubscribeToEvent(MVEventCodes eventCode, Action<EventData> callback)
	{
		eventHandling.UnSubscribeToEvent(eventCode, callback);
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
		switch (NetworkGameStateListener.CurrentGameState)
		{
		case MVGameStateType.RoundEnded:
			playerContainer.LocalPlayer.ResetCheckpoint();
			break;
		case MVGameStateType.Round:
			logicObjectManagerClientWrapper.Reset();
			worldNetwork.WorldObjectClientManagerNetwork.ResetWorld();
			WinningConditionManager.Reset();
			break;
		}
		MVGameControllerBase.GameEventManager.GameState.NotifyGameStateType(NetworkGameStateListener.CurrentGameState);
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
			LogicObjectManager.Clear();
		}
		if (MaterialRepository != null)
		{
			MaterialRepository.Reset();
		}
		PricesManager.Reset();
	}

	private void UpdateGame()
	{
		if (Peer != null)
		{
			Service();
			if (logicObjectManagerClientWrapper != null)
			{
				logicObjectManagerClientWrapper.Update();
			}
			if (MVGameControllerBase.JoinState == MVJoinState.Playing)
			{
				runtimeVariableNetworkManager.SendRuntimeData();
				transformNetworkManager.Update(this);
				worldNetwork.Update(this);
				GameCoinManager.Update(this);
			}
			NetworkGameStateListener.Update(this);
		}
	}

	public void Service()
	{
		if (Peer != null && Time.realtimeSinceStartup - prevServiceCallTime >= 0.07f)
		{
			Peer.Service();
			prevServiceCallTime = Time.realtimeSinceStartup;
		}
	}

	public bool Join()
	{
		ConnState = MVConnState.Connecting;
		return Peer.Connect(MVGameControllerBase.GameSessionData.serverIP, "MVGameServer");
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
		global::LogicObjectManager.ResetChunk(worldObjectID, MVGameControllerBase.WOCM);
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

	public void AllModesSetup(EventData photonEvent)
	{
		string value = (string)photonEvent[245];
		SpawnRolesRuntimeData spawnRolesRuntimeData = JsonConvert.DeserializeObject<SpawnRolesRuntimeData>(value);
		Debug.Log(spawnRolesRuntimeData);
		MVGameControllerBase.LocalPlayer.SetupPlayerWorldObjects((int)photonEvent[191], spawnRolesRuntimeData);
		WorldNetwork worldNetwork = this.worldNetwork;
		worldNetwork.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Combine(worldNetwork.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(WOCM_InitializedGameQueryDataHandler));
		SetupLogicManager((int)photonEvent[35]);
		string value2 = (string)photonEvent[207];
		SpawnRolesMetaData spawnRoleMetaData = JsonConvert.DeserializeObject<SpawnRolesMetaData>(value2);
		MVGameControllerBase.LocalPlayer.SetSpawnRoleMetaData(spawnRoleMetaData);
	}

	public void PlayModeSetup(EventData photonEvent)
	{
		CreatePlayersFromUserList((Dictionary<object, object>)photonEvent[13]);
		MVGameStateType gameStateType = (MVGameStateType)(int)photonEvent[65];
		int startTime = (int)photonEvent[67];
		int duration = (int)photonEvent[66];
		byte[] stats = (byte[])photonEvent[158];
		gameStatCounterManager.SetStats(stats);
		NetworkGameStateListener.ChangeState(gameStateType, startTime, duration, fromGameSnapshot: true);
	}

	public void BuildModeSetup(EventData photonEvent)
	{
	}

	private void SetupLogicManager(int stepTimestamp)
	{
		if (stepTimestamp % 1000 != 0)
		{
			Debug.LogError("stepTimestamp is not correctly incremented");
		}
		LogicObjectManager = new LogicObjectManagerClient(stepTimestamp, trackLoops: false);
		logicObjectManagerClientWrapper = new LogicObjectManagerClientWrapper(this, stepTimestamp);
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
		if (PurchaseProductResponseHandler != null)
		{
			PurchaseProductResponseHandler(returnCode, purchaseResponseData);
			BrowserComm.ToJavaScript.ExternalCall("refreshCredentials");
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
		AntiCheatData antiCheatData = JsonConvert.DeserializeObject<AntiCheatData>((string)returnValues[211]);
		HackingToolDetector.Initialize(antiCheatData.applicationDescFactoryBase.ApplicationDescs.ToArray());
		Dictionary<object, object> prices = (Dictionary<object, object>)returnValues[182];
		PricesManager.Init(prices);
		GameCoinManager = new MVGameCoinManager();
		MarketPlaceLevel = (int)returnValues[181];
		PublishLevel = (int)returnValues[184];
		XpKey = SecurityHelper.Decrypt((string)returnValues[177]);
		if (!MVGameControllerBase.UsingDevSessionData)
		{
			new SessionLocatorPing();
		}
		GameType = (MVGameType)returnValues[170];
		if (GameType == MVGameType.Platformer)
		{
			Debug.LogWarning("Deprecated platformer mode");
			GameType = MVGameType.Classic;
		}
		Region = (Region)(byte)returnValues[16];
		InitializeManagers();
		int actorNumber = (int)returnValues[254];
		int num = (int)returnValues[14];
		UserProfileData userProfileData = JsonConvert.DeserializeObject<UserProfileData>((string)returnValues[224]);
		MVLocalPlayer mVLocalPlayer;
		if (MVGameControllerBase.IsTouristSession)
		{
			mVLocalPlayer = new MVLocalPlayerTourist(actorNumber, MVGameControllerBase.GameSessionData.profileID, MVGameControllerBase.GameSessionData.language, num, userProfileData);
		}
		else if (MVGameControllerBase.GameMode == MVGameMode.Edit || MVGameControllerBase.GameMode == MVGameMode.CharacterEditor)
		{
			mVLocalPlayer = new MVLocalPlayerBuilder(actorNumber, MVGameControllerBase.GameSessionData.profileID, MVGameControllerBase.GameSessionData.language, num, userProfileData);
		}
		else
		{
			if (MVGameControllerBase.GameMode != MVGameMode.Play)
			{
				throw new Exception("Unknown game mode");
			}
			mVLocalPlayer = new MVLocalPlayerRegistered(actorNumber, MVGameControllerBase.GameSessionData.profileID, MVGameControllerBase.GameSessionData.language, num, userProfileData);
		}
		ThemeRepository.Instance.ThemesEnabled = (bool)returnValues[212];
		mVLocalPlayer.Team = (MVTeam)returnValues[89];
		playerContainer.Add(mVLocalPlayer);
		playerContainer.SetLocalPlayer(mVLocalPlayer.ActorNr);
		MVClientSettings.ClientSettingFlags = (ClientSettingFlags)returnValues[168];
		MVClientSettings.PostGameInterstitialIntervalInSeconds = (int)returnValues[215];
		MVClientSettings.ReviveFlags = (int)returnValues[233];
		AdConsentEndpointURL = (string)returnValues[225];
		KogamaMainpageURL = (string)returnValues[226];
		CreySettings = new CreySettings((int)returnValues[228], (string)returnValues[229], (bool)returnValues[230]);
		EliteSettings = new ElitePromotionSettings((bool)returnValues[231], (int)returnValues[232]);
		isPublished = (bool)returnValues[82];
		MVGameControllerBase.JoinState = MVJoinState.LoadGUI;
		LoadModeGui();
		string apiUrl = (string)returnValues[174];
		string streamingAssetsUrl = (string)returnValues[104];
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
		PlayerController = new MVLocalObjectController(worldNetwork.WorldObjectClientManagerNetwork);
		MaterialRepository = new MVMaterialRepository();
		PlayerRepository = new PlayerRepository();
		ShopRepository = new ShopRepository();
		GameTierShopRepository = new GameTierShopRepository();
		AvatarShopRepository = new AvatarRepository();
		Friends = new FriendList();
		GameStateController = new MVGameModeChangeNotifier();
		teamManager.OnTeamAdded += gameStatCounterManager.OnTeamAdded;
		teamManager.OnTeamRemoved += gameStatCounterManager.OnTeamRemoved;
		WinningConditionManager = new WinningConditionManagerClient();
		WinningConditionManager.Initialize(gameStatCounterManager);
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
			bool isUnlocked = (bool)dictionary[(byte)58];
			float[] physicalProperties = (float[])dictionary[(byte)111];
			MaterialRepository.AddMaterial(name, description, path, (MaterialSound)materialSound, (AvatarModifierPackageType)modifierPackageType, priceGold, isUnlocked, physicalProperties, materialButtonTextureGenerator);
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
		if (userList != null)
		{
			List<MVPlayer> list = new List<MVPlayer>();
			foreach (int key in userList.Keys)
			{
				Dictionary<byte, object> dictionary = (Dictionary<byte, object>)userList[key];
				if (key != playerContainer.LocalPlayer.ActorNr)
				{
					int profileID = (int)dictionary[11];
					int team = (int)dictionary[89];
					int level = (int)dictionary[169];
					string regionCode = (string)dictionary[154];
					bool isReady = (bool)dictionary[210];
					PlayerPlanetDataRemote playerPlanetDataRemote = JsonConvert.DeserializeObject<PlayerPlanetDataRemote>((string)dictionary[223]);
					UserProfileData userProfileData = JsonConvert.DeserializeObject<UserProfileData>((string)dictionary[224]);
					BuildTarget buildTarget = (BuildTarget)dictionary[188];
					MVPlayer mVPlayer = new MVPlayer(key, profileID, level, regionCode, buildTarget, userProfileData, isReady, playerPlanetDataRemote);
					mVPlayer.Team = (MVTeam)team;
					SpawnRolesRuntimeData spawnRolesRuntimeData = JsonConvert.DeserializeObject<SpawnRolesRuntimeData>((string)dictionary[245]);
					SpawnRoleChangeHandlerRemote spawnRoleChangeHandler = new SpawnRoleChangeHandlerRemote();
					mVPlayer.SetupSpawnRoleManager(spawnRoleChangeHandler, spawnRolesRuntimeData);
					list.Add(mVPlayer);
				}
			}
			playerContainer.Add(list);
		}
		else
		{
			Debug.LogError("UserList is null");
		}
	}

	private void OnGetBuiltInItemBusinessData(Dictionary<object, object> builtInItemBusinessData)
	{
		foreach (KeyValuePair<object, object> builtInItemBusinessDatum in builtInItemBusinessData)
		{
			MVItem mVItem = new MVItem();
			mVItem.itemID = (int)builtInItemBusinessDatum.Key;
			Dictionary<object, object> dictionary = (Dictionary<object, object>)builtInItemBusinessDatum.Value;
			mVItem.itemCategoryID = (int)dictionary[(byte)112];
			mVItem.itemTypeID = (int)dictionary[(byte)15];
			mVItem.name = (string)dictionary[(byte)10];
			mVItem.resellable = (bool)dictionary[(byte)100];
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
				FriendStatus status = (FriendStatus)dictionary[(byte)28];
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
		int id = (int)returnValues[22];
		int ownerActorNr = (int)returnValues[20];
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
		int id = (int)returnValues[22];
		bool lockObject = (bool)returnValues[63];
		worldNetwork.WorldObjectClientManagerNetwork.LockHierarchyResponse(id, lockObject, returnCode == 0);
	}

	private void OnRequestWoUniquePrototypeFailed(Dictionary<byte, object> returnValues)
	{
		Debug.LogWarning("OnRequestWoUniquePrototypeFailed");
		int woId = (int)returnValues[22];
		worldNetwork.WorldInventory.UnpendRuntimePrototype(woId);
	}

	private void OnLockHierarchyEvent(EventData eventData)
	{
		worldNetwork.WorldObjectClientManagerNetwork.LockHierarchyProxy((int)eventData[22], (int)eventData[20]);
	}

	private void OnUnregisterWorldObjectEvent(int worldObjectID)
	{
		worldNetwork.OnUnregisterWorldObject(worldObjectID);
	}

	private void OnUpdateWorldObjectEvent(EventData photonEvent)
	{
		if (MVGameControllerBase.JoinState == MVJoinState.Playing)
		{
			int woID = (int)photonEvent[22];
			NetworkTransformPackage networkTransformPackage = new NetworkTransformPackage();
			networkTransformPackage.position = TransformHelper.GetPosition(photonEvent.Parameters);
			networkTransformPackage.rotation = QuaternionCompression.ToQuaternion((byte[])photonEvent[157]);
			networkTransformPackage.timestamp = (int)photonEvent[35];
			networkTransformPackage.packageType = (TransformPackageType)(byte)photonEvent[36];
			transformNetworkManager.AddTransformPackage(woID, networkTransformPackage);
		}
	}

	private void OnWorldObjectRPCEvent(EventData photonEvent)
	{
		if (MVGameControllerBase.JoinState == MVJoinState.Playing)
		{
			int id = (int)photonEvent[22];
			if (WorldObjectClientManager.GetWorldObjectClient(id) == null)
			{
				Debug.LogError("Attempt to update world object, but object not registered in world");
				return;
			}
			int actorNumber = (int)photonEvent[254];
			MVPlayer p = playerContainer[actorNumber];
			MVWorldObjectClient worldObjectClient = WorldObjectClientManager.GetWorldObjectClient(id);
			worldObjectClient.ReceivePackage(p, (Dictionary<object, object>)photonEvent[83]);
		}
	}

	private void OnTransferOwnershipEvent(EventData photonEvent)
	{
		bool flag = (bool)photonEvent[84];
		int num = (int)photonEvent[22];
		int ownerActorNr = (int)photonEvent[20];
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
		int num = LogicObjectManager.OnLinkAdded(link, MVGameControllerBase.WOCM);
		Debug.Log("reset count " + num);
	}

	private void OnRemoveLinkEvent(int linkID)
	{
		Link link = worldNetwork.RemoveLink(linkID);
		if (link != null)
		{
			int num = LogicObjectManager.OnLinkRemoved(link, MVGameControllerBase.WOCM);
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
			mVTriggerBox.OnEnter(playerContainer[actorNr]);
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
			mVTriggerBox.OnExit(playerContainer[actorNr]);
		}
		else
		{
			Debug.LogError("OnTriggerBoxStayBegin received, but worldObjectID: " + worldObjectID + " is not a triggerbox or a togglebox");
		}
	}

	private void OnTriggerBoxStayBegin(int worldObjectID, int instigatorId)
	{
		MVWorldObjectClient worldObjectClient = WorldObjectClientManager.GetWorldObjectClient(worldObjectID);
		if (worldObjectClient == null)
		{
			Debug.LogError("OnTriggerBoxStayBegin received, but worldObjectID: " + worldObjectID + " does not exist");
		}
		else if (worldObjectClient is ITriggerBoxEventsHandler triggerBoxEventsHandler)
		{
			triggerBoxEventsHandler.Enter(instigatorId);
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

	private void OnRemoveItemFromInventory(int itemID)
	{
		MVGameControllerBase.EditModeUI.PlayerInventoryRepository.RemoveItem(itemID);
	}

	private void OnWoUniquePrototypeEvent(int woId, int worldInventoryId)
	{
		worldNetwork.WorldInventory.OnReplaceWoPrototype(woId, worldInventoryId);
	}

	public void ResetPlayer()
	{
		worldNetwork.WorldObjectClientManagerNetwork.ResetLocalWorldObject();
		GameCoinManager.Reset(this);
		playerContainer.LocalPlayer.ResetCheckpoint();
		GameStatCounterManager.RemoveStatsFromActor(playerContainer.LocalPlayer.ActorNr);
	}

	public void OnSetWorldObjectsToPurchasedEvent(int purchaseProfileId, int itemId)
	{
		worldNetwork.WorldObjectClientManagerNetwork.OnSetWorldObjectsToPurchasedEvent(purchaseProfileId, itemId);
	}

	public void OnTransferWorldObjectsToGroup(EventData eventData)
	{
		int groupId = (int)eventData[22];
		int[] worldObjectsToGroup = (int[])eventData[72];
		worldNetwork.WorldObjectClientManagerNetwork.OnTransferWorldObjectsToGroupEvent(groupId, worldObjectsToGroup);
	}

	public MVWorldObjectClient OnCloneWorldObjectTree(EventData eventData)
	{
		int[] array = (int[])eventData[72];
		int ownerActorNumber = (int)eventData[20];
		int cloneLinkId = (int)eventData[58];
		int cloneObjectLinkId = (int)eventData[92];
		bool flag = (bool)eventData[101];
		Debug.Log("CloneToRootGroup " + flag);
		int previewProfileOwnerId = (int)eventData[128];
		return worldNetwork.OnCloneWorldObjectTreeEvent(ownerActorNumber, previewProfileOwnerId, flag, array[0], array[1], cloneLinkId, cloneObjectLinkId);
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
		int[] array = (int[])eventData[72];
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
		QueryType queryType = (QueryType)eventData[133];
		int queryId = -1;
		bool queryDataLeft = false;
		if (eventData.Parameters.ContainsKey(99))
		{
			queryId = (int)eventData[99];
		}
		if (eventData.Parameters.ContainsKey(100))
		{
			queryDataLeft = (bool)eventData[100];
		}
		gameDataQueryManager.HandleDataBatch(instigator, queryId, queryType, queryDataLeft, bp);
	}

	private void OnGameQueryReady(EventData eventData)
	{
		int queryId = (int)eventData[99];
		gameDataQueryManager.OnGameQueryReady(queryId);
	}

	private void OnPostWinnerReportEvent()
	{
		IWinningCondition obj = null;
		if (WinningConditionManager.WinningConditionFound)
		{
			List<IWinningCondition> forfilledWinningConditions = WinningConditionManager.GetForfilledWinningConditions();
			if (forfilledWinningConditions.Count == 0)
			{
				Debug.Log("No winning condition found even though server reported game ended.");
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
			Debug.Log("Round was reset without winning condition victory, this is probably due to new winning condition object being added or removed");
		}
		if (OnWinningConditionFulfilled != null)
		{
			OnWinningConditionFulfilled(obj);
		}
	}

	private void OnCollectiblePickedUp(EventData photonEvent)
	{
		int actorNr = (int)photonEvent[254];
		int id = (int)photonEvent[22];
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

	private void OnGetActiveAvatarResponse(int woid)
	{
		if (OnActiveAvatar != null)
		{
			OnActiveAvatar(woid);
		}
	}

	public void OnSetTeamEvent(int actorNr, MVTeam team)
	{
		if (!playerContainer.ContainsKey(actorNr))
		{
			Debug.LogError("!Players.ContainsKey(actorNr): " + actorNr);
		}
		if (playerContainer.ContainsKey(actorNr) && team != playerContainer.GetPlayerUnsafe(actorNr).Team)
		{
			GameStatCounterManager.RemoveTeamScoreOnActorLeave(actorNr, playerContainer.GetPlayerUnsafe(actorNr).Team);
		}
		playerContainer.UpdateTeam(actorNr, team);
		GameStatCounterManager.RemoveStatsFromActor(actorNr);
		if (playerContainer.LocalPlayer.ActorNr == actorNr && MVGameControllerBase.Game.IsPlaying)
		{
			MVGameControllerBase.Game.ResetPlayer();
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
		ItemCategories = new ItemCategories(dictionary);
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
		if (MVGameControllerBase.EditModeUI.PlayerInventoryRepository == null)
		{
			MVGameControllerBase.EditModeUI.PlayerInventoryRepository = new PlayerInventoryRepository();
		}
		foreach (int key in outData.Keys)
		{
			Dictionary<object, object> dictionary = (Dictionary<object, object>)outData[key];
			InventoryItem inventoryItem = new InventoryItem(key, dictionary);
			if (!inventoryItem.isDeleted)
			{
				inventoryItem.slotPosition = (int)dictionary[(byte)22];
				MVGameControllerBase.EditModeUI.PlayerInventoryRepository.AddItem(inventoryItem);
			}
			itemBusinessLogic.AddItemWithNoData(key, inventoryItem.resellable, inventoryItem.itemCategoryID, inventoryItem.itemTypeID, inventoryItem.name);
		}
	}

	private void OnShopInventoryResultSetResponse(Dictionary<object, object> outData, bool isDone)
	{
		if (MVGameControllerBase.EditModeUI.ClientShopRepository == null)
		{
			MVGameControllerBase.EditModeUI.ClientShopRepository = new ClientShopRepository();
		}
		foreach (int key in outData.Keys)
		{
			ShopItem item = new ShopItem(key, outData);
			MVGameControllerBase.EditModeUI.ClientShopRepository.AddItem(item);
		}
		if (isDone)
		{
			MVGameControllerBase.EditModeUI.ClientShopRepository.ReorganizeBySlotPositions();
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

	public void OnAddItemToInventory(Dictionary<byte, object> returnValues, short returnCode)
	{
		if (returnCode == -1)
		{
			Debug.LogWarning("Failed to add to inventory. This is probably because world object was deleted before operation req reached server.");
			MVGameControllerBase.EditModeUI.PlayerInventoryRepository.OnFailedToAddItem();
			return;
		}
		int id = (int)returnValues[22];
		InventoryItem inventoryItem = new InventoryItem(returnValues);
		MVGameControllerBase.EditModeUI.PlayerInventoryRepository.AddItem(inventoryItem);
		itemBusinessLogic.AddItemWithNoData(inventoryItem.itemID, inventoryItem.resellable, inventoryItem.itemCategoryID, inventoryItem.itemTypeID, inventoryItem.name);
		MVWorldObjectClient.CallBackDelegate callBack = (MVWorldObjectClient wo) =>
		{
			wo.ItemId = (int)returnValues[40];
		};
		MVWorldObjectClient worldObjectClient = WorldObjectClientManager.GetWorldObjectClient(id);
		worldObjectClient.TraverseRecursiveTail(callBack);
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
			gameDataQuery = new GameDataQueryManager.GameDataQuery(bytePacker, playerContainer.LocalPlayer.ActorNr, queryType);
		}
		else
		{
			gameDataQuery.AddGameDataQuery(new GameDataQueryManager.GameDataQuery(bytePacker, playerContainer.LocalPlayer.ActorNr, queryType));
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

	private void OnLevelChanged(int actorNr, int level)
	{
		Debug.Log("MVNetworkGame.OnLevelChanged");
		playerContainer.GetPlayerUnsafe(actorNr).Level = level;
	}

	private void OnSetSayChatBubbleVisible(int actorNr, bool visible)
	{
		if (SayChatBubbleVisibilityManager.OnSayChatIndicatorVisibilityChange != null)
		{
			SayChatBubbleVisibilityManager.OnSayChatIndicatorVisibilityChange(actorNr, visible);
		}
	}

	private void LoadModeGui()
	{
		MVGameControllerBase.LevelLoader.LoadScenes(MVGameControllerBase.GameMode, MVGameControllerBase.IsTouristSession, operationRequests.Syncronize);
	}

	public void DebugReturn(DebugLevel level, string debug)
	{
		if (level == DebugLevel.ERROR)
		{
			Debug.LogError("DebugReturn: " + debug);
		}
		Debug.Log(debug);
	}
}
