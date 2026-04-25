namespace Examination_System.Features.Admin.DTOs;

public class  AdminDashboardStatsDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsersToday { get; set; }
    public int TotalQuizzes { get; set; }
    public int TotalAttempts { get; set; }
    public double AvgPassRate { get; set; }
}
