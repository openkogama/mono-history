using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Localize;
using MV.Common;
using UnityEngine;

public class MVGUISoundEmitterSettingsBox : UXCustomDialogBox
{
	public UXTextButton okButton;

	public UXTextButton cancelButton;

	public UXToggleIconButton muteToggle;

	public UXSlider pitchSlider;

	public UXSlider rangeSlider;

	public UXSlider volumeSlider;

	public UXText selectedSoundText;

	public UXComboBox categoryComboBox;

	public UXScrollableBox soundBitesScrollableBox;

	private Dictionary<AmbientAudioCategory, List<SoundBite>> categoryToAssets;

	private bool _preview;

	private int selectedSoundID;

	private string selectedSoundUrl;

	private string selectedSoundName;

	private bool soundDoesntExist;

	private StreamingAssetInfo _selectedAssetInfo;

	private MVGUISoundEmitterLine _selectedLine;

	private MVGUISoundEmitterLine _previewLine;

	private MVGUISoundEmitterLine _loadingLine;

	private float volume;

	private float range;

	private float pitch;

	private bool mute;

	private bool isInitialized;

	private MVNetworkGame Game => MVGameController.Instance.Game;

	private bool Preview
	{
		get
		{
			return _preview;
		}
		set
		{
			_preview = value;
			if ((Object)(object)PreviewLine != (Object)null && !_preview)
			{
				PreviewLine.playButton.SetToggleState(toggle: false);
				PreviewLine.PlayOnLoad = false;
			}
			if ((Object)(object)_loadingLine != (Object)null)
			{
				_loadingLine.PlayOnLoad = false;
			}
		}
	}

