using UnityEngine;

public class OverrideMaterial : MonoBehaviour
{
	public float friction = 0.43f;

	public float bouncyness;

	public float softness = 1f;

	public float staticFriction = 20f;

	public float toughness;

	public string materialName = "Light Red";

	public string description = "A basic building material.\nHint: Try using different colors!";

	public string path = "Cube/Materials/scarletred00";

	public MaterialSound materialSound;

	public AvatarModifierPackageType modifierPackageType;

	public int priceGold;

	public int priceSilver;

	public bool isUnlocked = true;

	public void Register()
	{
		MVGameController.Game.MaterialRepository.AddMaterial(materialName, description, path, materialSound, modifierPackageType, priceGold, priceSilver, isUnlocked, new float[5] { friction, bouncyness, softness, staticFriction, toughness });
	}

	public override string ToString()
	{
		return $"INSERT INTO `Material` (`Name`, `Description`, `Path`, `MaterialSound`, `AvatarModifierPackageType`, `MaterialAnimatorTypeName`, `UnlockPrice`, `UnlockPriceSilver`, `Friction`, `Bouncyness`, `Softness`, `StaticFriction`, `Toughness`) VALUES ('{name}', '{description}', '{path}', '{(int)materialSound}', '{(int)modifierPackageType}', '', '{priceGold}', '{priceSilver}', '{friction}', '{bouncyness}', '{softness}', '{staticFriction}', '{toughness}');\n";
	}
}
