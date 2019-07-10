using System.Collections.Generic;
using UnityEngine;

public class FirstTimeAvatarJumpAnimator : MonoBehaviour
{
	private int jumps = 6;

	private string jumpAnimationName = "Jump";

	private MVAvatarLocal avatarLocal;

	public void Initialize(MVAvatarLocal avatarLocal)
	{
		this.avatarLocal = avatarLocal;
	}

	private void Update()
	{
		if (!avatarLocal.Body.Animation.IsPlaying(jumpAnimationName) && jumps > 1)
		{
			avatarLocal.Body.Animation.Play(jumpAnimationName);
			jumps--;
		}
		else if (jumps == 1)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary["state"] = "Walk";
			dictionary["timeStamp"] = MVGameControllerBase.Game.ServerTimeInMilliSeconds;
			avatarLocal.Body.Animation.ComputeBlendAnimation(dictionary);
			jumps--;
		}
		else if (jumps == 0)
		{
			Dictionary<object, object> dictionary2 = new Dictionary<object, object>();
			dictionary2["state"] = "Idle";
			dictionary2["timeStamp"] = MVGameControllerBase.Game.ServerTimeInMilliSeconds;
			avatarLocal.Body.Animation.ComputeBlendAnimation(dictionary2);
			Object.Destroy(this);
		}
	}
}
