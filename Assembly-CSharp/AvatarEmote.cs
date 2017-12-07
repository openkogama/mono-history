using System;

public class AvatarEmote
{
	public Action<EmoteTypes> OnEmoteEnd;

	protected EmoteTypes emote;

	protected AvatarLimbManager limbManager;

	protected bool isActive;

	protected float duration;

	protected float lifeTime;

	public float LifeTime => lifeTime;

	public virtual void Initialize(AvatarLimbManager limbManager, float lifeTime)
	{
		this.limbManager = limbManager;
		this.lifeTime = lifeTime;
	}

	public virtual void StartEmote()
	{
		isActive = true;
		duration = lifeTime;
	}

	public virtual void Update()
	{
	}

	public virtual void StopEmote()
	{
		duration = 0f;
		isActive = false;
		if (OnEmoteEnd != null)
		{
			OnEmoteEnd(emote);
		}
	}
}
