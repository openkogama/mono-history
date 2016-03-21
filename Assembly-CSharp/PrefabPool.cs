using UnityEngine;

public class PrefabPool : MonoBehaviour
{
	private static PrefabPool instance;

	[Header("World Objects")]
	[SerializeField]
	public ObjectPrefab MVFirePrefab;

	[SerializeField]
	public ObjectPrefab MVAndPrefab;

	[SerializeField]
	public GameObject MVAvatarPrefab;

	[SerializeField]
	public GameObject MVAdvancedGhostPrefab;

	[SerializeField]
	public GameObject MVBodyPrefab;

	[SerializeField]
	public GameObject MVHamsterWheelPrefab;

	[SerializeField]
	public GameObject MVHoverCraftPrefab;

	[SerializeField]
	public GameObject MVJetPackPrefab;

	[SerializeField]
	public GameObject MVJetPackDeluxePrefab;

	[SerializeField]
	public ObjectPrefab MVBatteryPrefab;

	[SerializeField]
	public ObjectPrefab MVCameraSettingsPrefab;

	[SerializeField]
	public MVCollectibleObject MVCollectiblePrefab;

	[SerializeField]
	public MVCollectibleObject MVCollectibleFantaPrefab;

	[SerializeField]
	public ObjectPrefab MVCheckpointPrefab;

	[SerializeField]
	public ObjectPrefab MVExplosivesPrefab;

	[SerializeField]
	public ObjectPrefab MVFlagPrefab;

	[SerializeField]
	public ObjectPrefab MVGameCoinChestPrefab;

	[SerializeField]
	public MVGameCoinObject MVGameCoinPrefab;

	[SerializeField]
	public GameObject MVGhostPrefab;

	[SerializeField]
	public ObjectPrefab MVGoalPrefab;

	[SerializeField]
	public GameObject MVGhostInstancePrefab;

	[SerializeField]
	public ObjectPrefab MVGravityCubePrefab;

	[SerializeField]
	public ObjectPrefab MVNegatePrefab;

	[SerializeField]
	public MVPointLightObject MVPointLightPrefab;

	[SerializeField]
	public MVPressurePlateObject MVPressurePlatePrefab;

	[SerializeField]
	public ObjectPrefab MVPulseBoxPrefab;

	[SerializeField]
	public ObjectPrefab MVRandomBoxPrefab;

	[SerializeField]
	public ObjectPrefab MVRoundCubePrefab;

	[SerializeField]
	public ObjectPrefab MVSkyboxPrefab;

	[SerializeField]
	public ObjectPrefab MVSmokePrefab;

	[SerializeField]
	public ObjectPrefab MVSoundEmitterPrefab;

	[SerializeField]
	public ObjectPrefab MVSpawnPointBluePrefab;

	[SerializeField]
	public ObjectPrefab MVSpawnPointGreenPrefab;

	[SerializeField]
	public ObjectPrefab MVSpawnPointRedPrefab;

	[SerializeField]
	public ObjectPrefab MVSpawnPointYellowPrefab;

	[SerializeField]
	public MVTextMsgObject MVTextMsgPrefab;

	[SerializeField]
	public ObjectPrefab MVTimeTriggerPrefab;

	[SerializeField]
	public ObjectPrefab MVToggleBoxPrefab;

	[SerializeField]
	public MVTriggerBoxObject MVTriggerBoxPrefab;

	[SerializeField]
	public GameObject MVMovingPlatformGroupPrefab;

	[SerializeField]
	public GameObject MVMovingPlatformNodePrefab;

	[SerializeField]
	public MVObjectEnablerObject MVObjectEnablerPrefab;

	[SerializeField]
	public ObjectPrefab MVKillLimitPrefab;

	[SerializeField]
	public ObjectPrefab MVOculusKillLimitPrefab;

	[SerializeField]
	public MVSentryGunObject MVSentryGunPrefab;

	[SerializeField]
	public ShootableButtonObject ShootableButtonPrefab;

	[SerializeField]
	public GameObject MVTeleportGroupPrefab;

	[SerializeField]
	public MVTeleporterObject MVTeleporterPrefab;

