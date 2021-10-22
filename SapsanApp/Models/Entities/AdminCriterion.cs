using System;

namespace SapsanApp.Models.Entities
{
    public class AdminCriterion
    {
        public int Id { get; set; }
        public int CenterId { get; set; }
        public Center Center { get; set; }
        public int TrainingProgramId { get; set; }
        public TrainingProgram TrainingProgram { get; set; }
        public int MainCriterionId { get; set; }
        public MainCriterion MainCriterion { get; set; }
        public int SubCriterionId { get; set; }
        public SubCriterion SubCriterion { get; set; }
        public DateTime Created { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
    }
}