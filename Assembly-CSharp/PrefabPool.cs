using UnityEngine;

public class PrefabPool : MonoBehaviour
{
	[SerializeField]
	private EnumPoolManager enumPoolManager;

	private static PrefabPool instance;

	[SerializeField]
	[Header("World Objects")]
	private ObjectPrefab mvFirePrefab;

	[SerializeField]
	private ObjectPrefab mvAndPrefab;

	[SerializeField]
	private GameObject mvAvatarPrefab;

	[SerializeField]
	private GameObject mvAdvancedGhostPrefab;

	[SerializeField]
	private GameObject mvBodyPrefab;

	[SerializeField]
	private GameObject mvHamsterWheelPrefab;

	[SerializeField]
	private GameObject mvHoverCraftPrefab;

	[SerializeField]
	private GameObject mvJetPackPrefab;

	[SerializeField]
	private GameObject mvJetPackDeluxePrefab;

	[SerializeField]
	private ObjectPrefab mvBatteryPrefab;

	[SerializeField]
	private ObjectPrefab mvCameraSettingsPrefab;

	[SerializeField]
	private MVCollectibleObject mvCollectiblePrefab;

	[SerializeField]
	private MVCollectibleObject mvCollectibleFantaPrefab;

	[SerializeField]
	private ObjectPrefab mvCheckpointPrefab;

	[SerializeField]
	private ObjectPrefab mvExplosivesPrefab;

	[SerializeField]
	private ObjectPrefab mvFlagPrefab;

	[SerializeField]
	private ObjectPrefab mvGameCoinChestPrefab;

	[SerializeField]
	private MVGameCoinObject mvGameCoinPrefab;

	[SerializeField]
	private GameObject mvGhostPrefab;

	[SerializeField]
	private ObjectPrefab mvGoalPrefab;

	[SerializeField]
	private GameObject mvGhostInstancePrefab;

	[SerializeField]
	private ObjectPrefab mvGravityCubePrefab;

	[SerializeField]
	private ObjectPrefab mvNegatePrefab;

	[SerializeField]
	private MVPointLightObject mvPointLightPrefab;

	[SerializeField]
	private MVPressurePlateObject mvPressurePlatePrefab;

	[SerializeField]
	private ObjectPrefab mvPulseBoxPrefab;

	[SerializeField]
	private ObjectPrefab mvRandomBoxPrefab;

	[SerializeField]
	private ObjectPrefab mvRoundCubePrefab;

	[SerializeField]
	private ObjectPrefab mvSkyboxPrefab;

	[SerializeField]
	private ObjectPrefab mvSmokePrefab;

	[SerializeField]
	private ObjectPrefab mvSoundEmitterPrefab;

	[SerializeField]
	private ObjectPrefab mvSpawnPointBluePrefab;

	[SerializeField]
	private ObjectPrefab mvSpawnPointGreenPrefab;

	[SerializeField]
	private ObjectPrefab mvSpawnPointRedPrefab;

	[SerializeField]
	private ObjectPrefab mvSpawnPointYellowPrefab;

	[SerializeField]
	private MVTextMsgObject mvTextMsgPrefab;

	[SerializeField]
	private ObjectPrefab mvTimeTriggerPrefab;

	[SerializeField]
	private ObjectPrefab mvToggleBoxPrefab;

	[SerializeField]
	private MVTriggerBoxObject mvTriggerBoxPrefab;

	[SerializeField]
	private GameObject mvMovingPlatformGroupPrefab;

	[SerializeField]
	private GameObject mvMovingPlatformNodePrefab;

	[SerializeField]
	private MVObjectEnablerObject mvObjectEnablerPrefab;

	[SerializeField]
	private ObjectPrefab mvKillLimitPrefab;

	[SerializeField]
	private ObjectPrefab mvOculusKillLimitPrefab;

	[SerializeField]
	private MVSentryGunObject mvSentryGunPrefab;

	[SerializeField]
	private ShootableButtonObject shootableButtonPrefab;

	[SerializeField]
	private GameObject mvTeleportGroupPrefab;

	[SerializeField]
	private MVTeleporterObject mvTeleporterPrefab;

	[SerializeField]
	private UseLeverObject useLeverPrefab;

	[SerializeField]
	private ObjectPrefab mvWaterPlanePrefab;

