using System;

namespace SapsanApp.Models.Entities
{
    public class Subject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; } = 0;
        public int LessonsCount { get; set; } = 0;
        public DateTime Created { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
    }
}