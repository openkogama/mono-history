using UnityEngine;

public class CollectTheItemObject : ObjectPrefab
{
	private const float timeBeforeBlink = 3f;

	[SerializeField]
	private Collider editCollider;

	[SerializeField]
	private RotateLocal rotator;

	[SerializeField]
	private GameObject visualObject;

	[SerializeField]
	private GameObject cullingObject;

	[SerializeField]
	private TriggerBoxEvents triggerBoxEvents;

	[SerializeField]
	private GreyOutObjectScript greyOutObject;

	[SerializeField]
	private GreyOutObjectScript greyOutScriptEditMode;

	[SerializeField]
	private CollectTheItemBlinker blinker;

	private float fadeTimer;

	private bool greyIn;

	public Collider EditCollider => editCollider;

	public GameObject VisualObject => visualObject;

	public GameObject CullingObject => cullingObject;

	public TriggerBoxEvents TriggerBoxEvents => triggerBoxEvents;

	public RotateLocal RotateLocal => rotator;

	public GreyOutObjectScript GreyOutObject => greyOutObject;

	public GreyOutObjectScript GreyOutScriptEditMode => greyOutScriptEditMode;

	public CollectTheItemBlinker Blinker => blinker;

	public bool EnableFading
	{
		get
		{
			return enabled;
		}
		set
		{
			enabled = value;
		}
	}

	private void Update()
	{
		if (ShouldDoBlinking())
		{
			Blinker.StartBlinking(BlinkType.AboutToExpire);
			EnableFading = false;
		}
	}

	private bool ShouldDoBlinking()
	{
		if (fadeTimer >= 3f)
		{
			return true;
		}
		fadeTimer += Time.deltaTime;
		return false;
	}

	public void InitializeGreyOutScript()
	{
		greyOutObject.InitializeOriginalMaterials();
	}

	protected override void OnValidate()
	{
	}
}
