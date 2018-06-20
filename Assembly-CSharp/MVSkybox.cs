using System.Collections.Generic;
using UnityEngine;

public class MVSkybox : MVLogicObject, ILogicWorldObject
{
	private const float sunAngle = 80f;

	private const float fogDensity = 0.007f;

	private bool inventoryObject;

	protected SkyboxManager skybox;

	private InteractionFlags defaultInteractionFlags;

	public override MVWorldObjectDocumentationType DocumentationType => MVWorldObjectDocumentationType.Skybox;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => false;

	public Color SkyboxColor
	{
		get
		{
			if (Data.ContainsKey("color"))
			{
				float[] array = (float[])Data["color"];
				return new Color(array[0], array[1], array[2]);
			}
			Debug.LogError("Skybox data does not contain color");
			return SkyboxManager.defaultColor;
		}
	}

	public float SunAngle
	{
		get
		{
			if (Data.ContainsKey("sunAngle"))
			{
				return (float)Data["sunAngle"];
			}
			Debug.LogError("Skybox data does not contain sunAngle");
			return 80f;
		}
	}

	public float FogDensity
	{
		get
		{
			if (Data.ContainsKey("fogDensity"))
			{
				return (float)Data["fogDensity"];
			}
			Debug.LogError("Skybox data does not contain fogDensity");
			return 0.007f;
		}
	}

	public IInputSignalReceiver InputSignalReceiver { get; private set; }

	public bool SkyboxActive => InputSignalReceiver.CurrentlyIsHot;

	public MVSkybox(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVSkyboxPrefab, worldObjects)
	{
		interactionFlags |= InteractionFlags.CanResetLogic;
		interactionFlags |= InteractionFlags.HasSettings;
		defaultInteractionFlags = interactionFlags;
	}

	public override void Initialize()
	{
		base.Initialize();
		skybox = MVGameControllerBase.SkyboxManager;
		skybox.Add(this);
		gameObject.transform.localScale = Vector3.one;
		SetupCulling(gameObject);
		InputSignalReceiver = LogicClientsideFactory.CreateStateChangeInputSignalReceiver(this, defaultInput: true, null, InputStateUpdateCallback);
		skybox.RefreshColor();
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		inventoryObject = true;
	}

	private void InputStateUpdateCallback(LogicInputState logicInputState, LogicObjectManager logicObjectManager)
	{
		if (logicInputState == LogicInputState.FromColdToHot || logicInputState == LogicInputState.FromHotToCold)
		{
			skybox.RefreshColor();
		}
	}

	public override void OnDataUpdate()
	{
		LogicObjectManager.ResetChunk(Id, MVGameControllerBase.WOCM);
	}

	public override void Reset()
	{
		skybox.RefreshColor();
	}

	public override void Destroy()
	{
		if (inventoryObject)
		{
			base.Destroy();
			return;
		}
		skybox.Remove(this);
		skybox.RefreshColor();
		base.Destroy();
	}

	public void SetDefaultInteractionFlags()
	{
		interactionFlags = defaultInteractionFlags;
	}

	public void SetDeleteOnlyInteractionFlags()
	{
		interactionFlags = InteractionFlags.Selectable;
	}
}
