using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.Models.DataTransferObjects
{
    public class ResultDto
    {
        public string Id { get; set; }

        public string ProjectId { get; set; }
        public string FileUrl { get; set; }
    }
}