using System.ComponentModel.DataAnnotations;

namespace AIStudyPlanner.Models
{
    public class Subject
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = "";

        public string Description { get; set; } = "";

        public int WeeklyHours { get; set; }

        // The user who owns this subject
        [Required]
        public string UserId { get; set; } = "";
    }
}