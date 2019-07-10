using System;
using System.Collections.Generic;
using UnityEngine;

public class ChatBubbleController : MonoBehaviour
{
	private Dictionary<int, ChatBubble> chatBubbleList;

	private void Start()
	{
		ChatBubbleManager.OnShowChatBubble = (Action<string, int, ChatAnchor>)Delegate.Combine(ChatBubbleManager.OnShowChatBubble, new Action<string, int, ChatAnchor>(ShowChatBubble));
		chatBubbleList = new Dictionary<int, ChatBubble>();
	}

	private void ShowChatBubble(string text, int anchorId, ChatAnchor chatBubbleAnchor)
	{
		if (gameObject.activeInHierarchy)
		{
			if (!chatBubbleList.ContainsKey(anchorId))
			{
				ChatBubble chatBubble = UnityEngine.Object.Instantiate(PrefabPool.Instance.ChatBubble);
				chatBubble.transform.SetParent(transform);
				chatBubbleList.Add(anchorId, chatBubble);
				chatBubbleAnchor.BindAttachedBubble(chatBubbleList[anchorId]);
			}
			if (chatBubbleList[anchorId].IsActive)
			{
				chatBubbleList[anchorId].BindMessageValue(text);
			}
			chatBubbleList[anchorId].rectTransform.SetAsLastSibling();
		}
	}
}
