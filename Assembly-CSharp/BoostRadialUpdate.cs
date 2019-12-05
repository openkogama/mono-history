using UnityEngine;
using UnityEngine.UI;

public class BoostRadialUpdate : MonoBehaviour
{
	[SerializeField]
	private Image radialImage;

	private Boost boost;

	public void Initialize(Boost boost)
	{
		this.boost = boost;
	}

	private void Update()
	{
		if (boost != null)
		{
			radialImage.fillAmount = 1f - boost.BoostSecondsLeft / boost.BoostMaxDurationSeconds;
		}
	}
}
