using UnityEngine;

public class PrefabPool : MonoBehaviour
{
	private static PrefabPool instance;

	[SerializeField]
	[Header("World Objects")]
	public GameObject MVFirePrefab;

	[SerializeField]
	public GameObject MVAndPrefab;

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
	public GameObject MVBatteryPrefab;

	[SerializeField]
	public GameObject MVCameraSettingsPrefab;

	[SerializeField]
	public GameObject MVCollectiblePrefab;

	[SerializeField]
	public GameObject MVCollectibleFantaPrefab;

	[SerializeField]
	public GameObject MVCheckpointPrefab;

	[SerializeField]
	public GameObject MVExplosivesPrefab;

	[SerializeField]
	public GameObject MVFlagPrefab;

	[SerializeField]
	public GameObject MVGameCoinChestPrefab;

	[SerializeField]
	public GameObject MVGameCoinPrefab;

	[SerializeField]
	public GameObject MVGhostPrefab;

	[SerializeField]
	public GameObject MVGoalPrefab;

	[SerializeField]
	public GameObject MVGhostInstancePrefab;

	[SerializeField]
	public GameObject MVGravityCubePrefab;

	[SerializeField]
	public GameObject MVNegatePrefab;

	[SerializeField]
	public GameObject MVPointLightPrefab;

	[SerializeField]
	public GameObject MVPressurePlatePrefab;

	[SerializeField]
	public GameObject MVPulseBoxPrefab;

	[SerializeField]
	public GameObject MVRandomBoxPrefab;

	[SerializeField]
	public GameObject MVRoundCubePrefab;

	[SerializeField]
	public GameObject MVSkyboxPrefab;

	[SerializeField]
	public GameObject MVSmokePrefab;

	[SerializeField]
	public GameObject MVSoundEmitterPrefab;

	[SerializeField]
	public GameObject MVSpawnPointBluePrefab;

	[SerializeField]
	public GameObject MVSpawnPointGreenPrefab;

	[SerializeField]
	public GameObject MVSpawnPointRedPrefab;

	[SerializeField]
	public GameObject MVSpawnPointYellowPrefab;

	[SerializeField]
	public GameObject MVTextMsgPrefab;

	[SerializeField]
	public GameObject MVTimeTriggerPrefab;

	[SerializeField]
	public GameObject MVToggleBoxPrefab;

	[SerializeField]
	public GameObject MVTriggerBoxPrefab;

	[SerializeField]
	public GameObject MVMovingPlatformGroupPrefab;

	[SerializeField]
	public GameObject MVMovingPlatformNodePrefab;

	[SerializeField]
	public GameObject MVObjectEnablerPrefab;

	[SerializeField]
	public GameObject MVKillLimitPrefab;

	[SerializeField]
	public GameObject MVOculusKillLimitPrefab;

	[SerializeField]
	public GameObject MVSentryGunPrefab;

	[SerializeField]
	public GameObject ShootableButtonPrefab;

	[SerializeField]
	public GameObject MVTeleportGroupPrefab;

	[SerializeField]
	public GameObject MVTeleporterPrefab;

	[SerializeField]
	public GameObject TestLogicCubePrefab;

	[SerializeField]
	public GameObject UseLeverPrefab;

	[SerializeField]
	public GameObject MVWaterPlanePrefab;

	[SerializeField]
	public GameObject WindTurbinePrefab;

	[SerializeField]
	public GameObject MVCountingCube;

	[SerializeField]
	public GameObject TeleportAvatarPrefab;

	[Space(20f)]
	[SerializeField]
	[Header("Game")]
	public Material GhostMarkerMaterial;

	[SerializeField]
	public GameObject TimeTriggerSoundObject;

	[SerializeField]
	public GameObject TriggerBoxSoundObject;

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

	[Header("Pick up")]
	[SerializeField]
	[Space(20f)]
	public GameObject AvatarCenterGunPrefab;

	[SerializeField]
	public GameObject AvatarImpulseGunPrefab;

	[SerializeField]
	public GameObject AvatarHealthPrefab;

	[SerializeField]
	public GameObject AvatarBazookaPrefab;

