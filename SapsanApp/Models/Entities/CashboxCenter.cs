using System;

namespace SapsanApp.Models.Entities
{
    public class CashboxCenter
    {
        public int Id { get; set; }
        public int CenterId { get; set; }
        public Center Center { get; set; }
        // Аренда помещений
        public double? Rent { get; set; } = 0;
        // Коммунальные услуги
        public double? Utilities { get; set; } = 0;
        // Связь, интернет и т. д.
        public double? Communication { get; set; } = 0;
        // Маркетинг
        public double? Marketing { get; set; } = 0;
        // Фонд оплаты труда
        public double? WageFund { get; set; } = 0;
        // Обучение тренеров
        public double? CoachEducation { get; set; } = 0;
        // Транспорт
        public double? Transport { get; set; } = 0;
        // Канц. товары, инвентарь
        public double? Stock { get; set; } = 0;
        // Налоги
        public double? Taxes { get; set; } = 0;
        // Другое(тип расхода)
        public string AnotherType { get; set; } = string.Empty;
        // Другое(сумма расхода)
        public double? AnotherSum { get; set; } = 0;
        public DateTime Created { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
    }
}
