using System;
using System.Collections;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class MVPlayerContainer : IEnumerator, IEnumerable, IEnumerable<KeyValuePair<int, MVPlayer>>
{
	private readonly Dictionary<int, MVPlayer> players = new Dictionary<int, MVPlayer>();

	private readonly Dictionary<int, MVPlayer> pendingPlayers = new Dictionary<int, MVPlayer>();

	private bool sendPlayerListChangeEvents = true;

	private int localPlayerActorNumber = -1;

	public Action OnPlayerListChanged;

	public Action OnPlayerListLoaded;

	public MVLocalPlayer LocalPlayer => (MVLocalPlayer)GetPlayerUnsafe(localPlayerActorNumber);

	public int Count => players.Count;

	public int PendingPlayersCount => pendingPlayers.Count;

	public Dictionary<int, MVPlayer>.ValueCollection Values => players.Values;

	public MVPlayer this[int actorNumber] => players[actorNumber];

	public object Current => players.GetEnumerator().Current;

	IEnumerator IEnumerable.GetEnumerator()
	{
		return players.GetEnumerator();
	}

	public void SetLocalPlayer(int actorNumber)
	{
		localPlayerActorNumber = actorNumber;
	}

	public bool TryGetPlayerByProfileId(int profileId, out MVPlayer player)
	{
		if (profileId <= 0)
		{
			Debug.LogError("Trying to get tourist profile by profileId");
			player = null;
			return false;
		}
		foreach (MVPlayer value in Values)
		{
			if (value.ProfileID == profileId)
			{
				player = value;
				return true;
			}
		}
		player = null;
		return false;
	}

	public void Add(MVPlayer player)
	{
		Debug.Log("player.IsReady " + player.IsReady);
		if (player.IsReady)
		{
			players.Add(player.ActorNr, player);
			SendPlayerListEvents();
		}
		else
		{
			Debug.Log("Adding to pending players " + player.ActorNr);
			pendingPlayers.Add(player.ActorNr, player);
		}
	}

	public void Add(List<MVPlayer> playerList)
	{
		sendPlayerListChangeEvents = false;
		bool flag = false;
		foreach (MVPlayer player in playerList)
		{
			if (player.IsReady)
			{
				flag = true;
			}
			Add(player);
		}
		sendPlayerListChangeEvents = true;
		if (flag)
		{
			SendPlayerListEvents();
		}
		if (OnPlayerListLoaded != null)
		{
			OnPlayerListLoaded();
		}
	}

	public void Remove(int actorNr)
	{
		if (players.ContainsKey(actorNr) && pendingPlayers.ContainsKey(actorNr))
		{
			Debug.LogError("Player contained in both players and pending players");
		}
		if (!players.ContainsKey(actorNr) && !pendingPlayers.ContainsKey(actorNr))
		{
			Debug.LogError("Player not contained in either players or pendingPlayers");
		}
		if (players.ContainsKey(actorNr))
		{
			players.Remove(actorNr);
			SendPlayerListEvents();
		}
		if (pendingPlayers.ContainsKey(actorNr))
		{
			pendingPlayers.Remove(actorNr);
		}
	}

	public void UpdateTeam(int actorNr, MVTeam team)
	{
		MVPlayer playerUnsafe = GetPlayerUnsafe(actorNr);
		playerUnsafe.Team = team;
		if (playerUnsafe.IsReady)
		{
			SendPlayerListEvents();
		}
	}

	public MVPlayer GetPlayerUnsafe(int actorNr)
	{
		if (pendingPlayers.ContainsKey(actorNr))
		{
			return pendingPlayers[actorNr];
		}
		if (players.ContainsKey(actorNr))
		{
			return players[actorNr];
		}
		Debug.Log(StackTraceUtility.ExtractStackTrace());
		Debug.LogError("Could not get player unsafe");
		return null;
	}

	public void SetPlayerReady(int actorNr)
	{
		if (!pendingPlayers.ContainsKey(actorNr) && players.ContainsKey(actorNr))
		{
			Debug.LogError("Player already added to players list. IsReady: " + players[actorNr].IsReady);
			return;
		}
		if (!pendingPlayers.ContainsKey(actorNr))
		{
			Debug.LogError("Pending player not found");
			return;
		}
		MVPlayer mVPlayer = pendingPlayers[actorNr];
		pendingPlayers.Remove(actorNr);
		players.Add(mVPlayer.ActorNr, mVPlayer);
		mVPlayer.SetReady();
	}

	public bool TryGetValue(int actorNr, out MVPlayer player)
	{
		return players.TryGetValue(actorNr, out player);
	}

	public bool ContainsKey(int actorNr)
	{
		return players.ContainsKey(actorNr);
	}

	public IEnumerator<KeyValuePair<int, MVPlayer>> GetEnumerator()
	{
		return players.GetEnumerator();
	}

	public bool MoveNext()
	{
		return players.GetEnumerator().MoveNext();
	}

	public void Reset()
	{
		((IEnumerator)players.GetEnumerator()).Reset();
		players.GetEnumerator().MoveNext();
	}

	private void SendPlayerListEvents()
	{
		if (sendPlayerListChangeEvents && OnPlayerListChanged != null)
		{
			OnPlayerListChanged();
		}
	}
}
