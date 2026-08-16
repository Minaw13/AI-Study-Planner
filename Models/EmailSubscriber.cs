using System.ComponentModel.DataAnnotations;

namespace AIStudyPlanner.Models
{
    public class EmailSubscriber
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}