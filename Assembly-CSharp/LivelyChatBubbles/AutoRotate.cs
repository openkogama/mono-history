using UnityEngine;

namespace LivelyChatBubbles;

public class AutoRotate : MonoBehaviour
{
	public Vector3 Angle;

	private void Update()
	{
		transform.Rotate(Angle * Time.deltaTime);
	}
}
