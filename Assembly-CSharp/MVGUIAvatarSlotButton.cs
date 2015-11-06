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

	private CharacterEditorController CEController => MVGameControllerLegacyUI.CharacterEditorController;

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
			StartCoroutine(pTween.To(0.4f, 0f, 1f, (float t) =>
			{
				transform.localScale = Vector3.one * t;
			}));
			StartCoroutine(ShowPurchaseEffect());
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
		yield return new WaitForSeconds(UnlockSystem.duration);
		UnlockSystem.Stop();
		IconButton.SetVisible(visible: true);
		_viewPlane.SetVisible(visible: true);
	}

	public void UpdateAvatarPicture()
	{
		MVGUIAvatarPictureTaker component = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/AvatarPictureTaker")) as GameObject).GetComponent<MVGUIAvatarPictureTaker>();
		component.TakePicture(AvatarBodies[Index], OnAvatarPictureTaken);
	}

	private void OnAvatarPictureTaken(Texture2D avatarPicture)
	{
		if (!(this == null))
		{
			if (_avatarImageMaterial == null)
			{
				_avatarImageMaterial = new Material(ItemPreviewMaterial);
				_avatarImageMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
			else
			{
				UnityEngine.Object.Destroy(_avatarImageMaterial.mainTexture as Texture2D);
			}
			_avatarImageMaterial.mainTexture = avatarPicture;
			UXUtils.AddComponentIfNotExists<MeshRenderer>(_viewPlane.gameObject).material = _avatarImageMaterial;
			if (OnFinishedUpdating != null)
			{
				OnFinishedUpdating();
			}
		}
	}

	private void BuildViewPlane()
	{
		GameObject gameObject = new GameObject("View Plane");
		gameObject.layer = LayerMask.NameToLayer("UXElement");
		gameObject.transform.parent = transform;
		gameObject.transform.localScale = Vector3.one;
		gameObject.transform.localPosition = new Vector3(0f, 0f, -0.01f);
		_viewPlane = gameObject.AddComponent<UXPlane>();
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
