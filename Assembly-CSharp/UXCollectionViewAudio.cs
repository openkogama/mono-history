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
		component.OnItemSelection = (UXCollectionView.OnBasicItemEventDelegate)Delegate.Combine(component.OnItemSelection, new UXCollectionView.OnBasicItemEventDelegate(HandleItemSelection));
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
