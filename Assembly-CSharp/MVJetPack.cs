using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using MV.WorldObject.RuntimeEvents;
using UnityEngine;

public class MVJetPack : MVVehicleBase
{
	public enum JetModeType : byte
	{
		Off,
		On,
		Overheating,
		NotSet
	}

	public enum JetPackType : byte
	{
		JetPack,
		JetPackDeluxe
	}

	protected class LocalObjectsJetPack : LocalObjectsBase
	{
		private float thrustTimeOverheatThreshold = 3f;

		private float thrustTimeWarning = 1f;

		private float thrustTime;

		private float coolDownFactor = 0.5f;

		private JetPackMotor vehicleMotor;

		private MVJetPack owner;

		private MVTriggerHandler triggerHandler;

		private MVPickupOwner avatarPickupOwner;

		private bool walkMode = true;

		private bool leaveMode;

		private SmoothCharacterController avatarController;

		private Camera mainCamera;

		private JetPackVisualization jetPackVisualization;

		private MVAvatarLocal vehicleUser;

		private bool wasFiring;

		private int framesGrounded;

		private int framesGroundedThreshold = 2;

		public override int Id => owner.Id;

		protected override MVVehicleBase Owner => owner;

		public LocalObjectsJetPack(MVJetPack vehicleBase, MVAvatarLocal vehicleUser, JetPackParameters jetPackTypeParameters, VehicleSeatBase seat)
		{
			avatarController = vehicleUser.GameObject.GetComponent<SmoothCharacterController>();
			AvatarInteractable component = vehicleUser.GameObject.GetComponent<AvatarInteractable>();
			MVEquipable component2 = vehicleUser.GameObject.GetComponent<MVEquipable>();
			avatarPickupOwner = vehicleUser.GameObject.GetComponent<MVPickupOwner>();
			if (avatarController == null || component == null || component2 == null || avatarPickupOwner == null)
			{
				Debug.LogError("Failed to get component. Cant create LocalObjects for JetPack");
				return;
			}
			SmoothCharacterController smoothCharacterController = avatarController.Clone(vehicleBase.GameObject, seat.gameObject, null, vehicleBase);
			vehicleUser.SetCharacterController(smoothCharacterController);
			thrustTimeOverheatThreshold = jetPackTypeParameters.thrustTimeOverheatThreshold;
			coolDownFactor = jetPackTypeParameters.coolDownFactor;
			MVRuntimeDataVariableClampedFloat health = vehicleBase.Health;
			health.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(health.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(OnHealthChange));
			GameObject gameObject = vehicleBase.GameObject;
			jetPackVisualization = gameObject.GetComponentInChildren<JetPackVisualization>();
			VehicleInteractable vehicleInteractable = gameObject.AddComponent<VehicleInteractable>();
			vehicleInteractable.Init(vehicleBase.Modifiers, vehicleBase.Health, null, vehicleBase.Shield, null);
			JetPackMotor jetPackMotor = gameObject.AddComponent<JetPackMotor>();
			jetPackMotor.Init(component, vehicleInteractable, smoothCharacterController, jetPackTypeParameters.thrustStrength, jetPackTypeParameters.density);
			MVEquipableProxy mVEquipableProxy = gameObject.AddComponent<MVEquipableProxy>();
			mVEquipableProxy.Init(component2);
			triggerHandler = gameObject.AddComponent<MVTriggerHandler>();
			localComponents.Add(triggerHandler);
			localComponents.Add(vehicleInteractable);
			localComponents.Add(jetPackMotor);
			localComponents.Add(smoothCharacterController);
			localComponents.Add(mVEquipableProxy);
			vehicleMotor = jetPackMotor;
			owner = vehicleBase;
			this.vehicleUser = vehicleUser;
			mainCamera = Camera.main;
			owner.interactionDataHandlerBase = vehicleUser.InteractionDataHandlerBase;
		}

		private void OnFiring(bool isFiring)
		{
			wasFiring = isFiring;
		}

		public override void Destroy()
		{
			base.Destroy();
			MVRuntimeDataVariableClampedFloat health = owner.Health;
			health.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Remove(health.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(OnHealthChange));
			MVPickupOwner mVPickupOwner = avatarPickupOwner;
			mVPickupOwner.onHandleFiring = (MVPickupOwner.OnHandleFiringDelegate)Delegate.Remove(mVPickupOwner.onHandleFiring, new MVPickupOwner.OnHandleFiringDelegate(OnFiring));
		}

