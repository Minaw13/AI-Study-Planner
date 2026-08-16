using AIStudyPlanner.Data;
using AIStudyPlanner.Models;
using AIStudyPlanner.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AIStudyPlanner.Pages.AIPlanner
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly StudyPlannerAIService _studyPlannerAIService;

        public IndexModel(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            StudyPlannerAIService studyPlannerAIService)
        {
            _context = context;
            _userManager = userManager;
            _studyPlannerAIService = studyPlannerAIService;
        }

        public int SubjectCount { get; set; }
        public int GoalCount { get; set; }
        public int TaskCount { get; set; }
        public double WeeklyStudyHours { get; set; }

        public string? GeneratedPlan { get; set; }

        public JsonDocument? PlanDocument { get; set; }

        public async Task OnGetAsync()
        {
            await LoadSummaryAsync();

            var userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return;
            }

            // Load the latest saved AI study plan for this user
            var latestPlan = await _context.AIStudyPlans
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .FirstOrDefaultAsync();

            if (latestPlan != null &&
                !string.IsNullOrWhiteSpace(latestPlan.PlanContent))
            {
                GeneratedPlan = latestPlan.PlanContent;

                try
                {
                    PlanDocument = JsonDocument.Parse(GeneratedPlan);
                }
                catch
                {
                    PlanDocument = null;
                }
            }
        }

        public async Task<IActionResult> OnPostGenerateAsync()
        {
            var userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return Challenge();
            }

            var subjects = await _context.Subjects
                .Where(s => s.UserId == userId)
                .ToListAsync();

            var goals = await _context.StudyGoals
                .Where(g => g.UserId == userId)
                .ToListAsync();

            var tasks = await _context.StudyTasks
                .Where(t => t.UserId == userId)
                .OrderBy(t => t.DueDate)
                .ToListAsync();

            var availability = await _context.StudyAvailabilities
                .Where(a => a.UserId == userId)
                .OrderBy(a => a.DayOfWeek)
                .ToListAsync();

            var plannerData = new
            {
                subjects = subjects.Select(s => new
                {
                    name = s.Name,
                    description = s.Description,
                    weeklyHours = s.WeeklyHours
                }),

                goals = goals.Select(g => new
                {
                    title = g.Title,
                    description = g.Description,
                    targetDate = g.TargetDate.ToString("yyyy-MM-dd"),
                    targetGrade = g.TargetGrade
                }),

                tasks = tasks.Select(t => new
                {
                    title = t.Title,
                    description = t.Description,
                    dueDate = t.DueDate.ToString("yyyy-MM-dd"),
                    isCompleted = t.IsCompleted,
                    subjectId = t.SubjectId
                }),

                availability = availability.Select(a => new
                {
                    dayOfWeek = a.DayOfWeek.ToString(),
                    availableHours = a.AvailableHours
                })
            };

            GeneratedPlan =
                await _studyPlannerAIService.GenerateStudyPlanAsync(
                    plannerData);

            try
            {
                PlanDocument = JsonDocument.Parse(GeneratedPlan);
            }
            catch
            {
                PlanDocument = null;
            }

            // Save the generated plan to the database
            var savedPlan = new AIStudyPlan
            {
                UserId = userId,
                PlanTitle = "AI Personalized Study Plan",
                PlanContent = GeneratedPlan,
                CreatedAt = DateTime.UtcNow
            };

            _context.AIStudyPlans.Add(savedPlan);

            await _context.SaveChangesAsync();

            await LoadSummaryAsync();

            return Page();
        }

        private async Task LoadSummaryAsync()
        {
            var userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return;
            }

            SubjectCount = await _context.Subjects
                .CountAsync(s => s.UserId == userId);

            GoalCount = await _context.StudyGoals
                .CountAsync(g => g.UserId == userId);

            TaskCount = await _context.StudyTasks
                .CountAsync(t => t.UserId == userId);

            WeeklyStudyHours = await _context.StudyAvailabilities
                .Where(a => a.UserId == userId)
                .SumAsync(a => a.AvailableHours);
        }
    }
}