	[SerializeField]
	private WindTurbineObject windTurbinePrefab;

	[SerializeField]
	private MVCountingCubeObject mvCountingCubePrefab;

	[SerializeField]
	private TeleportAvatar teleportAvatarPrefab;

	[SerializeField]
	[Header("Game")]
	[Space(20f)]
	private Material ghostMarkerMaterial;

	[SerializeField]
	private Material objectHiddenMaterial;

	[SerializeField]
	private SentryGunBeam iceBeamObject;

	[SerializeField]
	private SentryGunBeam fireBeamObject;

	[SerializeField]
	private StarDisplayObject starDisplayPrefab;

	[SerializeField]
	private LevelDisplayCube levelDisplayPrefab;

	[SerializeField]
	private GameCoinDisplayObject gameCoinDisplayPrefab;

	[SerializeField]
	private CubeModelChunkPrefab cubeModelChunkPrefab;

	[Space(20f)]
	[Header("Pick up")]
	[SerializeField]
	private MVPickupItemBaseObject avatarCenterGunPrefab;

	[SerializeField]
	private MVPickupItemBaseObject avatarImpulseGunPrefab;

	[SerializeField]
	private MVPickupItemBaseObject avatarHealthPrefab;

	[SerializeField]
	private MVPickupItemBaseObject avatarBazookaPrefab;

	[SerializeField]
	private MVPickupItemBaseObject avatarRailGunPrefab;

	[SerializeField]
	private MVPickupItemBaseObject avatarMutantPrefab;

	[SerializeField]
	private MVPickupItemBaseObject avatarSwordPrefab;

	[SerializeField]
	private MVPickupItemBaseObject avatarShotgunPrefab;

	[SerializeField]
	private MVPickupItemBaseObject avatarFlamethrowerPrefab;

	[SerializeField]
	private MVPickupItemBaseObject avatarCubeGunPrefab;

	[SerializeField]
	private MVPickupItemBaseObject avatarNinjaRunPrefab;

	[SerializeField]
	private MVPickupItemBaseObject avatarSixShooterPrefab;

	[SerializeField]
	private MVPickupItemBaseObject avatarDoubleSixShooterPrefab;

	[SerializeField]
	private MVPickupItemBaseObject avatarThrowingStarPrefab;

	[SerializeField]
	private MVPickupItemBaseObject avatarMultiThrowingStarPrefab;

	[SerializeField]
	private MVPickupItemBaseObject avatarMouseGunPrefab;

	[SerializeField]
	private MVPickupItemBaseObject avatarGrowthGunPrefab;

	[SerializeField]
	private MVPickupItemBaseObject avatarMousePackPrefab;

	[SerializeField]
	private MVPickupItemBaseObject avatarGrowthPackPrefab;

	[Header("Avatar item pick up")]
	[Space(20f)]
	[SerializeField]
	private GameObject avatarItemCenterGun;

	[SerializeField]
	private GameObject avatarItemImpulseGun;

	[SerializeField]
	private GameObject avatarItemLaserPointer;

	[SerializeField]
	private GameObject avatarItemBazooka;

	[SerializeField]
	private GameObject avatarItemHand;

	[SerializeField]
	private GameObject avatarItemRailGun;

	[SerializeField]
	private GameObject avatarItemSword;

	[SerializeField]
	private GameObject avatarItemShotgun;

	[SerializeField]
	private GameObject avatarItemFlamethrower;

	[SerializeField]
	private GameObject avatarItemCubeGun;

	[SerializeField]
	private GameObject avatarItemSixShooter;

	[SerializeField]
	private GameObject avatarItemDoubleSixShooter;

	[SerializeField]
	private GameObject avatarItemThrowingStar;

	[SerializeField]
	private GameObject avatarItemMultiThrowingStar;

	[SerializeField]
	private GameObject avatarItemGrowthGun;

	[SerializeField]
	private GameObject avatarItemMouseGun;

	[SerializeField]
	private GameObject avatarItemSlapGun;

	[Space(20f)]
	[SerializeField]
	[Header("Avatar modifier")]
	private AvatarModifier burningModifier;

	[SerializeField]
	private AvatarModifier mutantModifier;

	[SerializeField]
	private AvatarModifier poisonModifier;

	[SerializeField]
	private AvatarModifier frozenModifier;

	[SerializeField]
	private AvatarModifier ninjaRunModifier;

