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

	public bool isUnlocked = true;

	public void Register()
	{
		MVGameControllerBase.Game.MaterialRepository.AddMaterial(materialName, description, path, materialSound, modifierPackageType, priceGold, isUnlocked, new float[5] { friction, bouncyness, softness, staticFriction, toughness }, null);
	}

	public override string ToString()
	{
		return $"INSERT INTO `Material` (`Name`, `Description`, `Path`, `MaterialSound`, `AvatarModifierPackageType`, `MaterialAnimatorTypeName`, `UnlockPrice`, `Friction`, `Bouncyness`, `Softness`, `StaticFriction`, `Toughness`) VALUES ('{name}', '{description}', '{path}', '{(int)materialSound}', '{(int)modifierPackageType}', '', '{priceGold}', '{friction}', '{bouncyness}', '{softness}', '{staticFriction}', '{toughness}');\n";
	}
}
