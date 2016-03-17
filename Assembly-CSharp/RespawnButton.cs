using UnityEngine;

public class RespawnButton : MonoBehaviour
{
	public void Respawn()
	{
		MVGameControllerBase.WOCM.AvatarLocal.Respawn();
	}
}
