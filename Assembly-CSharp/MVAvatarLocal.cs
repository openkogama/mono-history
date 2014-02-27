using System;
using System.Collections;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class MVAvatarLocal : MVAvatar, ILocalObject
{
	private enum AvatarState
	{
		Editing,
		Playing,
		Dead
	}

	private const float maxFallBelow = 200f;

	private AvatarMotor avatarMotor;

	private MVInteractableBase interactableLocal;

	private UseInteractorHandler useInteractorHandler;

	private MVTriggerHandler triggerHandler;

	private float deadTime;

	private float deadInterval = 2.5f;

	private float respawnTime;

	private float respawnTimeOut = 2f;

	private AvatarModes modes;

	private AvatarMode mode;

	private AvatarState state;

	private ThirdPersonCamera thirdPersonCam;

	private bool showingRespawnDelay;

	private AvatarPickupOwner pickupOwner;

	private PickupGUI pickupGUI;

	private bool CanReceivePackages => Time.time - respawnTime > respawnTimeOut;

	private bool IsSeated
	{
		get
		{
			if (!RunTimeData.ContainsKey("seat"))
			{
				Debug.LogError((object)"MVAvatarLocal does not contain key seat");
				return false;
			}
			return (int)RunTimeData["seat"] != -1;
		}
	}

	private bool InGunMode
	{
		get
		{
			if ((Object)(object)pickupOwner == (Object)null)
			{
				return false;
			}
			return pickupOwner.InGunMode;
		}
	}

	public bool ForceRotateAvatarToFiringDirection { private get; set; }

	public ILaserPointer LaserPointer => avatarPickupOwner.LaserPointer;

	public MVRigidBody RigidBody => avatarMotor;

	public bool IsDead => state == AvatarState.Dead;

	public bool IsEnteringVehicle => MVGameController.Instance.Game.PlayerController.IsEnteringVehicle;

	public AvatarModes AvatarModes => modes;

	public AvatarMode AvatarMode
	{
		get
		{
			return Mode;
		}
		set
		{
			if (value is WalkMode)
			{
				Mode = value;
				ShowHealth = true;
				ResetAvatar();
				gameObject.collider.enabled = true;
			}
			else if (value is JetPackMode)
			{
				ResetAvatar();
				Mode = value;
				ShowHealth = false;
				gameObject.collider.enabled = false;
			}
		}
	}

	public bool ShowHealth
	{
		set
		{
			avatar.ShowHealth = value;
		}
	}

	public HealthBar HealthAndOxygenBar => avatar.HealthAndOxygenBar;

	private AvatarMode Mode
	{
		get
		{
			return mode;
		}
		set
		{
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
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
			RigidBody.Reset();
			if (value == modes.JetPackMode)
			{
				state = AvatarState.Editing;
				MVEquipable component = gameObject.GetComponent<MVEquipable>();
				if ((Object)(object)component != (Object)null)
				{
					component.Unequip();
				}
				MVTriggerHandler component2 = gameObject.GetComponent<MVTriggerHandler>();
				if ((Object)(object)component2 == (Object)null)
				{
					Debug.LogError((object)"TriggerHandler on avatar is null");
				}
				else
				{
					((Behaviour)component2).enabled = false;
				}
				MVGameController.Instance.Game.LocalPlayer.ResetCheckpoint();
			}
			else if (value == modes.WalkMode)
			{
				MVGameController.Instance.WOCM.UpdateWorldBounds(SharedCubeFunctions.GetAxisAlignedBoundsRecursively(MVGameController.Instance.WOCM.GetSingletonWorldObject<MVCubeModelPrototypeTerrain>().Transform).Value);
				MVTriggerHandler component3 = gameObject.GetComponent<MVTriggerHandler>();
				if ((Object)(object)component3 == (Object)null)
				{
					Debug.LogError((object)"TriggerHandler on avatar is null");
				}
				else
				{
					((Behaviour)component3).enabled = true;
				}
				state = AvatarState.Playing;
			}
		}
	}

	public event EventHandler Respawned;

	public MVAvatarLocal(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, worldObjects)
	{
		SetNetworkObject(local: true);
	}

	public override void Initialize()
	{
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize();
		avatar.NameTag = string.Empty;
		avatarMotor = gameObject.AddComponent<AvatarMotor>();
		triggerHandler = gameObject.AddComponent<MVTriggerHandler>();
		AvatarInteractable avatarInteractable = gameObject.AddComponent<AvatarInteractable>();
		avatarInteractable.Init(Modifiers, Invulnerable, Health);
		interactableLocal = avatarInteractable;
		AvatarEquipable avatarEquipable = gameObject.AddComponent<AvatarEquipable>();
		avatarEquipable.Init(interactableLocal, CurrentItem);
		avatarMotor.Init(interactableLocal, CharacterControllerCenterOffset);
		Initialize(avatarPickupOwner, avatarMotor);
		MVGameController.Instance.WOCM.AvatarLocal = this;
		useInteractorHandler = gameObject.AddComponent<UseInteractorHandler>();
		useInteractorHandler.Init(gameObject.collider);
		MVGameController.Instance.Game.PlayerController.Push(this);
		InitializeCamera();
	}

	public void SetCharacterController(MvCharacterController characterController)
	{
		avatarMotor.CharacterController = characterController;
	}

	public void LeaveVehicle()
	{
		int vehicleID = -1;
		if (!MVGameController.Instance.Game.PlayerController.DetachWorldObjectFromVehicle(Id, ref vehicleID))
		{
			return;
		}
		if (vehicleID != -1)
		{
			MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(vehicleID);
			if (worldObjectClient != null && worldObjectClient is MVVehicleBase)
			{
				((MVVehicleBase)worldObjectClient).LeaveLocal();
			}
			else
			{
				Debug.LogError((object)("vehicleWO is null or type is not IVehicle " + vehicleID));
			}
		}
		HandleLeaveVehicle();
		MVGameController.Instance.Game.CameraController.SetCamera(CameraType.ThirdPerson);
		RigidBody.Reset();
		((Behaviour)RigidBody).enabled = true;
		triggerHandler.Reset();
		((Behaviour)triggerHandler).enabled = true;
		if (networkObject == null || networkObject is MVNetworkListener)
		{
			Debug.LogError((object)"NetworkObject null or listener on detach. Should be reporter");
		}
		else
		{
			((MVNetworkReporter)networkObject).suspendTransformReporting = false;
		}
	}

	public override void VehicleEntered()
	{
		if ((Object)(object)Group.GameObject.GetComponent<MVRigidBody>() != (Object)null)
		{
			((Behaviour)RigidBody).enabled = false;
			((Behaviour)triggerHandler).enabled = false;
		}
	}

	public MovementMap Update(MovementMap movementMap)
	{
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		UpdateInvulnerable();
		if (state == AvatarState.Playing)
		{
			pickupOwner.HandleFire(movementMap.Fire, IsFiring);
		}
		if (state != AvatarState.Dead && ((Behaviour)RigidBody).enabled)
		{
			Mode.FrameUpdate(movementMap);
		}
		if (InGunMode)
		{
			pickupGUI.Update();
			if (Object.op_Implicit((Object)(object)thirdPersonCam))
			{
				thirdPersonCam.lookAtOffset = thirdPersonCam.shoulderOffset;
			}
		}
		else if (Object.op_Implicit((Object)(object)thirdPersonCam))
		{
			thirdPersonCam.lookAtOffset = new Vector3(0f, 0.7f, 0f);
		}
		if (!MVGameController.Instance.Game.IsPlaying)
		{
			return movementMap;
		}
		float y = gameObject.transform.position.y;
		Bounds worldBounds = MVGameController.Instance.WOCM.WorldBounds;
		if (y < worldBounds.min.y - 200f && !IsDead)
		{
			DieByFalling();
			Debug.Log((object)"Death by falling");
		}
		if (movementMap.Use && !RigidBody.IsMovementLocked)
		{
			if (IsSeated)
			{
				LeaveVehicle();
			}
			else
			{
				useInteractorHandler.Use();
			}
		}
		if (movementMap.Drop)
		{
			gameObject.GetComponent<MVEquipable>().Equip(AvatarItemType.Hand, null);
		}
		if (!IsSeated && avatarMotor.IsStuck())
		{
			HandleStuck();
		}
		return movementMap;
	}

	public MovementMap FixedUpdate(MovementMap movementMap)
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		modes.WalkMode.InGunMode = InGunMode;
		modes.WalkMode.ForceRotateAvatarToFiringDirection = ForceRotateAvatarToFiringDirection;
		switch (state)
		{
		case AvatarState.Editing:
			FixedUpdateCurrentMode(movementMap);
			break;
		case AvatarState.Playing:
			FixedUpdateCurrentMode(movementMap);
			if ((InGunMode || ForceRotateAvatarToFiringDirection) && ((Behaviour)RigidBody).enabled)
			{
				Vector3 velocity = RigidBody.Velocity;
				if (velocity.sqrMagnitude > 0.001f)
				{
					RotateAvatarToFiringDirection();
				}
			}
			if (!((Behaviour)avatarMotor).enabled)
			{
				avatarMotor.UpdateVelocity();
			}
			break;
		case AvatarState.Dead:
			FixedUpdateCurrentMode(movementMap);
			if (Time.time - deadTime > deadInterval)
			{
				Respawn();
			}
			break;
		}
		return movementMap;
	}

	public void SetAnimation(string animationState)
	{
		Hashtable hashtable = (Hashtable)Animation.Value;
		string text = (string)hashtable["state"];
		if (text != animationState)
		{
			int serverTimeInMilliSeconds = MVGameController.Instance.Game.ServerTimeInMilliSeconds;
			Hashtable hashtable2 = new Hashtable();
			hashtable2.Add("state", animationState);
			hashtable2.Add("timeStamp", serverTimeInMilliSeconds);
			hashtable = hashtable2;
			Animation.Value = hashtable;
		}
	}

	public void Respawn(bool suicide = false, bool delayRespawn = false)
	{
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		Debug.Log((object)"Respawn!");
		MVTriggerHandler component = gameObject.GetComponent<MVTriggerHandler>();
		if ((Object)(object)component == (Object)null)
		{
			Debug.LogError((object)"TriggerHandler on avatar is null");
		}
		else
		{
			((Behaviour)component).enabled = true;
		}
		if (showingRespawnDelay)
		{
			return;
		}
		if (!MVGameController.Instance.Game.TeamManager.IsTeamActive(MVGameController.Instance.Game.TeamManager.GetTeamFromActorNr(OwnerActorNr)))
		{
			List<MVTeam> teamList = MVGameController.Instance.Game.TeamManager.GetTeamList();
			MVGameController.Instance.Game.SetTeam((teamList.Count != 1) ? teamList[0] : MVTeam.None);
		}
		respawnTime = Time.time;
		ResetAvatar();
		MVCheckpoint checkpoint = MVGameController.Instance.Game.LocalPlayer.GetCheckpoint();
		if (checkpoint != null)
		{
			gameObject.transform.position = checkpoint.WorldPosition;
			gameObject.transform.rotation = checkpoint.WorldRotation;
			MVGameController.Instance.Game.CameraController.Respawn();
		}
		else
		{
			MVLogicObject validSpawnPoint = MVGameController.Instance.WOCM.GetValidSpawnPoint();
			if (validSpawnPoint == null)
			{
				Debug.LogError((object)"No spawn-point found on planet!");
			}
			else
			{
				if (suicide && state != AvatarState.Dead)
				{
					Debug.Log((object)"Avatar dead");
					MVGameController.Instance.Game.PostGameMsg(MVGameMsgType.AvatarKilled, GameMessages.MakePlayerKilledMessage(MVGameController.Instance.Game.LocalPlayerActorNumber, MVGameController.Instance.Game.LocalPlayerActorNumber, PlayerKilledByType.Suicide));
				}
				GameObject.transform.position = validSpawnPoint.WorldPosition;
				GameObject.transform.rotation = validSpawnPoint.WorldRotation;
				MVGameController.Instance.Game.CameraController.Respawn();
			}
		}
		if (Respawned != null)
		{
			Respawned(this, EventArgs.Empty);
		}
	}

	protected override void AttachBody(MVBody newBody)
	{
		base.AttachBody(newBody);
		if (!MVGameController.Instance.Game.IsPlaying)
		{
			newBody.Visible = false;
		}
		else
		{
			newBody.Visible = true;
		}
		MVGameController.Instance.Game.TransferOwnership(newBody.Id, 0, null);
		foreach (MVWorldObjectClient child in newBody.Children)
		{
			MVGameController.Instance.Game.TransferOwnership(child.Id, 0, null);
		}
	}

	private void DieByFalling()
	{
		Health.Value = 0f;
		MVGameController.Instance.Game.PostGameMsg(MVGameMsgType.AvatarKilled, GameMessages.MakePlayerKilledMessage(MVGameController.Instance.Game.LocalPlayerActorNumber, MVGameController.Instance.Game.LocalPlayerActorNumber, PlayerKilledByType.FallOffWorld));
	}

	private void RotateAvatarToFiringDirection()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		Vector3 lookDirection = avatarPickupOwner.LookDirection;
		lookDirection.y = 0f;
		((Component)avatarMotor.CharacterController).transform.forward = lookDirection;
	}

	private void InitializeCamera()
	{
		if (MVGameController.Instance.Game.GameMode == MVGameMode.Play)
		{
			MVGameController.Instance.Game.CameraController.SetCamera(CameraType.ThirdPerson);
		}
		else
		{
			MVGameController.Instance.Game.CameraController.SetCamera(CameraType.JetPackCamera);
		}
	}

	private void FixedUpdateCurrentMode(MovementMap movementMap)
	{
		if (((Behaviour)RigidBody).enabled)
		{
			Mode.FixedUpdate(movementMap);
			State = MVWorldObjectState.Dirty;
		}
	}

	private void Initialize(AvatarPickupOwner pickupOwner, AvatarMotor avatarMotor)
	{
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		thirdPersonCam = MVGameController.Instance.Game.CameraController.GetCamera<ThirdPersonCamera>();
		this.pickupOwner = pickupOwner;
		pickupOwner.onHandleFiring = (MVPickupOwner.OnHandleFiringDelegate)Delegate.Combine(pickupOwner.onHandleFiring, new MVPickupOwner.OnHandleFiringDelegate(OnHandleFiring));
		pickupGUI = new PickupGUI(pickupOwner);
		modes = new AvatarModes();
		modes.JetPackMode = new JetPackMode(this);
		modes.WalkMode = new WalkMode(this, avatarMotor);
		if (MVGameController.Instance.Game.GameMode == MVGameMode.Edit)
		{
			Mode = modes.JetPackMode;
			state = AvatarState.Editing;
		}
		else
		{
			Mode = modes.WalkMode;
			state = AvatarState.Playing;
		}
		MVRuntimeDataVariableClampedFloat health = Health;
		health.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(health.OnChange, (MVRuntimeDataVariable.OnChangeDelegate)((object obj) =>
		{
			if ((float)obj == 0f)
			{
				Die();
			}
		}));
		UXCamera uXCamera = UXUtils.FindGUIObjectOfType<UXCamera>();
		if (Object.op_Implicit((Object)(object)uXCamera))
		{
			Camera camera = ((Component)uXCamera).camera;
			HealthBar componentInChildren = gameObject.GetComponentInChildren<HealthBar>();
			((Component)componentInChildren).transform.parent = ((Component)camera).transform;
			((Component)componentInChildren).transform.localRotation = Quaternion.identity;
			((Component)componentInChildren).transform.localPosition = new Vector3(0f, 92f, 9f);
			((Component)componentInChildren).transform.localScale = new Vector3(70f, 45f, 1f);
			Renderer[] componentsInChildren = ((Component)componentInChildren).GetComponentsInChildren<Renderer>();
			Renderer[] array = componentsInChildren;
			foreach (Renderer val in array)
			{
				((Component)val).gameObject.layer = LayerMask.NameToLayer("UXElement");
			}
		}
		else
		{
			Debug.LogWarning((object)"Cannot find UX Camera to place health bar under!");
		}
		respawnTime = Time.time;
	}

	private void HandleStuck()
	{
		if (state == AvatarState.Playing)
		{
			Die();
			MVGameController.Instance.Game.PostGameMsg(MVGameMsgType.AvatarKilled, GameMessages.MakePlayerKilledMessage(MVGameController.Instance.Game.LocalPlayerActorNumber, MVGameController.Instance.Game.LocalPlayerActorNumber, PlayerKilledByType.Crushed));
		}
	}

	public void Suicide()
	{
		if (state != AvatarState.Dead)
		{
			Die();
			Debug.Log((object)"Avatar dead");
			MVGameController.Instance.Game.PostGameMsg(MVGameMsgType.AvatarKilled, GameMessages.MakePlayerKilledMessage(MVGameController.Instance.Game.LocalPlayerActorNumber, MVGameController.Instance.Game.LocalPlayerActorNumber, PlayerKilledByType.Suicide));
		}
	}

	private void Die()
	{
		if (state == AvatarState.Playing)
		{
			state = AvatarState.Dead;
			deadTime = Time.time;
			SetAnimation("Dead");
			MVEquipable component = gameObject.GetComponent<MVEquipable>();
			if ((Object)(object)component != (Object)null)
			{
				component.Unequip();
			}
			if (IsSeated)
			{
				LeaveVehicle();
			}
			MVTriggerHandler component2 = gameObject.GetComponent<MVTriggerHandler>();
			if ((Object)(object)component2 == (Object)null)
			{
				Debug.LogError((object)"TriggerHandler on avatar is null");
			}
			else
			{
				((Behaviour)component2).enabled = false;
			}
		}
	}

	private void ResetAvatar()
	{
		showingRespawnDelay = false;
		State = MVWorldObjectState.Dirty;
		SetAnimation("Idle");
		RigidBody.Reset();
		Health.Value = 100f;
		if (IsSeated)
		{
			Debug.Log((object)"Left vehicle");
			LeaveVehicle();
		}
		MVEquipable component = GameObject.GetComponent<MVEquipable>();
		if ((Object)(object)component != (Object)null)
		{
			component.Unequip();
		}
		if ((Object)(object)interactableLocal != (Object)null)
		{
			interactableLocal.ClearModifiers();
		}
		AvatarState avatarState = AvatarState.Playing;
		if (MVGameController.Instance.EditController != null && !MVGameController.Instance.EditController.PlayInEditor)
		{
			avatarState = AvatarState.Editing;
		}
		state = avatarState;
	}

	private void OnHandleFiring(bool isFiring)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (InGunMode && isFiring && ((Behaviour)RigidBody).enabled)
		{
			Vector3 velocity = RigidBody.Velocity;
			if (velocity.sqrMagnitude < 0.01f)
			{
				RotateAvatarToFiringDirection();
			}
		}
	}

	private void UpdateInvulnerable()
	{
		bool flag = !CanReceivePackages;
		if ((bool)Invulnerable.Value != flag)
		{
			Invulnerable.Value = flag;
		}
	}
}
