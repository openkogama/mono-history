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

	public void AddTierGoldRevenue(int goldAmount, GamePassTier gamePassTier)
	{
		earningsReport.AddTierGoldRevenue(goldAmount, gamePassTier);
	}

	public void AddGameBoosterGoldRevenue(int goldAmount, string gameBooster)
	{
		earningsReport.AddGameBoosterGoldRevenue(goldAmount, gameBooster);
	}

	public override string ToString()
	{
		return $"{earningsReport}. ";
	}
}