		public override void Leave()
		{
			base.Leave();
			walkMode = false;
			leaveMode = true;
			vehicleMotor.LeaveMode = true;
			vehicleUser.SetCharacterController(avatarController);
			vehicleUser.ForceRotateAvatarToFiringDirection = false;
			owner.interactionDataHandlerBase = null;
			MVPickupOwner mVPickupOwner = avatarPickupOwner;
			mVPickupOwner.onHandleFiring = (MVPickupOwner.OnHandleFiringDelegate)Delegate.Remove(mVPickupOwner.onHandleFiring, new MVPickupOwner.OnHandleFiringDelegate(OnFiring));
		}

		public override InputToInGameAction Update(InputToInGameAction interactionInput)
		{
			if (!walkMode)
			{
				vehicleMotor.FrameUpdate();
			}
			return interactionInput;
		}

		public override void Enter()
		{
			base.Enter();
			leaveMode = false;
			vehicleMotor.LeaveMode = false;
			triggerHandler.enabled = true;
			vehicleUser.ForceRotateAvatarToFiringDirection = true;
			MVPickupOwner mVPickupOwner = avatarPickupOwner;
			mVPickupOwner.onHandleFiring = (MVPickupOwner.OnHandleFiringDelegate)Delegate.Combine(mVPickupOwner.onHandleFiring, new MVPickupOwner.OnHandleFiringDelegate(OnFiring));
		}

		public override IInputToPlayerMovement FixedUpdate(IInputToPlayerMovement movementMap)
		{
			movementMap = HandleWalkMode(movementMap);
			bool thrust = false;
			if (movementMap != null)
			{
				thrust = movementMap.Jump;
			}
			thrust = EvaluateThrust(thrust);
			OverheatUpdate(thrust);
			if (walkMode)
			{
				vehicleUser.RigidBody.AddImpulse(vehicleMotor.Impulses);
				return movementMap;
			}
			Quaternion setQuaternion = Quaternion.identity;
			bool shouldSetRotation = false;
			if (movementMap != null)
			{
				Transform transform = mainCamera.transform;
				Vector3 vector = movementMap.Direction;
				if (vector.magnitude > 0f)
				{
					Vector3 vector2 = transform.rotation * vector;
					vector2.y = 0f;
					vector = vector2.normalized;
				}
				if (vehicleMotor.Velocity.sqrMagnitude > 0.001f || wasFiring)
				{
					setQuaternion = FiringDirectionRotation();
					shouldSetRotation = true;
				}
				vehicleMotor.InputMoveDirection = vector;
			}
			vehicleMotor.Thrust = thrust;
			vehicleMotor.FixedUpdateFunction(setQuaternion, shouldSetRotation);
			return movementMap;
		}

		private bool WalkMode(bool thrust)
		{
			framesGrounded++;
			if (thrust)
			{
				framesGrounded = 0;
				return false;
			}
			if (!vehicleUser.RigidBody.Grounded && walkMode)
			{
				return false;
			}
			if (!vehicleMotor.Grounded)
			{
				framesGrounded = 0;
				return false;
			}
			if (framesGrounded > framesGroundedThreshold)
			{
				return true;
			}
			return false;
		}

		private bool EvaluateThrust(bool thrust)
		{
			if (vehicleMotor.IsUnderWater)
			{
				thrust = false;
			}
			if (leaveMode && !vehicleMotor.IsUnderWater)
			{
				thrust = true;
			}
			if (owner.IsDead)
			{
				thrust = false;
			}
			return thrust;
		}

		private void OverheatUpdate(bool thrust)
		{
			JetModeType jetModeType = (JetModeType)owner.JetMode.Value;
			JetModeType jetModeType2 = JetModeType.Off;
			if (thrust)
			{
				thrustTime += Time.deltaTime;
				jetModeType2 = JetModeType.On;
			}
			else
			{
				thrustTime -= Time.deltaTime * coolDownFactor;
			}
			thrustTime = Mathf.Clamp(thrustTime, 0f, float.MaxValue);
			if (thrustTime > thrustTimeOverheatThreshold)
			{
				owner.Health.Value -= owner.Health.Value;
			}
			else if (thrustTime > thrustTimeOverheatThreshold - thrustTimeWarning)
			{
				jetModeType2 = JetModeType.Overheating;
				if (avatarPickupOwner.CurrentItem != null && (!avatarPickupOwner.CurrentItem.ActivateGunModeOnEquip || avatarPickupOwner.CurrentItem.IsHolstered))
				{
					jetPackVisualization.DoOverheatBlinking();
				}
				if (!vehicleMotor.LeaveMode && MVGameControllerBase.MainCameraManager.CurrentCamera.CameraType == CameraType.FirstPersonCamera)
				{
					jetPackVisualization.ShowOverHeatWarning();
				}
			}
			if (jetModeType != jetModeType2)
			{
				owner.JetMode.Value = (byte)jetModeType2;
			}
		}

