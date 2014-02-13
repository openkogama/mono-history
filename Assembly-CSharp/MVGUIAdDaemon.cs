using Localize;
using UnityEngine;

public class MVGUIAdDaemon : MonoBehaviour
{
	private UXDialogFactory dialogFactory;

	public float InitialAdWaitTime = 60f;

	public float AdWaitTime = 1800f;

	private float lastAdShowTime;

	private float time;

	private void Start()
	{
		dialogFactory = UXUtils.FindGUIObjectOfType<UXDialogFactory>();
		lastAdShowTime = 0f - (AdWaitTime - InitialAdWaitTime);
		time = 0f;
	}

	private void Update()
	{
		if (time - lastAdShowTime > AdWaitTime)
		{
			if ((Object)(object)dialogFactory.CurrentDialogBox != (Object)null)
			{
				lastAdShowTime += 10f;
			}
			else
			{
				ShowAd();
				lastAdShowTime = time;
			}
		}
		time += Time.deltaTime;
	}

	private void ShowAd()
	{
		if (MVGameController.Instance.EditorController != null)
		{
			dialogFactory.CreateCustomDialog("Prefabs/GUI/Ads/ShopAdDialog", TextSlotIndex.Empty, noButtons: true, stackDialog: true);
			MVGUIShopAdDialog mVGUIShopAdDialog = (MVGUIShopAdDialog)dialogFactory.CurrentlyBuildingDialogBox;
			if (mVGUIShopAdDialog.CanShow())
			{
				dialogFactory.Show();
			}
			else
			{
				dialogFactory.DestroyBuildingDialog();
			}
		}
	}
}
