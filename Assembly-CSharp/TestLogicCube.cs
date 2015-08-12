using System.Collections.Generic;

public class TestLogicCube : MVLogicObject
{
	private const string prefabPath = "Prefabs/TestLogicCubeObject";

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => true;

	public TestLogicCube(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/TestLogicCubeObject", worldObjects)
	{
	}

	public override void DeSelect()
	{
		selectedConnector = SelectedConnector.None;
		base.DeSelect();
	}
}
