using System;
using UnityEngine;

public abstract class ActivateOnAnimationBase : MonoBehaviour
{
	private MVAvatar mvAvatar;

	protected virtual void Start()
	{
		Avatar avatar = GetAvatar();
		if (!(avatar == null))
		{
			mvAvatar = avatar.mvAvatar;
			BoneAnimation animation = mvAvatar.Body.Animation;
			animation.OnAnimationChange = (Action<string>)Delegate.Combine(animation.OnAnimationChange, new Action<string>(OnAvatarAnimationChange));
			AvatarLimbManager limbManager = mvAvatar.LimbManager;
			limbManager.OnEmoteStart = (Action<string>)Delegate.Combine(limbManager.OnEmoteStart, new Action<string>(OnAvatarAnimationChange));
		}
	}

	private void OnDestroy()
	{
		if (mvAvatar != null)
		{
			BoneAnimation animation = mvAvatar.Body.Animation;
			animation.OnAnimationChange = (Action<string>)Delegate.Remove(animation.OnAnimationChange, new Action<string>(OnAvatarAnimationChange));
			AvatarLimbManager limbManager = mvAvatar.LimbManager;
			limbManager.OnEmoteStart = (Action<string>)Delegate.Remove(limbManager.OnEmoteStart, new Action<string>(OnAvatarAnimationChange));
		}
	}

	private Avatar GetAvatar()
	{
		Avatar avatar = null;
		Transform parent = transform.parent;
		while (parent != null)
		{
			avatar = parent.GetComponent<Avatar>();
			parent = parent.parent;
			if (avatar != null)
			{
				break;
			}
		}
		return avatar;
	}

	public abstract void OnAvatarAnimationChange(string newAnimation);
}
