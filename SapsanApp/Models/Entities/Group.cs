using System;
using System.Collections.Generic;

namespace SapsanApp.Models.Entities
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TrainingProgramId { get; set; }
        public TrainingProgram TrainingProgram { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public string Level { get; set; } = string.Empty;
        public int? Capacity { get; set; } = 10;
        public int LanguageId { get; set; }
        public Language Language { get; set; }
        public string AgeGroup { get; set; } = string.Empty;
        public int CenterId { get; set; }
        public Center Center { get; set; }
        public DateTime Created { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

        public List<StudentGroup> StudentGroups { get; set; }
    }
}
