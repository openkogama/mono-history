using System.Collections;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class AdvancedGhostCubeModelWrapper : EditableCubeModelWrapper
{
	private Transform transformParent;

	private bool cubeModelIsBeingEdited;

	public bool CubeModelIsBeingEdited => cubeModelIsBeingEdited;

	public AdvancedGhostCubeModelWrapper(MVCubeModelBase cubeModelBase, Transform transformParent)
		: base(cubeModelBase)
	{
		this.transformParent = transformParent;
		SetToTransformParent();
		SetConstraints(new IntVector(-11, -4, -11), new IntVector(11, 4, 11), 180);
		cubeModelBase.ReactsToLODChanges = false;
		foreach (KeyValuePair<IntVector, ChunkInstances.ChunkInstanceVariables> item in (IEnumerable)cubeModelBase.ChunkInstances)
		{
			item.Value.renderer.enabled = true;
		}
	}

	public bool OnEnterObject(EditorStateMachine e, Transform transform)
	{
		cubeModelIsBeingEdited = true;
		EnterEdit(transform);
		return base.OnEnterObject(e);
	}

	public override bool OnExitObject(EditorStateMachine e)
	{
		ExitEdit();
		return base.OnExitObject(e);
	}

	private void EnterEdit(Transform transform)
	{
		CubeModel.Transform.parent = transform;
		CubeModel.Transform.localPosition = Vector3.zero;
		CubeModel.Transform.localRotation = Quaternion.identity;
		CubeModel.GameObject.SetActive(value: true);
		MonoBehaviour[] components = CubeModel.Transform.GetComponents<MonoBehaviour>();
		foreach (MonoBehaviour monoBehaviour in components)
		{
			monoBehaviour.enabled = false;
		}
	}

	public void ExitEdit()
	{
		SetToTransformParent();
		MonoBehaviour[] components = CubeModel.Transform.GetComponents<MonoBehaviour>();
		foreach (MonoBehaviour monoBehaviour in components)
		{
			monoBehaviour.enabled = true;
		}
		cubeModelIsBeingEdited = false;
	}

	private void SetToTransformParent()
	{
		CubeModel.Transform.parent = transformParent;
		CubeModel.Transform.localPosition = Vector3.zero;
		CubeModel.Transform.localRotation = Quaternion.identity;
	}
}
