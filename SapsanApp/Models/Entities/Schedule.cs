using System;

namespace SapsanApp.Models.Entities
{
    public class Schedule
    {
        public int Id { get; set; }
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Color { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int GroupId { get; set; }
        public Group Group { get; set; }
        // Пользователь, который создал в расписании
        public Guid UserId { get; set; }
        public int CenterId { get; set; }
        public Center Center { get; set; }
        public DateTime Created { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
    }
}