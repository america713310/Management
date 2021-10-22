using SapsanApp.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace SapsanApp.Models.Entities
{
    public class Lead
    {
        [Key]
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int? CenterId { get; set; }
        public Center Center { get; set; }
        public int? TrainingProgramId { get; set; }
        public TrainingProgram TrainingProgram { get; set; }
        public string ChildFirstName { get; set; } = string.Empty;
        public string ChildLastName { get; set; } = string.Empty;
        public string ChildAgeGroup { get; set; } = string.Empty;
        public int? SourceId { get; set; }
        public Source Source { get; set; }
        public LeadStatusEnum Status { get; set; } = LeadStatusEnum.NewLead;
        public string Comment { get; set; } = string.Empty;
        public string RefusalReason { get; set; } = string.Empty;
        public DateTime Created { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
    }
}
