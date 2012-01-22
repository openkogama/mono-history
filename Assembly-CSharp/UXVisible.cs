using UnityEngine;

public class UXVisible : MonoBehaviour
{
	public delegate void OnVisibleChangeDelegate(bool visible);

	private bool visible;

	private bool isInitialized;

	public OnVisibleChangeDelegate OnVisibleChange;

	public bool Visible
	{
		get
		{
			return visible;
		}
		set
		{
			visible = value;
			if (OnVisibleChange != null)
			{
				OnVisibleChange(visible);
			}
		}
	}

	public void Start()
	{
		Initialize();
	}

	private void Initialize()
	{
		if (!isInitialized)
		{
			Visible = false;
			isInitialized = true;
		}
	}
}
