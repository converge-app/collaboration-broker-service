namespace Application.Models.DataTransferObjects
{
  public class ProjectCompleteDto
  {
    public string Id { get; set; }
    public string ProjectId { get; set; }
    public string FileUrl { get; set; }
    public string FreelancerId { get; set; }
    public string EmployerId { get; set; }
    public string State { get; set; }
  }
}