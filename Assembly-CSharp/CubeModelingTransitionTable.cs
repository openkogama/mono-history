using System.Collections.Generic;

internal class CubeModelingTransitionTable : StateTransitionTable
{
	public CubeModelingTransitionTable()
	{
		table.Add(CubeModelingEvent.EditCubes, new EditCubes());
		table.Add(CubeModelingEvent.DeleteCubes, new DeleteCubes());
		table.Add(CubeModelingEvent.PaintCubes, new PaintCubes());
		table.Add(CubeModelingEvent.SprayCubes, new SprayCubes());
		foreach (KeyValuePair<object, IState> item in table)
		{
			CubeModelTool cubeModelTool = (CubeModelTool)item.Value;
			cubeModelTool.SetStateType((CubeModelingEvent)item.Key);
		}
	}
}
