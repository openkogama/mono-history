using MV.Common;

namespace Assets.Scripts.GamePasses;

public class TogglePreviewState
{
	public readonly GamePassTier previewTier;

	private readonly GamePassTier currentTier;

	private bool freeTry;

	public bool FreeTryWithoutAdAvailable
	{
		get
		{
			return freeTry;
		}
		set
		{
			freeTry = value;
		}
	}

	public bool IsOn => previewTier != GamePassTier.Tier0;

	public bool CanToggle
	{
		get
		{
			if (IsOn)
			{
				return true;
			}
			if (currentTier == GamePassTier.Tier3)
			{
				return false;
			}
			return true;
		}
	}

	public TogglePreviewState(GamePassTier previewTier, GamePassTier currentTier, bool freeFirstTry)
	{
		FreeTryWithoutAdAvailable = freeFirstTry;
		this.previewTier = previewTier;
		this.currentTier = currentTier;
	}
}
