using SapsanApp.Enums;
using System;

namespace SapsanApp.Models.Entities
{
    public class StudentGroup
    {
        public Guid StudentId { get; set; }
        public Student Student { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; }
        public int? DiscountId { get; set; }
        public Discount Discount { get; set; }
        public StudentStatusEnum StudentStatusEnum { get; set; } = StudentStatusEnum.Studying;
    }
}