	[SerializeField]
	private AvatarModifier mouseModifier;

	[SerializeField]
	private AvatarModifier growthModifier;

	[Header("Particles")]
	[SerializeField]
	[Space(20f)]
	private GameObject particleBlood;

	[SerializeField]
	private GameObject particleBloodSixShooter;

	[SerializeField]
	private GameObject particleBlooxThrowingStar;

	[SerializeField]
	private GameObject particleCFX_GroundAura;

	[SerializeField]
	private ParticleSystem particleCubeDust;

	[SerializeField]
	private ParticleSystem particleCubeDustDestroyed;

	[SerializeField]
	private GameObject particleDetailedSmoke;

	[SerializeField]
	private GameObject particleDustStorm;

	[SerializeField]
	private ParticleSystem particleExplosion;

	[SerializeField]
	private GameObject particleFire1;

	[SerializeField]
	private ParticleSystem particleFluffySmoke;

	[SerializeField]
	private GameObject particleFluffySmokeLarge;

	[SerializeField]
	private GameObject particleGhostDeath;

	[SerializeField]
	private GameObject particleGunSmoke;

	[SerializeField]
	private GameObject particleJetPackParticles;

	[SerializeField]
	private GameObject particleSmallExplosion;

	[SerializeField]
	private GameObject particleSmokeTrail;

	[SerializeField]
	private GameObject particleSparks;

	[SerializeField]
	private GameObject particleSparksSixShooter;

	[SerializeField]
	private GameObject particleSparksThrowingStar;

	[SerializeField]
	private ParticleSystem particleXP;

	[Header("Logic object prefabs")]
	[Space(20f)]
	[SerializeField]
	private GameObject logicInputConnectorPrefab;

	[SerializeField]
	private GameObject logicOutputConnectorPrefab;

	[SerializeField]
	private GameObject logicObjectConnectorPrefab;

	[SerializeField]
	private GameObject linkObject;

	[SerializeField]
	private GameObject objectLinkObject;

	[SerializeField]
	private Material logicCubeConnectorRedMaterial;

	[SerializeField]
	private Material logicCubeConnectorRedSelectedMaterial;

	[SerializeField]
	private Material logicCubeConnectorBlueMaterial;

	[SerializeField]
	private Material logicCubeConnectorBlueSelectedMaterial;

	[Space(20f)]
	[SerializeField]
	[Header("GUI")]
	private Texture2D avatarAccessoryMoveIcon;

	[SerializeField]
	private GameObject drawPlaneObject;

	[SerializeField]
	private Material modelConstraintsMaterial;

	[Header("UGUI")]
	[Space(20f)]
	[SerializeField]
	private AvatarInputControllerAndroidSettings avatarInputControllerAndroidSettings;

	[SerializeField]
	private Texture2D crosshairCursor;

	[SerializeField]
	private MaterialButtonTextureGenerator materialButtonTextureGenerator;

	[SerializeField]
	private InsertCursor insertCursor;

	[SerializeField]
	[Space(20f)]
	[Header("Editor")]
	private Material cellCursorErrorMaterial;

	[SerializeField]
	private Material cellCursorMaterial;

	[SerializeField]
	private Material modelCubeSpaceMaterial;

	[SerializeField]
	private Material cursor2dEdgeMaterial;

	[SerializeField]
	private Material cursor2dCornerMaterial;

	[SerializeField]
	private Material cursorMaterial;

	[SerializeField]
	private Material cursorCornerMaterial;

	[SerializeField]
	private Material cursorNoneMaterial;

	[SerializeField]
	private Material insertPreviewMaterial;

	[SerializeField]
	private Material previewBoxMaterial;

	[SerializeField]
	private Material selectBoxMaterial;

	[SerializeField]
	private SphereVolumeIndicator rangeVisualizationObject;

	[SerializeField]
	private AdvancedGhostIcon ghostEditorIconObject;

	[SerializeField]
	private Material indentMaterial;

	public EnumPoolManager EnumPoolManager => enumPoolManager;

	public static PrefabPool Instance => instance;

	public ObjectPrefab MVFirePrefab => mvFirePrefab;

	public ObjectPrefab MVAndPrefab => mvAndPrefab;

	public GameObject MVAvatarPrefab => mvAvatarPrefab;

	public GameObject MVAdvancedGhostPrefab => mvAdvancedGhostPrefab;

