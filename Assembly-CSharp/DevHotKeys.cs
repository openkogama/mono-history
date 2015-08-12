using System;
using System.Collections;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class DevHotKeys
{
	public void HandleInput()
	{
		EditorTestKeys();
	}

	private void EditorTestKeys()
	{
		if (Application.isEditor && MVInputWrapper.DebugGetKey(KeyCode.Alpha0))
		{
			if (MVInputWrapper.DebugGetKey(KeyCode.Alpha8))
			{
				ChristianHotKeys.Handle();
			}
			else if (MVInputWrapper.DebugGetKey(KeyCode.Alpha9))
			{
				ValdisHotKeys.Handle();
			}
			else if (MVInputWrapper.DebugGetKey(KeyCode.Alpha7))
			{
				CasparHotKeys.Handle();
			}
			else if (MVInputWrapper.DebugGetKey(KeyCode.Alpha6))
			{
				ReneHotKeys.Handle();
			}
			else if (MVInputWrapper.DebugGetKey(KeyCode.Alpha5))
			{
				JakobHotKeys.Handle();
			}
			else
			{
				HandleCommonHotKeys();
			}
		}
	}

	private void HandleCommonHotKeys()
	{
		if (MVInputWrapper.DebugGetKeyDown(KeyCode.D))
		{
			UXDialogFactory uXDialogFactory = UXUtils.UXDialogFactory;
			uXDialogFactory.CreateCustomDevelopmentDialog("Prefabs/GUI/Dev Tools/SelectDevToolDialog", "Dev Tools", noButtons: true, stackDialog: true).Show();
		}
		if (MVInputWrapper.DebugGetKeyDown(KeyCode.O))
		{
			UXDialogFactory uXDialogFactory2 = UXUtils.UXDialogFactory;
			if (uXDialogFactory2.CurrentDialogBox == null)
			{
				uXDialogFactory2.CreateCustomDevelopmentDialog("Prefabs/GUI/Dev Tools/MovableDebugDialog", "Movable").AddPositiveButton(TM._("Apply")).AddNegativeButton(TM._("Cancel"))
					.SetOnResultCallback(UpdateMovable)
					.Show();
			}
		}
		else if (MVInputWrapper.DebugGetKeyDown(KeyCode.U))
		{
			MVGameController.EditController.EditorStateMachine.PushState(EditorEvent.ESWaitForUngroup);
		}
		else if (MVInputWrapper.DebugGetKeyDown(KeyCode.G))
		{
			MVGameController.EditController.EditorStateMachine.PushState(EditorEvent.ESWaitForGroup);
		}
		if (MVInputWrapper.DebugGetKeyDown(KeyCode.L) && !MVInputWrapper.DebugGetKey(KeyCode.LeftShift))
		{
			MVWorldObjectClient settingsDialogSelectionWO = MVGameController.EditorController.GetSettingsDialogSelectionWO();
			if (settingsDialogSelectionWO != null && settingsDialogSelectionWO is MVCubeModelBase)
			{
				MVCubeModelBase cm = (MVCubeModelBase)settingsDialogSelectionWO;
				ObjExporterScript.CubeModelToFile(cm);
			}
		}
		else
		{
			if (!MVInputWrapper.DebugGetKeyDown(KeyCode.L) || !MVInputWrapper.DebugGetKey(KeyCode.LeftShift))
			{
				return;
			}
			if (MVGameController.GameMode == MVGameMode.CharacterEditor)
			{
				MVBody currentBody = MVGameController.CharacterEditorController.CurrentBody;
				{
					foreach (MVWorldObjectClient child in currentBody.Children)
					{
						if (child is MVCubeModelBase)
						{
							MVCubeModelBase cm2 = (MVCubeModelBase)child;
							ObjExporterScript.CubeModelToFile(cm2);
						}
					}
					return;
				}
			}
			ObjExporterScript.CubeModelToFile(MVGameController.WOCM.GetSingletonWorldObject<MVCubeModelPrototypeTerrain>());
		}
	}

	private void RemoveFromHugeChunkTest()
	{
		MVCubeModelPrototypeTerrain singletonWorldObject = MVGameController.WOCM.GetSingletonWorldObject<MVCubeModelPrototypeTerrain>();
		MVCubeModelFineGrainedTerrain singletonWorldObject2 = MVGameController.WOCM.GetSingletonWorldObject<MVCubeModelFineGrainedTerrain>();
		RemoveCubes.RemoveOneCube.HandleRemoveOneCube(new IntVector(7, 23, 7), singletonWorldObject, singletonWorldObject2);
	}

	private void InitialRemoveFromHugeChunkTest()
	{
		MVCubeModelPrototypeTerrain singletonWorldObject = MVGameController.WOCM.GetSingletonWorldObject<MVCubeModelPrototypeTerrain>();
		MVCubeModelFineGrainedTerrain singletonWorldObject2 = MVGameController.WOCM.GetSingletonWorldObject<MVCubeModelFineGrainedTerrain>();
		Debug.Log(Time.realtimeSinceStartup);
		IntVector intVector = new IntVector(-8, 8, -8);
		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				for (int k = 0; k < 8; k++)
				{
					IntVector fineGrainedPosition = intVector + new IntVector(i, j, k) * 2;
					RemoveCubes.RemoveOneCube.HandleRemoveOneCube(fineGrainedPosition, singletonWorldObject, singletonWorldObject2);
				}
			}
		}
		Debug.Log(Time.realtimeSinceStartup);
	}

	private void DoRemoveCubeTest()
	{
		MVCubeModelPrototypeTerrain singletonWorldObject = MVGameController.WOCM.GetSingletonWorldObject<MVCubeModelPrototypeTerrain>();
		MVCubeModelFineGrainedTerrain singletonWorldObject2 = MVGameController.WOCM.GetSingletonWorldObject<MVCubeModelFineGrainedTerrain>();
		Debug.Log(Time.realtimeSinceStartup);
		RemoveCubes.RemoveCubesWithinRadius.HandleRemoveCubes(singletonWorldObject, 5f, new IntVector(0, 3, 0), 100f, DamageFallOffType.Linear, singletonWorldObject2, MVGameController.Game.MaterialRepository.GetMaterialPhysicalProperties);
		Debug.Log(Time.realtimeSinceStartup);
	}

	private MVBody FindUnattachedBody()
	{
		return (MVBody)MVGameController.WOCM.GetWorldObjectClientWhere((MVWorldObjectClient wo) => wo is MVBody mVBody && mVBody.AttachedAvatar == null);
	}

	private string GetHashtableString(Hashtable data)
	{
		string text = string.Empty;
		bool flag = true;
		foreach (DictionaryEntry datum in data)
		{
			if (!flag)
			{
				text += ", ";
			}
			text += "{\"";
			text += datum.Key;
			text += "\", ";
			text += datum.Value;
			text += "}";
			flag = false;
		}
		return text;
	}

	private void InitializedAvatarBodyDataHandler(object sender, InitializedGameQueryDataEventArgs e)
	{
		World world = MVGameController.Game.World;
		world.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Remove(world.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(InitializedAvatarBodyDataHandler));
		Debug.Log("InitializedAvatarBodyDataHandler");
		if (e.RootWO != null)
		{
			int id = e.RootWO.Id;
			MVNetworkGame game = MVGameController.Game;
			game.LockHierarchy(id, lockHierarchy: true);
			game.TransferOwnership(id, 0, null);
		}
	}

	private void UpdateMovable(UXDialogBox dialogBox)
	{
		if (dialogBox.DialogResult != UXDialogResult.Positive)
		{
			return;
		}
		Hashtable hashtable = (Hashtable)dialogBox.GetResult();
		string text = (string)hashtable["Rot"];
		string text2 = (string)hashtable["Vel"];
		string text3 = (string)hashtable["AngVel"];
		string text4 = (string)hashtable["Dist"];
		string text5 = (string)hashtable["ParentID"];
		if (MVGameController.EditorController.EditorStateMachine.SingleSelectedWO == null)
		{
			return;
		}
		Debug.Log(MVGameController.EditorController.EditorStateMachine.SingleSelectedWO);
		if (MVGameController.EditorController.EditorStateMachine.SingleSelectedWO is MVMovable mVMovable)
		{
			Debug.Log("Setting movable params");
			if (text != null && text.Length != 0)
			{
				Quaternion orgRotation = Quaternion.Euler(StringToVec3(text));
				mVMovable.SetOrgRotation(orgRotation, updateWOData: true);
			}
			if (text2 != null && text2.Length != 0)
			{
				Vector3 velocity = StringToVec3(text2);
				mVMovable.SetVelocity(velocity, updateWOData: true);
			}
			if (text3 == null || text3.Length != 0)
			{
			}
			if (text4 != null && text4.Length != 0)
			{
				text4 = text4.Replace("x", ".");
				float distance = Convert.ToSingle(text4);
				mVMovable.SetDistance(distance, updateWOData: true);
			}
			if (text5 != null && text5.Length != 0)
			{
				text5 = text5.Replace("x", ".");
				int parentMoverID = Convert.ToInt32(text5);
				mVMovable.SetParentMoverID(parentMoverID, updateWOData: true);
			}
			mVMovable.SyncProperties();
		}
	}

	private Vector3 StringToVec3(string vecText)
	{
		string text = vecText.Replace("x", ".");
		string[] array = text.Split(' ');
		return new Vector3(Convert.ToSingle(array[0]), Convert.ToSingle(array[1]), Convert.ToSingle(array[2]));
	}

	private void CommonOverlapTestTest()
	{
		MVSpawnPoint mVSpawnPoint = (MVSpawnPoint)MVGameController.WOCM.GetWorldObjectsByType(WorldObjectType.SpawnPoint)[0];
		float num = 1.5f;
		Vector3 position = mVSpawnPoint.Position;
		DebugClass.DrawPoint(mVSpawnPoint.Position, 100f);
		Collider[] array = Physics.OverlapSphere(mVSpawnPoint.Position, num);
		List<CommonOverlapResult> list = new List<CommonOverlapResult>();
		List<int> list2 = new List<int>();
		for (int i = 0; i < array.Length; i++)
		{
			MVWorldObjectClient mVObject = MVWorldObjectClientManager.GetMVObject(array[i].transform);
			if (mVObject is MVCubeModelBase)
			{
				CommonOverlapArg overlapArg = new CommonOverlapArg(mVObject);
				if (SphereOverlapTest.OverlapWo(overlapArg, num, position, out var overlapResult))
				{
					list.Add(overlapResult);
					Debug.Log(overlapResult.cubes.Count);
					list2.Add(overlapResult.woId);
				}
			}
		}
	}

	private void FireBazooka()
	{
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(MVInputWrapper.GetPointerPosition().x, MVInputWrapper.GetPointerPosition().y));
		VoxelHit voxelHit = default;
		if (CollisionDetection.MVHit(ray, out voxelHit))
		{
			Missile missile = Missile.CreateMissile();
			missile.transform.position = MVGameController.WOCM.AvatarLocal.GameObject.transform.position;
			missile.Fire(voxelHit.point);
		}
	}

	private void MoveGroupToCenterOfMass(EditorStateMachine e)
	{
		if (e.SingleSelectedWO != null && !(e.SingleSelectedWO is MVGroup))
		{
			Debug.Log("MoveGroupToCenterOfMass: selected object is not a MVGroup");
			return;
		}
		HashSet<int> selectionSet = new HashSet<int>(e.SelectedIDs);
		if (!e.NetworkSelector.RequestOwnership(selectionSet))
		{
			Debug.Log("MoveGroupToCenterOfMass: Could not get ownership");
			return;
		}
		float gridSize = ((!MVGameController.EditorController.IsGridSnap()) ? 0.0625f : 1f);
		MVGroup mVGroup = e.SingleSelectedWO as MVGroup;
		Vector3 closestGridPoint = mVGroup.GetClosestGridPoint(gridSize, mVGroup.GetLocalBounds(BoundsContext.Default).center);
		mVGroup.Position += closestGridPoint;
		foreach (MVWorldObjectClient child in mVGroup.Children)
		{
			child.Position -= closestGridPoint;
		}
		e.NetworkSelector.RequestReleaseOwnership(selectionSet);
	}
}
