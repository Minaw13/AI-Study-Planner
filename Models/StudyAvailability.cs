using System.ComponentModel.DataAnnotations;

namespace AIStudyPlanner.Models
{
    public class StudyAvailability
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = "";

        [Required]
        public DayOfWeek DayOfWeek { get; set; }

        [Range(0, 24)]
        public double AvailableHours { get; set; }
        
    }
}