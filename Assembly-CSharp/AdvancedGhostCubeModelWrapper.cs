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
		foreach (GameObject chunk in cubeModelBase.Chunks)
		{
			chunk.renderer.enabled = true;
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
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		CubeModel.Transform.parent = transform;
		CubeModel.Transform.localPosition = Vector3.zero;
		CubeModel.Transform.localRotation = Quaternion.identity;
		CubeModel.GameObject.SetActiveRecursively(true);
		MonoBehaviour[] components = ((Component)CubeModel.Transform).GetComponents<MonoBehaviour>();
		foreach (MonoBehaviour val in components)
		{
			((Behaviour)val).enabled = false;
		}
	}

	public void ExitEdit()
	{
		SetToTransformParent();
		CubeModel.GameObject.SetActiveRecursively(false);
		MonoBehaviour[] components = ((Component)CubeModel.Transform).GetComponents<MonoBehaviour>();
		foreach (MonoBehaviour val in components)
		{
			((Behaviour)val).enabled = true;
		}
		cubeModelIsBeingEdited = false;
	}

	private void SetToTransformParent()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		CubeModel.Transform.parent = transformParent;
		CubeModel.Transform.localPosition = Vector3.zero;
		CubeModel.Transform.localRotation = Quaternion.identity;
	}
}
