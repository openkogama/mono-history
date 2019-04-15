using MV.Common;

namespace MV.WorldObject.GamePassSystem.GamePassEarnings;

public class ProjectMemberEarningsReport
{
	public EarningsReport earningsReport = new EarningsReport();

	public ProjectMemberEarningsReport()
	{
	}

	public ProjectMemberEarningsReport(EarningsReport earningsReport)
	{
		this.earningsReport = earningsReport;
	}

	public void AddGoldRevenue(int goldAmount, GamePassTier gamePassTier)
	{
		earningsReport.AddGoldRevenue(goldAmount, gamePassTier);
	}

	public override string ToString()
	{
		return $"{earningsReport}. ";
	}
}
