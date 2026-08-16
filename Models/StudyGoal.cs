using System.ComponentModel.DataAnnotations;

namespace AIStudyPlanner.Models
{
    public class StudyGoal
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = "";

        public string Description { get; set; } = "";

        [DataType(DataType.Date)]
        public DateTime TargetDate { get; set; }

        public string TargetGrade { get; set; } = "";

        [Required]
        public string UserId { get; set; } = "";

        // Goal completion status
        public bool IsCompleted { get; set; } = false;
    }
}