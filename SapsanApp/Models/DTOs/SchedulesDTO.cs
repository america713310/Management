using System;

namespace SapsanApp.Models.DTOs
{
    public class SchedulesDTO
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int CenterId { get; set; }
        public int? TrainingProgramId { get; set; }
        public Guid? TrainerId { get; set; }
        public int? GroupId { get; set; }
        public DateTime? Date { get; set; }
    }
}