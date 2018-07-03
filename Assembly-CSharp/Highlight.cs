using MV.WorldObject.HighlightSystem.HighlightPayloads;

public class Highlight<T> where T : HighlightPayloadBase
{
	public readonly int id;

	public readonly T highlightData;

	public Highlight(int id, T highlightData)
	{
		this.id = id;
		this.highlightData = highlightData;
	}
}