	private Hashtable selectedSoundData
	{
		get
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("id", selectedSoundID);
			hashtable.Add("url", selectedSoundUrl);
			hashtable.Add("name", selectedSoundName);
			return hashtable;
		}
	}

	private MVGUISoundEmitterLine SelectedLine
	{
		get
		{
			return _selectedLine;
		}
		set
		{
			if ((Object)(object)_selectedLine != (Object)null)
			{
				_selectedLine.SetSelected(selected: false);
			}
			_selectedLine = value;
			_selectedLine.SetSelected(selected: true);
			_selectedAssetInfo = _selectedLine.AssetInfo;
			selectedSoundID = _selectedAssetInfo.ProductID;
			selectedSoundUrl = _selectedAssetInfo.AssetPath;
			selectedSoundName = _selectedAssetInfo.Name;
			selectedSoundText.Text = _selectedLine.AssetInfo.Name;
		}
	}

	private MVGUISoundEmitterLine PreviewLine
	{
		get
		{
			return _previewLine;
		}
		set
		{
			if ((Object)(object)_previewLine != (Object)null)
			{
				_previewLine.PlayOnLoad = false;
			}
			_previewLine = value;
		}
	}

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		if (!isInitialized)
		{
			UXTextButton uXTextButton = okButton;
			uXTextButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
			{
				Preview = false;
				OnPositiveClose();
				DialogFactory.CloseDialog();
			}));
			UXTextButton uXTextButton2 = cancelButton;
			uXTextButton2.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton2.OnClick, (UXBaseButton.OnClickDelegate)(() =>
			{
				Preview = false;
				OnNegativeClose();
				DialogFactory.CloseDialog();
			}));
			UXComboBox uXComboBox = categoryComboBox;
			uXComboBox.OnComboBoxItemSelect = (UXComboBox.OnComboBoxItemSelectDelegate)Delegate.Combine(uXComboBox.OnComboBoxItemSelect, new UXComboBox.OnComboBoxItemSelectDelegate(UpdateSoundBites));
			UXComboBox uXComboBox2 = categoryComboBox;
			uXComboBox2.OnComboBoxOpen = (UXComboBox.OnComboBoxActionDelegate)Delegate.Combine(uXComboBox2.OnComboBoxOpen, (UXComboBox.OnComboBoxActionDelegate)(() =>
			{
				soundBitesScrollableBox.SetSliderEnabled(enabled: false);
			}));
			UXComboBox uXComboBox3 = categoryComboBox;
			uXComboBox3.OnComboBoxClose = (UXComboBox.OnComboBoxActionDelegate)Delegate.Combine(uXComboBox3.OnComboBoxClose, (UXComboBox.OnComboBoxActionDelegate)(() =>
			{
				soundBitesScrollableBox.SetSliderEnabled(enabled: true);
			}));
			InitializeListeners();
			isInitialized = true;
		}
		else if (Preview)
		{
			categoryComboBox.SetCurrentItem(PreviewLine.AssetInfo.CategoryID - 1);
			soundBitesScrollableBox.FocusOnLine(PreviewLine);
		}
		else
		{
			SetCurrentSound(selectedSoundData);
		}
		categoryComboBox.Close();
		volume = volumeSlider.Value;
		range = rangeSlider.Value;
		pitch = pitchSlider.Value;
		mute = muteToggle.ToggleState;
	}

	public override void OnCloseDialog()
	{
		base.OnCloseDialog();
		Preview = false;
	}

	private void InitializeListeners()
	{
		UXToggleIconButton uXToggleIconButton = muteToggle;
		uXToggleIconButton.OnToggle = (UXToggleIconButton.OnToggleDelegate)Delegate.Combine(uXToggleIconButton.OnToggle, (UXToggleIconButton.OnToggleDelegate)((bool toggle) =>
		{
			mute = toggle;
			FireIntermediateResult();
		}));
		UXSlider uXSlider = volumeSlider;
		uXSlider.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider slider, float value) =>
		{
			volume = value;
			FireIntermediateResult();
		}));
		UXSlider uXSlider2 = rangeSlider;
		uXSlider2.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider2.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider slider, float value) =>
		{
			range = value;
			FireIntermediateResult();
		}));
		UXSlider uXSlider3 = pitchSlider;
		uXSlider3.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider3.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider slider, float value) =>
		{
			pitch = value;
			FireIntermediateResult();
		}));
		UXSlider uXSlider4 = volumeSlider;
		uXSlider4.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider4.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider slider) =>
		{
			volume = slider.Value;
		}));
		UXSlider uXSlider5 = rangeSlider;
		uXSlider5.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider5.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider slider) =>
		{
			range = slider.Value;
		}));
		UXSlider uXSlider6 = pitchSlider;
		uXSlider6.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider6.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider slider) =>
		{
			pitch = slider.Value;
		}));
	}

	public void SetCurrentSound(Hashtable data)
	{
		int soundID = (selectedSoundID = (int)data["id"]);
		selectedSoundUrl = (string)data["url"];
		selectedSoundName = (string)data["name"];
		BuildSoundBiteCategories();
		int num = FindCategoryID(soundID);
		bool flag = false;
		if (num == -1)
		{
			num = categoryComboBox.Items.Count;
			if (!soundDoesntExist)
			{
				flag = true;
				categoryComboBox.Add("Not Available");
				num = categoryComboBox.Items.Count;
				soundDoesntExist = true;
			}
			selectedSoundText.Text = selectedSoundName;
		}
		categoryComboBox.SetCurrentItem(num - 1);
		soundBitesScrollableBox.FocusOnLine(SelectedLine);
		if (flag)
		{
			UXUtils.FindGUIObjectOfType<UXDialogFactory>().CreateDialog(TextSlotIndex.SoundEmitterWarning, TextSlotIndex.Notice, UXDialogType.Simple, noButtons: false, stackDialog: true).Show();
		}
	}

	private int FindCategoryID(int soundID)
	{
		int num = -1;
		if (num == -1)
		{
			num = Game.StreamingAssetShopInventory.GetCategoryIDFromAssetID(soundID);
		}
		return num;
	}

	private void BuildSoundBiteCategories()
	{
		categoryToAssets = new Dictionary<AmbientAudioCategory, List<SoundBite>>();
		foreach (ProductInventoryInfo<StreamingAssetInfo> item in Game.StreamingAssetInventory.Get(StreamingAssetType.AmbientAudio))
		{
			AmbientAudioCategory categoryID = (AmbientAudioCategory)item.ProductInfo.CategoryID;
			if (!categoryToAssets.ContainsKey(categoryID))
			{
				categoryToAssets.Add(categoryID, new List<SoundBite>());
			}
			categoryToAssets[categoryID].Add(new SoundBite
			{
				assetInfo = item.ProductInfo,
				unlocked = true
			});
		}
		StreamingAssetInfo assetInfo;
		foreach (StreamingAssetInfo item2 in Game.StreamingAssetShopInventory.Get(StreamingAssetType.AmbientAudio))
		{
			assetInfo = item2;
			AmbientAudioCategory categoryID2 = (AmbientAudioCategory)assetInfo.CategoryID;
			if (!categoryToAssets.ContainsKey(categoryID2))
			{
				categoryToAssets.Add(categoryID2, new List<SoundBite>());
			}
			if (!categoryToAssets[categoryID2].Any((SoundBite sb) => sb.assetInfo.ProductID == assetInfo.ProductID))
			{
				categoryToAssets[categoryID2].Add(new SoundBite
				{
					assetInfo = assetInfo
				});
			}
		}
	}

	private void UpdateSoundBites(int comboboxItemIndex)
	{
		string category = (string)categoryComboBox.Items[comboboxItemIndex].GetValue();
		if (Preview)
		{
			Preview = false;
			FireIntermediateResult();
		}
		soundBitesScrollableBox.RemoveAllLines();
		CreateSoundBiteLines(category);
		soundBitesScrollableBox.SetSliderValue(0f);
	}

	private void UpdateSelectedSoundBite(MVGUISoundEmitterLine soundEmitterLine)
	{
		Preview = false;
		if (GetSoundBiteFromName(soundEmitterLine.AssetInfo.Name).unlocked)
		{
			SelectedLine = soundEmitterLine;
		}
		else
		{
			PreviewLine = soundEmitterLine;
			Preview = true;
			UXDialogFactory uXDialogFactory = UXUtils.FindGUIObjectOfType<UXDialogFactory>();
			uXDialogFactory.CreateCustomDialog("Prefabs/GUI/Dialogs/ProductShopDialog", TextSlotIndex.Empty, noButtons: true, stackDialog: true).SetOnResultCallback(OnShopDialogResult).SetValues(BuildDialogData())
				.Show();
			MVGUIProductShopDialog mVGUIProductShopDialog = (MVGUIProductShopDialog)uXDialogFactory.CurrentDialogBox;
			mVGUIProductShopDialog.SetPrice(PreviewLine.AssetInfo.ShopInfo.PriceGold, PreviewLine.AssetInfo.ShopInfo.PriceSilver);
			mVGUIProductShopDialog.OnTryPurchaseProduct = () =>
			{
				MVGameController.Instance.Game.PurchaseStreamingAsset(StreamingAssetType.AmbientAudio, PreviewLine.AssetInfo.ProductID);
			};
		}
		FireIntermediateResult();
	}

	private Dictionary<string, DialogData> BuildDialogData()
	{
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		dictionary.Add("ProductName", new TextData
		{
			text = PreviewLine.AssetInfo.Name
		});
		dictionary.Add("ProductDescription", new TextData
		{
			text = PreviewLine.AssetInfo.Desc,
			useWordWrap = true
		});
		Object val = Object.Instantiate(Resources.Load("Prefabs/GUI/ShopPreview/SoundEmitterPreview"));
		GameObject productPreview = (GameObject)(object)((val is GameObject) ? val : null);
		dictionary.Add("ProductPreview", new ProductPreviewData
		{
			productPreview = productPreview
		});
		return dictionary;
	}

	private void OnShopDialogResult(UXDialogBox dialogBox)
	{
		Preview = false;
		if (dialogBox.DialogResult == UXDialogResult.Positive)
		{
			SelectedLine = PreviewLine;
			Hashtable hashtable = (Hashtable)dialogBox.GetResult();
			int inventoryID = (int)hashtable[(byte)74];
			ProductInventoryInfo<StreamingAssetInfo> invInfo = new ProductInventoryInfo<StreamingAssetInfo>(purchaseTime: new DateTime((long)hashtable[(byte)84]), inventoryID: inventoryID, productInfo: SelectedLine.AssetInfo);
			Game.StreamingAssetInventory.Add(invInfo);
			SetSoundBiteToUnlocked(SelectedLine.AssetInfo.CategoryID - 1, SelectedLine.AssetInfo.Name);
			categoryComboBox.SetCurrentItem(SelectedLine.AssetInfo.CategoryID - 1);
		}
		FireIntermediateResult();
	}

	private void LoadPreview(MVGUISoundEmitterLine soundEmitterLine)
	{
		if ((Object)(object)_loadingLine != (Object)null)
		{
			_loadingLine.PlayOnLoad = false;
		}
		_loadingLine = soundEmitterLine;
		_loadingLine.PlayOnLoad = true;
	}

	private void PlayPreview(MVGUISoundEmitterLine soundEmitterLine)
	{
		Preview = false;
		PreviewLine = soundEmitterLine;
		PreviewLine.playButton.SetToggleState(toggle: true);
		Preview = true;
		FireIntermediateResult();
	}

	private void StopPreview(MVGUISoundEmitterLine soundEmitterLine)
	{
		Preview = false;
		FireIntermediateResult();
	}

	private void CreateSoundBiteLines(string category)
	{
		if (!Enumerable.Contains(Enum.GetNames(typeof(AmbientAudioCategory)), category))
		{
			return;
		}
		AmbientAudioCategory key = (AmbientAudioCategory)(int)Enum.Parse(typeof(AmbientAudioCategory), category);
		if (!categoryToAssets.ContainsKey(key))
		{
			return;
		}
		List<SoundBite> list = categoryToAssets[key];
		foreach (SoundBite item in list)
		{
			Object val = Object.Instantiate(Resources.Load("Prefabs/GUI/Lines/SoundEmitterLine"));
			MVGUISoundEmitterLine component = ((GameObject)((val is GameObject) ? val : null)).GetComponent<MVGUISoundEmitterLine>();
			component.BuildLine(item.assetInfo, item.unlocked);
			component.OnLineClick = (MVGUISoundEmitterLine.OnLineActionDelegate)Delegate.Combine(component.OnLineClick, new MVGUISoundEmitterLine.OnLineActionDelegate(UpdateSelectedSoundBite));
			component.OnNodePreviewClick = (MVGUISoundEmitterLine.OnLineActionDelegate)Delegate.Combine(component.OnNodePreviewClick, new MVGUISoundEmitterLine.OnLineActionDelegate(LoadPreview));
			component.OnPlayNodePreview = (MVGUISoundEmitterLine.OnLineActionDelegate)Delegate.Combine(component.OnPlayNodePreview, new MVGUISoundEmitterLine.OnLineActionDelegate(PlayPreview));
			component.OnStopNodePreview = (MVGUISoundEmitterLine.OnLineActionDelegate)Delegate.Combine(component.OnStopNodePreview, new MVGUISoundEmitterLine.OnLineActionDelegate(StopPreview));
			soundBitesScrollableBox.AddLine(component);
			if (item.assetInfo.ProductID == selectedSoundID)
			{
				SelectedLine = component;
			}
		}
	}

	private SoundBite GetSoundBiteFromName(string soundName)
	{
		string value = (string)categoryComboBox.CurrentlySelectedItem.GetValue();
		AmbientAudioCategory key = (AmbientAudioCategory)(int)Enum.Parse(typeof(AmbientAudioCategory), value);
		foreach (SoundBite item in categoryToAssets[key])
		{
			if (item.assetInfo.Name == soundName)
			{
				return item;
			}
		}
		return default;
	}

	private void SetSoundBiteToUnlocked(int categoryIndex, string soundName)
	{
		string text = (string)categoryComboBox.Items[categoryIndex].GetValue();
		Debug.Log((object)(text + " - " + soundName));
		AmbientAudioCategory key = (AmbientAudioCategory)(int)Enum.Parse(typeof(AmbientAudioCategory), text);
		List<SoundBite> list = categoryToAssets[key];
		for (int i = 0; i < list.Count; i++)
		{
			SoundBite value = list[i];
			if (value.assetInfo.Name == soundName)
			{
				value.unlocked = true;
				list[i] = value;
				break;
			}
		}
	}

	public override object GetResult()
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("volume", volume);
		hashtable.Add("pitch", pitch);
		hashtable.Add("range", Mathf.RoundToInt(range));
		hashtable.Add("mute", mute);
		hashtable.Add("loop", true);
		Hashtable hashtable2 = hashtable;
		if (Preview)
		{
			hashtable2.Add("name", PreviewLine.AssetInfo.Name);
			hashtable2.Add("url", PreviewLine.AssetInfo.AssetPath);
			hashtable2.Add("id", PreviewLine.AssetInfo.ProductID);
		}
		else
		{
			hashtable2.Add("name", selectedSoundName);
			hashtable2.Add("url", selectedSoundUrl);
			hashtable2.Add("id", selectedSoundID);
		}
		return hashtable2;
	}
}
