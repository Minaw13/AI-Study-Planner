using AIStudyPlanner.Data;
using AIStudyPlanner.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
namespace AIStudyPlanner.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public IndexModel(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public List<StudyTask> PendingTasks { get; set; } = new();
        // Both names are kept because the Razor page may use either one.
        public bool HasSavedStudyPlan { get; set; }
        public bool HasStudyPlan { get; set; }
        public async Task OnGetAsync()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return;
            }
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return;
            }
            // Load unfinished tasks
            PendingTasks = await _context.StudyTasks
                .Where(t =>
                    t.UserId == currentUserId &&
                    !t.IsCompleted)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
            // Check whether this user already has an AI Study Plan
            var hasPlan = await _context.AIStudyPlans
                .AnyAsync(p => p.UserId == currentUserId);
            HasSavedStudyPlan = hasPlan;
            HasStudyPlan = hasPlan;
        }
    }
}