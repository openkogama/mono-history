using System;
using MV.WorldObject;
using UnityEngine;

public class AvatarController : MonoBehaviour
{
	private InputListener inputListener;

	private MVAvatar mvAvatar;

	private Avatar avatar;

	private AvatarModes modes;

	private AvatarMode mode;

	private AvatarState state;

	private MvCharacterController characterController;

	private float deadTime;

	private float deadInterval = 2.5f;

	public AvatarModes AvatarModes => modes;

	public AvatarState AvatarState => state;

	public bool InGunMode => avatar.InGunMode;

	public AvatarMode Mode
	{
		get
		{
			return mode;
		}
		set
		{
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			if (mode != null)
			{
				mode.Deactivate();
			}
			mode = value;
			if (mode == null)
			{
				return;
			}
			mode.Activate();
			mode.Reset();
			if (value == modes.JetPackMode)
			{
				state = AvatarState.Editing;
				mvAvatar.UnequipAll();
			}
			else if (value == modes.WalkMode)
			{
				if (MVGameController.Instance.WOCM.Terrain != null)
				{
					MVGameController.Instance.WOCM.UpdateWorldBounds(SharedCubeFunctions.GetAxisAlignedBoundsRecursively(MVGameController.Instance.WOCM.Terrain.GameObject.transform).Value);
				}
				state = AvatarState.Playing;
			}
		}
	}

	public MVAvatar MVAvatar => mvAvatar;

	public Avatar Avatar => avatar;

	public void Initialize(MVAvatar mvAvatar, Avatar avatar)
	{
		this.mvAvatar = mvAvatar;
		this.avatar = avatar;
		characterController = ((Component)this).GetComponent<MvCharacterController>();
		characterController.Init(mvAvatar.Id);
		modes = new AvatarModes();
		modes.JetPackMode = new JetPackMode(characterController, this);
		modes.WalkMode = new WalkMode(characterController, this);
		if (MVGameController.Instance.Game.EditorMode)
		{
			Mode = modes.JetPackMode;
			state = AvatarState.Editing;
		}
		else
		{
			Mode = modes.WalkMode;
			state = AvatarState.Playing;
		}
		inputListener = ((Component)this).gameObject.AddComponent<InputListener>();
		inputListener.Init(this);
		((Behaviour)inputListener).enabled = true;
		MVRuntimeDataVariableClampedFloat health = mvAvatar.Health;
		health.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(health.OnChange, (MVRuntimeDataVariable.OnChangeDelegate)((object obj) =>
		{
			if ((float)obj == 0f)
			{
				Die();
			}
		}));
	}

	public void Die()
	{
		state = AvatarState.Dead;
		deadTime = Time.time;
		mvAvatar.Animation.Value = "Dead";
		mvAvatar.UnequipAll();
		((Component)avatar).audio.Play();
	}

	public void Respawn()
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		MVSpawnPoint validSpawnPoint = MVGameController.Instance.WOCM.GetValidSpawnPoint();
		if (validSpawnPoint == null)
		{
			Debug.LogError((object)"No spawn-point found on planet!");
			((Component)avatar).transform.position = MVGameController.Instance.WOCM.GetValidAvatarStartPosition();
			((Component)avatar).transform.rotation = Quaternion.identity;
		}
		else
		{
			((Component)avatar).transform.position = validSpawnPoint.GameObject.transform.position;
			((Component)avatar).transform.rotation = validSpawnPoint.GameObject.transform.rotation;
			MVGameController.Instance.WOCM.WeCamera.Respawn();
		}
		mvAvatar.State = MVWorldObjectState.Dirty;
		mvAvatar.Animation.Value = "Walk";
		modes.JetPackMode.Reset();
		modes.WalkMode.Reset();
		mvAvatar.Health.Value = 100f;
		state = AvatarState.Playing;
	}

	public void HandleInput(NetworkInputActionCodes actionCode, NetworkInputKeyCodes keyCode)
	{
		if (mode != null)
		{
			mode.HandleInput(actionCode, keyCode);
		}
		if (keyCode == NetworkInputKeyCodes.Fire)
		{
			mvAvatar.IsFiring.Value = actionCode == NetworkInputActionCodes.Down;
		}
	}

	public void ApplyImpulse(Vector3 impulse, bool suspendImpactDamage = false)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		mode.ApplyImpulse(impulse, suspendImpactDamage);
	}

	public void ApplyProximityDamage(float damage)
	{
		if (state != AvatarState.Editing)
		{
			mvAvatar.Health.Value -= damage;
		}
	}

	private void FixedUpdate()
	{
		switch (state)
		{
		case AvatarState.Editing:
		case AvatarState.Playing:
			Mode.FixedUpdate();
			mvAvatar.State = MVWorldObjectState.Dirty;
			break;
		case AvatarState.Dead:
			if (Time.time - deadTime > deadInterval)
			{
				Respawn();
			}
			break;
		}
	}
}
