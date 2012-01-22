internal class ESWalkMode : ESStateBase
{
	public override void Enter(EditorStateMachine esm)
	{
		MVCameraController weCamera = MVGameController.Instance.WOCM.WeCamera;
		weCamera.StartTransitionCam(1f);
		weCamera.SetCamera(CameraType.ThirdPerson);
	}

	public override void Execute(EditorStateMachine e)
	{
		if (!MVGameController.Instance.EditorController.PlayInEditor)
		{
			e.PopState();
		}
	}

	public override void Exit(EditorStateMachine esm)
	{
		MVCameraController weCamera = MVGameController.Instance.WOCM.WeCamera;
		weCamera.StartTransitionCam(1f);
		weCamera.SetCamera(CameraType.JetPackCamera);
	}
}
