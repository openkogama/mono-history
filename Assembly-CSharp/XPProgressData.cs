public class XPProgressData
{
	private XPLevelLimits xpLevelLimits;

	private int xp;

	private byte xpId;

	public XPLevelLimits XPLevelLimits
	{
		set
		{
			xpLevelLimits = value;
		}
	}

	public byte XpID
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
			return xp;
		}
		set
		{
			xp = value;
		}
	}

	public string XPString => XPManager.GetXPText(xpId);

	public int XPDelta => XPManager.GetXPAmount(xpId);

	public int XpRel => xpLevelLimits.XpRel(xp);

	public int XpNextRel => xpLevelLimits.XPNextRel;

	public bool XPLimitExceeded => !xpLevelLimits.Validate(xp);

	public int Level => xpLevelLimits.Level;

	public XPProgressData(int xp, XPLevelLimits xpLevelLimits)
	{
		this.xp = xp;
		this.xpLevelLimits = xpLevelLimits;
	}
}
