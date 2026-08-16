using AIStudyPlanner.Data;
using AIStudyPlanner.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AIStudyPlanner.Pages
{
    [Authorize]
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DashboardModel(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public int SubjectCount { get; set; }

        public int GoalCount { get; set; }

        public int TaskCount { get; set; }

        public int CompletedTaskCount { get; set; }

        public double WeeklyStudyHours { get; set; }

        public List<StudyTask> Tasks { get; set; } = new();

        public List<StudyGoal> Goals { get; set; } = new();

        public List<Subject> Subjects { get; set; } = new();

        // DAILY STUDY HOURS
        public Dictionary<DayOfWeek, double> StudyHours { get; set; } = new();


        // =====================================================
        // LOAD DASHBOARD
        // =====================================================

        public async Task OnGetAsync()
        {
            var currentUserId = _userManager.GetUserId(User);

            if (currentUserId == null)
            {
                return;
            }

            // =========================
            // SUBJECTS COUNT
            // =========================

            SubjectCount = await _context.Subjects
                .CountAsync(s => s.UserId == currentUserId);


            // =========================
            // GOALS COUNT
            // =========================

            GoalCount = await _context.StudyGoals
                .CountAsync(g => g.UserId == currentUserId);


            // =========================
            // TASKS COUNT
            // =========================

            TaskCount = await _context.StudyTasks
                .CountAsync(t => t.UserId == currentUserId);


            // =========================
            // COMPLETED TASKS
            // =========================

            CompletedTaskCount = await _context.StudyTasks
                .CountAsync(t =>
                    t.UserId == currentUserId &&
                    t.IsCompleted);


            // =========================
            // STUDY AVAILABILITY
            // =========================

            var availability = await _context.StudyAvailabilities
                .Where(a => a.UserId == currentUserId)
                .ToListAsync();


            WeeklyStudyHours = availability
                .Sum(a => a.AvailableHours);


            StudyHours = availability
                .GroupBy(a => a.DayOfWeek)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(a => a.AvailableHours)
                );


            // Make sure every day exists
            foreach (DayOfWeek day in Enum.GetValues<DayOfWeek>())
            {
                if (!StudyHours.ContainsKey(day))
                {
                    StudyHours[day] = 0;
                }
            }


            // =========================
            // ALL USER TASKS
            // =========================

            Tasks = await _context.StudyTasks
                .Where(t => t.UserId == currentUserId)
                .OrderBy(t => t.IsCompleted)
                .ThenBy(t => t.DueDate)
                .ToListAsync();


            // =========================
            // ALL USER GOALS
            // =========================

            Goals = await _context.StudyGoals
                .Where(g => g.UserId == currentUserId)
                .OrderBy(g => g.TargetDate)
                .ToListAsync();


            // =========================
            // ALL USER SUBJECTS
            // =========================

            Subjects = await _context.Subjects
                .Where(s => s.UserId == currentUserId)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }


        // =====================================================
        // COMPLETE GOAL
        // =====================================================

        public async Task<IActionResult> OnPostCompleteGoalAsync(int id)
        {
            var currentUserId = _userManager.GetUserId(User);

            if (currentUserId == null)
            {
                return RedirectToPage("/Account/Login");
            }


            var goal = await _context.StudyGoals
                .FirstOrDefaultAsync(g =>
                    g.Id == id &&
                    g.UserId == currentUserId);


            if (goal == null)
            {
                return RedirectToPage();
            }


            goal.IsCompleted = true;

            await _context.SaveChangesAsync();


            TempData["DashboardMessage"] =
                "🎯 Goal marked as complete!";


            return RedirectToPage();
        }

        // =====================================================
        // COMPLETE TASK
        // =====================================================

        public async Task<IActionResult> OnPostCompleteTaskAsync(int id)
        {
            var currentUserId = _userManager.GetUserId(User);

            if (currentUserId == null)
            {
                return RedirectToPage("/Account/Login");
            }


            var task = await _context.StudyTasks
                .FirstOrDefaultAsync(t =>
                    t.Id == id &&
                    t.UserId == currentUserId);


            if (task == null)
            {
                return RedirectToPage();
            }


            task.IsCompleted = true;

            await _context.SaveChangesAsync();


            TempData["DashboardMessage"] =
                "💌 Task marked as complete!";


            return RedirectToPage();
        }
    }
}