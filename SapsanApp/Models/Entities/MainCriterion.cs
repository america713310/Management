namespace SapsanApp.Models.Entities
{
    public class MainCriterion
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TrainingProgramId { get; set; }
        public TrainingProgram TrainingProgram { get; set; }
    }
}