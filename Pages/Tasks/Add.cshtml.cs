using AIStudyPlanner.Data;
using AIStudyPlanner.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AIStudyPlanner.Pages.Tasks
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
        public StudyTask Task { get; set; } = new StudyTask();

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

            Task.UserId = currentUserId;

            _context.StudyTasks.Add(Task);

            await _context.SaveChangesAsync();

            return RedirectToPage("/Dashboard");
        }
    }
}