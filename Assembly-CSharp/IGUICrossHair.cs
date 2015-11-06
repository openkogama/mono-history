using UnityEngine;

public interface IGUICrossHair
{
	bool Visible { get; set; }

	Vector3 Direction { get; set; }

	Vector3 Origin { get; set; }

	bool FiredThisFrame { get; }

	void UpdateCrossHair(int ammo, Color color, float chargeState, bool firedThisFrame);
}
