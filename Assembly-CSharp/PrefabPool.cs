using UnityEngine;

public class PrefabPool : MonoBehaviour
{
	[SerializeField]
	private EnumPoolManager enumPoolManager;

	private static PrefabPool instance;

	[Header("World Objects")]
	[SerializeField]
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
	private VehicleBaseObject mvHamsterWheelPrefab;

	[SerializeField]
	private VehicleBaseObject mvHoverCraftPrefab;

	[SerializeField]
	private CollectTheItemDropOffObject collectTheItemDropOff;

	[SerializeField]
	private CollectTheItemObject collectTheItemCollectable;

	[SerializeField]
	private CollectTheItemLineObject collectTheItem;

	[SerializeField]
	private ObjectiveArrow collectTheItemDropOffArrow;

	[SerializeField]
	private VehicleBaseObject mvJetPackPrefab;

	[SerializeField]
	private VehicleBaseObject mvJetPackDeluxePrefab;

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
	private SoundEmitterObject mvSoundEmitterPrefab;

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
	private SpawnerObject spawnerObject;

	[SerializeField]
	private GodzillaTriggerObject godzillaTriggerPrefab;

	[SerializeField]
	private GameObject godzillaAreaPrefab;

	[Space(20f)]
	[Header("Game")]
	[SerializeField]
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

	[SerializeField]
	[Space(20f)]
	[Header("Pick up")]
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

	[SerializeField]
	[Header("Avatar item pick up")]
	[Space(20f)]
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
	private GameObject avatarItemGodzillaLaser;

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

	[SerializeField]
	private GameObject avatarItemCollectTheItem;

	[Header("Avatar modifier")]
	[Space(20f)]
	[SerializeField]
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

	[SerializeField]
	private AvatarModifier shieldModifier;

	[SerializeField]
	private GodzillaModifier godzillaModifier;

	[SerializeField]
	private GodzillaLaserBurnModifier godzillaLaserBurnModifier;

	[SerializeField]
	private InvulnerabilityModifier invulnerabilityModifier;

	[Header("Particles")]
	[Space(20f)]
	[SerializeField]
	private GameObject particleCFX_GroundAura;

	[SerializeField]
	private ParticleSystem particleCubeDust;

	[SerializeField]
	private ParticleSystem particleCubeDustDestroyed;

	[SerializeField]
	private ParticleSystem particleExplosion;

	[SerializeField]
	private ParticleSystem particleFluffySmoke;

	[SerializeField]
	private ParticleSystem goldExplosion;

	[SerializeField]
	private ParticleSystem collectTheItemParticles;

	[SerializeField]
	[Space(20f)]
	[Header("Logic object prefabs")]
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

	[SerializeField]
	[Header("GUI")]
	[Space(20f)]
	private Texture2D avatarAccessoryMoveIcon;

	[SerializeField]
	private GameObject drawPlaneObject;

	[SerializeField]
	private Material modelConstraintsMaterial;

	[Space(20f)]
	[SerializeField]
	[Header("UGUI")]
	private AvatarInputControllerAndroidSettings avatarInputControllerAndroidSettings;

	[SerializeField]
	private Texture2D crosshairCursor;

	[SerializeField]
	private MaterialButtonTextureGenerator materialButtonTextureGenerator;

	[SerializeField]
	private InsertCursor insertCursor;

	[Space(20f)]
	[SerializeField]
	[Header("Cameras")]
	private GodzillaCamera godzillaCamera;

	[SerializeField]
	private GodzillaCamera2D godzillaCamera2D;

	[SerializeField]
	private FirstPersonDeathCamera firstPersonDeathCamera;

	[Space(20f)]
	[SerializeField]
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

	public VehicleBaseObject MVHamsterWheelPrefab => mvHamsterWheelPrefab;

	public VehicleBaseObject MVHoverCraftPrefab => mvHoverCraftPrefab;

	public CollectTheItemDropOffObject CollectTheItemDropOffPrefab => collectTheItemDropOff;

	public CollectTheItemObject CollectTheItemCollectablePrefab => collectTheItemCollectable;

	public CollectTheItemLineObject CollectTheItemPrefab => collectTheItem;

	public ObjectiveArrow CollectTheItemDropOffArrowPrefab => collectTheItemDropOffArrow;

	public VehicleBaseObject MVJetPackPrefab => mvJetPackPrefab;

