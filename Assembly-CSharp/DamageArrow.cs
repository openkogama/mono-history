using UnityEngine;
using UnityEngine.UI;

public class DamageArrow : MonoBehaviour
{
	[Header("Dependencies")]
	[SerializeField]
	private Image image;

	public Sprite Sprite
	{
		set
		{
			image.sprite = value;
		}
	}

	public RectTransform RectTransform => image.rectTransform;

	protected void OnEnable()
	{
		image.enabled = true;
	}

	protected void OnDisable()
	{
		image.enabled = false;
	}
}
