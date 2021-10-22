using System;

namespace SapsanApp.Models.Entities
{
    public class ReferenceBook
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ReferenceBookFile { get; set; } = string.Empty;
        public DateTime Created { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
    }
}
