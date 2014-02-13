using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MVMovable : MVBlueprintBase
{
	private static float direction = 1f;

	private List<MVMovable> MoveableChildren = new List<MVMovable>();

	private Vector3 localPos = Vector3.zero;

	private float timeToEnd;

	private float linearTime;

	private float fraction;

	private MVCubeModelInstance cubeModel;

	private float distance = 5f;

	private Quaternion orgRotation;

	private Vector3 velocity;

	private Vector3 angularDirection;

	private float angularSpeed;

	private int parentMoverID;

	private bool pausedMovement;

	private MVMovable parentMover;

	private MVNetworkGame Game => MVGameController.Instance.Game;

	public MVCubeModelInstance CubeModel => cubeModel;

	public int CubeModelID => cubeModel.Id;

	public float Distance => distance;

	public Quaternion OrgRotation
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return orgRotation;
		}
	}

	public Vector3 Velocity
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return velocity;
		}
	}

	public Vector3 AngularVelocity
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			return angularDirection * angularSpeed;
		}
	}

	public Vector3 AngularDirection
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return angularDirection;
		}
	}

	public float AngularSpeed => angularSpeed;

	public int ParentMoverID => parentMoverID;

	public bool PausedMovement
	{
		get
		{
			return pausedMovement;
		}
		set
		{
			pausedMovement = value;
		}
	}

	protected virtual Vector3 WorldVelocity
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			return Vector4.op_Implicit(Transform.localToWorldMatrix * Vector4.op_Implicit(velocity));
		}
	}

	public MVMovable ParentMover => parentMover;

	public bool IsRoot => parentMover == null;

	public MVMovable RootMover
	{
		get
		{
			if (ParentMover == null)
			{
				return this;
			}
			return ParentMover.RootMover;
		}
	}

	public MVMovable(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, worldObjects)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		interactionFlags |= InteractionFlags.CanClone;
	}

	public override void Initialize()
	{
		base.Initialize();
		InitializeCommon();
		MVGameController.Instance.WOCM.MoveableController.AddMovable(this, isInventoryPreviewMovable: false);
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		InitializeCommon();
		MVGameController.Instance.WOCM.MoveableController.AddMovable(this, isInventoryPreviewMovable: true);
	}

	private void InitializeCommon()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		ReadWOData();
		if (cubeModel != null)
		{
			cubeModel.Position = Vector3.zero;
			cubeModel.Rotation = Quaternion.identity;
			orgRotation = cubeModel.WorldRotation;
		}
	}

	private string GetParamPath(string param)
	{
		return "BlueprintData\\" + param.ToString();
	}

	public void SetDistance(float distance, bool updateWOData = false, bool syncServer = false)
	{
		this.distance = distance;
		if (updateWOData)
		{
			blueprintData["Distance"] = distance;
			if (syncServer)
			{
				Game.UpdateWorldObjectDataPartial(Id, GetParamPath("Distance"), distance);
			}
		}
		RecalcTimeToEnd();
	}

	public void SetOrgRotation(Quaternion orgRotation, bool updateWOData = false, bool syncServer = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		this.orgRotation = orgRotation;
		if (updateWOData)
		{
			blueprintData["Rotation"] = orgRotation.eulerAngles.ToSerializeString();
			if (syncServer)
			{
				Game.UpdateWorldObjectDataPartial(Id, GetParamPath("Rotation"), orgRotation);
			}
		}
	}

	public void SetVelocity(Vector3 velocity, bool updateWOData = false, bool syncServer = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		this.velocity = velocity;
		if (updateWOData)
		{
			blueprintData["Velocity"] = velocity.ToSerializeString();
			if (syncServer)
			{
				Game.UpdateWorldObjectDataPartial(Id, GetParamPath("Velocity"), velocity);
			}
		}
		RecalcTimeToEnd();
	}

	public void SetAngularDirection(Vector3 angularDirection, bool updateWOData = false, bool syncServer = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		this.angularDirection = angularDirection;
		if (updateWOData)
		{
			blueprintData["AngularDirection"] = angularDirection.ToSerializeString();
			if (syncServer)
			{
				Game.UpdateWorldObjectDataPartial(Id, GetParamPath("AngularDirection"), angularDirection);
			}
		}
	}

	public void SetAngularSpeed(float angularSpeed, bool updateWOData = false, bool syncServer = false)
	{
		this.angularSpeed = angularSpeed;
		if (updateWOData)
		{
			blueprintData["AngularSpeed"] = angularSpeed;
			if (syncServer)
			{
				Game.UpdateWorldObjectDataPartial(Id, GetParamPath("AngularSpeed"), angularSpeed);
			}
		}
	}

	public void SetParentMoverID(int parentMoverID, bool updateWOData = false, bool syncServer = false)
	{
		if (this.parentMoverID != parentMoverID)
		{
			MVMovable mVMovable = null;
			if (parentMoverID != -1)
			{
				mVMovable = MVGameController.Instance.WOCM.MoveableController.MoveControllers.Where((KeyValuePair<int, MVMovable> x) => x.Value.Id == parentMoverID).FirstOrDefault().Value;
				if (mVMovable == null)
				{
					Debug.LogError((object)("Couldn't find parent " + parentMoverID));
					return;
				}
			}
			if (parentMover != null)
			{
				parentMover.RemoveMovableChild(this);
			}
			this.parentMoverID = parentMoverID;
			parentMover = mVMovable;
			if (parentMover != null)
			{
				parentMover.AddMovableChild(this);
			}
		}
		if (updateWOData)
		{
			blueprintData["ParentMoverID"] = parentMoverID;
			if (syncServer)
			{
				Game.UpdateWorldObjectDataPartial(Id, GetParamPath("ParentMoverID"), parentMoverID);
			}
		}
	}

	public void SyncProperties()
	{
		Hashtable hashtable = new Hashtable();
		hashtable["BlueprintData"] = blueprintData;
		Game.UpdateWorldObjectDataPartial(Id, hashtable);
	}

	private void ReadWOData()
	{
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		string empty = string.Empty;
		foreach (DictionaryEntry blueprintDatum in blueprintData)
		{
			empty = blueprintDatum.Value.ToString();
			switch (blueprintDatum.Key.ToString())
			{
			case "Rotation":
			{
				Vector3 val = empty.ToVector3FromSerializeString();
				orgRotation = Quaternion.Euler(val);
				break;
			}
			case "Velocity":
				velocity = empty.ToVector3FromSerializeString();
				break;
			case "AngularVelocity":
				Debug.LogWarning((object)"Movable still deprecated data AngularVelocity");
				break;
			case "AngularDirection":
				angularDirection = empty.ToVector3FromSerializeString();
				break;
			case "AngularSpeed":
				angularSpeed = (float)blueprintData["AngularSpeed"];
				break;
			case "Distance":
				distance = Convert.ToSingle(empty);
				break;
			case "ParentID":
			{
				int newParentMoverID = Convert.ToInt32(empty);
				if (parentMoverID == newParentMoverID)
				{
					break;
				}
				MVMovable value = MVGameController.Instance.WOCM.MoveableController.MoveControllers.Where((KeyValuePair<int, MVMovable> x) => x.Value.Id == newParentMoverID).FirstOrDefault().Value;
				if (value == null)
				{
					Debug.LogError((object)("Couldn't find parent " + empty));
					break;
				}
				if (parentMover != null)
				{
					parentMover.RemoveMovableChild(this);
				}
				parentMoverID = newParentMoverID;
				parentMover = value;
				parentMover.AddMovableChild(this);
				break;
			}
			case "ChildrenMap":
				if (childIdMap.ContainsKey("movable"))
				{
					int num = (int)childIdMap["movable"];
					cubeModel = (MVCubeModelInstance)GetChild(num);
					if (cubeModel == null)
					{
						Debug.LogWarning((object)("Movable " + id + " init - Could not find child " + num + " to move! If this is a new movable restart the session. Otherwise it is broken."));
					}
				}
				break;
			}
		}
		RecalcTimeToEnd();
	}

	public void UpdateMoverTree(float directionFactor)
	{
		if (!IsRoot)
		{
			ParentMover.UpdateMoverTree(directionFactor);
		}
		else
		{
			Move(directionFactor, 0);
		}
	}

	public void UpdateMoverSubTree(float directionFactor, int breakid)
	{
		if (!IsRoot)
		{
			ParentMover.UpdateMoverSubTree(directionFactor, breakid);
		}
		else
		{
			Move(directionFactor, breakid);
		}
	}

	public void AddMovableChild(MVMovable child)
	{
		MoveableChildren.Add(child);
	}

	public void RemoveMovableChild(MVMovable child)
	{
		MoveableChildren.Remove(child);
	}

	public void Move(float directionFactor, int breakid)
	{
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		if (cubeModel == null)
		{
			return;
		}
		float num = MVGameController.Instance.WOCM.MoveableController.time;
		if (pausedMovement)
		{
			linearTime = fraction * timeToEnd;
		}
		else
		{
			if (directionFactor < 0f)
			{
				num -= Time.fixedDeltaTime;
			}
			linearTime = 0f;
			fraction = 0f;
			if (timeToEnd != 0f)
			{
				linearTime = num % (2f * timeToEnd);
				fraction = linearTime / timeToEnd;
				if (timeToEnd > 1f)
				{
					fraction = 2f - fraction;
				}
			}
		}
		Vector3 val = WorldPosition;
		if (ParentMover != null)
		{
			val = ParentMover.cubeModel.WorldPosition + ParentMover.cubeModel.WorldRotation * (WorldPosition - ParentMover.WorldPosition);
		}
		if (timeToEnd > 0.01f)
		{
			direction = 1f;
			if (linearTime < timeToEnd)
			{
				localPos = WorldVelocity * linearTime;
			}
			else
			{
				localPos = WorldVelocity * (2f * timeToEnd - linearTime);
				direction = -1f;
			}
			if (directionFactor > 0f)
			{
				MVGameController.Instance.WOCM.MoveableController.Velocities[GameObjectID] = direction * WorldVelocity * directionFactor * Time.fixedDeltaTime;
			}
		}
		else
		{
			localPos = Vector3.zero;
		}
		cubeModel.WorldPosition = val + localPos;
		Vector3 val2 = AngularVelocity * num;
		float num2 = 57.29578f * val2.magnitude;
		Vector3 angularVelocity = AngularVelocity;
		Quaternion val3 = Quaternion.AngleAxis(num2, angularVelocity.normalized);
		Quaternion worldRotation = WorldRotation;
		if (ParentMoverID != 0)
		{
			worldRotation = ParentMover.cubeModel.WorldRotation;
		}
		if (!pausedMovement)
		{
			cubeModel.WorldRotation = worldRotation * val3;
		}
		if (breakid == CubeModelID)
		{
			return;
		}
		foreach (MVMovable moveableChild in MoveableChildren)
		{
			moveableChild.Move(directionFactor, breakid);
		}
	}

	private void RecalcTimeToEnd()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Vector3 worldVelocity = WorldVelocity;
		float magnitude = worldVelocity.magnitude;
		timeToEnd = 0f;
		if (magnitude > 0.001f)
		{
			timeToEnd = Distance / magnitude;
		}
	}

	public override void OnDataUpdate()
	{
		base.OnDataUpdate();
		ReadWOData();
	}

	public override void Destroy()
	{
		MVGameController.Instance.WOCM.MoveableController.RemoveMovable(this);
		base.Destroy();
	}

	public override bool OnEnterObject(EditorStateMachine e)
	{
		MVGameController.Instance.Game.CameraController.CurCamera.FocusOnObject(CubeModel);
		e.EnterGroup(this);
		e.SelectWO(CubeModelID, addToSelection: false);
		e.Event = EditorEvent.EditCubes;
		return true;
	}

	public override bool OnExitObject(EditorStateMachine e)
	{
		e.ExitGroupToRoot();
		e.Event = EditorEvent.ESTerrainEdit;
		return true;
	}
}
