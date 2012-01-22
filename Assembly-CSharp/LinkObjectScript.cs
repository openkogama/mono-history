using UnityEngine;

public class LinkObjectScript : MonoBehaviour
{
	public int linkID = -1;

	private void Awake()
	{
		((Behaviour)this).enabled = false;
	}
}
