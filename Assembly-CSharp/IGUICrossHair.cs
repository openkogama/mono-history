public interface IGUICrossHair
{
	bool Visible { get; set; }

	void UpdateCrossHair(PickupItem pickupItem);

	void HolsterStateChanged(bool isHolstered);

	void ShowHasHitEffect();
}
