using UnityEngine;

public class CollectTheItemObject : ObjectPrefab
{
	private const float timeBeforeBlink = 3f;

	[SerializeField]
	private RotateLocal rotator;

	[SerializeField]
	private GameObject visualObject;

	[SerializeField]
	private TriggerBoxEvents triggerBoxEvents;

	[SerializeField]
	private GreyOutObjectScript greyOutObject;

	private float fadeTimer = 3f;

	private bool greyIn;

	private float blinkTime = 0.6f;

	public GameObject VisualObject => visualObject;

	public TriggerBoxEvents TriggerBoxEvents => triggerBoxEvents;

	public RotateLocal RotateLocal => rotator;

	public GreyOutObjectScript GreyOutObject => greyOutObject;

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
