using UnityEngine;

public class SayChatBubbleHandler : MonoBehaviour
{
	private const float sayHearingDistance = 15f;

	[SerializeField]
	private MeshRenderer sayChatBubble;

	private bool isActive;

	private void Update()
	{
		if (isActive && MVGameControllerBase.WOCM.AvatarLocal != null)
		{
			float magnitude = (MVGameControllerBase.WOCM.AvatarLocal.Avatar.transform.position - transform.position).magnitude;
			if (15f > magnitude)
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

	public void SetSayBubbleVisibility(bool shouldBeVisible)
	{
		sayChatBubble.gameObject.SetActive(shouldBeVisible);
		isActive = shouldBeVisible;
	}
}
