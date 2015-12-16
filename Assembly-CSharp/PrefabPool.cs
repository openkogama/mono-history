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

	[SerializeField]
	[Header("Pick up")]
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

	[SerializeField]
	[Header("Particles")]
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

	[SerializeField]
	[Space(20f)]
	[Header("Logic object prefabs")]
	public GameObject LogicInputConnectorPrefab;

	[SerializeField]
	public GameObject LogicOutputConnectorPrefab;

	[SerializeField]
	public GameObject LogicObjectConnectorPrefab;

	public static PrefabPool Instance => instance;

	private void Awake()
	{
		instance = this;
		Object.DontDestroyOnLoad(this);
	}
}
