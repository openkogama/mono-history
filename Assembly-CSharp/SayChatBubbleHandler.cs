using System;
using System.Collections.Generic;
using UnityEngine;

public class SayChatBubbleHandler : MonoBehaviour
{
	[SerializeField]
	private MeshRenderer sayChatBubble;

	private const float sayHearingDistance = 15f;

	private bool isActive;

	private bool isIndicatorActive;

	private int ownerActorNr;

	private ChatAnchor chatAnchor;

	public ChatAnchor ChatAnchor => chatAnchor;

	public void Initialize(int actorNr, ChatAnchor chatAnchor)
	{
		ownerActorNr = actorNr;
		this.chatAnchor = chatAnchor;
		SayChatBubbleVisibilityManager.OnSayChatIndicatorVisibilityChange = (Action<int, bool>)Delegate.Combine(SayChatBubbleVisibilityManager.OnSayChatIndicatorVisibilityChange, new Action<int, bool>(SetSayBubbleIndicatorVisibility));
		SayChatBubbleVisibilityManager.OnSayChatMessageRecieved = (Action<int, Dictionary<object, object>>)Delegate.Combine(SayChatBubbleVisibilityManager.OnSayChatMessageRecieved, new Action<int, Dictionary<object, object>>(OnSayChatMessageRecieved));
	}

	public void Activate()
	{
		isActive = true;
	}

	public void Deactivate()
	{
		isActive = false;
		sayChatBubble.gameObject.SetActive(value: false);
		isIndicatorActive = false;
	}

	public void OnSayChatMessageRecieved(int actorNr, Dictionary<object, object> data)
	{
		if (isActive && ownerActorNr == actorNr && IsPlayerInHearingDistance())
		{
			int instanceID = chatAnchor.GetInstanceID();
			string text = (string)data[(byte)5];
			ChatBubbleManager.ShowChatBubble(text, instanceID, chatAnchor);
			if (SayChatBubbleVisibilityManager.OnSayChatMessageHeard != null)
			{
				SayChatBubbleVisibilityManager.OnSayChatMessageHeard(data);
			}
		}
	}

	public void SetSayBubbleIndicatorVisibility(int actorNr, bool shouldBeVisible)
	{
		if (isActive && ownerActorNr == actorNr)
		{
			sayChatBubble.gameObject.SetActive(shouldBeVisible);
			isIndicatorActive = shouldBeVisible;
		}
	}

	public bool IsPlayerInHearingDistance()
	{
		if (!MVGameControllerBase.Game.LocalPlayer.IsReady)
		{
			return false;
		}
		float magnitude = (MVGameControllerBase.SpawnRoleDataMediatorLocal.Position - transform.position).magnitude;
		return 15f > magnitude;
	}

	private void Update()
	{
		if (isIndicatorActive)
		{
			if (IsPlayerInHearingDistance())
			{
				Color white = Color.white;
				sayChatBubble.material.color = white;
				return;
			}
			Color white2 = Color.white;
			white2.r = 100f / 255f;
			white2.g = 100f / 255f;
			white2.b = 100f / 255f;
			white2.a = 100f / 255f;
			sayChatBubble.material.color = white2;
		}
	}

	private void OnDestroy()
	{
		SayChatBubbleVisibilityManager.OnSayChatIndicatorVisibilityChange = (Action<int, bool>)Delegate.Remove(SayChatBubbleVisibilityManager.OnSayChatIndicatorVisibilityChange, new Action<int, bool>(SetSayBubbleIndicatorVisibility));
		SayChatBubbleVisibilityManager.OnSayChatMessageRecieved = (Action<int, Dictionary<object, object>>)Delegate.Remove(SayChatBubbleVisibilityManager.OnSayChatMessageRecieved, new Action<int, Dictionary<object, object>>(OnSayChatMessageRecieved));
	}
}