	[SerializeField]
	public GameObject AvatarRailGunPrefab;

	[SerializeField]
	public GameObject AvatarMutantPrefab;

	[SerializeField]
	public GameObject AvatarSwordPrefab;

	[SerializeField]
	public GameObject AvatarShotgunPrefab;

	[SerializeField]
	public GameObject AvatarFlamethrowerPrefab;

	[SerializeField]
	public GameObject AvatarCubeGunPrefab;

	[SerializeField]
	public GameObject AvatarNinjaRunPrefab;

	[SerializeField]
	public GameObject AvatarSixShooterPrefab;

	[SerializeField]
	public GameObject AvatarDoubleSixShooterPrefab;

	[SerializeField]
	public GameObject AvatarThrowingStarPrefab;

	[SerializeField]
	public GameObject AvatarMultiThrowingStarPrefab;

	[SerializeField]
	public GameObject AvatarMouseGunPrefab;

	[SerializeField]
	public GameObject AvatarGrowthGunPrefab;

	[SerializeField]
	public GameObject AvatarMousePackPrefab;

	[SerializeField]
	public GameObject AvatarGrowthPackPrefab;

	[Header("Avatar item pick up")]
	[Space(20f)]
	[SerializeField]
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

	[Header("Avatar modifier")]
	[Space(20f)]
	public AvatarModifier BurningModifier;

	public AvatarModifier MutantModifier;

	public AvatarModifier PoisonModifier;

	public AvatarModifier FrozenModifier;

	public AvatarModifier NinjaRunModifier;

	public AvatarModifier MouseModifier;

	public AvatarModifier GrowthModifier;

	[Header("Particles")]
	[SerializeField]
	[Space(20f)]
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
	public GameObject ParticleFluffySmoke;

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

	[Space(20f)]
	[SerializeField]
	[Header("Logic object prefabs")]
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

	[Space(20f)]
	[SerializeField]
	[Header("Audio Bank Sounds")]
	public AudioClip ButtonClickAudio;

	[SerializeField]
	public AudioClip HoverAudio;

	[SerializeField]
	public AudioClip ScreenShotAudio;

	[SerializeField]
	public AudioClip TextFieldCharDeleteAudio;

	[SerializeField]
	public AudioClip TextFieldCharEntryAudio;

	[SerializeField]
	public AudioClip ToggleOffAudio;

	[SerializeField]
	public AudioClip ToggleOnAudio;

	[SerializeField]
	public AudioClip InventoryCloseAudio;

	[SerializeField]
	public AudioClip InventoryOpenAudio;

	[SerializeField]
	public AudioClip MenuClickAudio;

	[SerializeField]
	public AudioClip MenuHoverAudio;

	[SerializeField]
	public AudioClip MenuNextAudio;

	[SerializeField]
	public AudioClip MenuPopupCloseAudio;

	[SerializeField]
	public AudioClip MenuPopupOpenAudio;

	[SerializeField]
	public AudioClip MenuPrevAudio;

	[SerializeField]
	public AudioClip MenuPulldownCloseAudio;

	[SerializeField]
	public AudioClip MenuPulldownOpenAudio;

	[SerializeField]
	public AudioClip ModelDeleteAudio;

	[SerializeField]
	public AudioClip StaticMenuCloseAudio;

	[SerializeField]
	public AudioClip StaticMenuOpenAudio;

	[SerializeField]
	[Space(20f)]
	[Header("GUI")]
	public GameObject AvatarAccessoryMoveIcon;

	[SerializeField]
	public GameObject DrawPlaneObject;

	[SerializeField]
	public Material ModelConstraintsMaterial;

	[SerializeField]
	[Header("UX")]
	[Space(20f)]
	public GameObject UXTextObject;

	[SerializeField]
	public GameObject SimpleTextComboBoxItemObject;

	[SerializeField]
	public Material MouseOverMaterial;

	[SerializeField]
	public Material ListMenuDividerMaterial;

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
	public GameObject SelectionGizmoObject;

	[SerializeField]
	public GameObject RightClickGizmoObject;

	[SerializeField]
	public GameObject ItemShopPreviewObject;

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
