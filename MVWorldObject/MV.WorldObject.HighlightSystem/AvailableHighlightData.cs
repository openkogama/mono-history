namespace MV.WorldObject.HighlightSystem;

public class AvailableHighlightData
{
	public int id;

	public int slot;

	public HighlightType highlightType;

	public string payload;

	public override string ToString()
	{
		return $"id {id}. slot {slot}. highlightType {highlightType}. payload {payload}.";
	}
}
