using SapsanApp.Enums;
using System;

namespace SapsanApp.Models.Entities
{
    public class Visit
    {
        public int Id { get; set; }
        public Guid StudentId { get; set; }
        public int GroupId { get; set; }
        public DateTime VisitDate { get; set; }
        public VisitEnum? VisitValue { get; set; }
    }
}