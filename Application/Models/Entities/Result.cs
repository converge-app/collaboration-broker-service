using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Application.Models.Entities
{
    public class Result
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string ProjectId { get; set; }
        public string FileUrl { get; set; }
        public string FreelancerId { get; set; }
        public string EmployerId { get; set; }
        public string State { get; set; }
    }
}