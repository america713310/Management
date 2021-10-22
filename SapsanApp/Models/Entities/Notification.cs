using System;

namespace SapsanApp.Models.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int RoleId { get; set; }
        public int CenterId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool HasRead { get; set; } = false;
        public DateTime Created { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
    }
}
