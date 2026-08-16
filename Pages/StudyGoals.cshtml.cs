using AIStudyPlanner.Data;
using AIStudyPlanner.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AIStudyPlanner.Pages
{
    [Authorize]
    public class StudyGoalsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public StudyGoalsModel(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<StudyGoal> Goals { get; set; } = new();

        public async Task OnGetAsync()
        {
            var currentUserId = _userManager.GetUserId(User);

            if (currentUserId == null)
            {
                return;
            }

            Goals = await _context.StudyGoals
                .Where(g => g.UserId == currentUserId)
                .OrderBy(g => g.TargetDate)
                .ToListAsync();
        }
    }
}