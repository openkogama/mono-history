using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic))]
public class ColorStyleObject : MonoBehaviour
{
	[SerializeField]
	private Graphic graphic;

	[SerializeField]
	private ColorStyle imageStyle = ColorStyle.White;

	private void Awake()
	{
		Styles.SetStyle(graphic, imageStyle);
	}

	private void Reset()
	{
		graphic = GetComponent<Graphic>();
		Styles.SetStyle(graphic, imageStyle);
	}

	private void OnValidate()
	{
		if (!Application.isPlaying)
		{
			Styles.SetStyle(graphic, imageStyle);
		}
	}
}
