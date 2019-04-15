using System;
using MV.WorldObject.GamePassSystem.GamePassEarnings;

public static class GamePassesProjectEarningsManager
{
	public static Action<ProjectEarningsReport> OnEarningsDataUpdated;

	private static ProjectEarningsReport projectEarningReport;

	public static ProjectEarningsReport ProjectEarningReport => projectEarningReport;

	public static void UpdateProjectEarningReport(ProjectEarningsReport newProjectEarningReport)
	{
		projectEarningReport = newProjectEarningReport;
		if (OnEarningsDataUpdated != null)
		{
			OnEarningsDataUpdated(projectEarningReport);
		}
	}
}
