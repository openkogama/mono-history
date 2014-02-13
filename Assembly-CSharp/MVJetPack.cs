using System;
using System.Collections;
using System.Collections.Generic;
using MV.WorldObject;
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

	private enum JetPackType : byte
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

		private MvCharacterController avatarController;

		private JetPackVisualization jetPackVisualization;

		private MVAvatarLocal vehicleUser;

		private bool wasFiring;

		private int framesGrounded;

		private int framesGroundedThreshold = 2;

		public override int Id => owner.Id;

		protected override MVVehicleBase Owner => owner;

		public LocalObjectsJetPack(MVJetPack vehicleBase, MVAvatarLocal vehicleUser, JetPackParameters jetPackTypeParameters, VehicleSeatBase seat)
		{
			avatarController = vehicleUser.GameObject.GetComponent<MvCharacterController>();
			MVInteractableBase component = vehicleUser.GameObject.GetComponent<MVInteractableBase>();
			MVEquipable component2 = vehicleUser.GameObject.GetComponent<MVEquipable>();
			avatarPickupOwner = vehicleUser.GameObject.GetComponent<MVPickupOwner>();
			if ((Object)(object)avatarController == (Object)null || (Object)(object)component == (Object)null || (Object)(object)component2 == (Object)null || (Object)(object)avatarPickupOwner == (Object)null)
			{
				Debug.LogError((object)"Failed to get component. Cant create LocalObjects for JetPack");
				return;
			}
			MvCharacterController mvCharacterController = avatarController.CloneToGameObject(vehicleBase.GameObject, ((Component)seat).gameObject);
			vehicleUser.SetCharacterController(mvCharacterController);
			thrustTimeOverheatThreshold = jetPackTypeParameters.thrustTimeOverheatThreshold;
			coolDownFactor = jetPackTypeParameters.coolDownFactor;
			MVRuntimeDataVariableClampedFloat health = vehicleBase.Health;
			health.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(health.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(OnHealthChange));
			GameObject gameObject = vehicleBase.GameObject;
			jetPackVisualization = gameObject.GetComponent<JetPackVisualization>();
			VehicleInteractable vehicleInteractable = gameObject.AddComponent<VehicleInteractable>();
			vehicleInteractable.Init(vehicleBase.Modifiers, vehicleBase.Health);
			JetPackMotor jetPackMotor = gameObject.AddComponent<JetPackMotor>();
			jetPackMotor.Init(component, vehicleInteractable, mvCharacterController, jetPackTypeParameters.thrustStrength, jetPackTypeParameters.density);
			MVEquipableProxy mVEquipableProxy = gameObject.AddComponent<MVEquipableProxy>();
			mVEquipableProxy.Init(component2);
			triggerHandler = gameObject.AddComponent<MVTriggerHandler>();
			localComponents.Add((Component)(object)triggerHandler);
			localComponents.Add((Component)(object)vehicleInteractable);
			localComponents.Add((Component)(object)jetPackMotor);
			localComponents.Add((Component)(object)mvCharacterController);
			localComponents.Add((Component)(object)mVEquipableProxy);
			vehicleMotor = jetPackMotor;
			owner = vehicleBase;
			this.vehicleUser = vehicleUser;
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
			((Behaviour)triggerHandler).enabled = false;
			MVPickupOwner mVPickupOwner = avatarPickupOwner;
			mVPickupOwner.onHandleFiring = (MVPickupOwner.OnHandleFiringDelegate)Delegate.Remove(mVPickupOwner.onHandleFiring, new MVPickupOwner.OnHandleFiringDelegate(OnFiring));
		}

		public override void Enter()
		{
			base.Enter();
			leaveMode = false;
			vehicleMotor.LeaveMode = false;
			((Behaviour)triggerHandler).enabled = true;
			vehicleUser.ForceRotateAvatarToFiringDirection = true;
			MVPickupOwner mVPickupOwner = avatarPickupOwner;
			mVPickupOwner.onHandleFiring = (MVPickupOwner.OnHandleFiringDelegate)Delegate.Combine(mVPickupOwner.onHandleFiring, new MVPickupOwner.OnHandleFiringDelegate(OnFiring));
		}

		public override MovementMap FixedUpdate(MovementMap movementMap)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			owner.State = MVWorldObjectState.Dirty;
			bool thrust = false;
			if (movementMap != null)
			{
				thrust = movementMap.Jump;
			}
			thrust = EvaluateThrust(thrust);
			OverheatUpdate(thrust);
			if (walkMode)
			{
				return movementMap;
			}
			Quaternion setQuaternion = Quaternion.identity;
			bool shouldSetRotation = false;
			if (movementMap != null)
			{
				Transform transform = ((Component)Camera.main).transform;
				Vector3 val = movementMap.Direction;
				if (val.magnitude > 0f)
				{
					Vector3 val2 = transform.rotation * val;
					val2.y = 0f;
					val = val2.normalized;
				}
				Vector3 velocity = vehicleMotor.Velocity;
				if (velocity.sqrMagnitude > 0.001f || wasFiring)
				{
					setQuaternion = FiringDirectionRotation();
					shouldSetRotation = true;
				}
				vehicleMotor.InputMoveDirection = val;
			}
			vehicleMotor.Thrust = thrust;
			vehicleMotor.UpdateFunction(setQuaternion, shouldSetRotation);
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
			JetModeType jetModeType = (JetModeType)(byte)owner.JetMode.Value;
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
				jetPackVisualization.DoOverheatBlinking();
				jetModeType2 = JetModeType.Overheating;
			}
			if (jetModeType != jetModeType2)
			{
				owner.JetMode.Value = (byte)jetModeType2;
			}
		}

		private Quaternion FiringDirectionRotation()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			Vector3 lookDirection = avatarPickupOwner.LookDirection;
			lookDirection.y = 0f;
			return Quaternion.LookRotation(lookDirection);
		}

		public override MovementMap Update(MovementMap movementMap)
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
			if (((Behaviour)vehicleUser.RigidBody).enabled != walkMode)
			{
				((Behaviour)vehicleUser.RigidBody).enabled = walkMode;
			}
			if (walkMode)
			{
				return movementMap;
			}
			if (vehicleMotor.IsStuck())
			{
				Debug.Log((object)"Vehicle is stuck");
			}
			return movementMap;
		}
	}

	private EditableCubeModelWrapper editableCubeModelWrapper;

	public MVRuntimeDataVariableClampedFloat Health;

	public MVRuntimeDataVariable Modifiers;

	public MVRuntimeDataVariable JetMode;

	private JetPackParameters jetPackParameters;

	private JetPackType jetPackType;

	private static readonly Dictionary<JetPackType, string> jetPackTypes = new Dictionary<JetPackType, string>
	{
		{
			JetPackType.JetPack,
			"Prefabs/Blueprints/Vehicles/JetPack"
		},
		{
			JetPackType.JetPackDeluxe,
			"Prefabs/Blueprints/Vehicles/JetPackDeluxe"
		}
	};

	public MVJetPack(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
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
		Modifiers = RuntimeDataVariables.New("modifiers", 1f, writeThrough: false);
		JetMode = RuntimeDataVariables.New("jetMode", 1f, writeThrough: false);
		MVRuntimeDataVariable isVehicleDead = IsVehicleDead;
		isVehicleDead.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(isVehicleDead.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(OnIsDeadChange));
		MVCubeModelBase mVCubeModelBase = (MVCubeModelBase)GetChild("JetPackCubeModel");
		editableCubeModelWrapper = new EditableCubeModelWrapper(mVCubeModelBase, new IntVector(jetPackParameters.lowerCubeConstraint[0], jetPackParameters.lowerCubeConstraint[1], jetPackParameters.lowerCubeConstraint[2]), new IntVector(jetPackParameters.upperCubeConstraint[0], jetPackParameters.upperCubeConstraint[1], jetPackParameters.upperCubeConstraint[2]), jetPackParameters.minNumberOfCubes);
		JetPackVisualization component = gameObject.GetComponent<JetPackVisualization>();
		component.Init(IsInSpawner, mVCubeModelBase.GameObject.transform, JetMode);
		visualization = component;
		if (!IsInSpawner)
		{
			InteractionDataHandler interactionDataHandler = mVCubeModelBase.GameObject.AddComponent<InteractionDataHandler>();
			interactionDataHandler.WorldObjectParent = this;
		}
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		gameObject.GetComponent<JetPackVisualization>().EnableThruster(enable: true);
	}

	public override bool OnEnterObject(EditorStateMachine e)
	{
		return editableCubeModelWrapper.OnEnterObject(e);
	}

	public override bool OnExitObject(EditorStateMachine e)
	{
		return editableCubeModelWrapper.OnExitObject(e);
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
		AvatarPickupOwner component = vehicleUser.GameObject.GetComponent<AvatarPickupOwner>();
		if ((Object)(object)component == (Object)null)
		{
			Debug.LogError((object)"Failed to get avatarPickupOwner");
			return;
		}
		HashSet<int> worldIDsRecursive = WorldIDsRecursive;
		worldIDsRecursive.ExceptWith(component.IgnoreWOIDs);
		component.AdditionalIgnoreWOIDS = worldIDsRecursive;
	}

	protected override LocalObjectsBase CreateLocalObjects(int seatID, MVAvatarLocal vehicleUser)
	{
		return new LocalObjectsJetPack(this, vehicleUser, jetPackParameters, seatManager.seats[seatID]);
	}

	public override Bounds GetLocalBounds(BoundsContext boundsContext)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (boundsContext == BoundsContext.Insert || boundsContext == BoundsContext.BoxVisualization || boundsContext == BoundsContext.Preview)
		{
			return new Bounds(Vector3.zero, Vector3.one * 2f);
		}
		return base.GetLocalBounds(boundsContext);
	}

	private void OnIsDeadChange(object isDead)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		if ((bool)isDead)
		{
			HashSet<int> worldIDsRecursive = WorldIDsRecursive;
			Vector3 val = gameObject.transform.rotation * Vector3.back;
			MVGameController.Instance.WOCM.SharedWorldObjectGameplayFunctions.ExplosionCreator.Explode(gameObject.transform.position + val, 10f, 5f, 1000f, worldIDsRecursive);
		}
	}

	private static JetPackType GetJetPackType(Hashtable data)
	{
		Hashtable hashtable = (Hashtable)data[WorldObjectDataParameters.Data];
		if (!hashtable.Contains("jetPackType"))
		{
			Debug.LogError((object)"WoData does not contain jetPackType ");
		}
		return (JetPackType)(byte)hashtable["jetPackType"];
	}

	private static string GetPickupPrefabName(Hashtable data)
	{
		return jetPackTypes[GetJetPackType(data)];
	}
}
