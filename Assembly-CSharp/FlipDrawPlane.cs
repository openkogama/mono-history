using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlipDrawPlane : MonoBehaviour
{
	[SerializeField]
	private Color SelectedColor;

	[SerializeField]
	private Color NormalColor;

	[SerializeField]
	private Image XAxisImage;

	[SerializeField]
	private Image YAxisImage;

	[SerializeField]
	private Image ZAxisImage;

	private int currentIndex;

	private List<DrawPlaneAxis> drawPlaneAxises = new List<DrawPlaneAxis>
	{
		DrawPlaneAxis.Y,
		DrawPlaneAxis.X,
		DrawPlaneAxis.Z
	};

	private Image currentlySelectedImage;

	private void Awake()
	{
		InitalizeImages();
		DrawPlane.Orientation = DrawPlaneAxis.Y;
	}

	private void OnEnable()
	{
		for (int i = 0; i < drawPlaneAxises.Count; i++)
		{
			if (drawPlaneAxises[i] == DrawPlane.Orientation)
			{
				currentIndex = i;
				break;
			}
		}
		HighlightImages();
	}

	private void InitalizeImages()
	{
		XAxisImage.color = NormalColor;
		YAxisImage.color = SelectedColor;
		ZAxisImage.color = NormalColor;
		currentlySelectedImage = YAxisImage;
	}

	public void Flip()
	{
		currentIndex++;
		currentIndex %= drawPlaneAxises.Count;
		DrawPlane.Orientation = drawPlaneAxises[currentIndex];
		HighlightImages();
	}

	private void HighlightImages()
	{
		DrawPlaneAxis orientation = DrawPlane.Orientation;
		currentlySelectedImage.color = NormalColor;
		switch (orientation)
		{
		case DrawPlaneAxis.X:
			currentlySelectedImage = XAxisImage;
			break;
		case DrawPlaneAxis.Y:
			currentlySelectedImage = YAxisImage;
			break;
		case DrawPlaneAxis.Z:
			currentlySelectedImage = ZAxisImage;
			break;
		}
		currentlySelectedImage.color = SelectedColor;
	}
}
