namespace StudentMgmt.Application.DTOs;

public class DashboardDto
{
    public List<StatisticItem> StudentsByClass { get; set; } = new();
    public List<StatisticItem> StudentsByStatus { get; set; } = new();
    public int TotalStudents { get; set; }
    public int TotalClasses { get; set; }
}
public class StatisticItem
{
    public string Label { get; set; } = default!;
    public double Value { get; set; }
}