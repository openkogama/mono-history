using System.Collections;
using ExitGames.Client.Photon;

namespace ExitGames.RealtimeDemo;

public class LitePeer : PhotonPeer
{
	public LitePeer(IPhotonPeerListener listener)
		: base(listener, useTcp: false)
	{
		Listener.DebugReturn(DebugLevel.INFO, "ExitGames.RealtimeDemo.LitePeer()");
	}

	public LitePeer(IPhotonPeerListener listener, bool useTcp)
		: base(listener, useTcp)
	{
		Listener.DebugReturn(DebugLevel.INFO, "ExitGames.RealtimeDemo.LitePeer()");
	}

	public virtual short OpRaiseEvent(byte eventCode, Hashtable evData, bool sendReliable)
	{
		return OpRaiseEvent(eventCode, evData, sendReliable, 0);
	}

	public virtual short OpRaiseEvent(byte eventCode, Hashtable evData, bool sendReliable, byte channelId)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[LiteOpKey.Data] = evData;
		hashtable[LiteOpKey.Code] = eventCode;
		return OpCustom(92, hashtable, sendReliable, channelId);
	}

	public virtual short OpRaiseEvent(byte eventCode, Hashtable evData, bool sendReliable, byte channelId, bool encrypt)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[LiteOpKey.Data] = evData;
		hashtable[LiteOpKey.Code] = eventCode;
		return OpCustom(92, hashtable, sendReliable, channelId, encrypt);
	}

	public virtual short OpSetPropertiesOfActor(int actorNr, Hashtable properties, bool broadcast, byte channelId)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add(LiteOpKey.Properties, properties);
		hashtable.Add(LiteOpKey.ActorNr, actorNr);
		if (broadcast)
		{
			hashtable.Add(LiteOpKey.Broadcast, broadcast);
		}
		return OpCustom(93, hashtable, sendReliable: true, channelId);
	}

	public virtual short OpSetPropertiesOfGame(Hashtable properties, bool broadcast, byte channelId)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add(LiteOpKey.Properties, properties);
		if (broadcast)
		{
			hashtable.Add(LiteOpKey.Broadcast, broadcast);
		}
		return OpCustom(93, hashtable, sendReliable: true, channelId);
	}

	public virtual short OpGetProperties(byte channelId)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add(LiteOpKey.Properties, (byte)3);
		return OpCustom(94, hashtable, sendReliable: true, channelId);
	}

	public virtual short OpGetPropertiesOfActor(int[] actorNrList, string[] properties, byte channelId)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add(LiteOpKey.Properties, LitePropertyTypes.Actor);
		if (properties != null)
		{
			hashtable.Add(LiteOpKey.ActorProperties, properties);
		}
		if (actorNrList != null)
		{
			hashtable.Add(LiteOpKey.ActorNr, actorNrList);
		}
		return OpCustom(94, hashtable, sendReliable: true, channelId);
	}

	public virtual short OpGetPropertiesOfActor(int[] actorNrList, byte[] properties, byte channelId)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add(LiteOpKey.Properties, LitePropertyTypes.Actor);
		if (properties != null)
		{
			hashtable.Add(LiteOpKey.ActorProperties, properties);
		}
		if (actorNrList != null)
		{
			hashtable.Add(LiteOpKey.ActorNr, actorNrList);
		}
		return OpCustom(94, hashtable, sendReliable: true, channelId);
	}

	public virtual short OpGetPropertiesOfGame(string[] properties, byte channelId)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add(LiteOpKey.Properties, LitePropertyTypes.Game);
		if (properties != null)
		{
			hashtable.Add(LiteOpKey.GameProperties, properties);
		}
		return OpCustom(94, hashtable, sendReliable: true, channelId);
	}

	public virtual short OpGetPropertiesOfGame(byte[] properties, byte channelId)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add(LiteOpKey.Properties, LitePropertyTypes.Game);
		if (properties != null)
		{
			hashtable.Add(LiteOpKey.GameProperties, properties);
		}
		return OpCustom(94, hashtable, sendReliable: true, channelId);
	}

	public virtual short OpJoin(string gameName)
	{
		return OpJoin(gameName, null, null, broadcastActorProperties: false);
	}

	public virtual short OpJoin(string gameName, Hashtable gameProperties, Hashtable actorProperties, bool broadcastActorProperties)
	{
		if ((int)DebugOut >= 5)
		{
			Listener.DebugReturn(DebugLevel.ALL, "OpJoin(" + gameName + ")");
		}
		Hashtable hashtable = new Hashtable();
		hashtable[(byte)4] = gameName;
		if (actorProperties != null)
		{
			hashtable[(byte)14] = actorProperties;
		}
		if (gameProperties != null)
		{
			hashtable[(byte)15] = gameProperties;
		}
		if (broadcastActorProperties)
		{
			hashtable[(byte)13] = broadcastActorProperties;
		}
		return OpCustom(90, hashtable, sendReliable: true, 0);
	}

	public virtual short OpLeave(string gameName)
	{
		if ((int)DebugOut >= 5)
		{
			Listener.DebugReturn(DebugLevel.ALL, "OpLeave()");
		}
		Hashtable hashtable = new Hashtable();
		hashtable[(byte)4] = gameName;
		return OpCustom(91, hashtable, sendReliable: true, 0);
	}
}