		private Quaternion FiringDirectionRotation()
		{
			Vector3 lookDirection = avatarPickupOwner.LookDirection;
			lookDirection.y = 0f;
			return Quaternion.LookRotation(lookDirection);
		}

		public IInputToPlayerMovement HandleWalkMode(IInputToPlayerMovement movementMap)
		{
			if (movementMap == null)
			{
				return null;
			}
			walkMode = WalkMode(movementMap.Jump);
			if (!walkMode)
			{
				vehicleUser.SetAnimation("Idle");
			}
			if (vehicleUser.RigidBody.enabled != walkMode)
			{
				vehicleUser.RigidBody.enabled = walkMode;
			}
			if (walkMode)
			{
				return movementMap;
			}
			if (vehicleMotor.IsStuck())
			{
				Debug.Log("Vehicle is stuck");
			}
			return movementMap;
		}
	}

	private CullingSubscriberDynamic cullingSubscriberDynamic;

	private EditableCubeModelWrapper editableCubeModelWrapper;

	private MVRuntimeDataVariableClampedFloat shield;

	public MVRuntimeDataVariableClampedFloat Health;

	public MVRuntimeDataVariable Modifiers;

	public MVRuntimeDataVariable JetMode;

	private JetPackParameters jetPackParameters;

	private JetPackType jetPackType;

	public override MVWorldObjectDocumentationType DocumentationType => jetPackType switch
	{
		JetPackType.JetPack => MVWorldObjectDocumentationType.SmallJetpack, 
		JetPackType.JetPackDeluxe => MVWorldObjectDocumentationType.BigJetpack, 
		_ => MVWorldObjectDocumentationType.Missing, 
	};

	public MVRuntimeDataVariableClampedFloat Shield
	{
		get
		{
			return shield;
		}
		set
		{
			shield = value;
		}
	}

	public MVJetPack(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, GetPickupPrefabName(data), worldObjects)
	{
		interactionFlags |= InteractionFlags.CanEdit;
		jetPackParameters = gameObject.GetComponent<JetPackParameters>();
		jetPackType = GetJetPackType(data);
	}

	public override void Initialize()
	{
		base.Initialize();
		float maxValue = (float)RuntimeVariablesRepository.GetRuntimeVariables(WorldObjectType)["health"];
		Health = RuntimeDataVariables.NewClampedFloat("health", 0.2f, writeThrough: false, 0f, maxValue);
		Shield = RuntimeDataVariables.NewClampedFloat("shield", 0.2f, writeThrough: false, 0f, maxValue);
		Modifiers = RuntimeDataVariables.New("modifiers", 1f, writeThrough: false);
		JetMode = RuntimeDataVariables.New("jetMode", 1f, writeThrough: false);
		MVRuntimeDataVariable isVehicleDead = IsVehicleDead;
		isVehicleDead.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(isVehicleDead.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(OnIsDeadChange));
		MVCubeModelInstance mVCubeModelInstance = (MVCubeModelInstance)GetChild("JetPackCubeModel");
		editableCubeModelWrapper = new EditableCubeModelWrapper(mVCubeModelInstance, new IntVector(jetPackParameters.lowerCubeConstraint[0], jetPackParameters.lowerCubeConstraint[1], jetPackParameters.lowerCubeConstraint[2]), new IntVector(jetPackParameters.upperCubeConstraint[0], jetPackParameters.upperCubeConstraint[1], jetPackParameters.upperCubeConstraint[2]), jetPackParameters.minNumberOfCubes);
		JetPackVisualization componentInChildren = gameObject.GetComponentInChildren<JetPackVisualization>();
		componentInChildren.Init(IsInSpawner, mVCubeModelInstance.GameObject.transform, JetMode);
		visualization = componentInChildren;
		if (!IsInSpawner)
		{
			InteractionDataHandler interactionDataHandler = mVCubeModelInstance.GameObject.AddComponent<InteractionDataHandler>();
			interactionDataHandler.WorldObjectParent = this;
			interactionDataHandlerBase = interactionDataHandler;
			mVCubeModelInstance.Visible = true;
			cullingSubscriberDynamic = new CullingSubscriberDynamic(4f, 3, componentInChildren.gameObject);
		}
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		gameObject.GetComponentInChildren<JetPackVisualization>().EnableThruster(enable: true);
	}

