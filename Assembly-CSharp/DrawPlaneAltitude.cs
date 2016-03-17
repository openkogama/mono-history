using UnityEngine;
using UnityEngine.UI;

public class DrawPlaneAltitude : MonoBehaviour
{
	[SerializeField]
	private Text altitudeText;

	private void Update()
	{
		if (altitudeText.text != DrawPlane.Altitude.ToString())
		{
			altitudeText.text = DrawPlane.Altitude.ToString();
		}
	}
}
