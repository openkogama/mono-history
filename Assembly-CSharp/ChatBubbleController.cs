using System;
using System.Collections.Generic;
using LivelyChatBubbles;
using UnityEngine;

public class ChatBubbleController : MonoBehaviour
{
	private Dictionary<int, ChatBubble> chatBubbleList;

	private void Start()
	{
		ChatBubbleManager.OnShowChatBubble = (Action<string, int, ChatAnchor>)Delegate.Combine(ChatBubbleManager.OnShowChatBubble, new Action<string, int, ChatAnchor>(ShowChatBubble));
		chatBubbleList = new Dictionary<int, ChatBubble>();
	}

	private void ShowChatBubble(string text, int woid, ChatAnchor chatBubbleAnchor)
	{
		if (!chatBubbleList.ContainsKey(woid))
		{
			ChatBubble chatBubble = UnityEngine.Object.Instantiate(PrefabPool.Instance.ChatBubble);
			chatBubble.transform.SetParent(transform);
			chatBubbleList.Add(woid, chatBubble);
			chatBubbleAnchor.BindAttachedBubble(chatBubbleList[woid]);
		}
		if (chatBubbleList[woid].IsActive)
		{
			chatBubbleList[woid].BindMessageValue(text);
		}
		chatBubbleList[woid].rectTransform.SetAsLastSibling();
		chatBubbleList[woid].gameObject.SetActive(chatBubbleList[woid].IsActive);
	}
}
