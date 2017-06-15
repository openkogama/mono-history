using UnityEngine;

public class MVCollectibleObject : ObjectPrefab
{
	[SerializeField]
	private StreamedTextureToSharedMaterial streamedTextureToSharedMaterialPrefab;

	private static bool streamComponentSet;

	[SerializeField]
	private GreyOutObjectScript pickupItem;

	[SerializeField]
	private GameObject pickupMesh;

	[SerializeField]
	private ObjectParticleEmitterScript particles;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private WorldObjectEnableController worldObjectEnableController;

	[SerializeField]
	private TriggerBoxEvents triggerBoxEvents;

	[SerializeField]
	private AllWorldObjectTriggerBoxEvents allWorldObjectTriggerBoxEvents;

	[SerializeField]
	private CollectibleEffects collectibleEffects;

	public GreyOutObjectScript PickupItem => pickupItem;

	public GameObject PickupMesh => pickupMesh;

	public ObjectParticleEmitterScript Particles => particles;

	public AudioSource AudioSource => audioSource;

	public WorldObjectEnableController WorldObjectEnableController => worldObjectEnableController;

	public TriggerBoxEvents TriggerBoxEvents => triggerBoxEvents;

	public AllWorldObjectTriggerBoxEvents AllWorldObjectTriggerBoxEvents => allWorldObjectTriggerBoxEvents;

	public CollectibleEffects CollectibleEffects => collectibleEffects;

	public void StartTextureStreaming()
	{
		if (!streamComponentSet)
		{
			MVGameControllerBase.StreamingAssetManager.Instantiate(streamedTextureToSharedMaterialPrefab);
			streamComponentSet = true;
		}
	}

	protected override void OnValidate()
	{
	}
}
