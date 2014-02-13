using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MVGUIAvatarSlotButton : MonoBehaviour
{
	public delegate void OnAvatarSlotPickDelegate(int index);

	public delegate void OnFinishedUpdatingDelegate();

	public OnAvatarSlotPickDelegate OnAvatarSlotPick;

	public OnFinishedUpdatingDelegate OnFinishedUpdating;

	public UXIconButton IconButton;

	public Material ItemPreviewMaterial;

	public ParticleSystem UnlockSystem;

	private UXPlane _viewPlane;

	private Material _avatarImageMaterial;

	private bool _scaleButton;

	private CharacterEditorController CEController => MVGameController.Instance.CharacterEditorController;

	private List<MVBody> AvatarBodies => AvatarSelectionAnimator.Instance.Bodies;

	public int Index { get; private set; }

	public void BuildAvatarSlotButton(int index, bool purchased)
	{
		Index = index;
		AvatarBodies[Index].AccessoriesChanged += MVBody_AccessoriesChanged;
		BuildViewPlane();
		if (purchased)
		{
			IconButton.SetVisible(visible: false);
			_viewPlane.SetVisible(visible: false);
			((MonoBehaviour)this).StartCoroutine(pTween.To(0.4f, 0f, 1f, (float t) =>
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				((Component)this).transform.localScale = Vector3.one * t;
			}));
			((MonoBehaviour)this).StartCoroutine(ShowPurchaseEffect());
		}
		UXIconButton iconButton = IconButton;
		iconButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(iconButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			if (OnAvatarSlotPick != null)
			{
				OnAvatarSlotPick(Index);
			}
		}));
	}

	private IEnumerator ShowPurchaseEffect()
	{
		UnlockSystem.Play();
		yield return (object)new WaitForSeconds(UnlockSystem.duration);
		UnlockSystem.Stop();
		IconButton.SetVisible(visible: true);
		_viewPlane.SetVisible(visible: true);
	}

	public void UpdateAvatarPicture()
	{
		Object val = Object.Instantiate(Resources.Load("Prefabs/GUI/AvatarPictureTaker"));
		MVGUIAvatarPictureTaker component = ((GameObject)((val is GameObject) ? val : null)).GetComponent<MVGUIAvatarPictureTaker>();
		component.TakePicture(AvatarBodies[Index], OnAvatarPictureTaken);
	}

	private void OnAvatarPictureTaken(Texture2D avatarPicture)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected Obj, but got Unknown
		if (!((Object)(object)this == (Object)null))
		{
			if ((Object)(object)_avatarImageMaterial == (Object)null)
			{
				_avatarImageMaterial = new Material(ItemPreviewMaterial);
				((Object)_avatarImageMaterial).hideFlags = (HideFlags)13;
			}
			else
			{
				Texture mainTexture = _avatarImageMaterial.mainTexture;
				Object.Destroy((Object)(object)((mainTexture is Texture2D) ? mainTexture : null));
			}
			_avatarImageMaterial.mainTexture = (Texture)(object)avatarPicture;
			((Renderer)UXUtils.AddComponentIfNotExists<MeshRenderer>(((Component)_viewPlane).gameObject)).material = _avatarImageMaterial;
			if (OnFinishedUpdating != null)
			{
				OnFinishedUpdating();
			}
		}
	}

	private void BuildViewPlane()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected Obj, but got Unknown
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("View Plane");
		val.layer = LayerMask.NameToLayer("UXElement");
		val.transform.parent = ((Component)this).transform;
		val.transform.localScale = Vector3.one;
		val.transform.localPosition = new Vector3(0f, 0f, -0.01f);
		_viewPlane = val.AddComponent<UXPlane>();
		_viewPlane.SetSize(IconButton.Width, IconButton.Height);
	}

	public void SetVisible(bool visible)
	{
		IconButton.SetVisible(visible);
		_viewPlane.SetVisible(visible);
	}

	private void MVBody_AccessoriesChanged(object sender, EventArgs e)
	{
		MVBody mVBody = sender as MVBody;
		if (mVBody.AccessoriesLoaded)
		{
			UpdateAvatarPicture();
		}
	}
}
