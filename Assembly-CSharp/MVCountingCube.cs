using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.WorldObject;
using UnityEngine;

public class MVCountingCube : MVLogicObject
{
	private const float ConnectorOffset = 1.5f;

	private Vector3 ObjectSize = new Vector3(2f, 1.2f, 0.35f);

	private MVCountingCubeObject cubeObject;

	private int currentValue;

	private int startingValue;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => true;

	public override Vector3 OutputConnectorOffset => Vector3.right * 1.5f;

	public override Vector3 InputConnectorOffset => Vector3.left * 1.5f;

	public MVCountingCube(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVCountingCube, worldObjects)
	{
		cubeObject = (MVCountingCubeObject)component;
		if (Data.ContainsKey("startingValue"))
		{
			startingValue = (int)Data["startingValue"];
		}
		if (RunTimeData.ContainsKey("currentValue"))
		{
			currentValue = (ObscuredInt)RunTimeData["currentValue"];
		}
		else if (Data.ContainsKey("startingValue"))
		{
			currentValue = (int)Data["startingValue"];
		}
		interactionFlags |= InteractionFlags.HasSettings;
		SetText();
	}

	public override void OnDataUpdate()
	{
		startingValue = (currentValue = (int)Data["startingValue"]);
		SetText();
	}

	public void UpdateCurrentValue(int currentValue)
	{
		this.currentValue = currentValue;
		SetText();
		SetOutgoing(currentValue == 0);
	}

	public override void OnInputStateChanged()
	{
		base.OnInputStateChanged();
		if (InputState)
		{
			PlaySound();
		}
	}

	public override Bounds GetLocalBounds(BoundsContext boundsContext)
	{
		return new Bounds(Vector3.zero, ObjectSize);
	}

	private void SetOutgoing(bool isActive)
	{
		if (isActive)
		{
			foreach (Link outputLinkRef in OutputLinkRefs)
			{
				outputLinkRef.isSet = true;
			}
			return;
		}
		foreach (Link outputLinkRef2 in OutputLinkRefs)
		{
			outputLinkRef2.isSet = false;
		}
	}

	public override void Reset()
	{
		UpdateCurrentValue(startingValue);
		PlaySound();
	}

	private void SetText()
	{
		cubeObject.DigitManager.Number = currentValue;
	}

	private void PlaySound()
	{
		cubeObject.AudioSource.Play();
	}

	public override Vector3 GetClosestGridPoint(float gridSize, Vector3 position)
	{
		Vector3 vector = default;
		vector.x *= 2f;
		vector.y = 1.1f;
		vector.z = 0.3f;
		return SharedCubeFunctions.GetClosestGridPoint(position, gameObject.transform.rotation, gridSize, vector);
	}
}
