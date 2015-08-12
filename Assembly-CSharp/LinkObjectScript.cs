using UnityEngine;

public class LinkObjectScript : MonoBehaviour
{
	public int linkID = -1;

	public bool isObjectLink;

	private void Awake()
	{
		enabled = false;
	}
}