	public VehicleBaseObject MVJetPackDeluxePrefab => mvJetPackDeluxePrefab;

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

	public SoundEmitterObject MVSoundEmitterPrefab => mvSoundEmitterPrefab;

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

	public SpawnerObject SpawnerObjectPrefab => spawnerObject;

	public GodzillaTriggerObject GodzillaTriggerPrefab => godzillaTriggerPrefab;

	public GameObject GodzillaAreaPrefab => godzillaAreaPrefab;

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

	public GameObject AvatarItemGodzillaLaser => avatarItemGodzillaLaser;

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

	public GameObject AvatarItemCollectTheItem => avatarItemCollectTheItem;

	public AvatarModifier BurningModifier => burningModifier;

	public AvatarModifier MutantModifier => mutantModifier;

	public AvatarModifier PoisonModifier => poisonModifier;

	public AvatarModifier FrozenModifier => frozenModifier;

	public AvatarModifier NinjaRunModifier => ninjaRunModifier;

	public AvatarModifier MouseModifier => mouseModifier;

	public AvatarModifier GrowthModifier => growthModifier;

	public AvatarModifier ShieldModifier => shieldModifier;

	public GodzillaModifier GodzillaModifierS
	{
		get
		{
			godzillaModifier.Type = GodzillaModifier.GodzillaModifierPackageType.S;
			return godzillaModifier;
		}
	}

	public GodzillaModifier GodzillaModifierM
	{
		get
		{
			godzillaModifier.Type = GodzillaModifier.GodzillaModifierPackageType.M;
			return godzillaModifier;
		}
	}

	public GodzillaModifier GodzillaModifierL
	{
		get
		{
			godzillaModifier.Type = GodzillaModifier.GodzillaModifierPackageType.L;
			return godzillaModifier;
		}
	}

	public GodzillaModifier GodzillaModifierXL
	{
		get
		{
			godzillaModifier.Type = GodzillaModifier.GodzillaModifierPackageType.XL;
			return godzillaModifier;
		}
	}

	public GodzillaLaserBurnModifier GodzillaLaserBurnModifierS
	{
		get
		{
			godzillaLaserBurnModifier.SetType(AvatarModifierPackageType.GodzillaLaserBurnS);
			return godzillaLaserBurnModifier;
		}
	}

	public GodzillaLaserBurnModifier GodzillaLaserBurnModifierM
	{
		get
		{
			godzillaLaserBurnModifier.SetType(AvatarModifierPackageType.GodzillaLaserBurnM);
			return godzillaLaserBurnModifier;
		}
	}

	public GodzillaLaserBurnModifier GodzillaLaserBurnModifierL
	{
		get
		{
			godzillaLaserBurnModifier.SetType(AvatarModifierPackageType.GodzillaLaserBurnL);
			return godzillaLaserBurnModifier;
		}
	}

	public GodzillaLaserBurnModifier GodzillaLaserBurnModifierXL
	{
		get
		{
			godzillaLaserBurnModifier.SetType(AvatarModifierPackageType.GodzillaLaserBurnXL);
			return godzillaLaserBurnModifier;
		}
	}

	public InvulnerabilityModifier GodzillaGrowthInvulnerabilityModifier
	{
		get
		{
			invulnerabilityModifier.SetType(AvatarModifierPackageType.GodzillaGrowthInvulnerability);
			return invulnerabilityModifier;
		}
	}

	public InvulnerabilityModifier SpawnProtectionModifier
	{
		get
		{
			invulnerabilityModifier.SetType(AvatarModifierPackageType.SpawnProtection);
			return invulnerabilityModifier;
		}
	}

	public GameObject ParticleCFX_GroundAura => particleCFX_GroundAura;

	public ParticleSystem ParticleCubeDust => particleCubeDust;

	public ParticleSystem ParticleCubeDustDestroyed => particleCubeDustDestroyed;

	public ParticleSystem ParticleExplosion => particleExplosion;

	public ParticleSystem ParticleFluffySmoke => particleFluffySmoke;

	public ParticleSystem GoldExplosion => goldExplosion;

	public ParticleSystem CollectTheItemParticles => collectTheItemParticles;

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

	public GodzillaCamera GodzillaCamera => godzillaCamera;

	public GodzillaCamera2D GodzillaCamera2D => godzillaCamera2D;

	public FirstPersonDeathCamera FirstPersonDeathCamera => firstPersonDeathCamera;

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
