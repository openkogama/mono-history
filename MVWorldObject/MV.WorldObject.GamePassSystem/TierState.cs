namespace MV.WorldObject.GamePassSystem;

public class TierState
{
	public readonly TierLockState tierLockState;

	public readonly int remainingGamePointRequired;

	public readonly int remainingGoldPriceRequired;

	public readonly int goldPriceRequirementBase;

	public readonly int gamePointRequirementBase;

	public TierState(TierLockState tierLockState, int remainingGamePointRequired, int gamePointRequirementBase, int remainingGoldPriceRequired, int goldPriceRequirementBase)
	{
		this.tierLockState = tierLockState;
		this.remainingGamePointRequired = remainingGamePointRequired;
		this.remainingGoldPriceRequired = remainingGoldPriceRequired;
		this.gamePointRequirementBase = gamePointRequirementBase;
		this.goldPriceRequirementBase = goldPriceRequirementBase;
	}

	public override string ToString()
	{
		return $"tierLockState {tierLockState}. remainingGamePointRequired {remainingGamePointRequired}. gamePointRequirementBase {gamePointRequirementBase}. remainingGoldPriceRequired {remainingGoldPriceRequired}. goldPriceRequirementBase {goldPriceRequirementBase}.";
	}
}
