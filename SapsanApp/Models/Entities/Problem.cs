using System;

namespace SapsanApp.Models.Entities
{
    // Апи задач
    public class Problem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string Director { get; set; } = string.Empty;
        public string Responsible { get; set; } = string.Empty;
        public DateTime Created { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
    }
}
