using System;
using MV.Common;
using UnityEngine;

public class MVGUISoundEmitterLine : UXLine
{
	public delegate void OnLineActionDelegate(MVGUISoundEmitterLine soundEmitterLine);

	public OnLineActionDelegate OnLineClick;

	public OnLineActionDelegate OnNodePreviewClick;

	public OnLineActionDelegate OnPlayNodePreview;

	public OnLineActionDelegate OnStopNodePreview;

	public UXPlane lineBG;

	public UXPlane unlockStatus;

	public UXText nameText;

	public UXToggleIconButton playButton;

	public UXPlane loadingCircle;

	public Material lockedMaterial;

	public Material unlockedMaterial;

	public Color MouseOverColor;

	public Color SelectedColor;

	public Color SelectedMouseOverColor;

	private bool mouseOver;

	private bool mouseDown;

	private bool loading;

	[HideInInspector]
	public bool PlayOnLoad;

	private bool selected;

	public StreamingAssetInfo AssetInfo { get; private set; }

	public void BuildLine(StreamingAssetInfo assetInfo, bool unlocked)
	{
		AssetInfo = assetInfo;
		lineBG.SetSize(Width, Height);
		lineBG.SetColor(Color.black, string.Empty);
		unlockStatus.GetComponent<MeshRenderer>().material = new Material((!unlocked) ? lockedMaterial : unlockedMaterial);
		nameText.Text = assetInfo.Name;
		InitializeClickListeners();
		UXToggleIconButton uXToggleIconButton = playButton;
		uXToggleIconButton.OnToggle = (UXToggleIconButton.OnToggleDelegate)Delegate.Combine(uXToggleIconButton.OnToggle, new UXToggleIconButton.OnToggleDelegate(OnPlayClick));
		loadingCircle.gameObject.SetActive(value: false);
	}

	public void SetSelected(bool selected)
	{
		this.selected = selected;
		if (selected)
		{
			lineBG.SetColor(SelectedColor, string.Empty);
		}
		else
		{
			lineBG.Visible = false;
		}
	}

	private void UpdateMouseOver()
	{
		if (mouseOver)
		{
			lineBG.Visible = true;
			if (selected)
			{
				lineBG.SetColor(SelectedMouseOverColor, string.Empty);
			}
			else
			{
				lineBG.SetColor(MouseOverColor, string.Empty);
			}
		}
		else
		{
			SetSelected(selected);
		}
	}

	private void InitializeClickListeners()
	{
		gameObject.AddComponent<BoxCollider>();
		BoxCollider component = gameObject.GetComponent<BoxCollider>();
		component.size = new Vector3(Width - 5f, Height, 1f);
		UXMouseClickObject uXMouseClickObject = gameObject.AddComponent<UXMouseClickObject>();
		uXMouseClickObject.OnMouseDown = (UXMouseClickObject.OnMouseDownDelegate)Delegate.Combine(uXMouseClickObject.OnMouseDown, (UXMouseClickObject.OnMouseDownDelegate)((UXMouseClickObject clickObject, Vector3 mousePos) =>
		{
			if (GetClippedBounds().Contains(mousePos))
			{
				mouseDown = true;
			}
			return false;
		}));
		uXMouseClickObject.OnMouseUp = (UXMouseClickObject.OnMouseUpDelegate)Delegate.Combine(uXMouseClickObject.OnMouseUp, (UXMouseClickObject.OnMouseUpDelegate)((UXMouseClickObject clickObject, Vector3 mousePos) =>
		{
			if (Visible && mouseDown && OnLineClick != null)
			{
				OnLineClick(this);
			}
			mouseDown = false;
		}));
	}

	public void OnMouseOver()
	{
		mouseOver = true;
	}

	public override void Update()
	{
		base.Update();
		UpdateMouseOver();
		mouseOver = false;
		if (Visible && loading)
		{
			loadingCircle.transform.Rotate(Vector3.forward, 500f * Time.deltaTime);
		}
	}

	private void OnPlayClick(bool play)
	{
		if (play)
		{
			if (OnNodePreviewClick != null)
			{
				OnNodePreviewClick(this);
			}
			playButton.SetToggleState(toggle: false);
			playButton.gameObject.SetActive(value: false);
			loadingCircle.gameObject.SetActive(value: true);
			loading = true;
			AsyncWWWManager.WWWRequest(new StreamingAssetRequest(Urls.StreamingAssets + AssetInfo.RequestPath, OnDoneLoading));
		}
		else
		{
			if (OnStopNodePreview != null)
			{
				OnStopNodePreview(this);
			}
			PlayOnLoad = false;
		}
	}

	private void OnDoneLoading(WWW loadedBundle)
	{
		Debug.Log("Done loading");
		Debug.Log(loadedBundle.assetBundle);
		loadingCircle.gameObject.SetActive(value: false);
		playButton.gameObject.SetActive(gameObject.activeInHierarchy);
		if (OnPlayNodePreview != null && PlayOnLoad)
		{
			OnPlayNodePreview(this);
		}
	}
}
