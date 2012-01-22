using System;
using UnityEngine;

[AddComponentMenu("UX/Audio/Collection View")]
public class UXCollectionViewAudio : MonoBehaviour
{
	private AudioBankSound menuNextSound;

	private AudioBankSound menuPrevSound;

	private AudioBankSound selectSound;

	private void Awake()
	{
		menuNextSound = GUIAudioBank.Instance.GetSound("menu_next");
		menuPrevSound = GUIAudioBank.Instance.GetSound("menu_prev");
		selectSound = GUIAudioBank.Instance.GetSound("menu_click");
		UXCollectionView component = ((Component)this).GetComponent<UXCollectionView>();
		component.OnItemSelection = (UXCollectionView.OnItemSelectionDelegate)Delegate.Combine(component.OnItemSelection, new UXCollectionView.OnItemSelectionDelegate(HandleItemSelection));
		component.OnNextPage = (UXCollectionView.OnNextPageDelegate)Delegate.Combine(component.OnNextPage, new UXCollectionView.OnNextPageDelegate(HandleNextPage));
		component.OnPreviousPage = (UXCollectionView.OnPreviousPageDelegate)Delegate.Combine(component.OnPreviousPage, new UXCollectionView.OnPreviousPageDelegate(HandlePreviousPage));
	}

	private void HandleItemSelection(IUXCollectionItem item)
	{
		selectSound.Play();
	}

	private void HandleNextPage()
	{
		menuNextSound.Play();
	}

	private void HandlePreviousPage()
	{
		menuPrevSound.Play();
	}
}
