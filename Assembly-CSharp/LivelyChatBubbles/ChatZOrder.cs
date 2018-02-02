using System.Linq;
using UnityEngine;

namespace LivelyChatBubbles;

public class ChatZOrder : MonoBehaviour
{
	private void OnEnable()
	{
		InvokeRepeating("Process", 0f, 0.25f);
	}

	private void OnDisable()
	{
		CancelInvoke("Process");
	}

	private void Process()
	{
		Camera camera = Camera.main;
		(from d in GetComponentsInChildren<ChatAnchor>()
			where d.AttachedBubble
			orderby Vector3.Distance(d.transform.position, camera.transform.position)
			select d.AttachedBubble.rectTransform).ToList().ForEach((RectTransform d) =>
		{
			d.SetAsFirstSibling();
		});
	}
}
