using System.Collections.Generic;
using MV.Common;

namespace MV.WorldObject.GamePassSystem.GamePassEarnings;

public class ProjectEarningsReport
{
	public EarningsReport earningsReport = new EarningsReport();

	public Dictionary<int, ProjectMemberEarningsReport> projectMemberEarningsReports = new Dictionary<int, ProjectMemberEarningsReport>();

	public ProjectEarningsReport()
	{
	}

	public ProjectEarningsReport(EarningsReport earningsReport, Dictionary<int, ProjectMemberEarningsReport> projectMemberEarningsReports)
	{
		this.earningsReport = earningsReport;
		this.projectMemberEarningsReports = projectMemberEarningsReports;
	}

	public void AddGoldRevenue(int goldPerRegularMember, int goldPerSubscriberMember, GamePassTier gamePassTier, Dictionary<int, bool> projectMembers)
	{
		int num = 0;
		foreach (KeyValuePair<int, bool> projectMember in projectMembers)
		{
			if (!projectMemberEarningsReports.ContainsKey(projectMember.Key))
			{
				projectMemberEarningsReports.Add(projectMember.Key, new ProjectMemberEarningsReport());
			}
			int num2 = goldPerRegularMember;
			if (projectMember.Value)
			{
				num2 = goldPerSubscriberMember;
			}
			projectMemberEarningsReports[projectMember.Key].AddGoldRevenue(num2, gamePassTier);
			num += num2;
		}
		earningsReport.AddGoldRevenue(num, gamePassTier);
	}

	public override string ToString()
	{
		string text = earningsReport.ToString();
		foreach (KeyValuePair<int, ProjectMemberEarningsReport> projectMemberEarningsReport in projectMemberEarningsReports)
		{
			text += $"{projectMemberEarningsReport.Key}. {projectMemberEarningsReport.Value}.";
		}
		return text;
	}
}
