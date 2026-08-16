using System.ComponentModel.DataAnnotations;

namespace AIStudyPlanner.Models
{
    public class StudyTask
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = "";

        public string? Description { get; set; }

        public DateTime DueDate { get; set; }

        public bool IsCompleted { get; set; }

        public int? SubjectId { get; set; }

        public string UserId { get; set; } = "";
    }
}