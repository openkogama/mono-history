using UnityEngine;

namespace LivelyChatBubbles;

public class AutoFace : MonoBehaviour
{
	public Transform Target;

	private void Update()
	{
		if ((bool)Target)
		{
			transform.LookAt(Target);
		}
	}
}
