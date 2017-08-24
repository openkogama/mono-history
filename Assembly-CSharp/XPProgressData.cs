using MV.Common;

public class XPProgressData
{
	private XPLevelLimits xpLevelLimits;

	private int playerCurrentXP;

	private XPRewardType xpId;

	private int xpDelta;

	public XPLevelLimits XPLevelLimits
	{
		set
		{
			xpLevelLimits = value;
		}
	}

	public int NextXP => xpLevelLimits.NextXP;

	public int PrevXP => xpLevelLimits.PrevXP;

	public XPRewardType XpID
	{
		get
		{
			return xpId;
		}
		set
		{
			xpId = value;
		}
	}

	public int XP
	{
		get
		{
			return playerCurrentXP;
		}
		set
		{
			playerCurrentXP = value;
		}
	}

	public string XPString => LocalizedEnums._(xpId);

	public int XPDelta
	{
		get
		{
			return xpDelta;
		}
		set
		{
			xpDelta = value;
		}
	}

	public int XpRel => xpLevelLimits.XpRel(playerCurrentXP);

	public int XpNextRel => xpLevelLimits.XPNextRel;

	public bool XPLimitExceeded => !xpLevelLimits.Validate(playerCurrentXP);

	public int Level => xpLevelLimits.Level;

	public XPProgressData(int playerCurrentXP, XPLevelLimits xpLevelLimits)
	{
		this.playerCurrentXP = playerCurrentXP;
		this.xpLevelLimits = xpLevelLimits;
	}
}
