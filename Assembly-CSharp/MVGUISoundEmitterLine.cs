using System;
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
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected Obj, but got Unknown
		AssetInfo = assetInfo;
		lineBG.SetSize(Width, Height);
		lineBG.SetColor(Color.black, string.Empty);
		((Renderer)((Component)unlockStatus).GetComponent<MeshRenderer>()).material = new Material((!unlocked) ? lockedMaterial : unlockedMaterial);
		nameText.Text = assetInfo.Name;
		InitializeClickListeners();
		UXToggleIconButton uXToggleIconButton = playButton;
		uXToggleIconButton.OnToggle = (UXToggleIconButton.OnToggleDelegate)Delegate.Combine(uXToggleIconButton.OnToggle, new UXToggleIconButton.OnToggleDelegate(OnPlayClick));
		((Component)loadingCircle).gameObject.SetActiveRecursively(false);
	}

	public void SetSelected(bool selected)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).gameObject.AddComponent<BoxCollider>();
		BoxCollider component = ((Component)this).gameObject.GetComponent<BoxCollider>();
		component.size = new Vector3(Width - 5f, Height, 1f);
		UXMouseClickObject uXMouseClickObject = ((Component)this).gameObject.AddComponent<UXMouseClickObject>();
		uXMouseClickObject.OnMouseDown = (UXMouseClickObject.OnMouseDownDelegate)Delegate.Combine(uXMouseClickObject.OnMouseDown, (UXMouseClickObject.OnMouseDownDelegate)((UXMouseClickObject clickObject, Vector3 mousePos) =>
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			Rect clippedBounds = GetClippedBounds();
			if (clippedBounds.Contains(mousePos))
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
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		base.Update();
		UpdateMouseOver();
		mouseOver = false;
		if (Visible && loading)
		{
			((Component)loadingCircle).transform.Rotate(Vector3.forward, 500f * Time.deltaTime);
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
			((Component)playButton).gameObject.SetActiveRecursively(false);
			((Component)loadingCircle).gameObject.SetActiveRecursively(true);
			loading = true;
			MVGameController.Instance.Game.AssetBundleMgr.RequestAssetBundle(AssetInfo.RequestPath, OnDoneLoading, autoRetry: false, highPriority: true);
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

	private void OnDoneLoading(AssetBundle loadedBundle, string assetBundleUrl)
	{
		MVGameController.Instance.Game.AssetBundleMgr.UnsubscribeBundleCallback(assetBundleUrl, OnDoneLoading);
		((Component)loadingCircle).gameObject.SetActiveRecursively(false);
		((Component)playButton).gameObject.SetActiveRecursively(((Component)this).gameObject.active);
		if (OnPlayNodePreview != null && PlayOnLoad)
		{
			OnPlayNodePreview(this);
		}
	}
}
