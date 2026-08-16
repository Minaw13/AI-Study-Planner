using System.ComponentModel.DataAnnotations;

namespace AIStudyPlanner.Models
{
    public class AIStudyPlan
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = "";

        [Required]
        public string PlanTitle { get; set; } = "";

        public string PlanContent { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}