using AIStudyPlanner.Data;
using AIStudyPlanner.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AIStudyPlanner.Pages
{
    [Authorize]
    public class TasksModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TasksModel(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<StudyTask> Tasks { get; set; } = new();

        public async Task OnGetAsync()
        {
            var currentUserId = _userManager.GetUserId(User);

            if (currentUserId == null)
            {
                return;
            }

            Tasks = await _context.StudyTasks
                .Where(t => t.UserId == currentUserId && !t.IsCompleted)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }
    }
}