	public override bool OnEnterObject(EditorStateMachine e)
	{
		return editableCubeModelWrapper.OnEnterObject(e);
	}

	public override bool OnExitObject(EditorStateMachine e)
	{
		return editableCubeModelWrapper.OnExitObject(e);
	}

	public override void Destroy()
	{
		base.Destroy();
		if (cullingSubscriberDynamic != null)
		{
			cullingSubscriberDynamic.Destroy();
			cullingSubscriberDynamic = null;
		}
	}

	public override bool CompareWithKoGaMaPackage(MVWorldObjectClient wo, KoGaMaPackageClient koGaMaPackageClient, ref int insertedByProfileId)
	{
		MVJetPack mVJetPack = (MVJetPack)wo;
		if (jetPackType != mVJetPack.jetPackType)
		{
			return false;
		}
		return base.CompareWithKoGaMaPackage(wo, koGaMaPackageClient, ref insertedByProfileId);
	}

	protected override void VehicleEntered(MVAvatar vehicleUser, int seatID)
	{
		base.VehicleEntered(vehicleUser, seatID);
		seatManager.EnterVehicleDisabled = true;
		AvatarPickupOwner avatarPickupOwner = vehicleUser.GameObject.GetComponent<AvatarPickupOwner>();
		if (avatarPickupOwner == null)
		{
			Debug.LogError("Failed to get avatarPickupOwner");
			return;
		}
		HashSet<int> worldIDsRecursive = WorldIDsRecursive;
		worldIDsRecursive.ExceptWith(avatarPickupOwner.IgnoreWOIDs);
		avatarPickupOwner.AdditionalIgnoreWOIDS = worldIDsRecursive;
	}

	protected override LocalObjectsBase CreateLocalObjects(int seatID, MVAvatarLocal vehicleUser)
	{
		return new LocalObjectsJetPack(this, vehicleUser, jetPackParameters, seatManager.seats[seatID]);
	}

	public override Bounds GetLocalBounds(BoundsContext boundsContext)
	{
		if (boundsContext == BoundsContext.Insert || boundsContext == BoundsContext.BoxVisualization || boundsContext == BoundsContext.Preview)
		{
			return new Bounds(Vector3.zero, Vector3.one * 2f);
		}
		return base.GetLocalBounds(boundsContext);
	}

	private void OnIsDeadChange(object isDead)
	{
		if ((bool)isDead)
		{
			HashSet<int> worldIDsRecursive = WorldIDsRecursive;
			Vector3 vector = gameObject.transform.rotation * Vector3.back;
			if (localObjects == null)
			{
				SharedWorldObjectGameplayFunctions.Explosion.Explode(PrefabPool.Instance.ParticleExplosion, gameObject.transform.position + vector, 10f, 5f, 1000f, local: true, null, worldIDsRecursive);
				return;
			}
			ExplosionEvent explosionEvent = new ExplosionEvent(RuntimeEventType.Bazooka, gameObject.transform.position + vector);
			SharedWorldObjectGameplayFunctions.Explosion.Explode(PrefabPool.Instance.ParticleExplosion, gameObject.transform.position + vector, 10f, 5f, 1000f, local: false, explosionEvent, worldIDsRecursive);
		}
	}

	private static JetPackType GetJetPackType(Dictionary<object, object> data)
	{
		Dictionary<object, object> dictionary = (Dictionary<object, object>)data[WorldObjectDataParameters.Data];
		if (!dictionary.ContainsKey("jetPackType"))
		{
			Debug.LogError("WoData does not contain jetPackType ");
		}
		return (JetPackType)dictionary["jetPackType"];
	}

	private static VehicleBaseObject GetPickupPrefabName(Dictionary<object, object> data)
	{
		return PrefabPool.JetPackPrefabLUT[GetJetPackType(data)];
	}
}
