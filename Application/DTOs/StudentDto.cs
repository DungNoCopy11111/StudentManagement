namespace StudentMgmt.Application.DTOs;

public class StudentDto
{
    public Guid Id { get; set; }
    public Guid ClassroomId { get; set; }
    public string StudentCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string Address { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
}