	public GameObject MVBodyPrefab => mvBodyPrefab;

	public GameObject MVHamsterWheelPrefab => mvHamsterWheelPrefab;

	public GameObject MVHoverCraftPrefab => mvHoverCraftPrefab;

	public GameObject MVJetPackPrefab => mvJetPackPrefab;

	public GameObject MVJetPackDeluxePrefab => mvJetPackDeluxePrefab;

	public ObjectPrefab MVBatteryPrefab => mvBatteryPrefab;

	public ObjectPrefab MVCameraSettingsPrefab => mvCameraSettingsPrefab;

	public MVCollectibleObject MVCollectiblePrefab => mvCollectiblePrefab;

	public MVCollectibleObject MVCollectibleFantaPrefab => mvCollectibleFantaPrefab;

	public ObjectPrefab MVCheckpointPrefab => mvCheckpointPrefab;

	public ObjectPrefab MVExplosivesPrefab => mvExplosivesPrefab;

	public ObjectPrefab MVFlagPrefab => mvFlagPrefab;

	public ObjectPrefab MVGameCoinChestPrefab => mvGameCoinChestPrefab;

	public MVGameCoinObject MVGameCoinPrefab => mvGameCoinPrefab;

	public GameObject MVGhostPrefab => mvGhostPrefab;

	public ObjectPrefab MVGoalPrefab => mvGoalPrefab;

	public GameObject MVGhostInstancePrefab => mvGhostInstancePrefab;

	public ObjectPrefab MVGravityCubePrefab => mvGravityCubePrefab;

	public ObjectPrefab MVNegatePrefab => mvNegatePrefab;

	public MVPointLightObject MVPointLightPrefab => mvPointLightPrefab;

	public MVPressurePlateObject MVPressurePlatePrefab => mvPressurePlatePrefab;

	public ObjectPrefab MVPulseBoxPrefab => mvPulseBoxPrefab;

	public ObjectPrefab MVRandomBoxPrefab => mvRandomBoxPrefab;

	public ObjectPrefab MVRoundCubePrefab => mvRoundCubePrefab;

	public ObjectPrefab MVSkyboxPrefab => mvSkyboxPrefab;

	public ObjectPrefab MVSmokePrefab => mvSmokePrefab;

	public ObjectPrefab MVSoundEmitterPrefab => mvSoundEmitterPrefab;

	public ObjectPrefab MVSpawnPointBluePrefab => mvSpawnPointBluePrefab;

	public ObjectPrefab MVSpawnPointGreenPrefab => mvSpawnPointGreenPrefab;

	public ObjectPrefab MVSpawnPointRedPrefab => mvSpawnPointRedPrefab;

	public ObjectPrefab MVSpawnPointYellowPrefab => mvSpawnPointYellowPrefab;

	public MVTextMsgObject MVTextMsgPrefab => mvTextMsgPrefab;

	public ObjectPrefab MVTimeTriggerPrefab => mvTimeTriggerPrefab;

	public ObjectPrefab MVToggleBoxPrefab => mvToggleBoxPrefab;

	public MVTriggerBoxObject MVTriggerBoxPrefab => mvTriggerBoxPrefab;

	public GameObject MVMovingPlatformGroupPrefab => mvMovingPlatformGroupPrefab;

	public GameObject MVMovingPlatformNodePrefab => mvMovingPlatformNodePrefab;

	public MVObjectEnablerObject MVObjectEnablerPrefab => mvObjectEnablerPrefab;

	public ObjectPrefab MVKillLimitPrefab => mvKillLimitPrefab;

	public ObjectPrefab MVOculusKillLimitPrefab => mvOculusKillLimitPrefab;

	public MVSentryGunObject MVSentryGunPrefab => mvSentryGunPrefab;

	public ShootableButtonObject ShootableButtonPrefab => shootableButtonPrefab;

	public GameObject MVTeleportGroupPrefab => mvTeleportGroupPrefab;

	public MVTeleporterObject MVTeleporterPrefab => mvTeleporterPrefab;

	public UseLeverObject UseLeverPrefab => useLeverPrefab;

	public ObjectPrefab MVWaterPlanePrefab => mvWaterPlanePrefab;

	public WindTurbineObject WindTurbinePrefab => windTurbinePrefab;

