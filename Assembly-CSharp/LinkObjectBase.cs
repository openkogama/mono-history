using UnityEngine;

public class LinkObjectBase : MonoBehaviour
{
	public int linkID = -1;

	public bool isObjectLink;

	protected Vector3 startPos;

	protected Vector3 endPos;

	protected bool UpdatePositions(Vector3 newStartPos, Vector3 newEndPos)
	{
		bool result = false;
		if (!Mathf.Approximately((startPos - newStartPos).sqrMagnitude, 0f))
		{
			startPos = newStartPos;
			result = true;
		}
		if (!Mathf.Approximately((endPos - newEndPos).sqrMagnitude, 0f))
		{
			endPos = newEndPos;
			result = true;
		}
		return result;
	}
}
