using UnityEngine;
using UnityEngine.UI;

public class ProgressBarAndroid : MonoBehaviour
{
	[SerializeField]
	private Scrollbar ProgressBar;

	private float progress;

	public float Progress
	{
		get
		{
			return progress;
		}
		set
		{
			progress = Mathf.Clamp01(value);
			ProgressBar.size = progress;
		}
	}
}
