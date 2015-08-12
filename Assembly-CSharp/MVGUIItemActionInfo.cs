using System;
using MV.WorldObject;
using UnityEngine;

public class MVGUIItemActionInfo : MVGUIItemAction
{
	public UXText nameText;

	public UXText silverPriceText;

	public UXText goldPriceText;

	public Transform viewItemRoot;

	public Transform descriptionOffset;

	public UXText descriptionText;

	public UXPlane descriptionBG;

	public UXSlider descriptionSlider;

	private bool showDescriptionSlider;

	private MVGUIItemShopPreview preview;

	private float descriptionTextBoxHeight => descriptionBG.Height - 0.5f;

	public override void UpdateItemAction(MVItem item)
	{
		base.UpdateItemAction(item);
		nameText.Text = ItemNameToLocalizedString.GetToolTipTextFromItemName(item.name);
		silverPriceText.Text = string.Empty + item.priceSilver;
		goldPriceText.Text = string.Empty + item.priceGold;
		if (item.description != null)
		{
			descriptionText.Text = item.description;
		}
		BuildViewItem(item);
		if (descriptionText.TextHeight > descriptionBG.Height)
		{
			InitializeSlider();
			showDescriptionSlider = true;
		}
	}

	protected override void Initialize()
	{
		base.Initialize();
		UXGroup component = GetComponent<UXGroup>();
		component.OnShowGroup = (UXGroup.OnGroupEventDelegate)Delegate.Combine(component.OnShowGroup, (UXGroup.OnGroupEventDelegate)(() =>
		{
			if (preview != null)
			{
				preview.ItemImagePlane.SetVisible(visible: true);
			}
			viewItemRoot.gameObject.SetActive(value: true);
			descriptionText.SetVisible(visible: true);
			descriptionSlider.SetVisible(showDescriptionSlider);
		}));
		UXGroup component2 = GetComponent<UXGroup>();
		component2.OnHideGroup = (UXGroup.OnGroupEventDelegate)Delegate.Combine(component2.OnHideGroup, (UXGroup.OnGroupEventDelegate)(() =>
		{
			if (preview != null)
			{
				preview.ItemImagePlane.SetVisible(visible: false);
			}
			viewItemRoot.gameObject.SetActive(value: false);
			descriptionText.SetVisible(visible: false);
		}));
	}

	private void BuildViewItem(MVItem item)
	{
		preview = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/ShopPreview/ItemShopPreview")) as GameObject).GetComponent<MVGUIItemShopPreview>();
		preview.BuildItemShopPreview(item, 16f, 16f);
		preview.transform.parent = viewItemRoot;
		preview.transform.localPosition = Vector3.zero;
		preview.transform.localScale = Vector3.one;
	}

	private void InitializeSlider()
	{
		descriptionSlider.Initialize();
		descriptionSlider.OnValueChangedIntermediate = Slide;
		descriptionSlider.OnValueChanged = SlideEnd;
		UpdateSliderSize();
	}

	private void Slide(UXSlider slider, float sliderValue)
	{
		float textOffset = (descriptionText.TextHeight - descriptionTextBoxHeight) * (sliderValue / slider.MaxValue);
		if (descriptionText.TextHeight < descriptionTextBoxHeight)
		{
			textOffset = 0f;
		}
		UpdateText(textOffset);
	}

	private void SlideEnd(UXSlider slider)
	{
		Slide(slider, slider.Value);
	}

	private void UpdateSliderSize()
	{
		float num = descriptionTextBoxHeight / descriptionText.TextHeight;
		float value = ((!(descriptionText.TextHeight < descriptionTextBoxHeight)) ? (num * descriptionTextBoxHeight) : descriptionTextBoxHeight);
		descriptionSlider.SetSliderSize(1f, Mathf.Clamp(value, 2f, descriptionTextBoxHeight));
		if (descriptionText.TextHeight < descriptionTextBoxHeight)
		{
			UpdateText(0f);
		}
	}

	private void UpdateText(float textOffset)
	{
		descriptionOffset.transform.localPosition = Vector3.up * textOffset;
	}
}
