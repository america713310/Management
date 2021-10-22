using System;

namespace SapsanApp.Models.Entities
{
    public class CashboxRetail
    {
        public int Id { get; set; }
        public int CenterId { get; set; }
        public Center Center { get; set; }
        public string PaymentType { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public double? PaymentSum { get; set; } = 0;
        public DateTime Created { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
    }
}