	public MVCountingCubeObject MVCountingCubePrefab => mvCountingCubePrefab;

	public TeleportAvatar TeleportAvatarPrefab => teleportAvatarPrefab;

	public Material GhostMarkerMaterial => ghostMarkerMaterial;

	public Material ObjectHiddenMaterial => objectHiddenMaterial;

	public SentryGunBeam IceBeamObject => iceBeamObject;

	public SentryGunBeam FireBeamObject => fireBeamObject;

	public StarDisplayObject StarDisplayPrefab => starDisplayPrefab;

	public LevelDisplayCube LevelDisplayPrefab => levelDisplayPrefab;

	public GameCoinDisplayObject GameCoinDisplayPrefab => gameCoinDisplayPrefab;

	public CubeModelChunkPrefab CubeModelChunkPrefab => cubeModelChunkPrefab;

	public MVPickupItemBaseObject AvatarCenterGunPrefab => avatarCenterGunPrefab;

	public MVPickupItemBaseObject AvatarImpulseGunPrefab => avatarImpulseGunPrefab;

	public MVPickupItemBaseObject AvatarHealthPrefab => avatarHealthPrefab;

	public MVPickupItemBaseObject AvatarBazookaPrefab => avatarBazookaPrefab;

	public MVPickupItemBaseObject AvatarRailGunPrefab => avatarRailGunPrefab;

	public MVPickupItemBaseObject AvatarMutantPrefab => avatarMutantPrefab;

	public MVPickupItemBaseObject AvatarShotgunPrefab => avatarShotgunPrefab;

	public MVPickupItemBaseObject AvatarSwordPrefab => avatarSwordPrefab;

	public MVPickupItemBaseObject AvatarFlamethrowerPrefab => avatarFlamethrowerPrefab;

	public MVPickupItemBaseObject AvatarCubeGunPrefab => avatarCubeGunPrefab;

	public MVPickupItemBaseObject AvatarNinjaRunPrefab => avatarNinjaRunPrefab;

	public MVPickupItemBaseObject AvatarSixShooterPrefab => avatarSixShooterPrefab;

	public MVPickupItemBaseObject AvatarDoubleSixShooterPrefab => avatarDoubleSixShooterPrefab;

	public MVPickupItemBaseObject AvatarThrowingStarPrefab => avatarThrowingStarPrefab;

	public MVPickupItemBaseObject AvatarMultiThrowingStarPrefab => avatarMultiThrowingStarPrefab;

	public MVPickupItemBaseObject AvatarMouseGunPrefab => avatarMouseGunPrefab;

	public MVPickupItemBaseObject AvatarGrowthGunPrefab => avatarGrowthGunPrefab;

	public MVPickupItemBaseObject AvatarMousePackPrefab => avatarMousePackPrefab;

	public MVPickupItemBaseObject AvatarGrowthPackPrefab => avatarGrowthPackPrefab;

	public GameObject AvatarItemCenterGun => avatarItemCenterGun;

	public GameObject AvatarItemImpulseGun => avatarItemImpulseGun;

	public GameObject AvatarItemLaserPointer => avatarItemLaserPointer;

	public GameObject AvatarItemBazooka => avatarItemBazooka;

	public GameObject AvatarItemHand => avatarItemHand;

	public GameObject AvatarItemRailGun => avatarItemRailGun;

	public GameObject AvatarItemSword => avatarItemSword;

	public GameObject AvatarItemShotgun => avatarItemShotgun;

	public GameObject AvatarItemFlamethrower => avatarItemFlamethrower;

	public GameObject AvatarItemCubeGun => avatarItemCubeGun;

	public GameObject AvatarItemSixShooter => avatarItemSixShooter;

	public GameObject AvatarItemDoubleSixShooter => avatarItemDoubleSixShooter;

	public GameObject AvatarItemThrowingStar => avatarItemThrowingStar;

	public GameObject AvatarItemMultiThrowingStar => avatarItemMultiThrowingStar;

	public GameObject AvatarItemGrowthGun => avatarItemGrowthGun;

	public GameObject AvatarItemMouseGun => avatarItemMouseGun;

	public GameObject AvatarItemSlapGun => avatarItemSlapGun;

	public AvatarModifier BurningModifier => burningModifier;

	public AvatarModifier MutantModifier => mutantModifier;

	public AvatarModifier PoisonModifier => poisonModifier;

