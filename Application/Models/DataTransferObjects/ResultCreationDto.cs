using System.ComponentModel.DataAnnotations;

namespace Application.Models.DataTransferObjects
{
    public class ResultCreationDto
    {
        [Required]
        public string ProjectId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string FileUrl { get; set; }
    }

    public class PayCreationDto
    {
        [Required]
        public string ProjectId { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}