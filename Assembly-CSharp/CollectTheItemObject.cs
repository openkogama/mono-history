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

	private float fadeTimer = 3f;

	private bool greyIn;

	private float blinkTime = 0.6f;

	public Collider EditCollider => editCollider;

	public GameObject VisualObject => visualObject;

	public GameObject CullingObject => cullingObject;

	public TriggerBoxEvents TriggerBoxEvents => triggerBoxEvents;

	public RotateLocal RotateLocal => rotator;

	public GreyOutObjectScript GreyOutObject => greyOutObject;

	public GreyOutObjectScript GreyOutScriptEditMode => greyOutScriptEditMode;

	public bool EnableFading { get; set; }

	private void Update()
	{
		if (EnableFading)
		{
			DoBlinking();
		}
	}

	private void DoBlinking()
	{
		fadeTimer -= Time.deltaTime;
		if (fadeTimer <= 0f)
		{
			fadeTimer = blinkTime;
			if (greyIn)
			{
				greyOutObject.GreyIn();
				blinkTime /= 1.3f;
			}
			else
			{
				greyOutObject.GreyOut();
			}
			greyIn = !greyIn;
		}
	}

	public void InitializeGreyOutScript()
	{
		greyOutObject.InitializeOriginalMaterials();
	}

	protected override void OnValidate()
	{
	}
}
