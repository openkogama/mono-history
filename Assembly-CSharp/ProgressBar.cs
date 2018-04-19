using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
	[SerializeField]
	private Scrollbar progressBar;

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
			progressBar.size = progress;
		}
	}
}
