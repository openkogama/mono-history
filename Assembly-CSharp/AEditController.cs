public abstract class AEditController : AIngameController
{
	protected CubeModelingController cubeModelingController;

	public WorldEditorDrawPlane WorldEditorDrawPlane => cubeModelingController.WorldEditorDrawPlane;

	public CubeModelingController CubeModelingController => cubeModelingController;

	public EditorStateMachine EditorStateMachine { get; protected set; }

	public override void Initialize()
	{
		EditorStateMachine = new EditorStateMachine();
		base.Initialize();
	}

	public override void HandleInput()
	{
		base.HandleInput();
		EditorStateMachine.Update();
	}
}