	public AvatarModifier FrozenModifier => frozenModifier;

	public AvatarModifier NinjaRunModifier => ninjaRunModifier;

	public AvatarModifier MouseModifier => mouseModifier;

	public AvatarModifier GrowthModifier => growthModifier;

	public GameObject ParticleBlood => particleBlood;

	public GameObject ParticleBloodSixShooter => particleBloodSixShooter;

	public GameObject ParticleBlooxThrowingStar => particleBlooxThrowingStar;

	public GameObject ParticleCFX_GroundAura => particleCFX_GroundAura;

	public ParticleSystem ParticleCubeDust => particleCubeDust;

	public ParticleSystem ParticleCubeDustDestroyed => particleCubeDustDestroyed;

	public GameObject ParticleDetailedSmoke => particleDetailedSmoke;

	public GameObject ParticleDustStorm => particleDustStorm;

	public ParticleSystem ParticleExplosion => particleExplosion;

	public GameObject ParticleFire1 => particleFire1;

	public ParticleSystem ParticleFluffySmoke => particleFluffySmoke;

	public GameObject ParticleFluffySmokeLarge => particleFluffySmokeLarge;

	public GameObject ParticleGhostDeath => particleGhostDeath;

	public GameObject ParticleGunSmoke => particleGunSmoke;

	public GameObject ParticleJetPackParticles => particleJetPackParticles;

	public GameObject ParticleSmallExplosion => particleSmallExplosion;

	public GameObject ParticleSmokeTrail => particleSmokeTrail;

	public GameObject ParticleSparks => particleSparks;

	public GameObject ParticleSparksSixShooter => particleSparksSixShooter;

	public GameObject ParticleSparksThrowingStar => particleSparksThrowingStar;

	public ParticleSystem ParticleXP => particleXP;

	public GameObject LogicInputConnectorPrefab => logicInputConnectorPrefab;

	public GameObject LogicOutputConnectorPrefab => logicOutputConnectorPrefab;

	public GameObject LogicObjectConnectorPrefab => logicObjectConnectorPrefab;

	public GameObject LinkObject => linkObject;

	public GameObject ObjectLinkObject => objectLinkObject;

	public Material LogicCubeConnectorRedMaterial => logicCubeConnectorRedMaterial;

	public Material LogicCubeConnectorRedSelectedMaterial => logicCubeConnectorRedSelectedMaterial;

	public Material LogicCubeConnectorBlueMaterial => logicCubeConnectorBlueMaterial;

	public Material LogicCubeConnectorBlueSelectedMaterial => logicCubeConnectorBlueSelectedMaterial;

	public Texture2D AvatarAccessoryMoveIcon => avatarAccessoryMoveIcon;

	public GameObject DrawPlaneObject => drawPlaneObject;

	public Material ModelConstraintsMaterial => modelConstraintsMaterial;

	public AvatarInputControllerAndroidSettings AvatarInputControllerAndroidSettings => avatarInputControllerAndroidSettings;

	public Texture2D CrosshairCursor => crosshairCursor;

	public MaterialButtonTextureGenerator MaterialButtonTextureGenerator => materialButtonTextureGenerator;

	public InsertCursor InsertCursor => insertCursor;

	public Material CellCursorErrorMaterial => cellCursorErrorMaterial;

	public Material CellCursorMaterial => cellCursorMaterial;

	public Material ModelCubeSpaceMaterial => modelCubeSpaceMaterial;

	public Material Cursor2dEdgeMaterial => cursor2dEdgeMaterial;

	public Material Cursor2dCornerMaterial => cursor2dCornerMaterial;

	public Material CursorMaterial => cursorMaterial;

	public Material CursorCornerMaterial => cursorCornerMaterial;

	public Material CursorNoneMaterial => cursorNoneMaterial;

	public Material InsertPreviewMaterial => insertPreviewMaterial;

	public Material PreviewBoxMaterial => previewBoxMaterial;

	public Material SelectBoxMaterial => selectBoxMaterial;

	public SphereVolumeIndicator RangeVisualizationObject => rangeVisualizationObject;

	public AdvancedGhostIcon GhostEditorIconObject => ghostEditorIconObject;

	public Material IndentMaterial => indentMaterial;

	private void Awake()
	{
		instance = this;
		Object.DontDestroyOnLoad(this);
	}
}
