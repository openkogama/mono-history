using System.Collections.Generic;

public class TestLogicCube : MVLogicObject
{
	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => true;

	public TestLogicCube(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.TestLogicCubePrefab, worldObjects)
	{
	}

	public override void DeSelect()
	{
		selectedConnector = SelectedConnector.None;
		base.DeSelect();
	}
}