	[SerializeField]
	public UseLeverObject UseLeverPrefab;

	[SerializeField]
	public ObjectPrefab MVWaterPlanePrefab;

	[SerializeField]
	public WindTurbineObject WindTurbinePrefab;

	[SerializeField]
	public MVCountingCubeObject MVCountingCube;

	[SerializeField]
	public TeleportAvatar TeleportAvatarPrefab;

	[Header("Game")]
	[Space(20f)]
	[SerializeField]
	public Material GhostMarkerMaterial;

	[SerializeField]
	public Material ObjectHiddenMaterial;

	[SerializeField]
	public SentryGunBeam IceBeamObject;

	[SerializeField]
	public SentryGunBeam FireBeamObject;

	[SerializeField]
	public StarDisplayObject StarDisplayPrefab;

	[SerializeField]
	public LevelDisplayCube LevelDisplayPrefab;

	[SerializeField]
	public GameCoinDisplayObject GameCoinDisplayPrefab;

	[SerializeField]
	public CubeModelChunkPrefab CubeModelChunkPrefab;

	[Header("Pick up")]
	[Space(20f)]
	[SerializeField]
	public MVPickupItemBaseObject AvatarCenterGunPrefab;

	[SerializeField]
	public MVPickupItemBaseObject AvatarImpulseGunPrefab;

	[SerializeField]
	public MVPickupItemBaseObject AvatarHealthPrefab;

	[SerializeField]
	public MVPickupItemBaseObject AvatarBazookaPrefab;

	[SerializeField]
	public MVPickupItemBaseObject AvatarRailGunPrefab;

	[SerializeField]
	public MVPickupItemBaseObject AvatarMutantPrefab;

	[SerializeField]
	public MVPickupItemBaseObject AvatarSwordPrefab;

	[SerializeField]
	public MVPickupItemBaseObject AvatarShotgunPrefab;

	[SerializeField]
	public MVPickupItemBaseObject AvatarFlamethrowerPrefab;

	[SerializeField]
	public MVPickupItemBaseObject AvatarCubeGunPrefab;

	[SerializeField]
	public MVPickupItemBaseObject AvatarNinjaRunPrefab;

	[SerializeField]
	public MVPickupItemBaseObject AvatarSixShooterPrefab;

	[SerializeField]
	public MVPickupItemBaseObject AvatarDoubleSixShooterPrefab;

	[SerializeField]
	public MVPickupItemBaseObject AvatarThrowingStarPrefab;

	[SerializeField]
	public MVPickupItemBaseObject AvatarMultiThrowingStarPrefab;

	[SerializeField]
	public MVPickupItemBaseObject AvatarMouseGunPrefab;

	[SerializeField]
	public MVPickupItemBaseObject AvatarGrowthGunPrefab;

	[SerializeField]
	public MVPickupItemBaseObject AvatarMousePackPrefab;

	[SerializeField]
	public MVPickupItemBaseObject AvatarGrowthPackPrefab;

	[Space(20f)]
	[SerializeField]
	[Header("Avatar item pick up")]
	public GameObject AvatarItemCenterGun;

	public GameObject AvatarItemImpulseGun;

	public GameObject AvatarItemLaserPointer;

	public GameObject AvatarItemBazooka;

	public GameObject AvatarItemHand;

	public GameObject AvatarItemRailGun;

	public GameObject AvatarItemSword;

	public GameObject AvatarItemShotgun;

	public GameObject AvatarItemFlamethrower;

	public GameObject AvatarItemCubeGun;

	public GameObject AvatarItemSixShooter;

	public GameObject AvatarItemDoubleSixShooter;

	public GameObject AvatarItemThrowingStar;

	public GameObject AvatarItemMultiThrowingStar;

	public GameObject AvatarItemGrowthGun;

	public GameObject AvatarItemMouseGun;

	public GameObject AvatarItemSlapGun;

	[Space(20f)]
	[Header("Avatar modifier")]
	public AvatarModifier BurningModifier;

	public AvatarModifier MutantModifier;

	public AvatarModifier PoisonModifier;

	public AvatarModifier FrozenModifier;

	public AvatarModifier NinjaRunModifier;

	public AvatarModifier MouseModifier;

