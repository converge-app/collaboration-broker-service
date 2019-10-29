using System.ComponentModel.DataAnnotations;

namespace Application.Models.DataTransferObjects
{
  public class CompleteProjectDto
  {
    [Required]
    public string ProjectId { get; set; }
  }
}