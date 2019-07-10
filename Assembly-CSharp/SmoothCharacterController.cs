using UnityEngine;

public class SmoothCharacterController : MonoBehaviour
{
	private MvCharacterController controller;

	private SmoothPhysicsMovement smoothPhysicsMovement;

	public MvCharacterController Controller => controller;

	public void Init(GameObject worldObjectRoot, CullingSubscriberBase cullingSubscriberBase, MVWorldObjectClient worldObjectOwner)
	{
		GameObject gameObject = new GameObject(worldObjectRoot.name + " physics controller");
		gameObject.transform.parent = worldObjectRoot.transform.parent;
		gameObject.transform.position = worldObjectRoot.transform.position;
		gameObject.transform.rotation = worldObjectRoot.transform.rotation;
		controller = gameObject.AddComponent<MVCharacterController3D>();
		smoothPhysicsMovement = base.gameObject.AddComponent<SmoothPhysicsMovement>();
		smoothPhysicsMovement.Init(controller.transform, cullingSubscriberBase, worldObjectOwner);
	}

	public void Reset()
	{
		controller.Velocity = Vector3.zero;
		smoothPhysicsMovement.Reset();
	}

	public void SmoothMove()
	{
		smoothPhysicsMovement.SmoothMove();
	}

	private void OnDestroy()
	{
		Object.Destroy(controller.gameObject);
		controller = null;
		Object.Destroy(smoothPhysicsMovement);
	}

	public SmoothCharacterController Clone(GameObject targetGameObject, GameObject seat, CullingSubscriberBase cullingSubscriberBase, MVWorldObjectClient worldObjectOwner)
	{
		SmoothCharacterController smoothCharacterController = targetGameObject.AddComponent<SmoothCharacterController>();
		smoothCharacterController.Init(targetGameObject, cullingSubscriberBase, worldObjectOwner);
		smoothCharacterController.Controller.Init(Controller.Radius, Controller.Height, seat.transform.localPosition);
		return smoothCharacterController;
	}
}
