namespace SapsanApp.Models.Entities
{
    public class SubCriterion
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int MainCriterionId { get; set; }
        public MainCriterion MainCriterion { get; set; }
        public double Max { get; set; } = 0;
    }
}
