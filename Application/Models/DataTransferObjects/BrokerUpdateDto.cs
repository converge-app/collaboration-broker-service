using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.Models.DataTransferObjects
{
    public class BrokerUpdateDto
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string ProjectId { get; set; }

        [Required]
        public string FileUrl
        {
            get;
            set;
        }
    }
}