using AIStudyPlanner.Data;
using AIStudyPlanner.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AIStudyPlanner.Pages.StudyGoals
{
    [Authorize]
    public class AddModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AddModel(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public StudyGoal Goal { get; set; } = new StudyGoal();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var currentUserId = _userManager.GetUserId(User);

            if (currentUserId == null)
            {
                return Challenge();
            }

            Goal.UserId = currentUserId;

            _context.StudyGoals.Add(Goal);

            await _context.SaveChangesAsync();

            TempData["DashboardMessage"] =
                "🎯 Goal added successfully!";

            return RedirectToPage("/Dashboard");
        }
    }
}