using UnityEngine;

public class GameCoinChestModelSelector : MonoBehaviour
{
	public Renderer closedMesh;

	public Renderer openMesh;

	private void Awake()
	{
		enabled = false;
	}

	public void Open()
	{
		openMesh.enabled = true;
		closedMesh.enabled = false;
	}

	public void Close()
	{
		openMesh.enabled = false;
		closedMesh.enabled = true;
	}

	public bool IsVisible()
	{
		return openMesh.enabled || closedMesh.enabled;
	}
}