	public AvatarModifier GrowthModifier;

	[Header("Particles")]
	[Space(20f)]
	[SerializeField]
	public GameObject ParticleBlood;

	[SerializeField]
	public GameObject ParticleBloodSixShooter;

	[SerializeField]
	public GameObject ParticleBlooxThrowingStar;

	[SerializeField]
	public GameObject ParticleCFX_GroundAura;

	[SerializeField]
	public GameObject ParticleCubeDust;

	[SerializeField]
	public GameObject ParticleCubeDustDestroyed;

	[SerializeField]
	public GameObject ParticleDetailedSmoke;

	[SerializeField]
	public GameObject ParticleDustStorm;

	[SerializeField]
	public GameObject ParticleExplosion;

	[SerializeField]
	public GameObject ParticleFire1;

	[SerializeField]
	public ParticleSystem ParticleFluffySmoke;

	[SerializeField]
	public GameObject ParticleFluffySmokeLarge;

	[SerializeField]
	public GameObject ParticleGhostDeath;

	[SerializeField]
	public GameObject ParticleGunSmoke;

	[SerializeField]
	public GameObject ParticleJetPackParticles;

	[SerializeField]
	public GameObject ParticleSmallExplosion;

	[SerializeField]
	public GameObject ParticleSmokeTrail;

	[SerializeField]
	public GameObject ParticleSparks;

	[SerializeField]
	public GameObject ParticleSparksSixShooter;

	[SerializeField]
	public GameObject ParticleSparksThrowingStar;

	[SerializeField]
	public GameObject ParticleXP;

	[SerializeField]
	[Header("Logic object prefabs")]
	[Space(20f)]
	public GameObject LogicInputConnectorPrefab;

	[SerializeField]
	public GameObject LogicOutputConnectorPrefab;

	[SerializeField]
	public GameObject LogicObjectConnectorPrefab;

	[SerializeField]
	public GameObject LinkObject;

	[SerializeField]
	public GameObject ObjectLinkObject;

	[SerializeField]
	public Material LogicCubeConnectorRedMaterial;

	[SerializeField]
	public Material LogicCubeConnectorRedSelectedMaterial;

	[SerializeField]
	public Material LogicCubeConnectorBlueMaterial;

	[SerializeField]
	public Material LogicCubeConnectorBlueSelectedMaterial;

	[Header("GUI")]
	[SerializeField]
	[Space(20f)]
	public Texture2D AvatarAccessoryMoveIcon;

	[SerializeField]
	public GameObject DrawPlaneObject;

	[SerializeField]
	public Material ModelConstraintsMaterial;

	[Header("UGUI")]
	[SerializeField]
	[Space(20f)]
	public AvatarInputControllerAndroidSettings AvatarInputControllerAndroidSettings;

	[SerializeField]
	public Texture2D crosshairCursor;

	[SerializeField]
	public MaterialButtonTextureGenerator MaterialButtonTextureGenerator;

	[SerializeField]
	public InsertCursor InsertCursor;

	[Space(20f)]
	[SerializeField]
	[Header("Editor")]
	public Material CellCursorErrorMaterial;

	[SerializeField]
	public Material CellCursorMaterial;

	[SerializeField]
	public Material ModelCubeSpaceMaterial;

	[SerializeField]
	public Material Cursor2dEdgeMaterial;

	[SerializeField]
	public Material Cursor2dCornerMaterial;

	[SerializeField]
	public Material CursorMaterial;

	[SerializeField]
	public Material CursorCornerMaterial;

	[SerializeField]
	public Material CursorNoneMaterial;

	[SerializeField]
	public Material InsertPreviewMaterial;

	[SerializeField]
	public Material PreviewBoxMaterial;

	[SerializeField]
	public Material SelectBoxMaterial;

	[SerializeField]
	public SphereVolumeIndicator RangeVisualizationObject;

	[SerializeField]
	public AdvancedGhostIcon GhostEditorIconObject;

	[SerializeField]
	public Material IndentMaterial;

	public static PrefabPool Instance => instance;

	private void Awake()
	{
		instance = this;
		Object.DontDestroyOnLoad(this);
	}
}
