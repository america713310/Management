using System;
using System.Collections.Generic;

namespace SapsanApp.Models.Entities
{
    public class TrainingProgram
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
        public int TariffId { get; set; }
        public Tariff Tariff { get; set; }
        public DateTime? FirstLesson { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        public string Comment { get; set; } = string.Empty;
        public DateTime Created { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

        public List<Lead> Leads { get; set; }
        public List<Group> Groups { get; set; }
    }
}
