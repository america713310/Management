using System;

namespace SapsanApp.Models.Entities
{
    public class CashboxProgram
    {
        public int Id { get; set; }
        public Guid StudentId { get; set; }
        public Student Student { get; set; }
        public int CenterId { get; set; }
        public Center Center { get; set; }
        public string PaymentType { get; set; } = string.Empty;
        public int? TrainingProgramId { get; set; }
        public TrainingProgram TrainingProgram { get; set; }
        public int? TrainingMaterialId { get; set; }
        public TrainingMaterial TrainingMaterial { get; set; }
        public double Balance { get; set; } = 0;
        public double? PaymentSum { get; set; } = 0;
        public DateTime Created { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
    }
}