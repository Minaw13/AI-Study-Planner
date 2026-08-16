using AIStudyPlanner.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AIStudyPlanner.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Subject> Subjects { get; set; } = null!;

        public DbSet<StudyGoal> StudyGoals { get; set; } = null!;

        public DbSet<StudyTask> StudyTasks { get; set; } = null!;

        public DbSet<StudyAvailability> StudyAvailabilities { get; set; } = null!;

        public DbSet<AIStudyPlan> AIStudyPlans { get; set; } = null!;

        public DbSet<EmailSubscriber> EmailSubscribers { get; set; } = null!;
    }
}