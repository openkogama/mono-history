using System;
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

	private MovableVisualization movableVisualization;

	private MVCubeModelInstance cubeModel;

	private float distance = 5f;

	private Quaternion orgRotation;

	private Vector3 velocity;

	private Vector3 angularDirection;

	private float angularSpeed;

	private int parentMoverID = -1;

	private bool pausedMovement;

	private MVMovable parentMover;

	public MVCubeModelInstance CubeModel => cubeModel;

	public int CubeModelID => cubeModel.Id;

	public float Distance => distance;

	public Quaternion OrgRotation => orgRotation;

	public Vector3 Velocity => velocity;

	public Vector3 AngularVelocity => angularDirection * angularSpeed;

	public Vector3 AngularDirection => angularDirection;

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

	protected virtual Vector3 WorldVelocity => Transform.localToWorldMatrix * velocity;

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

	public override bool Visible
	{
		get
		{
			return movableVisualization.Visible;
		}
		set
		{
			movableVisualization.Visible = value;
		}
	}

	public MVMovable(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, worldObjects)
	{
		interactionFlags |= InteractionFlags.CanClone;
	}

	public override void Initialize()
	{
		base.Initialize();
		InitializeCommon();
		MVGameControllerBase.WOCM.MoveableController.AddMovable(this, isInventoryPreviewMovable: false);
		movableVisualization = gameObject.AddComponent<MovableVisualization>();
		movableVisualization.Init(cubeModel);
		cubeModel.ReactsToLODChanges = false;
		cubeModel.Visible = false;
		Visible = true;
	}

	public override void ChangeLOD(float distance)
	{
		base.ChangeLOD(distance);
		movableVisualization.ChangeLOD(distance);
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		InitializeCommon();
		MVGameControllerBase.WOCM.MoveableController.AddMovable(this, isInventoryPreviewMovable: true);
	}

	private void InitializeCommon()
	{
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
				MVGameControllerBase.Game.UpdateWorldObjectDataPartial(Id, GetParamPath("Distance"), distance);
			}
		}
		RecalcTimeToEnd();
	}

	public void SetOrgRotation(Quaternion orgRotation, bool updateWOData = false, bool syncServer = false)
	{
		this.orgRotation = orgRotation;
		if (updateWOData)
		{
			blueprintData["Rotation"] = orgRotation.eulerAngles.ToSerializeString();
			if (syncServer)
			{
				MVGameControllerBase.Game.UpdateWorldObjectDataPartial(Id, GetParamPath("Rotation"), orgRotation);
			}
		}
	}

	public void SetVelocity(Vector3 velocity, bool updateWOData = false, bool syncServer = false)
	{
		this.velocity = velocity;
		if (updateWOData)
		{
			blueprintData["Velocity"] = velocity.ToSerializeString();
			if (syncServer)
			{
				MVGameControllerBase.Game.UpdateWorldObjectDataPartial(Id, GetParamPath("Velocity"), velocity);
			}
		}
		RecalcTimeToEnd();
	}

	public void SetAngularDirection(Vector3 angularDirection, bool updateWOData = false, bool syncServer = false)
	{
		this.angularDirection = angularDirection;
		if (updateWOData)
		{
			blueprintData["AngularDirection"] = angularDirection.ToSerializeString();
			if (syncServer)
			{
				MVGameControllerBase.Game.UpdateWorldObjectDataPartial(Id, GetParamPath("AngularDirection"), angularDirection);
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
				MVGameControllerBase.Game.UpdateWorldObjectDataPartial(Id, GetParamPath("AngularSpeed"), angularSpeed);
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
				mVMovable = MVGameControllerBase.WOCM.MoveableController.MoveControllers.Where((KeyValuePair<int, MVMovable> x) => x.Value.Id == parentMoverID).FirstOrDefault().Value;
				if (mVMovable == null)
				{
					Debug.LogError("Couldn't find parent " + parentMoverID);
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
				MVGameControllerBase.Game.UpdateWorldObjectDataPartial(Id, GetParamPath("ParentMoverID"), parentMoverID);
			}
		}
	}

	public void SyncProperties()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary["BlueprintData"] = blueprintData;
		MVGameControllerBase.Game.UpdateWorldObjectDataPartial(Id, dictionary);
	}

	private void ReadWOData()
	{
		string empty = string.Empty;
		foreach (KeyValuePair<object, object> blueprintDatum in blueprintData)
		{
			empty = blueprintDatum.Value.ToString();
			switch (blueprintDatum.Key.ToString())
			{
			case "Rotation":
			{
				Vector3 euler = empty.ToVector3FromSerializeString();
				orgRotation = Quaternion.Euler(euler);
				break;
			}
			case "Velocity":
				velocity = empty.ToVector3FromSerializeString();
				break;
			case "AngularVelocity":
				Debug.LogWarning("Movable still deprecated data AngularVelocity");
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
				MVMovable value = MVGameControllerBase.WOCM.MoveableController.MoveControllers.Where((KeyValuePair<int, MVMovable> x) => x.Value.Id == newParentMoverID).FirstOrDefault().Value;
				if (value == null)
				{
					Debug.LogError("Couldn't find parent " + empty);
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
						Debug.LogWarning("Movable " + id + " init - Could not find child " + num + " to move! If this is a new movable restart the session. Otherwise it is broken.");
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

	private void Move(float directionFactor, int breakid)
	{
		if (cubeModel == null)
		{
			return;
		}
		float num = MVGameControllerBase.WOCM.MoveableController.time;
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
		Vector3 vector = WorldPosition;
		if (ParentMover != null)
		{
			vector = ParentMover.cubeModel.WorldPosition + ParentMover.cubeModel.WorldRotation * (WorldPosition - ParentMover.WorldPosition);
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
				MVGameControllerBase.WOCM.MoveableController.Velocities[GameObjectID] = direction * WorldVelocity * directionFactor * Time.fixedDeltaTime;
			}
		}
		else
		{
			localPos = Vector3.zero;
		}
		cubeModel.WorldPosition = vector + localPos;
		Quaternion quaternion = Quaternion.AngleAxis(57.29578f * (AngularVelocity * num).magnitude, AngularVelocity.normalized);
		Quaternion worldRotation = WorldRotation;
		if (ParentMoverID != -1)
		{
			worldRotation = ParentMover.cubeModel.WorldRotation;
		}
		if (!pausedMovement)
		{
			cubeModel.WorldRotation = worldRotation * quaternion;
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
		float magnitude = WorldVelocity.magnitude;
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
		MVGameControllerBase.WOCM.MoveableController.RemoveMovable(this);
		base.Destroy();
	}

	public override bool OnEnterObject(EditorStateMachine e)
	{
		MVGameControllerBase.CameraController.CurCamera.FocusOnObject(CubeModel);
		e.EnterGroup(this);
		e.SelectWO(CubeModelID, addToSelection: false);
		e.Event = EditorEvent.EditCubes;
		cubeModel.Visible = true;
		return true;
	}

	public override bool OnExitObject(EditorStateMachine e)
	{
		e.ExitGroupToRoot();
		e.Event = EditorEvent.ESTerrainEdit;
		cubeModel.Visible = false;
		return true;
	}
}
