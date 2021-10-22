using SapsanApp.Enums;

namespace SapsanApp.Models.Entities
{
    public class Retail
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int CountryId { get; set; }
        public Country Country { get; set; }
        public double Price { get; set; } = 0;
        public CurrencyEnum Currency { get; set; } = CurrencyEnum.KZT;
    }
}
